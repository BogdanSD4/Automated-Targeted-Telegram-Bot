using Google.Cloud.Speech.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.Data.User;

namespace TelegarmBot_Jmenka.BotServices.GoogleServises
{
    public static class GoogleAudioService
    {
		private static SpeechClient speechClient;
		public static SpeechClient SpeechClient { get 
			{
				if(speechClient is null)
				{
					SpeechClientBuilder builder = new SpeechClientBuilder() { GoogleCredential = GoogleServices.GoogleService.credential };
					speechClient = builder.Build();
					return speechClient;
				}
				else { return speechClient; }
			}
		}
        public static string SpeechToText(UserInfo userInfo, MemoryStream stream)
        {
			RecognitionConfig config = new RecognitionConfig
			{
				Encoding = RecognitionConfig.Types.AudioEncoding.OggOpus,
				SampleRateHertz = 48000,
				LanguageCode = userInfo.UserAssistant.LangCode,
			};
			RecognitionAudio recognition = RecognitionAudio.FromBytes(stream.ToArray());
			RecognizeResponse response = SpeechClient.Recognize(config, recognition);

			if (response.Results.Count > 0)
			{
				var result = response.Results[0];
				if (result.Alternatives.Count > 0)
				{
					return result.Alternatives[0].Transcript;
				}
				return result.LanguageCode;
			}
			else
			{
				return "I have nothing to answer to melody or silence";
			}
		}
    }
}
