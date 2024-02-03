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
		internal const int ElementWidth = 46, ElementHeight = 46;

		private UIImageButton _button;

		public override void OnInitialize() {
			Width.Pixels = ElementWidth;
			Height.Pixels = ElementHeight;

			_button = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/EquipmentCheckButton"));
			_button.Width.Set(ElementWidth, 0f);
			_button.Height.Set(ElementHeight, 0f);
			_button.OnLeftClick += (e, l) => EquipmentCheck.CheckAlliesEquipment();
			Append(_button);
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

			Left.Pixels = ETUDUI.MainPanels[0].Left.Pixels - ElementWidth - 10;
			Top.Pixels = ETUDUI.MainPanels[0].Top.Pixels;
		}
	}
}
