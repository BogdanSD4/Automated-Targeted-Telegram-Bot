using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions
{
    public class QAnswer : IQuestion
    {
        public QAnswer(string qCode, int[] answer)
        {
            QCode = qCode;
            Answers = answer;
        }
        public int Id { get; set; }
        public string QCode { get; set; }
        public int[] Answers { get; set; }
    }
}
