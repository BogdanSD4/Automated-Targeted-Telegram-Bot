using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TelegarmBot_Jmenka.BotServices;
using TelegarmBot_Jmenka.BotServices.AdsServises;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Ads;
using TelegarmBot_Jmenka.BotServices.Assistant;
using TelegarmBot_Jmenka.BotServices.GoogleServices;
using TelegarmBot_Jmenka.Database;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegarmBot_Jmenka
{
	public class Startup
	{
		public static ITelegramBotClient telegramBot;

		public static void EnableServices(ITelegramBotClient client, Configuration configuration)
		{
			#region ReadConfigFile
			string creditialPath = configuration.AppSettings.Settings["GoogleCreditials"].Value;
			string apiKey_OpenAi = configuration.AppSettings.Settings["OpenAIAPI"].Value;
			#endregion

			telegramBot = client;
			client.SetMyCommandsAsync(CreateBotCommands());

			AdsManager.SetUp(new AdsDataSet[]
			{
				new AdsDataSet(DataSetPreferance.AccountLite),
				new AdsDataSet(DataSetPreferance.AccountStandart),
				new AdsDataSet(DataSetPreferance.AccountPremium),
			});
			GoogleService.Initial(creditialPath);
			GPT_Manager.Initial(apiKey_OpenAi);	

			DatabaseConnector.Connection();
		}

		public static List<BotCommand> CreateBotCommands()
		{
			return new List<BotCommand>()
			{
				new BotCommand() {Command = "start", Description = "Start dialog"},
				new BotCommand() {Command = "events", Description = "Your events list"},
				new BotCommand() {Command = "schedule", Description = "Create new schedule"},
				new BotCommand() {Command = "assistant", Description = "Help with your problem"},
			};
		}
	}
}
