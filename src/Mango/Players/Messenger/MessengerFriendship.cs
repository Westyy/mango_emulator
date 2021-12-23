using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Messenger
{
    class MessengerFriendship
    {
        public int Id { get; set; }
        public int UserOneId { get; set; }
        public int UserTwoId { get; set; }
        public bool Confirmed { get; set; }

        public MessengerFriendship(int Id, int UserOneId, int UserTwoId, bool Confirmed)
        {
            this.Id = Id;
            this.UserOneId = UserOneId;
            this.UserTwoId = UserTwoId;
            this.Confirmed = Confirmed;
        }

        private WeakReference TargetFriend = null;

        public Player Friend
        {
            get
            {
                if (TargetFriend == null)
                {
                    Player Player = null;

                    if (!Mango.GetServer().GetPlayerManager().TryGet(this.UserTwoId, out Player))
                    {
                        return null;
                    }

                    TargetFriend = new WeakReference(Player);
                    return Player;
                }

                if (TargetFriend.IsAlive)
                {
                    Player TPlayer = (Player)TargetFriend.Target;

                    if (TPlayer.Online)
                    {
                        return TPlayer;
                    }

                    TargetFriend = null;

                    return null;
                }

                TargetFriend = null;

                return null;
            }
        }
    }
}
