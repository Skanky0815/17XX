using UnityEngine;

namespace Core
{
    public static class Hex
    {
        public static Color32 ToColor32(string hex)
        {
            if (hex.StartsWith("#")) hex = hex.Substring(1);

            var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            var a = hex.Length >= 8
                ? byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber)
                : (byte)255;

            return new Color32(r, g, b, a);
        }
    }
}
