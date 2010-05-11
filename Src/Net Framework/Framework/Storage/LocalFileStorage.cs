using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Storage
{
    internal class LocalFileStorage : DataStorage
    {
        /// <summary>
        /// Inserts new or updates existing records
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void Save(string key, object value)
        {

        }

        public override T Get<T>(string key)
        {
            throw new Exception();
        }

        public override void SaveFile(string fileName, string content)
        {

        }

        public override string GetFile(string fileName)
        {
            return string.Empty;
        }
    }
}
