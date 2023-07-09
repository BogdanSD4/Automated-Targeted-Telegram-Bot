using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace TelegarmBot_Jmenka.BotServices.EventManager.UserNotes
{
    public class Note
    {
        public Note() { }
        public Note(MessageType type, (string?, List<string>) bytes)
        {
            Type = type;
            Data = bytes;
        }
        public MessageType Type { get; set; }
        public (string?, List<string>) Data { get; set; }
    }
}
