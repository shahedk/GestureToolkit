using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using Framework.DataService;
using Framework.Components.GestureRecording;
using System.Reflection;
using System.ServiceModel;
using Framework.HardwareListeners;
using System.Threading;
using Gestures.Objects;
using Framework.Utility;

namespace Framework.UI
{
    public partial class GestureRecordingPanel : UserControl
    {
        #region Constructor & Global variables
        public class ControlCaptions
        {
            public const string StartRecording = "Start Recording";
            public const string StopRecording = "Stop Recording";
            public const string RunGesture = "Run";
            public const string StopGesture = "Stop";
        }

        private ParameterizedThreadStart _backgroundThreadStart;
        private Thread _backgroundThread;
        private bool _reloadExistingProjectList = true;

        private TouchInputRecorder _recorder = new TouchInputRecorder();
        private IsolatedStorageSettings _appSettings = IsolatedStorageSettings.ApplicationSettings;
        private GestureServiceSoapClient _clientService = new GestureServiceSoapClient();

        public GestureRecordingPanel()
        {
            InitializeComponent();

            Init();
        }
        #endregion

        #region AppSettings
        private DateTime LastSyncTime
        {
            get
            {
                if (_appSettings.Contains("lastsynctime"))
                    return (DateTime)_appSettings["lastsynctime"];
                else
                    return DateTime.MinValue;
            }
            set
            {
                _appSettings["lastsynctime"] = value;
                _appSettings.Save();
            }
        }

        private string UserName
        {
            get
            {
                if (_appSettings.Contains("username"))
                    return _appSettings["username"] as string;

                return null;
            }
            set
            {
                _appSettings["username"] = value;
                _appSettings.Save();
            }
        }

        public Dictionary<string, List<string>>.KeyCollection ProjectList
        {
            get
            {
                return GestureDictionary.Keys;
            }
        }

        public List<string> GestureList
        {
            get
            {
                if (ExistingProjectNameComboBox.SelectedValue != null)
                    if (GestureDictionary[ExistingProjectNameComboBox.SelectedValue as string] != null)
                        return GestureDictionary[ExistingProjectNameComboBox.SelectedValue as string] as List<string>;

                return new List<string>();
            }
        }

        public Dictionary<string, List<string>> GestureDictionary
        {
            get
            {
                if (!_appSettings.Contains("projectsAndGesture"))
                {
                    _appSettings["projectsAndGesture"] = new Dictionary<string, List<string>>();
                    _appSettings.Save();
                }

                return _appSettings["projectsAndGesture"] as Dictionary<string, List<string>>;
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

        IsolatedStorageSettings _userSettings = IsolatedStorageSettings.ApplicationSettings;
        #endregion

        #region Gesture Recording
        private void StartRecording()
        {
            _recorder.StartRecording();
        }

        private void StopRecording()
        {
            string data = _recorder.StopRecording();
            SaveRecordedData(data);
        }

        private void SaveRecordedData(string data)
        {
            string projectName = ProjectNameTextBox.Text.Trim();
            string gestureName = GetUniqueGestureName();

            string gestureDataKey = UserName + projectName + gestureName;

            // Gesture data
            _userSettings.Add(gestureDataKey, data);
            if (CheckIsolatedStorageAvaliableSpace(data))
                _userSettings.Save();


            // Project name
            if (!ProjectList.Contains(projectName))
            {
                GestureDictionary[projectName] = new List<string>();
                _reloadExistingProjectList = true;
            }
            else
            {
                _reloadExistingProjectList = false;
            }

            // Gesture name
            GestureDictionary[projectName].Add(gestureName);

            // ** Save in server
            _clientService.AddGestureDataAsync(UserName, projectName, gestureName, data);

        }

        private bool CheckIsolatedStorageAvaliableSpace(string data)
        {
            Int64 requiredSpace = data.Count() * 2;
            Int64 fiveMB = 1024 * 1024 * 5;
            bool enoughSpaceAvailable = false;

            IsolatedStorageFile storage = null;
            try
            {
                storage = IsolatedStorageFile.GetUserStoreForApplication();
                if (storage.AvailableFreeSpace < requiredSpace)
                {
                    long newQuota = storage.Quota + fiveMB;
                    enoughSpaceAvailable = storage.IncreaseQuotaTo(newQuota);

                    if (!enoughSpaceAvailable)
                    {
                        MessageBox.Show("Failed to save content in local cache due to space limitation!");
                    }
                }
                else
                {
                    enoughSpaceAvailable = true;
                }
            }
            catch
            {
                //TODO: handle possible error
            }

            return enoughSpaceAvailable;
        }

        private string GetUniqueGestureName()
        {
            string projectName = ProjectNameTextBox.Text.Trim();
            string gestureName = GestureNameTextBox.Text.Trim();

            if (GestureDictionary.Keys.Contains(projectName))
                if (GestureDictionary[projectName].Contains(gestureName))
                    gestureName += "_" + DateTime.Now.Ticks.ToString();

            return gestureName;
        }

        #endregion

        #region Init & Content Loading
        private void Init()
        {
            // Initializing background thread to playback recorded gestures
            _backgroundThreadStart = new ParameterizedThreadStart(_recorder.RunGesture);

            // Set control captions
            RunButton.Content = ControlCaptions.RunGesture;
            StartRecordingButton.Content = ControlCaptions.StartRecording;

            // Subscribe to service callbacks
            _clientService.ConnectivityCheckCompleted += client_ConnectivityCheckCompleted;
            _clientService.LastUpdatedAtCompleted += clientService_LastUpdatedAtCompleted;
            _clientService.AddGestureDataCompleted += _clientService_AddGestureDataCompleted;
            _clientService.GetProjectsByUserCompleted += clientService_GetProjectsByUserCompleted;
            _clientService.GetGestureDataCompleted += clientService_GetGestureDataCompleted;

            // Step 1: Check server connectivity
            _clientService.ConnectivityCheckAsync();
        }

        void clientService_GetGestureDataCompleted(object sender, GetGestureDataCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ShowErrorMessage("Unable to download gesture data!");
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

        void _clientService_AddGestureDataCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_reloadExistingProjectList)
                ReloadProjectComboBox();

            if (ProjectNameTextBox.Text.Trim() == (string)ExistingProjectNameComboBox.SelectedItem)
                ReloadGestureList();
        }

        private void ReloadGestureList()
        {
            // Reload gesture list
            ExistingGestureNameComboBox.ItemsSource = new string[0];
            ExistingGestureNameComboBox.ItemsSource = GestureList;
        }

        private void LoadUserData()
        {
            if (UserName == null)
            {
                ShowLoginScreen();
            }
            else
            {
                // Show registered user screen
                UserNameTextBlock.Text = UserName;
                LoginScreenGrid.Visibility = System.Windows.Visibility.Collapsed;
                RegisteredUserGrid.Visibility = System.Windows.Visibility.Visible;

                // Do we need to get latest data from Server??
                ValidateCache();
            }
        }

        private void ShowLoginScreen()
        {
            // Show login screen
            LoginScreenGrid.Visibility = System.Windows.Visibility.Visible;
            RegisteredUserGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void ValidateCache()
        {
            _clientService.LastUpdatedAtAsync(UserName);
        }
        #endregion

        #region Utility Methods
        private void ShowErrorMessage(string msg)
        {
            MessageBox.Show(msg);
            MessageTextBlock.Text = msg;
        }
        #endregion

        #region Service Callbacks
        void client_ConnectivityCheckCompleted(object sender, ConnectivityCheckCompletedEventArgs e)
        {
            // Step 2: If connection available, load user screen otherwise disable this control and ask user to fix internet
            if (e.Error != null || e.Result == false)
            {
                ShowErrorMessage("Unable to establish server connection. Please check your internet connection!");
            }
            else
            {
                LoadingScreen.Visibility = Visibility.Collapsed;

                // Step 3: Load user data
                LoadUserData();
            }
        }

        void clientService_LastUpdatedAtCompleted(object sender, LastUpdatedAtCompletedEventArgs e)
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
                    // Since dynamic data binding will autometically show the data in comboBox, 
                    // just make the panel visible
                    LoadingUserData.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            else
            {
                ShowErrorMessage("Unable to download content, please check your internet connection");
            }
        }

        void clientService_GetProjectsByUserCompleted(object sender, GetProjectsByUserCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ShowErrorMessage("Sorry, the system failed to load project list!");
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
                ReloadProjectComboBox();
                LoadingUserData.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ReloadProjectComboBox()
        {
            // Reload project list
            ExistingProjectNameComboBox.ItemsSource = new string[0];
            ExistingProjectNameComboBox.ItemsSource = ProjectList;


        }

        #endregion

        #region Play Recorded Gestures
        private void RunGesture()
        {
            // Update "start/stop" button text
            RunButton.Content = ControlCaptions.StopGesture;

            if (ExistingGestureNameComboBox.SelectedItem != null && ExistingGestureNameComboBox.SelectedItem != null)
            {
                // Get Gesture Data 
                string projName = (string)ExistingProjectNameComboBox.SelectedItem;
                string gestureName = (string)ExistingGestureNameComboBox.SelectedItem;

                RunGesture(projName, gestureName);

                // Run Gesture
                // Show remaining time
                // Provide stop option, on stop change button-state to start mode
                // On end, change button-state to start move
            }
            else
            {
                MessageBox.Show("Please select the \"Gesture\" you want to run!");
            }
        }

        private void RunGesture(string projName, string gestureName)
        {
            GestureInfo gestureInfo = null;

            string gestureDataKey = UserName + projName + gestureName;
            if (_userSettings.Contains(gestureDataKey))
            {
                string data = (string)_userSettings[gestureDataKey];
                gestureInfo = SerializationHelper.Desirialize(data);

                RunGesture(gestureInfo);
            }
            else
            {
                _clientService.GetGestureDataAsync(UserName, projName, gestureName, gestureDataKey);
            }

        }

        private void RunGesture(GestureInfo gestureInfo)
        {
            _backgroundThread = new Thread(_backgroundThreadStart);
            Tuple<GestureInfo, TouchInputRecorder.GesturePlaybackCompleted> args = new Tuple<GestureInfo, TouchInputRecorder.GesturePlaybackCompleted>(gestureInfo, StopGesture);
            _backgroundThread.Start(args);
        }



        private void StopGesture()
        {
            Dispatcher.BeginInvoke(() =>
            {
                RunButton.IsEnabled = true;
                RunButton.Content = ControlCaptions.RunGesture;
            });
        }
        #endregion

        #region UI Events
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text.Trim().Length > 0)
            {
                UserName = UserNameTextBox.Text;
                Init();
            }
            else
            {
                MessageBox.Show("Please enter your username!");
            }
        }

        private void StartRecordingButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)StartRecordingButton.Content == ControlCaptions.StartRecording)
            {
                StartRecording();
                StartRecordingButton.Content = ControlCaptions.StopRecording;
            }
            else if ((string)StartRecordingButton.Content == ControlCaptions.StopRecording)
            {
                StopRecording();
                StartRecordingButton.Content = ControlCaptions.StartRecording;
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)RunButton.Content == ControlCaptions.RunGesture)
            {
                RunGesture();
            }
            else if ((string)RunButton.Content == ControlCaptions.StopGesture)
            {
                StopGesture();

            }
        }

        private void ExistingProjectNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadGestureList();
        }

        private void ChangeAccount_MouseEnter(object sender, MouseEventArgs e)
        {
            //          Cursor = Cursors.Hand; 
        }

        private void ChangeAccount_MouseLeave(object sender, MouseEventArgs e)
        {
            //            Cursor = Cursors.None;
        }

        private void ChangeAccount_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ShowLoginScreen();
        }

        private void UserNameTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(sender, e);
        }

        #endregion

    }
}
