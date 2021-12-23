using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mango.Chat.Logs
{
    sealed class ChatlogManager
    {
        private const int FLUSH_ON_COUNT = 100;

        private readonly List<ChatlogEntry> _chatlogs;
        private readonly ReaderWriterLockSlim _lock;

        private int _onStoreCount;

        public ChatlogManager()
        {
            this._chatlogs = new List<ChatlogEntry>();
            this._lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

            this._onStoreCount = 0;
        }

        public void StoreChatlog(ChatlogEntry Entry)
        {
            this._lock.EnterUpgradeableReadLock();

            this._chatlogs.Add(Entry);

            this.OnChatlogStore();

            this._lock.ExitUpgradeableReadLock();
        }

        private void OnChatlogStore()
        {
            if (Interlocked.CompareExchange(ref this._onStoreCount, 1, 0) == 0)
            {
                if (this._chatlogs.Count >= FLUSH_ON_COUNT)
                {
                    this.FlushAndSave();
                }
            }
        }

        public void FlushAndSave()
        {
            this._lock.EnterWriteLock();

            if (this._chatlogs.Count > 0)
            {
                // todo: store chatlogs

                /*using (var DbCon = Mango.GetServer().GetDatabase().GetConnection())
                {
                    DbCon.Open();
                    DbCon.SetQuery("");
                }*/
            }

            this._lock.ExitWriteLock();
        }
    }
}
