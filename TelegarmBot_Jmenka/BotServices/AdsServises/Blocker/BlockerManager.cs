using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.Data.User;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.Blocker
{
	public static class BlockerManager
	{
		public static IDictionary<long, List<BlockBase>> blocks { get; set; } = new Dictionary<long, List<BlockBase>>();

		public static bool CheckBlocks(UserInfo userInfo)
		{
			if (blocks.ContainsKey(userInfo.UserId))
			{
				List<BlockBase> dellList = new List<BlockBase>(); 
				bool result = false;
				var list = blocks[userInfo.UserId];
				foreach (var block in list)
				{
					bool res = block.Checking(userInfo);
					if(!res) dellList.Add(block);
					if(!result && res) result = res;
				}

				foreach (var block in dellList)
				{
					blocks[userInfo.UserId].Remove(block);
				}

				return result;
			}
			return false;
		}

		public static void AddBlock(UserInfo userInfo, BlockBase block)
		{
			if (blocks.ContainsKey(userInfo.UserId))
			{
				blocks[userInfo.UserId].Add(block);
			}
			else
			{
				blocks.Add(userInfo.UserId, new List<BlockBase> { block });
			}
		}

		public static void RemoveBlock(UserInfo userInfo, BlockBase block)
		{
			if (blocks.ContainsKey(userInfo.UserId))
			{
				blocks[userInfo.UserId].Remove(block);
			}
		}
	}
}
