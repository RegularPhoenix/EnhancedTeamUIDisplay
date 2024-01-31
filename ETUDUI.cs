using EnhancedTeamUIDisplay.UIElements;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDUI : ModSystem
	{
		private bool anyBossActive = false;

		private GameTime lastUpdateUIGameTime;
		internal static UserInterface MainInterface, AllyInfoInterface;

		internal static List<MainPanel> Panels = new();

		public override void OnModLoad() {
			if (!Main.dedServ) {
				MainInterface = new();
				AllyInfoInterface = new();
			}
		}

		public override void Unload()
			=> MainInterface = AllyInfoInterface = null;


		public override void PreSaveAndQuit() {
			if (MainInterface?.CurrentState is not null)
				MainInterface.SetState(null);

			if (AllyInfoInterface?.CurrentState is not null)
				AllyInfoInterface.SetState(null);
		}

		internal static void OpenMainInterface() {
			UIState state = new();

			MainPanel firstPanel = new(0);
			AllyInfoButton firstButton = new(0);
			Panels.Add(firstPanel);
			state.Append(firstPanel);
			state.Append(firstButton);

			if (Config.Instanse.ShowBuffCheckButton) {
				EquipmentCheckButton button = new();
				state.Append(button);
			}

			if (Config.Instanse.EnableDamageMeter) {
				DamageMeterPanel panel = new();
				state.Append(panel);
			}

			for (int i = 1; i < Config.Instanse.PanelAmount; i++) {
				MainPanel additionalPanel = new(i);
				AllyInfoButton additionalButton = new(i);
				Panels.Add(additionalPanel);
				state.Append(additionalPanel);
				state.Append(additionalButton);
			}

			MainInterface.SetState(state);
		}

		internal static void CloseMainInterface() {
			MainInterface.SetState(null);
			Panels.Clear();
		}

		internal static void ToggleMainInterface() {
			if (MainInterface.CurrentState is null)
				OpenMainInterface();
			else
				CloseMainInterface();
		}

		internal static void OpenAllyInfoInterface(int number, bool isExtended, float left, float top) {
			AllyInfoPanel allyInfoPanel = new(number, isExtended, left, top);
			UIState state = new();
			state.Append(allyInfoPanel);
			AllyInfoInterface.SetState(state);
		}

		internal static void CloseAllyInfoInterface() => AllyInfoInterface.SetState(null);

		public override void UpdateUI(GameTime gameTime) {
			if (MainInterface?.CurrentState?.GetElementAt(Main.MouseScreen) is not null and not UIState)
				Main.LocalPlayer.mouseInterface = true;

			if (Main.LocalPlayer.team != 0) {
				foreach (MainPanel panel in Panels) {
					if (panel.Ally is not null) {
						if (panel.Ally.team != Main.LocalPlayer.team || (!panel.Ally.active && !Config.Instanse.ShowOfflinePlayers)) {
							panel.Ally = null;
						}

						if (!panel.Ally.active && Main.player.First(p => p.name == panel.Ally.name) is Player p) {
							panel.Ally = p;
						}
					}

					// Create a list with all tracked players
					List<Player> trackedAllies = new();
					foreach (MainPanel temp in Panels)
						trackedAllies.Add(temp.Ally);

					// Find new player
					panel.Ally ??= Main.player.FirstOrDefault(
						player =>
							player is not null
							&& player != Main.LocalPlayer
							&& player.team == Main.LocalPlayer.team
							&& (player.active || Config.Instanse.ShowOfflinePlayers)
							// Exclude tracked players from search to avoid duplicating
							&& !trackedAllies.Contains(player),
						null
					);
				}

				for (int i = Panels.Count - 1; i > 0; i--) {
					if (Panels[i].Ally is not null && Panels[i - 1].Ally is null) {
						Panels[i - 1].Ally = Panels[i].Ally;
						Panels[i].Ally = null;
					}
				}
			}

			if (Config.Instanse.EnableAutoReset) {
				if (!anyBossActive && Main.CurrentFrameFlags.AnyActiveBossNPC)
					Main.LocalPlayer.GetModPlayer<DamageMeterPlayer>().ResetTables();
			}

			anyBossActive = Main.CurrentFrameFlags.AnyActiveBossNPC;

			if (Config.Instanse.EnableAutoToggle) {
				if (anyBossActive && MainInterface?.CurrentState is null)
					OpenMainInterface();
				else if (!anyBossActive && MainInterface?.CurrentState is not null)
					CloseMainInterface();
			}

			lastUpdateUIGameTime = gameTime;

			if (MainInterface?.CurrentState is not null)
				MainInterface.Update(gameTime);

			if (AllyInfoInterface?.CurrentState is not null)
				AllyInfoInterface.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (index != -1) {
				layers.Insert(++index, new LegacyGameInterfaceLayer(
					"ETUD: Main Interface",
					delegate {
						if (lastUpdateUIGameTime is not null && MainInterface?.CurrentState is not null) {
							MainInterface.Draw(Main.spriteBatch, lastUpdateUIGameTime);
						}

						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(++index, new LegacyGameInterfaceLayer(
					"ETUD: Ally Info Interface",
					delegate {
						if (lastUpdateUIGameTime is not null && AllyInfoInterface?.CurrentState is not null) {
							AllyInfoInterface.Draw(Main.spriteBatch, lastUpdateUIGameTime);
						}

						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}