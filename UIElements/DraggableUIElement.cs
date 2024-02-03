using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace EnhancedTeamUIDisplay.UIElements
{
	internal class DraggableUIElement : UIElement
	{
		internal bool IsLocked;

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (!IsLocked) {
				Move();
			}
		}

		public void Move() {
			if (ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}

			if (_isDragging) {
				Left.Set(Main.mouseX - _offset.X, 0f);
				Top.Set(Main.mouseY - _offset.Y, 0f);
				Recalculate();
			}

			Rectangle parentSpace = Parent.GetDimensions().ToRectangle();
			if (!GetDimensions().ToRectangle().Intersects(parentSpace)) {
				Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
				Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
				Recalculate();
			}
		}

		private Vector2 _offset;
		private bool _isDragging;

		public override void LeftMouseDown(UIMouseEvent evt) {
			if (!IsLocked) {
				base.LeftMouseDown(evt);
				DragStart(evt);
			}
		}

		public override void LeftMouseUp(UIMouseEvent evt) {
			if (!IsLocked) {
				base.LeftMouseUp(evt);
				DragEnd(evt);
			}
		}

		public virtual void DragStart(UIMouseEvent evt) {
			_offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
			_isDragging = true;
		}

		public virtual void DragEnd(UIMouseEvent evt) {
			Vector2 end = evt.MousePosition;
			_isDragging = false;

			Left.Set(end.X - _offset.X, 0f);
			Top.Set(end.Y - _offset.Y, 0f);

			Recalculate();
		}
	}
}
