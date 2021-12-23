using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Rooms.Avatar
{
    interface IAvatarData
    {
        int Id { get; }
        string Username { get; }
        string Figure { get; }
        string Motto { get; }
    }
}
