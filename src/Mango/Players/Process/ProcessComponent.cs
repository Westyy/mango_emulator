using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Mango.Players.Process
{
    sealed class ProcessComponent
    {
        private static readonly ILog log = LogManager.GetLogger("Mango.Players.Process.ProcessComponent");

        /// <summary>
        /// Player to update, handle, change etc.
        /// </summary>
        private Player _player = null;

        /// <summary>
        /// ThreadPooled Timer.
        /// </summary>
        private Timer _timer = null;

        /// <summary>
        /// Prevents the timer from overlapping itself.
        /// </summary>
        private bool _timerRunning = false;

        /// <summary>
        /// Checks if the timer is lagging behind (server can't keep up).
        /// </summary>
        private bool _timerLagging = false;

        /// <summary>
        /// Enable/Disable the timer WITHOUT disabling the timer itself.
        /// </summary>
        private bool _disabled = false;

        /// <summary>
        /// Used for disposing the ProcessComponent safely.
        /// </summary>
        private AutoResetEvent _resetEvent = new AutoResetEvent(true);

        /// <summary>
        /// How often the timer should execute.
        /// </summary>
        private static int _runtimeInSec = 60;

        /// <summary>
        /// Default.
        /// </summary>
        public ProcessComponent()
        {
        }
        
        /// <summary>
        /// Initializes the ProcessComponent.
        /// </summary>
        /// <param name="Player">Player.</param>
        public void Init(Player Player)
        {
            if (Player == null)
            {
                throw new InvalidOperationException("Player cannot be null.");
            }
            else if (this._player != null)
            {
                throw new InvalidOperationException("Cannot re-initialize the process componenet.");
            }

            this._player = Player;
            this._timer = new Timer(new TimerCallback(Run), null, _runtimeInSec * 1000, _runtimeInSec * 1000);
        }

        /// <summary>
        /// Called for each time the timer ticks.
        /// </summary>
        /// <param name="State"></param>
        public void Run(object State)
        {
            if (this._disabled)
            {
                return;
            }

            if (this._timerRunning)
            {
                this._timerLagging = true;
                log.Warn("<Player " + this._player.Id + "> Server can't keep up, Player timer is lagging behind.");
                return;
            }

            this._resetEvent.Reset();

            // BEGIN CODE

            this._player.Effects().CheckEffectExpiry(this._player);

            // END CODE

            // Reset the values
            this._timerRunning = false;
            this._timerLagging = false;

            this._resetEvent.Set();
        }

        /// <summary>
        /// Stops the timer and disposes everything.
        /// </summary>
        public void Dispose()
        {
            // Wait until any processing is complete first.
            try
            {
                this._resetEvent.WaitOne(TimeSpan.FromMinutes(5));
            }
            catch { } // give up

            // Set the timer to disabled
            this._disabled = true;

            // Dispose the timer to disable it.
            this._timer.Dispose();

            // Remove reference to the timer.
            this._timer = null;

            // Null the player so we don't reference it here anymore
            this._player = null;
        }
    }
}
