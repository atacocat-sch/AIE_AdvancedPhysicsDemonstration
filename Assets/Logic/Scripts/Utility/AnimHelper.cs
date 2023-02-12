using System.Text;
using UnityEngine;

namespace BoschingMachine.Utility
{
    public static class AnimHelper
    {
        public static string Typewriter (string s, float p)
        {
            if (p >= 1.0f) return s;
            if (p <= 0.0f) return string.Empty;

            int charCount = (int)(s.Length * p);
            var sb = new StringBuilder(charCount);

            for (int i = 0; i < charCount; i++)
            {
                sb.Append(s[i]);
            }

            return sb.ToString();
        }

        public static string TypewriterReversed(string s, float p) => Reverse(Typewriter(Reverse(s), p));
    
    
        private static string Reverse(string s)
        {
            var sb = new StringBuilder(s.Length);
            for (int i = s.Length - 1; i >= 0; i--)
            {
                sb.Append(s[i]);
            }
            return sb.ToString();
        }

        //public static int StrLen(string s)
        //{
        //    int l = 0;
        //}
    }
}
