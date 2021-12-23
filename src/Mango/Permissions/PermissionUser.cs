using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Permissions
{
    class PermissionUser
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual int UserId
        {
            get;
            set;
        }

        public virtual int PermissionId
        {
            get;
            set;
        }
    }
}
