using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.Answers;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using TelegarmBot_Jmenka.Instruments;

namespace TelegarmBot_Jmenka.BotServices.EventManager.UserNotes
{
    public class NoteContainer
    {
        public NoteContainer()
        {

        }
        public NoteContainer(List<Note> noteCache, string time)
        {
            Data = new List<Note>();
            Data.AddRange(noteCache);
            Time = time;
            LastMessagesId = new List<int>();
        }

        public string Time { get; set; }
        public List<Note> Data { get; set; }
        public List<int> LastMessagesId { get; set; }
    }


}
