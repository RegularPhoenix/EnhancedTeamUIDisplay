using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDAdditionalOptions
	{
		public static readonly Color ETUDTextColor = new(215, 195, 240);

		public static void CheckForBuffs()
		{
			if (Main.LocalPlayer.team == 0) return;

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
						// Melee
						case "Melee":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassWarrior")}]";

							if (!(MiscEventHandler.HasItemsInInventory(Ally, new int[] { ItemID.FlaskofCursedFlames, ItemID.FlaskofFire, ItemID.FlaskofGold, ItemID.FlaskofIchor, ItemID.FlaskofNanites, ItemID.FlaskofParty, ItemID.FlaskofPoison, ItemID.FlaskofVenom })
								|| MiscEventHandler.HasBuffs(Ally, new int[] { BuffID.WeaponImbueCursedFlames, BuffID.WeaponImbueFire, BuffID.WeaponImbueIchor, BuffID.WeaponImbueNanites, BuffID.WeaponImbueConfetti, BuffID.WeaponImbuePoison, BuffID.WeaponImbueVenom })))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.NoFlask")} ";

							if (!Ally.HasBuff(BuffID.Sharpened)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.SharpBuff")} ";

							break;

						// Ranged
						case "Ranged":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassRanger")}]";
							if (!(MiscEventHandler.HasItemsInInventory(Ally, new int[] { ItemID.AmmoReservationPotion, ItemID.ArcheryPotion })
								|| MiscEventHandler.HasBuffs(Ally, new int[] { BuffID.AmmoReservation, BuffID.Archery })))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.RangerBuff")} ";

							if (!Ally.HasBuff(BuffID.AmmoBox)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.AmmoBoxBuff")} ";

							break;

						// Magic
						case "Magic":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassMage")}]";

							if (!(MiscEventHandler.HasItemsInInventory(Ally, new int[] { ItemID.ManaRegenerationPotion, ItemID.MagicPowerPotion })
								|| MiscEventHandler.HasBuffs(Ally, new int[] { BuffID.ManaRegeneration, BuffID.MagicPower })))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.MageBuff")} ";
							
							if (!Ally.HasBuff(BuffID.Clairvoyance)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.CrystalBuff")} ";

							int ManaAmount = MiscEventHandler.CountItemsInInventory(Ally, new int[] { ItemID.ManaPotion, ItemID.GreaterManaPotion, ItemID.LesserManaPotion, ItemID.SuperManaPotion, ItemID.RestorationPotion });
							if (ETUD.CalamityMod != null) if (ETUD.CalamityMod.TryFind<ModItem>("SupremeManaPotion", out var SupremeManaPotion)) ManaAmount += Ally.CountItem(SupremeManaPotion.Type);

							if (ManaAmount == 0) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.OutOfManaPotions")} ";
							else if (ManaAmount <= 5) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.IsLowOnManaPotions")} ";
							break;

						// Summon
						case "Summon":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.Summoner")}]";
							if (!(Ally.HasItem(ItemID.SummoningPotion) || Ally.HasBuff(BuffID.Summoning)))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.SummonBuff")} ";
							
							if (!Ally.HasBuff(BuffID.Bewitched)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.BewitchedBuff")} ";
							break;

						// Rogue #CALAMITY
						case "Rogue":
							classname += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassRogue")}]";
							if (!(MiscEventHandler.HasItemsInInventory(Ally, new int[] { ItemID.FlaskofCursedFlames, ItemID.FlaskofFire, ItemID.FlaskofGold, ItemID.FlaskofIchor, ItemID.FlaskofNanites, ItemID.FlaskofParty, ItemID.FlaskofPoison, ItemID.FlaskofVenom })
								|| MiscEventHandler.HasBuffs(Ally, new int[] { BuffID.WeaponImbueCursedFlames, BuffID.WeaponImbueFire, BuffID.WeaponImbueIchor, BuffID.WeaponImbueNanites, BuffID.WeaponImbueConfetti, BuffID.WeaponImbuePoison, BuffID.WeaponImbueVenom })))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.NoFlask")} ";

							if (ETUD.CalamityMod != null)
							{
								if (ETUD.CalamityMod.TryFind<ModItem>("ShadowPotion", out var ShadowPotion) && ETUD.CalamityMod.TryFind<ModBuff>("ShadowBuff", out var ShadowBuff))
								{
									if (!(MiscEventHandler.HasItemsInInventory(Ally, new int[] { ItemID.InvisibilityPotion, ShadowPotion.Type })
									|| MiscEventHandler.HasBuffs(Ally, new int[] { BuffID.Invisibility, ShadowBuff.Type })))
										output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.RogueBuff")} ";
								}
							}
							break;
						case "None":
							break;
						default:
							break;
					}

					int HealsAmount = MiscEventHandler.CountItemsInInventory(Ally, new int[] { ItemID.HealingPotion, ItemID.GreaterHealingPotion, ItemID.LesserHealingPotion, ItemID.SuperHealingPotion, ItemID.RestorationPotion });

					if (ETUD.CalamityMod != null) if (ETUD.CalamityMod.TryFind<ModItem>("SupremeHealingPotion", out var SupremeHealingPotion) && ETUD.CalamityMod.TryFind<ModItem>("OmegaHealingPotion", out var OmegaHealingPotion)) HealsAmount += Ally.CountItem(SupremeHealingPotion.Type) + Ally.CountItem(OmegaHealingPotion.Type);

					if (HealsAmount == 0) { output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.OutOfHPPotions")} "; textColor = Color.Red; }
					else if (HealsAmount <= 5) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.IsLowOnHPPotions")} ";

					if (output == "") { output = "- Has required buffs and potions"; textColor = Color.Green; }
					Main.NewText($"{Ally.name} {classname} {output}", textColor);
				}
			}
		}

		private static bool started;
		private static DateTime FightStartTime;
		private static DateTime FightEndTime;
		private static TimeSpan FightDuration;

		public static void StartBossSummary()
		{
			FightStartTime = DateTime.Now;
			started = true;
		}

		// Arg - Boss Name, Addarg - Dictionary call(boss name, won and lost battles), Special - special text if first boss has escaped but some other has been defeated 
		public static void EndBossSummary(string arg, string addarg = "", bool special = false)
		{
			if (!started) return;
			started = false;

			FightEndTime = DateTime.Now;
			FightDuration = FightEndTime - FightStartTime;

			string output = Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ETUDInfoDPS").Value + "\n";
			if (!special) output += (arg == "" ? "> Team wiped in " : "> " + arg + " killed in ") + FightDuration.ToString(@"hh\:mm\:ss") + "\n";
			else output += arg + "Fight time: " + FightDuration.ToString(@"hh\:mm\:ss") + "\n";
			output += addarg + "\n";

			// TODO: Top-3 player by damage dealt (DPS)
			/*output += "> Top DPS:" + "\n";
			output += " 1: " + Main.player[playersdps[0][0]].name + (playersdps[0][1] != 0 ? ( " - Peak DPS: " + playersdps[0][1]) : " - No damage") + "\n";
			if (playersdps[1][0] != -1) output += " 2: " + Main.player[playersdps[1][0]].name + (playersdps[1][1] != 0 ? (" - Peak DPS: " + playersdps[1][1]) : " - No damage") + "\n";
			if (playersdps[2][0] != -1) output += " 3: " + Main.player[playersdps[2][0]].name + (playersdps[2][1] != 0 ? (" - Peak DPS: " + playersdps[2][1]) : " - No damage");*/

			Main.NewText(output, ETUDTextColor);
		}

		private static int ErrorsAmount = 0;

		public static void CreateErrorMessage(string className, Exception exception, int? num = null)
		{
			if (ETUDConfig.Instanse.ShowErrorMessages)
			{
				ErrorsAmount++;
				Main.NewText($"ETUD Error: [{Regex.Replace(className, "[^A-Z]", "")}-{Regex.Replace(exception.GetType().Name, "[^A-Z]", "")}-{Convert.ToString(num ?? 0, 16)}] If this error persists, please contact the mod creator.", Color.Red);
				ETUD.Instance.Logger.Error($"Enhanced Team UI Display Error: In:{className} Error type:{exception.GetType().Name} Stack trace:{exception.StackTrace} ETUD Error Number:{num}");
				if (ErrorsAmount >= 5) { ETUDConfig.Instanse.ShowErrorMessages = false; Main.NewText("ETUD Warning: There were too many errors, so \"Show error messages\" option in config was disabled. If you see anything strange in ETUD, please contact the mod author. There is a possibility that this is a false positive, so if you don't notice anything strange, simply ignore this message, the problem will probably be fixed in the next update.", Color.OrangeRed); }
			}
		}
	}	
}