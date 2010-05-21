using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.TouchInputProviders;
using libSMARTMultiTouch.Table;
using libSMARTMultiTouch.Input;
using System.Windows;
using System.Windows.Controls;
using Gestures.Objects;
using System.Windows.Shapes;
using System.Windows.Media;
using Framework;

namespace SMARTTabletop_Application.Providers
{
    public class SMARTTabletopTouchInputProvider : TouchInputProvider
    {
        public override event TouchInputProvider.FrameChangeEventHandler FrameChanged;

        public override event TouchInputProvider.SingleTouchChangeEventHandler SingleTouchChanged;

        public override event TouchInputProvider.MultiTouchChangeEventHandler MultiTouchChanged;
        
        private MainWindow _mainWindow;

        private Dictionary<int, TouchPoint2> _activeTouchPoints = new Dictionary<int, TouchPoint2>();
        private Dictionary<int, TouchInfo> _activeTouchInfos = new Dictionary<int, TouchInfo>();

        public SMARTTabletopTouchInputProvider(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public override void Init()
        {
            libSMARTMultiTouch.Input.TouchInputManager.AddTouchContactDownHandler(_mainWindow, new TouchContactEventHandler(TouchContactDown));
            libSMARTMultiTouch.Input.TouchInputManager.AddTouchContactUpHandler(_mainWindow, new TouchContactEventHandler(TouchContactUp));
            libSMARTMultiTouch.Input.TouchInputManager.AddTouchContactMoveHandler(_mainWindow, new TouchContactEventHandler(TouchMove));
        
            //Thread like implementation of FrameRecieved implement here
        }

        private void TouchContactDown(object sender, TouchContactEventArgs e)
        {
            UpdateActiveTouchPoints(TouchAction2.Down, e);            
        }

        private void TouchContactUp(object sender, TouchContactEventArgs e)
        {
            UpdateActiveTouchPoints(TouchAction2.Up, e);
        }

        private void TouchMove(object sender, TouchContactEventArgs e)
        {
            UpdateActiveTouchPoints(TouchAction2.Move, e);
        }

        public void UpdateActiveTouchPoints(TouchAction2 action, TouchContactEventArgs e)
        {
            Point position = e.TouchContact.GetPosition(GestureFramework.LayoutRoot);

            TouchInfo info = new TouchInfo();

            info.ActionType = action;

            info.Position = position;

            info.TouchDeviceId = e.TouchContact.ID;

            TouchPoint2 touchPoint = null;

            //If it is contact down, we want to add the point, otherwise we want to update that particular point
            if (action == TouchAction2.Down)
            {
                //add the new touch point to the base
                touchPoint = base.AddNewTouchPoint(info, e.OriginalSource as UIElement);
            }
            else
            {
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
