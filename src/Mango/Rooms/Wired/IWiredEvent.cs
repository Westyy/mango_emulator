using Mango.Items;
using Mango.Rooms.Wired.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms.Wired
{
    interface IWiredEvent
    {
        WiredTriggerType TriggerType { get; set; }
        void Parse(Item Item);
    }
}
