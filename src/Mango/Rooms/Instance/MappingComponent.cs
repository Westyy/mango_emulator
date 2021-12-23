using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Rooms.Mapping;
using Mango.Items;
using Mango.Communication.Packets.Outgoing.Room.Engine;
using log4net;

namespace Mango.Rooms.Instance
{
    sealed class MappingComponent
    {
        private RoomInstance Instance = null;

        /// <summary>
        /// Redirects certain maps like beds.
        /// </summary>
        private Vector2D[,] _redirectGrid = null;

        /// <summary>
        /// Tile States which present Open, Blocked or Door tiles.
        /// </summary>
        private RoomTileState[,] _tileStates = null;

        private RoomTileEffect[,] _tileEffects = null;

        private double[,] _stackHeight = null;

        private double[,] _stackTopItemHeight = null;

        private List<RoomAvatar>[,] _userGrid = null;

        private UserMovementNode[,] _userMovementNodes = null;

        private string RelativeHeightmap = null;

        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int DoorX { get; set; }
        public int DoorY { get; set; }

        /// <summary>
        /// Initialize new instance of the MappingComponent.
        /// </summary>
        /// <param name="instance"></param>
        public MappingComponent(RoomInstance instance)
        {
            if (instance == null) throw new NullReferenceException("RoomInstance cannot be null");

            this.Instance = instance;
            this._redirectGrid = new Vector2D[instance.Model.Heightmap.SizeX, instance.Model.Heightmap.SizeY];
            this._tileStates = new RoomTileState[instance.Model.Heightmap.SizeX, instance.Model.Heightmap.SizeY];
            this._tileEffects = new RoomTileEffect[instance.Model.Heightmap.SizeX, instance.Model.Heightmap.SizeY];
            this._stackHeight = new double[instance.Model.Heightmap.SizeX, instance.Model.Heightmap.SizeY];
            this._stackTopItemHeight = new double[instance.Model.Heightmap.SizeX, instance.Model.Heightmap.SizeY];
            this._userGrid = new List<RoomAvatar>[instance.Model.Heightmap.SizeX, instance.Model.Heightmap.SizeY];
            this._userMovementNodes = new UserMovementNode[instance.Model.Heightmap.SizeX, instance.Model.Heightmap.SizeY];
            this.RelativeHeightmap = string.Empty;

            this.SizeX = instance.Model.Heightmap.SizeX;
            this.SizeY = instance.Model.Heightmap.SizeY;
            this.DoorX = instance.Model.DoorX;
            this.DoorY = instance.Model.DoorY;
        }

        public RoomTileEffect GetTileEffect(Vector2D Position)
        {
            return (this._tileEffects[Position.X, Position.Y] != null ? this._tileEffects[Position.X, Position.Y] : null);
        }

        public Vector2D GetRedirectedTarget(Vector2D Target)
        {
            return (this._redirectGrid[Target.X, Target.Y] != null ? this._redirectGrid[Target.X, Target.Y] : Target);
        }

        public bool IsValidPosition(Vector2D Position)
        {
            return (Position.X >= 0 && Position.Y >= 0 && Position.X < this.Instance.Model.Heightmap.SizeX && Position.Y < this.Instance.Model.Heightmap.SizeY);
        }

        public bool IsValidItemRotation(int Rotation)
        {
            return (Rotation == 0 || Rotation == 2 || Rotation == 4 || Rotation == 6);
        }

        /// <summary>
        /// Get the users step height for the position.
        /// </summary>
        /// <param name="Position"></param>
        /// <returns></returns>
        public double GetUserStepHeight(Vector2D Position)
        {
            RoomTileEffect Effect = this._tileEffects[Position.X, Position.Y];

            double Height = this._stackHeight[Position.X, Position.Y];

            if (Effect.InteractHeight >= 0)
            {
                Height -= this._stackTopItemHeight[Position.X, Position.Y];
            }

            return Height;
        }

        /// <summary>
        /// Checks if the position is available for the Avatar to move to.
        /// </summary>
        /// <param name="Position">The position to perform the check on.</param>
        /// <returns>True if it can or false if it can't.</returns>
        public bool CanInitiateMoveToPosition(Vector2D Position)
        {
            return (IsValidPosition(Position) && this._tileStates[Position.X, Position.Y] != RoomTileState.Blocked && this._userGrid[Position.X, Position.Y] == null
                && this._userMovementNodes[Position.X, Position.Y] != UserMovementNode.Blocked);
        }

        /// <summary>
        /// Is the target/position blocked?
        /// </summary>
        /// <param name="Target"></param>
        /// <returns></returns>
        public bool IsTargetBlocked(Vector2D Target)
        {
            if (_userMovementNodes[Target.X, Target.Y] == UserMovementNode.Blocked)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the next step is a valid move.
        /// </summary>
        /// <param name="From">The position the Avatar is currently on.</param>
        /// <param name="To">The position it wants to move to.</param>
        /// <param name="EndOfPath">Is this the very last step?</param>
        /// <param name="AvatarBlockGrid">The grid used to map out where users are (can be null)</param>
        /// <returns>True or false if the step is valid.</returns>
        public bool IsValidStep(Vector2D From, Vector2D To, bool EndOfPath, List<RoomAvatar>[,] AvatarBlockGrid = null)
        {
            if (AvatarBlockGrid == null)
            {
                AvatarBlockGrid = this._userGrid;
            }

            if (!IsValidPosition(To))
            {
                return false;
            }

            if (_tileStates[To.X, To.Y] == RoomTileState.Blocked || ((!Instance.DisableRoomBlocking ||
                EndOfPath) && AvatarBlockGrid[To.X, To.Y] != null) || this._userMovementNodes[To.X, To.Y] == UserMovementNode.Blocked ||
                (this._userMovementNodes[To.X, To.Y] == UserMovementNode.Walkable_End_Of_Route && !EndOfPath))
            {
                return false;
            }

            if ((To.X == this.DoorX && To.Y == DoorY) && !EndOfPath)
            {
                return false;
            }

            double HeightDiff = GetUserStepHeight(To) - GetUserStepHeight(From);

            if (HeightDiff > 1.5 || (this.Instance.Type == RoomType.PUBLIC && HeightDiff < -1.5))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the Avatars on the current position in the room.
        /// </summary>
        /// <param name="x">Position X.</param>
        /// <param name="y">Position Y.</param>
        /// <returns>The list of avatars on the square.</returns>
        public List<RoomAvatar> GetAvatarsOnPosition(Vector2D Position)
        {
            return this._userGrid[Position.X, Position.Y] == null ? new List<RoomAvatar>() : this._userGrid[Position.X, Position.Y];
        }

        /// <summary>
        /// Updates the Avatar grid to the new grid provided for the room.
        /// </summary>
        /// <param name="NewGrid">The new grid to replace the old.</param>
        public void UpdateUserGrid(List<RoomAvatar>[,] NewGrid)
        {
            this._userGrid = NewGrid;
        }

        /// <summary>
        /// Finds the correct placement height for the item.
        /// </summary>
        /// <param name="Item">The item to get the placement height for.</param>
        /// <param name="AffectedTiles">The tiles that are affected.</param>
        /// <param name="RotateOnly">Is the move a rotate only?</param>
        /// <returns>The placement height as a double.</returns>
        public double GetItemPlacementHeight(Item Item, List<Vector2D> AffectedTiles, bool RotateOnly)
        {
            double RootFloorHeight = -1;
            double HighestStack = 0;

            double[,] TempStackHeights = new double[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];

            foreach (Vector2D AffectedTile in AffectedTiles)
            {
                if (!IsValidPosition(AffectedTile) || this._tileStates[AffectedTile.X, AffectedTile.Y] == RoomTileState.Door)
                {
                    return -1;
                }

                if (RootFloorHeight == -1)
                {
                    RootFloorHeight = this.Instance.Model.Heightmap.FloorHeight[AffectedTile.X, AffectedTile.Y];
                }

                if (RootFloorHeight != this.Instance.Model.Heightmap.FloorHeight[AffectedTile.X, AffectedTile.Y])
                {
                    return -1;
                }

                TempStackHeights[AffectedTile.X, AffectedTile.Y] = RootFloorHeight;
                HighestStack = RootFloorHeight;

                if (Item.Data.StackingBehaviour != ItemStackingBehaviour.Ignore)
                {
                    bool CanRotateIntoAvatars = (Item.Data.Behaviour == ItemBehaviour.SEAT ||
                        Item.Data.Behaviour == ItemBehaviour.BED || Item.Data.Behaviour ==
                        ItemBehaviour.LOVE_SHUFFLER || Item.Data.Behaviour == ItemBehaviour.TELEPORTER ||
                        Item.Data.Behaviour == ItemBehaviour.ROLLER);

                    if ((!RotateOnly || !CanRotateIntoAvatars) && this.Instance.GetMapping().GetAvatarsOnPosition(AffectedTile).Count > 0)
                    {
                        return -1;
                    }
                }
            }

            if (Item.Data.StackingBehaviour == ItemStackingBehaviour.Ignore)
            {
                return RootFloorHeight;
            }

            ItemStackingBehaviour[,] TempStackBehaviors = new ItemStackingBehaviour[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];
            int[,] InitiatorLimitations = new int[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];

            foreach (Item StackItem in this.Instance.GetItems().GetWallAndFloor)
            {
                if (StackItem.Id == Item.Id || StackItem.Data.StackingBehaviour == ItemStackingBehaviour.Ignore)
                {
                    continue;
                }

                Vector2D MatchedTile = null;
                List<Vector2D> ItemTiles = CalculateAffectedTiles(StackItem, StackItem.Position.ToVector2D(), StackItem.RoomRot);

                foreach (Vector2D AffectedTile in AffectedTiles)
                {
                    foreach (Vector2D ItemTile in ItemTiles)
                    {
                        if (ItemTile.X == AffectedTile.X && ItemTile.Y == AffectedTile.Y)
                        {
                            MatchedTile = AffectedTile;

                            if (StackItem.Data.Behaviour == ItemBehaviour.ROLLER &&
                                    (Item.Data.SizeX != 1 || Item.Data.SizeY != 1))
                            {
                                return -1;
                            }

                            double ItemTotalHeight = StackItem.Position.Z + StackItem.Data.Height;

                            if (ItemTotalHeight >= TempStackHeights[MatchedTile.X, MatchedTile.Y])
                            {
                                TempStackHeights[MatchedTile.X, MatchedTile.Y] = ItemTotalHeight;
                                TempStackBehaviors[MatchedTile.X, MatchedTile.Y] = StackItem.Data.StackingBehaviour;
                            }

                            if (StackItem.Data.StackingBehaviour != ItemStackingBehaviour.Ignore)
                            {
                                InitiatorLimitations[MatchedTile.X, MatchedTile.Y]++;
                            }

                            if (ItemTotalHeight >= HighestStack)
                            {
                                HighestStack = ItemTotalHeight;
                            }
                        }
                    }
                }
            }

            if ((HighestStack + Item.Data.Height) >= 12) // to-do: stack height limit
            {
                return -1;
            }

            foreach (Vector2D AffectedTile in AffectedTiles)
            {
                if (((Item.Data.StackingBehaviour == ItemStackingBehaviour.Initiator || Item.Data.StackingBehaviour ==
                    ItemStackingBehaviour.Initiate_And_Terminate) && InitiatorLimitations[AffectedTile.X, AffectedTile.Y] > 0) ||
                    (TempStackBehaviors[AffectedTile.X, AffectedTile.Y] == ItemStackingBehaviour.Terminator ||
                    TempStackBehaviors[AffectedTile.X, AffectedTile.Y] == ItemStackingBehaviour.Initiate_And_Terminate))
                {
                    return -1;
                }
            }

            return HighestStack;
        }

        /// <summary>
        /// Finds the affected tiles for a item/position/rotation.
        /// </summary>
        /// <param name="Item"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Rot"></param>
        /// <returns></returns>
        public List<Vector2D> CalculateAffectedTiles(Item Item, Vector2D Position, int Rot)
        {
            List<Vector2D> Tiles = new List<Vector2D>();
            Tiles.Add(Position);

            if (Item.Data.SizeY > 0)
            {
                if (Rot == 0 || Rot == 4)
                {
                    for (int posY = 1; posY < Item.Data.SizeY; posY++)
                    {
                        Tiles.Add(new Vector2D(Position.X, Position.Y + posY));

                        for (int posX = 1; posX < Item.Data.SizeX; posX++)
                        {
                            Tiles.Add(new Vector2D(Position.X + posX, Position.Y + posY));
                        }
                    }
                }
                else if (Rot == 2 || Rot == 6)
                {
                    for (int posX = 1; posX < Item.Data.SizeY; posX++)
                    {
                        Tiles.Add(new Vector2D(Position.X + posX, Position.Y));

                        for (int posY = 1; posY < Item.Data.SizeX; posY++)
                        {
                            Tiles.Add(new Vector2D(Position.X + posX, Position.Y + posY));
                        }
                    }
                }
            }

            if (Item.Data.SizeX > 0)
            {
                if (Rot == 0 || Rot == 4)
                {
                    for (int posX = 1; posX < Item.Data.SizeX; posX++)
                    {
                        Vector2D Vector = new Vector2D(Position.X + posX, Position.Y);

                        if (!Tiles.Contains(Vector))
                        {
                            Tiles.Add(Vector);
                        }

                        for (int posY = 1; posY < Item.Data.SizeY; posY++)
                        {
                            Vector2D _Vector = new Vector2D(Position.X + posX, Position.Y + posY);

                            if (!Tiles.Contains(Vector))
                            {
                                Tiles.Add(_Vector);
                            }
                        }
                    }
                }
                else if (Rot == 2 || Rot == 6)
                {
                    for (int posY = 1; posY < Item.Data.SizeX; posY++)
                    {
                        Vector2D Vector = new Vector2D(Position.X, Position.Y + posY);

                        if (!Tiles.Contains(Vector))
                        {
                            Tiles.Add(Vector);
                        }

                        for (int posX = 1; posX < Item.Data.SizeY; posX++)
                        {
                            Vector2D _Vector = new Vector2D(Position.X + posX, Position.Y + posY);

                            if (!Tiles.Contains(Vector))
                            {
                                Tiles.Add(Vector);
                            }
                        }
                    }
                }
            }

            return Tiles;
        }

        /// <summary>
        /// Returns the relative heightmap as a string.
        /// </summary>
        /// <returns>Heightmap.</returns>
        public string GetRelativeHeightmap()
        {
            return this.RelativeHeightmap;
        }

        /// <summary>
        /// Regenerates the relative heightmap for a room, it must be recalculated each item placement or change.
        /// </summary>
        /// <param name="Broadcast">Should the map be broadcasted to all avatars? (resend the packet).</param>
        public void RegenerateRelativeHeightmap(bool Broadcast = false)
        {
            Instance.GetItems().GuestsCanPlaceStickies = false;

            /*this.StackHeight = new double[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];
            this.StackTopItemHeight = new double[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];
            this.UserMovementNodes = new UserMovementNode[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];
            this.TileEffects = new RoomTileEffect[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];
            this.RedirectGrid = new Vector2D[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];
            this.TileStates = new RoomTileState[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];*/

            for (int y = 0; y < this.Instance.Model.Heightmap.SizeY; y++)
            {
                for (int x = 0; x < this.Instance.Model.Heightmap.SizeX; x++)
                {
                    this._stackHeight[x, y] = this.Instance.Model.Heightmap.FloorHeight[x, y];
                    this._stackTopItemHeight[x, y] = 0;
                    this._userMovementNodes[x, y] = UserMovementNode.Walkable;
                    this._tileEffects[x, y] = new RoomTileEffect();
                    this._redirectGrid[x, y] = null;
                    this._tileStates[x, y] = RoomTileState.Open;
                }
            }

            bool[,] AnyItemInStack = new bool[this.Instance.Model.Heightmap.SizeX, this.Instance.Model.Heightmap.SizeY];

            foreach (Item Item in this.Instance.GetItems().GetWallAndFloor)
            {
                if (Item.Data.StackingBehaviour == ItemStackingBehaviour.Ignore)
                {
                    continue;
                }

                double TotalItemStackHeight = Item.Position.Z + Math.Round(Item.Data.Height, 1);
                List<Vector2D> AffectedTiles = CalculateAffectedTiles(Item, Item.Position.ToVector2D(), Item.RoomRot);

                RoomTileEffect Effect = new RoomTileEffect();
                UserMovementNode MovementNode = UserMovementNode.Blocked;

                switch (Item.Data.Behaviour)
                {
                    case ItemBehaviour.SEAT:
                    case ItemBehaviour.LOVE_SHUFFLER:

                        Effect = new RoomTileEffect(RoomTileEffectType.Sit, Item.Position.X, Item.Position.Y, Item.RoomRot, Item.Data.Height, 0, Item.Data.Id);
                        MovementNode = UserMovementNode.Walkable_End_Of_Route;
                        break;

                    case ItemBehaviour.BED:

                        Effect = new RoomTileEffect(RoomTileEffectType.Lay, Item.Position.X, Item.Position.Y, Item.RoomRot, Item.Data.Height, 0, Item.Data.Id);
                        MovementNode = UserMovementNode.Walkable_End_Of_Route;
                        break;

                    case ItemBehaviour.GATE:

                        MovementNode = Item.Flags == "1" ? UserMovementNode.Walkable : UserMovementNode.Blocked;
                        break;

                    case ItemBehaviour.AVATAR_EFFECT_GENERATOR:

                        Effect = new RoomTileEffect(RoomTileEffectType.Effect, Item.Position.X, Item.Position.Y, 0, 0, Item.Data.BehaviourData, 0);
                        break;

                    case ItemBehaviour.STICKY_POLE:

                        Instance.GetItems().GuestsCanPlaceStickies = true;
                        break;
                }

                foreach (Vector2D Tile in AffectedTiles)
                {
                    if (TotalItemStackHeight >= this._stackHeight[Tile.X, Tile.Y])
                    {
                        if (Item.Data.WalkableMode == ItemWalkableMode.Always)
                        {
                            MovementNode = UserMovementNode.Walkable;
                        }
                        else if (Item.Data.WalkableMode == ItemWalkableMode.Limited)
                        {
                            MovementNode = (AnyItemInStack[Tile.X, Tile.Y] && this._userMovementNodes[Tile.X, Tile.Y] != UserMovementNode.Walkable) ? UserMovementNode.Blocked : UserMovementNode.Walkable;
                        }

                        _stackHeight[Tile.X, Tile.Y] = TotalItemStackHeight;
                        _stackTopItemHeight[Tile.X, Tile.Y] = Item.Data.Height;
                        _userMovementNodes[Tile.X, Tile.Y] = MovementNode;
                        _tileEffects[Tile.X, Tile.Y] = Effect;

                        if (Item.Data.Behaviour == ItemBehaviour.BED)
                        {
                            if (Item.RoomRot == 2 || Item.RoomRot == 6)
                            {
                                _redirectGrid[Tile.X, Tile.Y] = new Vector2D(Item.Position.X, Tile.Y);
                            }

                            if (Item.RoomRot == 0 || Item.RoomRot == 4)
                            {
                                _redirectGrid[Tile.X, Tile.Y] = new Vector2D(Tile.X, Item.Position.Y);
                            }
                        }
                        else
                        {
                            _redirectGrid[Tile.X, Tile.Y] = null;
                        }

                        AnyItemInStack[Tile.X, Tile.Y] = true;
                    }
                }
            }

            foreach (ItemStaticPublics Object in Instance.GetItems().GetStaticItems)
            {
                _stackHeight[Object.X, Object.Y] = Instance.Model.Heightmap.FloorHeight[Object.X, Object.Y] + 1.0;
                _stackTopItemHeight[Object.X, Object.Y] = 1.0;
                _userMovementNodes[Object.X, Object.Y] = (Object.IsSeat ? UserMovementNode.Walkable_End_Of_Route : UserMovementNode.Blocked);

                if (Object.IsSeat)
                {
                    _tileEffects[Object.X, Object.Y] = new RoomTileEffect(RoomTileEffectType.Sit, Object.X, Object.Y, Object.Rotation, 1.0);
                }
                else if (Object.IsBed)
                {
                    _tileEffects[Object.X, Object.Y] = new RoomTileEffect(RoomTileEffectType.Lay, Object.X, Object.Y, Object.Rotation, 1.0);
                }
            }

            StringBuilder RelativeHeightmap = new StringBuilder();

            for (int y = 0; y < this.Instance.Model.Heightmap.SizeY; y++)
            {
                for (int x = 0; x < this.Instance.Model.Heightmap.SizeX; x++)
                {
                    if (this.Instance.Type == RoomType.FLAT && this.Instance.Model.DoorX == x && this.Instance.Model.DoorY == y)
                    {
                        this._tileStates[x, y] = RoomTileState.Door;
                        RelativeHeightmap.Append(Instance.Model.DoorZ);
                        continue;
                    }

                    if (Instance.Model.Heightmap.TileStates[x, y] == RoomTileState.Blocked)
                    {
                        this._tileStates[x, y] = RoomTileState.Blocked;
                        RelativeHeightmap.Append('x');
                        continue;
                    }

                    this._tileStates[x, y] = RoomTileState.Open;
                    RelativeHeightmap.Append(Instance.Model.Heightmap.FloorHeight[x, y]);
                }

                RelativeHeightmap.Append(Convert.ToChar(13));
            }

            string NewRelativeMap = RelativeHeightmap.ToString();

            if (!NewRelativeMap.Equals(this.RelativeHeightmap))
            {
                this.RelativeHeightmap = RelativeHeightmap.ToString();

                if (Broadcast)
                {
                    this.Instance.GetAvatars().BroadcastPacket(new FloorHeightMapComposer(NewRelativeMap));
                }
            }
        }

        public void Cleanup()
        {
            this.Instance = null;
            this._redirectGrid = null;
            this._tileStates = null;
            this._tileEffects = null;
            this._stackHeight = null;
            this._stackTopItemHeight = null;
            this._userGrid = null;
            this._userMovementNodes = null;
            this.RelativeHeightmap = null;
        }
    }
}
