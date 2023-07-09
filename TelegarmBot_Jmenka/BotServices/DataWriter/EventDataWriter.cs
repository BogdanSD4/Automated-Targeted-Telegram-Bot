using OpenAI_API.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.Data.User;

namespace TelegarmBot_Jmenka.BotServices.DataWriter
{
	public partial class EventDataWriter : DataWriter
	{
        public EventDataWriter(Func<UserInfo, Task<bool>> func) : base(null)
        {
			function = func;
        }
        public EventDataWriter(Func<UserInfo, Task<bool>> func, Func<Task> excepFunc) : base(excepFunc)
        {
			function = func;
		}

        public Func<UserInfo, Task<bool>> function { get; set; }
		public async override Task<bool> DataAction(UserInfo userInfo)
		{
			return await function.Invoke(userInfo);
		}
	}

	public partial class EventDataWriter<TResult> : DataWriter
	{
		public EventDataWriter(Func<UserInfo, Task<bool>> func, Func<Task<TResult>> excepFunc) : base(excepFunc)
		{
			function = func;
		}

		public Func<UserInfo, Task<bool>> function { get; set; }
		public async override Task<bool> DataAction(UserInfo userInfo)
		{
			return await function.Invoke(userInfo);
		}
	}
}
