using Microsoft.Xna.Framework;
using Terraria.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDAllyInfoPanel : UIElement
	{
		internal const int width = 232;
		internal const int height = 264;

		public static Player Ally;
		public static float GetLeft;
		public static float GetTop;
		public static bool extended;

		private UIElement MainElement;
		private UIImage panel;
		private UIImage BG;
		private UIText NameText;
		private UIText ArmorText;
		private UIText AccessoryText;
		private UIText StatTextR;
		private UIText StatTextL;

		private UIText playerClassText;
		private UIText statClassText;

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
			panel.Left.Set(0, 0f);
			panel.Top.Set(0, 0f);
			panel.Width.Set(232, 0f);
			panel.Height.Set(264, 0f);

			ArmorText = new UIText("");
			ArmorText.Left.Set(10, 0f);
			ArmorText.Top.Set(35, 0f);
			ArmorText.Width.Set(20, 0f);
			ArmorText.Height.Set(50, 0f);

			AccessoryText = new UIText("");
			AccessoryText.Left.Set(14, 0f);
			AccessoryText.Top.Set(72, 0f);
			AccessoryText.Width.Set(50, 0f);
			AccessoryText.Height.Set(50, 0f);

			NameText = new UIText("");
			NameText.Left.Set(0, 0f);
			NameText.Top.Set(8, 0f);
			NameText.Width.Set(50, 0f);
			NameText.Height.Set(50, 0f);
			NameText.HAlign = .5f;

			StatTextR = new UIText("");
			StatTextR.Left.Set(122, 0f);
			StatTextR.Top.Set(37, 0f);
			StatTextR.Width.Set(232, 0f);
			StatTextR.Height.Set(50, 0f);

			StatTextL = new UIText("");
			StatTextL.Left.Set(175, 0f);
			StatTextL.Top.Set(37, 0f);
			StatTextL.Width.Set(50, 0f);
			StatTextL.Height.Set(50, 0f);

			playerClassText = new UIText("");
			playerClassText.Left.Set(0, 0f);
			playerClassText.Top.Set(136, 0f);
			playerClassText.Width.Set(232, 0f);
			playerClassText.Height.Set(50, 0f);
			playerClassText.HAlign = .5f;

			statClassText = new UIText("");
			statClassText.Left.Set(10, 0f);
			statClassText.Top.Set(167, 0f);
			statClassText.Width.Set(50, 0f);
			statClassText.Height.Set(50, 0f);


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

				StatTextRValue += $"[i:{ItemID.LifeCrystal}]{Ally.statLifeMax2}\n[i:{ItemID.CobaltShield}]{Ally.statDefense}\n[i:{ItemID.HermesBoots}]{(int)((Ally.accRunSpeed + Ally.maxRunSpeed) / 2f * Ally.moveSpeed * 6)}";
				StatTextLValue += $"[i:{ItemID.RegenerationPotion}]{Ally.lifeRegen / 2}\n[i:{ItemID.PaladinsShield}]{(int)(Ally.endurance * 100)}\n[i:{ItemID.LeafWings}]{(Math.Round(Ally.wingTimeMax / 60.0, 2) <= 0 ? Math.Round(Ally.wingTimeMax / 60.0, 2) : "N/A")}";

				StatTextR.SetText(StatTextRValue);
				StatTextL.SetText(StatTextLValue);
				NameText.SetText(Ally.name);
				ArmorText.SetText(ArmorTextValue != "" ? ArmorTextValue : "No armor");
				AccessoryText.SetText(AccessoriesTextValue != "" ? AccessoriesTextValue : "No acc-ies");

				if (extended)
				{
					string AllyClass = MiscEventHandler.DeterminePlayerClass(Ally);

					playerClassText.SetText((AllyClass == "None" || string.IsNullOrEmpty(AllyClass)) ? "No class" : AllyClass);

					switch (AllyClass)
					{
						case "Melee":
							statClassText.SetText($"[i:{ItemID.IronBroadsword}] Damage: {GetClassDamage(DamageClass.Melee, Ally)}\nCrit. Chance: {(int)Ally.GetTotalCritChance(DamageClass.Melee)}\n[i:{ItemID.IronBroadsword}] Attack Speed: {1f / Ally.GetTotalAttackSpeed(DamageClass.Melee) * 100}%\n[i:{ItemID.FleshKnuckles}] Aggro: {Ally.aggro}");
							break;
						case "Ranged":
							statClassText.SetText($"[i:{ItemID.IronBow}] Damage: {GetClassDamage(DamageClass.Ranged, Ally)}\nCrit. Chance: {(int)Ally.GetTotalCritChance(DamageClass.Ranged)}\n[i:{ItemID.SharkToothNecklace}] Armor Penetr.:{Ally.GetArmorPenetration(DamageClass.Generic)}");
							break;
						case "Magic":
							statClassText.SetText($"[i:{ItemID.MagicalHarp}] Damage: {GetClassDamage(DamageClass.Magic, Ally)}\nCrit. Chance: {(int)Ally.GetTotalCritChance(DamageClass.Magic)}\n[i:{ItemID.CrystalBall}] MP Cost Reduct.: {Math.Round((1.0 - Ally.manaCost) * 100)}%");
							break;
						case "Summon":
							statClassText.SetText($"[i:{ItemID.ImpStaff}] Damage: {GetClassDamage(DamageClass.Summon, Ally)}\nCrit. Chance: {(int)Ally.GetTotalCritChance(DamageClass.Summon)}\n[i:{ItemID.ImpStaff}] Max Minions: {Ally.maxMinions}\n[i:{ItemID.DD2BallistraTowerT1Popper}] Max Centries: {Ally.maxTurrets}");
							break;
						case "Rogue":
							if (ETUD.CalamityMod.TryFind<DamageClass>("RogueDamageClass", out var rogueclass))
								statClassText.SetText($"[i:{ItemID.IronBroadsword}] Damage: {GetClassDamage(rogueclass, Ally)}\nCrit. Chance: {(int)Ally.GetTotalCritChance(rogueclass)}");
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
			catch(Exception e)
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