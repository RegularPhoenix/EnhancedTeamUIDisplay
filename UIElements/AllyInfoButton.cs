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

		internal const int width = 20, height = 24;

		private UIElement mainElement;
		private UIImageButton imageButton;

		private int PanelNumber { get; }

		private static bool hovered;

		public override void OnInitialize() {
			Width.Pixels = width;
			Height.Pixels = height;

			mainElement = new UIElement();
			mainElement.Left.Set(0, 0f);
			mainElement.Top.Set(0, 0f);
			mainElement.Width.Set(width, 0f);
			mainElement.Height.Set(height, 0f);

			//Button
			imageButton = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyInfoButton"));
			imageButton.Left.Set(0, 0f);
			imageButton.Top.Set(0, 0f);
			imageButton.Width.Set(width, 0f);
			imageButton.Height.Set(height, 0f);

			imageButton.OnMouseOver += OnMouseOverAction;
			imageButton.OnMouseOut += OnMouseOutAction;

			imageButton.OnLeftMouseDown += (e, l) => {
				ETUDUI.CloseAllyInfoInterface();
				ETUDUI.OpenAllyInfoInterface(
					PanelNumber,
					true,
					Left.Pixels - width - AllyInfoPanel.width,
					Top.Pixels + height
				);
			};

			imageButton.OnLeftMouseUp += (e, l) => {
				ETUDUI.CloseAllyInfoInterface();
				if (hovered) {
					ETUDUI.OpenAllyInfoInterface(
						PanelNumber,
						false,
						Left.Pixels - width - AllyInfoPanel.width,
						Top.Pixels + height
					);
				}
			};

			mainElement.Append(imageButton);
			Append(mainElement);
		}

		private void OnMouseOverAction(UIMouseEvent evt, UIElement listeningElement) {
			hovered = true;

			if (ETUDUI.AllyInfoInterface?.CurrentState is null) {
				ETUDUI.OpenAllyInfoInterface(
					PanelNumber,
					false,
					Left.Pixels - width - AllyInfoPanel.width,
					Top.Pixels + height
				);
			}
		}

		private void OnMouseOutAction(UIMouseEvent evt, UIElement listeningElement) {
			if (ETUDUI.AllyInfoInterface.CurrentState is not null)
				hovered = false;

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

			Left.Pixels = ETUDUI.Panels[PanelNumber].Left.Pixels - width - 10;
			Top.Pixels = ETUDUI.Panels[PanelNumber].Top.Pixels + 45;
		}
	}
}
