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

namespace EnhancedTeamUIDisplay
{
	internal class ETUDPanel1 : UIElement
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

		internal Color HPColor1;
		internal Color HPColor2;
		internal Color RColor1;
		internal Color RColor2;


		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;
			MainElement = new UIElement();

			//Panel position
			//MainElement.Left.Set(0, 0f);
			//MainElement.Top.Set(0, 0f);

			MainElement.Width.Set(200, 0f);
			MainElement.Height.Set(60, 0f);

			//Frame
			Frame = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/Panel"));
			Frame.Left.Set(0, 0f);
			Frame.Top.Set(0, 0f);
			Frame.Width.Set(200, 0f);
			Frame.Height.Set(60, 0f);


			//Name
			Name = new UIText("", 0.7f);
			Name.Left.Set(10, 0f);
			Name.Top.Set(12, 0f);
			Name.Width.Set(80, 0f);
			Name.Height.Set(60, 0f);
			Name.HAlign = 0;

			//HP
			HPlabel = new UIText("", 0.7f);
			HPlabel.Left.Set(-7, 0f);
			HPlabel.Top.Set(12, 0f);
			HPlabel.Width.Set(50, 0f);
			HPlabel.Height.Set(30, 0f);
			HPlabel.HAlign = 1;

			//R
			Rlabel = new UIText("", 0.7f);
			Rlabel.Left.Set(-7, 0f);
			Rlabel.Top.Set(37, 0f);
			Rlabel.Width.Set(50, 0f);
			Rlabel.Height.Set(30, 0f);
			Rlabel.HAlign = 1;

			//Ammo
			Ammolabel = new UIText("", 0.7f);
			Ammolabel.Left.Set(10, 0f);
			Ammolabel.Top.Set(37, 0f);
			Ammolabel.Width.Set(50, 0f);
			Ammolabel.Height.Set(30, 0f);

			MainElement.Append(Frame);
			MainElement.Append(Name);
			MainElement.Append(HPlabel);
			MainElement.Append(Rlabel);	
			MainElement.Append(Ammolabel);
			Append(MainElement);

			Left.Set(ETUDPlayer.PanelLeftOffset, 0f);
			Top.Set(ETUDPlayer.PanelTopOffset, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			if (IsMouseHovering && Ally != null && ETUDConfig.Instanse.AllowOnClickTeleport) Main.instance.MouseText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.AllowTPLabel")} {Ally.name}");
			if (IsMouseHovering && !ETUDConfig.Instanse.LockUIPosition) Main.instance.MouseText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.UIUnfrozen").Value);

			Rectangle frame = Frame.GetInnerDimensions().ToRectangle();
			spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/BG").Value, new Rectangle(frame.X + 8, frame.Y + 6, 184, 48), Color.White);

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);

			// HP Color
			if (ETUDConfig.Instanse.EnableColorMatch)
			{
				switch (PlayerClass)
				{
					case "Melee":
						HPColor1 = Color.SandyBrown;
						HPColor2 = Color.Brown;
						break;
					case "Ranged":
						HPColor1 = Color.Lime;
						HPColor2 = Color.LightGreen;
						break;
					case "Magic":
						HPColor1 = Color.LightSkyBlue;
						HPColor2 = Color.LightBlue;
						break;
					case "Summon":
						HPColor1 = Color.MediumPurple;
						HPColor2 = Color.Purple;
						break;
					case "Rogue":
						HPColor1 = Color.LightYellow;
						HPColor2 = Color.LightYellow;
						break;
					case "None":
						HPColor1 = Color.Green;
						HPColor2 = Color.LawnGreen;
						break;
					default:
						HPColor1 = Color.Green;
						HPColor2 = Color.LawnGreen;
						break;
				}
			}
			else
			{
				HPColor1 = Color.Green;
				HPColor2 = Color.LawnGreen;
			}

			// HPBar
			Rectangle HPBar = Frame.GetInnerDimensions().ToRectangle();
			HPBar.X += 8;
			HPBar.Width -= 16;
			HPBar.Y += 6;
			HPBar.Height = (HPBar.Height - 12) / 2;

			float HPQ;
			if (Ally != null)
			{
				HPQ = (float)Ally.statLife / Ally.statLifeMax2;
				HPQ = Utils.Clamp(HPQ, 0f, 1f);
			}
			else HPQ = 1;

			if (Ally != null && ETUDConfig.Instanse.ShowOfflinePlayers)
			{
				if (!Ally.active)
				{
					HPColor1 = Color.LightGray;
					HPColor2 = Color.LightGray;
					HPQ = 1;
				}
			}

			int HPleft = HPBar.Left;
			int HPright = HPBar.Right;
			int HPsteps = (int)((HPright - HPleft) * HPQ);
			for (int i = 0; i < HPsteps; i++)
			{
				float hppercent = (float)i / (HPright - HPleft);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(HPleft + i, HPBar.Y, 1, HPBar.Height), Color.Lerp(HPColor1, HPColor2, hppercent));
			}

			// Resource
			float RQ;
			if (ETUDConfig.Instanse.EnableColorMatch)
			{
				switch (PlayerClass)
				{
					case "Melee":
						RColor1 = Color.IndianRed;
						RColor2 = Color.DarkRed;
						RQ = 1;
						// TODO: Show exact healing potion CD
						break;
					case "Ranged":
						RColor1 = Color.OrangeRed;
						RColor2 = Color.Orange;
						RQ = 1;
						break;
					case "Magic":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "Summon":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "Rogue":
						RColor1 = Color.Yellow;
						RColor2 = Color.Yellow;
						if(Ally != null)
						{							
							RQ = CalamityHelper.RogueStealth(Ally) / CalamityHelper.RogueStealthMax(Ally);
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "None":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					default:
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
				}
			}
			else
			{
				RColor1 = Color.Blue;
				RColor2 = Color.DeepSkyBlue;
				if (Ally != null)
				{
					RQ = (float)Ally.statMana / Ally.statManaMax2;
					RQ = Utils.Clamp(RQ, 0f, 1f);
				}
				else RQ = 1;
			}

			// ResourceBar
			Rectangle RBar = Frame.GetInnerDimensions().ToRectangle();
			RBar.X += 8;
			RBar.Width -= 16;
			RBar.Y += 30;
			RBar.Height = (RBar.Height - 12) / 2;

			if (Ally != null && ETUDConfig.Instanse.ShowOfflinePlayers)
			{
				if (!Ally.active)
				{
					RColor1 = Color.Gray;
					RColor2 = Color.Gray;
					RQ = 1;
				}
			}

			int Rleft = RBar.Left;
			int Rright = RBar.Right;
			int Rsteps = (int)((Rright - Rleft) * RQ);
			for (int i = 0; i < Rsteps; i++)
			{
				float Rpercent = (float)i / (Rright - Rleft);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Rleft + i, RBar.Y, 1, RBar.Height), Color.Lerp(RColor1, RColor2, Rpercent));
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);
			
			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			if ((allyFound || Ally != null) && !Ally.active && !ETUDConfig.Instanse.ShowOfflinePlayers)
			{
				Ally = null;
				allyFound = false;
			}

			MainLeft = Left; MainTop = Top;

			if (Main.LocalPlayer.team == 0) { Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.EnterTeamLabel"));  HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText(""); }
			else if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel")); HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText("");
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] != null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDPanel3.Ally && Main.player[i] != ETUDPanel2.Ally && (ETUDConfig.Instanse.ShowOfflinePlayers || Main.player[i].active)) // && Main.player[i].active
					{
						Ally = Main.player[i];
						allyFound = true;
						break;
					}
				}
			}

			if (Ally != null && Ally.team != Main.LocalPlayer.team) { Ally = null; allyFound = false; }

			if (Ally != null)
			{
				if (!Ally.dead)
				{
					HPlabel.SetText($"{Ally.statLife}/{Ally.statLifeMax2}");
					
					if (PlayerClass != "Ranged") Ammolabel.SetText("");

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
						Rlabel.SetText("");
					}
					else if (PlayerClass == "Rogue") if (CalamityHelper.RogueStealth(Ally) == CalamityHelper.RogueStealthMax(Ally) && CalamityHelper.RogueStealthMax(Ally) != 0) Rlabel.SetText("Stealthed "); else Rlabel.SetText("");
					else Rlabel.SetText($"{Ally.statMana}/{Ally.statManaMax2}");

					if (!Ally.active)
					{
						HPlabel.SetText("");
						Rlabel.SetText("");
						Ammolabel.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Offline"));
					}

					Name.SetText(Ally.name);				
				}
				else
				{
					Name.SetText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Dead")} {(Ally.respawnTimer / 60 + 1)}");
					Rlabel.SetText("");
					HPlabel.SetText("");
					Ammolabel.SetText("");
				}
			}
			else if (Main.LocalPlayer.team != 0)
			{
				allyFound = false; Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel")); HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText("");
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
			else if (ETUDConfig.Instanse.LockUIPosition && ETUDConfig.Instanse.AllowOnClickTeleport && Main.LocalPlayer.HasUnityPotion() && Ally != null && !Ally.dead && !Main.LocalPlayer.dead)
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

			ETUDPlayer.PanelLeftOffset = (int)Left.Pixels;
			ETUDPlayer.PanelTopOffset = (int)Top.Pixels;

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

		internal Color HPColor1;
		internal Color HPColor2;
		internal Color RColor1;
		internal Color RColor2;

		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;
			MainElement = new UIElement();

			//Panel position
			MainElement.Left.Set(0, 0f);
			MainElement.Top.Set(0, 0f);

			MainElement.Width.Set(200, 0f);
			MainElement.Height.Set(60, 0f);

			//Frame
			Frame = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/Panel"));
			Frame.Left.Set(0, 0f);
			Frame.Top.Set(0, 0f);
			Frame.Width.Set(200, 0f);
			Frame.Height.Set(60, 0f);

			//Name
			Name = new UIText("", 0.7f);
			Name.Left.Set(10, 0f);
			Name.Top.Set(12, 0f);
			Name.Width.Set(80, 0f);
			Name.Height.Set(60, 0f);
			Name.HAlign = 0;

			//HP
			HPlabel = new UIText("", 0.7f);
			HPlabel.Left.Set(-7, 0f);
			HPlabel.Top.Set(12, 0f);
			HPlabel.Width.Set(50, 0f);
			HPlabel.Height.Set(30, 0f);
			HPlabel.HAlign = 1;

			//R
			Rlabel = new UIText("", 0.7f);
			Rlabel.Left.Set(-7, 0f);
			Rlabel.Top.Set(37, 0f);
			Rlabel.Width.Set(50, 0f);
			Rlabel.Height.Set(30, 0f);
			Rlabel.HAlign = 1;

			//Ammo
			Ammolabel = new UIText("", 0.7f);
			Ammolabel.Left.Set(10, 0f);
			Ammolabel.Top.Set(37, 0f);
			Ammolabel.Width.Set(50, 0f);
			Ammolabel.Height.Set(30, 0f);

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

			if (IsMouseHovering && Ally != null && ETUDConfig.Instanse.AllowOnClickTeleport) Main.instance.MouseText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.AllowTPLabel")} {Ally.name}");
			if (IsMouseHovering && !ETUDConfig.Instanse.LockUIPosition) Main.instance.MouseText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.UIUnfrozen").Value);

			Rectangle frame = Frame.GetInnerDimensions().ToRectangle();
			spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/BG").Value, new Rectangle(frame.X + 8, frame.Y + 6, 184, 48), Color.White);

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);

			// HP Color
			if (ETUDConfig.Instanse.EnableColorMatch)
			{
				switch (PlayerClass)
				{
					case "Melee":
						HPColor1 = Color.SandyBrown;
						HPColor2 = Color.Brown;
						break;
					case "Ranged":
						HPColor1 = Color.Lime;
						HPColor2 = Color.LightGreen;
						break;
					case "Magic":
						HPColor1 = Color.LightSkyBlue;
						HPColor2 = Color.LightBlue;
						break;
					case "Summon":
						HPColor1 = Color.MediumPurple;
						HPColor2 = Color.Purple;
						break;
					case "Rogue":
						HPColor1 = Color.LightYellow;
						HPColor2 = Color.LightYellow;
						break;
					case "None":
						HPColor1 = Color.Green;
						HPColor2 = Color.LawnGreen;
						break;
					default:
						HPColor1 = Color.Green;
						HPColor2 = Color.LawnGreen;
						break;
				}
			}
			else
			{
				HPColor1 = Color.Green;
				HPColor2 = Color.LawnGreen;
			}

			// HPBar
			Rectangle HPBar = Frame.GetInnerDimensions().ToRectangle();
			HPBar.X += 8;
			HPBar.Width -= 16;
			HPBar.Y += 6;
			HPBar.Height = (HPBar.Height - 12) / 2;

			float HPQ;
			if (Ally != null)
			{
				HPQ = (float)Ally.statLife / Ally.statLifeMax2;
				HPQ = Utils.Clamp(HPQ, 0f, 1f);
			}
			else HPQ = 1;

			if (Ally != null && ETUDConfig.Instanse.ShowOfflinePlayers)
			{
				if (!Ally.active)
				{
					HPColor1 = Color.LightGray;
					HPColor2 = Color.LightGray;
					HPQ = 1;
				}
			}

			int HPleft = HPBar.Left;
			int HPright = HPBar.Right;
			int HPsteps = (int)((HPright - HPleft) * HPQ);
			for (int i = 0; i < HPsteps; i++)
			{
				float hppercent = (float)i / (HPright - HPleft);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(HPleft + i, HPBar.Y, 1, HPBar.Height), Color.Lerp(HPColor1, HPColor2, hppercent));
			}

			// Resource
			float RQ;
			if (ETUDConfig.Instanse.EnableColorMatch)
			{
				switch (PlayerClass)
				{
					case "Melee":
						RColor1 = Color.IndianRed;
						RColor2 = Color.DarkRed;
						RQ = 1;
						// TODO: Show exact healing potion CD
						break;
					case "Ranged":
						RColor1 = Color.OrangeRed;
						RColor2 = Color.Orange;
						RQ = 1;
						break;
					case "Magic":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "Summon":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "Rogue":
						RColor1 = Color.Yellow;
						RColor2 = Color.Yellow;
						if (Ally != null)
						{
							RQ = CalamityHelper.RogueStealth(Ally) / CalamityHelper.RogueStealthMax(Ally);
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "None":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					default:
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
				}
			}
			else
			{
				RColor1 = Color.Blue;
				RColor2 = Color.DeepSkyBlue;
				if (Ally != null)
				{
					RQ = (float)Ally.statMana / Ally.statManaMax2;
					RQ = Utils.Clamp(RQ, 0f, 1f);
				}
				else RQ = 1;
			}

			// ResourceBar
			Rectangle RBar = Frame.GetInnerDimensions().ToRectangle();
			RBar.X += 8;
			RBar.Width -= 16;
			RBar.Y += 30;
			RBar.Height = (RBar.Height - 12) / 2;

			if (Ally != null && ETUDConfig.Instanse.ShowOfflinePlayers)
			{
				if (!Ally.active)
				{
					RColor1 = Color.Gray;
					RColor2 = Color.Gray;
					RQ = 1;
				}
			}

			int Rleft = RBar.Left;
			int Rright = RBar.Right;
			int Rsteps = (int)((Rright - Rleft) * RQ);
			for (int i = 0; i < Rsteps; i++)
			{
				float Rpercent = (float)i / (Rright - Rleft);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Rleft + i, RBar.Y, 1, RBar.Height), Color.Lerp(RColor1, RColor2, Rpercent));
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);
			
			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			if (allyFound && !Ally.active)
			{
				Ally = null;
				allyFound = false;
			}

			Left.Pixels = ETUDPanel1.MainLeft.Pixels;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + height + 10;

			if (Main.LocalPlayer.team == 0) { Name.SetText(""); HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText(""); }
			else if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel2")); HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText("");
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] != null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDPanel3.Ally && Main.player[i] != ETUDPanel1.Ally && (ETUDConfig.Instanse.ShowOfflinePlayers || Main.player[i].active)) // && Main.player[i].active
					{
						Ally = Main.player[i];
						allyFound = true;
						break;
					}
				}
			}

			if (Ally != null && allyFound == true && ETUDPanel1.Ally == null && ETUDPanel1.allyFound == false)
			{
				ETUDPanel1.Ally = Ally;
				ETUDPanel1.allyFound = true;

				Ally = null;
				allyFound = false;
			}

			if (Ally != null && Ally.team != Main.LocalPlayer.team) { Ally = null; allyFound = false; }

			if (Ally != null)
			{
				if (!Ally.dead)
				{
					HPlabel.SetText($"{Ally.statLife}/{Ally.statLifeMax2}");

					if (PlayerClass != "Ranged") Ammolabel.SetText("");

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
						Rlabel.SetText("");
					}
					else if (PlayerClass == "Rogue") if (CalamityHelper.RogueStealth(Ally) == CalamityHelper.RogueStealthMax(Ally) && CalamityHelper.RogueStealthMax(Ally) != 0) Rlabel.SetText("Stealthed "); else Rlabel.SetText("");
					else Rlabel.SetText($"{Ally.statMana}/{Ally.statManaMax2}");

					if (!Ally.active)
					{
						HPlabel.SetText("");
						Rlabel.SetText("");
						Ammolabel.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Offline"));
					}

					Name.SetText(Ally.name);
				}
				else
				{
					Name.SetText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Dead")} {(Ally.respawnTimer / 60 + 1)}");
					Rlabel.SetText("");
					HPlabel.SetText("");
					Ammolabel.SetText("");
				}
			}
			else if (Main.LocalPlayer.team != 0)
			{
				allyFound = false; Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel2")); HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText("");
			}
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			if (ETUDConfig.Instanse.LockUIPosition && ETUDConfig.Instanse.AllowOnClickTeleport && Main.LocalPlayer.HasUnityPotion() && Ally != null && !Ally.dead && !Main.LocalPlayer.dead)
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

		internal Color HPColor1;
		internal Color HPColor2;
		internal Color RColor1;
		internal Color RColor2;

		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;
			MainElement = new UIElement();

			//Panel position
			MainElement.Left.Set(0, 0f);
			MainElement.Top.Set(0, 0f);

			MainElement.Width.Set(200, 0f);
			MainElement.Height.Set(60, 0f);

			//Frame
			Frame = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/Panel"));
			Frame.Left.Set(0, 0f);
			Frame.Top.Set(0, 0f);
			Frame.Width.Set(200, 0f);
			Frame.Height.Set(60, 0f);

			//Name
			Name = new UIText("", 0.7f);
			Name.Left.Set(10, 0f);
			Name.Top.Set(12, 0f);
			Name.Width.Set(80, 0f);
			Name.Height.Set(60, 0f);
			Name.HAlign = 0;

			//HP
			HPlabel = new UIText("", 0.7f);
			HPlabel.Left.Set(-7, 0f);
			HPlabel.Top.Set(12, 0f);
			HPlabel.Width.Set(50, 0f);
			HPlabel.Height.Set(30, 0f);
			HPlabel.HAlign = 1;

			//R
			Rlabel = new UIText("", 0.7f);
			Rlabel.Left.Set(-7, 0f);
			Rlabel.Top.Set(37, 0f);
			Rlabel.Width.Set(50, 0f);
			Rlabel.Height.Set(30, 0f);
			Rlabel.HAlign = 1;

			//Ammo
			Ammolabel = new UIText("", 0.7f);
			Ammolabel.Left.Set(10, 0f);
			Ammolabel.Top.Set(37, 0f);
			Ammolabel.Width.Set(50, 0f);
			Ammolabel.Height.Set(30, 0f);

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

			if (IsMouseHovering && Ally != null && ETUDConfig.Instanse.AllowOnClickTeleport) Main.instance.MouseText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.AllowTPLabel")} {Ally.name}");
			if (IsMouseHovering && !ETUDConfig.Instanse.LockUIPosition) Main.instance.MouseText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.UIUnfrozen").Value);

			Rectangle frame = Frame.GetInnerDimensions().ToRectangle();
			spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/BG").Value, new Rectangle(frame.X + 8, frame.Y + 6, 184, 48), Color.White);

			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);

			// HP Color
			if (ETUDConfig.Instanse.EnableColorMatch)
			{
				switch (PlayerClass)
				{
					case "Melee":
						HPColor1 = Color.SandyBrown;
						HPColor2 = Color.Brown;
						break;
					case "Ranged":
						HPColor1 = Color.Lime;
						HPColor2 = Color.LightGreen;
						break;
					case "Magic":
						HPColor1 = Color.LightSkyBlue;
						HPColor2 = Color.LightBlue;
						break;
					case "Summon":
						HPColor1 = Color.MediumPurple;
						HPColor2 = Color.Purple;
						break;
					case "Rogue":
						HPColor1 = Color.LightYellow;
						HPColor2 = Color.LightYellow;
						break;
					case "None":
						HPColor1 = Color.Green;
						HPColor2 = Color.LawnGreen;
						break;
					default:
						HPColor1 = Color.Green;
						HPColor2 = Color.LawnGreen;
						break;
				}
			}
			else
			{
				HPColor1 = Color.Green;
				HPColor2 = Color.LawnGreen;
			}

			// HPBar
			Rectangle HPBar = Frame.GetInnerDimensions().ToRectangle();
			HPBar.X += 8;
			HPBar.Width -= 16;
			HPBar.Y += 6;
			HPBar.Height = (HPBar.Height - 12) / 2;

			float HPQ;
			if (Ally != null)
			{
				HPQ = (float)Ally.statLife / Ally.statLifeMax2;
				HPQ = Utils.Clamp(HPQ, 0f, 1f);
			}
			else HPQ = 1;

			if (Ally != null && ETUDConfig.Instanse.ShowOfflinePlayers)
			{
				if (!Ally.active)
				{
					HPColor1 = Color.LightGray;
					HPColor2 = Color.LightGray;
					HPQ = 1;
				}
			}

			int HPleft = HPBar.Left;
			int HPright = HPBar.Right;
			int HPsteps = (int)((HPright - HPleft) * HPQ);
			for (int i = 0; i < HPsteps; i++)
			{
				float hppercent = (float)i / (HPright - HPleft);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(HPleft + i, HPBar.Y, 1, HPBar.Height), Color.Lerp(HPColor1, HPColor2, hppercent));
			}

			// Resource
			float RQ;
			if (ETUDConfig.Instanse.EnableColorMatch)
			{
				switch (PlayerClass)
				{
					case "Melee":
						RColor1 = Color.IndianRed;
						RColor2 = Color.DarkRed;
						RQ = 1;
						// TODO: Show exact healing potion CD
						break;
					case "Ranged":
						RColor1 = Color.OrangeRed;
						RColor2 = Color.Orange;
						RQ = 1;
						break;
					case "Magic":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "Summon":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "Rogue":
						RColor1 = Color.Yellow;
						RColor2 = Color.Yellow;
						if (Ally != null)
						{
							RQ = CalamityHelper.RogueStealth(Ally) / CalamityHelper.RogueStealthMax(Ally);
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					case "None":
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
					default:
						RColor1 = Color.Blue;
						RColor2 = Color.DeepSkyBlue;
						if (Ally != null)
						{
							RQ = (float)Ally.statMana / Ally.statManaMax2;
							RQ = Utils.Clamp(RQ, 0f, 1f);
						}
						else RQ = 1;
						break;
				}
			}
			else
			{
				RColor1 = Color.Blue;
				RColor2 = Color.DeepSkyBlue;
				if (Ally != null)
				{
					RQ = (float)Ally.statMana / Ally.statManaMax2;
					RQ = Utils.Clamp(RQ, 0f, 1f);
				}
				else RQ = 1;
			}

			// ResourceBar
			Rectangle RBar = Frame.GetInnerDimensions().ToRectangle();
			RBar.X += 8;
			RBar.Width -= 16;
			RBar.Y += 30;
			RBar.Height = (RBar.Height - 12) / 2;

			if (Ally != null && ETUDConfig.Instanse.ShowOfflinePlayers)
			{
				if (!Ally.active)
				{
					RColor1 = Color.Gray;
					RColor2 = Color.Gray;
					RQ = 1;
				}
			}

			int Rleft = RBar.Left;
			int Rright = RBar.Right;
			int Rsteps = (int)((Rright - Rleft) * RQ);
			for (int i = 0; i < Rsteps; i++)
			{
				float Rpercent = (float)i / (Rright - Rleft);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Rleft + i, RBar.Y, 1, RBar.Height), Color.Lerp(RColor1, RColor2, Rpercent));
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			string PlayerClass = MiscEventHandler.DeterminePlayerClass(Ally);

			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			if (allyFound && !Ally.active)
			{
				Ally = null;
				allyFound = false;
			}

			Left.Pixels = ETUDPanel1.MainLeft.Pixels;
			Top.Pixels = ETUDPanel1.MainTop.Pixels + (height + 10) * 2;

			if (Main.LocalPlayer.team == 0) { Name.SetText(""); HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText(""); }
			else if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel2")); HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText("");
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] != null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDPanel1.Ally && Main.player[i] != ETUDPanel2.Ally && (ETUDConfig.Instanse.ShowOfflinePlayers || Main.player[i].active)) // && Main.player[i].active
					{
						Ally = Main.player[i];
						allyFound = true;
						break;
					}
				}
			}

			if (Ally != null && allyFound == true && ETUDPanel1.Ally == null && ETUDPanel1.allyFound == false)
			{
				ETUDPanel1.Ally = Ally;
				ETUDPanel1.allyFound = true;

				Ally = null;
				allyFound = false;
			}

			if (Ally != null && Ally.team != Main.LocalPlayer.team) { Ally = null; allyFound = false; }

			if (Ally != null)
			{
				if (!Ally.dead)
				{
					HPlabel.SetText($"{Ally.statLife}/{Ally.statLifeMax2}");

					if (PlayerClass != "Ranged") Ammolabel.SetText("");

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
						Rlabel.SetText("");
					}
					else if (PlayerClass == "Rogue") if (CalamityHelper.RogueStealth(Ally) == CalamityHelper.RogueStealthMax(Ally) && CalamityHelper.RogueStealthMax(Ally) != 0) Rlabel.SetText("Stealthed "); else Rlabel.SetText("");
					else Rlabel.SetText($"{Ally.statMana}/{Ally.statManaMax2}");

					if (!Ally.active)
					{
						HPlabel.SetText("");
						Rlabel.SetText("");
						Ammolabel.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Offline"));
					}

					Name.SetText(Ally.name);
				}
				else
				{
					Name.SetText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.Dead")} {(Ally.respawnTimer / 60 + 1)}");
					Rlabel.SetText("");
					HPlabel.SetText("");
					Ammolabel.SetText("");
				}
			}
			else if (Main.LocalPlayer.team != 0)
			{
				allyFound = false; Name.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDPanels.NoPlayersLabel2")); HPlabel.SetText(""); Rlabel.SetText(""); Ammolabel.SetText("");
			}
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			if (ETUDConfig.Instanse.LockUIPosition && ETUDConfig.Instanse.AllowOnClickTeleport && Main.LocalPlayer.HasUnityPotion() && Ally != null && !Ally.dead && !Main.LocalPlayer.dead)
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
			if (player == null) return "None";

			if (ModLoader.TryGetMod("CalamityMod", out var mod))
			{
				if (mod.TryFind<DamageClass>("RogueDamageClass", out var rogueclass))
				{
					float RogueCoeff = player.GetTotalDamage(rogueclass).Additive + (player.GetCritChance(rogueclass) / 100) + (player.GetAttackSpeed(rogueclass) / 2);
					float MeleeCoeff = player.GetTotalDamage(DamageClass.Melee).Additive + (player.GetCritChance(DamageClass.Melee) / 100) + (player.GetAttackSpeed(DamageClass.Melee) / 2);
					float RangedCoeff = player.GetTotalDamage(DamageClass.Ranged).Additive + (player.GetCritChance(DamageClass.Ranged) / 100) + (player.GetAttackSpeed(DamageClass.Ranged) / 2);
					float MagicCoeff = player.GetTotalDamage(DamageClass.Magic).Additive + (player.GetCritChance(DamageClass.Magic) / 100) + (player.GetAttackSpeed(DamageClass.Magic) / 2);
					float SummonCoeff = player.GetTotalDamage(DamageClass.Summon).Additive + (player.GetCritChance(DamageClass.Summon) / 100) + (player.GetAttackSpeed(DamageClass.Summon) / 2 + (player.maxMinions - 1));

					if (MeleeCoeff == RangedCoeff && RangedCoeff == MagicCoeff && MagicCoeff == SummonCoeff && SummonCoeff == RogueCoeff) return "None";
					else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff, RogueCoeff }.Max() == MeleeCoeff) return "Melee";
					else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff, RogueCoeff }.Max() == RangedCoeff) return "Ranged";
					else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff, RogueCoeff }.Max() == MagicCoeff) return "Magic";
					else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff, RogueCoeff }.Max() == SummonCoeff) return "Summon";
					else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff, RogueCoeff }.Max() == RogueCoeff) return "Rogue";
					else return "None";
				}
				else return "None";
			}
			else
			{

				float MeleeCoeff = player.GetTotalDamage(DamageClass.Melee).Additive + (player.GetCritChance(DamageClass.Melee) / 100) + (player.GetAttackSpeed(DamageClass.Melee) / 2);
				float RangedCoeff = player.GetTotalDamage(DamageClass.Ranged).Additive + (player.GetCritChance(DamageClass.Ranged) / 100) + (player.GetAttackSpeed(DamageClass.Ranged) / 2);
				float MagicCoeff = player.GetTotalDamage(DamageClass.Magic).Additive + (player.GetCritChance(DamageClass.Magic) / 100) + (player.GetAttackSpeed(DamageClass.Magic) / 2 + ((1 - player.manaCost) * 12));
				float SummonCoeff = player.GetTotalDamage(DamageClass.Summon).Additive + (player.GetCritChance(DamageClass.Summon) / 100) + (player.GetAttackSpeed(DamageClass.Summon) / 2 + (player.maxMinions - 1));


				if (MeleeCoeff == RangedCoeff && RangedCoeff == MagicCoeff && MagicCoeff == SummonCoeff) return "None";
				else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff }.Max() == MeleeCoeff) return "Melee";
				else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff }.Max() == RangedCoeff) return "Ranged";
				else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff }.Max() == MagicCoeff) return "Magic";
				else if (new float[] { MeleeCoeff, RangedCoeff, MagicCoeff, SummonCoeff }.Max() == SummonCoeff) return "Summon";
				else return "None";
			}
		}

		public static bool HasItemInInventory(Player player, int itemtype)
		{
			bool itemFound = false;
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (player.inventory[i].type == itemtype) itemFound = true;
			}
			return itemFound;
		}

		public static int CountItemsInInventory(Player player, int itemtype)
		{
			int itemCount = 0;
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (player.inventory[i].type == itemtype) itemCount += player.inventory[i].stack;
			}
			return itemCount;
		}
	}

	[JITWhenModsEnabled("CalamityMod")]
	public class CalamityHelper
	{
		public static float RogueStealth(Player player)
		{
			return player.GetModPlayer<CalamityMod.CalPlayer.CalamityPlayer>().rogueStealth;
		}

		public static float RogueStealthMax(Player player)
		{
			return player.GetModPlayer<CalamityMod.CalPlayer.CalamityPlayer>().rogueStealthMax;
		}
	}
}
