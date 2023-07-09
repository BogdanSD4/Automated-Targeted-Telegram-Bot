using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelegarmBot_Jmenka.Data.User;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegarmBot_Jmenka.BotServices.MarkupService
{
    public class MarkupBuilder
    {
        /// <summary>
        /// This methot allow only <see cref="InlineKeyboardMarkup"/> or <see cref="ReplyKeyboardMarkup"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns>T or default T if you don't follow recomendation</returns>
        public static IReplyMarkup? CreateMarkup<T>(IEnumerable<IEnumerable<IEnumerable<string>>> value) where T : IReplyMarkup
        {
            Type current = typeof(T);

            IReplyMarkup? res = current.Name switch
            {
                "InlineKeyboardMarkup" => new InlineKeyboardMarkup(MarkupBase<InlineKeyboardButton>(value, InlineMarkup)),
                "ReplyKeyboardMarkup" => new ReplyKeyboardMarkup(MarkupBase<KeyboardButton>(value, ReplyMarkup)) { ResizeKeyboard = true },
                _ => default(T),
            };

            return res;
        }
        private static InlineKeyboardButton InlineMarkup(IEnumerator<string> value)
        {
            if (!value.MoveNext()) throw new Exception("Array value can not be null");
            string text = value.Current;
            if (text == "") throw new Exception("Array value can not be null");

            if (!value.MoveNext())
            {
                return new InlineKeyboardButton(text) { CallbackData = text};
            }
            else
            {
                string data = value.Current;
                if (data == "") throw new Exception("no array value can be null");

                return new InlineKeyboardButton(text) { CallbackData = data };
            }
        }
        private static KeyboardButton ReplyMarkup(IEnumerator<string> value)
        {
            if(!value.MoveNext()) throw new Exception("Array value can not be null");
			string text = value.Current;
            if (text == "") throw new Exception("Array value can not be null");

            return new KeyboardButton(text);
        }
        private static IEnumerable<IEnumerable<T>> MarkupBase<T>(IEnumerable<IEnumerable<IEnumerable<string>>> value, Func<IEnumerator<string>, T> func) where T: IKeyboardButton
		{
			#region Preconditions
			if (value == null) throw new Exception("IEnumerable<IEnumerable<IEnumerable<string>>> can't be null");
			else if (value.Count() < 1) throw new Exception("IEnumerable<IEnumerable<IEnumerable<string>>> can't be empty");
			#endregion
			List<List<T>> result = new List<List<T>>();

			foreach (var item in value)
			{
				#region Preconditions
				if (item == null) throw new Exception("IEnumerable<IEnumerable<string>> can't be null");
				if (item.Count() < 1) throw new Exception("IEnumerable<IEnumerable<string>> can't be empty");
				#endregion

				List<T> buttons = new List<T>();
				foreach (var button in item)
				{
					#region Preconditions
					if (item == null) throw new Exception("IEnumerable<string> can't be null");
					#endregion

                    object res = func(button.GetEnumerator());
					buttons.Add((T)res);
				}
				result.Add(buttons);
			}

			return result;
		}

        public static InlineKeyboardMarkup StandartAddOrRemove(string addQuery, string removeQuery) 
        {
            return new InlineKeyboardMarkup(new[]
            {
                InlineKeyboardButton.WithCallbackData("Add", addQuery),
                InlineKeyboardButton.WithCallbackData("Remove", removeQuery),
            });
        }
		public static InlineKeyboardMarkup StandartAddOrRemove(string addText, string removeText, string addQuery, string removeQuery)
		{
			return new InlineKeyboardMarkup(new[]
			{
				InlineKeyboardButton.WithCallbackData(addText, addQuery),
				InlineKeyboardButton.WithCallbackData(removeText, removeQuery),
			});
		}
        public static InlineKeyboardMarkup OneCell(string text, string query)
        {
			return new InlineKeyboardMarkup(new[]
			{
				InlineKeyboardButton.WithCallbackData(text, query),
			});
		}

		public static ReplyKeyboardMarkup StandartBackButton
        {
            get
            {
                return new ReplyKeyboardMarkup(new KeyboardButton("<= back")) { ResizeKeyboard = true};
            }
        }
    }
}
