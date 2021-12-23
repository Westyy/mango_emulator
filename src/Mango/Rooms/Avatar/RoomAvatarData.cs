using System.Collections.Generic;
using Mango.Players;
using Mango.Rooms.Mapping;
using Mango.Rooms.Avatar;
using Mango.Rooms.Mapping.PathFinding;

namespace Mango.Rooms
{
    /// <summary>
    /// RoomAvatarData holds all the information required for an Avatar.
    /// </summary>
    class RoomAvatarData
    {
        /// <summary>
        /// If this Avatar is a Player, we need direct access to the Player class so we reference it here.
        /// </summary>
        protected Player _player = null;

        /// <summary>
        /// The Avatar type, are we a Player, Pet or Bot? Default is Player.
        /// </summary>
        protected RoomAvatarType _type = RoomAvatarType.Player;

        /// <summary>
        /// The current 3D position of the Avatar.
        /// </summary>
        protected Vector3D _position = new Vector3D(0, 0, 0);

        /// <summary>
        /// Head Rotation with a default of 0.
        /// </summary>
        protected int _headRotation = 0;

        /// <summary>
        /// Body Rotation with a default of 0.
        /// </summary>
        protected int _bodyRotation = 0;

        /// <summary>
        /// The statusses the Avatar currently has (sit, lay etc).
        /// </summary>
        protected Dictionary<string, string> _statusses = new Dictionary<string, string>();

        /// <summary>
        /// The path this Avatar is currently walking.
        /// </summary>
        protected List<Vector2D> _path = new List<Vector2D>();

        /// <summary>
        /// The path the Avatar wants to change to (null if none).
        /// </summary>
        protected List<Vector2D> _pathToSet = null;

        /// <summary>
        /// The position the Avatar is requesting to move to.
        /// </summary>
        protected Vector2D _positionToSet = null;

        /// <summary>
        /// The target the Avatar is attempting to get to.
        /// </summary>
        protected Vector2D _target = null;

        /// <summary>
        /// Requests the Avatar to be updated (Effect Statusses, Walking etc).
        /// </summary>
        protected bool _updateNeeded = false;

        /// <summary>
        /// Item ID to move to and interact with.
        /// </summary>
        protected int _moveToAndInteract = 0;

        /// <summary>
        /// Data.
        /// </summary>
        protected int _moveToAndInteractData = 0;

        /// <summary>
        /// Can the user skip squares and walk fast.
        /// </summary>
        protected bool _fastWalking = false;

        /// <summary>
        /// Is this Avatar taking the teleporter?
        /// </summary>
        protected bool _isTeleporting = false;

        /// <summary>
        /// The teleporter id we are targeting.
        /// </summary>
        protected int _teleporterTargetId = 0;

        /// <summary>
        /// Current dance ID that this Avatar has.
        /// </summary>
        protected int _danceId = 0;

        /// <summary>
        /// The Item ID the Avatar is currently holding.
        /// </summary>
        protected int _carryItemId = 0;

        /// <summary>
        /// The timer for the current item this Avatar is holding.
        /// </summary>
        protected int _carryItemTimer = 0;

        /// <summary>
        /// The Effect ID this Avatar currently has.
        /// </summary>
        protected int _effectId = 0;

        /// <summary>
        /// Does this Avatar hold an effect by Item (water etc).
        /// </summary>
        protected bool _effectByItem = false;

        /// <summary>
        /// Effect ID set by the user using enable or costume etc.
        /// </summary>
        protected int _userSetEffectId = 0;

        /// <summary>
        /// Is this Avatar leaving the room.
        /// </summary>
        protected bool _leavingRoom = false;

        /// <summary>
        /// Is the Avatar forced leaving.
        /// </summary>
        protected bool _forcedLeave = false;

        /// <summary>
        /// Counts how many steps taken when leaving the room.
        /// </summary>
        protected int _leaveStepsTaken = 0;

        /// <summary>
        /// Ignores all furni redirections or restrictions.
        /// </summary>
        protected bool _overriding = false;

        /// <summary>
        /// Is this Avatars walking disabled.
        /// </summary>
        protected bool _walkingDisabled = false;

        /// <summary>
        /// How long this Avatar has been idle for.
        /// </summary>
        protected int _idleTime = 0;

        /// <summary>
        /// Is this Avatar sleeping.
        /// </summary>
        protected bool _sleeping = false;

        /// <summary>
        /// Prevents chat spam.
        /// </summary>
        protected int _chatMessagesSent = 0;

        /// <summary>
        /// Prevents chat spam.
        /// </summary>
        protected int _chatSpamTicks = 0;

        /// <summary>
        /// Sit
        /// </summary>
        protected bool _forceSit = false;

        /// <summary>
        /// Signs
        /// </summary>
        protected int _applySignId = 0;

        /// <summary>
        /// Signs
        /// </summary>
        protected int _signApplyTicks = 0;

        public RoomAvatarData()
        {
        }

        /// <summary>
        /// Resets all the values back to there defaults.
        /// </summary>
        public void ResetToDefaults()
        {
            this._position.X = 0;
            this._position.Y = 0;
            this._position.Z = 0;
            this._headRotation = 0;
            this._bodyRotation = 0;
            this._statusses.Clear();
            this._path.Clear();
            this._pathToSet = null;
            this._positionToSet = null;
            this._target = null;
            this._updateNeeded = false;
            this._moveToAndInteract = 0;
            this._moveToAndInteractData = 0;
            this._fastWalking = false;
            this._isTeleporting = false;
            this._teleporterTargetId = 0;
            this._danceId = 0;
            this._carryItemId = 0;
            this._carryItemTimer = 0;
            this._effectId = 0;
            this._effectByItem = false;
            this._leavingRoom = false;
            this._forcedLeave = false;
            this._leaveStepsTaken = 0;
            this._overriding = false;
            this._walkingDisabled = false;
            this._idleTime = 0;
            this._sleeping = false;
            this._forceSit = false;
            this._applySignId = 0;
            this._signApplyTicks = 0;
            //this._chatMessagesSent = 0;
            //this._chatSpamTicks = 0;
        }
    }
}
