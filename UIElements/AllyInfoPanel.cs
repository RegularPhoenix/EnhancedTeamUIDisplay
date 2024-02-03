using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace EnhancedTeamUIDisplay.UIElements
{
	internal class AllyInfoPanel : UIElement
	{
		internal AllyInfoPanel(int number, bool extended, float left, float top) {
			PanelNumber = number;
			IsExtended = extended;
			FixedLeft = left;
			FixedTop = top;
		}

		internal const int ElementWidth = 232, ElementHeight = 264;

		private int PanelNumber { get; }
		private bool IsExtended { get; }
		private float FixedLeft { get; }
		private float FixedTop { get; }

		private UIImage _frame, _background;

		// Default
		private UIText _nameText, _armorText, _accessoriesText, _rightStatText, _leftStatText;

		// Extended
		private UIText _playerClassText, _classStatsText;

		// TODO: Some elements' position seem to be a bit off
		public override void OnInitialize() {
			Width.Pixels = ElementWidth;
			Height.Pixels = ElementHeight;

			_background = new UIImage(
				IsExtended
				? ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoPanel/BackgroundExtended")
				: ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoPanel/Background")
			);
			_background.Width.Set(ElementWidth, 0f);
			_background.Height.Set(ElementHeight, 0f);
			Append(_background);

			_frame = new UIImage(
				IsExtended
				? ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoPanel/FrameExtended")
				: ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoPanel/Frame")
			);
			_frame.Width.Set(ElementWidth, 0f);
			_frame.Height.Set(ElementHeight, 0f);
			Append( _frame );

			_armorText = new UIText(string.Empty);
			_armorText.Top.Set(35, 0f);
			_armorText.Width.Set(50, 0f);
			_armorText.HAlign = .15f;
			_armorText.TextOriginX = 0;
			Append(_armorText);

			_accessoriesText = new UIText(string.Empty);
			_accessoriesText.Top.Set(72, 0f);
			_accessoriesText.Width.Set(50, 0f);
			_accessoriesText.HAlign = .10f;
			_accessoriesText.TextOriginX = 0;
			Append( _accessoriesText);

			_nameText = new UIText(string.Empty);
			_nameText.Top.Set(8, 0f);
			_nameText.Width.Set(160, 0f);
			_nameText.HAlign = .5f;
			Append(_nameText);

			_rightStatText = new UIText(string.Empty);
			_rightStatText.Top.Set(37, 0f);
			_rightStatText.Height.Set(50, 0f);
			_rightStatText.HAlign = .92f;
			_rightStatText.TextOriginX = 1;
			Append( _rightStatText);

			_leftStatText = new UIText(string.Empty);
			_leftStatText.Top.Set(37, 0f);
			_leftStatText.Height.Set(50, 0f);
			_leftStatText.HAlign = .67f;
			_leftStatText.TextOriginX = 0;
			Append( _leftStatText);

			_playerClassText = new UIText(string.Empty);
			_playerClassText.Top.Set(136, 0f);
			_playerClassText.Width.Set(160, 0f);
			_playerClassText.HAlign = .5f;

			_classStatsText = new UIText(string.Empty, .8f);
			_classStatsText.Top.Set(167, 0f);
			_classStatsText.Width.Set(160, 0f);
			_classStatsText.HAlign = .2f;
			_classStatsText.TextOriginX = 0;
			_classStatsText.DynamicallyScaleDownToWidth = true;

			if (IsExtended) {
				Append(_playerClassText);
				Append(_classStatsText);
			}
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			Left.Pixels = FixedLeft;
			Top.Pixels = FixedTop;

			string armorTextValue, accessoriesTextValue, rightStatTextValue, leftStatTextValue;
			armorTextValue = accessoriesTextValue = string.Empty;

			Player ally = ETUDUI.Panels[PanelNumber]?.Ally;

			try {
				for (int i = 0; i < 3; i++) {
					if (ally.armor[i].type != ItemID.None)
						armorTextValue += $"[i:{ally.armor[i].type}]";
				}

				// HACK: Is 10 really maximum?
				// Not sure, but values bigger than 10 seem to show social.
				// If there can be more than 7, find a way to display more
				for (int i = 3; i < 10; i++) {
					if (ally.armor[i].type != ItemID.None) {
						accessoriesTextValue += $"[i:{ally.armor[i].type}]";

						if ((i - 2) % 4 == 0)
							accessoriesTextValue += "\n";
					}
				}

				rightStatTextValue = $"""
					[i:{ItemID.LifeCrystal}]{ally.statLifeMax2}
					[i:{ItemID.CobaltShield}]{ally.statDefense}
					[i:{ItemID.HermesBoots}]{(int) ((ally.accRunSpeed + ally.maxRunSpeed) / 2f * ally.moveSpeed * 6)}
					""";

				leftStatTextValue = $"""
					[i:{ItemID.RegenerationPotion}]{ally.lifeRegen / 2}
					[i:{ItemID.PaladinsShield}]{(int) (ally.endurance * 100)}
					[i:{ItemID.LeafWings}]{(Math.Round(ally.wingTimeMax / 60.0, 2) > 0 ? Math.Round(ally.wingTimeMax / 60.0, 2) : "-")}
					""";

				_rightStatText.SetText(rightStatTextValue);
				_leftStatText.SetText(leftStatTextValue);
				_nameText.SetText(ally.name);

				_armorText.SetText(
					string.IsNullOrEmpty(armorTextValue)
					? "No armor"
					: armorTextValue
				);

				_accessoriesText.SetText(
					string.IsNullOrEmpty(accessoriesTextValue)
					? "No acc-ies"
					: accessoriesTextValue
				);

				if (IsExtended) {
					Util.PlayerClass allyClass = Util.GuessPlayerClass(ally);

					_playerClassText.SetText(
						Language.GetText("Mods.EnhancedTeamUIDisplay.GeneralNouns." + allyClass switch {
							Util.PlayerClass.Melee => "ClassMelee",
							Util.PlayerClass.Ranger => "ClassRanger",
							Util.PlayerClass.Mage => "ClassMage",
							Util.PlayerClass.Summoner => "ClassSummoner",
							Util.PlayerClass.Rogue => "ClassRogue",
							Util.PlayerClass.Bard => "ClassBard",
							Util.PlayerClass.Healer => "ClassHealer",
							_ => "ClassNone"
						})
					);

					// CrossMod
					DamageClass rogueClass = CrossModHelper.CalamityMod?.Find<DamageClass>("RogueDamageClass");
					DamageClass bardClass = CrossModHelper.ThoriumMod?.Find<DamageClass>("BardDamage");
					DamageClass healerClass = CrossModHelper.ThoriumMod?.Find<DamageClass>("HealerDamage");

					static int getClassDamage(DamageClass damageClass, Player player) {
						return (int) Math.Round(
							(player.GetTotalDamage(damageClass).Additive * player.GetTotalDamage(damageClass).Multiplicative * 100) - 100
						);
					};

					_classStatsText.SetText(
						allyClass switch {
							Util.PlayerClass.Melee => $"""
								[i:{ItemID.IronBroadsword}] Damage Increase: {getClassDamage(DamageClass.Melee, ally)}
								[i:{ItemID.PsychoKnife}] Crit. Chance: {(int) ally.GetTotalCritChance(DamageClass.Melee)}
								[i:{ItemID.Arkhalis}] Attack Speed: {(int) (ally.GetTotalAttackSpeed(DamageClass.Melee) * 100)}%
								[i:{ItemID.FleshKnuckles}] Aggro: {ally.aggro}
								""",

							Util.PlayerClass.Ranger => $"""
								[i:{ItemID.IronBow}] Damage Increase: {getClassDamage(DamageClass.Ranged, ally)}
								[i:{ItemID.SniperRifle}] Crit. Chance: {(int) ally.GetTotalCritChance(DamageClass.Ranged)}
								[i:{ItemID.SharkToothNecklace}] Armor Penetration: {ally.GetArmorPenetration(DamageClass.Generic)}
								""",

							Util.PlayerClass.Mage => $"""
								[i:{ItemID.MagicalHarp}] Damage Increase: {getClassDamage(DamageClass.Magic, ally)}
								[i:{ItemID.SkyFracture}] Crit. Chance: {(int) ally.GetTotalCritChance(DamageClass.Magic)}
								[i:{ItemID.CrystalBall}] MP Cost Reduction: {Math.Round((1.0 - ally.manaCost) * 100)}%
								""",

							Util.PlayerClass.Summoner => $"""
								[i:{ItemID.StardustCellStaff}] Damage Increase: {getClassDamage(DamageClass.Summon, ally)}
								[i:{ItemID.MonkAltHead}] Crit. Chance: {(int) ally.GetTotalCritChance(DamageClass.Summon)}
								[i:{ItemID.ImpStaff}] Max Minions: {ally.maxMinions}
								[i:{ItemID.DD2BallistraTowerT1Popper}] Max Centries: {ally.maxTurrets}
								""",

							Util.PlayerClass.Rogue => $"""
								[i:{CrossModHelper.CalamityMod.Find<ModItem>("HeavenfallenStardisk").Type}] Damage Increase: {getClassDamage(rogueClass, ally)}
								[i:{CrossModHelper.CalamityMod.Find<ModItem>("GleamingDagger").Type}] Crit. Chance: {(int) ally.GetTotalCritChance(rogueClass)}
								""",

							Util.PlayerClass.Bard => $"""
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("ScholarsHarp").Type}] Damage Increase: {getClassDamage(bardClass, ally)}
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("MixTape").Type}] Crit. Chance: {(int) ally.GetTotalCritChance(bardClass)}
								""",

							Util.PlayerClass.Healer => $"""
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("Liberation").Type}] Damage Increase: {getClassDamage(healerClass, ally)}
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("DarkGlaze").Type}] Crit. Chance: {(int) ally.GetTotalCritChance(healerClass)}
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("Twinkle").Type}] Bonus healing: {CrossModHelper.GetHealerHealBonus(ally)}
								""",

							_ => ""
						}
					);
				}
			} catch (Exception e) {
				_nameText.SetText("Error");
				_rightStatText.SetText("N/A");
				_leftStatText.SetText("N/A");
				_armorText.SetText("N/A");
				_accessoriesText.SetText("N/A");

				Util.CreateErrorMessage("AllyInfoPanel", e);
			}
		}
	}
}
