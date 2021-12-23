using System;
using System.Collections.Generic;
using Mango.Players;
using Mango.Rooms.Mapping;
using Mango.Rooms.Avatar;
using Mango.Communication.Packets.Outgoing.Room.Session;
using Mango.Communication.Packets.Outgoing.Navigator;
using Mango.Communication.Packets.Outgoing.Handshake;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Communication.Packets.Outgoing.Room.Action;
using Mango.Communication.Packets.Outgoing.Room.Chat;
using Mango.Communication.Packets.Outgoing;
using Mango.Rooms.Mapping.PathFinding;
using Mango.Chat.Emotions;
using Mango.Items;
using Mango.Communication.Packets.Incoming.Room.Engine;

namespace Mango.Rooms
{
    class RoomAvatar : RoomAvatarData
    {
        /// <summary>
        /// Reference to the current room this avatar is in (null if none)
        /// </summary>
        private volatile RoomInstance _currentRoom = null;

        /// <summary>
        /// Links data for username, figure, motto etc.
        /// </summary>
        private readonly IAvatarData _data;

        /// <summary>
        /// Authentication Checking
        /// </summary>
        private bool _authOk = false;

        /// <summary>
        /// Initialize new instance of the RoomAvatar linked to the player.
        /// </summary>
        /// <param name="Player">Player.</param>
        public RoomAvatar(Player Player)
        {
            if (Player == null) throw new NullReferenceException("Player cannot be null.");

            base._player = Player;
            this._data = Player;
            base._type = RoomAvatarType.Player;
        }

        /// <summary>
        /// Returns the AvatarData (mainly used for packets only)
        /// </summary>
        public IAvatarData Data
        {
            get { return this._data; }
        }

        public Player Player
        {
            get
            {
                if (this.Type != RoomAvatarType.Player) { throw new InvalidOperationException("Avatar is not linked to a Player."); }
                return this._player;
            }
        }

        public RoomInstance CurrentRoom
        {
            get { return this._currentRoom; }
            set { this._currentRoom = value; }
        }

        public RoomAvatarType Type
        {
            get { return this._type; }
            set { this._type = value; }
        }

        public Vector3D Position
        {
            get { return this._position; }
            set { this._position = value; }
        }

        public int HeadRotation
        {
            get { return this._headRotation; }
            set { this._headRotation = value; }
        }

        public int BodyRotation
        {
            get { return this._bodyRotation; }
            set { this._bodyRotation = value; }
        }

        public Dictionary<string, string> Statusses
        {
            get { return this._statusses; }
            //set { this._statusses = value; }
        }

        public List<Vector2D> Path
        {
            get { return this._path; }
            set { this._path = value; }
        }

        public List<Vector2D> PathToSet
        {
            get { return this._pathToSet; }
            set { this._pathToSet = value; }
        }

        public Vector2D PositionToSet
        {
            get { return this._positionToSet; }
            set { this._positionToSet = value; }
        }

        public Vector2D Target
        {
            get { return this._target; }
            set { this._target = value; }
        }

        public bool UpdateNeeded
        {
            get { return this._updateNeeded; }
            set { this._updateNeeded = value; }
        }

        public int MoveToAndInteract
        {
            get { return this._moveToAndInteract; }
            set { this._moveToAndInteract = value; }
        }

        public int MoveToAndInteractData
        {
            get { return this._moveToAndInteract; }
            set { this._moveToAndInteractData = value; }
        }

        public bool FastWalking
        {
            get { return this._fastWalking; }
            set { this._fastWalking = value; }
        }

        public bool IsTeleporting
        {
            get { return this._isTeleporting; }
            set { this._isTeleporting = value; }
        }

        public int TeleporterTargetId
        {
            get { return this._teleporterTargetId; }
            set { this._teleporterTargetId = value; }
        }

        public int DanceId
        {
            get { return this._danceId; }
            set { this._danceId = value; }
        }

        public int CarryItemId
        {
            get { return this._carryItemId; }
            set { this._carryItemId = value; }
        }

        public int CarryItemTimer
        {
            get { return this._carryItemTimer; }
            set { this._carryItemTimer = value; }
        }

        public int EffectId
        {
            get { return this._effectId; }
            set { this._effectId = value; }
        }

        public bool EffectByItem
        {
            get { return this._effectByItem; }
            set { this._effectByItem = value; }
        }

        public int UserSetEffectId
        {
            get { return this._userSetEffectId; }
            set { this._userSetEffectId = value; }
        }

        public bool LeavingRoom
        {
            get { return this._leavingRoom; }
            set { this._leavingRoom = value; }
        }

        public bool ForcedLeave
        {
            get { return this._forcedLeave; }
            set { this._forcedLeave = value; }
        }

        public int LeaveStepsTaken
        {
            get { return this._leaveStepsTaken; }
            set { this._leaveStepsTaken = value; }
        }

        public bool Overriding
        {
            get { return this._overriding; }
            set { this._overriding = value; }
        }

        public bool WalkingDisabled
        {
            get { return this._walkingDisabled; }
            set { this._walkingDisabled = value; }
        }

        public int IdleTime
        {
            get { return this._idleTime; }
            set { this._idleTime = value; }
        }

        public bool Sleeping
        {
            get { return this._sleeping; }
            set { this._sleeping = value; }
        }

        public int ChatMessagesSent
        {
            get { return this._chatMessagesSent; }
            set { this._chatMessagesSent = value; }
        }

        public int ChatSpamTicks
        {
            get { return this._chatSpamTicks; }
            set { this._chatSpamTicks = value; }
        }

        public bool ForceSit
        {
            get { return this._forceSit; }
            set { this._forceSit = value; }
        }

        public int ApplySignId
        {
            get { return this._applySignId; }
            set { this._applySignId = value; }
        }

        public int SignApplyTicks
        {
            get { return this._signApplyTicks; }
            set { this._signApplyTicks = value; }
        }

        public bool InRoom
        {
            get { return (this.CurrentRoom != null); }
        }

        public int CurrentRoomId
        {
            get
            {
                return (this.CurrentRoom != null ? this.CurrentRoom.Id : 0);
            }
        }

        public Vector2D SquareInFront
        {
            get
            {
                if (!InRoom)
                {
                    throw new InvalidOperationException("Invalid call to method, item is not in a room.");
                }

                Vector2D posNow = new Vector2D(this.Position.X, this.Position.Y);

                if (this.BodyRotation == 0)
                {
                    posNow.Y--;
                }
                else if (this.BodyRotation == 2)
                {
                    posNow.X++;
                }
                else if (this.BodyRotation == 4)
                {
                    posNow.Y++;
                }
                else if (this.BodyRotation == 6)
                {
                    posNow.X--;
                }

                return posNow;
            }
        }

        public Vector2D SquareBehind
        {
            get
            {
                if (!InRoom)
                {
                    throw new InvalidOperationException("Invalid call to method, item is not in a room.");
                }

                Vector2D posNow = new Vector2D(this.Position.X, this.Position.Y);

                if (this.BodyRotation == 0)
                {
                    posNow.Y++;
                }
                else if (this.BodyRotation == 2)
                {
                    posNow.X--;
                }
                else if (this.BodyRotation == 4)
                {
                    posNow.Y--;
                }
                else if (this.BodyRotation == 6)
                {
                    posNow.X++;
                }

                return posNow;
            }
        }

        public bool HasStatus(string Key)
        {
            return this.Statusses.ContainsKey(Key);
        }

        public void RemoveStatus(string Key)
        {
            this.Statusses.Remove(Key);
        }

        public void SetStatus(string Key, string Value = "")
        {
            if (this.Statusses.ContainsKey(Key))
            {
                this.Statusses[Key] = Value;
            }
            else
            {
                this.Statusses.Add(Key, Value);
            }
        }

        public bool IsMoving
        {
            get
            {
                return (this.Path.Count > 0);
            }
        }

        public bool AuthOK
        {
            get { return this._authOk; }
            set { this._authOk = value; }
        }

        /// <summary>
        /// Attempts to load the requested room.
        /// </summary>
        /// <param name="roomId">RoomId to load.</param>
        /// <param name="password">Password input if required.</param>
        /// <param name="ignoreAuth">Ignore any form of authing for rooms.</param>
        public void PrepareRoom(int roomId, string password = "", bool ignoreAuth = false)
        {
            if (this.InRoom)
            {
                ExitRoom(false);
            }

            if (this.CurrentRoom != null)
            {
                Player.GetSession().SendPacket(new CloseConnectionComposer());
                throw new InvalidOperationException("CurrentRoom still has a reference to a room.");
            }

            RoomInstance instance = null;

            if (!Mango.GetServer().GetRoomManager().TryLoadRoom(roomId, out instance))
            {
                Player.GetSession().SendPacket(new CloseConnectionComposer());
                return;
            }

            this.CurrentRoom = instance;

            if (instance.Model == null)
            {
                Player.GetSession().SendPacket(new CloseConnectionComposer());
                return; // model is missing..
            }

            // check if room max users is reached
            if (instance.UsersNow >= instance.MaxUsers && !Player.GetPermissions().HasRight("room_enter_full"))
            {
                Player.GetSession().SendPacket(new CantConnectComposer(1));
                Player.GetSession().SendPacket(new CloseConnectionComposer());
                return;
            }

            // check if user is banned from room
            if (instance.GetBans().IsBanned(Player.GetAvatar()))
            {
                Player.GetSession().SendPacket(new CantConnectComposer(4));
                Player.GetSession().SendPacket(new CloseConnectionComposer());
                return;
            }

            this.AuthOK = (ignoreAuth || instance.OwnerId == Player.Id || Player.GetPermissions().HasRight("room_enter_locked"));

            if (instance.Type == RoomType.FLAT)
            {
                Player.GetSession().SendPacket(new OpenConnectionComposer());
            }

            if (!this.AuthOK)
            {
                if (instance.Access == RoomAccess.Password_Protected)
                {
                    if (instance.Password != password)
                    {
                        Player.GetSession().SendPacket(new GenericErrorComposer(-100002));
                        Player.GetSession().SendPacket(new CloseConnectionComposer());
                        return;
                    }
                }
                else if (instance.Access == RoomAccess.Locked)
                {
                    if (instance.UsersNow > 0)
                    {
                        Player.GetSession().SendPacket(new DoorbellComposer(null));
                        instance.GetAvatars().BroadcastPacket(new DoorbellComposer(Player), true);
                        return;
                    }
                    else
                    {
                        Player.GetSession().SendPacket(new FlatAccessDeniedComposer());
                        Player.GetSession().SendPacket(new CloseConnectionComposer());
                        //ExitRoom();
                        return;
                    }
                }
            }

            this.AuthOK = true;

            if (!EnterRoom())
            {
                Player.GetSession().SendPacket(new CloseConnectionComposer());
            }
        }

        /// <summary>
        /// Finalizes the enter room stage.
        /// </summary>
        /// <returns>If entering was successful or not.</returns>
        public bool EnterRoom()
        {
            if (!this.AuthOK)
            {
                return false;
            }

            // URL?
            Player.GetSession().SendPacket(new RoomReadyComposer(CurrentRoom, CurrentRoom.Model));

            if (CurrentRoom.Type == RoomType.FLAT)
            {
                Dictionary<string, string> decorations = new Dictionary<string, string>(CurrentRoom.Decorations); // concurrent shit
                foreach (KeyValuePair<string, string> kvp in decorations)
                {
                    Player.GetSession().SendPacket(new RoomPropertyComposer(kvp.Key, kvp.Value));
                }

                Player.GetSession().SendPacket(new RoomRatingComposer(CurrentRoom.Score)); // need to handle room ratings // -1 = (not yet rated room)
                //Player.GetSession().SendPacket(new RoomEventComposer(CurrentRoom.Event));
            }

            return true;
        }

        public void ExitRoom(bool SendKick = true)
        {
            if (!this.InRoom) { throw new InvalidOperationException("RoomAvatar is not in room."); }

            this.CurrentRoom.GetAvatars().RemoveAvatarFromRoom(this);
            this.ResetToDefaults();

            this.CurrentRoom = null;

            if (SendKick)
            {
                Player.GetSession().SendPacket(new CloseConnectionComposer());
            }

            this.Player.GetMessenger().SetUpdateNeeded(false);
        }

        public void Reset() // reset any values back to defaults
        {
            base.ResetToDefaults();
        }

        public void LeaveRoom(bool Forced = false)
        {
            if (!this.InRoom) { throw new InvalidOperationException("RoomAvatar is not in room."); }

            MoveTo(new Vector2D(this.GetCurrentRoom().Model.DoorX, this.GetCurrentRoom().Model.DoorY));
            this.LeavingRoom = true;
            this.ForcedLeave = Forced;
        }

        public void BlockWalking()
        {
            this.Path.Clear();
            this.WalkingDisabled = true;
        }

        public void UnblockWalking()
        {
            this.WalkingDisabled = false;
        }

        public void MoveToAndInteractItem(Item Item, int RequestData, Vector2D MoveToPosition = null)
        {
            this.MoveToAndInteract = Item.Id;
            this.MoveToAndInteractData = RequestData;

            MoveTo(MoveToPosition == null ? Item.SquareInFront : MoveToPosition);
        }

        public void MoveTo(Vector2D Target, bool IgnoreCanInitiate = false, bool IgnoreRedirections = false, bool Overriding = false)
        {
            if (!this.InRoom) throw new InvalidOperationException("RoomAvatar is not in room.");

            UnIdle();

            if (!this.GetCurrentRoom().GetMapping().IsValidPosition(Target))
            {
                return;
            }

            if (!MangoStaticSettings.WalkUpToFurniOnClick)
            {
                if (this.GetCurrentRoom().GetMapping().IsTargetBlocked(Target))
                {
                    return;
                }
            }

            this.Overriding = Overriding;

            if (this.Overriding)
            {
                IgnoreCanInitiate = true;
            }

            if (!IgnoreRedirections)
            {
                Vector2D rt = this.GetCurrentRoom().GetMapping().GetRedirectedTarget(Target);
                Target.X = rt.X;
                Target.Y = rt.Y;
            }

            if ((Target.X == this.Position.X && Target.Y == this.Position.Y) || this.ForcedLeave ||
                ((!IgnoreCanInitiate && !GetCurrentRoom().GetMapping().CanInitiateMoveToPosition(Target))) ||
                (this.WalkingDisabled && !Overriding))
            {
                return;
            }

            /*this.MoveToAndInteract = 0;
            this.MoveToAndInteractData = 0;*/

            if (this.PositionToSet != null)
            {
                this.Position.X = this.PositionToSet.X;
                this.Position.Y = this.PositionToSet.Y;
                this.Position.Z = this.CurrentRoom.GetMapping().GetUserStepHeight(new Vector2D(this.PositionToSet.X, this.PositionToSet.Y));

                this.PositionToSet = null;
            }

            this.ForceSit = false;
            this.Target = Target;
            this.PathToSet = PathFinder.FindPath(this.CurrentRoom.GetMapping(), this.Position.ToVector2D(), Target);
        }

        public void IncreaseIdleTime(bool IsOwner, bool Instant = false)
        {
            if (!this.InRoom) throw new InvalidOperationException("RoomAvatar is not in room.");

            if (Instant) { this.IdleTime = 600; } else { this.IdleTime++; }

            if (!this.Sleeping && this.IdleTime >= 600)
            {
                this.GetCurrentRoom().GetAvatars().BroadcastPacket(new SleepComposer(this, true));
                this.Sleeping = true;
            }
            else if (!this.LeavingRoom && this.IdleTime >= 1800)
            {
                if (MangoStaticSettings.RoomOwnerNoAFKKick && !IsOwner)
                {
                    return;
                }

                LeaveRoom();
                return;
            }
        }

        /// <summary>
        /// Increases the CarryItem Timer.
        /// </summary>
        public void IncreaseCarryItemTimer()
        {
            if (this.CarryItemTimer > 0)
            {
                this.CarryItemTimer--;

                if (this.CarryItemTimer == 0)
                {
                    CarryItem(0);
                }
            }
        }

        /// <summary>
        /// Handles the chat ticks to prevent chat spam.
        /// </summary>
        public void HandleChatSpamTicks()
        {
            if (this.ChatSpamTicks >= 0)
            {
                this.ChatSpamTicks--;

                if (this.ChatSpamTicks == -1)
                {
                    this.ChatMessagesSent = 0;
                }
            }
        }

        /// <summary>
        /// Increases the AntiSpam Ticks as the user is typing.
        /// </summary>
        public void IncreaseAntiSpam()
        {
            if (!this.InRoom) { throw new InvalidOperationException("RoomAvatar is not in room."); }

            this.ChatMessagesSent++;

            if (this.ChatSpamTicks == -1)
            {
                this.ChatSpamTicks = 16; // divide it by 2 to get the real 'seconds'
            }
            else if (this.ChatMessagesSent >= 5)
            {
                this.Player.Mute(MangoStaticSettings.RoomFloodMuteTimeInSec);
                this.Player.GetSession().SendPacket(new FloodControlComposer(MangoStaticSettings.RoomFloodMuteTimeInSec));
            }
        }

        public void UnIdle()
        {
            if (!this.InRoom) { throw new InvalidOperationException("RoomAvatar is not in room."); }

            this.IdleTime = 0;

            if (this.Sleeping)
            {
                this.GetCurrentRoom().GetAvatars().BroadcastPacket(new SleepComposer(this, false));
                this.Sleeping = false;
            }
        }

        public void ApplySign(int SignId)
        {
            if (!this.InRoom) { throw new InvalidOperationException("RoomAvatar is not in room."); }

            UnIdle();

            this._signApplyTicks = 0;
            this._applySignId = SignId;
        }

        public void Wave(int Action)
        {
            if (!this.InRoom) { throw new InvalidOperationException("RoomAvatar is not in room."); }

            if (this.DanceId > 0) // you stop dancing when you wave
            {
                this.DanceId = 0;
            }

            UnIdle();
            this.GetCurrentRoom().GetAvatars().BroadcastPacket(new WaveComposer(this, Action));

            if (Action == 5)
            {
                //this.GetCurrentRoom().GetAvatars().BroadcastPacket(new SleepComposer(this, true));
                this.IncreaseIdleTime(false, true);
            }
        }

        public void Dance(int DanceId, bool Broadcast = true)
        {
            if (!this.InRoom) { throw new InvalidOperationException("RoomAvatar is not in room."); }

            if (this.DanceId == DanceId)
            {
                return;
            }

            UnIdle();

            this.DanceId = DanceId;

            if (Broadcast)
            {
                this.GetCurrentRoom().GetAvatars().BroadcastPacket(new DanceComposer(this, DanceId));
            }
        }

        public void CarryItem(int ItemId, bool Broadcast = true)
        {
            if (!this.InRoom) { throw new InvalidOperationException("RoomAvatar is not in room."); }

            UnIdle();

            this.CarryItemId = ItemId;

            if (ItemId > 0)
            {
                this.CarryItemTimer = 90;
            }

            if (Broadcast)
            {
                this.GetCurrentRoom().GetAvatars().BroadcastPacket(new CarryObjectComposer(this, ItemId));
            }
        }

        public void ApplyEffect(int EffectId, bool Brodcast = true, bool ByItem = false)
        {
            UnIdle();

            if ((!ByItem && this._effectByItem) || EffectId == this._effectId)
            {
                return;
            }

            this._effectId = EffectId;
            this._effectByItem = (ByItem && EffectId > 0);

            if (Brodcast)
            {
                if (!this.InRoom)
                {
                    throw new InvalidOperationException("Cannot broadcast when Avatar is not in a room.");
                }

                this.CurrentRoom.GetAvatars().BroadcastPacket(new AvatarEffectComposer(this, EffectId));
            }
        }

        public void Chat(string Message, int Colour, bool OverrideRoomMute = false)
        {
            if (!this.InRoom) throw new InvalidOperationException("RoomAvatar is not in room.");

            if (this.Player.Muted)
            {
                return;
            }

            if (this.LeavingRoom && this.ForcedLeave)
            {
                return;
            }

            UnIdle();

            if (Mango.GetServer().GetChatManager().GetCommands().Parse(Player.GetSession(), Message))
            {
                return;
            }

            IncreaseAntiSpam();

            this.GetCurrentRoom().GetAvatars().BroadcastPacket(new ChatComposer(this.Player, Message, Mango.GetServer().GetChatManager().GetEmotions().GetEmotionsForText(Message), Colour));
        }

        public void Shout(string Message, int Colour, bool OverrideRoomMute = false)
        {
            if (!this.InRoom) throw new InvalidOperationException("RoomAvatar is not in room.");

            if (this.Player.Muted)
            {
                return;
            }

            if (this.LeavingRoom && this.ForcedLeave)
            {
                return;
            }

            UnIdle();

            IncreaseAntiSpam();

            this.GetCurrentRoom().GetAvatars().BroadcastPacket(new ShoutComposer(this.Player, Message, Mango.GetServer().GetChatManager().GetEmotions().GetEmotionsForText(Message), Colour));
        }

        public void Whisper(string Message, string Username, int Colour, bool OverrideRoomMute = false)
        {
            if (!this.InRoom) throw new InvalidOperationException("RoomAvatar is not in room.");

            Player InteractingPlayer = null;

            if (!Mango.GetServer().GetPlayerManager().TryGet(Username, out InteractingPlayer))
            {
                return;
            }

            if (this.Player.Muted)
            {
                return;
            }

            if (this.LeavingRoom && this.ForcedLeave)
            {
                return;
            }

            UnIdle();

            IncreaseAntiSpam();

            this.Player.GetSession().SendPacket(new WhisperComposer(this.Player, Message, 0, Colour));
            InteractingPlayer.GetSession().SendPacket(new WhisperComposer(this.Player, Message, 0, Colour));
        }

        public RoomInstance GetCurrentRoom()
        {
            return this.CurrentRoom;
        }
    }
}
