using Mango.Database.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Players.Wardrobe
{
    class WardrobeItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SlotId { get; set; }
        public string Figure { get; set; }
        public PlayerGender Gender { get; set; }

        public WardrobeItem(int Id, int UserId, int SlotId, string Figure, string Gender)
        {
            this.SlotId = SlotId;
            this.Figure = Figure;

            if (Gender.ToLower() != "m" && Gender.ToLower() != "f")
                throw new DatabaseException("Unable to determine player gender for wardrobe figure.");

            switch (Gender.ToLower())
            {
                case "m":
                    this.Gender = PlayerGender.MALE;
                    break;

                case "f":
                    this.Gender = PlayerGender.FEMALE;
                    break;
            }
        }
    }
}
