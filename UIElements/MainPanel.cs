using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace EnhancedTeamUIDisplay.UIElements
{
	internal class MainPanel : UIElement
	{
		internal MainPanel(int number)
			=> PanelNumber = number;

		internal const int ElementWidth = 200, ElementHeight = 60;

		internal Player Ally { get; set; }

		private int PanelNumber { get; }

		private UIImage _frameImage;
		private UIText _nameText, _healthText, _resourceText, _ammoText;

		public override void OnInitialize() {
			Width.Pixels = ElementWidth;
			Height.Pixels = ElementHeight;

			/*
			 * Layout:
			 * 
			 *  --Frame---------
			 * | Name       HP  |
			 * | Ammo       Res |
			 *  ----------------
			 */

			// Frame
			_frameImage = new UIImage(ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/MainPanel/Frame"));
			_frameImage.Width.Set(ElementWidth, 0f);
			_frameImage.Height.Set(ElementHeight, 0f);
			Append(_frameImage);

			// Name
			_nameText = new UIText(string.Empty, 0.7f);
			_nameText.Top.Set(12, 0f);
			_nameText.Width.Set(80, 0f);
			_nameText.HAlign = .15f;
			_nameText.TextOriginX = 0;
			Append(_nameText);

			// HP
			_healthText = new UIText(string.Empty, 0.7f);
			_healthText.Top.Set(12, 0f);
			_healthText.Width.Set(90, 0f);
			_healthText.HAlign = .85f;
			_healthText.TextOriginX = 1;
			Append(_healthText);

			// Resource (Mana or smth different from modded classes)
			_resourceText = new UIText(string.Empty, 0.7f);
			_resourceText.Top.Set(37, 0f);
			_resourceText.Width.Set(90, 0f);
			_resourceText.HAlign = .85f;
			_resourceText.TextOriginX = 1;
			Append(_resourceText);

			// Ammo
			_ammoText = new UIText(string.Empty, 0.7f);
			_ammoText.Top.Set(37, 0f);
			_ammoText.Width.Set(90, 0f);
			_ammoText.HAlign = .15f;
			_ammoText.TextOriginX = 0;
			Append(_ammoText);

			Left.Set(Main.LocalPlayer.GetModPlayer<ETUDPlayer>().MainPanelLeftOffset, 0f);
			Top.Set(Main.LocalPlayer.GetModPlayer<ETUDPlayer>().MainPanelTopOffset, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			Rectangle frame = _frameImage.GetInnerDimensions().ToRectangle();
			spriteBatch.Draw(
				ModContent.Request<Texture2D>("EnhancedTeamUIDisplay/Sprites/MainPanel/Background").Value,
				new Rectangle(frame.X + 8, frame.Y + 6, 184, 48), Color.White
			);

			// Hover text
			if (IsMouseHovering) {
				if (Ally is not null && Config.Instanse.IsOnClickTeleportEnabled) {
					Main.instance.MouseText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.Teleport")} {Ally.name}");
				} else if (!Config.Instanse.IsUILocked) {
					Main.instance.MouseText(Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.Unfrozen").Value);
				}
			}

			Util.PlayerClass allyClass = Util.GuessPlayerClass(Ally);

			// HP
			(Color healthColor, Color resourceColor) = Util.GetClassColours(
				Main.LocalPlayer.team != 0
				? (Config.Instanse.IsColorMatchEnabled
					? allyClass
					: Util.PlayerClass.None)
				: Util.PlayerClass.Offline
			);

			Rectangle healthBar = _frameImage.GetInnerDimensions().ToRectangle();
			healthBar.X += 8;
			healthBar.Width -= 16;
			healthBar.Y += 6;
			healthBar.Height = (healthBar.Height - 12) / 2;

			float healthRelation = 1;
			if (Ally is not null) {
				healthRelation = (float) Ally.statLife / Ally.statLifeMax2;
				healthRelation = Utils.Clamp(healthRelation, 0f, 1f);
			}

			int healthBarLeft = healthBar.Left;
			int healthBarRight = healthBar.Right;
			int healthBarSteps = (int) ((healthBarRight - healthBarLeft) * healthRelation);

			for (int i = 0; i < healthBarSteps; i++) {
				spriteBatch.Draw(
					TextureAssets.MagicPixel.Value,
					new Rectangle(healthBarLeft + i, healthBar.Y, 1, healthBar.Height),
					healthColor
				);
			}

			// Resource
			float resourceRelation = Util.GetClassResourceRelation(Ally, allyClass);

			Rectangle resourceBar = _frameImage.GetInnerDimensions().ToRectangle();
			resourceBar.X += 8;
			resourceBar.Width -= 16;
			resourceBar.Y += 30;
			resourceBar.Height = (resourceBar.Height - 12) / 2;

			int resourceBarLeft = resourceBar.Left;
			int resourceBarRight = resourceBar.Right;
			int resourceBarSteps = (int) ((resourceBarRight - resourceBarLeft) * resourceRelation);

			for (int i = 0; i < resourceBarSteps; i++) {
				spriteBatch.Draw(
					TextureAssets.MagicPixel.Value,
					new Rectangle(resourceBarLeft + i, resourceBar.Y, 1, resourceBar.Height),
					resourceColor
				);
			}
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			// If this is a leader panel, move it when dragging
			if (PanelNumber == 0 && !Config.Instanse.IsUILocked) {
				if (ContainsPoint(Main.MouseScreen)) {
					Main.LocalPlayer.mouseInterface = true;
				}

				if (_dragging) {
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

			// Otherwise, align it properly
			if (PanelNumber != 0) {
				Left.Pixels = ETUDUI.MainPanels[0].Left.Pixels;
				Top.Pixels = ETUDUI.MainPanels[0].Top.Pixels + (PanelNumber * (ElementHeight + 10));
			}

			// Reset all labels
			_nameText.SetText(string.Empty);
			_resourceText.SetText(string.Empty);
			_healthText.SetText(string.Empty);
			_ammoText.SetText(string.Empty);

			if (Main.LocalPlayer.team == 0) {
				if (PanelNumber == 0)
					_nameText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.NoTeam"));

				return;
			}

			// Set all labels
			if (Ally is not null) {
				Util.PlayerClass allyClass = Util.GuessPlayerClass(Ally);

				_nameText.SetText(Ally.name.Length > 15 ? Ally.name[..12] + "..." : Ally.name);

				if (!Ally.active) {
					_ammoText.SetText(Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.Offline"));
				} else if (!Ally.dead) {
					// HACK: Use packets instead of referencing player stats through Player directly?
					// BUG: Lifeforce potion? causes health to reach absurtly high values; Unconfirmed
					_healthText.SetText($"{Ally.statLife}/{Ally.statLifeMax2}");

					switch (allyClass) {
						case Util.PlayerClass.Melee:
							_resourceText.SetText(
								(
									Ally.HasBuff(BuffID.PotionSickness)
									? Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.PotionCooldown")
									: Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.PotionReady")
								).Value
							);
							break;

						case Util.PlayerClass.Ranger:
							int bulletCount = 0, arrowCount = 0;

							for (int i = 0; i < Ally.inventory.Length; i++) {
								if (Ally.inventory[i].ammo == AmmoID.Bullet)
									bulletCount += Ally.inventory[i].stack;

								if (Ally.inventory[i].ammo == AmmoID.Arrow)
									arrowCount += Ally.inventory[i].stack;
							}

							_ammoText.SetText(
								$"{Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.Bullets")} {(bulletCount > 9999 ? "9999+" : bulletCount)} " +
								$"{Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.Arrows")} {(arrowCount > 9999 ? "9999+" : arrowCount)}"
							);
							break;

						case Util.PlayerClass.Rogue:
							_resourceText.SetText(
								Language.GetText(
									CrossModHelper.GetRogueStealth(Ally) < CrossModHelper.GetRogueStealthMax(Ally)
									? "Mods.EnhancedTeamUIDisplay.MainPanel.Overt"
									: "Mods.EnhancedTeamUIDisplay.MainPanel.Hidden"
								)
							);
							break;

						case Util.PlayerClass.Bard:
							_resourceText.SetText($"{CrossModHelper.GetBardInspiration(Ally)}/{CrossModHelper.GetBardInspirationMax(Ally)}");
							break;

						default:
							_resourceText.SetText($"{Ally.statMana}/{Ally.statManaMax2}");
							break;
					}
				} else {
					_ammoText.SetText($"{Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.Dead")} {(Ally.respawnTimer / 60) + 1}");
				}
			} else {
				_nameText.SetText(
					PanelNumber == 0
					? Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.NoPlayers")
					: Language.GetText("Mods.EnhancedTeamUIDisplay.MainPanel.NoOtherPlayers")
				);
			}
		}

		private Vector2 _offset;
		private bool _dragging;

		public override void LeftMouseDown(UIMouseEvent evt) {
			if (PanelNumber == 0 && !Config.Instanse.IsUILocked) {
				base.LeftMouseDown(evt); // HACK: Probably remove to prevent inventory interaction
				DragStart(evt);
			}
		}

		public override void LeftMouseUp(UIMouseEvent evt) {
			if (PanelNumber == 0 && !Config.Instanse.IsUILocked) {
				base.LeftMouseUp(evt);
				DragEnd(evt);
			} else if (Config.Instanse.IsUILocked
				  && Config.Instanse.IsOnClickTeleportEnabled
				  && Main.LocalPlayer.HasUnityPotion()
				  && Ally is not null
				  && !Ally.dead
				  && !Main.LocalPlayer.dead
			  ) {
				Main.LocalPlayer.UnityTeleport(Ally.TopLeft);
				Main.LocalPlayer.TakeUnityPotion();
			}
		}

		private void DragStart(UIMouseEvent evt) {
			_offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
			_dragging = true;
		}

		private void DragEnd(UIMouseEvent evt) {
			Vector2 end = evt.MousePosition;
			_dragging = false;

			Left.Set(end.X - _offset.X, 0f);
			Top.Set(end.Y - _offset.Y, 0f);

			Main.LocalPlayer.GetModPlayer<ETUDPlayer>().MainPanelLeftOffset = (int) Left.Pixels;
			Main.LocalPlayer.GetModPlayer<ETUDPlayer>().MainPanelTopOffset = (int) Top.Pixels;

			Recalculate();
		}
	}
}
