using System;
using System.Drawing;
using System.Text.RegularExpressions;
using PowerSpeckUtilities;

namespace PowerSpeckLib
{
    public class SlideParser
    {
        private static readonly Regex RegexText = new Regex(@"(?<posx>\d{1,5}),(?<posy>\d{1,5}),(?<size>[1-9][0-9]{1,3}|[1-9]),(?<color>#?\w{1,30}),(?<text>.*)"),
            RegexImage = new Regex("(?<posx>-?(?:[1-9][0-9]{1,4}|[0-9])),(?<posy>-?(?:[1-9][0-9]{1,4}|[0-9])),(?<width>auto|[1-9][0-9]{1,4}|[1-9]),(?<height>auto|[1-9][0-9]{1,4}|[1-9]),(?<file>.+)");


        public static SlideCollection Parse(string filePath, Color defaultBackground)
        {
            // Read slides
            var currentSlide = 1;
            var slides = new SlideCollection();
            var c = new IniParser(filePath);

            do
            {
                var slide = "slide" + currentSlide++;
                var newSlide = new Slide(c.GetSettingAsInteger(slide, "hold", 0));
                
                // Transition
                var tmp = ParseTransition(c.GetSetting(slide, "transition"));

                if(tmp!=null)
                    newSlide.Transition = tmp.Value;

                // Read the items
                foreach (var type in new[] {SlideObjectType.Text, SlideObjectType.Image})
                    ParseSlideObjects(type, slide, ref newSlide, c);

                if (newSlide.IsVisible)
                {
                    newSlide.Background = Utilities.ParseColorOrDefault(c.GetSetting(slide, "background"), defaultBackground);
                    slides.AddSlide(newSlide);
                }
                else
                    break;
            } while (true);

            return slides;
        }

        private static SlideTransition? ParseTransition(string p)
        {
            if (!String.IsNullOrEmpty(p) && p.Contains(","))
            {
                // Parse transition
                var parts = p.Split(new [] {','},2);

                if (parts.Length > 1)
                {
                    int length;
                    if (int.TryParse(parts[1], out length))
                        foreach (var t in Enum.GetNames(typeof (SlideTransitionEffect)))
                            if (parts[0].EndsWith(t, StringComparison.OrdinalIgnoreCase))
                                return new SlideTransition
                                {
                                    Duration = length,
                                    Type = (SlideTransitionEffect) Enum.Parse(typeof (SlideTransitionEffect), t)
                                };
                }
            }

            return null;
        }

        private static void ParseSlideObjects(SlideObjectType type, string slide, ref Slide newSlide, IniParser c)
        {
            int t = 1;
            do
            {
                string txt = (type == SlideObjectType.Text ? "txt" : "img") + t++;
                string tmp;
                var z = c.GetLine(slide, txt, null, out tmp);

                if (String.IsNullOrEmpty(tmp))
                    break;

                // Process command
                var m = (type == SlideObjectType.Text ? RegexText : RegexImage).Match(tmp);

                if (!m.Success) continue;

                switch (type)
                {
                    case SlideObjectType.Text:
                        newSlide.AddObject(new SlideText(m.Groups["text"].Value, int.Parse(m.Groups["posx"].Value),
                            int.Parse(m.Groups["posy"].Value), int.Parse(m.Groups["size"].Value),
                            Utilities.ParseColorOrDefault(m.Groups["color"].Value, Color.White)),z);
                        break;
                    case SlideObjectType.Image:
                        newSlide.AddObject(new SlideImage(m.Groups["file"].Value, int.Parse(m.Groups["posx"].Value),
                            int.Parse(m.Groups["posy"].Value),
                            int.Parse(m.Groups["width"].Value.Replace("auto", "-1")),
                            int.Parse(m.Groups["height"].Value.Replace("auto", "-1"))),z);
                        break;
                }
            } while (true);
        }
    }
}