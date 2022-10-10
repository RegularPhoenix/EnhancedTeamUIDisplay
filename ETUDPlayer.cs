using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.UI;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace EnhancedTeamUIDisplay
{
	internal class ETUDPlayer : ModPlayer
	{
		public static int PanelTopOffset;
		public static int PanelLeftOffset;

		// Key - Boss name, Value - { times killed, times wiped }
		public static Dictionary<string, int[]> BossFightAttempts;

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (ETUD.ETUDHotkey.JustPressed && Main.netMode != NetmodeID.SinglePlayer) ETUDUISystem.ToggleETUD();
		}

		public override void OnEnterWorld(Player player)
		{
			ETUDUISystem.CloseETUDInterface();
			if (Main.netMode == NetmodeID.SinglePlayer) Main.NewText("ETUD will not work in singleplayer. You might have wanted to host and play.", 255, 255, 0);
		}

		public override void SaveData(TagCompound tag)
		{
			tag["PanelTopOffset"] = PanelTopOffset;
			tag["PanelLeftOffset"] = PanelLeftOffset;

			if (BossFightAttempts == null) BossFightAttempts = new();
			var List = new List<TagCompound>();
			foreach (var item in BossFightAttempts)
			{
				List.Add(new TagCompound()
				{
					{"name", item.Key },
					{ "wins", item.Value[0]},
					{"losses",item.Value[1]},
				});
			}

			tag["BFA"] = List;
		}

		public override void LoadData(TagCompound tag)
		{
			if (tag.ContainsKey("PanelTopOffset")) PanelTopOffset = (int)tag["PanelTopOffset"];
			if (tag.ContainsKey("PanelLeftOffset")) PanelLeftOffset = (int)tag["PanelLeftOffset"];
			
			if (BossFightAttempts == null) BossFightAttempts = new();
			var List = tag.GetList<TagCompound>("BFA");
			foreach (var item in List)
			{
				string name = item.GetString("name");
				int wins = item.GetInt("wins");
				int losses = item.GetInt("losses");
				BossFightAttempts[name] = new int[] { wins, losses };
			}
		}
	}
}