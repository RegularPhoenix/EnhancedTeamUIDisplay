using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace EnhancedTeamUIDisplay.UIElements
{
	internal class DamageMeterPanel : UIElement
	{
		internal const int width = 200, height = 120;
		internal const int barWidth = 188, barHeight = 24;

		internal static byte StatNum = 0;

		private List<UIText> barTexts;
		private UIText statTitleText;
		private UIImage frameTop;
		private UIImageButton leftButton, rightButton, resetButton;

		private ETUDPlayer player;

		public override void OnInitialize() {
			Width.Pixels = width;
			Height.Pixels = height;

			player = Main.LocalPlayer.GetModPlayer<ETUDPlayer>();

			Left.Set(player.DamageMeterLeftOffset, 0f);
			Top.Set(player.DamageMeterTopOffset, 0f);

			frameTop = new(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/FrameTop"));
			Append(frameTop);

			barTexts = [];

			for (int i = 0; i < 4; i++) {
				UIText barText = new(string.Empty, .8f);
				barText.Top.Set(32 + ((barHeight + 4) * i), 0f);
				barText.Width.Set(140, 0);
				barText.HAlign = .86f;
				barText.TextOriginX = 1;
				barTexts.Add(barText);
				Append(barText);
			}

			statTitleText = new UIText(string.Empty, .8f);
			statTitleText.Top.Set(8, 0f);
			statTitleText.Width.Set(120, 0);
			statTitleText.HAlign = .13f;
			statTitleText.TextOriginX = 0;
			Append(statTitleText);

			leftButton = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/ArrowLeft"));
			leftButton.Width.Set(18, 0f);
			leftButton.Height.Set(22, 0f);
			leftButton.HAlign = .75f;
			leftButton.Top.Set(4, 0f);
			leftButton.OnLeftClick += (e, l) => {
				if (StatNum == 0)
					StatNum = 3;
				else
					StatNum--;
			};
			Append(leftButton);

			rightButton = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/ArrowRight"));
			rightButton.Width.Set(18, 0f);
			rightButton.Height.Set(22, 0f);
			rightButton.HAlign = .85f;
			rightButton.Top.Set(4, 0f);
			rightButton.OnLeftClick += (e, l) => {
				if (StatNum == 3)
					StatNum = 0;
				else
					StatNum++;
			};
			Append(rightButton);

			resetButton = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/X"));
			resetButton.Width.Set(18, 0f);
			resetButton.Height.Set(22, 0f);
			resetButton.HAlign = .95f;
			resetButton.Top.Set(4, 0f);
			resetButton.OnLeftClick += (e, l) => Main.LocalPlayer.GetModPlayer<DamageMeterPlayer>().ResetTables();
			Append(resetButton);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			Rectangle Bar = GetInnerDimensions().ToRectangle();
			Bar.X += 6;
			Bar.Width = barWidth;
			Bar.Y += 26;
			Bar.Height = barHeight + 2;

			for (int i = 0; i < 4; i++)
				barTexts[i].SetText(string.Empty);

			statTitleText.SetText(StatNum switch {
				0 => Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.DPS"),
				1 => Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.DealtDamage"),
				2 => Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.TakenDamage"),
				3 => Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.Deaths"),
				_ => Language.GetText("Mods.EnhancedTeamUIDisplay.GeneralNouns.Error")
			});

			DamageMeterPlayer damageMeterPlayer = Main.LocalPlayer.GetModPlayer<DamageMeterPlayer>();

			Dictionary<int, int> statValues = [];

			int[] sourceValues = StatNum switch {
				0 => damageMeterPlayer.DPSTable,
				1 => damageMeterPlayer.DealtDamageTable,
				2 => damageMeterPlayer.TakenDamageTable,
				3 => damageMeterPlayer.DeathsTable,
				_ => null
			};

			for (int i = 0; i < 256; i++) {
				if (sourceValues[i] == -1)
					continue;

				statValues.Add(i, sourceValues[i]);
			}

			statValues = statValues.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

			if (statValues.Count == 0) {
				spriteBatch.Draw(
					ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/FrameBottom").Value,
					new Rectangle(Bar.X - 6, Bar.Y + 27, 200, 4),
					Color.White
				);
				barTexts[0].SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.NoData"));
				return;
			}

			Player bestPlayer = Main.player[statValues.Keys.ElementAt(0)];
			int highestValue = statValues.Values.ElementAt(0);

			spriteBatch.Draw(
				TextureAssets.MagicPixel.Value,
				Bar,
				Util.GetClassColours(
					Util.GuessPlayerClass(bestPlayer)
				).health
			);

			barTexts[0].SetText($"{bestPlayer.name}({highestValue})");

			int playerCountToDraw = new int[2] {
				Config.Instanse.DamageMeterPlayerCountToShow,
				statValues.Count
			}.Min();

			for (int i = 1; i < playerCountToDraw; i++) {
				float currentValue = statValues.Values.ElementAt(i);
				Player currentPlayer = Main.player[statValues.Keys.ElementAt(i)];

				Bar.Y += Bar.Height + 2;

				spriteBatch.Draw(
					ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/Background").Value,
					new Rectangle(Bar.X - 6, Bar.Y, 200, 28),
					Color.White
				);

				spriteBatch.Draw(
					TextureAssets.MagicPixel.Value,
					new Rectangle(
						Bar.X,
						Bar.Y,
						currentValue != 0
						? (int) (Bar.Width * (currentValue / highestValue))
						: 1,
						Bar.Height
					),
					Util.GetClassColours(
						Util.GuessPlayerClass(currentPlayer)
					).health
				);

				barTexts[i].SetText($"{currentPlayer.name}({currentValue})");

				spriteBatch.Draw(
					ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/FrameMid").Value,
					new Rectangle(Bar.X - 6, Bar.Y, 200, 28),
					Color.White
				);
			}

			spriteBatch.Draw(
				ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/FrameBottom").Value,
				new Rectangle(Bar.X - 6, Bar.Y + 27, 200, 4),
				Color.White
			);
		}

		private Vector2 offset;
		public bool dragging;

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			player.DamageMeterLeftOffset = (int) Left.Pixels;
			player.DamageMeterTopOffset = (int) Top.Pixels;

			if (ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}

			if (dragging) {
				Left.Set(Main.mouseX - offset.X, 0f);
				Top.Set(Main.mouseY - offset.Y, 0f);
				Recalculate();
			}

			Rectangle parentSpace = Parent.GetDimensions().ToRectangle();
			if (!GetDimensions().ToRectangle().Intersects(parentSpace)) {
				Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
				Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
				Recalculate();
			}
		}

		public override void LeftMouseDown(UIMouseEvent evt) {
			base.LeftMouseDown(evt);
			DragStart(evt);
		}

		public override void LeftMouseUp(UIMouseEvent evt) {
			base.LeftMouseUp(evt);
			DragEnd(evt);
		}

		private void DragStart(UIMouseEvent evt) {
			offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
			dragging = true;
		}

		private void DragEnd(UIMouseEvent evt) {
			Vector2 end = evt.MousePosition;
			dragging = false;

			Left.Set(end.X - offset.X, 0f);
			Top.Set(end.Y - offset.Y, 0f);

			player.DamageMeterLeftOffset = (int) Left.Pixels;
			player.DamageMeterTopOffset = (int) Top.Pixels;

			Recalculate();
		}
	}
}
