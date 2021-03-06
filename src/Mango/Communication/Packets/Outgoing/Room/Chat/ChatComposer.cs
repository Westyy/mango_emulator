using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mango.Players;

namespace Mango.Communication.Packets.Outgoing.Room.Chat
{
    class ChatComposer : ServerPacket
    {
        public ChatComposer(PlayerData Player, string Text, int Emotion, int Colour)
            : base(ServerPacketHeadersNew.ChatMessageComposer)
        {
            Dictionary<int, string> flk;

            base.WriteInteger(Player.Id);
            base.WriteString(WriteChatText(Text, out flk));
            base.WriteInteger(Emotion);
            base.WriteInteger(Colour);
            base.WriteInteger(flk.Count);

            foreach (KeyValuePair<int, string> kvp in flk)
            {
                string url = kvp.Value;

                if (!url.StartsWith("http://"))
                {
                    url = "http://" + url;
                }

                base.WriteString("/link.php?url=" + url + "&hash=xx");
                base.WriteString(kvp.Value);
                base.WriteBoolean(true);
            }

            base.WriteInteger(0);
        }

        private string WriteChatText(string t, out Dictionary<int, string> lk)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<int, string> links = new Dictionary<int, string>();
            string[] bits = t.Split(' ');

            int i = 0;
            int j = 0;

            foreach (string b in bits)
            {
                if (j > 0)
                {
                    sb.Append(' ');
                }

                if (b.StartsWith("http://"))
                {
                    links.Add(i, b);
                    sb.Append("{" + i++ + "}");
                }
                else if (b.StartsWith("www."))
                {
                    links.Add(i, b);
                    sb.Append("{" + i++ + "}");
                }
                else
                {
                    sb.Append(b);
                }

                j++;
            }

            lk = links;
            return sb.ToString();
        }
    }
}
