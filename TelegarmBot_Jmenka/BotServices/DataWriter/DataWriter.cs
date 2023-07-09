using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.Data.User;

namespace TelegarmBot_Jmenka.BotServices.DataWriter
{
	public abstract class DataWriter
	{
        public DataWriter(Func<Task>? func)
        {
            exceptionFunc = func;
        }
        private Func<Task>? exceptionFunc;
        public Func<Task> ExceptionFunc { get
            {
                if(exceptionFunc is null)
                {
                    return async () => { };
                }
                else
                {
                    return exceptionFunc;
                }
            } 
        }
		public abstract Task<bool> DataAction(UserInfo userInfo);
	}
}
