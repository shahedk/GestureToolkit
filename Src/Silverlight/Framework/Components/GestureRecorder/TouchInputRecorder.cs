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
using Framework.HardwareListeners;
using System.Collections.Generic;
using Framework.Components.GestureRecording;
using Framework.Exceptions;
using Gestures.Objects;
using System.Threading;
using Framework.Utility;

namespace Framework.Components.GestureRecording
{
    public class TouchInputRecorder
    {
        public delegate void GesturePlaybackCompleted();

        public TouchInputRecorder()
        {

        }

        #region Recorder

        private List<FrameInfo> recordedEvents = new List<FrameInfo>();
        private bool isStarted = false;

        /// <summary>
        /// Starts capturing touch interactions
        /// </summary>
        public void StartRecording()
        {
            if (!isStarted)
            {
                isStarted = true;
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
                string serializedContent = recordedEvents.ToXml();

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
            recordedEvents.Add(frameInfo);
        }

        #endregion

        #region Player

        /// <summary>
        /// Starts the playback of recorded gesture
        /// </summary>
        /// <param name="param"></param>
        public void RunGesture(object param)
        {
            Tuple<GestureInfo, GesturePlaybackCompleted> info = param as Tuple<GestureInfo, GesturePlaybackCompleted>;

            GestureInfo gestureInfo = info.Item1;
            var existingInputProvider = TouchInputManager.ActiveTouchProvider;

            try
            {
                VirtualTouchInputProvider touchListener = new VirtualTouchInputProvider();
                GestureFramework.UpdateInputProvider(touchListener);

                foreach (FrameInfo frameInfo in gestureInfo.Frames)
                {
                    GestureFramework.LayoutRoot.Dispatcher.BeginInvoke(() =>
                    {
                        touchListener.Touch_FrameReported(frameInfo);
                    });

                    Thread.Sleep(frameInfo.WaitTime);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                //TODO: Log exception, handle it ...
            }
            finally
            {
                GestureFramework.UpdateInputProvider(existingInputProvider);
                info.Item2();
            }
        }
        #endregion
    }
}
