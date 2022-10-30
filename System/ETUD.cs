using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using EnhancedTeamUIDisplay.DamageCounter;

namespace EnhancedTeamUIDisplay
{
	public class ETUD : Mod
	{
		internal static ETUD Instance;

		internal static ModKeybind ETUDHotkey;

		internal static Mod CalamityMod;

		public override void Load()
		{
			ETUDHotkey = KeybindLoader.RegisterKeybind(this, "Show ETUD", "L");	

			Instance = this;

			DPSValues = new int[256];
			DeathValues = new int[256];
			TakenDamageValues = new int[256];
			DealtDamageValues = new int[256];

			for (int i = 0; i < 256; i++)
			{
				DPSValues[i] = -1;
				DeathValues[i] = -1;
				TakenDamageValues[i] = -1;
				DealtDamageValues[i] = -1;
			}
		}

		internal void ResetVariables()
		{
			var netMessage = GetPacket();
			netMessage.Write((byte)DamageCounterSystem.DamageCounterPacketType.InformClientsOfValues);

			byte count = (byte)Main.player.Count(x => x.active);
			netMessage.Write(count);

			for (int i = 0; i < 256; i++)
			{
				Player p = Main.player[i];
				if (p.active)
				{
					netMessage.Write((byte)i);
					netMessage.Write(-1);
					netMessage.Write(-1);
					netMessage.Write(-1);
					netMessage.Write(-1);
				}
			}
			netMessage.Send();

			DamageCounterSystem.AwaitsReset = false;
		}

		public override void Unload()
		{
			ETUDHotkey = null;
			Instance = null;
			CalamityMod = null;
		}

		public override void PostSetupContent() => CalamityMod = ModLoader.TryGetMod("CalamityMod", out var mod) ? mod : null;	

		// DAMAGE COUNTER

		internal static int[] DPSValues, DeathValues, TakenDamageValues, DealtDamageValues;

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			byte id = reader.ReadByte();
			switch (id)
			{
				case 0:
					byte count = reader.ReadByte();

					for (int i = 0; i < 256; i++) DPSValues[i] = -1;

					for (int i = 0; i < count; i++)
					{
						byte playerIndex = reader.ReadByte();
						DPSValues[playerIndex] =  reader.ReadInt32();
						DealtDamageValues[playerIndex] = reader.ReadInt32();
						TakenDamageValues[playerIndex] = reader.ReadInt32();
						DeathValues[playerIndex] = reader.ReadInt32();
					}
					break;
				case 1:
					int dps = reader.ReadInt32();
					DPSValues[whoAmI] = dps;
					break;
				case 2:
					int dealtdmg = reader.ReadInt32();
					DealtDamageValues[whoAmI] += dealtdmg;
					break;
				case 3:
					int takendmg = reader.ReadInt32();
					TakenDamageValues[whoAmI] += takendmg;
					break;
				case 4:
					if(DeathValues[whoAmI] is not -1) DeathValues[whoAmI]++; else DeathValues[whoAmI] = 1;
					break;
				default:
					ETUDAdditionalOptions.CreateErrorMessage(Name, new System.NotImplementedException("Invalid Packet ID"), id);
					break;
			}
		}
	}
}