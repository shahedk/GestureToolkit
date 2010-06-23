using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Storage
{
    internal class LocalFileStorage : IDataStorage
    {
        #region IDataStorage Members

        public string UserName
        {
            get { throw new NotImplementedException(); }
        }

        public void SaveGesture(string projectName, string gestureName, string value, SaveGestureCallback callback)
        {
            throw new NotImplementedException();
        }

        public void GetGesture(string projectName, string gestureName, GetGestureCallback callback)
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
