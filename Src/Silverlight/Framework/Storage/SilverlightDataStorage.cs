using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using Framework.Exceptions;

namespace Framework.Storage
{
    internal class SilverlightDataStorage : DataStorage
    {
        IsolatedStorageSettings _userSettings = IsolatedStorageSettings.ApplicationSettings;

        public override void Save(string key, object value)
        {
            throw new NotImplementedException();
        }

        public override T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public override void SaveFile(string fileName, string content)
        {
            _userSettings.Add(gestureDataKey, data);
            if (CheckIsolatedStorageAvaliableSpace(data))
                _userSettings.Save();

        }

        public override string GetFile(string fileName)
        {
            throw new NotImplementedException();
        }

        private bool CheckIsolatedStorageAvaliableSpace(string data)
        {
            Int64 requiredSpace = data.Count() * 2;
            Int64 fiveMB = 1024 * 1024 * 5;
            bool isEnoughSpaceAvailable = false;

            IsolatedStorageFile storage = null;
            try
            {
                storage = IsolatedStorageFile.GetUserStoreForApplication();
                if (storage.AvailableFreeSpace < requiredSpace)
                {
                    long newQuota = storage.Quota + fiveMB;
                    isEnoughSpaceAvailable = storage.IncreaseQuotaTo(newQuota);

                    if (!isEnoughSpaceAvailable)
                    {
                        throw new FrameworkException("Failed to save content in local cache due to space limitation!");
                    }
                }
                else
                {
                    isEnoughSpaceAvailable = true;
                }
            }
            catch
            {
                //TODO: handle possible error
            }

            return isEnoughSpaceAvailable;
        }
    }
}
