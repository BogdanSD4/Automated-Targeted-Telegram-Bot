using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.MessageServices;
using TelegarmBot_Jmenka.Data.User;
using Telegram.Bot.Types;

namespace TelegarmBot_Jmenka.BotServices.Answers
{
	public class ClientAnswerBase
	{
		protected static void SaveMessage(UserInfo userInfo, Message message)
		{
			MessageManager.AddMessage(userInfo.UserId, message);
		}
		protected static void SaveMessage(UserInfo userInfo, Message[] messages)
		{
			MessageManager.AddMessages(userInfo.UserId, messages);
		}
	}
}
