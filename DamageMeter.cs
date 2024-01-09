using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EnhancedTeamUIDisplay
{
	internal class DamageMeter : ModSystem
	{
		// We don't keep totals on server
		// instead send increases in values to clients
		// this allows clients to reset table whenever thay want
		// and to not depend on the server values
		internal static int[] DPSTable, DeathsIncreaseTable, TakenDamageIncreaseTable, DealtDamageIncreaseTable;

		private static void ResetIncreases() {
			for (int i = 0; i < 256; i++) {
				DeathsIncreaseTable[i]
					= TakenDamageIncreaseTable[i]
					= DealtDamageIncreaseTable[i]
					= 0;
			}
		}

		public override void Load() {
			DPSTable = new int[256];
			DeathsIncreaseTable = new int[256];
			TakenDamageIncreaseTable = new int[256];
			DealtDamageIncreaseTable = new int[256];

			ResetIncreases();
		}

		internal enum DamageMeterPacketType : byte
		{
			InformClientsOfValues,
			InformServerOfDPS,
			InformServerOfDealtDamage,
			InformServerOfTakenDamage,
			InformServerOfDeaths,
		}

		public override void PostUpdateWorld() {
			if (Main.netMode is NetmodeID.SinglePlayer)
				return;

			ModPacket netMessage = Mod.GetPacket();
			netMessage.Write((byte) DamageMeterPacketType.InformClientsOfValues);

			byte count = (byte) Main.CurrentFrameFlags.ActivePlayersCount;
			netMessage.Write(count);

			for (int i = 0; i < 256; i++) {
				Player p = Main.player[i];
				if (p.active) {
					netMessage.Write((byte) i);
					netMessage.Write(p.accDreamCatcher ? DPSTable[i] : -1);
					netMessage.Write(DealtDamageIncreaseTable[i]);
					netMessage.Write(TakenDamageIncreaseTable[i]);
					netMessage.Write(DeathsIncreaseTable[i]);
				}
			}

			ResetIncreases();

			netMessage.Send();
		}
	}
}
