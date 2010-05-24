using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Storage
{
    public class NETClientStorage : IDataStorage
    {
        private WebStorage _webStorage = null;
        //private LocalFileStorage _localStorage = null;

        private string _userName = string.Empty;
        public NETClientStorage(string userName)
        {
            _userName = userName;

            _webStorage = new WebStorage(userName);
        }



        #region IDataStorage Members

        public string UserName
        {
            get { return _userName; }
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

        #endregion
    }
}
