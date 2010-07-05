using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.TouchInputProviders;
using System.Windows.Controls;
using System.Windows.Media;
using mil.AnotoPen;
using mil.conoto;
using TouchToolkit.GestureProcessor.Objects;
using TouchToolkit.Framework.Utility;
using System.Threading;
using System.Windows.Threading;

namespace TouchToolkit.Framework.TouchInputProviders
{
    public class AnotoInputProvider : TouchInputProvider
    {
        public override event FrameChangeEventHandler FrameChanged;

        public override event MultiTouchChangeEventHandler MultiTouchChanged;

        public override event SingleTouchChangeEventHandler SingleTouchChanged;

        private AnotoStreamingServer anotoServer;
        private static ConotoElementManager manager;
        private static ConotoConvert conoto;
        private UIElement source;

        private Dictionary<int, TouchInfo> _activeTouchInfos = new Dictionary<int, TouchInfo>();
        private Dictionary<int, TouchPoint2> _activeTouchPoints = new Dictionary<int, TouchPoint2>();
        private List<int> _inactiveTouchPoints = new List<int>();
        long lastTimeStamp = 0;

        public AnotoInputProvider(Panel LayoutRoot)
        {
            source = LayoutRoot;
        }
        public override void Init()
        {
            // Initialize configuration
            manager = new ConotoElementManager();

            // Load config file created by MultiPatternConfigurator (or any other conoto config tool)
            // In this example the configuration file is stored in the folder "config".
            // This file maps the A4 sized Anoto-Pattern-Page "70.0.0.1" to a 1024x768 sized screen area.
            manager.LoadConfigFile("config/PatternConfig.pcf.xml");

            // Initialize event converter (needs the element manager that holds the configuration)
            conoto = new ConotoConvert(manager);

            // Initialize AnotoPen with the license key
            mil.AnotoPen.AnotoPen.Initialize("71ad1f27386ec4f5d91e15eca16f46c4");

            // Initialize the real-time streaming receiver of AnotoPen
            anotoServer = new AnotoStreamingServer();
            // Add event handler to receive pen up/down/move events.
            anotoServer.OnStroke += new AnotoStreamingServer.AnotoEventHandler(anotoServer_OnStroke);
            // Start receiver
            anotoServer.Start();
        }

        protected override void Dispose(bool value)
        {
            base.Dispose(value);

            // Stop receiver and dispose AnotoPen assembly. 
            // Otherwise the receiver thread will not be killed and the application process would
            // be still running after exit.
            anotoServer.Stop();
            mil.AnotoPen.AnotoPen.Dispose();
        }

        private void anotoServer_OnStroke(object sender, AnotoPenEvent args)
        {
            // Initialize some variables onto the conoto converter will write to.
            string function; // function string of the area hit by the pen (as configured in configuration)
            long functionid; // function id of the area hit by the pen (as configured in configuration)
            double x, y; // converted x/y coordinates
            TouchInfo info = new TouchInfo();
            TouchAction2 actionType = TouchAction2.Move;
            
            // handle the type of event we got and call other handler functions
            switch (args.Type)
            {
                case AnotoPenEventType.StrokeStart: // = pen down
                    actionType = TouchAction2.Down;
                    break;
                case AnotoPenEventType.StrokeDrag: // = pen move/drag (there is no hover due the AnotoPen does not support it)
                    actionType = TouchAction2.Move;
                    break;
                case AnotoPenEventType.StrokeEnd: // = pen up
                    actionType = TouchAction2.Up;
                    break;
            }

            // convert AnotoPen coordiantes to the configured ones (in this case, screen coordinates)
            conoto.Convert((ulong)args.PageId, args.X, args.Y,
                           out function, out functionid, out x, out y);

            info.ActionType = actionType;
            info.Position = new Point(x, y);
            info.TouchDeviceId = (int)args.PenId;

            if (_inactiveTouchPoints.Count > 0)
            {
                foreach (var key in _inactiveTouchPoints)
                {
                    if (_activeTouchPoints.ContainsKey(key))
                    {
                        _activeTouchPoints.Remove(key);
                    }
                    if (_activeTouchInfos.ContainsKey(key))
                    {
                        _activeTouchInfos.Remove(key);
                    }
                }
                _inactiveTouchPoints.Clear();
            }

            //If there's a Down action detected, add it to the active touch points
            if (actionType == TouchAction2.Down)
            {
                if (!_activeTouchPoints.ContainsKey(info.TouchDeviceId))
                {
                    _activeTouchPoints.Add(info.TouchDeviceId, new TouchPoint2(info, source));
                }
                else
                {
                    _activeTouchPoints[info.TouchDeviceId] = new TouchPoint2(info, source);
                }
            }
            else if (actionType == TouchAction2.Up)
            {
                if (_activeTouchPoints.ContainsKey(info.TouchDeviceId))
                {
                    _inactiveTouchPoints.Add(info.TouchDeviceId);
                    _activeTouchPoints[info.TouchDeviceId].Update(info);
                }
            }
            else
            {
                if (_activeTouchPoints.ContainsKey(info.TouchDeviceId))
                {
                    _activeTouchPoints[info.TouchDeviceId].Update(info);
                }
            }

            //Update the source to be the HitTest result
            foreach (var point in _activeTouchPoints)
            {
                if (point.Value.Action == TouchAction.Down)
                {
                    ThreadStart start = delegate()
                    {
                        GestureFramework.LayoutRoot.Dispatcher.Invoke(DispatcherPriority.Render,
                                          new Action<Point>(PerformHitTest), point.Value.Position);
                    };
                    start.Invoke();
                    point.Value.Source = source;
                }
            }

            //Add info to activetouchinfos
            if (_activeTouchInfos.ContainsKey(info.TouchDeviceId))
            {
                _activeTouchInfos[info.TouchDeviceId] = info;
            }
            else
            {
                _activeTouchInfos.Add(info.TouchDeviceId, info);
            }

            //Call the delegates
            if (SingleTouchChanged != null)
            {
                foreach (var point in _activeTouchPoints.Values)
                {
                    SingleTouchChanged(this, new SingleTouchEventArgs(point));
                }
            }

            if (MultiTouchChanged != null)
            {
                MultiTouchChanged(this, new MultiTouchEventArgs(_activeTouchPoints.Values.ToList<TouchPoint2>()));
            }

            if (FrameChanged != null)
            {
                FrameInfo finfo = new FrameInfo();
                List<TouchInfo> infos = _activeTouchInfos.Values.ToList<TouchInfo>();
                finfo.Touches = infos;
                finfo.TimeStamp = args.Timestamp;
                finfo.WaitTime = (int) args.Timestamp - (int) lastTimeStamp;
                FrameChanged(this, finfo);
            }
            lastTimeStamp = (int) args.Timestamp;
        }

        private void PerformHitTest(Point point)
        {
            if (GestureFramework.LayoutRoot.Parent == null)
            {
                //TODO: Its a fake UI created by the automated UnitTest. The VisualTreeHelper won't work in this case, so find an alternet way

                //Temporary workaround - point to root canvas
                source = GestureFramework.LayoutRoot;
            }
            else
            {
                var hitTestResult = VisualTreeHelper.HitTest(GestureFramework.LayoutRoot, point);

                if (hitTestResult == null)
                    source = GestureFramework.LayoutRoot;
                else
                    source = hitTestResult.VisualHit as UIElement;
                
                return;
            }
        }
    }
}
