using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Ads;
using TelegarmBot_Jmenka.Data.User;

namespace TelegarmBot_Jmenka.Database
{
	public class DBcontroller : DbContext
	{
        public DBcontroller(string connectionString) : base(connectionString)
        {

        }
        public DbSet<UserInfo> Users { get; set; }
	}
}
