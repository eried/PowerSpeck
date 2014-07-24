using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.VisualBasic;
using PowerSpeckLib;
using PowerSpeckUtilities;
using Shape = Microsoft.Office.Interop.PowerPoint.Shape;

namespace PowerSpeckConverter
{
    static internal class PowerPointHandling
    {
        internal static bool InitializeApplication(out Application app)
        {
            try
            {
                app = new Application();
            }
            catch
            {
                app = null;
                return true;
            }
            return false;
        }

        internal static int GetOfficeColor(Color color)
        {
            return Information.RGB(color.R, color.G, color.B);
        }

        internal static void ConvertToPowerpoint(string source, string destination)
        {
            Application ppApp;
            if (InitializeApplication(out ppApp)) throw new Exception("Can't start PowerPoint to create the output");

            var presentation = ppApp.Presentations.Add(MsoTriState.msoFalse);
            var n = 0;

            foreach (var src in SlideParser.Parse(source, Color.Black).Slides)
            {
                var dst = presentation.Slides.AddSlide(++n, presentation.SlideMaster.CustomLayouts[1]);
                dst.FollowMasterBackground = MsoTriState.msoFalse;
                dst.Background.Fill.ForeColor.RGB = GetOfficeColor(src.Background);

                if (src.Transition.Type != SlideTransitionEffect.None)
                {
                    var name = src.Transition.Type.ToString();
                    foreach (var t in (PpEntryEffect[])Enum.GetValues(typeof(PpEntryEffect)))
                        if(t.ToString().EndsWith(name))
                        {
                            var f = src.Transition.Duration/1000.0f;
                            dst.SlideShowTransition.Duration = f;

                            var d =dst.SlideShowTransition.Duration - f;

                            // Differences between the time adjustements
                            if (Math.Abs(d) > 0.0001)
                            {
                                f -= d;
                                dst.SlideShowTransition.Duration = f;
                            }

                            dst.SlideShowTransition.EntryEffect = t;
                            break;
                        }
                }

                dst.SlideShowTransition.AdvanceTime = src.Hold/1000.0f;
                dst.SlideShowTransition.AdvanceOnTime = MsoTriState.msoTrue;

                // Prepare temporal folder for images
                var imagesPath = Path.GetTempFileName();
                File.Delete(imagesPath);
                Directory.CreateDirectory(imagesPath);
                var imageNumber = 0;

                // Clear first
                while (dst.Shapes.Count > 0)
                    dst.Shapes[1].Delete();

                // Add objects
                foreach (var obj in src.SlideObjects)
                {
                    switch (obj.Type)
                    {
                        case SlideObjectType.Text:
                        {
                            var tmp = obj as SlideText;
                            var shape = dst.Shapes.AddTextbox(MsoTextOrientation.msoTextOrientationHorizontal, tmp.Top,
                                tmp.Left, 1000, tmp.Size);

                            shape.TextFrame.AutoSize = PpAutoSize.ppAutoSizeShapeToFitText;
                            shape.TextFrame.WordWrap = MsoTriState.msoFalse;
                            shape.TextFrame.TextRange.Font.Size = tmp.Size;
                            shape.TextFrame.TextRange.Font.Color.RGB = GetOfficeColor(tmp.Color.Color);
                            shape.TextFrame.TextRange.Text = tmp.Text;
                        }
                            break;

                        case SlideObjectType.Image:
                        {
                            var tmp = obj as SlideImage;
                            var tmpPath = Path.Combine(imagesPath, "img" + (imageNumber++)) + ".png";

                            if (tmp != null)
                            {
                                tmp.Image.Save(tmpPath, ImageFormat.Png);
                                dst.Shapes.AddPicture(tmpPath, MsoTriState.msoTrue, MsoTriState.msoTrue, tmp.Top,
                                    tmp.Left);
                            }
                        }
                            break;
                    }
                }
            }

            presentation.SaveAs(destination);
            presentation.Close();
            
            NAR(presentation);

            ppApp.Quit();
            NAR(ppApp);
        }

        internal static void ConvertToPowerSpeckPlayer(string source, string destination)
        {
            Application ppApp;
            if (InitializeApplication(out ppApp)) throw new Exception("Can't start PowerPoint to read the input");

            var presentation = ppApp.Presentations.Open(source, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);

            var o = new List<String>();
            var currentSlide = 0;

            var imagePath = FileHandling.FindAlternative(Path.GetFileNameWithoutExtension(destination), "_images", true);

            o.Add("; Generated by " + System.Windows.Forms.Application.ProductName);
            o.Add("; Source: " + source);

            foreach (Microsoft.Office.Interop.PowerPoint.Slide s in presentation.Slides)
            {
                int currentText = 0, currentImage = 0;

                o.Add(String.Empty);
                o.Add("[slide" + (++currentSlide) + "]");

                var holdTime = 5000;
                if (s.SlideShowTransition.AdvanceOnTime == MsoTriState.msoTrue)
                    holdTime = (int)s.SlideShowTransition.AdvanceTime*1000;

                o.Add("hold="+holdTime);
                o.Add("transition=" + s.SlideShowTransition.EntryEffect.ToString().Replace("ppEffect",String.Empty) + ","+(s.SlideShowTransition.Duration*1000));
                o.Add("background=" + GetOfficeColor(s.Background.Fill.ForeColor.RGB));

                foreach (Shape obj in s.Shapes)
                {
                    switch (obj.Type)
                    {
                        case MsoShapeType.msoTextBox:
                            if (obj.HasTextFrame == MsoTriState.msoTrue)
                            {
                                // Text
                                o.Add(String.Format("txt{0}={1},{2},{3},{4},{5}", ++currentText, (int) obj.Left,
                                    (int) obj.Top, (int) obj.TextFrame.TextRange.Font.Size,
                                    GetOfficeColor(obj.TextFrame.TextRange.Font.Color.RGB), obj.TextFrame.TextRange.Text));
                            }
                            break;

                        default:
                        {
                            if (!Directory.Exists(imagePath))
                                Directory.CreateDirectory(imagePath);

                            var imageFullName = Path.Combine(imagePath, "img" + currentSlide + "_" + currentImage + ".png");

                            obj.Export(Path.GetFullPath(imageFullName), PpShapeFormat.ppShapeFormatPNG, 0, 0, PpExportMode.ppScaleXY);
                            o.Add(String.Format("img{0}={1},{2},{3},{4},{5}", ++currentImage, (int)obj.Left, (int)obj.Top,
                                "auto", "auto", imageFullName));
                        }
                            break;
                    }

                
                }
            }

            File.WriteAllLines(destination, o);

            presentation.Close();
            NAR(presentation);

            ppApp.Quit();
            NAR(ppApp);
        }

        internal static string GetOfficeColor(int rgb)
        {
            return Utilities.GetColorName(Color.FromArgb((rgb >> 0) & 0xff, (rgb >> 8) & 0xff, (rgb >> 16) & 0xff));
        }

        internal static void NAR(object o)
        {
            try
            {
                Marshal.ReleaseComObject(o);
            }
            catch { }
            finally
            {
                o = null;
            }
        }
    }
}