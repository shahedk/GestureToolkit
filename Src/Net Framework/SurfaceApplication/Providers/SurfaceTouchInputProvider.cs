using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.TouchInputProviders;
using Framework.Utility;
using Gestures.Objects;
using Microsoft.Surface;
using Microsoft.Surface.Presentation.Controls;
using Framework;
using System.Windows;
using Microsoft.Surface.Core;
using ContactEventHandler = Microsoft.Surface.Presentation.ContactEventHandler;

namespace SurfaceApplication.Providers
{
    public class SurfaceTouchInputProvider : TouchInputProvider
    {
        public override event TouchInputProvider.FrameChangeEventHandler FrameChanged;
        public override event TouchInputProvider.SingleTouchChangeEventHandler SingleTouchChanged;
        public override event TouchInputProvider.MultiTouchChangeEventHandler MultiTouchChanged;

        private SurfaceWindow _window;
        public ContactTarget _contactTarget;

        public SurfaceTouchInputProvider(SurfaceWindow window)
        {
            _window = window;
        }

        private Dictionary<int, TouchPoint2> _activeTouchPoints = new Dictionary<int, TouchPoint2>();
        private Dictionary<int, TouchInfo> _activeTouchInfos = new Dictionary<int, TouchInfo>();

        public override void Init()
        {
            // Add the necessary event handlers
            _window.ContactDown += ContactDown;
            _window.ContactChanged += ContactChanged;
            _window.ContactLeave += ContactLeave;

            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(_window).Handle;
            _contactTarget = new Microsoft.Surface.Core.ContactTarget(hwnd);

            _contactTarget.FrameReceived += new EventHandler<FrameReceivedEventArgs>(_contactTarget_FrameReceived);
            _contactTarget.EnableInput();
        }

        void _contactTarget_FrameReceived(object sender, FrameReceivedEventArgs e)
        {
            List<TouchInfo> touchInfoList = _activeTouchInfos.Values.ToList<TouchInfo>();
            List<TouchPoint2> touchPoints = _activeTouchPoints.Values.ToList<TouchPoint2>();

            // Raise "SingleTouchChanged" event if necessary
            if (SingleTouchChanged != null)
            {
                foreach (var touchPoint in touchPoints)
                {
                    SingleTouchChanged(this, new SingleTouchEventArgs(touchPoint));
                }
            }

            // Raise "MultiTouchChanged" event if necessary
            if (MultiTouchChanged != null)
            {
                MultiTouchChanged(this, new MultiTouchEventArgs(touchPoints));
            }

            // Raise "MultiTouchChanged" event if necessary
            if (FrameChanged != null)
            {
                var frameInfo = new FrameInfo() { TimeStamp = e.FrameTimestamp, Touches = touchInfoList };
                FrameChanged(this, frameInfo);
            }

            // Clean up local cache
            foreach (var touchInfo in touchInfoList)
            {
                if (touchInfo.ActionType == TouchAction2.Up)
                {
                    _activeTouchInfos.Remove(touchInfo.TouchDeviceId);
                    _activeTouchPoints.Remove(touchInfo.TouchDeviceId);
                }
            }
        }

        public void ContactLeave(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            UpdateActiveTouchPoints(TouchAction2.Up, e);
        }

        public void ContactChanged(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            UpdateActiveTouchPoints(TouchAction2.Move, e);
        }

        public void ContactDown(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            UpdateActiveTouchPoints(TouchAction2.Down, e);
        }

        public void UpdateActiveTouchPoints(TouchAction2 action, Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            //Get the  point position from the ContactEventArgs (can optionally use e.Contact.getCenterPosition here for more accuracy)
            Point position = e.GetPosition(GestureFramework.LayoutRoot);

            //Create a new touchinfo which will be used later to add a touchpoint
            TouchInfo info = new TouchInfo();

            //Set the action type to the passed in action
            info.ActionType = action;

            //Set the position of the touchinfo to the previously found position from e
            info.Position = position;

            //Set the deviceid of the touchinfo to the id of the contact
            info.TouchDeviceId = e.Contact.Id;

            TouchPoint2 touchPoint = null;

            //If it is contact down, we want to add the point, otherwise we want to update that particular point
            if (action == TouchAction2.Down)
            {
                //add the new touch point to the base
                touchPoint = base.AddNewTouchPoint(info, e.OriginalSource as UIElement);
            }
            else
            {
                //add the new touch point to the base
                touchPoint = base.UpdateActiveTouchPoint(info);
            }

            // Update local cache
            if (_activeTouchPoints.ContainsKey(info.TouchDeviceId))
            {
                _activeTouchPoints[info.TouchDeviceId] = touchPoint;
                _activeTouchInfos[info.TouchDeviceId] = info;
            }
            else
            {
                _activeTouchPoints.Add(info.TouchDeviceId, touchPoint);
                _activeTouchInfos.Add(info.TouchDeviceId, info);
            }
        }
    }
}
