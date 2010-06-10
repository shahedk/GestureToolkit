using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TouchToolkit.Framework.TouchInputProviders;
using System.Collections.Generic;
using TouchToolkit.Framework.Exceptions;
using TouchToolkit.GestureProcessor.Objects;
using System.Threading;
using TouchToolkit.Framework.Utility;
using System.IO.IsolatedStorage;
using TouchToolkit.Framework.DataService;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using TouchToolkit.Framework.Storage;

namespace TouchToolkit.Framework.Components
{
    public class TouchInputRecorder
    {
        private VirtualTouchInputProvider _touchListener = new VirtualTouchInputProvider();
        private ParameterizedThreadStart _backgroundThreadStart;
        private Thread _backgroundThread;
        public event GesturePlaybackCompleted PlaybackCompleted;

        public delegate void GesturePlaybackCompleted();

        #region Recorder
        private List<FrameInfo> _recordedEvents = new List<FrameInfo>(200);
        private bool isStarted = false;

        /// <summary>
        /// Starts capturing touch interactions
        /// </summary>
        public void StartRecording()
        {
            if (!isStarted)
            {
                isStarted = true;
                _recordedEvents.Clear();
                TouchInputManager.ActiveTouchProvider.FrameChanged += ActiveHardware_FrameChanged;
            }
            else
            {
                throw new FrameworkException("Recording already in progress!");
            }
        }

        /// <summary>
        /// Stops recording of touch interaction and returns the recorded data serialized into xml
        /// </summary>
        /// <returns></returns>
        public string StopRecording()
        {
            if (isStarted)
            {
                isStarted = false;
                TouchInputManager.ActiveTouchProvider.FrameChanged -= ActiveHardware_FrameChanged;

                // Save the recording into persistant medium
                string serializedContent = _recordedEvents.ToXml();

                return serializedContent;
            }
            else
            {
                throw new FrameworkException("There is no on-going recording to stop!");
            }
        }

        /// <summary>
        /// Determines whether the serialized content logically matches with the provided object model
        /// </summary>
        /// <param name="serializedContent"></param>
        /// <param name="recordedEvents"></param>
        /// <returns></returns>
        public bool ValidateSerialization(string serializedContent, List<FrameInfo> recordedEvents)
        {
            GestureInfo gestureInfo = SerializationHelper.Desirialize(serializedContent);

            // Validate content
            int index = 0;
            bool? result = false;
            foreach (FrameInfo curObj in recordedEvents)
            {
                FrameInfo serializedObj = recordedEvents[index++];
                result = serializedObj.IsEqual(curObj);

                if (result != true)
                    break;
            }

            return (result == true);
        }

        private void ActiveHardware_FrameChanged(object sender, FrameInfo frameInfo)
        {
            _recordedEvents.Add(frameInfo);
        }

        #endregion

        #region Player
        /// <summary>
        /// Simulates the touch(s) as specified in the xml
        /// </summary>
        /// <param name="xml">XML serialized collection of FrameInfo objects</param>
        public void RunGesture(string xml, GesturePlaybackCompleted playbackCompleted = null)
        {
            GestureInfo gestureInfo = SerializationHelper.Desirialize(xml);
            // Initializing background thread to playback recorded gestures
            _backgroundThreadStart = new ParameterizedThreadStart(RunGesture);
            _backgroundThread = new Thread(_backgroundThreadStart);

            Tuple<GestureInfo, TouchInputRecorder.GesturePlaybackCompleted> args =
                new Tuple<GestureInfo, TouchInputRecorder.GesturePlaybackCompleted>(gestureInfo, playbackCompleted);

            _backgroundThread.Start(args);
        }

        /// <summary>
        /// Starts the playback of recorded gesture
        /// </summary>
        /// <param name="param"></param>
        protected void RunGesture(object param)
        {
            Tuple<GestureInfo, GesturePlaybackCompleted> info = param as Tuple<GestureInfo, GesturePlaybackCompleted>;

            GestureInfo gestureInfo = info.Item1;
            var existingInputProvider = TouchInputManager.ActiveTouchProvider;

            try
            {
                GestureFramework.UpdateInputProvider(_touchListener);

                Action<FrameInfo> act = delegate(FrameInfo frame)
                {
                    _touchListener.Touch_FrameReported(frame);
                };

                foreach (FrameInfo frameInfo in gestureInfo.Frames)
                {
                    TouchAction2 a = frameInfo.Touches[0].ActionType;
                    object[] val = new object[1];
                    val[0] = frameInfo;

                    if (GestureFramework.LayoutRoot.Parent == null)
                    {
                        // Its a fake UI created by the automated UnitTest
                        act(frameInfo);
                    }
                    else
                    {
                        // Running from actual UI
                        GestureFramework.LayoutRoot.Dispatcher.BeginInvoke(act, frameInfo);
                    }

                    Thread.Sleep(frameInfo.WaitTime);
                }

                // Notify playback complition
                if (info.Item2 != null)
                    info.Item2();

                if (PlaybackCompleted != null)
                    PlaybackCompleted();
            }
            catch
            {
                throw;
            }
            finally
            {
                GestureFramework.UpdateInputProvider(existingInputProvider);
                if (info.Item2 != null)
                    info.Item2();
            }
        }
        #endregion

        //private void PlaybackEnded()
        //{
        //    //if (PlaybackCompleted != null)
        //    //    PlaybackCompleted();
        //}

        public void StopPlayback()
        {
            //TODO: The thread that is playing the recorded gesture needs to be stopped

            //PlaybackEnded();
        }
    }
}
