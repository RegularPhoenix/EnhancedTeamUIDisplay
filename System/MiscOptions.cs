using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System;
using System.Linq;
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
				Player ally = null;
				
				if (i == -1) ally = Main.LocalPlayer;
				else if (Main.player[i] is not null && Main.player[i].active && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer) ally = Main.player[i];
				
				if(ally is not null)
				{
					Color textColor = Color.Yellow;
					string playerClass = MiscEventHandler.DeterminePlayerClass(ally);
					string output = "";
					string className = "";

					int manaPotionsAmount = 0, healingPotionsAmount = 0;

					for (int j = 0; j < ally.inventory.Length; j++)
					{
						Item item = ally.inventory[j];

						if (item.healLife > 0) healingPotionsAmount += item.stack;
						if (item.healMana > 0) manaPotionsAmount += item.stack;
					}

					switch (playerClass)
					{
						// Melee
						case "Melee":
							className += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassWarrior")}]";

							if (!(MiscEventHandler.HasItemsInInventory(ally, new int[] { ItemID.FlaskofCursedFlames, ItemID.FlaskofFire, ItemID.FlaskofGold, ItemID.FlaskofIchor, ItemID.FlaskofNanites, ItemID.FlaskofParty, ItemID.FlaskofPoison, ItemID.FlaskofVenom })
								|| MiscEventHandler.HasBuffs(ally, new int[] { BuffID.WeaponImbueCursedFlames, BuffID.WeaponImbueFire, BuffID.WeaponImbueIchor, BuffID.WeaponImbueNanites, BuffID.WeaponImbueConfetti, BuffID.WeaponImbuePoison, BuffID.WeaponImbueVenom })))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.NoFlask")} ";

							if (!ally.HasBuff(BuffID.Sharpened)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.SharpBuff")} ";

							break;

						// Ranged
						case "Ranged":
							className += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassRanger")}]";
							if (!(MiscEventHandler.HasItemsInInventory(ally, new int[] { ItemID.AmmoReservationPotion, ItemID.ArcheryPotion })
								|| MiscEventHandler.HasBuffs(ally, new int[] { BuffID.AmmoReservation, BuffID.Archery })))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.RangerBuff")} ";

							if (!ally.HasBuff(BuffID.AmmoBox)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.AmmoBoxBuff")} ";

							break;

						// Magic
						case "Magic":
							className += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassMage")}]";

							if (!(MiscEventHandler.HasItemsInInventory(ally, new int[] { ItemID.ManaRegenerationPotion, ItemID.MagicPowerPotion })
								|| MiscEventHandler.HasBuffs(ally, new int[] { BuffID.ManaRegeneration, BuffID.MagicPower })))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.MageBuff")} ";
							
							if (!ally.HasBuff(BuffID.Clairvoyance)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.CrystalBuff")} ";

							//int manaAmount = MiscEventHandler.CountItemsInInventory(ally, new int[] { ItemID.ManaPotion, ItemID.GreaterManaPotion, ItemID.LesserManaPotion, ItemID.SuperManaPotion, ItemID.RestorationPotion });
							//if (ETUD.CalamityMod is not null) if (ETUD.CalamityMod.TryFind<ModItem>("SupremeManaPotion", out var SupremeManaPotion)) manaAmount += ally.CountItem(SupremeManaPotion.Type);

							if (manaPotionsAmount == 0) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.OutOfManaPotions")} ";
							else if (manaPotionsAmount <= 5) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.IsLowOnManaPotions")} ";
							break;

						// Summon
						case "Summon":
							className += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassSummoner")}]";
							if (!(ally.HasItem(ItemID.SummoningPotion) || ally.HasBuff(BuffID.Summoning)))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.SummonBuff")} ";
							
							if (!ally.HasBuff(BuffID.Bewitched)) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.BewitchedBuff")} ";
							break;

						// Rogue #CALAMITY
						case "Rogue":
							className += $"[{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ClassRogue")}]";
							if (!(MiscEventHandler.HasItemsInInventory(ally, new int[] { ItemID.FlaskofCursedFlames, ItemID.FlaskofFire, ItemID.FlaskofGold, ItemID.FlaskofIchor, ItemID.FlaskofNanites, ItemID.FlaskofParty, ItemID.FlaskofPoison, ItemID.FlaskofVenom })
								|| MiscEventHandler.HasBuffs(ally, new int[] { BuffID.WeaponImbueCursedFlames, BuffID.WeaponImbueFire, BuffID.WeaponImbueIchor, BuffID.WeaponImbueNanites, BuffID.WeaponImbueConfetti, BuffID.WeaponImbuePoison, BuffID.WeaponImbueVenom })))
								output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.NoFlask")} ";

							if (ETUD.CalamityMod is not null)
							{
								if (ETUD.CalamityMod.TryFind<ModItem>("ShadowPotion", out var ShadowPotion) && ETUD.CalamityMod.TryFind<ModBuff>("ShadowBuff", out var ShadowBuff))
								{
									if (!(MiscEventHandler.HasItemsInInventory(ally, new int[] { ItemID.InvisibilityPotion, ShadowPotion.Type })
									|| MiscEventHandler.HasBuffs(ally, new int[] { BuffID.Invisibility, ShadowBuff.Type })))
										output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.RogueBuff")} ";
								}
							}
							break;
						default:
							CreateErrorMessage("ETUDAdditionalOptions", new ArgumentException(), i);
							break;
					}

					//int healsAmount = MiscEventHandler.CountItemsInInventory(ally, new int[] { ItemID.HealingPotion, ItemID.GreaterHealingPotion, ItemID.LesserHealingPotion, ItemID.SuperHealingPotion, ItemID.RestorationPotion });
					//if (ETUD.CalamityMod is not null) if (ETUD.CalamityMod.TryFind<ModItem>("SupremeHealingPotion", out var SupremeHealingPotion) && ETUD.CalamityMod.TryFind<ModItem>("OmegaHealingPotion", out var OmegaHealingPotion)) healsAmount += ally.CountItem(SupremeHealingPotion.Type) + ally.CountItem(OmegaHealingPotion.Type);

					if (healingPotionsAmount == 0) { output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.OutOfHPPotions")} "; textColor = Color.Red; }
					else if (healingPotionsAmount <= 5) output += $"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.IsLowOnHPPotions")} ";

					if (output == "") { output = "- Has required buffs and potions"; textColor = Color.Green; }
					Main.NewText($"{ally.name} {className} {output}", textColor);
				}
			}
		}

		public static DateTime LastFightEndTime => BossFightEndTime;		

		private static bool BossFightStarted = false;
		private static DateTime BossFightStartTime;
		private static DateTime BossFightEndTime = DateTime.MinValue;
		private static TimeSpan BossFightDuration;

		public static void OnBossFightStart()
		{
			BossFightStartTime = DateTime.Now;
			BossFightStarted = true;
			if (!Main.CurrentFrameFlags.AnyActiveBossNPC) ETUD.Instance.ResetVariables();
		}

		// Arg - Boss Name, Addarg - Dictionary call(boss name, won and lost battles), Special - special text if first boss has escaped but some other has been defeated 
		public static void OnBossFightEnd(string arg, string addarg = "", bool special = false)
		{
			if (!BossFightStarted) return;
			BossFightStarted = false;

			BossFightEndTime = DateTime.Now;
			BossFightDuration = BossFightEndTime - BossFightStartTime;

			string output = Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDAddOptions.ETUDInfoDPS").Value + "\n";
			if (!special) output += (arg == "" ? "> Team wiped in " : "> " + arg + " killed in ") + BossFightDuration.ToString(@"hh\:mm\:ss") + "\n";
			else output += arg + "Fight time: " + BossFightDuration.ToString(@"hh\:mm\:ss") + "\n";
			output += addarg + "\n";

			if (ETUDConfig.Instanse.ShowBossSummary) Main.NewText(output, ETUDTextColor);
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

	public class MiscEventHandler
	{
		public static string DeterminePlayerClass(Player player)
		{
			if (player is null) return "None";

			float meleeCoeff = player.GetTotalDamage(DamageClass.Melee).Additive + (player.GetCritChance(DamageClass.Melee) / 100) + (player.GetAttackSpeed(DamageClass.Melee) / 2);
			float rangedCoeff = player.GetTotalDamage(DamageClass.Ranged).Additive + (player.GetCritChance(DamageClass.Ranged) / 100) + (player.GetAttackSpeed(DamageClass.Ranged) / 2);
			float magicCoeff = player.GetTotalDamage(DamageClass.Magic).Additive + (player.GetCritChance(DamageClass.Magic) / 100) + (player.GetAttackSpeed(DamageClass.Magic) / 2);
			float summonCoeff = player.GetTotalDamage(DamageClass.Summon).Additive + (player.GetCritChance(DamageClass.Summon) / 100) + (player.GetAttackSpeed(DamageClass.Summon) / 2 + (player.maxMinions - 1));
			float rogueCoeff = 0;
			if (ETUD.CalamityMod is not null)
				if (ETUD.CalamityMod.TryFind<DamageClass>("RogueDamageClass", out var rogueclass))
					rogueCoeff = player.GetTotalDamage(rogueclass).Additive + (player.GetCritChance(rogueclass) / 100) + (player.GetAttackSpeed(rogueclass) / 2);

			float[] CoeffArray = new float[] { meleeCoeff, rangedCoeff, magicCoeff, summonCoeff, rogueCoeff };
			int StatNum = Array.IndexOf(CoeffArray, CoeffArray.Max());

			return StatNum switch
			{
				0 => "Melee",
				1 => "Ranged",
				2 => "Magic",
				3 => "Summon",
				4 => "Rogue",
				_ => "None",
			};
		}

		public static float GetClassRQ(string playerClass, Player ally)
		{
			return playerClass switch
			{
				"Melee" => 1,
				"Ranged" => 1,
				"Rogue" => Utils.Clamp(CalamityHelper.RogueStealth(ally) / CalamityHelper.RogueStealthMax(ally), 0f, 1f),
				_ => Utils.Clamp((float)ally.statMana / ally.statManaMax2, 0f, 1f),
			};
		}

		public static Tuple<Color, Color> GetClassColours(string playerClass)
		{
			return playerClass switch
			{
				"Melee" => new(new(200, 155, 100), new(145, 30, 50)),
				"Ranged" => new(new(170, 210, 115), new(165, 80, 40)),
				"Magic" => new(new(110, 200, 240), new(50, 80, 140)),
				"Summon" => new(new(150, 130, 200), new(50, 80, 140)),
				"Rogue" => new(new(255, 240, 110), new(180, 150, 20)),
				"Offline" => new(Color.Gray, Color.LightGray),
				"None" => new(Color.Green, Color.Blue),
				_ => new(Color.White, Color.White),
			};
		}

		public static bool HasItemsInInventory(Player player, int[] itemtypes)
		{
			for (int i = 0; i < itemtypes.Length; i++) if (player.HasItem(itemtypes[i])) return true;
			return false;
		}

		public static bool HasBuffs(Player player, int[] bufftypes)
		{
			for (int i = 0; i < bufftypes.Length; i++) if (player.HasBuff(bufftypes[i])) return true;
			return false;
		}

		public static int CountItemsInInventory(Player player, int[] itemtypes)
		{
			int count = 0;
			for (int i = 0; i < itemtypes.Length; i++) count += player.CountItem(itemtypes[i]);
			return count;
		}
	}

	public class CalamityHelper
	{
		public static float RogueStealth(Player player) => (float)ETUD.CalamityMod.Call("GetCurrentStealth", player);

		public static float RogueStealthMax(Player player) => (float)ETUD.CalamityMod.Call("GetMaxStealth", player);
	}
}