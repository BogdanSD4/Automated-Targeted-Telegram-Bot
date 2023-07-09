using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.Answers;
using TelegarmBot_Jmenka.BotServices.MarkupService;
using TelegarmBot_Jmenka.Data.User;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegarmBot_Jmenka.BotServices.MessageServices
{
	public static class MessageManager
	{
        public static IDictionary<long, MessageThread?> messages { get; set; } = new Dictionary<long, MessageThread?>();

		public static void StartPreviousDell(UserInfo userInfo)
		{
			#region Preconditions
			if (userInfo.UserId == default) return;
			#endregion

			StartNew(userInfo, (thread) =>
			{
				PreviousDell(userInfo, thread);
			});
		}
		public static void StartPreviousClear(UserInfo userInfo)
		{
			#region Preconditions
			if (userInfo.UserId == default) return;
			#endregion

			StartNew(userInfo, (thread) =>
			{
				thread.ClearMessage();
			});
		}
		private static void StartNew(UserInfo userInfo, Action<MessageThread> action)
		{
			long userId = userInfo.UserId;

			if (messages.ContainsKey(userId))
			{
				MessageThread thread = messages[userId];
				action(thread);
				thread.IsActive = true;
				thread.LastReplyMarkup = null;
			}
			else
			{
				MessageThread messageThread = new MessageThread() { IsActive = true};
				messages.Add(userId, messageThread);
			}
		}

		public static async Task AddThreat(UserInfo userInfo, Func<Task>? callback = null)
		{
			long userId = userInfo.UserId;

			if (messages.ContainsKey(userId))
			{
				MessageThread main = messages[userId];
				main.AddNext(new MessageThread() { IsActive = true });
                Console.WriteLine("AddNext");
                if (main.LastReplyMarkup is null)
				{
					main.LastReplyMarkup = await ClientAnswerBuilder.AllowMarkup(userInfo, MarkupBuilder.StandartBackButton);
				}
				if(callback != null) await callback();
			}
			return;
		}
		public static async Task AddThreat(UserInfo userInfo, Action? callback = null, bool IsMainThread = false)
		{
			long userId = userInfo.UserId;

			if (messages.ContainsKey(userId))
			{
				MessageThread main = messages[userId];
				main.AddNext(new MessageThread() { IsActive = true });

				if (main.LastReplyMarkup == null || IsMainThread)
				{
					main.LastReplyMarkup = await ClientAnswerBuilder.AllowMarkup(userInfo, MarkupBuilder.StandartBackButton);
				}
				if (callback != null)  await Task.Run(callback);
			}
			return;
		}
		public static async Task DellLastThread(UserInfo userInfo)
		{
			if (messages.ContainsKey(userInfo.UserId))
			{
				MessageThread main = messages[userInfo.UserId];
				MessageThread? thread = main.GetLastParent();
				
				await ClientAnswerBuilder.DeleteMessages(userInfo, thread.next.messages);
				thread.next = null;

				if (main.next is null)
				{
					main.LastReplyMarkup = null;
				}
			}
		}

		public static void AddMessage(long userId, Message message)
		{
			#region Preconditions
			if (userId == default) return;
			#endregion

			if (messages.ContainsKey(userId))
			{
				MessageThread thread = messages[userId];
                thread.AddMessage(message);
			}
		}
		public static void AddMessages(long userId, Message[] messages)
		{
			#region Preconditions
			if (userId == default) return;
			#endregion

			foreach (var item in messages)
			{
				AddMessage(userId, item);
			}
		}
		public static void AddRangeMessage(long userId, IEnumerable<Message> message)
		{
			#region Preconditions
			if (userId == default) return;
			#endregion

			MessageThread thread = messages[userId].GetLast();
			foreach (var item in message)
			{
				thread.AddMessage(item);
			}
		}

		public static void Stop(long userId)
		{
			#region Preconditions
			if (userId == default) return;
			#endregion

			messages[userId].IsActive = false;
		}
		public static void StopPreviousDell(UserInfo userInfo)
		{
			#region Preconditions
			if (userInfo.UserId == default) return;
			#endregion

			MessageThread thread = messages[userInfo.UserId];
			PreviousDell(userInfo);
			thread.IsActive = false;
		}

		public static void PreviousDell(UserInfo userInfo)
		{
			if (messages.ContainsKey(userInfo.UserId))
			{
				MessageThread thread = messages[userInfo.UserId];
				thread.DellMessages(userInfo);
			}
		}
		public static void PreviousDell(UserInfo userInfo, MessageThread thread)
		{
			thread.DellMessages(userInfo);
		}
		public static void LastDell(UserInfo userInfo)
		{
			if (messages.ContainsKey(userInfo.UserId))
			{
				MessageThread thread = messages[userInfo.UserId].GetLast();
				thread.DellLastMessage(userInfo);
			}
		}

		public static void DellOutOfManager(UserInfo userInfo, int messageId)
		{
			MessageThread thread = messages[userInfo.UserId];
			Action callback = null;

			while (true)
			{
				foreach (var item in thread.messages)
				{
					if(item.MessageId == messageId)
					{
						callback = () => 
						{
							thread.messages.Remove(item);
						};
					}
				}
				break;
			}

			if(callback != null)
			{
				callback.Invoke();
			}
		}
	}
}
