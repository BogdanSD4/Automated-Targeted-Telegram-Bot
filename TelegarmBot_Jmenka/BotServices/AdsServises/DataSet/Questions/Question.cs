using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions
{
    public class Question : IQuestion
    {
        public Question(string qCode, string questionTitle, IEnumerable<string> questions)
        {
            QCode = qCode;
            Title = questionTitle;
			QuestionList = questions;
        }

        public int Id { get; set; }
        public string QCode { get; set; }
        public string PollId { get; set; }
        public string Title { get; private set; }
        public int[] Answers { get; set; }
        public bool IsMultipleAnswer { get; set; }

        [NonSerialized] public IEnumerable<string> QuestionList;

		public static bool operator ==(Question left, IQuestion right)
        {
            if(left.QCode == right.QCode)
            {
                foreach (var itemLeft in left.Answers)
                {
                    foreach (var itemRight in right.Answers)
                    {
                        if(itemLeft == itemRight) return true;
                    }
                }
            }

            return false;
        }
		public static bool operator !=(Question left, IQuestion right)
        {
			if (left.QCode != right.QCode)
			{
				foreach (var itemLeft in left.Answers)
				{
					foreach (var itemRight in right.Answers)
					{
						if (itemLeft != itemRight) return false;
					}
				}
			}

			return true;
		}
	}
}
