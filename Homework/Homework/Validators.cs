

using System;
using System.Text;
using System.Globalization;
using System.Linq;

namespace Homework
{
	class IntValidators
	{
		public static int? GreaterThanZero(string? str)
		{
			int number;
			if ((int.TryParse(str, out number) == false))
			{
				return null;
			}

			if (number <= 0)
			{
				return null;
			}

			return (number);
		}

		public static bool TryGreaterThanZero(string? number)
		{
			int val;

			if ((number == null)
			    || (int.TryParse(number, out val) == false))
			{
				return false;
			}

			return (val > 0);
		}
	}

	class StringValidators
	{
		public static bool IsUpperCase(char letter)
			=> (letter >= 'A' && letter <= 'Z');

		public static bool IsLowerCase(char letter)
			=> (letter >= 'a' && letter <= 'z');

		public static bool isLetter(char letter)
			=> (IsUpperCase(letter) || IsLowerCase(letter));

		public static bool IsNumber(char letter)
			=> (letter >= '0' && letter <= '9');

		public static string? IsName(string? str)
		{
			if (str == null)
			{
				return null;
			}

			string[] strokes = str.Split(' ');
			int cnt = strokes.Count();

			if (cnt != 2)
			{
				return null;
			}

			for (int i = 0; i < cnt; ++i)
			{
				if (StringValidators.IsUpperCase(strokes[i][0]) == false)
				{
					return null;
				}
			}

			for (int i = 0; i < cnt; ++i)
			{
				for (int j = 1; j < strokes[i].Count(); ++j)
				{
					if (StringValidators.IsLowerCase(strokes[i][j]) == false)
					{
						return null;
					}
				}
			}

			return (string) str;
		}

		public static bool TryName(string? name)
			=> (StringValidators.IsName(name) != null);

		public static string? IsUaPhoneNumber(string? str)
		{
			if (str == null)
			{
				return null;
			}

			if (str.Count() != 13)
			{
				return null;
			}

			if (str[0..4] != "+380")
			{
				return null;
			}

			for (int i = 4; i < str.Count(); ++i)
			{
				if (IsNumber(str[i]) == false)
				{
					return null;
				}
			}

			return (string) str;
		}

		public static bool TryUaPhoneNumber(string? number)
			=> (StringValidators.IsUaPhoneNumber(number) != null);
	}

	class DateValidators
	{
		public static bool IsFuture(string? str)
		{
			DateOnly date;
			if (DateOnly.TryParse(str, out date) == false)
			{
				return false;
			}

			if (date <= DateOnly.FromDateTime(DateTime.Today))
			{
				return false;
			}

			return true;
		}
	}

	class TimeValidators
	{
		public static bool TryTime(
			string? str,
			TimeOnly? lower = null,
			TimeOnly? upper = null)
		{
			TimeOnly time;

			if (TimeOnly.TryParse(str, out time) == false)
			{
				return false;
			}

			if ((lower != null) && (time < lower))
			{
				return false;
			}

			if ((upper != null) && (time > upper))
			{
				return false;
			}

			return true;
		}
	}
}