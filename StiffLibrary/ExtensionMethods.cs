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
            int Pos2 = STR.IndexOf(LastString);
            if(Pos2 - Pos1 > 0)
            {
                FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            }
            return FinalString;
        }

        public static int Clamp(this int input, int min, int max)
        {
            return input < min ? min : input > max ? max : input;
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
    }


}
