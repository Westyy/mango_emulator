using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Mango.Items;

namespace Mango.Players.Inventory
{
    sealed class InventoryComponent
    {
        private ConcurrentDictionary<int, Item> _floorItems;
        private ConcurrentDictionary<int, Item> _wallItems;

        public InventoryComponent()
        {
            _floorItems = new ConcurrentDictionary<int, Item>();
            _wallItems = new ConcurrentDictionary<int, Item>();
        }

        public bool Init(int userId)
        {
            if (_floorItems.Count > 0 || _wallItems.Count > 0)
            {
                throw new InvalidOperationException("Cannot re-initialize the inventory component using this method.");
            }

            List<Item> Items = ItemLoader.GetItemsForUser(userId);

            foreach (Item Item in Items)
            {
                if (Item.Data.Type == ItemType.FLOOR)
                {
                    if (!this._floorItems.TryAdd(Item.Id, Item))
                    {
                        throw new InvalidOperationException("Failed to store item."); 
                    }
                }
                else if (Item.Data.Type == ItemType.WALL)
                {
                    if (!this._wallItems.TryAdd(Item.Id, Item))
                    {
                        throw new InvalidOperationException("Failed to store item.");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Item didn't match floor or wall.");
                }

                /*if (item.PendingExpiration && item.ExpireTimeLeft <= 0)
                {
                    continue;
                }*/

                /*if (!this.Items.TryAdd(item.Id, item))
                {
                    throw new InvalidOperationException("Failed to store item."); // debugging - should never fail but makes life easier :-D
                }*/
            }

            return true;
        }

        public ICollection<Item> GetFloorItems()
        {
            return this._floorItems.Values;
        }

        public ICollection<Item> GetWallItems()
        {
            return this._wallItems.Values;
        }

        public bool TryAddFloorItem(Item item)
        {
            if (item.Data.Type != ItemType.FLOOR)
            {
                throw new InvalidOperationException("Item type is not a floor");
            }

            return this._floorItems.TryAdd(item.Id, item);
        }

        public bool TryAddWallItem(Item item)
        {
            if (item.Data.Type != ItemType.WALL)
            {
                throw new InvalidOperationException("Item type is not a wall");
            }

            return this._wallItems.TryAdd(item.Id, item);
        }

        public bool TryAddItem(Item item)
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

        public bool TryRemoveItem(int itemId, out Item item)
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

        public void Dispose()
        {
        }
    }
}
