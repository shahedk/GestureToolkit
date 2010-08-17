using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouchToolkit.GestureProcessor.Objects
{
    /// <summary>
    /// TODO: Store the data in a queue or maintain an index sorted by timestamp... this will help improve performance
    /// of the cleanup process
    /// </summary>
    public class PartiallyEvaluatedGestures
    {
        private static List<ValidateBlockResult> _cache = new List<ValidateBlockResult>();

        public static void Add(string gestureName, int blockNo, ValidSetOfPointsCollection data, string blockName)
        {
            var item = new ValidateBlockResult()
            {
                Data = data,
                GestureName = gestureName,
                ValidateBlockNo = blockNo,
                ValidateBlockName = blockName
            };

            _cache.Add(item);
        }

        public static List<ValidateBlockResult> Get(string gestureName, int blockNo)
        {
            var results = _cache.Where(x => x.GestureName == gestureName && x.ValidateBlockNo == blockNo).ToList<ValidateBlockResult>();

            return results;
        }

        public static List<ValidateBlockResult> Get(string gestureName, string blockName)
        {
            var results = _cache.Where(x => x.GestureName == gestureName && x.ValidateBlockName == blockName).ToList<ValidateBlockResult>();

            return results;
        }

        public static ValidateBlockResult[] GetAll()
        {
            ValidateBlockResult[] list = new ValidateBlockResult[_cache.Count];
            _cache.CopyTo(list);

            return list;
        }

        /// <summary>
        /// Removes the item and any other results listed in item.AssociatedResults
        /// </summary>
        /// <param name="item"></param>
        public static void Remove(ValidateBlockResult item)
        {
            /*
            foreach (var id in item.AssociatedResults)
            {
                var relatedResult = _cache.SingleOrDefault(x => x.Id == id);
                Remove(relatedResult);
            }

            _cache.Remove(item);
             */

            //TODO: Should follow the above logic. The following code is only for testing another feature
            _cache.RemoveAll(x => x.GestureName == item.GestureName);
        }
    }
}
