using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDUI1 : UIPanel // LEGACY CONTENT - No longer supported or updated in versions 1.0+
	{
		private UIText PlayerNameText;
		private UIText PlayerHPText;
		private UIText PlayerMPText;
		private UIText PlayerRespawnTime;

		internal static Player Ally;
		internal static bool allyFound;

		internal static StyleDimension MainTop;
		internal static StyleDimension MainLeft;

		internal const int width = 200;
		internal const int height = 60;

		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;

			ETUDUIHelper.DefaultColor = BackgroundColor;

			PlayerNameText = new UIText("")
			{
				Top = { Pixels = -PaddingTop / 2 },
				HAlign = .5f
			};
			Append(PlayerNameText);

			PlayerHPText = new UIText("")
			{
				Top = { Pixels = (-PaddingTop / 2) + 30 },
				HAlign = .05f
			};
			Append(PlayerHPText);

			PlayerMPText = new UIText("")
			{
				Top = { Pixels = (-PaddingTop / 2) + 30 },
				HAlign = .95f
			};
			Append(PlayerMPText);

			PlayerRespawnTime = new UIText("")
			{
				Top = { Pixels = -PaddingTop / 2 + 30 },
				HAlign = .5f
			};
			Append(PlayerRespawnTime);

			//Left.Pixels = ETUDPlayer.LeftOffset;
			//Top.Pixels = ETUDPlayer.TopOffset;
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			ETUDUIHelper.changeBGColor(this, Ally);
			
			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			MainLeft = Left; //ETUDPlayer.LeftOffset = (int)Left.Pixels;
			MainTop = Top; //ETUDPlayer.TopOffset = (int)Top.Pixels;


			if (Main.LocalPlayer.team == 0) { PlayerNameText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.EnterTeamLine1")); PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.EnterTeamLine2")); PlayerHPText.SetText(""); PlayerMPText.SetText(""); }
			else if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				PlayerNameText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine1")); PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine2")); PlayerHPText.SetText(""); PlayerMPText.SetText("");
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] is not null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDUI3.Ally && Main.player[i] != ETUDUI2.Ally)
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
				PlayerNameText.SetText(Ally.name);
				if (!Ally.dead)
				{
					PlayerHPText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.HPLabel") + Ally.statLife.ToString());
					PlayerMPText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.MPLabel") + Ally.statMana.ToString());
					PlayerRespawnTime.SetText("");
				}
				else
				{
					PlayerHPText.SetText("");
					PlayerMPText.SetText("");
					PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.RespawnLabel") + (Ally.respawnTimer / 60 + 1).ToString());
				}
			}
			else if (Main.LocalPlayer.team != 0)
			{
				
				allyFound = false; PlayerNameText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine1")); PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine2")); PlayerHPText.SetText(""); PlayerMPText.SetText("");
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

			Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			if (IsMouseHovering && Ally is not null) Main.instance.MouseText("Teleport to " + Ally.name, 0, 0);
		}
	}

	internal class ETUDUI2 : UIPanel
	{
		private UIText PlayerNameText;
		private UIText PlayerHPText;
		private UIText PlayerMPText;
		private UIText PlayerRespawnTime;

		internal static Player Ally;
		internal static bool allyFound;

		internal const int width = 200;
		internal const int height = 60;

		internal int RelativeLeft => (int)(Main.screenWidth * .76f) - width / 2;
		internal int RelativeTop => (int)(Main.screenHeight * .12f);

		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;

			Top.Pixels = RelativeTop;
			Left.Pixels = RelativeLeft;

			PlayerNameText = new UIText("")
			{
				Top = { Pixels = -PaddingTop / 2 },
				HAlign = .5f
			};
			Append(PlayerNameText);

			PlayerHPText = new UIText("")
			{
				Top = { Pixels = (-PaddingTop / 2) + 30 },
				HAlign = .05f
			};
			Append(PlayerHPText);

			PlayerMPText = new UIText("")
			{
				Top = { Pixels = (-PaddingTop / 2) + 30 },
				HAlign = .95f
			};
			Append(PlayerMPText);

			PlayerRespawnTime = new UIText("")
			{
				Top = { Pixels = -PaddingTop / 2 + 30 },
				HAlign = .5f
			};
			Append(PlayerRespawnTime);

			ETUDUIHelper.changeBGColor(this, Ally);
			ETUDUIHelper.matchMainPosition(this, height, 1);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			ETUDUIHelper.changeBGColor(this, Ally);

			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			ETUDUIHelper.matchMainPosition(this, height, 1);

			if (Main.LocalPlayer.team == 0) { PlayerNameText.SetText(""); PlayerRespawnTime.SetText(""); PlayerHPText.SetText(""); PlayerMPText.SetText(""); }
			else if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				PlayerNameText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine1")); PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine2")); PlayerHPText.SetText(""); PlayerMPText.SetText("");
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] is not null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDUI1.Ally && Main.player[i] != ETUDUI3.Ally)
					{
						Ally = Main.player[i];
						allyFound = true;
						break;
					}
				}
			}

			if(Ally is not null && allyFound == true && ETUDUI1.Ally is null && ETUDUI1.allyFound == false)
			{
				ETUDUI1.Ally = Ally;
				ETUDUI1.allyFound = true;

				Ally = null;
				allyFound = false;
			}

			if (Ally is not null && Ally.team != Main.LocalPlayer.team) { Ally = null; allyFound = false; }			

			if (Ally is not null)
			{
				PlayerNameText.SetText(Ally.name);
				if (!Ally.dead)
				{
					PlayerHPText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.HPLabel") + Ally.statLife.ToString());
					PlayerMPText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.MPLabel") + Ally.statMana.ToString());
					PlayerRespawnTime.SetText("");
				}
				else
				{
					PlayerHPText.SetText("");
					PlayerMPText.SetText("");
					PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.RespawnLabel") + (Ally.respawnTimer / 60 + 1).ToString());
				}
			}
			else if (Main.LocalPlayer.team != 0)
			{
				allyFound = false; PlayerNameText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine1")); PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine2")); PlayerHPText.SetText(""); PlayerMPText.SetText("");
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

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			if (IsMouseHovering && Ally is not null) Main.instance.MouseText("Teleport to " + Ally.name, 0, 0);
		}
	}

	internal class ETUDUI3 : UIPanel
	{
		private UIText PlayerNameText;
		private UIText PlayerHPText;
		private UIText PlayerMPText;
		private UIText PlayerRespawnTime;

		internal static Player Ally;
		internal static bool allyFound;

		internal const int width = 200;
		internal const int height = 60;

		internal int RelativeLeft => (int)(Main.screenWidth * .76f) - width / 2;
		internal int RelativeTop => (int)(Main.screenHeight * .19f);

		public override void OnInitialize()
		{
			allyFound = false;
			Width.Pixels = width;
			Height.Pixels = height;

			Top.Pixels = RelativeTop;
			Left.Pixels = RelativeLeft;

			PlayerNameText = new UIText("")
			{
				Top = { Pixels = -PaddingTop / 2 },
				HAlign = .5f
			};
			Append(PlayerNameText);

			PlayerHPText = new UIText("")
			{
				Top = { Pixels = (-PaddingTop / 2) + 30 },
				HAlign = .05f
			};
			Append(PlayerHPText);

			PlayerMPText = new UIText("")
			{
				Top = { Pixels = (-PaddingTop / 2) + 30 },
				HAlign = .95f
			};
			Append(PlayerMPText);

			PlayerRespawnTime = new UIText("")
			{
				Top = { Pixels = -PaddingTop / 2 + 30 },
				HAlign = .5f
			};
			Append(PlayerRespawnTime);

			ETUDUIHelper.changeBGColor(this, Ally);
			ETUDUIHelper.matchMainPosition(this, height, 2);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			ETUDUIHelper.changeBGColor(this, Ally);

			if (allyFound && Main.LocalPlayer.team != Ally.team) allyFound = false;

			ETUDUIHelper.matchMainPosition(this, height, 2);

			if (Main.LocalPlayer.team == 0) { PlayerNameText.SetText(""); PlayerRespawnTime.SetText(""); PlayerHPText.SetText(""); PlayerMPText.SetText(""); }
			else if (!allyFound && Main.netMode != NetmodeID.SinglePlayer && Main.LocalPlayer.team != 0)
			{
				PlayerNameText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine1")); PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine2")); PlayerHPText.SetText(""); PlayerMPText.SetText("");
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i] is not null && Main.player[i].team == Main.LocalPlayer.team && Main.player[i] != Main.LocalPlayer && Main.player[i] != ETUDUI1.Ally && Main.player[i] != ETUDUI2.Ally)
					{
						Ally = Main.player[i];
						allyFound = true;
						break;
					}
				}
			}

			if (Ally is not null && allyFound == true && ETUDUI2.Ally is null && ETUDUI2.allyFound == false)
			{
				ETUDUI2.Ally = Ally;
				ETUDUI2.allyFound = true;

				Ally = null;
				allyFound = false;
			}

			if (Ally is not null && Ally.team != Main.LocalPlayer.team) { Ally = null; allyFound = false; }

			if (Ally is not null)
			{
				PlayerNameText.SetText(Ally.name);
				if (!Ally.dead)
				{
					PlayerHPText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.HPLabel") + Ally.statLife.ToString());
					PlayerMPText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.MPLabel") + Ally.statMana.ToString());
					PlayerRespawnTime.SetText("");
				}
				else
				{
					PlayerHPText.SetText("");
					PlayerMPText.SetText("");
					PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.RespawnLabel") + (Ally.respawnTimer / 60 + 1).ToString());
				}
			}
			else if(Main.LocalPlayer.team != 0)
			{
				allyFound = false; PlayerNameText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine1")); PlayerRespawnTime.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.ETUDUI.PlayersNotFoundLine2")); PlayerHPText.SetText(""); PlayerMPText.SetText("");
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

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			if (IsMouseHovering && Ally is not null) Main.instance.MouseText("Teleport to " + Ally.name, 0, 0);
		}
	}

	internal class ETUDUIHelper
	{
		internal static Color DefaultColor;

		internal static void changeBGColor(UIPanel panel, Player ally)
		{
			if (ETUDConfig.Instanse.EnableColorMatch)
			{
				if (Main.LocalPlayer.team == 0) panel.BackgroundColor = Color.Gray;
				else
				{
					panel.BackgroundColor.R = Main.teamColor[Main.LocalPlayer.team].R;
					panel.BackgroundColor.G = Main.teamColor[Main.LocalPlayer.team].G;
					panel.BackgroundColor.B = Main.teamColor[Main.LocalPlayer.team].B;
				}
				panel.BackgroundColor.A = 75;
			}
			else panel.BackgroundColor = DefaultColor;

			/*if (true)
			{
				float meleedamage = ally.GetDamage(Terraria.ModLoader.DamageClass.Melee).Flat;
				float rangeddamage = ally.GetDamage(Terraria.ModLoader.DamageClass.Ranged).Flat;
				float magicdamage = ally.GetDamage(Terraria.ModLoader.DamageClass.Magic).Flat;
				float summondamage = ally.GetDamage(Terraria.ModLoader.DamageClass.Summon).Flat;

				var max = new float[] { meleedamage, rangeddamage, magicdamage, summondamage }.Max();
			}*/
		}

		internal static void matchMainPosition(UIPanel panel, int height, int row)
		{
			panel.Left.Pixels = ETUDUI1.MainLeft.Pixels;
			panel.Top.Pixels = ETUDUI1.MainTop.Pixels + row * (height + 10);
		}
	}
}
