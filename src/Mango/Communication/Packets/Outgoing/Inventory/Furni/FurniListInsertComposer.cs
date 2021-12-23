using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Items;

namespace Mango.Communication.Packets.Outgoing.Inventory.Furni
{
    class FurniListInsertComposer : ServerPacket
    {
        public FurniListInsertComposer(Item item)
            : base(ServerPacketHeadersNew.FurniListInsertComposer)
        {
            WriteItem(item);
        }

        private void WriteItem(Item item)
        {
            int t = 1;
            int s = 0;

            switch (item.Data.Behaviour)
            {
                default:
                case ItemBehaviour.STATIC:
                    t = 1;
                    break;

                case ItemBehaviour.WALLPAPER:
                    t = 2;
                    break;

                case ItemBehaviour.FLOOR:
                    t = 3;
                    break;

                case ItemBehaviour.LANDSCAPE:
                    t = 4;
                    break;

                case ItemBehaviour.MUSIC_DISK:
                    t = 8;
                    try // necessary?
                    {
                        int.TryParse(item.DisplayFlags, out s);
                    }
                    catch (ArgumentException) { }
                    break;
            }

            base.WriteInteger(item.Id);
            base.WriteString(ItemTypeUtility.GetTypeLetter(item.Data.Type).ToUpper());
            base.WriteInteger(item.Id);
            base.WriteInteger(item.Data.SpriteId);
            base.WriteInteger(t);
            base.WriteInteger(0);
            base.WriteString(item.DisplayFlags);
            base.WriteBoolean(item.Data.AllowRecycle);
            base.WriteBoolean(item.Data.AllowTrade);
            base.WriteBoolean(item.Data.AllowInventoryStack);
            base.WriteBoolean(item.Data.AllowSell);
            base.WriteInteger(-1);

            if (item.Data.Type == ItemType.FLOOR)
            {
                base.WriteString(string.Empty);
                base.WriteInteger(s);
            }
        }
    }
}
