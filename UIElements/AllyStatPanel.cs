using System;
using Terraria;
using Terraria.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EnhancedTeamUIDisplay
{
	internal class AllyInfoButton : UIElement
	{
		internal const int width = 20;
		internal const int height = 24;

		private UIElement mainElement;
		private UIImageButton button;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			mainElement = new UIElement();
			mainElement.Left.Set(0, 0f);
			mainElement.Top.Set(0, 0f);
			mainElement.Width.Set(20, 0f);
			mainElement.Height.Set(24, 0f);

			//Button
			button = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatButton"));
			button.Left.Set(0, 0f);
			button.Top.Set(0, 0f);
			button.Width.Set(20, 0f);
			button.Height.Set(24, 0f);
			button.OnMouseDown += (e, l) => { ETUDUISystem.CloseAllyStatScreen(); ETUDAllyInfoPanel.extended = true; ETUDUISystem.OpenAllyStatScreen(); };
			button.OnMouseUp += (e, l) => { ETUDUISystem.CloseAllyStatScreen(); ETUDAllyInfoPanel.extended = false; ETUDUISystem.OpenAllyStatScreen(); };
			button.OnMouseOver += (e, l) => OnMouseSelect(e, l);
			button.OnMouseOut += (e, l) => OnMouseDeselect(e, l);

			mainElement.Append(button);
			Append(mainElement);
		}

		internal virtual void OnMouseSelect(UIMouseEvent evt, UIElement listeningElement) { if (ETUDUISystem.ETUDAllyStatScreen.CurrentState is null) { ETUDUISystem.OpenAllyStatScreen(); } ETUDAllyInfoPanel.GetLeft = Left.Pixels - ETUDAllyInfoPanel.width; ETUDAllyInfoPanel.GetTop = Top.Pixels; }

		internal virtual void OnMouseDeselect(UIMouseEvent evt, UIElement listeningElement) { if (ETUDUISystem.ETUDAllyStatScreen.CurrentState is not null) ETUDUISystem.CloseAllyStatScreen(); }
	}

	internal class AllyInfoButton1 : AllyInfoButton
	{
		internal override void OnMouseSelect(UIMouseEvent evt, UIElement listeningElement)
		{
			if (ETUDPanel1.Ally is null) return;
			ETUDAllyInfoPanel.Ally = ETUDPanel1.Ally;
			base.OnMouseSelect(evt, listeningElement);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (ETUDPanel1.Ally is null) return;

			base.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			if (ETUDPanel1.Ally is null) IgnoresMouseInteraction = true; else IgnoresMouseInteraction = false;

			base.Update(gameTime);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels - width - 10;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + 45;
		}
	}

	internal class AllyInfoButton2 : AllyInfoButton
	{
		internal override void OnMouseSelect(UIMouseEvent evt, UIElement listeningElement)
		{
			if (ETUDPanel2.Ally is null) return;
			ETUDAllyInfoPanel.Ally = ETUDPanel2.Ally;
			base.OnMouseSelect(evt, listeningElement);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (ETUDPanel2.Ally is null || (ETUDConfig.Instanse.PanelAmount != "Two panels" && ETUDConfig.Instanse.PanelAmount != "Three panels")) return;

			base.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			if (ETUDPanel2.Ally is null) IgnoresMouseInteraction = true; else IgnoresMouseInteraction = false;

			base.Update(gameTime);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels - width - 10;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + 45 + 70;
		}
	}

	internal class AllyInfoButton3 : AllyInfoButton
	{
		internal override void OnMouseSelect(UIMouseEvent evt, UIElement listeningElement)
		{
			if (ETUDPanel3.Ally is null) return;
			ETUDAllyInfoPanel.Ally = ETUDPanel3.Ally;
			base.OnMouseSelect(evt, listeningElement);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (ETUDPanel3.Ally is null || ETUDConfig.Instanse.PanelAmount != "Three panels") return;

			base.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			if (ETUDPanel3.Ally is null) IgnoresMouseInteraction = true; else IgnoresMouseInteraction = false;

			base.Update(gameTime);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels - width - 10;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + 45 + 140;
		}
	}

	internal class ETUDAllyInfoPanel : UIElement
	{
		internal const int width = 232;
		internal const int height = 264;

		internal static Player Ally;
		internal static float GetLeft;
		internal static float GetTop;
		internal static bool extended;

		private UIElement mainElement;
		private UIImage panel, BG;
		private UIText nameText, armorText, accessoryText, statTextR, statTextL;

		private UIText playerClassText, statClassText;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			mainElement = new UIElement();
			mainElement.Left.Set(0, 0f);
			mainElement.Top.Set(0, 0f);
			mainElement.Width.Set(232, 0f);
			mainElement.Height.Set(264, 0f);

			BG = new UIImage(extended ? ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanelExtendedBG") : ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanelBG"));
			BG.Left.Set(0, 0f);
			BG.Top.Set(0, 0f);
			BG.Width.Set(232, 0f);
			BG.Height.Set(264, 0f);

			panel = new UIImage(extended ? ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanelExtended") : ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanel"));
			panel.Width.Set(232, 0f);
			panel.Height.Set(264, 0f);

			armorText = new UIText("");
			armorText.Top.Set(35, 0f);
			armorText.Width.Set(50, 0f);
			armorText.HAlign = .15f;
			armorText.TextOriginX = 0;
			
			accessoryText = new UIText("");
			accessoryText.Top.Set(72, 0f);
			accessoryText.Width.Set(50, 0f);
			accessoryText.HAlign = .10f;
			accessoryText.TextOriginX = 0;

			nameText = new UIText("");
			nameText.Top.Set(8, 0f);
			nameText.Width.Set(160, 0f);
			nameText.HAlign = .5f;

			statTextR = new UIText("");
			statTextR.Top.Set(37, 0f);
			statTextR.Height.Set(50, 0f);
			statTextR.HAlign = .92f;
			statTextR.TextOriginX = 1;
			
			statTextL = new UIText("");
			statTextL.Top.Set(37, 0f);
			statTextL.Height.Set(50, 0f);
			statTextL.HAlign = .67f;
			statTextL.TextOriginX = 0;

			playerClassText = new UIText("");
			playerClassText.Top.Set(136, 0f);
			playerClassText.Width.Set(160, 0f);
			playerClassText.HAlign = .5f;

			statClassText = new UIText("", .8f);
			statClassText.Top.Set(167, 0f);
			statClassText.Width.Set(160, 0f);
			statClassText.HAlign = .2f;
			statClassText.TextOriginX = 0;
			statClassText.DynamicallyScaleDownToWidth = true;


			mainElement.Append(BG);
			mainElement.Append(nameText);
			if (extended) { mainElement.Append(playerClassText); mainElement.Append(statClassText); }
			mainElement.Append(panel);
			mainElement.Append(armorText);
			mainElement.Append(statTextR);
			mainElement.Append(statTextL);
			mainElement.Append(accessoryText);

			Append(mainElement);

			Left.Pixels = GetLeft;
			Top.Pixels = GetTop;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Left.Pixels = GetLeft;
			Top.Pixels = GetTop;

			int[] equipment = new int[10];
			string armorTextValue = "";
			string accessoriesTextValue = "";
			string statTextRValue = "";
			string statTextLValue = "";

			int currentAcc = 0;

			try
			{
				for (int i = 0; i < 10; i++) equipment[i] = Ally.armor[i].type;
				for (int i = 0; i < 3; i++) armorTextValue += equipment[i] != 0 ? $"[i:{equipment[i]}]" : "";
				for (int i = 3; i < 10; i++) { if (equipment[i] != 0) { accessoriesTextValue += $"[i:{equipment[i]}]"; currentAcc++; if (currentAcc == 4) accessoriesTextValue += "\n"; } }

				statTextLValue += $"[i:{ItemID.LifeCrystal}]{Ally.statLifeMax2}\n[i:{ItemID.CobaltShield}]{Ally.statDefense}\n[i:{ItemID.HermesBoots}]{(int)((Ally.accRunSpeed + Ally.maxRunSpeed) / 2f * Ally.moveSpeed * 6)}";
				statTextRValue += $"[i:{ItemID.RegenerationPotion}]{Ally.lifeRegen / 2}\n[i:{ItemID.PaladinsShield}]{(int)(Ally.endurance * 100)}\n[i:{ItemID.LeafWings}]{(Math.Round(Ally.wingTimeMax / 60.0, 2) <= 0 ? Math.Round(Ally.wingTimeMax / 60.0, 2) : "-")}";

				statTextR.SetText(statTextRValue);
				statTextL.SetText(statTextLValue);
				nameText.SetText(Ally.name);
				armorText.SetText(armorTextValue != "" ? armorTextValue : "No armor");
				accessoryText.SetText(accessoriesTextValue != "" ? accessoriesTextValue : "No acc-ies");

				if (extended)
				{
					string allyClass = MiscEventHandler.DeterminePlayerClass(Ally);
					Mod calamityMod = ETUD.CalamityMod;

					playerClassText.SetText((allyClass == "None" || string.IsNullOrEmpty(allyClass)) ? "No class" : allyClass);

					switch (allyClass)
					{
						case "Melee":
							statClassText.SetText($"[i:{ItemID.IronBroadsword}] Damage Increase: {GetClassDamage(DamageClass.Melee, Ally)}\n[i:{ItemID.PsychoKnife}] Crit. Chance: {(int)Ally.GetTotalCritChance(DamageClass.Melee)}\n[i:{ItemID.Arkhalis}] Attack Speed: {(int)(Ally.GetTotalAttackSpeed(DamageClass.Melee) * 100)}%\n[i:{ItemID.FleshKnuckles}] Aggro: {Ally.aggro}");
							break;
						case "Ranged":
							statClassText.SetText($"[i:{ItemID.IronBow}] Damage Increase: {GetClassDamage(DamageClass.Ranged, Ally)}\n[i:{ItemID.SniperRifle}] Crit. Chance: {(int)Ally.GetTotalCritChance(DamageClass.Ranged)}\n[i:{ItemID.SharkToothNecklace}] Armor Penetration: {Ally.GetArmorPenetration(DamageClass.Generic)}");
							break;
						case "Magic":
							statClassText.SetText($"[i:{ItemID.MagicalHarp}] Damage Increase: {GetClassDamage(DamageClass.Magic, Ally)}\n[i:{ItemID.SkyFracture}] Crit. Chance: {(int)Ally.GetTotalCritChance(DamageClass.Magic)}\n[i:{ItemID.CrystalBall}] MP Cost Reduction: {Math.Round((1.0 - Ally.manaCost) * 100)}%");
							break;
						case "Summon":
							statClassText.SetText($"[i:{ItemID.StardustCellStaff}] Damage Increase: {GetClassDamage(DamageClass.Summon, Ally)}\n[i:{ItemID.MonkAltHead}] Crit. Chance: {(int)Ally.GetTotalCritChance(DamageClass.Summon)}\n[i:{ItemID.ImpStaff}] Max Minions: {Ally.maxMinions}\n[i:{ItemID.DD2BallistraTowerT1Popper}] Max Centries: {Ally.maxTurrets}");
							break;
						case "Rogue":
							if (calamityMod is not null && calamityMod.TryFind<DamageClass>("RogueDamageClass", out var rogueclass))
								statClassText.SetText($"[i:{calamityMod.Find<ModItem>("HeavenfallenStardisk").Type}] Damage Increase: {GetClassDamage(rogueclass, Ally)}\n[i:{calamityMod.Find<ModItem>("GleamingDagger").Type}] Crit. Chance: {(int)Ally.GetTotalCritChance(rogueclass)}");
							break;
						case "None":
							statClassText.SetText($"");
							break;
						default:
							statClassText.SetText($"");
							break;
					}
				}
			}
			catch (Exception e)
			{
				statTextR.SetText("N/A");
				statTextL.SetText("N/A");
				nameText.SetText("Error");
				armorText.SetText("N/A");
				accessoryText.SetText("N/A");

				ETUDAdditionalOptions.CreateErrorMessage("MiscPanels", e);
			}
		}

		internal int GetClassDamage(DamageClass damageClass, Player player) => (int)Math.Round(player.GetTotalDamage(damageClass).Additive * player.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
	}
}
