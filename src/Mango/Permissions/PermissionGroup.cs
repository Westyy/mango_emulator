using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Permissions
{
    class PermissionGroup
    {
        public virtual int Id
        {
            get;
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual string Description
        {
            get;
            set;
        }
    }
}
