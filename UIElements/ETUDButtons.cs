using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace EnhancedTeamUIDisplay
{
	internal class BuffCheckButton : UIElement
	{
		internal const int width = 46;
		internal const int height = 46;

		private UIElement MainElement;
		private UIImageButton button;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			MainElement = new UIElement();
			MainElement.Left.Set(0, 0f);
			MainElement.Top.Set(0, 0f);
			MainElement.Width.Set(46, 0f);
			MainElement.Height.Set(46, 0f);

			//Button
			button = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/BuffCheckButtonSprite"));
			button.Left.Set(0, 0f);
			button.Top.Set(0, 0f);
			button.Width.Set(46, 0f);
			button.Height.Set(46, 0f);
			button.OnClick += new MouseEvent(OnBuffCheckButtonClick);

			MainElement.Append(button);
			Append(MainElement);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering)
			{
				if (Main.LocalPlayer.team == 0) Main.instance.MouseText("First off, enter a team"); else Main.instance.MouseText("Buff check");
			}		
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels - width - 10;
			Top.Pixels = ETUDPanel1.MainTop.Pixels;
		}

		private void OnBuffCheckButtonClick(UIMouseEvent evt, UIElement listeningElement)
		{
			ETUDAdditionalOptions.CheckForBuffs();
		}
	}

	internal class AllyInfoButton : UIElement
	{
		internal const int width = 20;
		internal const int height = 24;

		private UIElement MainElement;
		private UIImageButton button;

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			MainElement = new UIElement();
			MainElement.Left.Set(0, 0f);
			MainElement.Top.Set(0, 0f);
			MainElement.Width.Set(20, 0f);
			MainElement.Height.Set(24, 0f);

			//Button
			button = new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/AllyStatButton"));
			button.Left.Set(0, 0f);
			button.Top.Set(0, 0f);
			button.Width.Set(20, 0f);
			button.Height.Set(24, 0f);
			button.OnMouseDown += OnButtonDown;
			button.OnMouseUp += OnButtonUp;
			button.OnMouseOver += OnMouseSelect;
			button.OnMouseOut += OnMouseDeselect;

			MainElement.Append(button);
			Append(MainElement);
		}

		internal virtual void OnMouseSelect(UIMouseEvent evt, UIElement listeningElement) { if (ETUDUISystem.ETUDAllyStatScreen.CurrentState == null) { ETUDUISystem.OpenAllyStatScreen(); } ETUDAllyInfoPanel.GetLeft = Left.Pixels - ETUDAllyInfoPanel.width; ETUDAllyInfoPanel.GetTop = Top.Pixels; }

		internal virtual void OnMouseDeselect(UIMouseEvent evt, UIElement listeningElement) { if (ETUDUISystem.ETUDAllyStatScreen.CurrentState != null) ETUDUISystem.CloseAllyStatScreen(); }

		private void OnButtonDown(UIMouseEvent evt, UIElement listeningElement) { ETUDUISystem.CloseAllyStatScreen(); ETUDAllyInfoPanel.extended = true; ETUDUISystem.OpenAllyStatScreen(); }

		private void OnButtonUp(UIMouseEvent evt, UIElement listeningElement) { ETUDUISystem.CloseAllyStatScreen(); ETUDAllyInfoPanel.extended = false; ETUDUISystem.OpenAllyStatScreen(); }
	}

	internal class AllyInfoButton1 : AllyInfoButton
	{
		internal override void OnMouseSelect(UIMouseEvent evt, UIElement listeningElement)
		{
			if (ETUDPanel1.Ally == null) return;
			ETUDAllyInfoPanel.Ally = ETUDPanel1.Ally;
			base.OnMouseSelect(evt, listeningElement);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (ETUDPanel1.Ally == null) return;

			base.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels - width - 10;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + 45;
		}
	}

	internal class AllyInfoButton2 : AllyInfoButton
	{
		internal override void OnMouseSelect(UIMouseEvent evt, UIElement listeningElement)
		{
			if (ETUDPanel2.Ally == null) return;
			ETUDAllyInfoPanel.Ally = ETUDPanel2.Ally;
			base.OnMouseSelect(evt, listeningElement);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (ETUDPanel2.Ally == null || (ETUDConfig.Instanse.PanelAmount != "Two panels" && ETUDConfig.Instanse.PanelAmount != "Three panels")) return;

			base.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels - width - 10;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + 45 + 70;
		}
	}

	internal class AllyInfoButton3 : AllyInfoButton
	{
		internal override void OnMouseSelect(UIMouseEvent evt, UIElement listeningElement)
		{
			if (ETUDPanel3.Ally == null) return;
			ETUDAllyInfoPanel.Ally = ETUDPanel3.Ally;
			base.OnMouseSelect(evt, listeningElement);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (ETUDPanel3.Ally == null || ETUDConfig.Instanse.PanelAmount != "Three panels") return;

			base.Draw(spriteBatch);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels - width - 10;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + 45 + 140;
		}
	}
}
