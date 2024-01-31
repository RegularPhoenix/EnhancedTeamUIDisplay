using System.Collections.Generic;
using System.Linq;
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

		private struct PlayerStatIncreases {
			internal readonly byte Index;
			internal readonly int Dps;
			internal readonly int DealtDamage;
			internal readonly int TakenDamage;
			internal readonly int Deaths;

			internal PlayerStatIncreases(byte index, int dps, int dealtDamage, int takenDamage, int deaths) {
				Index = index;
				Dps = dps;
				DealtDamage = dealtDamage;
				TakenDamage = takenDamage;
				Deaths = deaths;
			}
		}

		public override void PostUpdateWorld() {
			if (Main.netMode is NetmodeID.SinglePlayer)
				return;

			List<PlayerStatIncreases> data
				= Main.player
					.Where(p => p.active)
					.Select((p, i) => new PlayerStatIncreases(
						(byte) i,
						p.accDreamCatcher ? DPSTable[i] : -1,
						DealtDamageIncreaseTable[i],
						TakenDamageIncreaseTable[i],
						DeathsIncreaseTable[i]
					)).ToList();

			ModPacket netMessage = Mod.GetPacket();
			netMessage.Write((byte) DamageMeterPacketType.InformClientsOfValues);

			netMessage.Write((byte) data.Count);

			foreach (PlayerStatIncreases inc in data) {
				netMessage.Write(inc.Index);
				netMessage.Write(inc.Dps);
				netMessage.Write(inc.DealtDamage);
				netMessage.Write(inc.TakenDamage);
				netMessage.Write(inc.Deaths);
			}

			netMessage.Send();

			ResetIncreases();
		}
	}
}
