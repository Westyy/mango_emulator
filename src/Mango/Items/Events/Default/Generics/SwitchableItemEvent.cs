using Mango.Communication.Sessions;
using Mango.Rooms;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Items.Events.Default.Generics
{
    class SwitchableItemEvent : IItemEvent
    {
        public bool IsAsynchronous
        {
            get { return false; }
        }

        public void Parse(Session Session, Item Item, ItemEventType Type, RoomInstance Instance, int RequestData)
        {
            switch (Type)
            {
                case ItemEventType.Interact:

                    if (!Instance.GetRights().CheckRights(Session.GetPlayer().GetAvatar()))
                    {
                        return;
                    }

                    int CurrentState = 0;
                    int.TryParse(Item.Flags, out CurrentState);

                    int NewState = CurrentState + 1;

                    if (CurrentState < 0 || CurrentState >= (Item.Data.BehaviourData - 1))
                    {
                        NewState = 0;
                    }

                    if (CurrentState != NewState)
                    {
                        using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                        {
                            try
                            {
                                DbCon.Open();
                                DbCon.BeginTransaction();

                                DbCon.SetQuery("UPDATE `items` SET `flags` = @flags, `flags_display` = @display WHERE `id` = @id LIMIT 1;");
                                DbCon.AddParameter("flags", NewState.ToString());
                                DbCon.AddParameter("display", NewState.ToString());
                                DbCon.AddParameter("id", Item.Id);
                                DbCon.ExecuteNonQuery();

                                Item.Flags = NewState.ToString();
                                Item.DisplayFlags = Item.Flags;

                                DbCon.Commit();
                            }
                            catch (MySqlException)
                            {
                                DbCon.Rollback();
                                break;
                            }
                        }

                        Instance.GetItems().BroadcastItemState(Item);
                    }

                    break;
            }
        }
    }
}
