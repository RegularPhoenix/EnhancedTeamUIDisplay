using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EnhancedTeamUIDisplay
{
	internal class BuffCheckButton : UIElement
	{
		internal const int width = 46;
		internal const int height = 46;

		private UIElement mainElement;
		private UIImageButton button;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			mainElement = new UIElement();
			mainElement.Left.Set(0, 0f);
			mainElement.Top.Set(0, 0f);
			mainElement.Width.Set(46, 0f);
			mainElement.Height.Set(46, 0f);

			//Button
			button = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/BuffCheckButtonSprite"));
			button.Left.Set(0, 0f);
			button.Top.Set(0, 0f);
			button.Width.Set(46, 0f);
			button.Height.Set(46, 0f);
			button.OnClick += (e, l) => ETUDAdditionalOptions.CheckForBuffs();

			mainElement.Append(button);
			Append(mainElement);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering) Main.instance.MouseText(Main.LocalPlayer.team == 0 ? "First off, enter a team" : "Buff check");
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels - width - 10;
			Top.Pixels = ETUDPanel1.MainTop.Pixels;
		}
	}
}