using Terraria.ModLoader;

namespace EnhancedTeamUIDisplay
{
	public class ETUD : Mod
	{
		internal static ModKeybind ETUDHotkey;

		public override void Load()
		{
			ETUDHotkey = KeybindLoader.RegisterKeybind(this, "Show ETUD", "L");
		}

		public override void Unload()
		{
			ETUDHotkey = null;
		}
	}
}