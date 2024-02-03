using System;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

#pragma warning disable 649

namespace EnhancedTeamUIDisplay
{
	internal class Config : ModConfig
	{
		public override ConfigScope Mode
			=> ConfigScope.ClientSide;

		public static Config Instanse
			=> ModContent.GetInstance<Config>();

		[Header("$Mods.EnhancedTeamUIDisplay.Configs.Config.PanelHeader")]

		[DrawTicks]
		[Increment(1)]
		[Range(1, 5)]
		[DefaultValue(1)]
		public int MaxPanelAmount;

		[Obsolete("Will be removed from config")]
		[DefaultValue(false)]
		public bool IsUILocked;

		[DefaultValue(true)]
		public bool IsOnClickTeleportEnabled;

		[DefaultValue(false)]
		public bool IsAutoToggleEnabled;

		[DefaultValue(true)]
		public bool AreOfflinePlayersDisplayed;

		[DefaultValue(true)]
		public bool IsColorMatchEnabled;

		[Header("$Mods.EnhancedTeamUIDisplay.Configs.Config.DamageMeterOptionsHeader")]

		[DefaultValue(true)]
		public bool IsDamageMeterEnabled;

		[DefaultValue(true)]
		public bool IsAutoResetEnabled;

		[DrawTicks]
		[Increment(1)]
		[Range(1, 4)]
		[DefaultValue(4)]
		[Slider]
		public int DamageMeterMaxPlayerCount;

		[Header("$Mods.EnhancedTeamUIDisplay.Configs.Config.AdditionalFeaturesHeader")]

		[DefaultValue(true)]
		public bool IsPlayerTeamSaved;

		[DefaultValue(true)]
		public bool IsEquipmentCheckButtonEnabled;

		[DefaultValue(true)]
		public bool AreErrorMessagesDisplayed;

		public override void OnChanged() {
			base.OnChanged();

			if (!IsUILocked && IsOnClickTeleportEnabled)
				IsOnClickTeleportEnabled = false;

			if (ETUDUI.MainInterface is not null && Main.netMode != NetmodeID.SinglePlayer) {
				ETUDUI.CloseMainInterface();

				if (!IsAutoToggleEnabled)
					ETUDUI.OpenMainInterface();
			}
		}
	}
}
