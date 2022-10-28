using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using EnhancedTeamUIDisplay.DamageCounter;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDUISystem : ModSystem
	{
		internal static UserInterface ETUDInterface, ETUDAllyStatScreen;
		private GameTime LastUpdateUIGameTime;
		private bool AnyBossFound; // Can be replaced with Main.CurrentFrameFlags.AnyActiveBossNPC ?
		private static bool BossEvaded;

		private string FirstBossName = "";
		private List<string> BossNames = new(), UnkilledBossNames = new();

		public override void OnModLoad()
		{
			if (!Main.dedServ) ETUDInterface = new UserInterface(); ETUDAllyStatScreen = new UserInterface();
			AnyBossFound = false;
		}

		public override void Unload()
		{
			ETUDInterface = null;
			ETUDAllyStatScreen = null;
		}

		internal static void OpenAllyStatScreen()
		{
			
			ETUDAllyInfoPanel allyInfoPanel = new();
			UIState state = new();
			state.Append(allyInfoPanel);
			ETUDAllyStatScreen.SetState(state);
		}

		internal static void CloseAllyStatScreen()
		{
			ETUDAllyStatScreen.SetState(null);
		}

		internal static void ToggleETUD()
		{
			if (ETUDInterface.CurrentState != null) CloseETUDInterface(); else OpenETUDInterface();
		}

		internal static void OpenETUDInterface()
		{			
			ETUDPanel1 newui1 = new();
			ETUDPanel2 newui2 = new();
			ETUDPanel3 newui3 = new();

			AllyInfoButton1 statbutton1 = new();
			AllyInfoButton2 statbutton2 = new();
			AllyInfoButton3 statbutton3 = new();

			BuffCheckButton button1 = new();

			UIState state = new();

			if (ETUDConfig.Instanse.EnableDamageCounter) { DamageCounterUI damageCounterUI = new(); state.Append(damageCounterUI); }

			state.Append(newui1);
			if (ETUDConfig.Instanse.PanelAmount == "Two panels" || ETUDConfig.Instanse.PanelAmount == "Three panels") state.Append(newui2);
			if (ETUDConfig.Instanse.PanelAmount == "Three panels") state.Append(newui3);

			state.Append(statbutton1);
			state.Append(statbutton2);
			state.Append(statbutton3);

			if (ETUDConfig.Instanse.ShowBuffCheckButton) state.Append(button1);
			
			ETUDInterface.SetState(state);
		}

		internal static void CloseETUDInterface()
		{
			ETUDInterface.SetState(null);
		}

		public override void PreSaveAndQuit()
		{
			if (ETUDInterface.CurrentState != null) ETUDInterface.SetState(null);
			if (ETUDAllyStatScreen.CurrentState != null) ETUDAllyStatScreen.SetState(null);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (ETUDInterface?.CurrentState?.GetElementAt(Main.MouseScreen) is not null and not UIState) Main.LocalPlayer.mouseInterface = true;

			if (!AnyBossFound)
			{
				BossNames.Clear();
				UnkilledBossNames.Clear();
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i] != null && Main.npc[i].active && Main.npc[i].boss)
					{
						if (ETUDConfig.Instanse.EnableAutoToggle && ETUDInterface.CurrentState == null) OpenETUDInterface();
						if (ETUDConfig.Instanse.AutoResetDamageCounter) DamageCounterSystem.ResetVariables();
						if (ETUDConfig.Instanse.ShowBossSummary) ETUDAdditionalOptions.StartBossSummary();
						AnyBossFound = true;
						BossEvaded = false;
						FirstBossName = Main.npc[i].FullName;
					}
				}
			}

			if (AnyBossFound)
			{
				bool FoundBoss = false;
				bool PlayersWiped = true;

				for (int j = 0; j < Main.maxPlayers; j++) if (Main.player[j] != null && Main.player[j].active && !Main.player[j].dead) { PlayersWiped = false; break; }

				for (int j = 0; j < Main.maxNPCs; j++)
				{
					if (Main.npc[j] != null && Main.npc[j].boss && Main.npc[j].active)
					{
						FoundBoss = true;
						if (!BossNames.Contains(Main.npc[j].FullName)) BossNames.Add(Main.npc[j].FullName);

						if (PlayersWiped && !UnkilledBossNames.Contains(Main.npc[j].FullName)) UnkilledBossNames.Add(Main.npc[j].FullName);
					}

					if (Main.npc[j] != null && Main.npc[j].boss && !Main.npc[j].active)
					{
						if (!PlayersWiped && Main.npc[j].life > 0) if (!UnkilledBossNames.Contains(Main.npc[j].FullName)) UnkilledBossNames.Add(Main.npc[j].FullName);
						if (Main.npc[j].FullName == FirstBossName && Main.npc[j].life > 0) BossEvaded = true;
					}
				}

				if (!FoundBoss)
				{
					if (ETUDConfig.Instanse.EnableAutoToggle) CloseETUDInterface();

					if (ETUDConfig.Instanse.ShowBossSummary)
					{
						List<string> KilledBosses = new();
						foreach (string boss in BossNames) if (!UnkilledBossNames.Contains(boss)) KilledBosses.Add(boss);

						Dictionary<string, int[]> tempDictionary = Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts ?? new();
						foreach (string boss in KilledBosses) if (tempDictionary.ContainsKey(boss)) tempDictionary[boss][0]++; else tempDictionary.Add(boss, new int[] { 1, 0 });
						foreach (string boss in UnkilledBossNames) if (tempDictionary.ContainsKey(boss)) tempDictionary[boss][1]++; else tempDictionary.Add(boss, new int[] { 0, 1 });
						Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts = tempDictionary;

						bool playeralive = false;
						for (int i = 0; i < Main.maxPlayers; i++)
						{
							if (Main.player[i].team == Main.LocalPlayer.team && Main.player[i].active && !Main.player[i].dead) playeralive = true;
						}

						if (playeralive && !BossEvaded)
						{
							ETUDAdditionalOptions.EndBossSummary(FirstBossName + (KilledBosses.Count > 1 ? (" and " + (KilledBosses.Count - 1) + " other bosses") : ""), "> You have killed this boss " + tempDictionary[FirstBossName][0] + " time(s).");
						}
						else if (playeralive && BossEvaded && KilledBosses.Count > 0) ETUDAdditionalOptions.EndBossSummary("First boss has escaped, but you killed " + KilledBosses.Count + " other bosses. ", "> You have wiped on this boss (" + FirstBossName + ") " + tempDictionary[FirstBossName][1] + " time(s).", true);
						else ETUDAdditionalOptions.EndBossSummary("", "> You have wiped on this boss (" + FirstBossName + ") " + tempDictionary[FirstBossName][1] + " time(s).");	
					}
					AnyBossFound = false;
					FirstBossName = "";
				}
			}
			
			LastUpdateUIGameTime = gameTime;
			if (ETUDInterface?.CurrentState != null) ETUDInterface.Update(gameTime);
			if (ETUDAllyStatScreen?.CurrentState != null) ETUDAllyStatScreen.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (index != -1)
			{
				layers.Insert(++index, new LegacyGameInterfaceLayer
					(
					"ETUD: ETUDUI",
					delegate
					{
						if (LastUpdateUIGameTime != null && ETUDInterface?.CurrentState != null)
						{
							ETUDInterface.Draw(Main.spriteBatch, LastUpdateUIGameTime);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);

				layers.Insert(++index, new LegacyGameInterfaceLayer
					(
					"ETUD: ETUDAllyStatScreen",
					delegate
					{
						if (LastUpdateUIGameTime != null && ETUDAllyStatScreen?.CurrentState != null)
						{
							ETUDAllyStatScreen.Draw(Main.spriteBatch, LastUpdateUIGameTime);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}

		}
	}
}