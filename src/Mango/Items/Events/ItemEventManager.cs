using Mango.Communication.Sessions;
using Mango.Items.Events.Default.Generics;
using Mango.Items.Events.Default.Randomizers;
using Mango.Items.Events.Default.Roller;
using Mango.Items.Events.Default.Teleporters;
using Mango.Items.Events.Default.Timed;
using Mango.Items.Events.Default.Wired;
using Mango.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.Items.Events
{
    sealed class ItemEventManager
    {
        private const bool ASYNC_INST_LOADED = false;

        private readonly Dictionary<ItemBehaviour, IItemEvent> _events;
        private readonly TaskFactory _eventDispatcher;

        public ItemEventManager()
        {
            this._events = new Dictionary<ItemBehaviour, IItemEvent>();
            this._eventDispatcher = new TaskFactory(TaskCreationOptions.PreferFairness, TaskContinuationOptions.None);

            RegisterDefault();
        }

        public void Handle(Session Session, Item Item, ItemEventType Type, RoomInstance Room, int Data = 0)
        {
            IItemEvent Event = null;

            if (this._events.TryGetValue(Item.Data.Behaviour, out Event))
            {
                // Check our values are OK
                if (!Validate(Session, Item, Type, Room, Data))
                {
                    throw new InvalidOperationException("Validator returned false, something is wrong.");
                }

                if (Event.IsAsynchronous || (ASYNC_INST_LOADED && Type == ItemEventType.InstanceLoaded))
                {
                    Task T = _eventDispatcher.StartNew(() =>
                        {
                            Event.Parse(Session, Item, Type, Room, Data);
                        });
                }
                else
                {
                    Event.Parse(Session, Item, Type, Room, Data);
                }
            }
        }

        private bool Validate(Session Session, Item Item, ItemEventType Type, RoomInstance Room, int Data = 0)
        {
            if (Session != null && Type == ItemEventType.UpdateTick)
            {
                return false;
            }

            return true;
        }

        public void RegisterDefault()
        {
            // Randomizers
            this._events.Add(ItemBehaviour.DICE, new DiceItemEvent());
            this._events.Add(ItemBehaviour.HOLO_DICE, new HoloDiceItemEvent());

            // Generics
            this._events.Add(ItemBehaviour.SEAT, new SeatItemEvent());
            this._events.Add(ItemBehaviour.BED, new BedItemEvent());
            this._events.Add(ItemBehaviour.SWITCHABLE, new SwitchableItemEvent());

            // Teleporter
            this._events.Add(ItemBehaviour.TELEPORTER, new TeleporterItemEvent());

            // Wired
            this._events.Add(ItemBehaviour.WIRED_TRIGGER, new WiredTriggerEvent());

            // Roller
            this._events.Add(ItemBehaviour.ROLLER, new RollerItemEvent());

            // Alerts
            this._events.Add(ItemBehaviour.TIMED_ALERT, new AlertItemEvent());
        }
    }
}
