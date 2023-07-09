using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Ads
{
    public class AdsDataSet
    {
        public AdsDataSet() { }
        public AdsDataSet(DataSetPreferance preferance)
        {
            Name = preferance.Name;
            Url = preferance.Url;
            Foto = preferance.Foto;
            Video = preferance.Video;
            AccountType = preferance.AdsAccountType;
            Answers = preferance.QAnswer;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Url { get; set; }
        public IEnumerable<string> Foto { get; set; }
        public IEnumerable<string> Video { get; set; }
        public AdsAccountType AccountType { get; set; }
        public IEnumerable<QAnswer> Answers { get; set; }
    }
}
