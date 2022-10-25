using System.IO;
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
			DamageCounterSystem.ResetVariables();
		}

		public override void Unload() => ETUDHotkey = null;

		public override void PostSetupContent() => CalamityMod = ModLoader.TryGetMod("CalamityMod", out var mod) ? mod : null;	

		// DAMAGE COUNTER

		internal static int[] DPSValues;
		internal static int[] DeathValues;
		internal static int[] TakenDamageValues;
		internal static int[] DealtDamageValues;

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			byte id = reader.ReadByte();
			switch (id)
			{
				case 0:
					int dps = reader.ReadInt32();
					DPSValues[whoAmI] = dps;
					break;
				case 1:
					byte count = reader.ReadByte();

					for (int i = 0; i < 256; i++) DPSValues[i] = -1;

					for (int i = 0; i < count; i++)
					{
						byte playerIndex = reader.ReadByte();
						int playerdps = reader.ReadInt32();
						DPSValues[playerIndex] = playerdps;
					}

					break;
				default:
					ETUDAdditionalOptions.CreateErrorMessage(Name, new System.NotImplementedException("Invalid Packet ID"), id);
					break;
			}
		}
	}
}