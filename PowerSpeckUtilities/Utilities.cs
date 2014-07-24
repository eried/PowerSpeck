using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PowerSpeckUtilities
{
    public static class Utilities
    {
        public static Bitmap ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratio = Math.Min((double) maxWidth/image.Width, (double) maxHeight/image.Height);

            var newWidth = (int) (image.Width*ratio);
            var newHeight = (int) (image.Height*ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return new Bitmap(newImage);
        }

        internal static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        public static string GetExceptionDetails(this Exception exception)
        {
            PropertyInfo[] properties = exception.GetType()
                .GetProperties();
            IEnumerable<string> fields = properties
                .Select(property => new
                {
                    property.Name,
                    Value = property.GetValue(exception, null)
                })
                .Select(x => String.Format("{0} = {1}", x.Name, x.Value != null ? x.Value.ToString() : String.Empty));
            return String.Join("\n", fields);
        }

        public static void Log(String details)
        {
            try
            {
                string e = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + details;
                File.AppendAllText("events.log", e + Environment.NewLine);
            }
            catch
            {
            }
        }

        public static Color ParseColorOrDefault(string colorName, Color defaultColor)
        {
            if (!String.IsNullOrEmpty(colorName))
            {
                if(!colorName.StartsWith("#"))
                    try
                    {
                        return Color.FromName(colorName);
                    }
                    catch
                    {
                    }

                try
                {
                    return ColorTranslator.FromHtml(colorName);
                }
                catch
                {
                }
            }
            return defaultColor;
        }

        internal static TimeSpan ParseTime(string p)
        {
            TimeSpan r;
            if (!TimeSpan.TryParseExact(p, @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture, out r))
            {
                Log("[Format] Time is invalid: " + p);
                return TimeSpan.MaxValue;
            }
            return r;
        }

        public static string GetColorName(Color color)
        {
            foreach (var known in Enum.GetValues(typeof (KnownColor)).Cast<KnownColor>().Select(Color.FromKnownColor).Where(known => !known.IsSystemColor && color.ToArgb() == known.ToArgb()))
                return known.Name;

            return ColorTranslator.ToHtml(color);
        }
    }
}