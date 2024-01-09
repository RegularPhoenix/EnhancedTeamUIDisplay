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

		internal const int width = 232, height = 264;

		private int PanelNumber { get; }
		private bool IsExtended { get; }
		private float FixedLeft { get; }
		private float FixedTop { get; }

		private UIElement mainElement;
		private UIImage frame, background;

		// Default
		private UIText nameText, armorText, accessoriesText, rightStatText, leftStatText;

		// Extended
		private UIText playerClassText, classStatsText;

		// TODO: Some elements' position seem to be a bit off
		public override void OnInitialize() {
			Width.Pixels = width;
			Height.Pixels = height;

			mainElement = new UIElement();
			mainElement.Left.Set(0, 0f);
			mainElement.Top.Set(0, 0f);
			mainElement.Width.Set(width, 0f);
			mainElement.Height.Set(height, 0f);

			background = new UIImage(
				IsExtended
				? ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoPanel/BackgroundExtended")
				: ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoPanel/Background")
			);
			background.Left.Set(0, 0f);
			background.Top.Set(0, 0f);
			background.Width.Set(width, 0f);
			background.Height.Set(height, 0f);

			frame = new UIImage(
				IsExtended
				? ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoPanel/FrameExtended")
				: ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoPanel/Frame")
			);
			frame.Width.Set(width, 0f);
			frame.Height.Set(height, 0f);

			armorText = new UIText(string.Empty);
			armorText.Top.Set(35, 0f);
			armorText.Width.Set(50, 0f);
			armorText.HAlign = .15f;
			armorText.TextOriginX = 0;

			accessoriesText = new UIText(string.Empty);
			accessoriesText.Top.Set(72, 0f);
			accessoriesText.Width.Set(50, 0f);
			accessoriesText.HAlign = .10f;
			accessoriesText.TextOriginX = 0;

			nameText = new UIText(string.Empty);
			nameText.Top.Set(8, 0f);
			nameText.Width.Set(160, 0f);
			nameText.HAlign = .5f;

			rightStatText = new UIText(string.Empty);
			rightStatText.Top.Set(37, 0f);
			rightStatText.Height.Set(50, 0f);
			rightStatText.HAlign = .92f;
			rightStatText.TextOriginX = 1;

			leftStatText = new UIText(string.Empty);
			leftStatText.Top.Set(37, 0f);
			leftStatText.Height.Set(50, 0f);
			leftStatText.HAlign = .67f;
			leftStatText.TextOriginX = 0;

			playerClassText = new UIText(string.Empty);
			playerClassText.Top.Set(136, 0f);
			playerClassText.Width.Set(160, 0f);
			playerClassText.HAlign = .5f;

			classStatsText = new UIText(string.Empty, .8f);
			classStatsText.Top.Set(167, 0f);
			classStatsText.Width.Set(160, 0f);
			classStatsText.HAlign = .2f;
			classStatsText.TextOriginX = 0;
			classStatsText.DynamicallyScaleDownToWidth = true;


			mainElement.Append(background);
			mainElement.Append(nameText);
			mainElement.Append(frame);
			mainElement.Append(armorText);
			mainElement.Append(rightStatText);
			mainElement.Append(leftStatText);
			mainElement.Append(accessoriesText);

			if (IsExtended) {
				mainElement.Append(playerClassText);
				mainElement.Append(classStatsText);
			}

			Append(mainElement);
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

				rightStatText.SetText(rightStatTextValue);
				leftStatText.SetText(leftStatTextValue);
				nameText.SetText(ally.name);

				armorText.SetText(
					string.IsNullOrEmpty(armorTextValue)
					? "No armor"
					: armorTextValue
				);

				accessoriesText.SetText(
					string.IsNullOrEmpty(accessoriesTextValue)
					? "No acc-ies"
					: accessoriesTextValue
				);

				if (IsExtended) {
					Util.PlayerClass allyClass = Util.GuessPlayerClass(ally);

					playerClassText.SetText(
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

					classStatsText.SetText(
						allyClass switch {
							Util.PlayerClass.Melee => $"""
								[i:{ItemID.IronBroadsword}] Damage Increase: {GetClassDamage(DamageClass.Melee, ally)}
								[i:{ItemID.PsychoKnife}] Crit. Chance: {(int) ally.GetTotalCritChance(DamageClass.Melee)}
								[i:{ItemID.Arkhalis}] Attack Speed: {(int) (ally.GetTotalAttackSpeed(DamageClass.Melee) * 100)}%
								[i:{ItemID.FleshKnuckles}] Aggro: {ally.aggro}
								""",

							Util.PlayerClass.Ranger => $"""
								[i:{ItemID.IronBow}] Damage Increase: {GetClassDamage(DamageClass.Ranged, ally)}
								[i:{ItemID.SniperRifle}] Crit. Chance: {(int) ally.GetTotalCritChance(DamageClass.Ranged)}
								[i:{ItemID.SharkToothNecklace}] Armor Penetration: {ally.GetArmorPenetration(DamageClass.Generic)}
								""",

							Util.PlayerClass.Mage => $"""
								[i:{ItemID.MagicalHarp}] Damage Increase: {GetClassDamage(DamageClass.Magic, ally)}
								[i:{ItemID.SkyFracture}] Crit. Chance: {(int) ally.GetTotalCritChance(DamageClass.Magic)}
								[i:{ItemID.CrystalBall}] MP Cost Reduction: {Math.Round((1.0 - ally.manaCost) * 100)}%
								""",

							Util.PlayerClass.Summoner => $"""
								[i:{ItemID.StardustCellStaff}] Damage Increase: {GetClassDamage(DamageClass.Summon, ally)}
								[i:{ItemID.MonkAltHead}] Crit. Chance: {(int) ally.GetTotalCritChance(DamageClass.Summon)}
								[i:{ItemID.ImpStaff}] Max Minions: {ally.maxMinions}
								[i:{ItemID.DD2BallistraTowerT1Popper}] Max Centries: {ally.maxTurrets}
								""",

							Util.PlayerClass.Rogue => $"""
								[i:{CrossModHelper.CalamityMod.Find<ModItem>("HeavenfallenStardisk").Type}] Damage Increase: {GetClassDamage(rogueClass, ally)}
								[i:{CrossModHelper.CalamityMod.Find<ModItem>("GleamingDagger").Type}] Crit. Chance: {(int) ally.GetTotalCritChance(rogueClass)}
								""",

							Util.PlayerClass.Bard => $"""
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("ScholarsHarp").Type}] Damage Increase: {GetClassDamage(bardClass, ally)}
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("MixTape").Type}] Crit. Chance: {(int) ally.GetTotalCritChance(bardClass)}
								""",

							Util.PlayerClass.Healer => $"""
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("Liberation").Type}] Damage Increase: {GetClassDamage(healerClass, ally)}
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("DarkGlaze").Type}] Crit. Chance: {(int) ally.GetTotalCritChance(healerClass)}
								[i:{CrossModHelper.ThoriumMod.Find<ModItem>("Twinkle").Type}] Bonus healing: {CrossModHelper.GetHealerHealBonus(ally)}
								""",

							_ => ""
						}
					);
				}
			} catch (Exception e) {
				nameText.SetText("Error");
				rightStatText.SetText("N/A");
				leftStatText.SetText("N/A");
				armorText.SetText("N/A");
				accessoriesText.SetText("N/A");

				Util.CreateErrorMessage("AllyInfoPanel", e);
			}
		}

		internal static int GetClassDamage(DamageClass damageClass, Player player)
			=> (int) Math.Round((player.GetTotalDamage(damageClass).Additive * player.GetTotalDamage(damageClass).Multiplicative * 100) - 100);
	}
}
