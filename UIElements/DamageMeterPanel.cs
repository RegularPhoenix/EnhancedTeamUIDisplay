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
	internal class DamageMeterPanel : DraggableUIElement
	{
		internal const int ElementWidth = 200, ElementHeight = 120;
		internal const int BarWidth = 188, BarHeight = 24;

		private byte _statNum = 0;

		private List<UIText> _barTexts;
		private UIText _statTitleText;
		private UIImage _frameTop;
		private UIImageButton _leftButton, _rightButton, _resetButton;

		private ETUDPlayer _player;

		public override void OnInitialize() {
			Width.Pixels = ElementWidth;
			Height.Pixels = ElementHeight;

			_frameTop = new(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/FrameTop"));
			Append(_frameTop);

			_statTitleText = new UIText(string.Empty, .8f);
			_statTitleText.Width.Set(120, 0);
			_statTitleText.Height.Set(20, 0);
			_statTitleText.Top.Set(4, 0f);
			_statTitleText.Left.Set(12, 0f);
			_statTitleText.TextOriginX = 0;
			_statTitleText.TextOriginY = .5f;
			Append(_statTitleText);

			_leftButton = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/ArrowLeft"));
			_leftButton.Width.Set(14, 0f);
			_leftButton.Height.Set(20, 0f);
			_leftButton.Top.Set(4, 0f);
			_leftButton.Left.Set(150, 0f);
			_leftButton.OnLeftClick += (e, l) => {
				if (_statNum == 0) {
					_statNum = 3;
				} else {
					_statNum--;
				}
			};
			Append(_leftButton);

			_rightButton = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/ArrowRight"));
			_rightButton.Width.Set(14, 0f);
			_rightButton.Height.Set(20, 0f);
			_rightButton.Top.Set(4, 0f);
			_rightButton.Left.Set(164, 0f);
			_rightButton.OnLeftClick += (e, l) => {
				if (_statNum == 3) {
					_statNum = 0;
				} else {
					_statNum++;
				}
			};
			Append(_rightButton);

			_resetButton = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/X"));
			_resetButton.Width.Set(14, 0f);
			_resetButton.Height.Set(20, 0f);
			_resetButton.Top.Set(4, 0f);
			_resetButton.Left.Set(178, 0f);
			_resetButton.OnLeftClick += (e, l) => Main.LocalPlayer.GetModPlayer<DamageMeterPlayer>().ResetTables();
			Append(_resetButton);

			_barTexts = new();

			for (int i = 0; i < 4; i++) {
				UIText barText = new(string.Empty, .8f);
				barText.Width.Set(180, 0);
				barText.Height.Set(22, 0);
				barText.Top.Set(28 + ((BarHeight + 4) * i), 0f);
				barText.Left.Set(10, 0f);
				barText.TextOriginX = 1;
				barText.TextOriginY = .5f;
				_barTexts.Add(barText);
				Append(barText);
			}

			_player = Main.LocalPlayer.GetModPlayer<ETUDPlayer>();

			Left.Set(_player.DamageMeterLeftOffset, 0f);
			Top.Set(_player.DamageMeterTopOffset, 0f);

			IsLocked = _player.IsDamageMeterLocked;
		}

		public override void OnDeactivate()
			=> Main.LocalPlayer.GetModPlayer<ETUDPlayer>().IsDamageMeterLocked = IsLocked;

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			Rectangle Bar = GetInnerDimensions().ToRectangle();
			Bar.X += 6;
			Bar.Width = BarWidth;
			Bar.Y += 26;
			Bar.Height = BarHeight + 2;

			for (int i = 0; i < 4; i++)
				_barTexts[i].SetText(string.Empty);

			_statTitleText.SetText(_statNum switch {
				0 => Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.DPS"),
				1 => Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.DealtDamage"),
				2 => Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.TakenDamage"),
				3 => Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.Deaths"),
				_ => Language.GetText("Mods.EnhancedTeamUIDisplay.GeneralNouns.Error")
			});

			DamageMeterPlayer damageMeterPlayer = Main.LocalPlayer.GetModPlayer<DamageMeterPlayer>();

			Dictionary<Player, int> statValues = new();

			int[] sourceValues = _statNum switch {
				0 => damageMeterPlayer.DPSTable,
				1 => damageMeterPlayer.DealtDamageTable,
				2 => damageMeterPlayer.TakenDamageTable,
				3 => damageMeterPlayer.DeathsTable,
				_ => null
			};

			for (int i = 0; i < 256; i++) {
				if (sourceValues[i] == -1 || !Main.player[i].active) // TODO: Show offline players option?
					continue;

				statValues.Add(Main.player[i], sourceValues[i]);
			}

			statValues = statValues.OrderByDescending(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

			if (statValues.Count == 0) {
				spriteBatch.Draw(
					ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/FrameBottom").Value,
					new Rectangle(Bar.X - 6, Bar.Y + 27, 200, 4),
					Color.White
				);
				_barTexts[0].SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.DamageMeter.NoData"));
				return;
			}

			Player bestPlayer = statValues.Keys.ElementAt(0);
			int highestValue = statValues.Values.ElementAt(0);

			spriteBatch.Draw(
				ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/MagicBar").Value,
				Bar,
				Util.GetClassColours(
					Util.GuessPlayerClass(bestPlayer)
				).health
			);

			_barTexts[0].SetText($"{(bestPlayer.name.Length > 15 ? bestPlayer.name[..12] + "..." : bestPlayer.name)} ({highestValue})");

			int playerCountToDraw = new int[2] {
				Config.Instanse.DamageMeterMaxPlayerCount,
				statValues.Count
			}.Min();

			for (int i = 1; i < playerCountToDraw; i++) {
				float currentValue = statValues.Values.ElementAt(i);
				Player currentPlayer = statValues.Keys.ElementAt(i);

				Bar.Y += Bar.Height + 2;

				spriteBatch.Draw(
					ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/DamageMeter/Background").Value,
					new Rectangle(Bar.X - 6, Bar.Y, 200, 28),
					Color.White
				);

				spriteBatch.Draw(
					ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/MagicBar").Value,
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

				_barTexts[i].SetText($"{(currentPlayer.name.Length > 15 ? currentPlayer.name[..12] + "..." : currentPlayer.name)} ({currentValue})");

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

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			_player.DamageMeterLeftOffset = (int) Left.Pixels;
			_player.DamageMeterTopOffset = (int) Top.Pixels;

			if (IsLocked
				&& ContainsPoint(Main.MouseScreen)
				&& !_leftButton.ContainsPoint(Main.MouseScreen)
				&& !_rightButton.ContainsPoint(Main.MouseScreen)
				&& !_resetButton.ContainsPoint(Main.MouseScreen)
			) {
				Main.LocalPlayer.mouseInterface = false;
			}
		}

		public override void DragEnd(UIMouseEvent evt) {
			base.DragEnd(evt);

			_player.DamageMeterLeftOffset = (int) Left.Pixels;
			_player.DamageMeterTopOffset = (int) Top.Pixels;
		}
	}
}
