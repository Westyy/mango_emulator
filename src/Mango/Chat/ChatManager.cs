using Mango.Chat.Commands;
using Mango.Chat.Emotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Chat
{
    sealed class ChatManager
    {
        /// <summary>
        /// Chat Emoticons.
        /// </summary>
        private ChatEmotionsManager _emotions;

        /// <summary>
        /// Commands.
        /// </summary>
        private CommandManager _commands;

        /// <summary>
        /// Initializes a new instance of the ChatManager class.
        /// </summary>
        public ChatManager()
        {
            this._emotions = new ChatEmotionsManager();
            this._commands = new CommandManager(":");
        }

        public ChatEmotionsManager GetEmotions()
        {
            return this._emotions;
        }

        public CommandManager GetCommands()
        {
            return this._commands;
        }
    }
}
