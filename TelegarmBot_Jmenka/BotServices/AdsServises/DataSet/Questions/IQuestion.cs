using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions
{
	public interface IQuestion
	{
		public string QCode { get; set; }
		public int[] Answers { get; set; }
	}
}
