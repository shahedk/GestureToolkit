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

namespace SurfaceApplication.Providers
{
    public class SurfaceTouchInputProvider : TouchInputProvider
    {
        public override event TouchInputProvider.FrameChangeEventHandler FrameChanged;

        public override event TouchInputProvider.SingleTouchChangeEventHandler SingleTouchChanged;

        public override event TouchInputProvider.MultiTouchChangeEventHandler MultiTouchChanged;

        private SurfaceWindow _window;

        public SurfaceTouchInputProvider(SurfaceWindow window)
        {
            _window = window;
        }

        public override void Init()
        {
            //Add the necessary event handlers
            _window.ContactDown+=new Microsoft.Surface.Presentation.ContactEventHandler(_window_ContactDown);
            
            _window.ContactChanged+= new Microsoft.Surface.Presentation.ContactEventHandler(_window_ContactChanged);

            _window.ContactLeave+=new Microsoft.Surface.Presentation.ContactEventHandler(_window_ContactLeave);
            
        }

        void _window_ContactLeave(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            //Get the point position from the ContactEventArgs, but the centered one, to be more accurate
            Point position = e.Contact.GetCenterPosition(GestureFramework.LayoutRoot);

            //Create a new touchinfo which will be used later to add a touchpoint
            TouchInfo info = new TouchInfo();

            //Set the action type of this to down, since the contact is down itself
            info.ActionType = TouchAction2.Up;

            //Set the position of the touchinfo to the previously found position from e
            info.Position = position;

            //Set the deviceid of the touchinfo to the id of the contact
            info.TouchDeviceId = e.Contact.Id;

            //add the new touch point to the base
            base.UpdateActiveTouchPoint(info);

            //work in progress, update the UpdateActiveTouchPoint 

        }

        void _window_ContactChanged(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            //Get the point position from the ContactEventArgs, but the centered one, to be more accurate
            Point position = e.Contact.GetCenterPosition(GestureFramework.LayoutRoot);

            //Create a new touchinfo which will be used later to add a touchpoint
            TouchInfo info = new TouchInfo();

            //Set the action type of this to down, since the contact is down itself
            info.ActionType = TouchAction2.Move;

            //Set the position of the touchinfo to the previously found position from e
            info.Position = position;

            //Set the deviceid of the touchinfo to the id of the contact
            info.TouchDeviceId = e.Contact.Id;

            //add the new touch point to the base
            base.UpdateActiveTouchPoint(info);

            //work in progress, update the UpdateActiveTouchPoint 

        }

        void _window_ContactDown(object sender, Microsoft.Surface.Presentation.ContactEventArgs e)
        {
            //Get the  point position from the ContactEventArgs
            Point position = e.GetPosition(GestureFramework.LayoutRoot);

            //Create a new touchinfo which will be used later to add a touchpoint
            TouchInfo info = new TouchInfo();

            //Set the action type of this to down, since the contact is down itself
            info.ActionType = TouchAction2.Down;

            //Set the position of the touchinfo to the previously found position from e
            info.Position = position;

            //Set the deviceid of the touchinfo to the id of the contact
            info.TouchDeviceId = e.Contact.Id;

            //add the new touch point to the base
            var touchPoint = base.AddNewTouchPoint(info, e.OriginalSource as UIElement);

            //Create the call back for the single touch point
            if (SingleTouchChanged != null)
            {
                SingleTouchChanged(this, new SingleTouchEventArgs(touchPoint));
            }

        }
    }
}
