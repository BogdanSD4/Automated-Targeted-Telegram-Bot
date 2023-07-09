using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.EventManager.UserNotes;
using TelegarmBot_Jmenka.Instruments;

namespace TelegarmBot_Jmenka.BotServices.EventManager
{
    public class UserEvents
    {
        public UserEvents()
        {
            currentData = DateTime.Now.Date;
            Notes = new Dictionary<string, List<NoteContainer>>();
            cacheNote = new List<Note>();
            MainMessageId = new List<int>();
        }

        private DateTime date;
        public DateTime currentData
        {
            get
            {
                return date;
            }
            set
            {
                date = value.Date;
                ClearCache();
            }
        }
        public IDictionary<string, List<NoteContainer>> Notes { get; set; }
        public List<Note> cacheNote { get; set; }
        public List<int> MainMessageId { get; set; }

        [JsonIgnore]
        public List<NoteContainer>? this[DateTime time]
        {
            get
            {
                foreach (var e in Notes)
                {
                    if (e.Key == time.Date.ToString()) return e.Value;
                }
                return null;
            }
        }
        [JsonIgnore]
        public List<NoteContainer>? this[string dateTime]
        {
            get
            {
                foreach (var e in Notes)
                {
                    if (e.Key == dateTime) return e.Value;
                }
                return null;
            }
        }
        [JsonIgnore]
        public List<NoteContainer>? currentEvent
        {
            get
            {
                foreach (var e in Notes)
                {
                    if (e.Key == date.ToString()) return e.Value;
                }
                return null;
            }
        }

        public void AddNote(Note note)
        {
            Note current = default;
            if (note.Data.Item1 is not null && cacheNote.Any(x =>
                {
                    if (x.Data.Item1 == note.Data.Item1)
                    {
                        current = x;
                        return true;
                    }
                    return false;
                }))
            {
                current.Data.Item2.AddRange(note.Data.Item2);
            }
            else
            {
                cacheNote.Add(note);
            }
        }

        public List<NoteContainer>? RemoveContainerByDate(string dateTime)
        {
            if (Notes.ContainsKey(dateTime))
            {
                var result = Notes[dateTime];

                Notes.Remove(dateTime);

                return result;
            }

            return null;
        }
        public NoteContainer? RemoveContainerByTime(string dateTime, string time)
        {
            if (Notes.ContainsKey(dateTime))
            {
                var container = Notes[dateTime].First(x => x.Time == time);

                Notes[dateTime].Remove(container);

                return container;
            }

            return null;
        }

        public bool Confirm()
        {
            #region Precondition
            if (cacheNote is null || cacheNote.Count == 0)
            {
                return false;
            }
            #endregion
            string dateTime = date.ToString();
            string time = DateTime.Now.AllAfterSym(' ');

            if (Notes.ContainsKey(dateTime))
            {
                Notes[dateTime].Add(new NoteContainer(cacheNote, time));
            }
            else
            {
                Notes.Add(dateTime, new List<NoteContainer> { new NoteContainer(cacheNote, time) });
            }
            ClearCache();
            return true;
        }

        public void ClearCache()
        {
            if (cacheNote is null)
            {
                cacheNote = new List<Note>();
            }
            else
            {
                cacheNote.Clear();
            }
        }
    }
}
