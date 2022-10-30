using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria.GameContent;
using System.Linq;
using System;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDPanel1 : UIElement // TODO - LP: Tried to change it, all in vain, needs complete rewrite
	{
		internal const int width = 200;
		internal const int height = 60;

		private UIElement MainElement;
		private UIImage Frame;
		private UIText Name;
		private UIText HPlabel;
		private UIText Rlabel;
		private UIText Ammolabel;

		internal static StyleDimension MainTop;
		internal static StyleDimension MainLeft;

		internal static Player Ally;
		internal static bool allyFound;

		internal Color HPColor;
		internal Color RColor;


		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;
			MainElement = new UIElement();

			MainElement.Width.Set(200, 0f);
			MainElement.Height.Set(60, 0f);

			//Frame
			Frame = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/Panel"));
			Frame.Width.Set(200, 0f);
			Frame.Height.Set(60, 0f);

			//Name
			Name = new UIText("", 0.7f);
			Name.Top.Set(12, 0f);
			Name.Width.Set(90, 0f);
			Name.HAlign = .15f;
			Name.TextOriginX = 0;

			//HP
			HPlabel = new UIText("", 0.7f);
			HPlabel.Top.Set(12, 0f);
			HPlabel.Width.Set(90, 0f);
			HPlabel.HAlign = .85f;
			HPlabel.TextOriginX = 1;

			//R
			Rlabel = new UIText("", 0.7f);
			Rlabel.Top.Set(37, 0f);
			Rlabel.Width.Set(90, 0f);
			Rlabel.HAlign = .85f;
			Rlabel.TextOriginX = 1;

			//Ammo
			Ammolabel = new UIText("", 0.7f);
			Ammolabel.Top.Set(37, 0f);
			Rlabel.Width.Set(90, 0f);
			Ammolabel.HAlign = .15f;
			Ammolabel.TextOriginX = 0;

			MainElement.Append(Frame);
			MainElement.Append(Name);
			MainElement.Append(HPlabel);
			MainElement.Append(Rlabel);	
			MainElement.Append(Ammolabel);
			Append(MainElement);

			Left.Set(Main.LocalPlayer.GetModPlayer<ETUDPlayer>().PanelLeftOffset, 0f);
			Top.Set(Main.LocalPlayer.GetModPlayer<ETUDPlayer>().PanelTopOffset, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering && Ally is not null && ETUDConfig.Instanse.AllowOnClickTeleport) Main.instance.MouseText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.AllowTPLabel")} {Ally.name}");
			if (IsMouseHovering && !ETUDConfig.Instanse.LockUIPosition) Main.instance.MouseText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.UIUnfrozen").Value);

			Rectangle frame = Frame.GetInnerDimensions().ToRectangle();
			spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/BG").Value, new Rectangle(frame.X + 8, frame.Y + 6, 184, 48), Color.White);

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);

			// HP
			Tuple<Color, Color> classColours = MiscEventHandler.GetClassColours((Ally is not null && ETUDConfig.Instanse.ShowOfflinePlayers && !Ally.active) ? "Offline" : ETUDConfig.Instanse.EnableColorMatch ? PlayerClass : "None");
			HPColor = classColours.Item1;

			Rectangle HPBar = Frame.GetInnerDimensions().ToRectangle();
			HPBar.X += 8;
			HPBar.Width -= 16;
			HPBar.Y += 6;
			HPBar.Height = (HPBar.Height - 12) / 2;

			float HPQ = 1;
			if (Ally is not null)
			{
				HPQ = (float)Ally.statLife / Ally.statLifeMax2;
				HPQ = Utils.Clamp(HPQ, 0f, 1f);
			}

			int HPleft = HPBar.Left;
			int HPright = HPBar.Right;
			int HPsteps = (int)((HPright - HPleft) * HPQ);
			for (int i = 0; i < HPsteps; i++)
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(HPleft + i, HPBar.Y, 1, HPBar.Height), HPColor);
			
			// Resource
			RColor = classColours.Item2;
			float RQ = Ally is not null ? MiscEventHandler.GetClassRQ(ETUDConfig.Instanse.EnableColorMatch ? PlayerClass : "None", Ally) : 1;

			Rectangle RBar = Frame.GetInnerDimensions().ToRectangle();
			RBar.X += 8;
			RBar.Width -= 16;
			RBar.Y += 30;
			RBar.Height = (RBar.Height - 12) / 2;

			int Rleft = RBar.Left;
			int Rright = RBar.Right;
			int Rsteps = (int)((Rright - Rleft) * RQ);
			for (int i = 0; i < Rsteps; i++)
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Rleft + i, RBar.Y, 1, RBar.Height), RColor);			
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Name.SetText(""); Rlabel.SetText(""); HPlabel.SetText(""); Ammolabel.SetText("");

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);
			
			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			if ((allyFound || Ally is not null) && !Ally.active && !ETUDConfig.Instanse.ShowOfflinePlayers)
			{
				Ally = null;
				allyFound = false;
			}

			MainLeft = Left; MainTop = Top;

			if (Main.LocalPlayer.team == 0) Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.EnterTeamLabel"));
			else if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel"));
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] is not null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDPanel3.Ally && Main.player[i] != ETUDPanel2.Ally && (ETUDConfig.Instanse.ShowOfflinePlayers || Main.player[i].active)) // && Main.player[i].active
					{
						Ally = Main.player[i];
						allyFound = true;
						break;
					}
				}
			}

			if (Ally is not null && Ally.team != Main.LocalPlayer.team) { Ally = null; allyFound = false; }

			if (Ally is not null)
			{
				if (!Ally.dead)
				{
					HPlabel.SetText($"{Ally.statLife}/{Ally.statLifeMax2}");

					if (PlayerClass == "Melee") Rlabel.SetText(Ally.HasBuff(BuffID.PotionSickness) ? Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.PotionCD").Value : Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.PotionReady").Value);
					else if (PlayerClass == "Ranged")
					{
						int bulletcount = 0;
						int arrowcount = 0;
						for (int i = 0; i < Ally.inventory.Length; i++)
						{
							if (Ally.inventory[i].ammo == AmmoID.Bullet) bulletcount += Ally.inventory[i].stack;
							if (Ally.inventory[i].ammo == AmmoID.Arrow) arrowcount += Ally.inventory[i].stack;
						}
						Ammolabel.SetText($"   {Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.BulletLabel")} {bulletcount} {Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.ArrowLabel")} {arrowcount}");
					}
					else if (PlayerClass == "Rogue") if (CalamityHelper.RogueStealth(Ally) == CalamityHelper.RogueStealthMax(Ally) && CalamityHelper.RogueStealthMax(Ally) != 0) Rlabel.SetText("Stealthed "); else Rlabel.SetText("");
					else Rlabel.SetText($"{Ally.statMana}/{Ally.statManaMax2}");

					if (!Ally.active) Ammolabel.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Offline"));

					Name.SetText(Ally.name);
				}
				else Name.SetText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Dead")} {(Ally.respawnTimer / 60 + 1)}");				
			}
			else if (Main.LocalPlayer.team != 0)
			{
				allyFound = false;
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel"));
			}

			if (!ETUDConfig.Instanse.LockUIPosition)
			{
				if (ContainsPoint(Main.MouseScreen))
				{
					Main.LocalPlayer.mouseInterface = true;
				}

				if (dragging)
				{
					Left.Set(Main.mouseX - offset.X, 0f);
					Top.Set(Main.mouseY - offset.Y, 0f);
					Recalculate();
				}

				var parentSpace = Parent.GetDimensions().ToRectangle();
				if (!GetDimensions().ToRectangle().Intersects(parentSpace))
				{
					Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
					Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
					Recalculate();
				}
			}
		}
		private Vector2 offset;
		public bool dragging;

		public override void MouseDown(UIMouseEvent evt)
		{
			if (!ETUDConfig.Instanse.LockUIPosition)
			{
				base.MouseDown(evt);
				DragStart(evt);
			}
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			if (!ETUDConfig.Instanse.LockUIPosition)
			{
				base.MouseUp(evt);
				DragEnd(evt);
			}
			else if (ETUDConfig.Instanse.LockUIPosition && ETUDConfig.Instanse.AllowOnClickTeleport && Main.LocalPlayer.HasUnityPotion() && Ally is not null && !Ally.dead && !Main.LocalPlayer.dead)
			{
				Main.LocalPlayer.UnityTeleport(Ally.TopLeft);
				Main.LocalPlayer.TakeUnityPotion();
			}
		}

		private void DragStart(UIMouseEvent evt)
		{
			offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
			dragging = true;
		}

		private void DragEnd(UIMouseEvent evt)
		{
			Vector2 end = evt.MousePosition;
			dragging = false;

			Left.Set(end.X - offset.X, 0f);
			Top.Set(end.Y - offset.Y, 0f);

			Main.LocalPlayer.GetModPlayer<ETUDPlayer>().PanelLeftOffset = (int)Left.Pixels;
			Main.LocalPlayer.GetModPlayer<ETUDPlayer>().PanelTopOffset = (int)Top.Pixels;

			Recalculate();
		}
	}

	internal class ETUDPanel2 : UIElement
	{
		internal const int width = 200;
		internal const int height = 60;

		private UIElement MainElement;
		private UIImage Frame;
		private UIText Name;
		private UIText HPlabel;
		private UIText Rlabel;
		private UIText Ammolabel;

		internal static Player Ally;
		internal static bool allyFound;

		internal Color HPColor;
		internal Color RColor;

		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;
			MainElement = new UIElement();

			MainElement.Width.Set(200, 0f);
			MainElement.Height.Set(60, 0f);

			//Frame
			Frame = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/Panel"));
			Frame.Width.Set(200, 0f);
			Frame.Height.Set(60, 0f);


			//Name
			Name = new UIText("", 0.7f);
			Name.Top.Set(12, 0f);
			Name.Width.Set(90, 0f);
			Name.HAlign = .25f;
			Name.TextOriginX = 0;

			//HP
			HPlabel = new UIText("", 0.7f);
			HPlabel.Top.Set(12, 0f);
			HPlabel.Width.Set(90, 0f);
			HPlabel.HAlign = .75f;
			HPlabel.TextOriginX = 1;

			//R
			Rlabel = new UIText("", 0.7f);
			Rlabel.Top.Set(37, 0f);
			Rlabel.Width.Set(90, 0f);
			Rlabel.HAlign = .75f;
			Rlabel.TextOriginX = 1;

			//Ammo
			Ammolabel = new UIText("", 0.7f);
			Ammolabel.Top.Set(37, 0f);
			Rlabel.Width.Set(90, 0f);
			Ammolabel.HAlign = .25f;
			Ammolabel.TextOriginX = 0;

			MainElement.Append(Frame);
			MainElement.Append(Name);
			MainElement.Append(HPlabel);
			MainElement.Append(Rlabel);
			MainElement.Append(Ammolabel);
			Append(MainElement);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + height + 10;
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering && Ally is not null && ETUDConfig.Instanse.AllowOnClickTeleport) Main.instance.MouseText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.AllowTPLabel")} {Ally.name}");
			if (IsMouseHovering && !ETUDConfig.Instanse.LockUIPosition) Main.instance.MouseText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.UIUnfrozen").Value);

			Rectangle frame = Frame.GetInnerDimensions().ToRectangle();
			spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/BG").Value, new Rectangle(frame.X + 8, frame.Y + 6, 184, 48), Color.White);

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);

			// HP
			Tuple<Color, Color> classColours = MiscEventHandler.GetClassColours((Ally is not null && ETUDConfig.Instanse.ShowOfflinePlayers && !Ally.active) ? "Offline" : ETUDConfig.Instanse.EnableColorMatch ? PlayerClass : "None");
			HPColor = classColours.Item1;

			Rectangle HPBar = Frame.GetInnerDimensions().ToRectangle();
			HPBar.X += 8;
			HPBar.Width -= 16;
			HPBar.Y += 6;
			HPBar.Height = (HPBar.Height - 12) / 2;

			float HPQ = 1;
			if (Ally is not null)
			{
				HPQ = (float)Ally.statLife / Ally.statLifeMax2;
				HPQ = Utils.Clamp(HPQ, 0f, 1f);
			}

			int HPleft = HPBar.Left;
			int HPright = HPBar.Right;
			int HPsteps = (int)((HPright - HPleft) * HPQ);
			for (int i = 0; i < HPsteps; i++)
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(HPleft + i, HPBar.Y, 1, HPBar.Height), HPColor);

			// Resource
			RColor = classColours.Item2;
			float RQ = Ally is not null ? MiscEventHandler.GetClassRQ(ETUDConfig.Instanse.EnableColorMatch ? PlayerClass : "None", Ally) : 1;

			Rectangle RBar = Frame.GetInnerDimensions().ToRectangle();
			RBar.X += 8;
			RBar.Width -= 16;
			RBar.Y += 30;
			RBar.Height = (RBar.Height - 12) / 2;

			int Rleft = RBar.Left;
			int Rright = RBar.Right;
			int Rsteps = (int)((Rright - Rleft) * RQ);
			for (int i = 0; i < Rsteps; i++)
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Rleft + i, RBar.Y, 1, RBar.Height), RColor);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Name.SetText(""); Rlabel.SetText(""); HPlabel.SetText(""); Ammolabel.SetText("");

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);
			
			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			if (allyFound && !Ally.active)
			{
				Ally = null;
				allyFound = false;
			}

			Left.Pixels = ETUDPanel1.MainLeft.Pixels;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + height + 10;

			if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel2"));
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] is not null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDPanel3.Ally && Main.player[i] != ETUDPanel1.Ally && (ETUDConfig.Instanse.ShowOfflinePlayers || Main.player[i].active)) // && Main.player[i].active
					{
						Ally = Main.player[i];
						allyFound = true;
						break;
					}
				}
			}

			if (Ally is not null && allyFound == true && ETUDPanel1.Ally is null && ETUDPanel1.allyFound == false)
			{
				ETUDPanel1.Ally = Ally;
				ETUDPanel1.allyFound = true;

				Ally = null;
				allyFound = false;
			}

			if (Ally is not null && Ally.team != Main.LocalPlayer.team) { Ally = null; allyFound = false; }

			if (Ally is not null)
			{
				if (!Ally.dead)
				{
					HPlabel.SetText($"{Ally.statLife}/{Ally.statLifeMax2}");

					if (PlayerClass == "Melee") Rlabel.SetText(Ally.HasBuff(BuffID.PotionSickness) ? Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.PotionCD").Value : Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.PotionReady").Value);
					else if (PlayerClass == "Ranged")
					{
						int bulletcount = 0;
						int arrowcount = 0;
						for (int i = 0; i < Ally.inventory.Length; i++)
						{
							if (Ally.inventory[i].ammo == AmmoID.Bullet) bulletcount += Ally.inventory[i].stack;
							if (Ally.inventory[i].ammo == AmmoID.Arrow) arrowcount += Ally.inventory[i].stack;
						}
						Ammolabel.SetText($"   {Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.BulletLabel")} {bulletcount} {Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.ArrowLabel")} {arrowcount}");
					}
					else if (PlayerClass == "Rogue") if (CalamityHelper.RogueStealth(Ally) == CalamityHelper.RogueStealthMax(Ally) && CalamityHelper.RogueStealthMax(Ally) != 0) Rlabel.SetText("Stealthed "); else Rlabel.SetText("");
					else Rlabel.SetText($"{Ally.statMana}/{Ally.statManaMax2}");

					if (!Ally.active) Ammolabel.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Offline"));				

					Name.SetText(Ally.name);
				}
				else Name.SetText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Dead")} {(Ally.respawnTimer / 60 + 1)}");
				
			}
			else if (Main.LocalPlayer.team != 0)
			{
				allyFound = false;
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel2"));
			}
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			if (ETUDConfig.Instanse.LockUIPosition && ETUDConfig.Instanse.AllowOnClickTeleport && Main.LocalPlayer.HasUnityPotion() && Ally is not null && !Ally.dead && !Main.LocalPlayer.dead)
			{
				Main.LocalPlayer.UnityTeleport(Ally.TopLeft);
				Main.LocalPlayer.TakeUnityPotion();
			}
		}
	}

	internal class ETUDPanel3 : UIElement
	{
		internal const int width = 200;
		internal const int height = 60;

		private UIElement MainElement;
		private UIImage Frame;
		private UIText Name;
		private UIText HPlabel;
		private UIText Rlabel;
		private UIText Ammolabel;

		internal static Player Ally;
		internal static bool allyFound;

		internal Color HPColor;
		internal Color RColor;

		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;
			MainElement = new UIElement();

			MainElement.Width.Set(200, 0f);
			MainElement.Height.Set(60, 0f);

			//Frame
			Frame = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/Panel"));
			Frame.Width.Set(200, 0f);
			Frame.Height.Set(60, 0f);


			//Name
			Name = new UIText("", 0.7f);
			Name.Top.Set(12, 0f);
			Name.Width.Set(90, 0f);
			Name.HAlign = .25f;
			Name.TextOriginX = 0;

			//HP
			HPlabel = new UIText("", 0.7f);
			HPlabel.Top.Set(12, 0f);
			HPlabel.Width.Set(90, 0f);
			HPlabel.HAlign = .75f;
			HPlabel.TextOriginX = 1;

			//R
			Rlabel = new UIText("", 0.7f);
			Rlabel.Top.Set(37, 0f);
			Rlabel.Width.Set(90, 0f);
			Rlabel.HAlign = .75f;
			Rlabel.TextOriginX = 1;

			//Ammo
			Ammolabel = new UIText("", 0.7f);
			Ammolabel.Top.Set(37, 0f);
			Rlabel.Width.Set(90, 0f);
			Ammolabel.HAlign = .25f;
			Ammolabel.TextOriginX = 0;

			MainElement.Append(Frame);
			MainElement.Append(Name);
			MainElement.Append(HPlabel);
			MainElement.Append(Rlabel);
			MainElement.Append(Ammolabel);
			Append(MainElement);

			Left.Pixels = ETUDPanel1.MainLeft.Pixels;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + (2 * (height + 10));
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering && Ally is not null && ETUDConfig.Instanse.AllowOnClickTeleport) Main.instance.MouseText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.AllowTPLabel")} {Ally.name}");
			if (IsMouseHovering && !ETUDConfig.Instanse.LockUIPosition) Main.instance.MouseText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.UIUnfrozen").Value);

			Rectangle frame = Frame.GetInnerDimensions().ToRectangle();
			spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/BG").Value, new Rectangle(frame.X + 8, frame.Y + 6, 184, 48), Color.White);

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);

			// HP
			Tuple<Color, Color> classColours = MiscEventHandler.GetClassColours((Ally is not null && ETUDConfig.Instanse.ShowOfflinePlayers && !Ally.active) ? "Offline" : ETUDConfig.Instanse.EnableColorMatch ? PlayerClass : "None");
			HPColor = classColours.Item1;

			Rectangle HPBar = Frame.GetInnerDimensions().ToRectangle();
			HPBar.X += 8;
			HPBar.Width -= 16;
			HPBar.Y += 6;
			HPBar.Height = (HPBar.Height - 12) / 2;

			float HPQ = 1;
			if (Ally is not null)
			{
				HPQ = (float)Ally.statLife / Ally.statLifeMax2;
				HPQ = Utils.Clamp(HPQ, 0f, 1f);
			}

			int HPleft = HPBar.Left;
			int HPright = HPBar.Right;
			int HPsteps = (int)((HPright - HPleft) * HPQ);
			for (int i = 0; i < HPsteps; i++)
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(HPleft + i, HPBar.Y, 1, HPBar.Height), HPColor);

			// Resource
			RColor = classColours.Item2;
			float RQ = Ally is not null ? MiscEventHandler.GetClassRQ(ETUDConfig.Instanse.EnableColorMatch ? PlayerClass : "None", Ally) : 1;

			Rectangle RBar = Frame.GetInnerDimensions().ToRectangle();
			RBar.X += 8;
			RBar.Width -= 16;
			RBar.Y += 30;
			RBar.Height = (RBar.Height - 12) / 2;

			int Rleft = RBar.Left;
			int Rright = RBar.Right;
			int Rsteps = (int)((Rright - Rleft) * RQ);
			for (int i = 0; i < Rsteps; i++)
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Rleft + i, RBar.Y, 1, RBar.Height), RColor);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Name.SetText(""); Rlabel.SetText(""); HPlabel.SetText(""); Ammolabel.SetText("");

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);

			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			if (allyFound && !Ally.active)
			{
				Ally = null;
				allyFound = false;
			}

			Left.Pixels = ETUDPanel1.MainLeft.Pixels;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + (height + 10) * 2;

			if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel2"));
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] is not null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDPanel1.Ally && Main.player[i] != ETUDPanel2.Ally && (ETUDConfig.Instanse.ShowOfflinePlayers || Main.player[i].active)) // && Main.player[i].active
					{
						Ally = Main.player[i];
						allyFound = true;
						break;
					}
				}
			}

			if (Ally is not null && allyFound == true && ETUDPanel1.Ally is null && ETUDPanel1.allyFound == false)
			{
				ETUDPanel1.Ally = Ally;
				ETUDPanel1.allyFound = true;

				Ally = null;
				allyFound = false;
			}

			if (Ally is not null && Ally.team != Main.LocalPlayer.team) { Ally = null; allyFound = false; }

			if (Ally is not null)
			{
				if (!Ally.dead)
				{
					HPlabel.SetText($"{Ally.statLife}/{Ally.statLifeMax2}");

					if (PlayerClass == "Melee") Rlabel.SetText(Ally.HasBuff(BuffID.PotionSickness) ? Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.PotionCD").Value : Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.PotionReady").Value);
					else if (PlayerClass == "Ranged")
					{
						int bulletcount = 0;
						int arrowcount = 0;
						for (int i = 0; i < Ally.inventory.Length; i++)
						{
							if (Ally.inventory[i].ammo == AmmoID.Bullet) bulletcount += Ally.inventory[i].stack;
							if (Ally.inventory[i].ammo == AmmoID.Arrow) arrowcount += Ally.inventory[i].stack;
						}
						Ammolabel.SetText($"   {Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.BulletLabel")} {bulletcount} {Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.ArrowLabel")} {arrowcount}");
					}
					else if (PlayerClass == "Rogue") if (CalamityHelper.RogueStealth(Ally) == CalamityHelper.RogueStealthMax(Ally) && CalamityHelper.RogueStealthMax(Ally) != 0) Rlabel.SetText("Stealthed "); else Rlabel.SetText("");
					else Rlabel.SetText($"{Ally.statMana}/{Ally.statManaMax2}");

					if (!Ally.active) Ammolabel.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Offline"));
					
					Name.SetText(Ally.name);
				}
				else Name.SetText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Dead")} {(Ally.respawnTimer / 60 + 1)}");		
			}
			else if (Main.LocalPlayer.team != 0)
			{
				allyFound = false;
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel2"));
			}
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			if (ETUDConfig.Instanse.LockUIPosition && ETUDConfig.Instanse.AllowOnClickTeleport && Main.LocalPlayer.HasUnityPotion() && Ally is not null && !Ally.dead && !Main.LocalPlayer.dead)
			{
				Main.LocalPlayer.UnityTeleport(Ally.TopLeft);
				Main.LocalPlayer.TakeUnityPotion();
			}
		}
	}
	
	public class MiscEventHandler
	{
		public static string DeterminePlayerClass(Player player)
		{
			if (player is null) return "None";

			float MeleeCoeff = player.GetTotalDamage(DamageClass.Melee).Additive + (player.GetCritChance(DamageClass.Melee) / 100) + (player.GetAttackSpeed(DamageClass.Melee) / 2);
			float RangedCoeff = player.GetTotalDamage(DamageClass.Ranged).Additive + (player.GetCritChance(DamageClass.Ranged) / 100) + (player.GetAttackSpeed(DamageClass.Ranged) / 2);
			float MagicCoeff = player.GetTotalDamage(DamageClass.Magic).Additive + (player.GetCritChance(DamageClass.Magic) / 100) + (player.GetAttackSpeed(DamageClass.Magic) / 2);
			float SummonCoeff = player.GetTotalDamage(DamageClass.Summon).Additive + (player.GetCritChance(DamageClass.Summon) / 100) + (player.GetAttackSpeed(DamageClass.Summon) / 2 + (player.maxMinions - 1));
			float RogueCoeff = 0;
			if (ETUD.CalamityMod is not null)
				if (ETUD.CalamityMod.TryFind<DamageClass>("RogueDamageClass", out var rogueclass))
					RogueCoeff = player.GetTotalDamage(rogueclass).Additive + (player.GetCritChance(rogueclass) / 100) + (player.GetAttackSpeed(rogueclass) / 2);

			float[] CoeffArray = new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff, RogueCoeff };
			int StatNum = Array.IndexOf(CoeffArray, CoeffArray.Max());

			return StatNum switch
			{
				0 => "Melee",
				1 => "Ranged",
				2 => "Magic",
				3 => "Summon",
				4 => "Rogue",
				_ => "None",
			};
		}

		public static float GetClassRQ(string playerClass, Player Ally)
		{
			return playerClass switch
			{
				"Melee" => 1,
				"Ranged" => 1,
				"Rogue" => Utils.Clamp(CalamityHelper.RogueStealth(Ally) / CalamityHelper.RogueStealthMax(Ally), 0f, 1f),
				_ => Utils.Clamp((float)Ally.statMana / Ally.statManaMax2, 0f, 1f),
			};
		}

		public static Tuple<Color, Color> GetClassColours(string playerClass)
		{
			return playerClass switch
			{
				"Melee" => new(new(200, 155, 100), new(145, 30, 50)),
				"Ranged" => new(new(170, 210, 115), new(165, 80, 40)),
				"Magic" => new(new(110, 200, 240), new(50, 80, 140)),
				"Summon" => new(new(150, 130, 200), new(50, 80, 140)),
				"Rogue" => new(new(255, 240, 110), new(180, 150, 20)),
				"Offline" => new(Color.Gray, Color.LightGray),
				"None" => new(Color.Green, Color.Blue),
				_ => new(Color.White, Color.White),
			};
		}

		public static bool HasItemsInInventory(Player player, int[] itemtypes)
		{
			for (int i = 0; i < itemtypes.Length; i++) if (player.HasItem(itemtypes[i])) return true;
			return false;
		}

		public static bool HasBuffs(Player player, int[] bufftypes)
		{
			for (int i = 0; i < bufftypes.Length; i++) if (player.HasBuff(bufftypes[i])) return true;
			return false;
		}

		public static int CountItemsInInventory(Player player, int[] itemtypes)
		{
			int count = 0;
			for (int i = 0; i < itemtypes.Length; i++) count += player.CountItem(itemtypes[i]);
			return count;
		}
	}

	public class CalamityHelper
	{
		public static float RogueStealth(Player player) => (float)ETUD.CalamityMod.Call("GetCurrentStealth", player);

		public static float RogueStealthMax(Player player) => (float)ETUD.CalamityMod.Call("GetMaxStealth", player);
	}
}
