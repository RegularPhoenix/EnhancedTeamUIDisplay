using EnhancedTeamUIDisplay.UIElements;
using Terraria;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDPlayer : ModPlayer
	{
		public int MainPanelTopOffset { get; set; }
		public int MainPanelLeftOffset { get; set; }

		public int DamageMeterTopOffset { get; set; }
		public int DamageMeterLeftOffset { get; set; }

		private int _team;

		public override void ProcessTriggers(TriggersSet triggersSet) {
			if (ETUD.UIHotkey.JustPressed && Main.netMode != NetmodeID.SinglePlayer)
				ETUDUI.ToggleMainInterface();
		}

		public override void OnEnterWorld() {
			ETUDUI.CloseMainInterface();

			if (Main.netMode == NetmodeID.SinglePlayer) {
				Main.NewText(Language.GetText("Mods.EnhancedTeamUIDisplay.Announcements.Singleplayer"), Util.ETUDTextColor);
			} else {
				ChatHelper.BroadcastChatMessage(Language.GetText("Mods.EnhancedTeamUIDisplay.Announcements.Multiplayer").ToNetworkText(), Util.ETUDTextColor); // TODO: Remove after some time

				if (Config.Instanse.KeepPlayerTeam && _team != 0) {
					Main.LocalPlayer.team = _team;
					NetMessage.SendData(MessageID.PlayerTeam, number: Main.myPlayer);
				}
			}
		}

		public override void SaveData(TagCompound tag) {
			tag.Set("MainPanelTopOffset", MainPanelTopOffset, true);
			tag.Set("MainPanelLeftOffset", MainPanelLeftOffset, true);

			tag.Set("DamageMeterTopOffset", DamageMeterTopOffset, true);
			tag.Set("DamageMeterLeftOffset", DamageMeterLeftOffset, true);

			tag.Set("Team", Main.LocalPlayer.team, true);
		}

		public override void LoadData(TagCompound tag) {
			MainPanelTopOffset =
				tag.ContainsKey("MainPanelTopOffset")
					? (int) tag["MainPanelTopOffset"]
					: -600 - MainPanel.width;

			MainPanelLeftOffset =
				tag.ContainsKey("MainPanelLeftOffset")
					? (int) tag["MainPanelLeftOffset"]
					: 30;

			DamageMeterTopOffset =
				tag.ContainsKey("DamageMeterTopOffset")
					? (int) tag["DamageMeterTopOffset"]
					: MainPanelTopOffset;

			DamageMeterLeftOffset =
				tag.ContainsKey("DamageMeterLeftOffset")
					? (int) tag["DamageMeterLeftOffset"]
					: MainPanelLeftOffset - 300;

			if (tag.ContainsKey("Team"))
				_team = (int) tag["Team"];
		}
	}

	internal class DamageMeterPlayer : ModPlayer
	{
		internal int[] DPSTable, DeathsTable, TakenDamageTable, DealtDamageTable;

		internal void ResetTables() {
			for (int i = 0; i < 256; i++) {
				DPSTable[i]
					= DeathsTable[i]
					= TakenDamageTable[i]
					= DealtDamageTable[i]
					= -1;
			}
		}

		public override void OnEnterWorld() {
			DPSTable = new int[256];
			DeathsTable = new int[256];
			TakenDamageTable = new int[256];
			DealtDamageTable = new int[256];
			ResetTables();
		}

		public override void PostUpdate() {
			if (Player.accDreamCatcher) {
				int dps = Player.dpsStarted ? Player.getDPS() : 0;
				SendInfoToServer(
					DamageMeter.DamageMeterPacketType.InformServerOfDPS,
					dps
				);
			}
		}

		public override void OnHurt(Player.HurtInfo info) {
			SendInfoToServer(
				DamageMeter.DamageMeterPacketType.InformServerOfTakenDamage,
				info.Damage
			);
		}

		public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) {
			SendInfoToServer(
				DamageMeter.DamageMeterPacketType.InformServerOfDealtDamage,
				damageDone
			);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) {
			SendInfoToServer(
				DamageMeter.DamageMeterPacketType.InformServerOfDealtDamage,
				damageDone
			);
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
			SendInfoToServer(
				DamageMeter.DamageMeterPacketType.InformServerOfDeaths
			);
		}

		private void SendInfoToServer(DamageMeter.DamageMeterPacketType type, int? info = null) {
			if (Main.netMode == NetmodeID.MultiplayerClient && Player.whoAmI == Main.myPlayer) {
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte) type);

				if (info is not null)
					packet.Write((int) info);

				packet.Send();
			}
		}
	}
}
