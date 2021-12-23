using Mango.Chat.Commands.Client;
using Mango.Chat.Commands.Debugging;
using Mango.Chat.Commands.Default;
using Mango.Chat.Commands.Updating;
using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Communication.Sessions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Chat.Commands
{
    sealed class CommandManager
    {
        /// <summary>
        /// Command Prefix only applies to custom commands.
        /// </summary>
        private string _prefix = ":";

        /// <summary>
        /// Commands registered for use.
        /// </summary>
        private readonly Dictionary<string, IChatCommand> _commands;

        /// <summary>
        /// Handles commands which run in the background. (not on the user thread)
        /// </summary>
        private readonly TaskFactory _asyncCmdHandler;

        /// <summary>
        /// The default initializer for the CommandManager
        /// </summary>
        public CommandManager(string Prefix)
        {
            this._commands = new Dictionary<string, IChatCommand>();
            this._asyncCmdHandler = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);
            this._prefix = Prefix;

            RegisterDefault();
            RegisterClient();
            RegisterUpdating();
            RegisterDebugging();
        }

        /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="Session">Session calling this method.</param>
        /// <param name="Message">The message to parse.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(Session Session, string Message)
        {
            if (!Message.StartsWith(this._prefix))
            {
                return false;
            }


            if (Message == this._prefix + "commands")
            {
                StringBuilder CommandList = new StringBuilder();

                foreach (KeyValuePair<String, IChatCommand> CmdList in this._commands)
                {
                    if (!string.IsNullOrEmpty(CmdList.Value.PermissionRequired))
                    {
                        if (!Session.GetPlayer().GetPermissions().HasRight(CmdList.Value.PermissionRequired))
                        {
                            continue;
                        }
                    }

                    if (CmdList.Value.RequiredRank > -1)
                    {
                        if (Session.GetPlayer().PermissionLevel < CmdList.Value.RequiredRank)
                        {
                            return false;
                        }
                    }

                    CommandList.Append(":" + CmdList.Key + "\n");
                }

                Session.SendPacket(new ModMessageComposer(CommandList.ToString()));
                return true;
            }

            Message = Message.Substring(1, Message.Length - 1);
            string[] MsgSplit =  Message.Split(' ');

            if (MsgSplit.Length == 0)
            {
                return false;
            }

            IChatCommand Cmd = null;

            if (this._commands.TryGetValue(MsgSplit[0].ToLower(), out Cmd))
            {
                if (!string.IsNullOrEmpty(Cmd.PermissionRequired))
                {
                    if (!Session.GetPlayer().GetPermissions().HasRight(Cmd.PermissionRequired))
                    {
                        return false;
                    }
                }

                if (Cmd.RequiredRank > -1)
                {
                    if (Session.GetPlayer().PermissionLevel < Cmd.RequiredRank)
                    {
                        return false;
                    }
                }

                if (Cmd.IsAsynchronous)
                {
                    Task t = this._asyncCmdHandler.StartNew(() =>
                        {
                            Cmd.Parse(Session, Message);
                        });
                }
                else
                {
                    Cmd.Parse(Session, Message);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Registers the default set of commands.
        /// </summary>
        private void RegisterDefault()
        {
            Register("about", new AboutCmd());
            Register("online", new OnlineCmd());
            Register("sit", new SitCmd());
            Register("enable", new EnableCmd());
            Register("roomalert", new RoomAlertCmd());
            Register("hidefurni", new HideFurniCmd());
            Register("showfriends", new ShowFriendsCmd());
            Register("hidefriends", new HideFriendsCmd());
            Register("ha", new HaCmd());
            Register("summon", new SummonCmd());
            Register("pull", new PullCmd());
            Register("rotate", new RotateCmd());
            Register("fastwalk", new FastWalkCmd());
            Register("height", new HeightCmd());
            Register("clearroombans", new ClearRoomBansCmd());
            Register("override", new OverrideCmd());
            Register("setmax", new SetMaxCmd());
            Register("maintenance", new MaintenanceCmd());
            Register("carryitem", new CarryItemCmd());

#if DEBUG
            Register("test", new TestCmd());
#endif
        }

        public void RegisterClient()
        {
            Register("pickall", new PickallCmd());
        }

        public void RegisterUpdating()
        {
            Register("update_items", new UpdateItemsCmd());
            Register("update_catalog", new UpdateCatalogCmd());
        }

        public void RegisterDebugging()
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                return;
            }

            Register("clearitemdata", new RemoveItemDataCmd());
#endif
        }

        /// <summary>
        /// Registers a Chat Command.
        /// </summary>
        /// <param name="CommandText">Text to type for this command.</param>
        /// <param name="Command">The command to execute.</param>
        public void Register(string CommandText, IChatCommand Command)
        {
            this._commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            StringBuilder MergedParams = new StringBuilder();

            for (int i = 0; i < Params.Length; i++)
            {
                if (i < Start)
                {
                    continue;
                }

                if (i > Start)
                {
                    MergedParams.Append(" ");
                }

                MergedParams.Append(Params[i]);
            }

            return MergedParams.ToString();
        }
    }
}
