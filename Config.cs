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
		public int PanelAmount;

		[DefaultValue(false)]
		public bool LockUIPosition;

		[DefaultValue(true)]
		public bool AllowOnClickTeleport;

		[DefaultValue(false)]
		public bool EnableAutoToggle;

		[DefaultValue(true)]
		public bool ShowOfflinePlayers;

		[DefaultValue(true)]
		public bool EnableColorMatch;

		[Header("$Mods.EnhancedTeamUIDisplay.Configs.Config.DamageMeterOptionsHeader")]

		[DefaultValue(true)]
		public bool EnableDamageMeter;

		[DrawTicks]
		[Increment(1)]
		[Range(1, 4)]
		[DefaultValue(4)]
		[Slider]
		public int DamageMeterPlayerCountToShow;

		[Header("$Mods.EnhancedTeamUIDisplay.Configs.Config.AdditionalFeaturesHeader")]

		[DefaultValue(true)]
		public bool ShowBuffCheckButton;

		[DefaultValue(true)]
		public bool ShowErrorMessages;

		public override void OnChanged() {
			base.OnChanged();

			if (!LockUIPosition && AllowOnClickTeleport)
				AllowOnClickTeleport = false;

			if (ETUDUI.MainInterface is not null && Main.netMode != NetmodeID.SinglePlayer) {
				ETUDUI.CloseMainInterface();

				if (!EnableAutoToggle)
					ETUDUI.OpenMainInterface();
			}
		}
	}
}
