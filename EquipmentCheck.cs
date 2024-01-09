using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace EnhancedTeamUIDisplay
{
	internal class EquipmentCheck
	{
		internal static void CheckAlliesEquipment() {
			if (Main.LocalPlayer.team == 0)
				return;

			// I really don't like all these long Language.GetTexts
			static string getEquipmentCheckText(string key)
				=> Language.GetText("Mods.EnhancedTeamUIDisplay.EquipmentCheck." + key).Value;

			Main.NewText(
				getEquipmentCheckText("Title"),
				Util.ETUDTextColor
			);

			List<Player> allies = Main.player.Where(
				player =>
					player is not null
					&& player.active
					&& player.team == Main.LocalPlayer.team
			).ToList();

			foreach (Player ally in allies) {
				int manaPotionsAmount = 0, healingPotionsAmount = 0;

				for (int j = 0; j < ally.inventory.Length; j++) {
					Item item = ally.inventory[j];

					if (item.healLife > 0)
						healingPotionsAmount += item.stack;

					if (item.healMana > 0)
						manaPotionsAmount += item.stack;
				}

				Util.PlayerClass allyClass = Util.GuessPlayerClass(ally);

				string output = $"{ally.name} [{Language.GetText("Mods.EnhancedTeamUIDisplay.GeneralNouns." + allyClass switch {
					Util.PlayerClass.Melee => "ClassMelee",
					Util.PlayerClass.Ranger => "ClassRanger",
					Util.PlayerClass.Mage => "ClassMage",
					Util.PlayerClass.Summoner => "ClassSummoner",
					Util.PlayerClass.Rogue => "ClassRogue",
					Util.PlayerClass.Bard => "ClassBard",
					Util.PlayerClass.Healer => "ClassHealer",
					_ => "ClassNone"
				}).Value}] ";

				bool hasClassBuffs = allyClass switch {
					Util.PlayerClass.Melee => Util.HasAnyItem(ally, meleeItems) || Util.HasAnyBuff(ally, meleeBuffs),
					Util.PlayerClass.Ranger => Util.HasAnyItem(ally, rangerItems) || Util.HasAnyBuff(ally, rangerBuffs),
					Util.PlayerClass.Mage => Util.HasAnyItem(ally, mageItems) || Util.HasAnyBuff(ally, mageBuffs),
					Util.PlayerClass.Summoner => ally.HasItem(ItemID.SummoningPotion) || ally.HasBuff(BuffID.Summoning),
					Util.PlayerClass.Rogue => Util.HasAnyItem(ally, rogueItems) || Util.HasAnyBuff(ally, rogueBuffs),
					Util.PlayerClass.Bard => Util.HasAnyItem(ally, bardItems) || Util.HasAnyBuff(ally, bardBuffs),
					Util.PlayerClass.Healer => Util.HasAnyItem(ally, healerItems) || Util.HasAnyBuff(ally, healerBuffs),
					_ => true
				};

				bool hasStationBuff = allyClass switch {
					Util.PlayerClass.Melee => ally.HasBuff(BuffID.Sharpened),
					Util.PlayerClass.Ranger => ally.HasBuff(BuffID.AmmoBox),
					Util.PlayerClass.Mage => ally.HasBuff(BuffID.Clairvoyance),
					Util.PlayerClass.Summoner => ally.HasBuff(BuffID.Summoning),
					// Calamity seems to have no station for rogues
					Util.PlayerClass.Bard => ally.HasBuff(CrossModHelper.ThoriumMod.Find<ModBuff>("ConductorsStandBuff").Type),
					Util.PlayerClass.Healer => ally.HasBuff(CrossModHelper.ThoriumMod.Find<ModBuff>("AltarBuff").Type),
					_ => true
				};

				bool? hasEnoughHealingPotions = healingPotionsAmount switch {
					0 => null,
					< 7 => false,
					_ => true
				};

				bool? hasEnoughManaPotions = manaPotionsAmount switch {
					0 => null,
					< 7 => false,
					_ => true
				};

				string classBuffMissing = allyClass switch {
					Util.PlayerClass.Melee => $" {getEquipmentCheckText("FlaskMissing")}",
					Util.PlayerClass.Ranger => $" {getEquipmentCheckText("RangerBuffMissing")}",
					Util.PlayerClass.Mage => $" {getEquipmentCheckText("MageBuffMissing")}",
					Util.PlayerClass.Summoner => $" {getEquipmentCheckText("SummonerBuffMissing")}",
					Util.PlayerClass.Rogue => $" {getEquipmentCheckText("RogueBuffMissing")}",
					Util.PlayerClass.Bard => $" {getEquipmentCheckText("BardBuffMissing")}",
					Util.PlayerClass.Healer => $" {getEquipmentCheckText("HealerBuffMissing")}",
					_ => string.Empty
				};

				string stationBuffMissing = allyClass switch {
					Util.PlayerClass.Melee => $" {getEquipmentCheckText("SharpenedBuffMissing")}",
					Util.PlayerClass.Ranger => $" {getEquipmentCheckText("AmmoBoxBuffMissing")}",
					Util.PlayerClass.Mage => $" {getEquipmentCheckText("ClairvoyanceBuffMissing")}",
					Util.PlayerClass.Summoner => $" {getEquipmentCheckText("BewitchedBuffMissing")}",
					Util.PlayerClass.Bard => $" {getEquipmentCheckText("ConductorsBuffMissing")}",
					Util.PlayerClass.Healer => $" {getEquipmentCheckText("AltarBuffMissing")}",
					_ => string.Empty
				};

				Color color = Color.Green;

				if (hasClassBuffs
					&& hasStationBuff
					&& (hasEnoughHealingPotions ?? false)
					&& (allyClass != Util.PlayerClass.Mage || (hasEnoughManaPotions ?? false))
				) {
					output += $" {getEquipmentCheckText("HasRecommended")}";
				} else {
					color = Color.Yellow;

					if (!hasClassBuffs)
						output += classBuffMissing;

					if (!hasStationBuff)
						output += stationBuffMissing;

					output += hasEnoughHealingPotions switch {
						null => $" {getEquipmentCheckText("HealthPotionsMissing")}",
						false => $" {getEquipmentCheckText("HealthPotionsLow")}",
						true => string.Empty
					};

					if (allyClass == Util.PlayerClass.Mage) {
						output += hasEnoughManaPotions switch {
							null => $" {getEquipmentCheckText("ManaPotionsMissing")}",
							false => $" {getEquipmentCheckText("ManaPotionsLow")}",
							true => string.Empty
						};
					}
				}

				Main.NewText(output, color);
			}
		}

		#region Items&Buffs arrays
		private static readonly int[] meleeItems = [
			ItemID.FlaskofCursedFlames,
			ItemID.FlaskofFire,
			ItemID.FlaskofGold,
			ItemID.FlaskofIchor,
			ItemID.FlaskofNanites,
			ItemID.FlaskofParty,
			ItemID.FlaskofPoison,
			ItemID.FlaskofVenom
		];

		private static readonly int[] meleeBuffs = [
			BuffID.WeaponImbueCursedFlames,
			BuffID.WeaponImbueFire,
			BuffID.WeaponImbueIchor,
			BuffID.WeaponImbueNanites,
			BuffID.WeaponImbueConfetti,
			BuffID.WeaponImbuePoison,
			BuffID.WeaponImbueVenom
		];

		private static readonly int[] rangerItems = [
			ItemID.AmmoReservationPotion,
			ItemID.ArcheryPotion
		];

		private static readonly int[] rangerBuffs = [
			BuffID.AmmoReservation,
			BuffID.Archery
		];

		private static readonly int[] mageItems = [
			ItemID.ManaRegenerationPotion,
			ItemID.MagicPowerPotion
		];

		private static readonly int[] mageBuffs = [
			BuffID.ManaRegeneration,
			BuffID.MagicPower
		];

		private static readonly int[] rogueItems = [
			CrossModHelper.CalamityMod.Find<ModItem>("FlaskOfBrimstone").Type,
			CrossModHelper.CalamityMod.Find<ModItem>("FlaskOfHolyFlames").Type,
			CrossModHelper.CalamityMod.Find<ModItem>("FlaskOfCrumbling").Type,
			CrossModHelper.CalamityMod.Find<ModItem>("ShadowPotion").Type
		];

		private static readonly int[] rogueBuffs = [
			CrossModHelper.CalamityMod.Find<ModBuff>("WeaponImbueBrimstone").Type,
			CrossModHelper.CalamityMod.Find<ModBuff>("WeaponImbueHolyFlames").Type,
			CrossModHelper.CalamityMod.Find<ModBuff>("WeaponImbueCrumbling").Type,
			CrossModHelper.CalamityMod.Find<ModBuff>("ShadowBuff").Type
		];

		private static readonly int[] bardItems = [
			CrossModHelper.ThoriumMod.Find<ModItem>("CreativityPotion").Type,
			CrossModHelper.ThoriumMod.Find<ModItem>("EarwormPotion").Type,
			CrossModHelper.ThoriumMod.Find<ModItem>("InspirationReachPotion").Type
		];

		private static readonly int[] bardBuffs = [
			CrossModHelper.ThoriumMod.Find<ModBuff>("CreativityPotionBuff").Type,
			CrossModHelper.ThoriumMod.Find<ModBuff>("EarwormPotionBuff").Type,
			CrossModHelper.ThoriumMod.Find<ModBuff>("InspirationReachPotionBuff").Type
		];

		private static readonly int[] healerItems = [
			CrossModHelper.ThoriumMod.Find<ModItem>("HolyPotion").Type,
			CrossModHelper.ThoriumMod.Find<ModItem>("GlowingPotion").Type
		];

		private static readonly int[] healerBuffs = [
			CrossModHelper.ThoriumMod.Find<ModBuff>("HolyPotionBuff").Type,
			CrossModHelper.ThoriumMod.Find<ModBuff>("GlowingPotionBuff").Type
		];
		#endregion
	}
}
