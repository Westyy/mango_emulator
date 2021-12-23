using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Mango.Rooms.Mapping;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using Mango.Rooms.Mapping.PathFinding;
using Mango.Items;
using Mango.Items.Events;
using log4net;

namespace Mango.Rooms.Instance
{
    sealed class ProcessComponent
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Rooms.Instance.ProcessComponent");

        /// <summary>
        /// RoomInstance.
        /// </summary>
        private RoomInstance Instance = null;

        /// <summary>
        /// The in-accurate timer itself which runs on the thread pool.
        /// </summary>
        private Timer _timer = null;

        /// <summary>
        /// Prevents the process from overlapping.
        /// </summary>
        private volatile bool _running = false;

        /// <summary>
        /// Checks if the server can keep up with the process.
        /// </summary>
        private volatile bool _lagging = false;

        private volatile int _retries = 0;

        private object _overlapLock = new object();

        public ProcessComponent(RoomInstance Instance)
        {
            if (Instance == null) throw new NullReferenceException("RoomInstance cannot be null");

            this.Instance = Instance;
        }

        /// <summary>
        /// Initializes and begins the timer running.
        /// </summary>
        public void Init()
        {
            this._timer = new Timer(new TimerCallback(Run), null, 2000, 500);
        }

        public void Run(object state)
        {
            lock (this._overlapLock)
            {
                if (this._running)
                {
                    this._retries++;

                    if (_retries > 12)
                    {
                        this._running = false;
                        this._retries = 0;
                    }

                    this._lagging = true;
                    log.Warn("<Room " + this.Instance.Id + "> Server can't keep up, Room Processing timer is lagging behind.");
                    return;
                }
            }

            DateTime Start = DateTime.Now;

            this._running = true;

            if (this.Instance.Unloaded)
            {
                return;
            }

            if (this.Instance.GetAvatars().Count == 0)
            {
                this.Instance.IdleTime++;
            }
            else if (this.Instance.IdleTime > 0)
            {
                this.Instance.IdleTime = 0;
            }

            if (this.Instance.HasActivePromotion && this.Instance.Promotion.HasExpired)
            {
                this.Instance.EndPromotion();
            }

            if (this.Instance.IdleTime >= 1200 && !this.Instance.HasActivePromotion)
            {
                Mango.GetServer().GetRoomManager().UnloadRoom(this.Instance);
                return;
            }

            List<RoomAvatar> UpdateNeeded = new List<RoomAvatar>();
            List<RoomAvatar> NeedsRemoving = new List<RoomAvatar>();
            List<RoomAvatar>[,] NewUserGrid = new List<RoomAvatar>[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];

            foreach (RoomAvatar Avatar in this.Instance.GetAvatars().Avatars)
            {
                if (Avatar.Type == RoomAvatarType.Player)
                {
                    // idle timer, item timers and chat spam ticks
                    Avatar.IncreaseIdleTime(Avatar.Player.Id == this.Instance.OwnerId);
                    Avatar.IncreaseCarryItemTimer();
                    Avatar.HandleChatSpamTicks();
                }

                if (Avatar.HasStatus("mv"))
                {
                    Avatar.RemoveStatus("mv");
                    Avatar.UpdateNeeded = true;
                }

                if (Avatar.PathToSet != null)
                {
                    Avatar.LeavingRoom = false;
                    Avatar.LeaveStepsTaken = 0;

                    Avatar.Path = new List<Vector2D>(Avatar.PathToSet);
                    Avatar.PathToSet.Clear();
                    Avatar.PathToSet = null;
                }

                if (Avatar.PositionToSet != null)
                {
                    if (Avatar.Type == RoomAvatarType.Player && Avatar.PositionToSet.X == this.Instance.Model.DoorX && Avatar.PositionToSet.Y == this.Instance.Model.DoorY)
                    {
                        NeedsRemoving.Add(Avatar);
                        continue;
                    }

                    Avatar.Position.X = Avatar.PositionToSet.X;
                    Avatar.Position.Y = Avatar.PositionToSet.Y;
                    Avatar.Position.Z = this.Instance.GetMapping().GetUserStepHeight(new Vector2D(Avatar.Position.X, Avatar.Position.Y));

                    Avatar.PositionToSet = null;

                    if (!Avatar.IsMoving && Avatar.MoveToAndInteract > 0 && Avatar.Type == RoomAvatarType.Player)
                    {
                        Item Item = null;

                        if (this.Instance.GetItems().TryGetItem(Avatar.MoveToAndInteract, out Item))
                        {
                            Mango.GetServer().GetItemEventManager().Handle(Avatar.Player.GetSession(), Item, ItemEventType.Interact, this.Instance);
                        }

                        Avatar.MoveToAndInteract = 0;
                        Avatar.MoveToAndInteractData = 0;
                    }
                }

                if (Avatar.IsMoving)
                {
                    Vector2D NextStep = Avatar.Path[0];
                    Avatar.Path.Remove(NextStep);

                    if (Avatar.FastWalking && Avatar.Path.Count > 0)
                    {
                        NextStep = Avatar.Path[0];
                        Avatar.Path.Remove(NextStep);
                    }

                    bool LastStep = (Avatar.Path.Count == 0);

                    if (NextStep != null && ((!Avatar.Overriding && this.Instance.GetMapping().IsValidPosition(NextStep)) ||
                        (this.Instance.GetMapping().IsValidStep(Avatar.Position.ToVector2D(), NextStep, LastStep, NewUserGrid))))
                    {
                        if (!Avatar.LeavingRoom && Avatar.Target.X == this.Instance.Model.DoorX && Avatar.Target.Y == this.Instance.Model.DoorY)
                        {
                            Avatar.LeavingRoom = true;
                        }

                        if (Avatar.LeavingRoom)
                        {
                            Avatar.LeaveStepsTaken++;
                        }

                        if (Avatar.LeaveStepsTaken == 6)
                        {
                            Avatar.ApplyEffect(108);
                        }

                        if (Avatar.LeaveStepsTaken >= 7)
                        {
                            NeedsRemoving.Add(Avatar);
                            continue;
                        }
                        
                        // Update the mv status
                        Avatar.SetStatus("mv", NextStep.X + "," + NextStep.Y + "," + (Math.Round(this.Instance.GetMapping().GetUserStepHeight(NextStep), 1)).ToString().Replace(',', '.'));
                        Avatar.PositionToSet = NextStep;

                        // Update new/temporary grid with our new move to position
                        if (NewUserGrid[NextStep.X, NextStep.Y] == null)
                        {
                            NewUserGrid[NextStep.X, NextStep.Y] = new List<RoomAvatar>();
                        }

                        NewUserGrid[NextStep.X, NextStep.Y].Add(Avatar);

                        // Remove any sit statusses
                        if (Avatar.HasStatus("sit"))
                        {
                            Avatar.RemoveStatus("sit");
                        }

                        // Remove any lay statusses
                        if (Avatar.HasStatus("lay"))
                        {
                            Avatar.RemoveStatus("lay");
                        }

                        // Update rotation
                        Avatar.BodyRotation = AvatarRotation.Calculate(Avatar.Position.ToVector2D(), NextStep);
                        Avatar.HeadRotation = Avatar.BodyRotation;

                        // Request update for next @B cycle
                        Avatar.UpdateNeeded = true;
                    }
                    else
                    {
                        // Update new/temporary grid with our new move to position
                        if (NewUserGrid[NextStep.X, NextStep.Y] == null)
                        {
                            NewUserGrid[NextStep.X, NextStep.Y] = new List<RoomAvatar>();
                        }

                        NewUserGrid[NextStep.X, NextStep.Y].Add(Avatar);

                        Avatar.Path.Clear();
                    }
                }
                else
                {
                    if (NewUserGrid[Avatar.Position.X, Avatar.Position.Y] == null)
                    {
                        NewUserGrid[Avatar.Position.X, Avatar.Position.Y] = new List<RoomAvatar>();
                    }

                    NewUserGrid[Avatar.Position.X, Avatar.Position.Y].Add(Avatar);

                    Avatar.Path.Clear();
                }

                if (!Avatar.IsMoving && Avatar.LeavingRoom)
                {
                    NeedsRemoving.Add(Avatar);
                    continue;
                }

                UpdateAvatarStatus(Avatar);

                if (Avatar.UpdateNeeded)
                {
                    UpdateNeeded.Add(Avatar);
                    Avatar.UpdateNeeded = false;
                }
            }

            foreach (RoomAvatar Avatar in NeedsRemoving)
            {
                Avatar.ExitRoom(true);
            }

            // Set the door position on the user grid
            NewUserGrid[Instance.Model.DoorX, Instance.Model.DoorY] = null;

            // Update the user grid
            Instance.GetMapping().UpdateUserGrid(NewUserGrid);

            if (UpdateNeeded.Count > 0)
            {
                this.Instance.GetAvatars().BroadcastPacket(new UserUpdateComposer(UpdateNeeded));
            }

            int Finish = (DateTime.Now - Start).Milliseconds;

            if (Finish >= 400)
            {
                log.Error("<Room " + this.Instance.Id + "> is overlapping and ran for " + Finish + "ms.");
            }

            DateTime ItemTime = DateTime.Now;

            // Process Items
            foreach (Item Item in this.Instance.GetItems().GetWallAndFloor)
            {
                Item.Update(this.Instance);
            }

            this.Instance.GetItems().ClearRolledItems();

            int FinishItems = (DateTime.Now - ItemTime).Milliseconds;

            if ((Finish + FinishItems) >= 400)
            {
                log.Error("<Room " + this.Instance.Id + "> is overlapping and finished items and users in " + (Finish + FinishItems) + "ms. Items took " + FinishItems + "ms and users took " + Finish + "ms.");
            }

            lock (this._overlapLock)
            {
                this._running = false;
                this._lagging = false;
            }
        }

        private void UpdateAvatarStatus(RoomAvatar Avatar)
        {
            // Apply Tile Effects
            RoomTileEffect Effect = this.Instance.GetMapping().GetTileEffect(Avatar.Position.ToVector2D());

            if (Effect == null)
            {
                return;
            }

            Dictionary<string, string> CurrentStatusses = Avatar.Statusses;

            // Apply Force Sit
            if (((Avatar.ForceSit && Effect.Type == RoomTileEffectType.None)
                || (Avatar.ForceSit && Effect.Type != RoomTileEffectType.Sit)) && !Avatar.IsMoving)
            {
                if (Avatar.BodyRotation % 2 != 0)
                {
                    Avatar.BodyRotation--;
                }

                Avatar.SetStatus("sit", (Avatar.Position.Z + 0.65).ToString().Replace(',', '.'));
                Avatar.UpdateNeeded = true;
            }
            else if (!Avatar.ForceSit && CurrentStatusses.ContainsKey("sit"))
            {
                Avatar.RemoveStatus("sit");
                Avatar.UpdateNeeded = true;
            }

            if (Effect.Type == RoomTileEffectType.Sit && !CurrentStatusses.ContainsKey("mv"))
            {
                string OldStatus = (CurrentStatusses.ContainsKey("sit") ? CurrentStatusses["sit"] : string.Empty);
                string NewStatus = Math.Round(Effect.InteractHeight, 1).ToString().Replace(',', '.');

                if (Avatar.BodyRotation != Effect.Rot)
                {
                    Avatar.BodyRotation = Effect.Rot;
                    Avatar.HeadRotation = Effect.Rot;
                    Avatar.UpdateNeeded = true;
                }

                if (NewStatus != OldStatus)
                {
                    Avatar.SetStatus("sit", NewStatus);
                    Avatar.UpdateNeeded = true;
                }
            }
            else if (CurrentStatusses.ContainsKey("sit") && Effect.Type != RoomTileEffectType.Sit && !Avatar.ForceSit)
            {
                Avatar.RemoveStatus("sit");
                Avatar.UpdateNeeded = true;
            }

            if (Effect.Type == RoomTileEffectType.Lay && !CurrentStatusses.ContainsKey("mv"))
            {
                string OldStatus = (CurrentStatusses.ContainsKey("lay") ? CurrentStatusses["lay"] : string.Empty);
                string NewStatus = Math.Round(Effect.InteractHeight, 1).ToString().Replace(',', '.');

                if (Avatar.BodyRotation != Effect.Rot)
                {
                    Avatar.BodyRotation = Effect.Rot;
                    Avatar.HeadRotation = Effect.Rot;
                    Avatar.UpdateNeeded = true;
                }

                if (OldStatus != NewStatus)
                {
                    Avatar.SetStatus("lay", NewStatus);
                    Avatar.UpdateNeeded = true;
                }
            }
            else if (CurrentStatusses.ContainsKey("lay") && Effect.Type != RoomTileEffectType.Lay)
            {
                Avatar.RemoveStatus("lay");
                Avatar.UpdateNeeded = true;
            }

            if (Effect.Type == RoomTileEffectType.Effect)
            {
                if ((Avatar.EffectId != Effect.EffectId) || !Avatar.EffectByItem)
                {
                    Avatar.ApplyEffect(Effect.EffectId, true, true);
                }
            }
            else if (Avatar.EffectByItem)
            {
                int ClearEffect = 0;

                if (Avatar.Type == RoomAvatarType.Player)
                {
                    ClearEffect = Avatar.UserSetEffectId;
                }
                else
                {
                    // bots
                }

                Avatar.ApplyEffect(ClearEffect, true, true);
            }

            if (Avatar.ApplySignId > 0)
            {
                if (Avatar.SignApplyTicks >= 12 && Avatar.HasStatus("sign"))
                {
                    Avatar.ApplySignId = 0;
                    Avatar.SignApplyTicks = 0;

                    Avatar.RemoveStatus("sign");
                }
                else
                {
                    Avatar.SetStatus("sign", Avatar.ApplySignId.ToString());
                    Avatar.UpdateNeeded = true;
                    Avatar.SignApplyTicks++;
                }
            }
        }

        public void Cleanup()
        {
            this._timer.Dispose(); // always must be run to stop the timer

            this.Instance = null;
            this._timer = null;
        }
    }
}
