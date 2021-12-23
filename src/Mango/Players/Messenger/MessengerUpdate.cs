using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Messenger
{
    class MessengerUpdate
    {
        private MessengerUpdateMode _mode;
        private PlayerData _player;

        public MessengerUpdate(MessengerUpdateMode mode, PlayerData player)
        {
            this._mode = mode;
            this._player = player;
        }

        public MessengerUpdateMode Mode
        {
            get
            {
                return this._mode;
            }
        }

        public PlayerData Player
        {
            get
            {
                return this._player;
            }
        }

        // to-do: convert player data as a 'weak reference'
    }
}
