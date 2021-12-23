using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Mango.Items;
using Mango.Rooms.Mapping;
using Mango.Players;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Packets.Outgoing.Moderation;
using Mango.Items.Events;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Mango.Rooms.Instance
{
    sealed class ItemsComponent
    {
        /// <summary>
        /// RoomInstance that uses this ItemsComponent.
        /// </summary>
        private RoomInstance Instance = null;

        /// <summary>
        /// Floor items in this room.
        /// </summary>
        private ConcurrentDictionary<int, Item> _floorItems = null;

        /// <summary>
        /// Wall items in this room.
        /// </summary>
        private ConcurrentDictionary<int, Item> _wallItems = null;

        /// <summary>
        /// Static items for this room.
        /// </summary>
        private List<ItemStaticPublics> _staticItems = null;

        private Dictionary<ItemBehaviour, int> _itemLimitStore = null;

        private ConcurrentDictionary<int, int> _tmpStickyEditingRights = null; // may not need to be concurrent need to review this :)

        /// <summary>
        /// Moodlight Item present in this room (null if not present)
        /// </summary>
        private Item MoodlightItem = null;

        private bool _guestsPlaceStickies;

        private List<int> _itemsJustRolled;

        public bool GuestsCanPlaceStickies
        {
            get { return this._guestsPlaceStickies; }
            set { this._guestsPlaceStickies = value; }
        }

        /// <summary>
        /// Initialize new instance of this ItemsComponent
        /// </summary>
        /// <param name="instance">RoomInstance that created this component.</param>
        public ItemsComponent(RoomInstance instance)
        {
            if (instance == null) throw new NullReferenceException("RoomInstance cannot be null");

            this.Instance = instance;
            this._floorItems = new ConcurrentDictionary<int, Item>();
            this._wallItems = new ConcurrentDictionary<int, Item>();
            this._staticItems = new List<ItemStaticPublics>();
            this._itemLimitStore = new Dictionary<ItemBehaviour, int>();
            this._tmpStickyEditingRights = new ConcurrentDictionary<int, int>();
            this._guestsPlaceStickies = false;
            this._itemsJustRolled = new List<int>();

            this.Initialize();
        }

        /// <summary>
        /// If this room has a moodlight present or not.
        /// </summary>
        public bool HasMoodlight
        {
            get
            {
                return this.MoodlightItem != null;
            }
        }

        private void Initialize()
        {
            if (this._floorItems.Count > 0 || this._wallItems.Count > 0)
            {
                throw new InvalidOperationException("FloorItems / WallItems have already been initialized for this room, they cannot be initialized again.");
            }

            List<Item> Items = ItemLoader.GetItemsForRoom(Instance.Id);

            foreach (Item Item in Items)
            {
                if (Item.Data.Type != ItemType.FLOOR && Item.Data.Type != ItemType.WALL)
                {
                    continue;
                }

                if (Item.Data.Type == ItemType.FLOOR)
                {
                    this._floorItems.TryAdd(Item.Id, Item);
                }
                else if (Item.Data.Type == ItemType.WALL)
                {
                    this._wallItems.TryAdd(Item.Id, Item);
                }

                if (this._itemLimitStore.ContainsKey(Item.Data.Behaviour))
                {
                    this._itemLimitStore[Item.Data.Behaviour]++;
                }
                else
                {
                    this._itemLimitStore.Add(Item.Data.Behaviour, 1);
                }

                Mango.GetServer().GetItemEventManager().Handle(null, Item, ItemEventType.InstanceLoaded, this.Instance);
            }
        }

        public ICollection<Item> GetFloor
        {
            get
            {
                return this._floorItems.Values;
            }
        }

        public ICollection<Item> GetWall
        {
            get
            {
                return this._wallItems.Values;
            }
        }

        public IEnumerable<Item> GetWallAndFloor
        {
            get
            {
                return this._floorItems.Values.Concat(this._wallItems.Values);
            }
        }

        public List<ItemStaticPublics> GetStaticItems
        {
            get
            {
                return this._staticItems;
            }
        }

        public bool TryGetItem(int itemId, out Item item)
        {
            if (this._floorItems.ContainsKey(itemId) && this._wallItems.ContainsKey(itemId))
            {
                throw new InvalidOperationException("Duplicate item ids in both wall and floor."); // debugging
            }

            if (this._floorItems.ContainsKey(itemId))
            {
                return this._floorItems.TryGetValue(itemId, out item);
            }
            else if (this._wallItems.ContainsKey(itemId))
            {
                return this._wallItems.TryGetValue(itemId, out item);
            }
            else
            {
                item = null;
                return false;
            }
        }

        public bool TryGetFloorItem(int itemId, out Item item)
        {
            return this._floorItems.TryGetValue(itemId, out item);
        }

        public bool TryGetWallItem(int itemId, out Item item)
        {
            return this._wallItems.TryGetValue(itemId, out item);
        }

        public bool TryRemoveItem(int itemId, out Item item) // removes only from collection, not used for removing items physically from the room
        {
            if (this._floorItems.ContainsKey(itemId) && this._wallItems.ContainsKey(itemId))
            {
                throw new InvalidOperationException("Duplicate item ids in both wall and floor."); // debugging
            }

            if (this._floorItems.ContainsKey(itemId))
            {
                return this._floorItems.TryRemove(itemId, out item);
            }
            else if (this._wallItems.ContainsKey(itemId))
            {
                return this._wallItems.TryRemove(itemId, out item);
            }
            else
            {
                item = null;
                return false;
            }
        }

        public bool TryAddItem(Item item) // adds it into the collection only, not used for storing items into rooms (i use it for like a 'rollback' feature when other things fail.. read source yourself)
        {
            if (item.Data.Type == ItemType.FLOOR)
            {
                return this._floorItems.TryAdd(item.Id, item);
            }
            else if (item.Data.Type == ItemType.WALL)
            {
                return this._wallItems.TryAdd(item.Id, item);
            }
            else
            {
                throw new InvalidOperationException("Item did not match neither floor or wall item");
            }
        }

        /// <summary>
        /// Attempts to place the floor item and returns the corrected placed position (if success).
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="Item"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Rotation"></param>
        /// <param name="PlacedPosition"></param>
        /// <returns></returns>
        public bool TrySetFloorItem(Player Player, Item Item, int X, int Y, int Rotation, out Vector3D PlacedPosition)
        {
            bool AlreadyContained = this._floorItems.ContainsKey(Item.Id);
            int TotalLimitCount = AlreadyContained ? this._floorItems.Count - 1 : this._floorItems.Count;
            int TypeLimitCount = _itemLimitStore.ContainsKey(Item.Data.Behaviour) ? (AlreadyContained ?
                this._itemLimitStore[Item.Data.Behaviour] - 1 : this._itemLimitStore[Item.Data.Behaviour]) : 0;

            if (Player != null)
            {
                if (Item.Data.RoomLimit > 0 && TypeLimitCount >= Item.Data.RoomLimit)
                {
                    PlacedPosition = null;
                    Player.GetSession().SendPacket(new ModMessageComposer("This room cannot hold any more furniture of this type."));
                    return false;
                }

                if (TotalLimitCount >= MangoStaticSettings.MaxFloorFurniInRoom) // to-do: furni limit for rooms
                {
                    PlacedPosition = null;
                    Player.GetSession().SendPacket(new PlaceObjectErrorComposer(ItemPlacementError.FURNI_LIMIT_REACHED));
                    return false;
                }
            }

            bool RotationOnly = (Item.RoomId == Instance.Id && Item.Position.X == X &&
                Item.Position.Y == Y && Rotation != Item.RoomRot);

            if (!this.Instance.GetMapping().IsValidItemRotation(Rotation))
            {
                PlacedPosition = null;
                return false;
            }

            if (!this.Instance.GetMapping().IsValidPosition(new Vector2D(X, Y)))
            {
                PlacedPosition = null;
                return false;
            }

            List<Vector2D> AffectedTiles = this.Instance.GetMapping().CalculateAffectedTiles(Item, new Vector2D(X, Y), Rotation);
            double PlacementHeight = this.Instance.GetMapping().GetItemPlacementHeight(Item, AffectedTiles, RotationOnly);

            if (PlacementHeight < 0)
            {
                PlacedPosition = null;
                return false;
            }

            if (!this._floorItems.ContainsKey(Item.Id))
            {
                if (this._floorItems.TryAdd(Item.Id, Item))
                {
                    lock (this._itemLimitStore)
                    {
                        if (this._itemLimitStore.ContainsKey(Item.Data.Behaviour))
                        {
                            this._itemLimitStore[Item.Data.Behaviour]++;
                        }
                        else
                        {
                            this._itemLimitStore.Add(Item.Data.Behaviour, 1);
                        }
                    }
                }
                else
                {
                    PlacedPosition = null;
                    return false;
                }
            }

            PlacedPosition = new Vector3D(X, Y, PlacementHeight);
            return true;
        }

        public bool TrySetWallItem(Player player, Item item, string[] data, out string position)
        {
            bool AlreadyContained = this._wallItems.ContainsKey(item.Id);
            int TotalLimitCount = AlreadyContained ? this._wallItems.Count - 1 : this._wallItems.Count;
            int TypeLimitCount = _itemLimitStore.ContainsKey(item.Data.Behaviour) ? (AlreadyContained ?
                this._itemLimitStore[item.Data.Behaviour] - 1 : this._itemLimitStore[item.Data.Behaviour]) : 0;

            if (item.Data.RoomLimit > 0 && TypeLimitCount >= item.Data.RoomLimit)
            {
                position = null;
                player.GetSession().SendPacket(new ModMessageComposer("This room cannot hold any more furniture of this type."));
                return false;
            }

            if (TotalLimitCount >= MangoStaticSettings.MaxWallFurniInRoom) // to-do: furni limit for rooms
            {
                position = null;
                player.GetSession().SendPacket(new PlaceObjectErrorComposer(ItemPlacementError.FURNI_LIMIT_REACHED));
                return false;
            }

            if (data.Length != 3 || !data[0].StartsWith(":w=") || !data[1].StartsWith("l=") || (data[2] != "r" &&
                data[2] != "l"))
            {
                position = null;
                return false;
            }

            string wBit = data[0].Substring(3, data[0].Length - 3);
            string lBit = data[1].Substring(2, data[1].Length - 2);

            if (!wBit.Contains(",") || !lBit.Contains(","))
            {
                position = null;
                return false;
            }

            int w1 = 0;
            int w2 = 0;
            int l1 = 0;
            int l2 = 0;

            int.TryParse(wBit.Split(',')[0], out w1);
            int.TryParse(wBit.Split(',')[1], out w2);
            int.TryParse(lBit.Split(',')[0], out l1);
            int.TryParse(lBit.Split(',')[1], out l2);

            if (!player.GetPermissions().HasRight("super_admin") && (w1 < 0 || w2 < 0 || l1 < 0 || l2 < 0 || w1 > 200 || w2 > 200 || l1 > 200 || l2 > 200))
            {
                position = null;
                return false;
            }

            string WallPos = ":w=" + w1 + "," + w2 + " l=" + l1 + "," + l2 + " " + data[2];

            if (!this._wallItems.ContainsKey(item.Id))
            {
                if (this._wallItems.TryAdd(item.Id, item))
                {
                    lock (this._itemLimitStore)
                    {
                        if (this._itemLimitStore.ContainsKey(item.Data.Behaviour))
                        {
                            this._itemLimitStore[item.Data.Behaviour]++;
                        }
                        else
                        {
                            this._itemLimitStore.Add(item.Data.Behaviour, 1);
                        }
                    }
                }
                else
                {
                    position = null;
                    return false;
                }
            }

            position = WallPos;
            return true;
        }

        public bool TrySetWallItem(Player player, Item item, string data, out string position)
        {
            return TrySetWallItem(player, item, data.Split(' '), out position);
        }

        public bool TryTakeItem(Item item)
        {
            Item RemovedItem = null;

            if (TryRemoveItem(item.Id, out RemovedItem))
            {
                lock (this._itemLimitStore)
                {
                    if (this._itemLimitStore.ContainsKey(item.Data.Behaviour))
                    {
                        this._itemLimitStore[item.Data.Behaviour]--;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void GiveTemporaryStickyEditingRights(int StickyId, int UserId)
        {
            this._tmpStickyEditingRights.TryAdd(StickyId, UserId);
        }

        public void BroadcastItemState(Item Item)
        {
            if (Item.Data.Type == ItemType.FLOOR)
            {
                this.Instance.GetAvatars().BroadcastPacket(new ObjectDataUpdateComposer(Item));
            }
            else if (Item.Data.Type == ItemType.WALL)
            {
                this.Instance.GetAvatars().BroadcastPacket(new ItemUpdateComposer(Item, this.Instance.OwnerId));
            }
            else
            {
                throw new InvalidOperationException("Item state cannot be broadcasted");
            }
        }

        public List<Item> GetItemsOnPosition(Vector2D Position)
        {
            List<Item> Items = new List<Item>();

            foreach (Item Item in this.GetFloor)
            {
                List<Vector2D> Tiles = this.Instance.GetMapping().CalculateAffectedTiles(Item, Item.Position.ToVector2D(), Item.RoomRot);

                foreach (Vector2D Tile in Tiles)
                {
                    if (Tile.X == Position.X && Tile.Y == Position.Y)
                    {
                        Items.Add(Item);
                    }
                }
            }

            return Items;
        }

        public void SetItemJustRolled(Item Item)
        {
            if (!this._itemsJustRolled.Contains(Item.Id))
            {
                this._itemsJustRolled.Add(Item.Id);
            }
        }

        public bool IsItemRolledAlready(Item Item)
        {
            return this._itemsJustRolled.Contains(Item.Id);
        }

        public void ClearRolledItems()
        {
            this._itemsJustRolled.Clear();
        }

        public void Cleanup()
        {
            this._floorItems.Clear();
            this._wallItems.Clear();
            this._staticItems.Clear();
            this._itemLimitStore.Clear();

            this.Instance = null;
            this._floorItems = null;
            this._wallItems = null;
            this._staticItems = null;
            this._itemLimitStore = null;
            this.MoodlightItem = null;
        }
    }
}
