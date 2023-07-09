using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegarmBot_Jmenka.Instruments
{
	public static class DataCodec
	{
		public static string TextEncoder(this string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			return Convert.ToBase64String(bytes);
		}
		public static string TextDecoder(this string data)
		{
			byte[] bytes = Convert.FromBase64String(data);
			return Encoding.UTF8.GetString(bytes);
		}
	}
}
