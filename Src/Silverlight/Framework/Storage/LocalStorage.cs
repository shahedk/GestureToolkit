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
using System.Collections;
using System.IO;

using TouchToolkit.Framework.Utility;
using System.Collections.Generic;

namespace TouchToolkit.Framework.Storage
{
    public class LocalStorage : IDataStorage
    {
        private string _accountName = string.Empty;
        private string _filename = "RecordedGestures.dat";
        private Dictionary<string, Dictionary<string, string>> _projectDictionary = new Dictionary<string, Dictionary<string, string>>();

        public string AccountName
        {
            get { return _accountName; }
        }

        #region login/logout methods
        public bool IsLoggedIn()
        {
            return AccountName != string.Empty;
        }

        public void Login(string accountName)
        {
            _accountName = accountName;
        }

        public void Logout()
        {
            _accountName = string.Empty;
        }
        #endregion

        public void SaveGesture(string projectName, string gestureName, string value, SaveGestureCallback callback)
        {
            //Save data locally
            if (_projectDictionary.ContainsKey(projectName))
            {
                if (_projectDictionary[projectName].ContainsKey(gestureName))
                {
                    //Can't overwrite gestures?
                    //_projectDictionary[projectName][gestureName] = value;
                }
                else
                {
                    _projectDictionary[projectName].Add(gestureName,value);
                }
            }
            else
            {
                _projectDictionary.Add(projectName, new Dictionary<string, string>());
                _projectDictionary[projectName].Add(gestureName, value);
            }

            /*
            FileInfo finfo = new FileInfo(_filename);
            if (!finfo.Exists)
            {
                finfo.Create();
            }
            TextWriter writer = new StreamWriter(_filename);
            writer.WriteLine(projectName + " " + gestureName + " " + value);
            writer.Close();
            */
        }

        public void GetGesture(string projectName, string gestureName, GetGestureCallback callback)
        {
            Exception e = null;
            string data = string.Empty;
            if (_projectDictionary.ContainsKey(projectName) && _projectDictionary[projectName].ContainsKey(gestureName))
            {
                data = _projectDictionary[projectName][gestureName];
            }
            else
            {
                e = new Exception("No Gesture or project by that name");
            }

            if (callback != null)
            {
                callback(projectName, gestureName, data, e);
            }
        }

        public void GetAllProjects(GetAllProjectsCallback callback)
        {
            List<ProjectDetail> projects = new List<ProjectDetail>();
            foreach (string projectName in _projectDictionary.Keys)
            {
                ProjectDetail project = new ProjectDetail();
                project.ProjectName = projectName;
                project.GestureNames = new List<string>();
                foreach (string gesture in _projectDictionary[projectName].Keys)
                {
                }
            }
        }
    }
}
