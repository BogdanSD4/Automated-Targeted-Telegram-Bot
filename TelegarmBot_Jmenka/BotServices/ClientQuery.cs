using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.AdsServises;
using TelegarmBot_Jmenka.BotServices.AdsServises.Blocker;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions;
using TelegarmBot_Jmenka.BotServices.Answers;
using TelegarmBot_Jmenka.BotServices.DataWriter;
using TelegarmBot_Jmenka.BotServices.EventManager.Calender;
using TelegarmBot_Jmenka.BotServices.EventManager.UserNotes;
using TelegarmBot_Jmenka.BotServices.GoogleServises;
using TelegarmBot_Jmenka.BotServices.MarkupService;
using TelegarmBot_Jmenka.BotServices.MessageServices;
using TelegarmBot_Jmenka.Data.User;
using TelegarmBot_Jmenka.Database;
using TelegarmBot_Jmenka.Instruments;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TelegarmBot_Jmenka.BotServices
{
    public class ClientQuery
	{
		private readonly Properties _properties;
        public ClientQuery(ITelegramBotClient telegram, Update update)
        {
            _properties = new Properties(telegram, update);
        }

        public void Query()
		{
			Console.WriteLine("Update type: " + _properties.update.Type);

			switch (_properties.update.Type)
			{
				case UpdateType.Unknown:
					break;
				case UpdateType.Message:
					{
						Console.WriteLine("Message type: " + _properties.update.Message.Type);
						MessageQuery();
					}
					break;
				case UpdateType.InlineQuery:
					break;
				case UpdateType.ChosenInlineResult:
					break;
				case UpdateType.CallbackQuery:
					{
						CallbackQuery();
					}
					break;
				case UpdateType.EditedMessage:
					break;
				case UpdateType.ChannelPost:
					break;
				case UpdateType.EditedChannelPost:
					break;
				case UpdateType.ShippingQuery:
					break;
				case UpdateType.PreCheckoutQuery:
					break;
				case UpdateType.PollAnswer:
					{
						PollAnswerQuery();
					}
					break;
				case UpdateType.MyChatMember:
					break;
				case UpdateType.ChatMember:
					break;
				case UpdateType.ChatJoinRequest:
					break;
				default:
					break;
			}	
		}

		private async void MessageQuery()
		{
			#region QuerySettings
			Message message = _properties.update.Message;
			UserInfo userInfo = UsersDataController.GetOrCreateUserInfo(message.From.Id, message.Chat.Id);
			userInfo.SessionProp = _properties;

			if (!userInfo.ShowAds) userInfo.ShowAds = true;

			MessageManager.AddMessage(userInfo.UserId, message);
			#endregion

			#region Blocker
			if (BlockerManager.CheckBlocks(userInfo))
			{
				Console.WriteLine("Blocker");
				UsersDataController.SetUserInfo(userInfo);
				return;
			}
			#endregion

			#region DataWriter
			if (await DataWriterManager.ReadDatas(userInfo))
			{
				Console.WriteLine("DataWriter");
				UsersDataController.SetUserInfo(userInfo);
				return;
			}
			#endregion

			switch (message.Type)
			{
				case MessageType.Unknown:
					break;
				case MessageType.Text:
					{
						string messageText = message.Text;
						#region IsCommand
						if (messageText[0] == '/')
						{
							switch (messageText)
							{
								case "/start":
									{
										await ClientAnswer.SendMessage(userInfo, $"Hello, {message.From.FirstName}{message.From.LastName}");

										#region User quiz
										if (userInfo.QuestionSet == null)
										{
											var questionSet = QuestionSet.standart;

											await ClientAnswer.SendMessage(userInfo, "Give answer for couple questions:");
											await ClientAnswerBuilder.SendPollSet(userInfo, questionSet);

											userInfo.QuestionSet = new QuestionSet();
											userInfo.QuestionSet.AddQuestions(questionSet.questions);
											BlockerManager.AddBlock(userInfo, new Block((user) =>
											{
												if (user.QuestionSet != null && !user.QuestionSet.HaveAllAnswers)
												{
													MessageManager.PreviousDell(user);
													return true;
												}
												else return false;
											}));

											await ClientAnswer.SendMessage(userInfo, "Fill out the forms to continue conversation");

											MessageManager.StartPreviousDell(userInfo);
										}
										#endregion
									}
									break;
								case "/events":
									{
										var inlineRequest = new string[][][]
										{
											new string[][] { new string[] { "Today", "events_today"} },
											new string[][] { new string[] { "Open calender", "events_calender" } },
										};
										var markup = MarkupBuilder.CreateMarkup<InlineKeyboardMarkup>(inlineRequest);
										await ClientAnswer.SendMessage(userInfo, "Here is my suggestions:", markup);

										MessageManager.StartPreviousClear(userInfo);
									}
									break;
								case "/schedule":
									{
										string text = 
											"To create a reminder:\r\n1. Hold down the send message button;" +
											"\r\n2. Select a scheduled message;  \r\n3. Select a date and time. " +
											"\r\nYou can view all your reminders by clicking on the calendar icon that appears.";

										await ClientAnswer.SendMessage(userInfo, text);

										MessageManager.StartPreviousClear(userInfo);
									}
									break;
								case "/assistant":
									{
										var inlineRequest = new string[][][]
										{
											new string[][] { new string[] { "Settings", "assistant_settings" } },
										};
										var markup = MarkupBuilder.CreateMarkup<InlineKeyboardMarkup>(inlineRequest);
										await ClientAnswer.SendMessage(userInfo, "I listen to you carefully..", markup);

										userInfo.UserAssistant.CreateNewChat();

										#region DataWriterException
										Func<Task> eFunc = async () => { };
										#endregion

										DataWriterManager.AddData(userInfo, new EventDataWriter(async (user) => 
										{
											#region Preconditions
											if (user.SessionProp.update is null) return false;
											Message currentMessage = user.SessionProp.update.Message;
											string result = null;
											switch (currentMessage.Type)
											{
												case MessageType.Text:
													{
														result = currentMessage.Text;
														if (result[0] == '/')
														{
                                                            return false;
														}
													}
													break;
												case MessageType.Voice: { }
													break;
												default:
													{
														await ClientAnswer.SendMessage(user, "I understand only text or audio message");
														return true;
													}
											}
											#endregion

											userInfo.UserAssistant.IsNewConversation = false;
											switch (currentMessage.Type)
											{
												case MessageType.Text:
													{

													}
													break;
												case MessageType.Voice:
													{
														#region ConvertVoiceToText
														Voice audio = currentMessage.Voice;
														MemoryStream stream = new MemoryStream();
														var file = await user.SessionProp.client.GetFileAsync(audio.FileId);
														await user.SessionProp.client.DownloadFileAsync(file.FilePath, stream);
														result = GoogleAudioService.SpeechToText(userInfo, stream);
														#endregion
													}
													break;
												default: return false;
											}

											await userInfo.UserAssistant.AppendMessage(userInfo, result);

											return true;
										}, eFunc));

										MessageManager.StartPreviousClear(userInfo);
									}
									break;
								default:
									break;
							}
						}
						#endregion

						#region IsReplyMarkup
						if (messageText == "<= back")
						{
							MessageManager.DellLastThread(userInfo);
						}
						else if (messageText == "_save")
						{
							if (userInfo.UserEvents.Confirm())
							{
								await ClientAnswer.SendMessage(userInfo, "Save succesfuly!", new ReplyKeyboardRemove());
							}
							else
							{
								await ClientAnswer.SendMessage(userInfo, "No notes, no save", new ReplyKeyboardRemove());
							}
							await ClientAnswer.SendMessage(userInfo, "Stop writing", 
								MarkupBuilder.OneCell("Add new note", $"events_today_add_{userInfo.UserEvents.currentData}"));
						}
						else if(messageText == "_stop")
						{
							userInfo.UserEvents.ClearCache();
							await ClientAnswer.SendMessage(userInfo, "Stop writing", new ReplyKeyboardRemove());
						}
						#endregion
					}
					break;
				case MessageType.Photo:
					break;
				case MessageType.Audio:
					break;
				case MessageType.Video:
					break;
				case MessageType.Voice:
					break;
				case MessageType.Document:
					break;
				case MessageType.Sticker:
					break;
				case MessageType.ChatMemberLeft:
					break;
				case MessageType.ChatTitleChanged:
					break;
				case MessageType.ChatPhotoChanged:

				case MessageType.Animation:
					break;
				default:
					break;
			}

			UsersDataController.SetUserInfo(userInfo);
		}

		private async void PollAnswerQuery() 
		{
			PollAnswer answer = _properties.update.PollAnswer;
			User user = answer.User;
			UserInfo userInfo = UsersDataController.GetUserInfo(user.Id);

			QuestionSet questionSet = userInfo.QuestionSet;
			if (questionSet is null || !questionSet.questions.Any(x => x.PollId == answer.PollId)) return;
			questionSet[answer.PollId].Answers = answer.OptionIds;

			if (userInfo.QuestionSet.HaveAllAnswers)
			{
				userInfo.SetAdsData(AdsManager.GetAdsDataSet(userInfo));
				MessageManager.Stop(user.Id);
			}

			UsersDataController.SetUserInfo(userInfo);
		}

		private async void CallbackQuery()
		{
			#region QuerySettings
			CallbackQuery query = _properties.update.CallbackQuery;
			UserInfo userInfo = UsersDataController.GetUserInfo(query.From.Id);
			userInfo.SessionProp = _properties;

			MessageManager.AddMessage(userInfo.UserId, query.Message);
			string data = query.Data;
			#endregion

			if (data.Contains("events"))
			{
				if (data.Contains("today"))
				{
					if(data.Contains("add"))
					{
						string textDate = data.AllAfterSym('_', 3);

						var markup = MarkupBuilder.CreateMarkup<ReplyKeyboardMarkup>(new string[][][]
						{
							new string[][]{new string[] { "_save" } },
							new string[][]{new string[] { "_stop" } },
						});

						await ClientAnswer.SendMessage(userInfo, "I ready to write...", markup);

						#region DataWriterException
						Func<Task<Message>> eFunc = async () =>
						{
							var markup = MarkupBuilder.CreateMarkup<InlineKeyboardMarkup>(new string[][][]
							{
									new string[][]{
										new string[] {"Yes", "events_today_confirm"},
										new string[] {"No", "events_today_clear"},
									}
							});
							return await ClientAnswer.SendMessage(userInfo, "Write down this notes?", markup);
						};
						#endregion

						DataWriterManager.AddData(userInfo, new EventDataWriter(async (user) =>
						{
							Update update = user.SessionProp.update;
							Message? message = update.Message;

							if (message is not null)
							{
								if (userInfo.UserEvents is null)
								{
									await ClientAnswer.SendMessage(userInfo, "Something went wrong... Try again");
									return true;
								}
								else
								{
									switch (message.Type)
									{
										case MessageType.Text:
											{
												if (message.Text[0] == '/')
												{
													return false;
												}
												else if (message.Text == "_save" || message.Text == "_stop") return false;

												user.UserEvents.AddNote(new Note(message.Type, (null, new List<string> { message.Text.TextEncoder() })));
												return true;
											}
										case MessageType.Photo:
											{
												string id = message.MediaGroupId;

												user.UserEvents.AddNote(new Note(message.Type, (id, new List<string> { message.Photo[0].FileId })));
												return true;
											}
										case MessageType.Audio:
											return true;
										case MessageType.Video:
											{
												string id = message.MediaGroupId;

												user.UserEvents.AddNote(new Note(message.Type, (id, new List<string> { message.Video.FileId })));
												return true;
											}
										case MessageType.Voice:
											return true;
										case MessageType.Document:
											return true;
										case MessageType.Sticker:
											return true;
										default:
											await eFunc();
											return false;
									}
								}
							}
							else return false;
						}, eFunc));
					}
					else if (data.Contains("confirm"))
					{
						userInfo.UserEvents.Confirm();
						await ClientAnswer.SendMessage(userInfo, "Save succesfuly!", new ReplyKeyboardRemove());
					}
					else if (data.Contains("clear"))
					{
						userInfo.UserEvents.ClearCache();
						await ClientAnswer.SendMessage(userInfo, "Note is clear", new ReplyKeyboardRemove());
					}
					else if (data.Contains("remove"))
					{
						if (data.Contains("all"))
						{
							string currentData = data.AllAfterSym('_', 3);

							var containers = userInfo.UserEvents.RemoveContainerByDate(currentData);
							if (containers is null) return;

							foreach (var item in containers)
							{
								await ClientAnswerBuilder.DeleteMessages(userInfo, item.LastMessagesId);
							}

							await ClientAnswerBuilder.DeleteMessages(userInfo, userInfo.UserEvents.MainMessageId);
						}
						else
						{
							string currentData = data.AllAfterSymWithStop('_', 2, '_');
							string currentTime = data.AllAfterSym('_', 3);

							var container = userInfo.UserEvents.RemoveContainerByTime(currentData, currentTime);
							if (container is null) return;

							await ClientAnswerBuilder.DeleteMessages(userInfo, container.LastMessagesId);

							if (userInfo.UserEvents.Notes[currentData].Count < 1)
							{
								userInfo.UserEvents.RemoveContainerByDate(currentData);
								await ClientAnswerBuilder.DeleteMessages(userInfo, userInfo.UserEvents.MainMessageId);
							}
						}
						
					}
					else
					{
						await MessageManager.AddThreat(userInfo, async () => 
						{
							userInfo.UserEvents.currentData = DateTime.Now;
							await EventsCalender.GetDayNotes(userInfo);
						});
					}
				}
				else if (data.Contains("calender"))
				{
					if (data.Contains("left") || data.Contains("right"))
					{
						var gg = data.AllAfterSymWithStop('_', 2, ' ');
						DateTime dateTime = DateTime.Parse(gg);
						var markup = EventsCalender.GetCalender(dateTime) as InlineKeyboardMarkup;
						await _properties.client.EditMessageReplyMarkupAsync(userInfo.ChatId, query.Message.MessageId, replyMarkup: markup);
					}
					else if (data.Contains("day"))
					{
						string date = data.AllAfterSym('_', 2);

						await MessageManager.AddThreat(userInfo, async () =>
						{
							userInfo.UserEvents.currentData = DateTime.Parse(date);
							await EventsCalender.GetDayNotes(userInfo);
							UsersDataController.SetUserInfo(userInfo);
						});
					}
					else if (data.Contains("showAll"))
					{
						await MessageManager.AddThreat(userInfo, async () =>
						{
							await EventsCalender.GetAllNotes(userInfo);
						});
					}
					else
					{
						await MessageManager.AddThreat(userInfo, async () => 
						{
							var markup = EventsCalender.GetCalender();
							await ClientAnswer.SendMessage(userInfo, "Choose date:", markup);
						});
					}
				}
			}
			else if (data.Contains("assistant"))
			{
				if (data.Contains("settings"))
				{
					bool isLeft = false;
					if ((isLeft = data.Contains("<")) || data.Contains(">"))
					{
						if (data.Contains("language"))
						{
							string[] lang = new string[] { "uk-UA", "en-US", "fr-FR", "es-ES", "de-DE", "ja-JP" };
							int currentIndex = lang.GetIndex(userInfo.UserAssistant.LangCode);

							if (isLeft)
							{
								if (--currentIndex < 0) currentIndex = lang.Length - 1;
							}
							else
							{
								if (++currentIndex >= lang.Length) currentIndex = 0;
							}

							userInfo.UserAssistant.LangCode = lang[currentIndex];

							await userInfo.SessionProp.client.EditMessageTextAsync(userInfo.ChatId, query.Message.MessageId,
								$"Language: {userInfo.UserAssistant.LangCode}", replyMarkup: (InlineKeyboardMarkup?)CreateArrows("language"));
						}
						else if (data.Contains("realtime"))
						{
							userInfo.UserAssistant.RealtimeAnswer = !userInfo.UserAssistant.RealtimeAnswer;

							await userInfo.SessionProp.client.EditMessageTextAsync(userInfo.ChatId, query.Message.MessageId,
								$"Realtime mode: {(userInfo.UserAssistant.RealtimeAnswer ? "On" : "Off")}", replyMarkup: (InlineKeyboardMarkup?)CreateArrows("realtime"));
						}
						else if (data.Contains("temperature"))
						{
							int current = userInfo.UserAssistant.Temperature;

							if (isLeft)
							{
								if (--current < 0) current = 10;
							}
							else
							{
								if (++current > 10) current = 0;
							}

							userInfo.UserAssistant.Temperature = current;

							await userInfo.SessionProp.client.EditMessageTextAsync(userInfo.ChatId, query.Message.MessageId,
								$"Temperature: {userInfo.UserAssistant.Temperature}", replyMarkup: (InlineKeyboardMarkup?)CreateArrows("temperature"));
						}
					}
					else if (data.Contains("save"))
					{
						var markup = MarkupBuilder.OneCell("Saved", "assistant_settings_saved");

						await userInfo.SessionProp.client.EditMessageReplyMarkupAsync(query.InlineMessageId, markup);

						userInfo.UserAssistant.CreateNewChat();
					}
					else
					{
						if (userInfo.UserAssistant.IsNewConversation)
						{
							userInfo.UserAssistant.IsNewConversation = false;

							await ClientAnswer.SendMessage(userInfo, $"Language: {userInfo.UserAssistant.LangCode}", CreateArrows("language"));
							await ClientAnswer.SendMessage(userInfo, $"Realtime mode: {(userInfo.UserAssistant.RealtimeAnswer ? "On" : "Off")}", CreateArrows("realtime"));
							await ClientAnswer.SendMessage(userInfo, $"Temperature: {userInfo.UserAssistant.Temperature}", CreateArrows("temperature"));

							var markup = MarkupBuilder.OneCell("Save", "assistant_settings_save");

							await userInfo.SessionProp.client.EditMessageReplyMarkupAsync(userInfo.ChatId, query.Message.MessageId, markup);
						}
					}
					IReplyMarkup? CreateArrows(string typeName)
					{
						string leftArrow = "assistant_settings_<_";
						string rightArrow = "assistant_settings_>_";

						return MarkupBuilder.CreateMarkup<InlineKeyboardMarkup>(new string[][][]
						{
										new string[][]
										{
											new string[] { "<", $"{leftArrow}{typeName}" },
											new string[] { ">", $"{rightArrow}{typeName}" },
										},
						});
					}
				}
			}
			UsersDataController.SetUserInfo(userInfo);
		}
	}
}
