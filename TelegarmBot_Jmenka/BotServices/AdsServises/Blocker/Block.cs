using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.Data.User;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.Blocker
{
	public class Block: BlockBase
	{
        public Block(Predicate<UserInfo> func)
		{
			function = func;
		}

		public Predicate<UserInfo> function { get; set; }

		public override bool Checking(UserInfo userInfo)
		{
			return function.Invoke(userInfo);
		}
	}
}
