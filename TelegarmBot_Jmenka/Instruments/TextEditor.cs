using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegarmBot_Jmenka.Instruments
{
	public static class TextEditor
	{
		public static string AllAfterSym<TValue>(this TValue obj, char sym)
		{
			string text = obj.ToString();
            
            StringBuilder stringBuilder = new StringBuilder();
			bool start = false;

			for (int i = 0; i < text.Length; i++)
			{
				if (!start)
				{
					if (text[i] == sym)
					{
						start = true;
					}
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
			}

			return stringBuilder.ToString();
		}
		public static string AllAfterSym<TValue>(this TValue obj, char sym, uint repeatCount)
		{
			string text = obj.ToString();

			StringBuilder stringBuilder = new StringBuilder();
			bool start = false;

			for (int i = 0; i < text.Length; i++)
			{
				if (!start)
				{
					if (text[i] == sym)
					{
						if (repeatCount-- <= 0)
						{
							start = true;
						}
					}
				}
				else
				{
					stringBuilder.Append(text[i]);
				}
			}
			
			return stringBuilder.ToString();
		}
		public static string AllAfterSymWithStop<TValue>(this TValue obj, char sym, uint repeatCount, char stopSym)
		{
			string text = obj.ToString();

			StringBuilder stringBuilder = new StringBuilder();
			bool start = false;

			for (int i = 0; i < text.Length; i++)
			{
				if (!start)
				{
					if (text[i] == sym)
					{
						if (repeatCount-- <= 0)
						{
							start = true;
						}
					}
				}
				else
				{
					if (!text[i].StopAtSym(stopSym, stringBuilder)) break;
				}
			}

			return stringBuilder.ToString();
		}

		public static string AllToSym<TValue>(this TValue obj, char sym)
		{
			string text = obj.ToString();

			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == sym)
				{
					break;
				}

				stringBuilder.Append(text[i]);
			}

			return stringBuilder.ToString();
		}
		public static string AllToSym<TValue>(this TValue obj, char sym, uint repeatCount)
		{
			string text = obj.ToString();
			bool start = false;

			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == sym)
				{
					if (repeatCount-- <= 0)
					{
						break;
					}
				}

				stringBuilder.Append(text[i]);
			}

			return stringBuilder.ToString();
		}
		public static string AllToSymWithStop<TValue>(this TValue obj, char sym, uint repeatCount, char stopSym)
		{
			string text = obj.ToString();
			bool start = false;

			StringBuilder stringBuilder = new StringBuilder();

			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == sym)
				{
					if (repeatCount-- <= 0)
					{
						break;
					}
				}

				if (!text[i].StopAtSym(stopSym, stringBuilder)) break;
			}

			return stringBuilder.ToString();
		}

		private static bool StopAtSym(this char text, char sym, StringBuilder builder)
		{
			if (text == sym)
			{
				return false;
			}
			builder.Append(text);
			return true;
		}
	}
	
}
