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

		private UIElement MainElement;
		private UIImageButton button;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			MainElement = new UIElement();
			MainElement.Left.Set(0, 0f);
			MainElement.Top.Set(0, 0f);
			MainElement.Width.Set(20, 0f);
			MainElement.Height.Set(24, 0f);

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

			MainElement.Append(button);
			Append(MainElement);
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

		public static Player Ally;
		public static float GetLeft;
		public static float GetTop;
		public static bool extended;

		private UIElement MainElement;
		private UIImage panel, BG;
		private UIText NameText, ArmorText, AccessoryText, StatTextR, StatTextL;

		private UIText playerClassText, statClassText;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			MainElement = new UIElement();
			MainElement.Left.Set(0, 0f);
			MainElement.Top.Set(0, 0f);
			MainElement.Width.Set(232, 0f);
			MainElement.Height.Set(264, 0f);

			BG = new UIImage(extended ? ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanelExtendedBG") : ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanelBG"));
			BG.Left.Set(0, 0f);
			BG.Top.Set(0, 0f);
			BG.Width.Set(232, 0f);
			BG.Height.Set(264, 0f);

			panel = new UIImage(extended ? ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanelExtended") : ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanel"));
			panel.Width.Set(232, 0f);
			panel.Height.Set(264, 0f);

			ArmorText = new UIText("");
			ArmorText.Top.Set(35, 0f);
			ArmorText.Width.Set(50, 0f);
			ArmorText.HAlign = .15f;
			ArmorText.TextOriginX = 0;

			AccessoryText = new UIText("");
			AccessoryText.Top.Set(72, 0f);
			AccessoryText.Width.Set(50, 0f);
			AccessoryText.HAlign = .10f;
			AccessoryText.TextOriginX = 0;

			NameText = new UIText("");
			NameText.Top.Set(8, 0f);
			NameText.Width.Set(160, 0f);
			NameText.HAlign = .5f;

			StatTextR = new UIText("");
			StatTextR.Top.Set(37, 0f);
			StatTextR.Height.Set(50, 0f);
			StatTextR.HAlign = .92f;
			StatTextR.TextOriginX = 1;

			StatTextL = new UIText("");
			StatTextL.Top.Set(37, 0f);
			StatTextL.Height.Set(50, 0f);
			StatTextL.HAlign = .67f;
			StatTextL.TextOriginX = 0;

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


			MainElement.Append(BG);
			MainElement.Append(NameText);
			if (extended) { MainElement.Append(playerClassText); MainElement.Append(statClassText); }
			MainElement.Append(panel);
			MainElement.Append(ArmorText);
			MainElement.Append(StatTextR);
			MainElement.Append(StatTextL);
			MainElement.Append(AccessoryText);

			Append(MainElement);

			Left.Pixels = GetLeft;
			Top.Pixels = GetTop;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Left.Pixels = GetLeft;
			Top.Pixels = GetTop;

			int[] Equipment = new int[10];
			string ArmorTextValue = "";
			string AccessoriesTextValue = "";
			string StatTextRValue = "";
			string StatTextLValue = "";

			int currentAcc = 0;

			try
			{
				for (int i = 0; i < 10; i++) Equipment[i] = Ally.armor[i].type;
				for (int i = 0; i < 3; i++) ArmorTextValue += Equipment[i] != 0 ? $"[i:{Equipment[i]}]" : "";
				for (int i = 3; i < 10; i++) { if (Equipment[i] != 0) { AccessoriesTextValue += $"[i:{Equipment[i]}]"; currentAcc++; if (currentAcc == 4) AccessoriesTextValue += "\n"; } }

				StatTextLValue += $"[i:{ItemID.LifeCrystal}]{Ally.statLifeMax2}\n[i:{ItemID.CobaltShield}]{Ally.statDefense}\n[i:{ItemID.HermesBoots}]{(int)((Ally.accRunSpeed + Ally.maxRunSpeed) / 2f * Ally.moveSpeed * 6)}";
				StatTextRValue += $"[i:{ItemID.RegenerationPotion}]{Ally.lifeRegen / 2}\n[i:{ItemID.PaladinsShield}]{(int)(Ally.endurance * 100)}\n[i:{ItemID.LeafWings}]{(Math.Round(Ally.wingTimeMax / 60.0, 2) <= 0 ? Math.Round(Ally.wingTimeMax / 60.0, 2) : "-")}";

				StatTextR.SetText(StatTextRValue);
				StatTextL.SetText(StatTextLValue);
				NameText.SetText(Ally.name);
				ArmorText.SetText(ArmorTextValue != "" ? ArmorTextValue : "No armor");
				AccessoryText.SetText(AccessoriesTextValue != "" ? AccessoriesTextValue : "No acc-ies");

				if (extended)
				{
					string AllyClass = MiscEventHandler.DeterminePlayerClass(Ally);
					Mod CMod = ETUD.CalamityMod;

					playerClassText.SetText((AllyClass == "None" || string.IsNullOrEmpty(AllyClass)) ? "No class" : AllyClass);

					switch (AllyClass)
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
							if (CMod is not null && CMod.TryFind<DamageClass>("RogueDamageClass", out var rogueclass))
								statClassText.SetText($"[i:{CMod.Find<ModItem>("HeavenfallenStardisk").Type}] Damage Increase: {GetClassDamage(rogueclass, Ally)}\n[i:{CMod.Find<ModItem>("GleamingDagger").Type}] Crit. Chance: {(int)Ally.GetTotalCritChance(rogueclass)}");
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
				StatTextR.SetText("N/A");
				StatTextL.SetText("N/A");
				NameText.SetText("Error");
				ArmorText.SetText("N/A");
				AccessoryText.SetText("N/A");

				ETUDAdditionalOptions.CreateErrorMessage("MiscPanels", e);
			}
		}

		internal int GetClassDamage(DamageClass damageClass, Player player) => (int)Math.Round(player.GetTotalDamage(damageClass).Additive * player.GetTotalDamage(damageClass).Multiplicative * 100 - 100);
	}
}
