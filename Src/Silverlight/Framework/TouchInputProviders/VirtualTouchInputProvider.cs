using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Framework.Utility;

using System.Collections.Generic;
using Gestures.Objects;
using Framework.TouchInputProviders;

namespace Framework.HardwareListeners
{
    public class VirtualTouchInputProvider : TouchInputProvider
    {
        public override event TouchInputProvider.FrameChangeEventHandler FrameChanged;
        public override event TouchInputProvider.SingleTouchChangeEventHandler SingleTouchChanged;
        public override event TouchInputProvider.MultiTouchChangeEventHandler MultiTouchChanged;

        internal void Touch_FrameReported(FrameInfo frameInfo)
        {
            // Raw data, frame changed
            if (FrameChanged != null)
                FrameChanged(this, frameInfo);

            // Get active touch points including stoke data
            List<TouchPoint2> touchPoints = base.UpdateActiveTouchPoints(frameInfo.Touches);

            // Creating one callback for each touch point
            if (SingleTouchChanged != null)
            {
                foreach (TouchPoint2 point in touchPoints)
                {
                    SingleTouchChanged(this, new SingleTouchEventArgs(point));
                }
            }

            // Sending all points in one callback
            if (MultiTouchChanged != null)
            {
                MultiTouchChanged(this, new MultiTouchEventArgs(touchPoints));
            }
        }
    }
}
