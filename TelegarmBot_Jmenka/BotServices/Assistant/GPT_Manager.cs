using OpenAI_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.BotServices.Assistant
{
	public class GPT_Manager
	{
		public static OpenAIAPI OpenAIAPI { get; set; }

		public static void Initial(string openAIkey)
		{
			OpenAIAPI = new OpenAIAPI(openAIkey);
		}
	}
}
