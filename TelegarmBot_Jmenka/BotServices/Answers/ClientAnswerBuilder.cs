using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Ads;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions;
using TelegarmBot_Jmenka.Data.User;
using TelegarmBot_Jmenka.Instruments;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegarmBot_Jmenka.BotServices.Answers
{
	public class ClientAnswerBuilder
	{
		public static async Task<Message[]> SendPollSet(UserInfo userInfo, QuestionSet questionSet)
		{
			#region Precondition
			if (questionSet == null || questionSet.questions.Count() == 0) throw new Exception("Question set can not be null");
			#endregion

			List<Message> result = new List<Message>();

			foreach (var item in questionSet.questions)
			{
				result.Add(await ClientAnswer.SendPoll(userInfo, item));
			}

			return result.ToArray();
		}
		public static async Task<Message[]> SendAds(UserInfo userInfo, AdsDataSet adsData)
		{
			List<Message> messages = new List<Message>();

			switch (adsData.AccountType)
			{
				case AdsAccountType.Lite:
					{
						messages.Add(await ClientAnswer.SendMessage(userInfo, adsData.Url.GetRandom()));
					}
					break;
				case AdsAccountType.Standart:
					{
						messages.Add(await ClientAnswer.SendMessage(userInfo, adsData.Url.GetRandom()));
						using (Stream stream = System.IO.File.OpenRead(adsData.Foto.GetRandom()))
						{
							messages.Add(await ClientAnswer.SendPhoto(userInfo, new InputFileStream(stream)));
						}
					}
					break;
				case AdsAccountType.Premium:
					{
						messages.Add(await ClientAnswer.SendMessage(userInfo, adsData.Url.GetRandom()));
						using (Stream stream = System.IO.File.OpenRead(adsData.Foto.GetRandom()))
						{
							messages.Add(await ClientAnswer.SendPhoto(userInfo, new InputFileStream(stream)));
						}
						using (Stream stream = System.IO.File.OpenRead(adsData.Video.GetRandom()))
						{
							messages.Add(await ClientAnswer.SendVideo(userInfo, new InputFileStream(stream)));
						}
					}
					break;
				default:
					break;
			}

			return messages.ToArray();
		}

		public static async Task<Message> AllowMarkup(UserInfo userInfo, ReplyKeyboardMarkup markup) 
		{
			Message message = await ClientAnswer.SendMessage(userInfo, "Enable menu", markup, true);
			
			return message;
		}
		public static async Task DisableMarkup(UserInfo userInfo)
		{
			Message message = await ClientAnswer.SendMessage(userInfo, "Disable menu", new ReplyKeyboardRemove(), false);
			await ClientAnswer.DeleteMessage(userInfo, message.MessageId);
		}

		public static async Task DeleteMessages(UserInfo userInfo, IEnumerable<Message> messages)
		{
			#region Preconditions
			if (messages == null) return;
			#endregion
			var delllist = messages.Select(x => x.MessageId).ToArray();

			for (int i = 0; i < delllist.Length; i++)
			{
				await ClientAnswer.DeleteMessage(userInfo, delllist[i]);
			}
		}
		public static async Task DeleteMessages(UserInfo userInfo, IEnumerable<int> messagesId)
		{
			foreach (var message in messagesId)
			{
				await ClientAnswer.DeleteMessage(userInfo, message);
			}
		}
	}
}
