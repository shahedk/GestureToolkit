using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Storage
{
    internal class SilverlightIsoStorage : IDataStorage
    {
        private string _userName = string.Empty;
        public SilverlightIsoStorage(string userName)
        {
            _userName = userName;
        }

        #region IDataStorage Members

        public string UserName
        {
            get
            {
                return _userName;
            }
        }

        public void SaveGesture(string projectName, string gestureName, string value, SaveGestureCallback callback)
        {
            throw new NotImplementedException();
        }

        public void GetGesture(string projectName, string gestureName, GetGestureCallback callback)
        {
            throw new NotImplementedException();
        }

        public void GetGestureNames(string projectName, GetGestureNamesCallback callback)
        {
            throw new NotImplementedException();
        }

        public void GetAllProjects(GetAllProjectsCallback callback)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
