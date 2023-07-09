
using Newtonsoft.Json;
using System.Configuration;
using System.Reflection;
using System.Text;
using TelegarmBot_Jmenka;
using TelegarmBot_Jmenka.BotServices;
using TelegarmBot_Jmenka.Data.User;
using TelegarmBot_Jmenka.Instruments;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

Configuration configuration = FileManager.GetAppSettings();
string botToken = configuration.AppSettings.Settings["TelegramApi"].Value;

TelegramBotClient botClient = new TelegramBotClient(botToken);
Startup.EnableServices(botClient, configuration);

botClient.StartReceiving(Update, Error);

Console.ReadLine();


async Task Update(ITelegramBotClient client, Update update, CancellationToken arg3)
{
	//return;
	new ClientQuery(client, update).Query();
}

async Task Error(ITelegramBotClient client, Exception error, CancellationToken arg3)
{
	throw error;
}

