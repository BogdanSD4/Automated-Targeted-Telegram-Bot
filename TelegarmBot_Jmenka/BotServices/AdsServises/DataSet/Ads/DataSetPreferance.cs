using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Ads
{
    public class DataSetPreferance
    {
        public string Name { get; set; }
        public IEnumerable<string> Url { get; set; }
        public IEnumerable<string> Foto { get; set; }
        public IEnumerable<string> Video { get; set; }
        public AdsAccountType AdsAccountType { get; set; }
        public IEnumerable<QAnswer> QAnswer { get; set; }

        public static DataSetPreferance AccountLite
        {
            get
            {
                return new DataSetPreferance
                {
                    Name = "Adidas",
                    AdsAccountType = AdsAccountType.Lite,
                    Url = new string[]
                    {
                        "https://www.adidas.ua/krosivki-lite-racer-2-0-performance-eh1426",
                        "https://www.adidas.ua/krosivki-forum-low-originals-gv7613",
                        "https://www.adidas.ua/krossovki-dlya-bega-climacool-sportswear-gx5581-1"
                    },
                    QAnswer = new QAnswer[]
                    {
                        new QAnswer("001", new int[]{1}),
                        new QAnswer("002", new int[]{4}),
                    }
                };
            }
        }

        public static DataSetPreferance AccountStandart
        {
            get
            {
                var foto = Directory.GetFiles("C:\\Users\\dokto\\OneDrive\\Рабочий стол\\TelegramBotKPI\\TelegarmBot_Jmenka\\TelegarmBot_Jmenka\\AdsData\\Foto\\");

                return new DataSetPreferance
                {
                    Name = "Abibas",
                    AdsAccountType = AdsAccountType.Standart,
                    Url = new string[]
                    {
                        "https://www.adidas.ua/krosivki-lite-racer-2-0-performance-eh1426",
                        "https://www.adidas.ua/krosivki-forum-low-originals-gv7613",
                        "https://www.adidas.ua/krossovki-dlya-bega-climacool-sportswear-gx5581-1"
                    },
                    Foto = foto,
                    QAnswer = new QAnswer[]
                    {
                        new QAnswer("001", new int[]{2}),
                        new QAnswer("002", new int[]{4, 6}),
                    }
                };
            }
        }

        public static DataSetPreferance AccountPremium
        {
            get
            {
                var foto = Directory.GetFiles("C:\\Users\\dokto\\OneDrive\\Рабочий стол\\TelegramBotKPI\\TelegarmBot_Jmenka\\TelegarmBot_Jmenka\\AdsData\\Foto\\");
                var video = Directory.GetFiles("C:\\Users\\dokto\\OneDrive\\Рабочий стол\\TelegramBotKPI\\TelegarmBot_Jmenka\\TelegarmBot_Jmenka\\AdsData\\Video\\");

                return new DataSetPreferance
                {
                    Name = "Abibos",
                    AdsAccountType = AdsAccountType.Premium,
                    Url = new string[]
                    {
                        "https://www.adidas.ua/krosivki-lite-racer-2-0-performance-eh1426",
                        "https://www.adidas.ua/krosivki-forum-low-originals-gv7613",
                        "https://www.adidas.ua/krossovki-dlya-bega-climacool-sportswear-gx5581-1"
                    },
                    Foto = foto,
                    Video = video,
                    QAnswer = new QAnswer[]
                    {
                        new QAnswer("001", new int[]{4}),
                        new QAnswer("002", new int[]{0, 3}),
                    }
                };
            }
        }
    }
}
