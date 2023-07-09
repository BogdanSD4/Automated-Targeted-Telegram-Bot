using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices;
using TelegarmBot_Jmenka.BotServices.AdsServises;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Ads;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions;
using TelegarmBot_Jmenka.BotServices.Assistant;
using TelegarmBot_Jmenka.BotServices.EventManager;
using TelegarmBot_Jmenka.Database;
using TelegarmBot_Jmenka.Instruments;

namespace TelegarmBot_Jmenka.Data.User
{

    public class UserInfo
	{
		public UserInfo() { }
        public UserInfo(long userId, long chatId) 
		{
			UserId = userId;
			ChatId = chatId;
			ShowAds = true;
			UserEvents = new UserEvents();
			UserAssistant = new UserAssistant();
		}
		public int Id { get; set; }
		public long UserId { get; private set; }
		public long ChatId { get; private set; }
		public string UserName { get; set; }
		public bool ShowAds { get; set; }
		[NotMapped]
		[JsonIgnore]
		public Properties SessionProp { get; set; }
		[NotMapped]
		public QuestionSet QuestionSet { get; set; }
		[JsonIgnore]
		public string QuestionSetData { get; private set; }
		[JsonIgnore]
		public string AdsSetsData { get; set; }
		[NotMapped]
		public UserEvents UserEvents { get; set; }
		[JsonIgnore]
		public string UserEventsData { get; private set; }
		[NotMapped]
		public UserAssistant UserAssistant { get; set; }
		[JsonIgnore]
		public string UserAssistantData { get; private set; }

		public void DownloadData()
		{
			QuestionSet = GetData<QuestionSet>(QuestionSetData);
			UserEvents = GetData<UserEvents>(UserEventsData);
			UserAssistant = GetData<UserAssistant>(UserAssistantData);
		}
		public void UploadData()
		{
			string? res = SetData(QuestionSet);
			QuestionSetData = res ?? QuestionSetData;
			res = SetData(UserEvents);
			UserEventsData = res ?? UserEventsData;
			res = SetData(UserAssistant);
			UserAssistantData = res ?? UserAssistantData;
		}

		#region DB_Data_Converter
		public void SetAdsData(List<AdsDataSet> adsDatas)
		{
			if (adsDatas is null) return;
			var nameList = adsDatas.Select(x => x.Name);

			var res = SetData(nameList);
			AdsSetsData = res ?? AdsSetsData;
		}
		public List<AdsDataSet> GetAdsData()
		{
			string[] adsName = JsonConvert.DeserializeObject<string[]>(AdsSetsData);
			var result = AdsManager.currentDataSet.Where(x => adsName.Contains(x.Name)).ToList();
			return result;
		}

		public TResult GetData<TResult>(string file)
		{
			return JsonConvert.DeserializeObject<TResult>(file);
		}
		public string? SetData(object value)
		{
			if (value is null) return null;
			return JsonConvert.SerializeObject(value);
		}
		#endregion
	}
}
