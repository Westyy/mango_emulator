using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Permissions
{
    class PermissionSubscription
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual int SubscriptionId
        {
            get;
            set;
        }

        public virtual int PermissionId
        {
            get;
            set;
        }

        public virtual int LevelRequired
        {
            get;
            set;
        }
    }
}
