using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Navigator
{
    class FlatCategory
    {
        public int Id { get; set; }
        public int OrderNum { get; set; }
        public bool Visible { get; set; }
        public string Title { get; set; }
        public bool AllowTrade { get; set; }

        public FlatCategory(int Id, int OrderNum, int Visible, string Title, int Trade)
        {
            this.Id = Id;
            this.OrderNum = OrderNum;
            this.Visible = Visible == 1 ? true : false;
            this.Title = Title;
            this.AllowTrade = Trade == 1 ? true : false;
        }
    }
}
