using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Storage
{
    public class NETClientStorage : IDataStorage
    {
        private WebStorage _webStorage = new WebStorage();
        //private LocalFileStorage _localStorage = null;

        public void Login(string accountName)
        {
            _webStorage.Login(accountName);
        }

        #region IDataStorage Members

        public string AccountName
        {
            get { return _webStorage.AccountName; }
        }

        public void SaveGesture(string projectName, string gestureName, string value, SaveGestureCallback callback)
        {
            _webStorage.SaveGesture(projectName, gestureName, value, callback);
        }

        public void GetGesture(string projectName, string gestureName, GetGestureCallback callback)
        {
            _webStorage.GetGesture(projectName, gestureName, callback);
        }

        public void GetAllProjects(GetAllProjectsCallback callback)
        {
            _webStorage.GetAllProjects(callback);
        }

        public bool IsLoggedIn()
        {
            return _webStorage.IsLoggedIn();
        }

        public void Logout()
        {
            _webStorage.Logout();
        }

        #endregion
    }
}
