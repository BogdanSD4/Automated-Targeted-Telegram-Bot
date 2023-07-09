using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.Data.User;
using Telegram.Bot;

namespace TelegarmBot_Jmenka.BotServices.Loading
{
	public class Loading
	{
		public static async Task LoadingMessage(UserInfo userInfo, int messageId, LoadingUIBase loading)
		{
			while (true)
			{
				await userInfo.SessionProp.client.EditMessageTextAsync(userInfo.ChatId, messageId, loading.Invoke());
				Thread.Sleep(1000);
			}
		}
		public static async Task LoadingMessage(UserInfo userInfo, int messageId, LoadingUIBase loading, int delay)
		{
			while (true)
			{
				await userInfo.SessionProp.client.EditMessageTextAsync(userInfo.ChatId, messageId, loading.Invoke());
				Thread.Sleep(delay);
			}
		}
	}
}
