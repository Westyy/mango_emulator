﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Moderation
{
    class ModerationPresetActionCategories
    {
        public int Id { get; set; }
        public string Caption { get; set; }

        public ModerationPresetActionCategories(int Id, string Caption)
        {
            this.Id = Id;
            this.Caption = Caption;
        }
    }
}
