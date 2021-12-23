using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;
using Mango.Communication.Packets.Outgoing;
using Mango.Communication.Packets.Outgoing.Messenger;
using System.Threading;

namespace Mango.Players.Messenger
{
    sealed class MessengerComponent
    {
        private Player _player;

        private readonly ConcurrentDictionary<int, MessengerFriendship> _friends;
        private readonly ConcurrentDictionary<int, MessengerFriendship> _requests;
        private readonly ConcurrentDictionary<int, MessengerUpdateMode> _updates;

        private int WriterCount = 0;

        public MessengerComponent()
        {
            this._friends = new ConcurrentDictionary<int,MessengerFriendship>();
            this._requests = new ConcurrentDictionary<int,MessengerFriendship>();
            this._updates = new ConcurrentDictionary<int, MessengerUpdateMode>();
        }

        public bool Init(Player Player)
        {
            if (this._friends.Count > 0)
            {
                throw new InvalidOperationException("Cannot re-initialize already initialized MessengerComponent.");
            }

            using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
            {
                DbCon.Open();
                DbCon.SetQuery("SELECT * FROM `messenger_friendships` WHERE `user_1_id` = @uid;");
                DbCon.AddParameter("uid", Player.Id);

                using (MySqlDataReader Reader = DbCon.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        MessengerFriendship Friend = new MessengerFriendship(Reader.GetInt32("id"), Reader.GetInt32("user_1_id"),
                            Reader.GetInt32("user_2_id"), Reader.GetInt32("confirmed") == 1 ? true : false);

                        if (Friend.Confirmed)
                        {
                            if (!this._friends.TryAdd(Friend.UserTwoId, Friend))
                            {
                                // log
                            }
                        }
                        else
                        {
                            if (!this._requests.TryAdd(Friend.UserTwoId, Friend))
                            {
                                // log
                            }
                        }
                    }
                }
            }

            this._player = Player;

            return true;
        }

        /// <summary>
        /// Sets all friends as update needs to be sent to the client.
        /// </summary>
        /// <param name="ForceInstant">If it should be sent instantly or only when requested from the client.</param>
        public void SetUpdateNeeded(bool ForceInstant)
        {
            foreach (MessengerFriendship Friends in this._friends.Values)
            {
                Player TargetPlayer = Friends.Friend;

                if (TargetPlayer == null)
                {
                    continue; // not online / not found
                }

                if (TargetPlayer.GetMessenger().AddUpdate(this._player.Id, MessengerUpdateMode.Update))
                {
                    {
                        TargetPlayer.GetMessenger().ProcessUpdates();
                    }
                }
            }
        }

        public void ProcessUpdates()
        {
            // Use Interlocked to quickly check if another thread is already sending updates
            if (Interlocked.CompareExchange(ref WriterCount, 1, 0) == 0)
            {
                while (this._updates.Count > 0)
                {
                    this._player.GetSession().SendPacket(AppendUpdatePacket());
                }

                Interlocked.Exchange(ref WriterCount, 0);
            }
        }

        private ServerPacket AppendUpdatePacket()
        {
            List<MessengerUpdate> Updates = new List<MessengerUpdate>();

            foreach (KeyValuePair<int, MessengerUpdateMode> Update in this._updates)
            {
                PlayerData Info = PlayerLoader.GetDataById(Update.Key);

                if (Info == null)
                {
                    continue; // User info is missing
                }

                Updates.Add(new MessengerUpdate(Update.Value, Info));
            }

            this._updates.Clear();

            return new FriendListUpdateComposer(Updates);
        }

        public bool TryAddRequest(MessengerFriendship Request)
        {
            if (Request.Confirmed)
            {
                throw new InvalidOperationException("This request is already marked as confirmed.");
            }

            return this._requests.TryAdd(Request.UserTwoId, Request);
        }

        public void RemoveAllRequests()
        {
            this._requests.Clear();
        }

        public bool TryRemoveRequest(int RequestUserId, out MessengerFriendship Friendship)
        {
            return this._requests.TryRemove(RequestUserId, out Friendship);
        }

        public bool TryGetRequest(int RequestUserId, out MessengerFriendship Request)
        {
            return this._requests.TryGetValue(RequestUserId, out Request);
        }

        public bool HasRequest(int RequestUserId)
        {
            return this._requests.ContainsKey(RequestUserId);
        }

        public ICollection<MessengerFriendship> GetRequests
        {
            get
            {
                return this._requests.Values;
            }
        }

        public bool AddUpdate(int UserId, MessengerUpdateMode Mode)
        {
            if (!this._friends.ContainsKey(UserId))
            {
                throw new InvalidOperationException("User is not a friend.");
            }

            return this._updates.TryAdd(UserId, Mode);
        }

        public bool IsFriends(int UserId)
        {
            MessengerFriendship Friendship = null;

            if (this._friends.TryGetValue(UserId, out Friendship))
            {
                return Friendship.Confirmed;
            }

            return false;
        }

        public bool TryAdd(MessengerFriendship Friend)
        {
            return this._friends.TryAdd(Friend.UserTwoId, Friend);
        }

        public bool TryRemove(int UserTwoId)
        {
            MessengerFriendship Value;

            return this._friends.TryRemove(UserTwoId, out Value);
        }

        public bool TryGet(int UserId, out MessengerFriendship Friend)
        {
            return this._friends.TryGetValue(UserId, out Friend);
        }

        public ICollection<MessengerFriendship> GetFriends
        {
            get
            {
                return this._friends.Values;
            }
        }

        public void Dispose()
        {
            this._friends.Clear();
        }
    }
}
