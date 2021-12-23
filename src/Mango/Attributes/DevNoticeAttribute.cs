using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Attributes
{
    [Serializable]
    class DevNoticeAttribute : Attribute
    {
        public DevNoticeAttribute()
        {
        }

        public DevNoticeAttribute(string Message)
        {
        }
    }
}
