using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PowerSpeckLib
{
    public class Slide
    {
        private Color _background;
        private int _hold;
        private readonly SortedList<int, SlideObject> _objects;

        public Slide(int hold = -1)
        {
            IsVisible = false;
            Hold = hold;
            Transition = new SlideTransition {Type = SlideTransitionEffect.None, Duration = 0};
            _objects = new SortedList<int, SlideObject>();
        }

        public int Hold
        {
            get { return _hold; }
            set
            {
                IsVisible = value > 0;
                _hold = value;
            }
        }

        public SlideTransition Transition { get; set; }

        public Color Background
        {
            get { return _background; }
            set
            {
                IsVisible = true;
                _background = value;
            }
        }

        public IList<SlideObject> SlideObjects 
        {
            get
            {
                return _objects.Values;
            }
        }

        public bool IsVisible { get; set; }

        public void AddObject(SlideObject obj, int position=-1)
        {
            _objects.Add(position, obj);
            IsVisible = true;
        }

        /// <summary>
        /// Invalidates all resources, forcing them to be reloaded from disk
        /// </summary>
        public void Invalidate()
        {
            foreach (var b in _objects)
            {
                if (b.Value.Type == SlideObjectType.Image)
                    b.Value.Invalidate();
            }
        }
    }
}