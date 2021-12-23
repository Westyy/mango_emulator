using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango
{
    class MangoConfiguration
    {
        // TCP Server Settings

        /// <summary>
        /// The IPAddress in which to listen for incoming connections on for this server.
        /// </summary>
        public string ServerIPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// The port in which to listen for incoming connections on for this server.
        /// </summary>
        public int ServerPort
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum allowed connections to this server.
        /// </summary>
        public int ServerMaxConnections
        {
            get;
            set;
        }

        // MySQL Settings

        /// <summary>
        /// The IPAddress in which to connect to the MySql Server.
        /// </summary>
        public string MySqlIPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// The port used to connect to the MySql Server.
        /// </summary>
        public uint MySqlPort
        {
            get;
            set;
        }

        /// <summary>
        /// The username used for authenticating with the MySql Server.
        /// </summary>
        public string MySqlUser
        {
            get;
            set;
        }

        /// <summary>
        /// The password used for authenticating with the MySql Server.
        /// </summary>
        public string MySqlPassword
        {
            get;
            set;
        }

        /// <summary>
        /// The database used with the MySql Server.
        /// </summary>
        public string MySqlDatabase
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum amount of connections available to MySql Server.
        /// </summary>
        public uint MySqlMaxConnections
        {
            get;
            set;
        }
    }
}
