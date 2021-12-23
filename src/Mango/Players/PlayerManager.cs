using System;
using System.Collections.Concurrent;
using Mango.Communication.Packets.Outgoing;
using log4net;
using System.Collections.Generic;

namespace Mango.Players
{
    sealed class PlayerManager
    {
        private static ILog log = LogManager.GetLogger("Mango.Players.PlayerManager");

        /// <summary>
        /// This collection is used for storing the active players by there player id.
        /// </summary>
        private readonly ConcurrentDictionary<int, Player> _players;

        /// <summary>
        /// This collection is used for storing the player usernames to the player id.
        /// </summary>
        private readonly ConcurrentDictionary<string, int> _playerNamesToId;

        /// <summary>
        /// Initializes new instance of the PlayerManager.
        /// </summary>
        /// <param name="ConcurrencyLevel"></param>
        /// <param name="MaxCapacity"></param>
        public PlayerManager(int ConcurrencyLevel, int MaxCapacity)
        {
            this._players = new ConcurrentDictionary<int, Player>(ConcurrencyLevel, MaxCapacity);
            this._playerNamesToId = new ConcurrentDictionary<string, int>(ConcurrencyLevel, MaxCapacity);
        }

        /// <summary>
        /// Collection of players currently active.
        /// </summary>
        public ICollection<Player> Players
        {
            get { return this._players.Values; }
        }

        /// <summary>
        /// Attempts to add the player, if it fails it performs a roll-back on any changes it did make.
        /// </summary>
        /// <param name="player">The player to store.</param>
        /// <returns>True if success, false if failed.</returns>
        public bool TryAdd(Player player)
        {
            if (this._players.TryAdd(player.Id, player))
            {
                if (this._playerNamesToId.TryAdd(player.Username, player.Id))
                {
                    return true;
                }
                else
                {
                    Player taken;
                    this._players.TryRemove(player.Id, out taken);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to remove a player.
        /// </summary>
        /// <param name="player">The player to remove.</param>
        /// <returns>True if success, false if failed.</returns>
        public bool TryRemove(Player player)
        {
            Player playerTaken;
            int idTaken;

            if (this._players.TryRemove(player.Id, out playerTaken))
            {
                if (this._playerNamesToId.TryRemove(player.Username, out idTaken))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to return a player from the user id supplied.
        /// </summary>
        /// <param name="playerId">The user id.</param>
        /// <param name="player">The player that is returned.</param>
        /// <returns>True if success, false if failed.</returns>
        public bool TryGet(int playerId, out Player player)
        {
            return this._players.TryGetValue(playerId, out player);
        }

        /// <summary>
        /// Attempts to return a player from the player username.
        /// </summary>
        /// <param name="playerUsername">The players username</param>
        /// <param name="player">The player that is returned.</param>
        /// <returns>True if success, false if failed.</returns>
        public bool TryGet(string playerUsername, out Player player)
        {
            int id;

            if (this._playerNamesToId.TryGetValue(playerUsername, out id))
            {
                return this._players.TryGetValue(id, out player);
            }
            else
            {
                player = null;
                return false;
            }
        }

        /// <summary>
        /// Broadcasts a packet to all online players.
        /// </summary>
        /// <param name="packet">The packet to broadcast.</param>
        public void BroadcastPacket(ServerPacket packet)
        {
            foreach (Player player in _players.Values)
            {
                player.GetSession().SendPacket(packet);
            }
        }

        public void DisconnectAll()
        {
            Dictionary<int, Player> Players = new Dictionary<int, Player>(_players);

            foreach (Player Player in Players.Values)
            {
                // TO-DO: Better way of cleaning up and dc players!
                Player.GetSession().Disconnect();
            }
        }

        public int Count
        {
            get
            {
                return this._players.Count;
            }
        }
    }
}
