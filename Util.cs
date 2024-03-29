﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace EnhancedTeamUIDisplay
{
	internal class Util
	{
		internal static readonly Color ETUDTextColor = new(215, 195, 240);

		internal static readonly Color GenericHealthColor = new(175, 40, 60);
		internal static readonly Color GenericManaColor = new(50, 80, 140);

		#region Classes
		internal enum PlayerClass
		{
			Melee,
			Ranger,
			Mage,
			Summoner,
			None,
			Offline,
			// CrossMod
			Rogue,
			Bard,
			Healer,
		}

		private static float GetClassCoefficient(Player player, DamageClass damageClass, float additional = 0) {;
			return player.GetTotalDamage(damageClass).ApplyTo(1) - 1.5f
				+ (player.GetCritChance(damageClass) / 50) // + 10% crit chance adds 0.2
				+ (player.GetAttackSpeed(damageClass) / 2) // + 10% aspd adds 0.05
				+ additional;
		}

		internal static PlayerClass GuessPlayerClass(Player player) {
			if (player is null || !player.active)
				return PlayerClass.Offline;

			float melee = GetClassCoefficient(player, DamageClass.Melee);
			float ranged = GetClassCoefficient(player, DamageClass.Ranged);
			float magic = GetClassCoefficient(player, DamageClass.Magic);
			float summon = GetClassCoefficient(player, DamageClass.Summon, (player.maxMinions - 1) / 10);

			Dictionary<PlayerClass, float> coefficients = new() {
				{ PlayerClass.Melee, melee },
				{ PlayerClass.Ranger, ranged },
				{ PlayerClass.Mage, magic },
				{ PlayerClass.Summoner, summon }
			};

			// CrossMod
			if (CrossModHelper.CalamityMod is not null
				&& CrossModHelper.CalamityMod.TryFind("RogueDamageClass", out DamageClass rogueClass)
			) {
				coefficients.Add(PlayerClass.Rogue, GetClassCoefficient(player, rogueClass));
			}

			if (CrossModHelper.ThoriumMod is not null) {
				if (CrossModHelper.ThoriumMod.TryFind("BardDamage", out DamageClass bardClass))
					coefficients.Add(PlayerClass.Bard, GetClassCoefficient(player, bardClass));

				if (CrossModHelper.ThoriumMod.TryFind("HealerDamage", out DamageClass healerClass))
					coefficients.Add(PlayerClass.Healer, GetClassCoefficient(player, healerClass, CrossModHelper.GetHealerHealBonus(player)));
			}

			var sorted = coefficients.OrderByDescending(pair => pair.Value);

			if (Math.Round(sorted.ElementAt(0).Value, 1) == Math.Round(sorted.ElementAt(1).Value, 1)) {
				return PlayerClass.None;
			}

			return sorted.First().Key;
		}

		internal static (Color health, Color resource) GetClassColours(PlayerClass playerClass) {
			return playerClass switch {
				PlayerClass.Melee => new(new(200, 155, 100), GenericHealthColor),
				PlayerClass.Ranger => new(new(170, 210, 115), new(185, 110, 50)),
				PlayerClass.Mage => new(new(110, 200, 240), GenericManaColor),
				PlayerClass.Summoner => new(new(150, 130, 200), GenericManaColor),
				PlayerClass.Offline => new(Color.Gray, Color.LightGray),
				// CrossMod
				PlayerClass.Rogue => new(new(255, 240, 110), new(180, 150, 20)),
				PlayerClass.Bard => new(new(200, 130, 160), new(95, 85, 125)),
				PlayerClass.Healer => new(new(0, 160, 95), GenericManaColor),
				_ => new(GenericHealthColor, GenericManaColor)
			};
		}

		internal static float GetClassResourceRelation(Player player, PlayerClass playerClass) {
			return player is null
				? 1
				: playerClass switch {
					PlayerClass.Melee => 1,
					PlayerClass.Ranger => 1,
					// CrossMod
					PlayerClass.Rogue => Utils.Clamp(
						CrossModHelper.GetRogueStealth(player) / CrossModHelper.GetRogueStealthMax(player),
						0f, 1f
					),
					PlayerClass.Bard => Utils.Clamp(
						CrossModHelper.GetBardInspiration(player) / CrossModHelper.GetBardInspirationMax(player),
						0f, 1f
					),
					_ => Utils.Clamp((float) player.statMana / player.statManaMax2, 0f, 1f),
				};
		}
		#endregion

		#region Error handling
		private static int _errorAmount = 0;

		public static void CreateErrorMessage(string header, Exception exception) {
			ETUD.Instance.Logger.Error($"Enhanced Team UI Display Error: In:{header} Error type:{exception.GetType().Name} Stack trace:{exception.StackTrace}");

			if (!Config.Instanse.AreErrorMessagesDisplayed)
				return;

			_errorAmount++;

			Main.NewText(Terraria.Localization.Language.GetText("Mods.EnhancedTeamUIDisplay.ErrorTexts.Error"), Color.Red);

			if (_errorAmount >= 5) {
				Config.Instanse.AreErrorMessagesDisplayed = false;
				Main.NewText("Mods.EnhancedTeamUIDisplay.ErrorTexts.TooManyErrors", Color.OrangeRed);
			}
		}
		#endregion
	}
}
