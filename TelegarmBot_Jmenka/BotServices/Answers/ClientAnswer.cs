using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions;
using TelegarmBot_Jmenka.BotServices.MessageServices;
using TelegarmBot_Jmenka.Data.User;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegarmBot_Jmenka.BotServices.Answers
{
    public class ClientAnswer : ClientAnswerBase
    {
        /// <summary>
        /// if method exist, return <color=green>Message</color>, else return Exception message
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replymarkup"></param>
        /// <returns></returns>
        public static async Task<Message> SendMessage(UserInfo userInfo, string text, IReplyMarkup? replymarkup = null, bool addToManager = true)
        {
            #region Precondition
            if (text == null)
            {
                throw new Exception("text can not be null");
            }
            #endregion
            Message message = await Startup.telegramBot
                .SendTextMessageAsync(userInfo.ChatId, text, replyMarkup: replymarkup);

			if(addToManager) SaveMessage(userInfo, message);

			return message;
        }
        public static async Task<Message> SendPoll(UserInfo userInfo, Question question)
        {
            Message message = await Startup.telegramBot
				.SendPollAsync(userInfo.ChatId, question.Title, question.QuestionList, 
                isAnonymous: false, 
                protectContent: true, 
                allowsMultipleAnswers: question.IsMultipleAnswer);

            question.PollId = message.Poll.Id;

			SaveMessage(userInfo, message);

			return message;
        }
        public static async Task<Message> SendPhoto(UserInfo userInfo, InputFile inputFile)
        {
            Message message = await Startup.telegramBot.SendPhotoAsync(userInfo.ChatId, inputFile);
            SaveMessage(userInfo, message);

            return message;
		}
		public static async Task<Message[]> SendDataSet(UserInfo userInfo, IEnumerable<InputFile> inputFile)
        {
            List<IAlbumInputMedia> albumInputs = new List<IAlbumInputMedia>();

            foreach (var item in inputFile)
            {
				InputMediaPhoto media = new InputMediaPhoto(item);
				albumInputs.Add(media);
			}
			
            Message[] messages = await Startup.telegramBot.SendMediaGroupAsync(userInfo.ChatId, albumInputs);
            SaveMessage(userInfo, messages);

            return messages;
        }
		public static async Task<Message> SendVideo(UserInfo userInfo, InputFile inputFile)
		{
			Message message = await Startup.telegramBot.SendVideoAsync(userInfo.ChatId, inputFile);

			SaveMessage(userInfo, message);

			return message;
		}

		public static async Task DeleteMessage(UserInfo userInfo, int messageId)
        {
            try
            {
				await Startup.telegramBot
				.DeleteMessageAsync(userInfo.ChatId, messageId);
			}
            catch (Exception) { }
			
		}
		public static async Task DeleteMessage(UserInfo userInfo, int messageId, bool dellInManager)
		{
            if (dellInManager)
            {
                MessageManager.DellOutOfManager(userInfo, messageId);
            }
			await Startup.telegramBot
				.DeleteMessageAsync(userInfo.ChatId, messageId);
		}
	}
}
