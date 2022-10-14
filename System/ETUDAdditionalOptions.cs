using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDAdditionalOptions
	{
		public static readonly Color ETUDTextColor = new(215, 195, 240);

		public static void CheckForBuffs()
		{
			if (Main.LocalPlayer.team == 0)
			{
				return;
			}

			Main.NewText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ETUDInfoBC"), ETUDTextColor);


			for (int i = -1; i < Main.maxPlayers; i++)
			{
				Player Ally = null;
				
				if (i == -1) Ally = Main.LocalPlayer;
				else if (Main.player[i] != null && Main.player[i].active && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer) Ally = Main.player[i];
				
				if(Ally != null)
				{
					Color textColor = Color.Yellow;
					string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);
					string output = "";
					string classname = "";

					switch (PlayerClass)
					{
						case "Melee":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassWarrior")}]";
							if (!(MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofCursedFlames)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofFire)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofGold)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofIchor)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofNanites)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofParty)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofPoison)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofVenom)
								|| Ally.HasBuff(BuffID.WeaponImbueCursedFlames)
								|| Ally.HasBuff(BuffID.WeaponImbueFire)
								|| Ally.HasBuff(BuffID.WeaponImbueGold)
								|| Ally.HasBuff(BuffID.WeaponImbueIchor)
								|| Ally.HasBuff(BuffID.WeaponImbueNanites)
								|| Ally.HasBuff(BuffID.WeaponImbueConfetti)
								|| Ally.HasBuff(BuffID.WeaponImbuePoison)
								|| Ally.HasBuff(BuffID.WeaponImbueVenom)
								))
							{
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.NoFlask")} ";
							}
							if (!Ally.HasBuff(BuffID.Sharpened)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.SharpBuff")} ";
							break;
						case "Ranged":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassRanger")}]";
							if (!(MiscEventHandler.HasItemInInventory(Ally, ItemID.AmmoReservationPotion)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.ArcheryPotion)
								|| Ally.HasBuff(BuffID.AmmoReservation)
								|| Ally.HasBuff(BuffID.Archery)
								))
							{
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.RangerBuff")} ";
							}
							if (!Ally.HasBuff(BuffID.AmmoBox)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.AmmoBoxBuff")} ";
							break;
						case "Magic":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassMage")}]";
							if (!(MiscEventHandler.HasItemInInventory(Ally, ItemID.ManaRegenerationPotion)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.MagicPowerPotion)
								|| Ally.HasBuff(BuffID.ManaRegeneration)
								|| Ally.HasBuff(BuffID.MagicPower)
								))
							{
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.MageBuff")} ";
							}
							if (!Ally.HasBuff(BuffID.Clairvoyance)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.CrystalBuff")} ";
							int ManaAmount = MiscEventHandler.CountItemsInInventory(Ally, ItemID.ManaPotion) +
							MiscEventHandler.CountItemsInInventory(Ally, ItemID.GreaterManaPotion) +
							MiscEventHandler.CountItemsInInventory(Ally, ItemID.LesserManaPotion) +
							MiscEventHandler.CountItemsInInventory(Ally, ItemID.SuperManaPotion) +
							MiscEventHandler.CountItemsInInventory(Ally, ItemID.RestorationPotion);

							if (ETUD.CalamityMod != null)
							{
								if (ETUD.CalamityMod.TryFind<ModItem>("SupremeManaPotion", out var SupremeManaPotion))
								{
									ManaAmount += MiscEventHandler.CountItemsInInventory(Ally, SupremeManaPotion.Type);
								}
							}

							if (ManaAmount == 0) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.OutOfManaPotions")} ";
							else if (ManaAmount <= 5) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.IsLowOnManaPotions")} ";
							break;
						case "Summon":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.Summoner")}]";
							if (!(MiscEventHandler.HasItemInInventory(Ally, ItemID.SummoningPotion)
								|| Ally.HasBuff(BuffID.Summoning)
								))
							{
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.SummonBuff")} ";
							}
							if (!Ally.HasBuff(BuffID.Bewitched)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.BewitchedBuff")} ";
							break;
						case "Rogue":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassRogue")}]";
							if (!(MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofCursedFlames)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofFire)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofGold)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofIchor)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofNanites)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofParty)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofPoison)
								|| MiscEventHandler.HasItemInInventory(Ally, ItemID.FlaskofVenom)
								|| Ally.HasBuff(BuffID.WeaponImbueCursedFlames)
								|| Ally.HasBuff(BuffID.WeaponImbueFire)
								|| Ally.HasBuff(BuffID.WeaponImbueGold)
								|| Ally.HasBuff(BuffID.WeaponImbueIchor)
								|| Ally.HasBuff(BuffID.WeaponImbueNanites)
								|| Ally.HasBuff(BuffID.WeaponImbueConfetti)
								|| Ally.HasBuff(BuffID.WeaponImbuePoison)
								|| Ally.HasBuff(BuffID.WeaponImbueVenom)
								))
							{
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.NoFlask")} ";
							}
							if (ETUD.CalamityMod != null)
							{
								if (ETUD.CalamityMod.TryFind<ModItem>("ShadowPotion", out var ShadowPotion) && ETUD.CalamityMod.TryFind<ModBuff>("ShadowBuff", out var ShadowBuff))
								{
									if (!(MiscEventHandler.HasItemInInventory(Ally, ItemID.InvisibilityPotion)
									|| MiscEventHandler.HasItemInInventory(Ally, ShadowPotion.Type)
									|| Ally.HasBuff(BuffID.Invisibility)
									|| Ally.HasBuff(ShadowBuff.Type)
									))
									{
										output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.RogueBuff")} ";
									}
								}
							}
							break;
						case "None":
							break;
						default:
							break;
					}

					int HealsAmount = MiscEventHandler.CountItemsInInventory(Ally, ItemID.HealingPotion) +
						MiscEventHandler.CountItemsInInventory(Ally, ItemID.GreaterHealingPotion) +
						MiscEventHandler.CountItemsInInventory(Ally, ItemID.LesserHealingPotion) +
						MiscEventHandler.CountItemsInInventory(Ally, ItemID.SuperHealingPotion) +
						MiscEventHandler.CountItemsInInventory(Ally, ItemID.RestorationPotion);

					if (ETUD.CalamityMod != null)
					{
						if (ETUD.CalamityMod.TryFind<ModItem>("SupremeHealingPotion", out var SupremeHealingPotion) && ETUD.CalamityMod.TryFind<ModItem>("OmegaHealingPotion", out var OmegaHealingPotion))
						{
							HealsAmount += MiscEventHandler.CountItemsInInventory(Ally, SupremeHealingPotion.Type) + MiscEventHandler.CountItemsInInventory(Ally, OmegaHealingPotion.Type);
						}
					}

					if (HealsAmount == 0) { output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.OutOfHPPotions")} "; textColor = Color.Red; }
					else if (HealsAmount <= 5) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.IsLowOnHPPotions")} ";

					if (output == "") { output = "- Has required buffs and potions"; textColor = Color.Green; }
					Main.NewText($"{Ally.name} {classname} {output}", textColor);
				}
			}
		}

		private static int[][] playersdps;
		public static int arraynum;
		private static bool started;
		private static DateTime FightStartTime;
		private static DateTime FightEndTime;
		private static TimeSpan FightDuration;

		public static void StartBossSummary()
		{
			playersdps = new int[Main.maxPlayers][];
			for (int i = 0; i < Main.maxPlayers; i++) playersdps[i] = new int[] { -1, -1 };
			ETUDUISystem.updatedps = true;
			arraynum = 0;
			FightStartTime = DateTime.Now;
			started = true;
		}

		// NOT WORKING
		public static void UpdateBossSummary()
		{		
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (Main.player[i] != null && Main.player[i].active && Main.player[i].team == Main.LocalPlayer.team)
				{
					bool exists = false;
					for (int j = 0; j <= arraynum; j++) if (playersdps[j][0] == i)
					{
							exists = true;
							int dps = Main.player[i].getDPS();
							playersdps[j][1] = dps > playersdps[j][1] ? dps : playersdps[j][1];
					}

					if (!exists)
					{
						int dps = Main.player[i].getDPS();
						playersdps[arraynum] = new int[] { i, (dps > playersdps[arraynum][1]) ? dps : 0 };
						arraynum++;
					}
				}
			}
		}

		public static void EndBossSummary(string arg, string addarg = "", bool special = false)
		{
			if (!started) return;

			started = false;
			FightEndTime = DateTime.Now;
			FightDuration = FightEndTime - FightStartTime;
			ETUDUISystem.updatedps = false;
			Array.Sort(playersdps, (x, y) => Comparer<int>.Default.Compare(y[1], x[1]));

			string output = Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ETUDInfoDPS").Value + "\n";
			if (!special) output += (arg == "" ? "> Team wiped in " : "> " + arg + " killed in ") + FightDuration.ToString(@"hh\:mm\:ss") + "\n";
			else output += arg + "Fight time: " + FightDuration.ToString(@"hh\:mm\:ss") + "\n";
			output += addarg + "\n";
			/*output += "> Top DPS:" + "\n";
			output += " 1: " + Main.player[playersdps[0][0]].name + (playersdps[0][1] != 0 ? ( " - Peak DPS: " + playersdps[0][1]) : " - No damage") + "\n";
			if (playersdps[1][0] != -1) output += " 2: " + Main.player[playersdps[1][0]].name + (playersdps[1][1] != 0 ? (" - Peak DPS: " + playersdps[1][1]) : " - No damage") + "\n";
			if (playersdps[2][0] != -1) output += " 3: " + Main.player[playersdps[2][0]].name + (playersdps[2][1] != 0 ? (" - Peak DPS: " + playersdps[2][1]) : " - No damage");*/

			Main.NewText(output, ETUDTextColor);
		}

		public static void CreateErrorMessage(string className, Exception exception, int? num = null) => Main.NewText($"ETUD Error: [{Regex.Replace(className, "[^A-Z]", "")}-{Regex.Replace(exception.GetType().Name, "[^A-Z]", "")}-{Convert.ToString(num ?? 0, 2)}] If this error persists, please contact the mod creator.", Color.Red);
	}	
}