// WaveGesture.cs
using Microsoft.Kinect;
using System;

namespace HeadSkeletal_Tracking
{
    public class WaveGesture
    {
        readonly int WINDOW_SIZE = 50;

        IGestureSegment[] _segments;

        int _currentSegment = 0;
        int _frameCount = 0;
        public Skeleton skeleton = new Skeleton();

        public event EventHandler GestureRecognized;

        public WaveGesture()
        {
            WaveSegment1 waveSegment1 = new WaveSegment1();
            WaveSegment2 waveSegment2 = new WaveSegment2();

            _segments = new IGestureSegment[]
            {
                waveSegment1,
                waveSegment2
            };
        }

        public void Update(Skeleton skeleton)
        {
            this.skeleton = skeleton;

            GesturePartResult result = _segments[_currentSegment].Update(skeleton);

            if (result == GesturePartResult.Succeeded)
            {
                if (_currentSegment + 1 < _segments.Length)
                {
                    _currentSegment++;
                    _frameCount = 0;
                }
                else
                {
                    if (GestureRecognized != null)
                    {
                        GestureRecognized(this, new EventArgs());
                        Reset();
                    }
                }
            }
            else if (result == GesturePartResult.Failed || _frameCount == WINDOW_SIZE)
            {
                Reset();
            }
            else
            {
                _frameCount++;
            }
        }

        public void Reset()
        {
            _currentSegment = 0;
            _frameCount = 0;
        }
    }
}