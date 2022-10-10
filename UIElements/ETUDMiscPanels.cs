using Microsoft.Xna.Framework;
using Terraria.UI;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using Terraria.IO;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDAllyInfoPanel : UIElement
	{
		internal const int width = 232;
		internal const int height = 136;

		public static Player Ally;
		public static float GetLeft;
		public static float GetTop;
		public static bool extended;

 		//private Rectangle defaultFrame = new Rectangle(0, 0, 40, 56);
		//private Vector2 drawCenter = new Vector2(22f, 18f);
		//private SpriteEffects spriteDirection = SpriteEffects.None;

		private UIElement MainElement;
		private UIImage panel;
		private UIImage BG;
		private UIText NameText;
		private UIText ArmorText;
		private UIText AccessoryText;
		private UIText StatText;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			MainElement = new UIElement();
			MainElement.Left.Set(0, 0f);
			MainElement.Top.Set(0, 0f);
			MainElement.Width.Set(232, 0f);
			MainElement.Height.Set(136, 0f);

			BG = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanelBG"));
			BG.Left.Set(0, 0f);
			BG.Top.Set(0, 0f);
			BG.Width.Set(232, 0f);
			BG.Height.Set(236, 0f);

			panel = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatPanel"));
			panel.Left.Set(0, 0f);
			panel.Top.Set(0, 0f);
			panel.Width.Set(232, 0f);
			panel.Height.Set(136, 0f);

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
			NameText.Left.Set(10, 0f);
			NameText.Top.Set(8, 0f);
			NameText.Width.Set(50, 0f);
			NameText.Height.Set(50, 0f);

			StatText = new UIText("");
			StatText.Left.Set(120, 0f);
			StatText.Top.Set(35, 0f);
			StatText.Width.Set(50, 0f);
			StatText.Height.Set(50, 0f);

			MainElement.Append(BG);
			MainElement.Append(NameText);
			MainElement.Append(panel);		
			MainElement.Append(ArmorText);
			MainElement.Append(StatText);
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
			string StatTextValue = "";

			int currentAcc = 0;

			try
			{
				for (int i = 0; i < 10; i++) Equipment[i] = Ally.armor[i].type;
				for (int i = 0; i < 3; i++) ArmorTextValue += Equipment[i] != 0 ? $"[i:{Equipment[i]}]" : "";
				for (int i = 3; i < 10; i++) { if (Equipment[i] != 0) { AccessoriesTextValue += $"[i:{Equipment[i]}]"; currentAcc++; if (currentAcc == 4) AccessoriesTextValue += "\n"; } }

				StatTextValue += $"[i:{ItemID.LifeCrystal}]" + Ally.statLifeMax2 + $" [i:{ItemID.RegenerationPotion}]" + Ally.lifeRegen / 2  + $"\n[i:{ItemID.CobaltShield}]" + Ally.statDefense + $" [i:{ItemID.PaladinsShield}]" + (int)(Ally.endurance * 100) + $"\n[i:{ItemID.HermesBoots}]" + (int)((Ally.accRunSpeed + Ally.maxRunSpeed) / 2f * Ally.moveSpeed * 6) + $" [i:{ItemID.LeafWings}]" + (Math.Round(Ally.wingTimeMax / 60.0, 2) <= 0 ? Math.Round(Ally.wingTimeMax / 60.0, 2) : "N/A");

				StatText.SetText(StatTextValue);
				NameText.SetText(Ally.name);
				ArmorText.SetText(ArmorTextValue != "" ? ArmorTextValue : "No armor");
				AccessoryText.SetText(AccessoriesTextValue != "" ? AccessoriesTextValue : "No acc-ies");
			}
			catch(Exception e)
			{
				StatText.SetText("N/A");
				NameText.SetText("Error");
				ArmorText.SetText("N/A");
				AccessoryText.SetText("N/A");
				
				ETUDAdditionalOptions.CreateErrorMessage("MiscPanels", e);
			}

			/*if (extended)
			{
				try
				{
					
				}
				catch
				{

				}
			}*/
		}

		/*protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			Player Ally = Main.LocalPlayer;

			Vector2 drawPos = new Vector2(250f, 250f);
			int skinVariant = Ally.skinVariant;

			//colors
			Color eyeColor = Ally.eyeColor;
			Color hairColor = Ally.hairColor;
			Color eyeWhitesColor = Color.White;
			Color skinColor = Ally.skinColor;
			Color shirtColor = Ally.shirtColor;
			Color underShirtColor = Ally.underShirtColor;
			Color pantsColor = Ally.pantsColor;
			Color shoeColor = Ally.shoeColor;

			if (Ally.wings > 0) spriteBatch.Draw(TextureAssets.Wings[Ally.wings].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);
			if (Ally.back > 0) spriteBatch.Draw(TextureAssets.AccBack[Ally.back].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);

			spriteBatch.Draw(TextureAssets.Players[0, 3].Value, drawPos, defaultFrame, skinColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.Players[0, 0].Value, drawPos, defaultFrame, skinColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.Players[0, 1].Value, drawPos, defaultFrame, eyeWhitesColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.Players[0, 2].Value, drawPos, defaultFrame, eyeColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.PlayerHair[0].Value, drawPos, defaultFrame, hairColor, 0f, drawCenter, 1f, spriteDirection, 1f);

			if (Ally.face > 0) spriteBatch.Draw(TextureAssets.AccFace[Ally.face].Value, drawPos, defaultFrame, skinColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			if (skinVariant == 4 || skinVariant == 9) spriteBatch.Draw(TextureAssets.Players[4, 10].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);
			if (skinVariant == 8) spriteBatch.Draw(TextureAssets.Players[0, 10].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);

			spriteBatch.Draw(TextureAssets.Players[skinVariant, 4].Value, drawPos, defaultFrame, underShirtColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.Players[skinVariant, 5].Value, drawPos, defaultFrame, skinColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.Players[skinVariant, 6].Value, drawPos, defaultFrame, shirtColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.Players[0, 7].Value, drawPos, defaultFrame, skinColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.Players[skinVariant, 8].Value, drawPos, defaultFrame, underShirtColor, 0f, drawCenter, 1f, spriteDirection, 1f);

			if (Ally.shield > 0) spriteBatch.Draw(TextureAssets.AccShield[Ally.shield].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);

			spriteBatch.Draw(TextureAssets.Players[skinVariant, 11].Value, drawPos, defaultFrame, pantsColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			spriteBatch.Draw(TextureAssets.Players[skinVariant, 12].Value, drawPos, defaultFrame, shoeColor, 0f, drawCenter, 1f, spriteDirection, 1f);

			if (Ally.shoe > 0) spriteBatch.Draw(TextureAssets.AccShoes[Ally.shoe].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);
			if (Ally.handoff > 0) spriteBatch.Draw(TextureAssets.AccHandsOff[Ally.handoff].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);
			if (Ally.neck > 0) spriteBatch.Draw(TextureAssets.AccNeck[Ally.neck].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);
			if (Ally.waist > 0) spriteBatch.Draw(TextureAssets.AccWaist[Ally.waist].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);

			if (skinVariant == 2 || skinVariant == 3 || skinVariant == 4 || skinVariant == 6 || skinVariant == 7 || skinVariant == 8 || skinVariant == 9) {
				spriteBatch.Draw(TextureAssets.Players[skinVariant, 13].Value, drawPos, defaultFrame, shirtColor, 0f, drawCenter, 1f, spriteDirection, 1f);
				if (skinVariant == 3 || skinVariant == 7 || skinVariant == 8)
					spriteBatch.Draw(TextureAssets.Players[skinVariant, 14].Value, drawPos, defaultFrame, shirtColor, 0f, drawCenter, 1f, spriteDirection, 1f);
			}

			if (Ally.handon > 0) spriteBatch.Draw(TextureAssets.AccHandsOn[Ally.handon].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);
			if (Ally.front > 0) spriteBatch.Draw(TextureAssets.AccFront[Ally.front].Value, drawPos, defaultFrame, Color.White, 0f, drawCenter, 1f, spriteDirection, 1f);
		}*/
	}
}
