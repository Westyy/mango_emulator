using Mango.Bots;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms.Instance
{
    sealed class BotsComponent
    {
        private readonly ConcurrentDictionary<int, Bot> _bots;

        public BotsComponent()
        {
            this._bots = new ConcurrentDictionary<int, Bot>();
        }

        public int GenerateBotId(int DataId)
        {
            return (DataId * -1);
        }

        public void SpawnBot(Bot Bot)
        {
        }
    }
}
