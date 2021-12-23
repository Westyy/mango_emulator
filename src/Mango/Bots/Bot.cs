using Mango.Bots.AI;
using Mango.Bots.Types;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Bots
{
    sealed class Bot
    {
        private readonly RoomAvatar _avatar;
        private readonly IBotAI _behaviour;
        private readonly BotWalkMode _walkMode;
        private readonly string _name;
        private readonly string _figure;

        public Bot()
        {
        }
    }
}
