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
		private bool _anyBossActive = false;

		private GameTime _lastUpdateUIGameTime;

		internal static UserInterface MainInterface, AllyInfoInterface;

		internal static List<MainPanel> MainPanels = new();

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
			MainPanels.Add(firstPanel);
			state.Append(firstPanel);
			state.Append(firstButton);

			if (Config.Instanse.IsEquipmentCheckButtonEnabled) {
				EquipmentCheckButton button = new();
				state.Append(button);
			}

			if (Config.Instanse.IsDamageMeterEnabled) {
				DamageMeterPanel panel = new();
				state.Append(panel);
			}

			for (int i = 1; i < Config.Instanse.MaxPanelAmount; i++) {
				MainPanel additionalPanel = new(i);
				AllyInfoButton additionalButton = new(i);
				MainPanels.Add(additionalPanel);
				state.Append(additionalPanel);
				state.Append(additionalButton);
			}

			MainInterface.SetState(state);
		}

		internal static void CloseMainInterface() {
			MainInterface.SetState(null);
			MainPanels.Clear();
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
				foreach (MainPanel panel in MainPanels) {
					if (panel.Ally is not null) {
						if (panel.Ally.team != Main.LocalPlayer.team || (!panel.Ally.active && !Config.Instanse.AreOfflinePlayersDisplayed)) {
							panel.Ally = null;
						}

						if (!(panel.Ally?.active ?? true) && Main.player.First(p => p.name == panel.Ally.name) is Player p) {
							panel.Ally = p;
						}
					}

					// Create a list with all tracked players
					List<Player> trackedAllies = new();
					foreach (MainPanel temp in MainPanels)
						trackedAllies.Add(temp.Ally);

					// Find new player
					panel.Ally ??= Main.player.FirstOrDefault(
						player =>
							player is not null
							&& player != Main.LocalPlayer
							&& player.team == Main.LocalPlayer.team
							&& (player.active || Config.Instanse.AreOfflinePlayersDisplayed)
							// Exclude tracked players from search to avoid duplicating
							&& !trackedAllies.Contains(player),
						null
					);
				}

				for (int i = MainPanels.Count - 1; i > 0; i--) {
					if (MainPanels[i].Ally is not null && MainPanels[i - 1].Ally is null) {
						MainPanels[i - 1].Ally = MainPanels[i].Ally;
						MainPanels[i].Ally = null;
					}
				}
			}

			if (Config.Instanse.IsAutoResetEnabled) {
				if (!_anyBossActive && Main.CurrentFrameFlags.AnyActiveBossNPC)
					Main.LocalPlayer.GetModPlayer<DamageMeterPlayer>().ResetTables();
			}

			_anyBossActive = Main.CurrentFrameFlags.AnyActiveBossNPC;

			if (Config.Instanse.IsAutoToggleEnabled) {
				if (_anyBossActive && MainInterface?.CurrentState is null)
					OpenMainInterface();
				else if (!_anyBossActive && MainInterface?.CurrentState is not null)
					CloseMainInterface();
			}

			_lastUpdateUIGameTime = gameTime;

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
						if (_lastUpdateUIGameTime is not null && MainInterface?.CurrentState is not null) {
							MainInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);
						}

						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(++index, new LegacyGameInterfaceLayer(
					"ETUD: Ally Info Interface",
					delegate {
						if (_lastUpdateUIGameTime is not null && AllyInfoInterface?.CurrentState is not null) {
							AllyInfoInterface.Draw(Main.spriteBatch, _lastUpdateUIGameTime);
						}

						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}