using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mango.Moderation
{
    class ModerationPresetActionMessages
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Caption { get; set; }
        public string MessageText { get; set; }

        public ModerationPresetActionMessages(int Id, int ParentId, string Caption, string MessageText)
        {
            this.Id = Id;
            this.ParentId = ParentId;
            this.Caption = Caption;
            this.MessageText = MessageText;
        }
    }
}
