using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Utilities;

namespace Mango.Catalog
{
    sealed class CatalogPage
    {
        private int _id;
        private int _parentId;
        private int _orderNum;
        private bool _enabled;
        private string _caption;
        private int _icon;
        private int _colour;
        private string _requiredRight;
        private bool _visible;
        private bool _dummyPage;
        private string _template;
        private List<string> _pageStrings1;
        private List<string> _pageStrings2;
        private Dictionary<int, CatalogItem> _items;

        public CatalogPage(int Id, int ParentId, int OrderNum, string Enabled, string Caption, int Icon, int Colour, string RequiredRight,
            string Visible, string DummyPage, string Template, string PageStrings1, string PageStrings2, Dictionary<int, CatalogItem> Items)
        {
            this._id = Id;
            this._parentId = ParentId;
            this._orderNum = OrderNum;
            this._enabled = Enabled.ToLower() == "y" ? true : false;
            this._caption = Caption;
            this._icon = Icon;
            this._colour = Colour;
            this._requiredRight = RequiredRight;
            this._visible = Visible.ToLower() == "y" ? true : false;
            this._dummyPage = DummyPage.ToLower() == "y" ? true : false;
            this._template = Template;

            foreach (string Str in PageStrings1.Split('|'))
            {
                if (this._pageStrings1 == null) { this._pageStrings1 = new List<string>(); }
                this._pageStrings1.Add(Str);
            }

            foreach (string Str in PageStrings2.Split('|'))
            {
                if (this._pageStrings2 == null) { this._pageStrings2 = new List<string>(); }
                this._pageStrings2.Add(Str);
            }

            this._items = Items;
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public int ParentId
        {
            get { return this._parentId; }
            set { this._parentId = value; }
        }

        public int OrderNum
        {
            get { return this._orderNum; }
            set { this._orderNum = value; }
        }

        public bool Enabled
        {
            get { return this._enabled; }
            set { this._enabled = value; }
        }

        public string Caption
        {
            get { return this._caption; }
            set { this._caption = value; }
        }

        public int Icon
        {
            get { return this._icon; }
            set { this._icon = value; }
        }

        public int Colour
        {
            get { return this._colour; }
            set { this._colour = value; }
        }

        public string RequiredRight
        {
            get { return this._requiredRight; }
            set { this._requiredRight = value; }
        }

        public bool Visible
        {
            get { return this._visible; }
            set { this._visible = value; }
        }

        public bool DummyPage
        {
            get { return this._dummyPage; }
            set { this._dummyPage = value; }
        }

        public string Template
        {
            get { return this._template; }
            set { this._template = value; }
        }

        public List<string> PageStrings1
        {
            get { return this._pageStrings1; }
            set { this._pageStrings1 = value; }
        }

        public List<string> PageStrings2
        {
            get { return this._pageStrings2; }
            set { this._pageStrings2 = value; }
        }

        public Dictionary<int, CatalogItem> Items
        {
            get { return this._items; }
            set { this._items = value; }
        }
    }
}
