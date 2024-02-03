using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace EnhancedTeamUIDisplay.UIElements
{
	internal class AllyInfoButton : UIElement
	{
		internal AllyInfoButton(int number)
			=> PanelNumber = number;

		internal const int ElementWidth = 20, ElementHeight = 24;

		private int PanelNumber { get; }

		private UIImageButton _imageButton;

		private bool _hovered;

		public override void OnInitialize() {
			Width.Pixels = ElementWidth;
			Height.Pixels = ElementHeight;

			_imageButton = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoButton"));
			_imageButton.Width.Set(ElementWidth, 0f);
			_imageButton.Height.Set(ElementHeight, 0f);

			_imageButton.OnMouseOver += OnMouseOverAction;
			_imageButton.OnMouseOut += OnMouseOutAction;

			_imageButton.OnLeftMouseDown += (e, l) => {
				ETUDUI.CloseAllyInfoInterface();
				ETUDUI.OpenAllyInfoInterface(
					PanelNumber,
					true,
					Left.Pixels - ElementWidth - AllyInfoPanel.ElementWidth,
					Top.Pixels + ElementHeight
				);
			};

			_imageButton.OnLeftMouseUp += (e, l) => {
				ETUDUI.CloseAllyInfoInterface();
				if (_hovered) {
					ETUDUI.OpenAllyInfoInterface(
						PanelNumber,
						false,
						Left.Pixels - ElementWidth - AllyInfoPanel.ElementWidth,
						Top.Pixels + ElementHeight
					);
				}
			};

			Append(_imageButton);
		}

		private void OnMouseOverAction(UIMouseEvent evt, UIElement listeningElement) {
			_hovered = true;

			if (ETUDUI.AllyInfoInterface?.CurrentState is null) {
				ETUDUI.OpenAllyInfoInterface(
					PanelNumber,
					false,
					Left.Pixels - ElementWidth - AllyInfoPanel.ElementWidth,
					Top.Pixels + ElementHeight
				);
			}
		}

		private void OnMouseOutAction(UIMouseEvent evt, UIElement listeningElement) {
			if (ETUDUI.AllyInfoInterface.CurrentState is not null)
				_hovered = false;

			ETUDUI.CloseAllyInfoInterface();
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (ETUDUI.Panels[PanelNumber]?.Ally is null)
				return;

			base.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime) {
			IgnoresMouseInteraction = ETUDUI.Panels[PanelNumber]?.Ally is null;

			base.Update(gameTime);

			Left.Pixels = ETUDUI.Panels[PanelNumber].Left.Pixels - ElementWidth - 10;
			Top.Pixels = ETUDUI.Panels[PanelNumber].Top.Pixels + 45;
		}
	}
}
