using UnityEngine;

namespace BoschingMachine
{
    public static class TMPUtil
    {
        public static string Color(string text, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
        }
    }
}
