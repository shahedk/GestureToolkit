using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchToolkit.Framework.Storage;
using System.IO;
using System.IO.IsolatedStorage;

namespace TouchToolkit.Framework.Storage
{
    public class NETClientStorage : IDataStorage
    {
        GestureDictionary _localCache = new GestureDictionary();
        WebStorage _webStorage = new WebStorage();

        private string _accountName;

        private bool _firstTime = true;
        private bool _online = false;

        public string AccountName
        {
            get
            {
                if (_accountName != string.Empty)
                {
                    return _accountName;
                }
                else
                {
                    return string.Empty;
                }
            }
            protected set
            {
                _accountName = value;
            }
        }

        private string _filename
        {
            get
            {
                return AccountName + ".dat";
            }
        }

        public bool IsLoggedIn()
        {
            if (!string.IsNullOrEmpty(AccountName))
            {
                if (_online)
                {
                    _webStorage.Login(AccountName);
                }
                return true;
            }
            else
                return false;
        }

        public void Login(string accountName)
        {
            AccountName = accountName;
            if (_online)
            {
                _webStorage.Login(accountName);
            }
            else
            {
                LoadFromFile(_filename);
            }
            _firstTime = true;
        }

        public void Logout()
        {
            SaveToFile(_filename);
            AccountName = string.Empty;
            if (_online)
            {
                _webStorage.Logout();
            }
        }

        public void ToggleOnlineMode()
        {
            _online = !_online;
        }
        
        public bool IsOnline()
        {
            return _online;
        }

        #region IDataStorage Members
        public void SaveGesture(string projectName, string gestureName, string value, SaveGestureCallback callback)
        {
            //Save to local cache
            _localCache.Add(projectName, gestureName, value);
            //Save to permanent storage
            if (_online)
            {
                _webStorage.SaveGesture(projectName, gestureName, value, callback);
            }
            else
            {
                SaveToFile(_filename);
                callback(gestureName);
            }
        }

        public void GetGesture(string projectName, string gestureName, GetGestureCallback callback)
        {
            Exception e = null;
            string data = string.Empty;
            if (_localCache.Contains(projectName, gestureName))
            {
                data = _localCache.Get(projectName, gestureName);
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
            if (_firstTime)
            {
                if (_online)
                {
                    _webStorage.GetAllProjects((projectDetails, error) =>
                    {
                        if (error == null)
                        {
                            LoadDictionary(projectDetails);
                            callback(projectDetails);
                        }
                    });
                }
                else
                {
                    LoadFromFile(_filename);
                    callback(_localCache.ProjectDetails);
                }
                _firstTime = false;
            }
            else
            {
                callback(_localCache.ProjectDetails);
            }
        }
        #endregion

        #region ILocalStorage Members

        public void ValidateCache()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region FileStorage Members
        public void LoadFromFile(string filename)
        {
            _localCache = new GestureDictionary();
            try
            {
                FileStream file = new FileStream(filename, FileMode.Open);
                TextReader reader = new StreamReader(file);
                string next = reader.ReadLine();
                while (next != null)
                {
                    string projectName = next;
                    string gestureName = reader.ReadLine();
                    string data = reader.ReadLine();
                    _localCache.Add(projectName, gestureName, data);
                    next = reader.ReadLine();
                }

                reader.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
            }
        }

        public void SaveToFile(string filename)
        {
            FileStream file = new FileStream(filename, FileMode.Create);
            TextWriter writer = new StreamWriter(file);
            List<ProjectDetail> details = _localCache.ProjectDetails;
            foreach (var detail in details)
            {
                string project = detail.ProjectName;
                foreach (var gesture in detail.GestureNames)
                {
                    writer.WriteLine(project);
                    writer.WriteLine(gesture);
                    writer.WriteLine(_localCache.Get(project, gesture));
                }
            }
            writer.Flush();
            writer.Close();
        }
        #endregion

        private void LoadDictionary(List<ProjectDetail> details)
        {
            foreach (var detail in details)
            {
                string projectName = detail.ProjectName;
                foreach (var gestureName in detail.GestureNames)
                {
                    _webStorage.GetGesture(projectName, gestureName, (projName, gesName, data, error) =>
                    {
                        _localCache.Add(projName, gesName, data);
                    });
                }
            }
        }
    }
}
