using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Framework.Storage
{
    public sealed class StorageManager : IDataStorage
    {
        private IDataStorage _storage = null;

        public StorageManager(string userName)
        {

#if SILVERLIGHT
          //  _storage = new SilverlightDataStorage(userName);
#elif XNA
            _storage = new XNAClientStorage(userName);
#else
            _storage = new NETClientStorage(userName);
#endif

        }





        #region IDataStorage Members

        public string UserName
        {
            get { return _storage.UserName; }
        }

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
    }
}
