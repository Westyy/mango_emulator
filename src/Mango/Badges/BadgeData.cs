using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Badges
{
    sealed class BadgeData
    {
        private int _id;
        private string _code;

        public BadgeData(int Id, string Code)
        {
            this._id = Id;
            this._code = Code;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Code
        {
            get { return this._code; }
            set { this._code = value; }
        }
    }
}
