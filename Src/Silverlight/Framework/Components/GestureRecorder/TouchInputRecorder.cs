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
using System.IO.IsolatedStorage;
using Framework.DataService;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;
using Framework.Storage;

namespace Framework.Components.GestureRecording
{
    public class TouchInputRecorder
    {
        //private IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;
        private GestureServiceSoapClient _clientService = new GestureServiceSoapClient();
        private ParameterizedThreadStart _backgroundThreadStart;
        private Thread _backgroundThread;
        private Dispatcher _dispatcher;

        public event EventHandler GestureSaved;
        public event EventHandler ConnectivityCheckCompleted;
        public event EventHandler ExistingContentDownloadCompleted;
        public event EventHandler ProjectListDownloadCompleted;
        public event GesturePlaybackCompleted PlaybackCompleted;

        public delegate void GesturePlaybackCompleted();

        #region AppSettings
        private DateTime LastSyncTime
        {
            get
            {
                DateTime? value = DataStorage.Instance.Get<DateTime?>("lastsynctime");
                if (value == null)
                    return DateTime.MinValue;
                else
                    return value.Value;
            }
            set
            {
                DataStorage.Instance.Save("lastsynctime", value);
            }
        }

        public string UserName
        {
            get
            {
                return DataStorage.Instance.Get<string>("username");
            }
            set
            {
                DataStorage.Instance.Save("username", value);
            }
        }

        public Dictionary<string, List<string>>.KeyCollection ProjectList
        {
            get
            {
                return GestureDictionary.Keys;
            }
        }

        public List<string> GetGestureList(string projectName)
        {
            if (projectName != null)
                if (GestureDictionary[projectName] != null)
                    return GestureDictionary[projectName] as List<string>;

            return new List<string>();
        }

        public Dictionary<string, List<string>> GestureDictionary
        {
            get
            {
                Dictionary<string, List<string>> dictionary = DataStorage.Instance.Get<Dictionary<string, List<string>>>("projectsAndGesture");
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, List<string>>();
                    DataStorage.Instance.Save("projectsAndGesture", dictionary);
                }

                return dictionary;
            }
        }

        bool isCacheValid = false;
        DateTime lastCacheValidated = DateTime.MinValue;
        public bool IsCacheValid
        {
            get
            {
                // If it was validated more than 3 mins ago, then check again. 
                // User may work in multiple machines at the same time
                if ((DateTime.Now - lastCacheValidated).TotalMinutes > 3)
                    return false;
                else
                    return isCacheValid;

            }
            set
            {
                isCacheValid = value;
            }
        }

        #endregion

        public TouchInputRecorder(string userName, Dispatcher uiThread)
        {
            UserName = userName;
            _dispatcher = uiThread;
            Init();
        }

        public TouchInputRecorder(Dispatcher uiThread)
        {
            _dispatcher = uiThread;
            Init();
        }

        private void Init()
        {
            // Initializing background thread to playback recorded gestures
            _backgroundThreadStart = new ParameterizedThreadStart(RunGesture);

            DataService.GestureServiceSoapClient c = new GestureServiceSoapClient();
            c.AddGestureData(

            // Subscribe to service callbacks
            _clientService.ConnectivityCheckCompleted += client_ConnectivityCheckCompleted;
            _clientService.LastUpdatedAtCompleted += clientService_LastUpdatedAtCompleted;
            _clientService.AddGestureDataCompleted += _clientService_AddGestureDataCompleted;
            _clientService.GetProjectsByUserCompleted += clientService_GetProjectsByUserCompleted;
            _clientService.GetGestureDataCompleted += clientService_GetGestureDataCompleted;

            // Step 1: Check server connectivity
            _clientService.ConnectivityCheckAsync();
        }

        #region Service callbacks

        private void clientService_GetGestureDataCompleted(object sender, GetGestureDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new FrameworkException("Unable to download gesture data!");
            }
            else
            {
                string gestureKey = e.UserState as string;
                if (CheckIsolatedStorageAvaliableSpace(e.Result))
                    _userSettings[gestureKey] = e.Result;

                GestureInfo gestureInfo = SerializationHelper.Desirialize(e.Result);
                RunGesture(gestureInfo);

            }
        }

        private void _clientService_AddGestureDataCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (GestureSaved != null)
                GestureSaved(sender, e);
        }

        private void client_ConnectivityCheckCompleted(object sender, ConnectivityCheckCompletedEventArgs e)
        {
            // Step 2: If connection available, load user screen otherwise disable this control and ask user to fix internet
            if (e.Error != null || e.Result == false)
            {
                throw new FrameworkException("Unable to establish server connection. Please check your internet connection!");
            }
            else
            {
                if (ConnectivityCheckCompleted != null)
                    ConnectivityCheckCompleted(sender, e);
            }
        }

        private void clientService_LastUpdatedAtCompleted(object sender, LastUpdatedAtCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                if (e.Result == DateTime.MinValue || e.Result > LastSyncTime)
                {
                    IsCacheValid = false;

                    // Download project list
                    _clientService.GetProjectsByUserAsync(UserName);
                }
                else
                {
                    if (ExistingContentDownloadCompleted != null)
                        ExistingContentDownloadCompleted(this, e);
                }
            }
            else
            {
                throw new FrameworkException("Unable to download content, please check your internet connection");
            }
        }

        private void clientService_GetProjectsByUserCompleted(object sender, GetProjectsByUserCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new FrameworkException("Sorry, the system failed to load project list!");
            }
            else
            {
                // Clear exising cache
                GestureDictionary.Clear();

                // Rebuild cache
                foreach (var projectInfo in e.Result)
                    foreach (var gestureName in projectInfo.GestureNames)
                    {
                        if (!GestureDictionary.Keys.Contains(projectInfo.ProjectName))
                            GestureDictionary[projectInfo.ProjectName] = new List<string>();

                        GestureDictionary[projectInfo.ProjectName].Add(gestureName);
                    }

                if (ProjectListDownloadCompleted != null)
                    ProjectListDownloadCompleted(sender, e);
            }
        }
        #endregion

        #region Load/Save Data
        /// <summary>
        /// Validates local cache with remote server and updates as necessary
        /// </summary>
        public void ValidateCache()
        {
            _clientService.LastUpdatedAtAsync(UserName);
        }

        /// <summary>
        /// Saves data in both remote server and local cache
        /// </summary>
        /// <param name="data"></param>
        /// <param name="projectName"></param>
        /// <param name="gestureName"></param>
        public void Save(string data, string projectName, string gestureName)
        {
            gestureName = GetUniqueGestureName(projectName, gestureName);
            string gestureDataKey = UserName + projectName + gestureName;

            // Gesture data
            DataStorage.Instance.SaveFile(gestureDataKey, data);
            
            // If its a new project, initialize the dictionary for the new project
            if (!GestureDictionary.Keys.Contains(projectName))
                GestureDictionary.Add(projectName, new List<string>());

            GestureDictionary[projectName].Add(gestureName);

            // ** Save in server
            _clientService.AddGestureDataAsync(UserName, projectName, gestureName, data);
        }
        #endregion

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
        /// Simulates the touch(s) as specified using the virtual touch input provider
        /// </summary>
        /// <param name="gestureInfo"></param>
        public void RunGesture(GestureInfo gestureInfo)
        {
            _backgroundThread = new Thread(_backgroundThreadStart);
            Tuple<GestureInfo, TouchInputRecorder.GesturePlaybackCompleted> args = new Tuple<GestureInfo, TouchInputRecorder.GesturePlaybackCompleted>(gestureInfo, PlaybackEnded);
            _backgroundThread.Start(args);
        }

        /// <summary>
        /// Simulates the touch(s) as specified using the virtual touch input provider
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="gestureName"></param>
        public void RunGesture(string projectName, string gestureName)
        {
            GestureInfo gestureInfo = null;

            string gestureDataKey = UserName + projectName + gestureName;
            if (_userSettings.Contains(gestureDataKey))
            {
                string data = (string)_userSettings[gestureDataKey];
                gestureInfo = SerializationHelper.Desirialize(data);

                RunGesture(gestureInfo);
            }
            else
            {
                _clientService.GetGestureDataAsync(UserName, projectName, gestureName, gestureDataKey);
            }

        }

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

        private string GetUniqueGestureName(string projectName, string gestureName)
        {
            if (GestureDictionary.Keys.Contains(projectName))
                if (GestureDictionary[projectName].Contains(gestureName))
                    gestureName += "_" + DateTime.Now.Ticks.ToString();

            return gestureName;
        }

        private void PlaybackEnded()
        {
            if (PlaybackCompleted != null)
                PlaybackCompleted();
        }

        public void StopPlayback()
        {
            //TODO: The thread that is playing the recorded gesture needs to be stopped

            PlaybackEnded();
        }
    }
}
