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
using Framework.Exceptions;
using System.Collections.ObjectModel;

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

        private bool _reloadExistingProjectList = true;
        private TouchInputRecorder _recorder;

        public List<string> GestureList
        {
            get
            {
                if (ExistingProjectNameComboBox.SelectedValue != null)
                    return _recorder.GetGestureList(ExistingProjectNameComboBox.SelectedValue as string);
                else
                    return new List<string>();
            }
        }

        public GestureRecordingPanel()
        {
            InitializeComponent();
            Init();
        }
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
            string gestureName = GestureNameTextBox.Text.Trim();
            _recorder.Save(data, projectName, gestureName);
        }

        #endregion

        #region Init & Content Loading
        private void Init()
        {
            // Set control captions
            RunButton.Content = ControlCaptions.RunGesture;
            StartRecordingButton.Content = ControlCaptions.StartRecording;

            // Initialize the recorder
            try
            {
                _recorder = new TouchInputRecorder(Dispatcher);
                _recorder.GestureSaved += _recorder_GestureSaved;
                _recorder.ConnectivityCheckCompleted += _recorder_ConnectivityCheckCompleted;
                _recorder.ExistingContentDownloadCompleted += _recorder_ExistingContentDownloadCompleted;
                _recorder.ProjectListDownloadCompleted += _recorder_ProjectListDownloadCompleted;
                _recorder.PlaybackCompleted += _recorder_PlaybackCompleted;
            }
            catch (FrameworkException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (Exception ex)
            {
                //TODO: Log unhandled exception and notify user
            }

        }

        void _recorder_PlaybackCompleted()
        {
            Dispatcher.BeginInvoke(() =>
            {
                RunButton.IsEnabled = true;
                RunButton.Content = ControlCaptions.RunGesture;
            });
        }

        void _recorder_ProjectListDownloadCompleted(object sender, EventArgs e)
        {
            ReloadProjectComboBox();
            LoadingUserData.Visibility = System.Windows.Visibility.Collapsed;
        }

        void _recorder_ExistingContentDownloadCompleted(object sender, EventArgs e)
        {
            // Since dynamic data binding will autometically show the data in comboBox, 
            // just make the panel visible
            LoadingUserData.Visibility = System.Windows.Visibility.Collapsed;
        }

        void _recorder_ConnectivityCheckCompleted(object sender, EventArgs e)
        {
            LoadingScreen.Visibility = Visibility.Collapsed;
            LoadUserData();
        }

        void _recorder_GestureSaved(object sender, EventArgs e)
        {
            // Check if we need to reload the project list
            string projectName = ProjectNameTextBox.Text.Trim();
            if (!_recorder.ProjectList.Contains(projectName))
            {
                _recorder.GestureDictionary[projectName] = new List<string>();
                _reloadExistingProjectList = true;
            }
            else
            {
                _reloadExistingProjectList = false;
            }

            // Refresh project/gesture list in UI
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
            if (_recorder.UserName == null)
            {
                ShowLoginScreen();
            }
            else
            {
                // Show registered user screen
                UserNameTextBlock.Text = _recorder.UserName;
                LoginScreenGrid.Visibility = System.Windows.Visibility.Collapsed;
                RegisteredUserGrid.Visibility = System.Windows.Visibility.Visible;

                // Do we need to get latest data from Server??
                _recorder.ValidateCache();
            }
        }

        private void ShowLoginScreen()
        {
            // Show login screen
            LoginScreenGrid.Visibility = System.Windows.Visibility.Visible;
            RegisteredUserGrid.Visibility = System.Windows.Visibility.Collapsed;
        }


        #endregion

        #region Utility Methods
        private void ShowErrorMessage(string msg)
        {
            MessageBox.Show(msg);
            MessageTextBlock.Text = msg;
        }

        private void ReloadProjectComboBox()
        {
            // Reload project list
            ExistingProjectNameComboBox.ItemsSource = new string[0];
            ExistingProjectNameComboBox.ItemsSource = _recorder.ProjectList;
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

                _recorder.RunGesture(projName, gestureName);

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
        #endregion

        #region UI Events
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text.Trim().Length > 0)
            {
                _recorder.UserName = UserNameTextBox.Text;
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
                _recorder.StopPlayback();
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
