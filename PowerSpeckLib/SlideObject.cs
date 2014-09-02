using System.Drawing;

namespace PowerSpeckLib
{
    public abstract class SlideObject
    {
        public object Tag { get; set; }

        public int Top { get; set; }

        public int Left { get; set; }

        public SlideObjectType Type { get; set; }

        public abstract void Draw(Graphics graphics);

        public virtual void Invalidate(){}
    }
}