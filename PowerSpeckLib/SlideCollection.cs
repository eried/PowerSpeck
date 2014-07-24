using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace PowerSpeckLib
{
    public class SlideCollection
    {
        private readonly Stopwatch _timer;
        private int _currentSlide, _previousSlide;

        public SlideCollection()
        {
            _previousSlide = -1;
            Slides = new List<Slide>();
            _timer = new Stopwatch();
        }

        public Slide CurrentSlide
        {
            get { return Slides[_currentSlide]; }
        }


        public Slide PreviousSlide
        {
            get { return _previousSlide>=0? Slides[_previousSlide] : new Slide(); }
        }

        public void AddSlide(Slide newSlide)
        {
            Slides.Add(newSlide);
        }

        public List<Slide> Slides { get; private set; }

        public bool Draw(Graphics graphics)
        {
            var sequenced = false;

            if (Slides.Count == 0)
                return false;

            if (!_timer.IsRunning)
                _timer.Start();

            if (CurrentSlide.Transition.Type != SlideTransitionEffect.None && _timer.ElapsedMilliseconds < CurrentSlide.Transition.Duration)
            {
                // Transitions
                graphics.DrawImage(DrawSlide(PreviousSlide, graphics.VisibleClipBounds.Size), 0, 0);
                graphics.DrawImage(DrawSlide(CurrentSlide, graphics.VisibleClipBounds.Size), InterpolateTransition(graphics.VisibleClipBounds, CurrentSlide.Transition, _timer.ElapsedMilliseconds));
                sequenced = true;
            }
            else
            {
                graphics.DrawImage(DrawSlide(CurrentSlide, graphics.VisibleClipBounds.Size), 0,0);
            }

            CheckTimer();
            return sequenced;
        }

        private static float Map(float x, float inMin, float inMax, float outMin, float outMax, InterpolationMode mode)
        {
            if (mode == InterpolationMode.Linear)
                return (x - inMin)*(outMax - outMin)/(inMax - inMin) + outMin;

            return inMin > inMax ? 0 : (float) ((Math.Pow((x - inMin)/(inMax - inMin),
                    Math.Pow(10, mode == InterpolationMode.FastStartSlowEnd ? -0.3f : 0.3f))*(outMax - outMin)) + outMin);
        }

        private static PointF InterpolateTransition(RectangleF bounds, SlideTransition transition, long elapsed)
        {
            var p = bounds.Location;
            var mode = InterpolationMode.Linear;

            var s = transition.Type.ToString();

            if (s.StartsWith("Pan"))
                mode = InterpolationMode.SlowStartFastEnd;
            else if (s.StartsWith("Cover") || s.StartsWith("Fly"))
                mode = InterpolationMode.FastStartSlowEnd;

            switch (transition.Type)
            {
                case SlideTransitionEffect.CoverLeft:
                case SlideTransitionEffect.UncoverLeft:
                case SlideTransitionEffect.FlyFromLeft:
                case SlideTransitionEffect.PanLeft:
                    p.X = Map(elapsed, 0, transition.Duration, bounds.Left - bounds.Width, 0f, mode);
                    break;

                case SlideTransitionEffect.CoverUp:
                case SlideTransitionEffect.UncoverUp:
                case SlideTransitionEffect.FlyFromTop:
                case SlideTransitionEffect.PanUp:
                    p.Y = Map(elapsed, 0, transition.Duration,bounds.Top - bounds.Height, 0f, mode);
                    break;

                case SlideTransitionEffect.CoverRight:
                case SlideTransitionEffect.UncoverRight:
                case SlideTransitionEffect.PanRight:
                case SlideTransitionEffect.FlyFromRight:
                    p.X = Map(elapsed, 0, transition.Duration, bounds.Right, 0f, mode);
                    break;

                case SlideTransitionEffect.CoverDown:
                case SlideTransitionEffect.UncoverDown:
                case SlideTransitionEffect.PanDown:
                case SlideTransitionEffect.FlyFromBottom:
                    p.Y = Map(elapsed, 0, transition.Duration,bounds.Bottom, 0f, mode);
                    break;

                case SlideTransitionEffect.CoverLeftUp:
                case SlideTransitionEffect.UncoverLeftUp:
                case SlideTransitionEffect.FlyFromTopLeft:
                    p.X = Map(elapsed, 0, transition.Duration, bounds.Left - bounds.Width, 0f, mode);
                    p.Y = Map(elapsed, 0, transition.Duration, bounds.Top - bounds.Height, 0f, mode);
                    break;

                case SlideTransitionEffect.CoverRightUp:
                case SlideTransitionEffect.UncoverRightUp:
                case SlideTransitionEffect.FlyFromTopRight:
                    p.X = Map(elapsed, 0, transition.Duration, bounds.Right, 0f, mode);
                    p.Y = Map(elapsed, 0, transition.Duration, bounds.Top - bounds.Height, 0f, mode);
                    break;

                case SlideTransitionEffect.CoverLeftDown:
                case SlideTransitionEffect.UncoverLeftDown:
                case SlideTransitionEffect.FlyFromBottomLeft:
                    p.X = Map(elapsed, 0, transition.Duration, bounds.Left - bounds.Width, 0f, mode);
                    p.Y = Map(elapsed, 0, transition.Duration, bounds.Top - bounds.Height, 0f, mode);
                    break;

                case SlideTransitionEffect.CoverRightDown:
                case SlideTransitionEffect.UncoverRightDown:
                case SlideTransitionEffect.FlyFromBottomRight:
                    p.X = Map(elapsed, 0, transition.Duration, bounds.Right, 0f, mode);
                    p.Y = Map(elapsed, 0, transition.Duration, bounds.Bottom, 0f, mode);
                    break;
            }

            return p;
        }

        private Image DrawSlide(Slide s, SizeF size)
        {
            var img = new Bitmap((int)size.Width,(int)size.Height);
            var g = Graphics.FromImage(img);

            g.Clear(s.Background);

            // Draw elements
            foreach (var obj in s.SlideObjects)
                obj.Draw(g);

            return img;
        }

        private void CheckTimer()
        {
            if (_timer.ElapsedMilliseconds > CurrentSlide.Hold+CurrentSlide.Transition.Duration)
                NextSlide();
        }

        private void NextSlide()
        {
            _previousSlide = _currentSlide;
            _currentSlide++;

            if (_currentSlide >= Slides.Count)
                _currentSlide = 0; // Loop

            _timer.Reset();
        }
    }

    internal enum InterpolationMode
    {
        Linear,
        SlowStartFastEnd,
        FastStartSlowEnd
    }
}