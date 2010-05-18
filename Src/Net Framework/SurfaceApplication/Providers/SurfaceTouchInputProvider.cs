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
using Microsoft.Surface.Presentation;

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

        public void _window_ContactLeave(object sender, ContactEventArgs e)
        {
            //Call helper function to create appropriate touch point and register its call back
            _touchPoint_and_CallBack_Creator(TouchAction2.Up, e);
        }

        public void _window_ContactChanged(object sender, ContactEventArgs e)
        {
            //Call helper function to create appropriate touch point and register its call back
            _touchPoint_and_CallBack_Creator(TouchAction2.Move, e);
        }

        public void _window_ContactDown(object sender, ContactEventArgs e)
        {
            //Call helper function to create appropriate touch point and register its call back
            _touchPoint_and_CallBack_Creator(TouchAction2.Down, e);
        }

        public void _touchPoint_and_CallBack_Creator(TouchAction2 action, ContactEventArgs e)
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

            //If it is contact down, we want to add the point, otherwise we want to update that particular point
            if (action == TouchAction2.Down)
            {
                //add the new touch point to the base
                var touchPoint = base.AddNewTouchPoint(info, e.OriginalSource as UIElement);

                //Create the call back for the single touch point
                if (SingleTouchChanged != null)
                {
                    SingleTouchChanged(this, new SingleTouchEventArgs(touchPoint));
                }
            }
            else 
            {
                //add the new touch point to the base
                base.UpdateActiveTouchPoint(info);

                //work in progress, update the UpdateActiveTouchPoint 
            }
        }
    }
}
