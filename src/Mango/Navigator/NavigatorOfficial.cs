using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mango.Rooms;
using Mango.Database.Exceptions;

namespace Mango.Navigator
{
    class NavigatorOfficial
    {
        public int Id { get; set; }
        public bool Enabled { get; set; }
        public int ParentId { get; set; }
        public int RoomId { get; set; }
        public bool Category { get; set; }
        public NavigatorOfficialDisplayType DisplayType { get; set; }
        public NavigatorOfficialImageType ImageType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string BannerLabel { get; set; }
        public bool CategoryAutoExpand { get; set; }

        public NavigatorOfficial(int Id, int Enabled, int ParentId, int RoomId, int Category, string DisplayType, string ImageType,
            string Name, string Description, string Image, string BannerLabel, int AutoExpand)
        {
            this.Id = Id;
            this.Enabled = Enabled == 1 ? true : false;
            this.ParentId = ParentId;
            this.RoomId = RoomId;
            this.Category = Category == 1 ? true : false;

            if (DisplayType != "banner" && DisplayType != "details")
                throw new DatabaseException(string.Format("Expected data to be 'banner' or 'details' but was '{0}'.", DisplayType));

            switch (DisplayType)
            {
                case "banner":
                    this.DisplayType = NavigatorOfficialDisplayType.BANNER;
                    break;

                case "details":
                    this.DisplayType = NavigatorOfficialDisplayType.DETAILED;
                    break;
            }

            if (ImageType != "internal" && ImageType != "external")
                throw new DatabaseException(string.Format("Expected data to be 'internal' or 'external' but was '{0}'.", ImageType));

            switch (ImageType)
            {
                case "internal":
                    this.ImageType = NavigatorOfficialImageType.INTERNAL;
                    break;

                case "external":
                    this.ImageType = NavigatorOfficialImageType.EXTERNAL;
                    break;
            }

            this.Name = Name;
            this.Description = Description;
            this.Image = Image;
            this.BannerLabel = BannerLabel;
            this.CategoryAutoExpand = AutoExpand == 1 ? true : false;
        }

        public virtual bool TryGetRoomData(out RoomData data)
        {
            return RoomLoader.TryGetData(RoomId, out data);
        }
    }
}
