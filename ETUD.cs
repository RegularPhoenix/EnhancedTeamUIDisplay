using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace EnhancedTeamUIDisplay
{
	internal class ETUD : Mod
	{
		internal static ModKeybind UIHotkey;

		internal static ETUD Instance;

		public override void Load() {
			UIHotkey = KeybindLoader.RegisterKeybind(this, "ToggleUI", "L");
			Instance = this;
		}

		public override void Unload() {
			UIHotkey = null;
			Instance = null;
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) {
			if (Main.netMode is Terraria.ID.NetmodeID.SinglePlayer)
				return;

			byte id = reader.ReadByte();
			switch (id) {
				case 0: // InformClientsOfValues
					byte count = reader.ReadByte();

					Dictionary<int, int[]> increaseTables = [];

					for (int i = 0; i < count; i++) {
						byte playerIndex = reader.ReadByte();

						int playerDPS = reader.ReadInt32();
						int playerDealtDamage = reader.ReadInt32();
						int playerTakenDamage = reader.ReadInt32();
						int playerDeaths = reader.ReadInt32();

						increaseTables.Add(playerIndex, [playerDPS, playerDealtDamage, playerTakenDamage, playerDeaths]);
					}

					List<DamageMeterPlayer> receivers = Main.player.Where(
						player => player.active
					).Select(
						player => player.GetModPlayer<DamageMeterPlayer>()
					).ToList();

					foreach (DamageMeterPlayer player in receivers) {
						if (player.DPSTable is null)
							continue;

						foreach (KeyValuePair<int, int[]> entry in increaseTables) {
							int playerID = entry.Key;

							player.DPSTable[playerID] = entry.Value[0];

							if (player.DealtDamageTable[playerID] == -1 && entry.Value[1] != 0)
								player.DealtDamageTable[playerID]++;

							player.DealtDamageTable[playerID] += entry.Value[1];

							if (player.TakenDamageTable[playerID] == -1 && entry.Value[2] != 0)
								player.TakenDamageTable[playerID]++;

							player.TakenDamageTable[playerID] += entry.Value[2];

							if (player.DeathsTable[playerID] == -1 && entry.Value[3] != 0)
								player.DeathsTable[playerID]++;

							player.DeathsTable[playerID] += entry.Value[3];
						}
					}

					break;

				case 1: // InformServerOfDPS
					DamageMeter.DPSTable[whoAmI] = reader.ReadInt32();
					break;

				case 2: // InformServerOfDealtDamage
					DamageMeter.DealtDamageIncreaseTable[whoAmI] += reader.ReadInt32();
					break;

				case 3: // InformServerOfTakenDamage
					DamageMeter.TakenDamageIncreaseTable[whoAmI] += reader.ReadInt32();
					break;

				case 4: // InformServerOfDeaths
					DamageMeter.DeathsIncreaseTable[whoAmI]++;
					break;

				default:
					Util.CreateErrorMessage(
						"ETUD.HandlePacket",
						new System.ArgumentOutOfRangeException($"Invalid Packet ID: {id}")
					);
					break;
			}
		}
	}
}
