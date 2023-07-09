using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Ads;
using TelegarmBot_Jmenka.BotServices.Answers;
using TelegarmBot_Jmenka.Data.User;
using TelegarmBot_Jmenka.Database;
using TelegarmBot_Jmenka.Instruments;

namespace TelegarmBot_Jmenka.BotServices.AdsServises
{
    public static class AdsManager
	{
		private const string SETTINGS_PATH = "../../../adsManagerSettings.json";	

		private static List<AdsDataSet> adsDataSets = new List<AdsDataSet>();

		public static List<AdsDataSet> currentDataSet {  get { return adsDataSets; } }
		public static void SetUp(IEnumerable<AdsDataSet> dataSets)
		{
			adsDataSets = dataSets.ToList();

			string file = File.ReadAllText(SETTINGS_PATH);
			JObject json = JObject.Parse(file);

			int time = 1000 * (int)json.SelectToken("repeatTime");

			Task.Run(() =>
			{
				while (true)
				{
					Thread.Sleep(time);
					SendAds();
				}
			});
		}

		private static async void SendAds()
		{
			Console.WriteLine("Try to show ads");
			IEnumerable<UserInfo> users = UsersDataController.GetAllUsers(x => x);

			#region Precondition
			if(users is null || users.Count() < 1)
			{
				Console.WriteLine("Show falied");
				return;
			}
			#endregion

			foreach (var user in users)
			{
				if (user.ShowAds)
				{
					if (user.AdsSetsData != null)
					{
						List<AdsDataSet> adsDatas = user.GetAdsData();
						user.ShowAds = false;

						if (adsDatas.Count < 1)
						{
							await ClientAnswer.SendMessage(user, "Random Ads");
							continue;
						}
						var ads = adsDatas.GetRandom();
						Console.WriteLine(ads.Name);
						await ClientAnswerBuilder.SendAds(user, ads);
						
						UsersDataController.SetUserInfo(user);
					}
				}
			}
		}

		public static List<AdsDataSet> GetAdsDataSet(UserInfo user)
		{
			List<AdsDataSet> dataSets = new List<AdsDataSet>();
			
			foreach (var set in adsDataSets)
			{
				bool end = false;
				foreach (var question in user.QuestionSet.questions)
				{
					if (end) break;
					foreach (var answer in set.Answers)
					{
						if (end) break;
						if(question == answer)
						{
							dataSets.Add(set);
							end = true;
						}
					}
				}
			}

			return dataSets;
		}
	}
}
