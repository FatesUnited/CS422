// WaveGestureSegments.cs
using Microsoft.Kinect;

namespace HeadSkeletal_Tracking
{
    public interface IGestureSegment
    {
        GesturePartResult Update(Skeleton skeleton);
    }

    public class WaveSegment1 : IGestureSegment
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand above elbow
            if (skeleton.Joints[JointType.HandRight].Position.Y >
                skeleton.Joints[JointType.ElbowRight].Position.Y)
            {
                // Hand right of elbow
                if (skeleton.Joints[JointType.HandRight].Position.X >
                    skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

    public class WaveSegment2 : IGestureSegment
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand above elbow
            if (skeleton.Joints[JointType.HandRight].Position.Y >
                skeleton.Joints[JointType.ElbowRight].Position.Y)
            {
                // Hand left of elbow
                if (skeleton.Joints[JointType.HandRight].Position.X <
                    skeleton.Joints[JointType.ElbowRight].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }
            }

            // Hand dropped
            return GesturePartResult.Failed;
        }
    }
}