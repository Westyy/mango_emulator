using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Subscriptions
{
    class SubscriptionData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Levels { get; set; }

        public SubscriptionData(int Id, string Name, int Levels)
        {
            this.Id = Id;
            this.Name = Name;
            this.Levels = Levels;
        }
    }
}
