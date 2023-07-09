using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.AdsServises.Blocker;
using TelegarmBot_Jmenka.Data.User;

namespace TelegarmBot_Jmenka.BotServices.DataWriter
{
	public static class DataWriterManager
	{
		public static IDictionary<long, DataWriter?> datas { get; set; } = new Dictionary<long, DataWriter?>();

		public static async Task<bool> ReadDatas(UserInfo userInfo)
		{
			if (datas.ContainsKey(userInfo.UserId))
			{
				var data = datas[userInfo.UserId];
				if(data is null) return false;

				bool res = await data.DataAction(userInfo);

				if (!res)
				{
					datas.Remove(userInfo.UserId);
				}

				return res;
			}
			return false;
		}

		public static void AddData(UserInfo userInfo, DataWriter dataWriter)
		{
			if (datas.ContainsKey(userInfo.UserId))
			{
				if(datas[userInfo.UserId] is not null)
				{
					datas[userInfo.UserId].ExceptionFunc();
				}
				datas[userInfo.UserId] = dataWriter;
			}
			else
			{
				datas.Add(userInfo.UserId,  dataWriter);
			}
		}
	}
}
