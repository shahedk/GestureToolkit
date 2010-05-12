using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Storage
{
    internal abstract class DataStorage
    {
#if SILVERLIGHT
        public static DataStorage Instance = new SilverlightDataStorage();
#else
        public static DataStorage Instance = new LocalFileStorage();
#endif


    }
}
