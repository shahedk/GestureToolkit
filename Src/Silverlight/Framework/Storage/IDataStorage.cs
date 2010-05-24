using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Storage
{
    internal interface IDataStorage
    {
        string UserName { get; }

        void SaveGesture(string projectName, string gestureName, string value, SaveGestureCallback callback);

        void GetGesture(string projectName, string gestureName, GetGestureCallback callback);

        void GetAllProjects(GetAllProjectsCallback callback);

    }
}
