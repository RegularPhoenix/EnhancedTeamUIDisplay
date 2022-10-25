using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		public static ETUDConfig Instanse => ModContent.GetInstance<ETUDConfig>();

		[Header("$Mods.EnhancedTeamUIDisplay.Config.PanelHeader")]

		[DrawTicks]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.PanelAmount.Label")]
		[OptionStrings(new string[] { "One panel", "Two panels", "Three panels" })]
		[DefaultValue("One panel")]
		public string PanelAmount;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.LockUIPosition.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.LockUIPosition.Label")]
		[DefaultValue(false)]
		public bool LockUIPosition;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.AllowOnClickTeleport.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.AllowOnClickTeleport.Label")]
		[DefaultValue(true)]
		public bool AllowOnClickTeleport;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.EnableAutoToggle.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.EnableAutoToggle.Label")]
		[DefaultValue(false)]		
		public bool EnableAutoToggle;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.ShowOfflinePlayers.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.ShowOfflinePlayers.Label")]
		[DefaultValue(true)]
		public bool ShowOfflinePlayers;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.EnableColorMatch.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.EnableColorMatch.Label")]
		[DefaultValue(true)]
		public bool EnableColorMatch;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.EnableLegacyUI.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.EnableLegacyUI.Label")]
		[DefaultValue(false)]
		public bool EnableLegacyUI;

		[Header("$Mods.EnhancedTeamUIDisplay.Config.DCOptionsHeader")]

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.EnableDamageCounter.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.EnableDamageCounter.Label")]
		[DefaultValue(true)]
		public bool EnableDamageCounter;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.AutoResetDamageCounter.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.AutoResetDamageCounter.Label")]
		[DefaultValue(true)]
		public bool AutoResetDamageCounter;

		[Header("$Mods.EnhancedTeamUIDisplay.Config.AddOptionsHeader")]

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.ShowBuffCheckButton.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.ShowBuffCheckButton.Label")]
		[DefaultValue(true)]
		public bool ShowBuffCheckButton;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.ShowBossSummary.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.ShowBossSummary.Label")]
		[DefaultValue(true)]
		public bool ShowBossSummary;

		[Tooltip("$Mods.EnhancedTeamUIDisplay.Config.ShowErrorMessages.Tooltip")]
		[Label("$Mods.EnhancedTeamUIDisplay.Config.ShowErrorMessages.Label")]
		[DefaultValue(true)]
		public bool ShowErrorMessages;

		public override void OnChanged()
		{
			base.OnChanged();
			if (!LockUIPosition && AllowOnClickTeleport) AllowOnClickTeleport = false;

			if (ETUDUISystem.ETUDInterface != null && Main.netMode != NetmodeID.SinglePlayer) {
				ETUDUISystem.CloseETUDInterface();
				if (!EnableAutoToggle) ETUDUISystem.OpenETUDInterface();
			}
		}
	}
}
