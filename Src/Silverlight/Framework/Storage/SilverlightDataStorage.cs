using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using TouchToolkit.Framework.Exceptions;

namespace TouchToolkit.Framework.Storage
{
    internal class SilverlightDataStorage : IDataStorage
    {
        WebStorage _storage = new WebStorage();
        static IsolatedStorageSettings _userSettings = IsolatedStorageSettings.ApplicationSettings;

        public string AccountName
        {
            get
            {
                if (_userSettings.Contains("accountName"))
                {
                    return (string)_userSettings["accountName"];
                }
                else
                {
                    return null;
                }
            }
            protected set
            {
                _userSettings["accountName"] = value;
                _userSettings.Save();
            }
        }

        public bool IsLoggedIn()
        {
            if (!string.IsNullOrEmpty(AccountName))
            {
                _storage.Login(AccountName);
                return true;
            }
            else
                return false;
        }

        public void Login(string accountName)
        {
            AccountName = accountName;
            _storage.Login(accountName);
        }

        public void Logout()
        {
            AccountName = string.Empty;
            _storage.Logout();
        }

        #region IDataStorage Members
        public void SaveGesture(string projectName, string gestureName, string value, SaveGestureCallback callback)
        {
            _storage.SaveGesture(projectName, gestureName, value, callback);
        }

        public void GetGesture(string projectName, string gestureName, GetGestureCallback callback)
        {
            _storage.GetGesture(projectName, gestureName, callback);
        }

        public void GetAllProjects(GetAllProjectsCallback callback)
        {
            _storage.GetAllProjects(callback);
        }

        #endregion

        #region ILocalStorage Members

        public void ValidateCache()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
