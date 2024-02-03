using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace EnhancedTeamUIDisplay.UIElements
{
	internal class LockButton : UIElement {
		internal LockButton(DraggableUIElement target)
			=> _target = target;

		internal const int ElementWidth = 18, ElementHeight = 18;

		internal enum LockType {
			Panels,
			DamageMeter
		}

		private readonly DraggableUIElement _target;

		private UIImageButton _lockButton;

		public override void OnInitialize() {
			Width.Pixels = ElementWidth;
			Height.Pixels = ElementHeight;

			ETUDPlayer player = Main.LocalPlayer.GetModPlayer<ETUDPlayer>();

			_lockButton = new UIImageButton(
				ModContent.Request<Texture2D>(
					_target.IsLocked
					? "EnhancedTeamUIDisplay/Sprites/Locked"
					: "EnhancedTeamUIDisplay/Sprites/Unlocked"
				)
			);
			_lockButton.Width.Set(ElementWidth, 0f);
			_lockButton.Height.Set(ElementHeight, 0f);
			_lockButton.OnLeftClick += OnLeftClickAction;
			Append(_lockButton);
		}

		private void OnLeftClickAction(UIMouseEvent evt, UIElement listeningElement) {
			_target.IsLocked = !_target.IsLocked;

			_lockButton.SetImage(
				ModContent.Request<Texture2D>(
					_target.IsLocked
					? "EnhancedTeamUIDisplay/Sprites/Locked"
					: "EnhancedTeamUIDisplay/Sprites/Unlocked"
				)
			);
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			Left.Pixels = _target.Left.Pixels + _target.Width.Pixels + 5;
			Top.Pixels = _target.Top.Pixels - 5;

			if (ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
		}
	}
}
