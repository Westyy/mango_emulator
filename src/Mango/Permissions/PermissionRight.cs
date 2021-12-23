using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Permissions
{
    class PermissionRight
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual int GroupId
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
