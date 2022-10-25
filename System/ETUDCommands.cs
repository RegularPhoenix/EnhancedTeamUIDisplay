using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EnhancedTeamUIDisplay
{
	// Check for buffs

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

	// All commands related to ETUD itself

	public class ETUDCommand : ModCommand
	{
		public override CommandType Type
			=> CommandType.Chat;

		public override string Command
			=> "setplayer";

		public override string Description
			=> "Change player displayed on panel";

		public override string Usage
			=> "/setplayer <player name> <panel number>";

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			int.TryParse(args[1], out var panelnum);

			if (panelnum < 1 || panelnum > 3) throw new UsageException("Incorrect panel number.");

			switch (panelnum)
			{
				case 1:
					if (ETUDPanel1.Ally != null) if (ETUDPanel1.Ally.name == args[0]) { caller.Reply("Requested player is already on that panel"); return; }
						
					bool done = false;

					if (ETUDPanel2.Ally != null)
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
					else if (ETUDPanel3.Ally != null)
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
							if (Main.player[i] != null && Main.player[i].team == caller.Player.team && Main.player[i].name == args[0])
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
					if (ETUDPanel2.Ally != null) if (ETUDPanel2.Ally.name == args[0]) { caller.Reply("Requested player is already on that panel"); return; }

					bool done2 = false;

					if (ETUDPanel1.Ally != null)
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
					else if (ETUDPanel3.Ally != null)
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
							if (Main.player[i] != null && Main.player[i].team == caller.Player.team && Main.player[i].name == args[0])
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
					if (ETUDPanel3.Ally != null) if (ETUDPanel3.Ally.name == args[0]) { caller.Reply("Requested player is already on that panel"); return; }

					bool done3 = false;

					if (ETUDPanel2.Ally != null)
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
					else if (ETUDPanel1.Ally != null)
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
							if (Main.player[i] != null && Main.player[i].team == caller.Player.team && Main.player[i].name == args[0])
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

	// Dev only

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

			if (args[0] == "get")
			{
				if (args[1] == "paneloffset")
				{
					caller.Reply(ETUDPlayer.PanelLeftOffset + " " + ETUDPlayer.PanelTopOffset);
				}
				if (args[1] == "allyFound")
				{
					int.TryParse(args[2], out int panelnum);

					switch (panelnum)
					{
						default:
							break;
						case 1:
							Main.NewText(ETUDPanel1.allyFound);
							break;
						case 2:
							Main.NewText(ETUDPanel1.allyFound);
							break;
						case 3:
							Main.NewText(ETUDPanel1.allyFound);
							break;
					}
				}

				if (args[1] == "name")
				{
					int.TryParse(args[2], out int panelnum);

					switch (panelnum)
					{
						default:
							break;
						case 1:
							Main.NewText(ETUDPanel1.Ally.name);
							break;
						case 2:
							Main.NewText(ETUDPanel1.Ally.name);
							break;
						case 3:
							Main.NewText(ETUDPanel1.Ally.name);
							break;
					}
				}

				if (args[1] == "BossFightAttempts")
				{
					if (ETUDPlayer.BossFightAttempts.Count == 0) caller.Reply("Empty");
					foreach (System.Collections.Generic.KeyValuePair<string, int[]> pair in ETUDPlayer.BossFightAttempts)
					{
						Main.NewText(pair.Key + " " + pair.Value[0].ToString() + " " + pair.Value[1].ToString());
					}
				}
			}
			else if (args[0] == "add")
			{
				if (args[1] == "BossFightAttempts")
				{
					ETUDPlayer.BossFightAttempts.Add(args[2], new int[] { int.Parse(args[3]), int.Parse(args[4]) });
					caller.Reply("Success");
				}
			}
			else if (args[0] == "set")
			{
				if (args[1] == "BossFightAttempts")
				{
					if (args[2] == "clear") ETUDPlayer.BossFightAttempts.Clear();
				}
			}
			else if (args[0] == "throw")
			{
				ETUDAdditionalOptions.CreateErrorMessage("TEST", new NotImplementedException(""), 255);
			}

			// /debugETUD add BossFightAttempts bos 12 412
			// /debugETUD get BossFightAttempts
			// /debugETUD set BossFightAttempts clear
		}
	}
}
