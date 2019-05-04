using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StiffLibrary
{
    public static class ExtensionMethods
    {

        public static string combineLines(this string[] lines, string flavor = "")
        {
            string combined = "";
            foreach(string line in lines)
            {
                combined += line+flavor;
            }
            return combined;
        }

        public static Control[] toArray(this Control.ControlCollection myControls)
        {
            List<Control> answer = new List<Control>();

            foreach(Control ct in myControls)
            {
                answer.Add(ct);
            }

            return answer.ToArray();
        }

        public static T[] justType<T>(this System.Windows.Forms.Control myControl)
        {
            List<T> listinha = new List<T>();

            foreach (object coisa in myControl.Controls)
            {
                if (coisa.GetType() == typeof(T))
                {
                    listinha.Add((T)coisa);
                }
            }
            return listinha.ToArray();
        }

        public static T[] justType<T>(this object[] myArray)
        {
            List<T> listinha = new List<T>();

            foreach (object coisa in myArray)
            {
                if (coisa.GetType() == typeof(T))
                {
                    listinha.Add((T)coisa);
                }
            }
            return listinha.ToArray();
        }

        public static bool Toggle(this bool me)
        {
            bool newValue = false;
            if (me == true)
                newValue = true;

            return newValue;
        }

        public static T[] ArrayAdd<T>(this T[] myArray, T item)
        {
            List<T> minhaLista = myArray.ToList();
            minhaLista.Add(item);
            return minhaLista.ToArray();
        }

        public static T[] ArrayAddRange<T>(this T[] myArray, T[] range)
        {
            List<T> minhaLista = myArray.ToList();
            minhaLista.AddRange(range);
            return minhaLista.ToArray();
        }

        public static T[] ArrayDeleteItem<T>(this T[] myArray, T item) where T : class
        {
            List<T> minhaLista = new List<T>();

            foreach (T tempItem in myArray)
            {
                if (tempItem != item)
                {
                    minhaLista.Add(tempItem);
                }
            }

            return minhaLista.ToArray();
        }

        public static T[] ArrayDeleteIndex<T>(this T[] myArray, int index)
        {
            List<T> minhaLista = new List<T>();

            for (int i = 0; i < myArray.Length; i++)
            {
                if (i != index)
                {
                    minhaLista.Add(myArray[i]);
                }
            }
            return minhaLista.ToArray();
        }

        public static string ExtractFromString(this string Text, string FirstString, string LastString)
        {
            string STR = Text;
            string STRFirst = FirstString;
            string STRLast = LastString;
            string FinalString = "";

            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            string subSTR = STR.Substring(Pos1);
            int Pos2 = subSTR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2);
            return FinalString;
        }

        public static int Clamp(this int input, int min, int max)
        {
            return input < min ? min : input > max ? max : input;
        }

        public enum ClampType
        {
            Greater,
            Lesser,
            GreaterOrEqual,
            LesserOrEqual
        }

        public static int ClampOneSide(this int input, ClampType clampType, int threshold)
        {
            switch (clampType)
            {
                case ClampType.Greater:
                    return input > threshold ? threshold : input;
                case ClampType.GreaterOrEqual:
                    return input >= threshold ? threshold - 1 : input;
                case ClampType.Lesser:
                    return input < threshold ? threshold : input;
                case ClampType.LesserOrEqual:
                    return input <= threshold ? threshold + 1 : input;
            }
            return 0;
        }

        public static T[] FilterBy<T>(this T[] me, Predicate<T> match)
        {
            List<T> tList = me.ToList();
            tList = tList.FindAll(match);
            return tList.ToArray();
        }

        public static bool IsNumeric(this string arg)
        {
            double i;
            return double.TryParse(arg, out i);
        }

        public static uint? ToNullableUInt(this string arg)
        {
            uint i;
            if (uint.TryParse(arg, out i)) return i;
            return null;
        }

        //==========================================PRIVATE
        private static bool Compare<T>(T x, T y) where T : class
        {
            return x == y;
        }

        private static IList createList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        private static Random random = new Random();
        public static string RandomString(this string me, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string BCryptHash(this string me)
        {
            return BCrypt.Net.BCrypt.HashPassword(me);
        }

        public static bool BCryptVerify(this string me, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(me, storedHash);
        }

        public static T[] AllEnumValues<T>(this T enumType) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            return (T[])Enum.GetValues(typeof(T));
        }

        /// <summary>
        /// Calculates number of business days, taking into account:
        ///  - weekends (Saturdays and Sundays)
        ///  - bank holidays in the middle of the week
        /// </summary>
        /// <param name="firstDay">First day in the time interval</param>
        /// <param name="lastDay">Last day in the time interval</param>
        /// <param name="bankHolidays">List of bank holidays excluding weekends</param>
        /// <returns>Number of business days during the 'span'</returns>
        public static int BusinessDaysUntil(this DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

    }


}
