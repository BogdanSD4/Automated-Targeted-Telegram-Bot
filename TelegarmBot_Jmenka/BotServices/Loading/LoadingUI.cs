using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.BotServices.Loading
{
	public partial class LoadingUI : LoadingUIBase
	{
        public LoadingUI(Func<string> action)
        {
            Func = action;
        }
        public LoadingUI(LoadingUI settings)
        {
            Func = settings.Func;
        }

        private Func<string> Func;

		public override string Invoke()
		{
			return StartText + Func();
		}
	}

	public partial class LoadingUI<T> : LoadingUIBase
	{
        public LoadingUI(Func<T, string> func, T arg1 , Func<T, T> callback)
        {
            Func = func;
            Value = arg1;
            Callback = callback;
        }
        public LoadingUI(LoadingUI<T> settings)
        {
            Func = settings.Func;
            Value = settings.Value;
            Callback = settings.Callback;
        }

        private Func<T, string> Func;
        private Func<T,T> Callback;
        private T Value;

        public override string Invoke()
        {
            string result = StartText + Func(Value);
            Value = Callback(Value);
            return result;
        }
	}
}
