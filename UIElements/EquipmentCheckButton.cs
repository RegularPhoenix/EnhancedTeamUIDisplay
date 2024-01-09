using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace EnhancedTeamUIDisplay.UIElements
{
	internal class EquipmentCheckButton : UIElement
	{
		internal const int width = 46, height = 46;

		private UIElement mainElement;
		private UIImageButton button;

		public override void OnInitialize() {
			Width.Pixels = width;
			Height.Pixels = height;

			mainElement = new UIElement();
			mainElement.Left.Set(0, 0f);
			mainElement.Top.Set(0, 0f);
			mainElement.Width.Set(width, 0f);
			mainElement.Height.Set(height, 0f);

			// Button
			button = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/EquipmentCheckButton"));
			button.Left.Set(0, 0f);
			button.Top.Set(0, 0f);
			button.Width.Set(width, 0f);
			button.Height.Set(height, 0f);
			button.OnLeftClick += (e, l) => EquipmentCheck.CheckAlliesEquipment();

			mainElement.Append(button);
			Append(mainElement);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering) {
				Main.instance.MouseText(
					Main.LocalPlayer.team == 0
					? Language.GetText("Mods.EnhancedTeamUIDisplay.EquipmentCheck.EnterTeam").Value
					: Language.GetText("Mods.EnhancedTeamUIDisplay.EquipmentCheck.Check").Value
				);
			}
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			Left.Pixels = ETUDUI.Panels[0].Left.Pixels - width - 10;
			Top.Pixels = ETUDUI.Panels[0].Top.Pixels;
		}
	}
}
