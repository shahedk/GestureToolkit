using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Framework.HardwareListeners;
using Framework.TouchInputProviders;
using Framework.Storage;
using Framework.Components;
using Framework.Exceptions;

namespace Framework
{
    public sealed class GestureTestFramework
    {
        private static string _userName = string.Empty;
        public static string UserName
        {
            get
            {
                return _userName;
            }
        }

        private static string _projectname = string.Empty;
        public static string ProjectName
        {
            get
            {
                return _projectname;
            }
            set
            {
                _projectname = value;
            }
        }

        private static Canvas _layoutRoot = null;
        private static TouchInputProvider _touchProvider = null;
        private static StorageManager _storage = null;
        private static TouchInputRecorder _recorder = null;
        private static bool _isInitialized = false;
        /// <summary>
        /// Provide the user name & project name to identify the gesture storage unit.
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="projectName"></param>
        public static void Init(string accountName, string projectName)
        {
            _userName = accountName;
            _projectname = projectName;
            _layoutRoot = new Canvas();
            _touchProvider = new VirtualTouchInputProvider();
            _storage = new StorageManager();
            _storage.Login(accountName);

            _recorder = new TouchInputRecorder();

            GestureFramework.Initialize(_touchProvider, _layoutRoot);

            _isInitialized = true;
        }

        public static void Validate(string gestureName, string dataKey, GestureEventHandler callback = null)
        {
            if (!_isInitialized)
                throw new FrameworkException("You must initialize the framework first!");

            _storage.GetGesture(_projectname, dataKey, (projName, gesName, data, error) =>
                {
                    // Callback on data receive from storage

                    if (error != null)
                    {
                        // Failed to retrieve data from storage
                        if (callback != null)
                        {
                            GestureEventArgs e = new GestureEventArgs();
                            e.Error = error;

                            callback(_layoutRoot, e);
                        }
                    }
                    else
                    {
                        // Subscribe to the specified gesture event
                        GestureFramework.EventManager.AddEvent(_layoutRoot, gestureName, callback);

                        // Playback the user interaction via virtual touch provider
                        _recorder.RunGesture(data);

                        // Unsubscrbe the gesture event (back to as before)
                        GestureFramework.EventManager.RemoveEvent(_layoutRoot, gestureName);
                    }
                });
        }
    }

    public delegate void ErrorHandler(Exception e);
}
