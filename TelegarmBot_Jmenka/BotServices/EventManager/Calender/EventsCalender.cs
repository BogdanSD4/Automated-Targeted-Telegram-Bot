using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.Answers;
using TelegarmBot_Jmenka.BotServices.EventManager.UserNotes;
using TelegarmBot_Jmenka.BotServices.MarkupService;
using TelegarmBot_Jmenka.Data.User;
using TelegarmBot_Jmenka.Instruments;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TelegarmBot_Jmenka.BotServices.EventManager.Calender
{
    public static class EventsCalender
    {
        private static readonly string[] monthNames = new string[] {
            "January", "February", "March", "April", "May", "June", "July",
            "August", "September", "October", "November", "December"
        };
        public static IReplyMarkup GetCalender()
        {
            DateTime date = DateTime.Now;
            date = new DateTime(date.Year, date.Month, 1);

            return BaseCalender(date);
        }
        public static IReplyMarkup GetCalender(DateTime date)
        {
            return BaseCalender(date);
        }
        public static IReplyMarkup BaseCalender(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;

            int dayOfWeek = (date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek) - 1;
            int dayInMonth = DateTime.DaysInMonth(date.Year, date.Month);

            List<List<string[]>> markup = new List<List<string[]>>();

            markup.Add(new List<string[]> { new string[] { year.ToString() } });
            markup.Add(new List<string[]> { new string[] { monthNames[month - 1] } });
            markup.Add(new List<string[]>
            {
                new string[] { "Mon" },
                new string[] { "Tue" },
                new string[] { "Wed" },
                new string[] { "Thu" },
                new string[] { "Fri" },
                new string[] { "Sat" },
                new string[] { "Sun" },
            });

            for (int i = 0; i < 6; i++)
            {
                List<string[]> week = new List<string[]>();
                for (int j = 0; j < 7; j++)
                {
                    string text = "";
                    string query = "";

                    if (i == 0)
                    {
                        text = j >= dayOfWeek ? day.ToString() : " ";

                        week.Add(GetDayValue());

                        continue;
                    }

                    text = day <= dayInMonth ? day.ToString() : " ";

                    week.Add(GetDayValue());

                    string[] GetDayValue()
                    {
                        if (text == " ")
                        {
                            query = text;
                        }
                        else
                        {
                            DateTime time = new DateTime(year, month, day);
                            query = $"events_calender_day_{time.Date}";
                            day++;
                        }

                        return new string[] { text.ToString(), query };
                    }
                }
                markup.Add(week);
            }

            markup.Add(new List<string[]>
            {
                new string[]{ "<", $"events_calender_left_{date.AddMonths(-1)}" },
                new string[]{ "Show all events", "events_calender_showAll" },
                new string[]{ ">", $"events_calender_right_{date.AddMonths(1)}" },
            });

            return MarkupBuilder.CreateMarkup<InlineKeyboardMarkup>(markup);
        }

        public static async Task GetDayNotes(UserInfo userInfo)
        {
            List<NoteContainer> points = userInfo.UserEvents.currentEvent;
            DateTime date = userInfo.UserEvents.currentData;
            string currentDate = date.AllToSym(' ');

            if (points == null || points.Count == 0)
            {
                var markup = MarkupBuilder.CreateMarkup<InlineKeyboardMarkup>(new string[][][]
                {
                    new string[][]{new string[] {"Add", $"events_today_add_{date}" } }
                });

                await ClientAnswer.SendMessage(userInfo, "No notes", markup);
            }
            else
            {
                Message header = await ClientAnswer.SendMessage(userInfo, $"--- Date: {currentDate} ---");
                userInfo.UserEvents.MainMessageId.Add(header.MessageId);

                foreach (var item in points)
                {
                    Message firstMessage = await ClientAnswer.SendMessage(userInfo, $"--- Time: {item.Time} ---");
                    item.LastMessagesId.Add(firstMessage.MessageId);

                    int length = item.Data.Count;
                    for (int i = 0; i < length; i++)
                    {
                        Note note = item.Data[i];
                        Message? message = null;

                        if (note.Data.Item1 is not null)
                        {
                            var list = note.Data.Item2.Select(x => new InputFileId(x));

                            var messages = await ClientAnswer.SendDataSet(userInfo, list);

                            item.LastMessagesId.AddRange(messages.Select(x => x.MessageId));
                        }
                        else
                        {
                            string data = note.Data.Item2[0];


                            switch (note.Type)
                            {
                                case MessageType.Text:
                                    message = await ClientAnswer.SendMessage(userInfo, data.TextDecoder());
                                    break;
                                case MessageType.Photo:
                                    message = await ClientAnswer.SendPhoto(userInfo, new InputFileId(data));
                                    break;
                                case MessageType.Audio:
                                    break;
                                case MessageType.Video:
                                    message = await ClientAnswer.SendVideo(userInfo, new InputFileId(data));
                                    break;
                                case MessageType.Voice:
                                    break;
                                case MessageType.Document:
                                    break;
                                case MessageType.Sticker:
                                    break;
                                default:
                                    break;
                            }

                            item.LastMessagesId.Add(message.MessageId);
                        }
                    }

                    var markup = MarkupBuilder.CreateMarkup<InlineKeyboardMarkup>(new string[][][]
                    {
                        new string[][] { new string[] { "Remove", $"events_today_remove_{date}_{item.Time}"} }
                    });
                    Message lastMessage = await ClientAnswer.SendMessage(userInfo, $"Remove this?", markup);
                    item.LastMessagesId.Add(lastMessage.MessageId);
                }

                Message footer = await ClientAnswer.SendMessage(userInfo, $"--- Note end ---",
                    MarkupBuilder.StandartAddOrRemove("Add", "Remove all", $"events_today_add_{date}", $"events_today_remove_all_{date}"));
                userInfo.UserEvents.MainMessageId.Add(footer.MessageId);
            }
        }

        public static async Task GetAllNotes(UserInfo userInfo)
        {
            #region Precontition
            if (userInfo.UserEvents.Notes.Count == 0)
            {
                await ClientAnswer.SendMessage(userInfo, "You don't have notes");
                return;
            }
            #endregion

            List<IEnumerable<IEnumerable<string>>> notes = new List<IEnumerable<IEnumerable<string>>>();

            var containers = userInfo.UserEvents.Notes.Keys;
            foreach (var item in containers)
            {
                if (item is not null)
                {
                    notes.Add(new string[][] { new string[] { item.AllToSym(' '), $"events_calender_day_{item}" } });
                }
            }

            var markup = MarkupBuilder.CreateMarkup<InlineKeyboardMarkup>(notes);

            await ClientAnswer.SendMessage(userInfo, "All notes:", markup);
        }
    }
}
