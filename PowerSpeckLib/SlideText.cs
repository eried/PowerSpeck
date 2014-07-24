using System.Drawing;

namespace PowerSpeckLib
{
    public class SlideText : SlideObject
    {
        public SlideText(string text, int x, int y, float size, Color color)
        {
            Type = SlideObjectType.Text;
            Text = text;
            Size = size;
            Color = new SolidBrush(color);
            Top = x;
            Left = y;
        }

        public string Text { get; set; }
        public float Size { get; set; }
        public SolidBrush Color { get; set; }


        public override void Draw(Graphics graphics)
        {
            graphics.DrawString(Text, new Font("Arial", Size), Color, Top, Left);
        }
    }
}