using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegarmBot_Jmenka.Instruments
{
	public static class FileManager
	{
		private static readonly string appConfigPath = "../../../appSettings.config";
		public static Configuration GetAppSettings()
		{
			ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
			fileMap.ExeConfigFilename = appConfigPath;
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
			return configuration;
		}
	}
}
