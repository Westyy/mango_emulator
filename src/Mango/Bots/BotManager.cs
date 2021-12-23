using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Bots
{
    sealed class BotManager
    {
        private readonly Dictionary<int, Bot> _staticBots;

        public BotManager()
        {
            this._staticBots = new Dictionary<int, Bot>();
        }
    }
}
