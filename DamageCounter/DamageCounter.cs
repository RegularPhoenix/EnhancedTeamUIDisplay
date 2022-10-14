using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace EnhancedTeamUIDisplay.DamageCounter
{
	internal class DamageCounterSystem : ModSystem
	{
		internal enum DamageCounterPacketType : byte
		{
			InformServerOfDPS,
			InformClientsOfDPS
		}

		public override void PostUpdateWorld()
		{
			var netMessage = Mod.GetPacket();
			netMessage.Write((byte)DamageCounterPacketType.InformClientsOfDPS);

			byte count = 0;
			for (int i = 0; i < 256; i++) if (Main.player[i].active && Main.player[i].accDreamCatcher) count++;
			netMessage.Write(count);

			for (int i = 0; i < 256; i++)
			{
				if (Main.player[i].active && Main.player[i].accDreamCatcher)
				{
					netMessage.Write((byte)i);
					netMessage.Write(ETUD.DPSValues[i]);
				}
			}

			netMessage.Send();
		}
	}

	internal class DamageCounterPlayer : ModPlayer
	{
		public override void PostUpdate()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer && Player.accDreamCatcher)
			{
				int DPS = Player.dpsStarted ? Player.getDPS() : 0;

				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)DamageCounterSystem.DamageCounterPacketType.InformServerOfDPS);
				packet.Write(DPS);
				packet.Send();
			}
		}
	}

	internal class DamageCounterUI : UIElement
	{
		internal const int width = 200;
		internal const int height = 120;

		internal const int BarWidth = 188;
		internal const int BarHeight = 24;

		private UIText[] UITexts;
		private UIImage image;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;
			Left.Set(ETUDPlayer.PanelLeftOffset - 400, 0f);
			Top.Set(ETUDPlayer.PanelTopOffset, 0f);

			image = new(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/DamageCounter/DamageCounterPanelTop"));
			Append(image);

			UITexts = new UIText[] { new UIText("", .8f), new UIText("", .8f), new UIText("", .8f), new UIText("", .8f) };
			for(int i = 0; i < 4; i++)
			{			
				UITexts[i].Width.Set(BarWidth, 0f);
				UITexts[i].Height.Set(BarHeight, 0f);
				UITexts[i].Top.Set(11 + ((BarHeight + 4) * i), 0f);
				UITexts[i].Left.Set(6, 0f);
				UITexts[i].HAlign = 1;
				Append(UITexts[i]);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			int PlayersCount = ETUD.DPSValues.Count(x => x != -1);

			if (PlayersCount == 0) return;

			int[][] playersDPSValues = new int[PlayersCount][];

			for (int i = 0; i < 4; i++) UITexts[i].SetText("");

			int c = 0;
			for (int i = 0; i < 256; i++) if (ETUD.DPSValues[i] != -1) { playersDPSValues[c] = new int[] { i, ETUD.DPSValues[i] }; c++; }

			Array.Sort(playersDPSValues, (x, y) => Comparer<int>.Default.Compare(y[1], x[1]));
			int HighestDPS = playersDPSValues[0][1];

			if (HighestDPS <= 0) { spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/DamageCounter/DamageCounterPanelBottom").Value, new Rectangle((int)Left.Pixels, (int)Top.Pixels + 26, 200, 10), Color.White); return; }

			

			Rectangle DPSBar = GetInnerDimensions().ToRectangle();
			DPSBar.X += 6;
			DPSBar.Width = BarWidth;
			DPSBar.Y += 6;
			DPSBar.Height = BarHeight;
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, DPSBar, MiscEventHandler.GetClassColours(MiscEventHandler.DeterminePlayerClass(Main.player[playersDPSValues[0][0]]))[0]);
			
			UITexts[0].SetText($"{Main.player[playersDPSValues[0][0]].name}({HighestDPS})");

			int PlayerCountToDraw = new int[2] { 4, PlayersCount }.Min();
			
			if (PlayerCountToDraw > 1)
				for (int i = 1; i < PlayerCountToDraw; i++)
				{
					DPSBar.Y += DPSBar.Height + 4;
					float currentPlayerDPS = playersDPSValues[i][1];
					spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(DPSBar.X, DPSBar.Y, currentPlayerDPS != 0 ? (int)(DPSBar.Width * (currentPlayerDPS / HighestDPS)) : 1, DPSBar.Height), MiscEventHandler.GetClassColours(MiscEventHandler.DeterminePlayerClass(Main.player[playersDPSValues[i][0]]))[0]);

					UITexts[i].SetText($"{Main.player[playersDPSValues[i][0]].name}({currentPlayerDPS})");

					spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/DamageCounter/DamageCounterPanelMid").Value, new Rectangle(DPSBar.X - 6, DPSBar.Y, 200, 28), Color.White);
				}

			spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/DamageCounter/DamageCounterPanelBottom").Value, new Rectangle(DPSBar.X - 6, DPSBar.Y + 20, 200, 10), Color.White);
		}
	}
}
