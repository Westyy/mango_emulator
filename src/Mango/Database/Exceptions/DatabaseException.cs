using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Mango.Database.Exceptions
{
    /// <summary>
    /// A generic database exception
    /// </summary>
    [Serializable]
    class DatabaseException : DbException
    {
        public DatabaseException()
            : base()
        {
        }

        public DatabaseException(string message)
            : base(message)
        {
        }

        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DatabaseException(string message, int errorCode)
            : base(message, errorCode)
        {
        }

        protected DatabaseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
