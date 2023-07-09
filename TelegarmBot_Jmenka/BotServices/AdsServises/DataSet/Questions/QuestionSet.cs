using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.BotServices.AdsServises.DataSet.Questions
{
    public class QuestionSet
    {
        public QuestionSet()
        {
            questions = new Question[] { };
        }
        public QuestionSet(IEnumerable<Question> quest)
        {
            questions = quest;
        }

        public IEnumerable<Question> questions { get; set; }

        [NotMapped]
        public Question? this[string id]
        {
            get
            {
                Console.WriteLine(questions.Count());
                foreach (var question in questions)
                {
                    Console.WriteLine(question.PollId);
                    if (question.PollId == id) return question;
                }
                return null;
            }
        }
        [NotMapped]
        [Newtonsoft.Json.JsonIgnore]
        public bool HaveAllAnswers
        {
            get
            {
                foreach (Question question in questions)
                {
                    if (question.Answers == null) return false;
                }
                return true;
            }
        }

        public void AddQuestions(IEnumerable<Question> newQuestions)
        {
            if (questions == null) questions = newQuestions;
            else
            {
                questions = questions.Concat(newQuestions);
            }

        }
        [NotMapped]
        public static QuestionSet standart
        {
            get
            {
                return new QuestionSet(new Question[]
                {
                    new Question("001","How old are you?", new string[]
                        {
                        "< 12",
                        "12 - 16",
                        "16 - 18",
                        "18 - 25",
                        "25 - 35",
                        "35 - 45",
                        "> 45",
                        }),
                    new Question("002", "What are your hobbies?", new string[]
                        {
                        "computer games",
                        "movies",
                        "books",
                        "music",
                        "sport",
                        "cooking",
                        "art"
                        }){IsMultipleAnswer = true}
                });
            }
        }
    }
}
