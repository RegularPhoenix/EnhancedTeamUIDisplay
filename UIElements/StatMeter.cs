using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace EnhancedTeamUIDisplay.DamageCounter
{
	internal class DamageCounterSystem : ModSystem
	{
		internal enum DamageCounterPacketType : byte
		{
			InformClientsOfValues,
			InformServerOfDPS,		
			InformServerOfDealtDamage,
			InformServerOfTakenDamage,
			InformServerOfDeaths,
		}

		public override void PostUpdateWorld()
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			var netMessage = Mod.GetPacket();
			netMessage.Write((byte)DamageCounterPacketType.InformClientsOfValues);

			byte count = (byte)Main.CurrentFrameFlags.ActivePlayersCount;
			netMessage.Write(count);

			for (int i = 0; i < 256; i++)
			{
				Player p = Main.player[i];
				if (p.active)
				{
					netMessage.Write((byte)i);
					netMessage.Write(p.accDreamCatcher ? ETUD.DPSValues[i] : -1);
					netMessage.Write(ETUD.DealtDamageValues[i]);
					netMessage.Write(ETUD.TakenDamageValues[i]);
					netMessage.Write(ETUD.DeathValues[i]);
				}
			}
			netMessage.Send();
		}

		internal static bool AwaitsReset = false;
		internal static bool NoPlayersInCombat = true;
		internal static bool BossFightEndedRecently = false;

		public override void PostUpdatePlayers()
		{		
			base.PostUpdatePlayers();

			if (Main.netMode == NetmodeID.SinglePlayer) return;

			if (ETUDAdditionalOptions.LastFightEndTime != DateTime.MinValue && !BossFightEndedRecently && (DateTime.Now - ETUDAdditionalOptions.LastFightEndTime).TotalSeconds < 20) BossFightEndedRecently = true;

			NoPlayersInCombat = true;

			for (int i = 0; i < 256; i++) if (Main.player[i].active) if (Main.player[i].GetModPlayer<DamageCounterPlayer>().InCombat) NoPlayersInCombat = false;
			
			if (!BossFightEndedRecently)
			{
				// Reset after 10 sec of no combat
				if (!NoPlayersInCombat) AwaitsReset = true;
				if (NoPlayersInCombat && AwaitsReset) ETUD.Instance.ResetVariables();							
			}
			else if (ETUDAdditionalOptions.LastFightEndTime != DateTime.MinValue && !NoPlayersInCombat && DateTime.Now > ETUDAdditionalOptions.LastFightEndTime.AddSeconds(20))
			{
				// Reset after 20 sec of last boss death on fight start
				ETUD.Instance.ResetVariables();
				BossFightEndedRecently = false;				
			}
		}
	}

	internal class DamageCounterPlayer : ModPlayer
	{
		internal bool InCombat = false;
		private DateTime lastCombatTime;

		#region Tracked stats
		public override void PostUpdate()
		{
			if (Player.accDreamCatcher)
			{
				int DPS = Player.dpsStarted ? Player.getDPS() : 0;
				SendInfoToServer(DamageCounterSystem.DamageCounterPacketType.InformServerOfDPS, DPS);
			}

			if (!Main.CurrentFrameFlags.AnyActiveBossNPC)
			{
				if (DateTime.Now > ETUDAdditionalOptions.LastFightEndTime.AddSeconds(20))
				{
					if (InCombat && (DateTime.Now - lastCombatTime).TotalSeconds >= 10) InCombat = false;
				}
				else InCombat = false;
			}
			else InCombat = true;
		}

		// Taken damage
		public override void OnHitByNPC(NPC npc, int damage, bool crit)
		{
			SendInfoToServer(DamageCounterSystem.DamageCounterPacketType.InformServerOfTakenDamage, damage);
			UpdateCombat();
		}

		public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
		{
			SendInfoToServer(DamageCounterSystem.DamageCounterPacketType.InformServerOfTakenDamage, damage);
			UpdateCombat();
		}

		// Dealt damage
		public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			SendInfoToServer(DamageCounterSystem.DamageCounterPacketType.InformServerOfDealtDamage, damage);
			UpdateCombat();
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			SendInfoToServer(DamageCounterSystem.DamageCounterPacketType.InformServerOfDealtDamage, damage);
			UpdateCombat();
		}

		// Deaths
		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) => SendInfoToServer(DamageCounterSystem.DamageCounterPacketType.InformServerOfDeaths);
		#endregion

		//Sending changes
		private void SendInfoToServer(DamageCounterSystem.DamageCounterPacketType pt, int? info = null)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)pt);
				if (info is not null) packet.Write((int)info);
				packet.Send();
			}
		}

		//UpdateCombat
		private void UpdateCombat()
		{
			lastCombatTime = DateTime.Now;
			InCombat = true;
		}
	}

	internal class DamageCounterUI : UIElement
	{
		internal const int width = 200;
		internal const int height = 120;

		internal const int BarWidth = 188;
		internal const int BarHeight = 24;

		private UIText[] UITexts;
		private UIText StatNameText;
		private UIImage image;

		private UIImageButton[] buttons;

		private ETUDPlayer player;

		internal static byte StatNum = 0;		

		public override void OnInitialize()
		{
			Width.Pixels = width;
			Height.Pixels = height;

			player = Main.LocalPlayer.GetModPlayer<ETUDPlayer>();

			Left.Set(player.DCLeftOffset == 0 ? player.PanelLeftOffset - 300 : player.DCLeftOffset, 0f);
			Top.Set(player.DCTopOffset == 0 ? player.PanelTopOffset : player.DCTopOffset, 0f);

			image = new(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/StatMeter/DamageCounterPanelTop"));
			Append(image);

			StatNameText = new UIText("", .8f);
			StatNameText.Top.Set(8, 0f);
			StatNameText.Width.Set(120, 0);
			StatNameText.HAlign = .13f;
			StatNameText.TextOriginX = 0;
			Append(StatNameText);

			UITexts = new UIText[]
			{ 
				new UIText("", .8f),
				new UIText("", .8f),
				new UIText("", .8f),
				new UIText("", .8f)
			};

			for(int i = 0; i < 4; i++)
			{			
				UITexts[i].Top.Set(32 + ((BarHeight + 4) * i), 0f);
				UITexts[i].Width.Set(140, 0);
				UITexts[i].HAlign = .86f;
				UITexts[i].TextOriginX = 1;
				Append(UITexts[i]);
			}

			buttons = new UIImageButton[]
			{
				new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/StatMeter/ArrowLeft")),
				new UIImageButton(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/StatMeter/ArrowRight")),
			};

			buttons[0].OnClick += (e, l) => { if (StatNum == 0) StatNum = 3; else StatNum--; };
			buttons[1].OnClick += (e, l) => { if (StatNum == 3) StatNum = 0; else StatNum++; };
			
			for (int i = 0; i < 2; i++)
			{
				buttons[i].Width.Set(18, 0f);
				buttons[i].Height.Set(22, 0f);
				buttons[i].HAlign = .85f + .1f * i;
				buttons[i].Top.Set(i == 2 ? 3 : 4, 0f);
				Append(buttons[i]);
			}
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);

			Rectangle Bar = GetInnerDimensions().ToRectangle();
			Bar.X += 6;
			Bar.Width = BarWidth;
			Bar.Y += 27;
			Bar.Height = BarHeight;

			for (int i = 0; i < 4; i++) UITexts[i].SetText("");
			StatNameText.SetText("");

			int[] SourceStatArray = new int[256];

			switch (StatNum)
			{
				case 0:
					Array.Copy(ETUD.DPSValues, SourceStatArray, 256);
					StatNameText.SetText("DPS:");
					break;
				case 1:
					Array.Copy(ETUD.DeathValues, SourceStatArray, 256);
					StatNameText.SetText("Deaths:");
					break;
				case 2:
					Array.Copy(ETUD.TakenDamageValues, SourceStatArray, 256);
					StatNameText.SetText("Damage taken:");
					break;
				case 3:
					Array.Copy(ETUD.DealtDamageValues, SourceStatArray, 256);
					StatNameText.SetText("Damage dealt:");
					break;
			}

			int PlayersCount = SourceStatArray.Count(x => x != -1);

			if (PlayersCount == 0)
			{
				spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/StatMeter/DamageCounterPanelBottom").Value, new Rectangle(Bar.X - 6, Bar.Y + 20, 200, 10), Color.White);
				UITexts[0].SetText("No data");
				return;
			}

			int[][] StatValues = new int[PlayersCount][];

			int c = 0;
			for (int i = 0; i < 256; i++) if(SourceStatArray[i] != -1) { StatValues[c] = new int[] { i, SourceStatArray[i] }; c++; }

			Array.Sort(StatValues, (x, y) => Comparer<int>.Default.Compare(y[1], x[1]));
			int HighestValue = StatValues[0][1];

			Player HighestStatPlayer = Main.player[StatValues[0][0]];
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, Bar, MiscEventHandler.GetClassColours(MiscEventHandler.DeterminePlayerClass(HighestStatPlayer)).Item1);

			UITexts[0].SetText($"{HighestStatPlayer.name}({HighestValue})");

			int PlayerCountToDraw = new int[2] { ETUDConfig.Instanse.DCPlayersToShowAmount, PlayersCount }.Min();

			if (PlayerCountToDraw > 1)
				for (int i = 1; i < PlayerCountToDraw; i++)
				{
					Bar.Y += Bar.Height + 4;
					float currentValue = StatValues[i][1];
					Player currentPlayer = Main.player[StatValues[i][0]];
					spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(Bar.X, Bar.Y, currentValue != 0 ? (int)(Bar.Width * (currentValue / HighestValue)) : 1, Bar.Height), MiscEventHandler.GetClassColours(MiscEventHandler.DeterminePlayerClass(currentPlayer)).Item1);

					UITexts[i].SetText($"{currentPlayer.name}({currentValue})");

					spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/StatMeter/DamageCounterPanelMid").Value, new Rectangle(Bar.X - 6, Bar.Y, 200, 28), Color.White);
				}

			spriteBatch.Draw(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/StatMeter/DamageCounterPanelBottom").Value, new Rectangle(Bar.X - 6, Bar.Y + 20, 200, 10), Color.White);
		}

		#region Dragging

		private Vector2 offset;
		public bool dragging;

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			player.DCLeftOffset = (int)Left.Pixels;
			player.DCTopOffset = (int)Top.Pixels;

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

		public override void MouseDown(UIMouseEvent evt)
		{
			base.MouseDown(evt);
			DragStart(evt);
		}

		public override void MouseUp(UIMouseEvent evt)
		{
			base.MouseUp(evt);
			DragEnd(evt);
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

			player.DCLeftOffset = (int)Left.Pixels;
			player.DCTopOffset = (int)Top.Pixels;

			Recalculate();
		}
		#endregion
	}
}
