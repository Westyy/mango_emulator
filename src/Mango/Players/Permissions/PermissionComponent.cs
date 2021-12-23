using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Permissions
{
    /// <summary>
    /// Permissions for a specific Player.
    /// </summary>
    sealed class PermissionComponent
    {
        /// <summary>
        /// Permission rights are stored here.
        /// </summary>
        private readonly List<string> _permissions;

        public PermissionComponent()
        {
            this._permissions = new List<string>();
        }

        /// <summary>
        /// Initialize the PermissionComponent.
        /// </summary>
        /// <param name="Player"></param>
        public bool Init(Player Player)
        {
            if (_permissions.Count > 0)
            {
                throw new InvalidOperationException("Permissions cannot be re-initialized, use the reload method");
            }

            List<string> PermissionSet = Mango.GetServer().GetPermissionManager().GetPermissionsForPlayer(Player);
            this._permissions.AddRange(PermissionSet);

            return true;
        }

        /// <summary>
        /// Reloads the permissions.
        /// </summary>
        /// <param name="Player"></param>
        public void Reload(Player Player)
        {
            // Clear the permissions first.
            this._permissions.Clear();

            // Reload the permissions.
            List<string> PermissionSet = Mango.GetServer().GetPermissionManager().GetPermissionsForPlayer(Player);
            this._permissions.AddRange(PermissionSet);
        }

        /// <summary>
        /// Checks if the user has the specified right.
        /// </summary>
        /// <param name="Right"></param>
        /// <returns></returns>
        public bool HasRight(string Right)
        {
            return this._permissions.Contains(Right);
        }
    }
}
