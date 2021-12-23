using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events
{
    enum ItemEventType
    {
        Placed,
        Moved,
        Removing,
        Interact,
        UpdateTick,
        InstanceLoaded
    }
}
