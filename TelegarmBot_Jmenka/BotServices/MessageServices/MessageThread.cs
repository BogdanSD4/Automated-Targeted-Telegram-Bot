using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.Answers;
using TelegarmBot_Jmenka.Data.User;
using Telegram.Bot.Types;

namespace TelegarmBot_Jmenka.BotServices.MessageServices
{
	public class MessageThread
	{
        public MessageThread()
        {
			messages = new List<Message>();
		}
        public MessageThread(Message message) 
		{
			messages = new List<Message>();
			messages.Add(message);
		}
        public MessageThread(IEnumerable<Message> message)
        {
			messages = new List<Message>();
			messages.AddRange(message);
		}

        private bool isActiv;
		public bool IsActive { get { return isActiv; }
			set 
			{
				isActiv = value;
				if (!value)
				{
					messages = new List<Message>();
				}
			}
		}
		public Message? LastReplyMarkup { get; set; }
		public List<Message> messages { get; private set; }
		public MessageThread? next { get; set; }

		public async void DellMessages(UserInfo userInfo)
		{
			await ClientAnswerBuilder.DeleteMessages(userInfo, messages);
			ClearMessage();
		}
		public async void DellLastMessage(UserInfo userInfo)
		{
			if (messages is null || messages.Count < 1) return;

			int length = messages.Count - 1;

			Message message = messages[length];
			messages.RemoveAt(length);
			await ClientAnswer.DeleteMessage(userInfo, message.MessageId);
		}
		public void ClearMessage()
		{
			messages = new List<Message>();
			next = null;
		}

		public void AddMessage(Message message)
		{
			if (IsActive)
			{
				if(next is not null)
				{
					next.AddMessage(message);
				}
				else { messages.Add(message); }
			}
		}	
		public void AddNext(MessageThread thread)
		{
			if (next is not null)
			{
				next.AddNext(thread);
			}
			else { next = thread; }
		}

		public MessageThread GetLast(int num = 0)
		{
            Console.WriteLine(num++);
            if (next is not null)
			{
				return next.GetLast(num);
			}
			else { return this; }
		}
		public MessageThread? GetLastParent()
		{
			if (next is not null)
			{
				if(next.next is not null)
				{
					return next.GetLastParent();
				}
				else
				{
					return this;
				}
			}
			else { return this; }
		}
	}
}
