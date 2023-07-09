using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.Data.User;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.Blocker
{
	public abstract class BlockBase
	{
        public BlockBase()
        {
            
        }

        public abstract bool Checking(UserInfo userInfo);
	}
}
