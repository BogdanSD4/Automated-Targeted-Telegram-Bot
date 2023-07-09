using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.BotServices.Loading
{
	public abstract class LoadingUIBase
	{
		public string StartText { get; set; }
		public abstract string Invoke();

		public static LoadingUIBase SimpleHorizontalUI { get 
			{
				return new LoadingUI<int>((num) =>
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (num == 10) num = 0;

					for (int i = 0; i < 10; i++)
					{
						if (i == num)
						{
							stringBuilder.Append('!');
							continue;
						}
						stringBuilder.Append('|');
					}
					return stringBuilder.ToString();
				}, 0, (num) => { return ++num; });
			}
		}
	}
}
