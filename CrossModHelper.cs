using Terraria;
using Terraria.ModLoader;

namespace EnhancedTeamUIDisplay
{
	internal class CrossModHelper : ModSystem
	{
		internal static Mod CalamityMod;
		internal static Mod ThoriumMod;

		public override void PostSetupContent() {
			CalamityMod = ModLoader.TryGetMod("CalamityMod", out Mod calamity) ? calamity : null;
			ThoriumMod = ModLoader.TryGetMod("ThoriumMod", out Mod thorium) ? thorium : null;
		}

		public override void Unload() {
			CalamityMod = null;
			ThoriumMod = null;
		}

		internal static float GetRogueStealth(Player player)
			=> (float) CalamityMod?.Call("GetCurrentStealth", player);

		internal static float GetRogueStealthMax(Player player)
			=> (float) CalamityMod?.Call("GetMaxStealth", player);

		internal static float GetBardInspiration(Player player)
			=> (int) ThoriumMod?.Call("GetBardInspiration", player);

		internal static float GetBardInspirationMax(Player player)
			=> (int) ThoriumMod?.Call("GetBardInspirationMax", player);

		internal static int GetHealerHealBonus(Player player)
			=> (int) ThoriumMod?.Call("GetHealerHealBonus", player);
	}
}
