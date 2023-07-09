using OpenAI_API.Chat;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using OpenAI_API.Models;
using TelegarmBot_Jmenka.Data.User;
using TelegarmBot_Jmenka.BotServices.AdsServises.Blocker;
using TelegarmBot_Jmenka.BotServices.MessageServices;
using TelegarmBot_Jmenka.Instruments;
using TelegarmBot_Jmenka.BotServices.Answers;
using Newtonsoft.Json;

namespace TelegarmBot_Jmenka.BotServices.Assistant
{
    public class UserAssistant
    {
        public UserAssistant()
        {
            LangCode = "uk-UA";
            RealtimeAnswer = true;
            Temperature = 8;
            IsNewConversation = true;
        }
        [JsonIgnore]
        public bool IsNewConversation { get; set; }
		public string LangCode { get; set; }
        public bool RealtimeAnswer { get; set; }
        public int Temperature { get; set; }
        public Conversation Conversation { get; set; }

        public void CreateNewChat()
        {
            var api = GPT_Manager.OpenAIAPI;

            ChatRequest chatRequest = new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
				Temperature = ((float)Temperature)/10,
                MaxTokens = 2000,
            };

            IsNewConversation = true;
            Conversation = api.Chat.CreateConversation(chatRequest);
        }
        public async Task AppendMessage(UserInfo userInfo, string request)
        {
            #region RequestAnalysis
            string[] words = new string[] { "draw", "намалюй", "нарисуй" };
            if(words.Contains(request.AllToSym(' ').ToLower()))
            {
                await ClientAnswer.SendPhoto(userInfo, new InputFileUrl(await Darw(request)));
                return;
            }
            #endregion

			Conversation.AppendUserInput(request);

            Block block = new Block((user) =>
            {
                MessageManager.LastDell(user);
                return true;
            });

            BlockerManager.AddBlock(userInfo, block);

            #region CreateAnswerString
            StringBuilder result = null;
            if (userInfo.SessionProp.update.Message.Type == Telegram.Bot.Types.Enums.MessageType.Voice)
            {
                result = new StringBuilder($"Request: {request}\nAnswer: ");
            }
            else
            {
				result = new StringBuilder("Answer: ");
			}
			#endregion

			int messageId = (await userInfo.SessionProp.client.SendTextMessageAsync(userInfo.ChatId, result.ToString())).MessageId;

			int maxUpdate = 15;
			int updateCounter = maxUpdate;

            if (RealtimeAnswer)
            {
                #region StreamResponse
                await Conversation.StreamResponseFromChatbotAsync(async (text) =>
                {
                    result.Append(text);
                    if (updateCounter-- <= 0)
                    {
                        updateCounter = maxUpdate;
                        await userInfo.SessionProp.client.EditMessageTextAsync(userInfo.ChatId, messageId, result.ToString());
                    }
                });
                if (updateCounter != 15)
                {
                    await userInfo.SessionProp.client.EditMessageTextAsync(userInfo.ChatId, messageId, result.ToString());
                }
                #endregion
            }
            else
            {
                #region SimpleResponse
                Task task = Task.Run(async () =>
                {
                    await Loading.Loading.LoadingMessage(userInfo, messageId, Loading.LoadingUIBase.SimpleHorizontalUI);
                });

                result.Append(await Conversation.GetResponseFromChatbotAsync());
                task.Dispose();

                await userInfo.SessionProp.client.EditMessageTextAsync(userInfo.ChatId, messageId, result.ToString());
                #endregion
            }

			Conversation.AppendExampleChatbotOutput(result.ToString());
            BlockerManager.RemoveBlock(userInfo, block);
        }

        public async Task<string> Darw(string request)
        {
            var api = GPT_Manager.OpenAIAPI;

            var result = await api.ImageGenerations.CreateImageAsync(request);

            return result.Data[0].Url;
        }
    }
}
