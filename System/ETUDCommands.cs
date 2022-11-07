using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace EnhancedTeamUIDisplay
{
	#region ReadyCommand
	public class ReadyCommand : ModCommand
	{
		public override CommandType Type
			=> CommandType.Chat;

		public override string Command
			=> "buffcheck";

		public override string Usage
			=> "/buffcheck";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if(Main.LocalPlayer.team == 0)
			{
				caller.Reply("Enter a team first");
				return;
			}

			ETUDAdditionalOptions.CheckForBuffs();
		}
	}
	#endregion

	#region ETUD Command
	public class ETUDCommand : ModCommand
	{
		public override CommandType Type
			=> CommandType.Chat;

		public override string Command
			=> "setplayer";

		public override string Description
			=> "Changes the player displayed on the panel";

		public override string Usage
			=> "/setplayer <player name> <panel number>";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			int.TryParse(args[1], out var panelnum);

			if (panelnum < 1 || panelnum > 3) throw new UsageException("Incorrect panel number.");

			switch (panelnum)
			{
				case 1:
					if (ETUDPanel1.Ally is not null) if (ETUDPanel1.Ally.name == args[0]) { caller.Reply("Requested player is already on that panel"); return; }
						
					bool done = false;

					if (ETUDPanel2.Ally is not null)
					{
						if (ETUDPanel2.Ally.name == args[0])
						{
							ETUDPanel1.Ally = ETUDPanel2.Ally;
							ETUDPanel1.allyFound = true;

							ETUDPanel2.Ally = null;
							ETUDPanel2.allyFound = false;

							done = true;

							caller.Reply("Player set.");
						}
					}
					else if (ETUDPanel3.Ally is not null)
					{
						if (ETUDPanel3.Ally.name == args[0])
						{
							ETUDPanel1.Ally = ETUDPanel3.Ally;
							ETUDPanel1.allyFound = true;

							ETUDPanel3.Ally = null;
							ETUDPanel3.allyFound = false;

							done = true;

							caller.Reply("Player set.");
						}
					}

					if (!done)
					{
						for (int i = 0; i < Main.maxPlayers; i++)
						{
							if (Main.player[i] is not null && Main.player[i].team == caller.Player.team && Main.player[i].name == args[0])
							{
								ETUDPanel1.Ally = Main.player[i];
								ETUDPanel1.allyFound = true;

								done = true;

								caller.Reply("Player set.");
							}
						}
					}

					if (!done) caller.Reply("Could not find requested player on your team");
						
					break;
				case 2:
					if (ETUDPanel2.Ally is not null) if (ETUDPanel2.Ally.name == args[0]) { caller.Reply("Requested player is already on that panel"); return; }

					bool done2 = false;

					if (ETUDPanel1.Ally is not null)
					{
						if (ETUDPanel1.Ally.name == args[0])
						{
							ETUDPanel2.Ally = ETUDPanel1.Ally;
							ETUDPanel2.allyFound = true;

							ETUDPanel1.Ally = null;
							ETUDPanel1.allyFound = false;

							done2 = true;

							caller.Reply("Player set.");
						}
					}
					else if (ETUDPanel3.Ally is not null)
					{
						if (ETUDPanel3.Ally.name == args[0])
						{
							ETUDPanel2.Ally = ETUDPanel3.Ally;
							ETUDPanel2.allyFound = true;

							ETUDPanel3.Ally = null;
							ETUDPanel3.allyFound = false;

							done2 = true;

							caller.Reply("Player set.");
						}
					}

					if (!done2)
					{
						for (int i = 0; i < Main.maxPlayers; i++)
						{
							if (Main.player[i] is not null && Main.player[i].team == caller.Player.team && Main.player[i].name == args[0])
							{
								ETUDPanel2.Ally = Main.player[i];
								ETUDPanel2.allyFound = true;

								done2 = true;

								caller.Reply("Player set.");
							}
						}
					}

					if (!done2) caller.Reply("Could not find requested player on your team");

					break;
				case 3:
					if (ETUDPanel3.Ally is not null) if (ETUDPanel3.Ally.name == args[0]) { caller.Reply("Requested player is already on that panel"); return; }

					bool done3 = false;

					if (ETUDPanel2.Ally is not null)
					{
						if (ETUDPanel2.Ally.name == args[0])
						{
							ETUDPanel3.Ally = ETUDPanel2.Ally;
							ETUDPanel3.allyFound = true;

							ETUDPanel2.Ally = null;
							ETUDPanel2.allyFound = false;

							done3 = true;

							caller.Reply("Player set.");
						}
					}
					else if (ETUDPanel1.Ally is not null)
					{
						if (ETUDPanel1.Ally.name == args[0])
						{
							ETUDPanel3.Ally = ETUDPanel1.Ally;
							ETUDPanel3.allyFound = true;

							ETUDPanel1.Ally = null;
							ETUDPanel1.allyFound = false;

							done3 = true;

							caller.Reply("Player set.");
						}
					}

					if (!done3)
					{
						for (int i = 0; i < Main.maxPlayers; i++)
						{
							if (Main.player[i] is not null && Main.player[i].team == caller.Player.team && Main.player[i].name == args[0])
							{
								ETUDPanel1.Ally = Main.player[i];
								ETUDPanel1.allyFound = true;

								done3 = true;

								caller.Reply("Player set.");
							}
						}
					}

					if (!done3) caller.Reply("Could not find requested player on your team");

					break;
			}
		}
	}
	#endregion

	#region DebugCommand
	public class DebugCommand : ModCommand
	{
		public override CommandType Type
			=> CommandType.Chat;

		public override string Command
			=> "debugETUD";

		public override string Description
			=> "Don't use this command. For development purposes. May crash the game";

		public override string Usage
			=> "/debugETUD <get/set/add> <variable(s)> <panel number (optional)>";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			switch (args[0])
			{
				case "get":
					caller.Reply(args[1] switch
					{
						"paneloffset" => $"{Main.LocalPlayer.GetModPlayer<ETUDPlayer>().PanelLeftOffset} {Main.LocalPlayer.GetModPlayer<ETUDPlayer>().PanelTopOffset}",
						"allyFound" => int.Parse(args[2]) switch
						{
							1 => $"{ETUDPanel1.allyFound}",
							2 => $"{ETUDPanel2.allyFound}",
							3 => $"{ETUDPanel3.allyFound}",
							_ => "Incorrect panel number"
						},
						"name" => int.Parse(args[2]) switch
						{
							1 => $"{ETUDPanel1.Ally.name}",
							2 => $"{ETUDPanel2.Ally.name}",
							3 => $"{ETUDPanel3.Ally.name}",
							_ => "Incorrect panel number"
						},
						"BFA" => Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts.Count == 0 ? "Empty" : string.Join(Environment.NewLine, Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts.Select(s => $"{s.Key} {s.Value[0]} {s.Value[1]}")),
						_ => "Incorrect argument(s): <variable(s)>",
						
					});
					break;
				case "set":
					if (args[1] == "BFA") if (args[2] == "clear" && Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts is not null) Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts.Clear();			
					break;
				case "add":
					if (args[1] == "BFA")
					{
						if (Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts is null) Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts = new();
						Main.LocalPlayer.GetModPlayer<ETUDPlayer>().BossFightAttempts.Add(args[2], new int[] { int.Parse(args[3]), int.Parse(args[4]) });
						caller.Reply("Success");
					}
					break;
				case "throw":
					ETUDAdditionalOptions.CreateErrorMessage("TEST", new NotImplementedException(""), 255);
					break;
				default:
					caller.Reply("Incorrect argument: 2 - <get/set/add>");
					break;
			}
		}
	}
	#endregion
}
