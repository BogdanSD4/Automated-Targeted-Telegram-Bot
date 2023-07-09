using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.BotServices.GoogleServices
{
	public class GoogleService
	{
		public static GoogleCredential? credential { get; set; }
		public static void Initial(string creditialsPath)
		{
			credential = GoogleCredential.FromFile(creditialsPath);
		}
	}
}
