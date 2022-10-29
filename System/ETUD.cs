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

			/*DPSValues = new int[256];
			DeathValues = new int[256];
			TakenDamageValues = new int[256];
			DealtDamageValues = new int[256];

			for (int i = 0; i < 256; i++)
			{
				DPSValues[i] = -1;
				DeathValues[i] = -1;
				TakenDamageValues[i] = -1;
				DealtDamageValues[i] = -1;
			}*/
		}

		/*internal void ResetVariables(int? clientNum = null) // TODO: It should reset values only on client not on server
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
			Logger.Info(Main.player[(int)clientNum].name);
			netMessage.Send(clientNum ?? -1);			
		}*/

		public override void Unload() => ETUDHotkey = null;

		public override void PostSetupContent() => CalamityMod = ModLoader.TryGetMod("CalamityMod", out var mod) ? mod : null;	

		// DAMAGE COUNTER

		/*internal static int[] DPSValues, DeathValues, TakenDamageValues, DealtDamageValues;

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			byte id = reader.ReadByte();
			switch (id)
			{
				case 0:
					byte count = reader.ReadByte();

					for (int i = 0; i < 256; i++)
					{
						DPSValues[i] = -1;
						DealtDamageValues[i] = -1;
					}

					for (int i = 0; i < count; i++)
					{
						byte playerIndex = reader.ReadByte();
						int playerdps = reader.ReadInt32();
						int dealtdamage = reader.ReadInt32();
						int takendamage = reader.ReadInt32();
						int deaths = reader.ReadInt32();
						DPSValues[playerIndex] = playerdps;
						DealtDamageValues[playerIndex] = dealtdamage;
						TakenDamageValues[playerIndex] = takendamage;
						DeathValues[playerIndex] = deaths;
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
					DeathValues[whoAmI]++;
					break;
				default:
					ETUDAdditionalOptions.CreateErrorMessage(Name, new System.NotImplementedException("Invalid Packet ID"), id);
					break;
			}
		}*/
	}
}