using System;
using TouchToolkit.GestureProcessor.PrimitiveConditions.Objects;

namespace LanguageParser.TestApp.Extensions.Primitive_Conditions
{
    public class Rule1 : IPrimitiveConditionData
    {
        #region IPrimitiveConditionData Members

        public bool Equals(IPrimitiveConditionData value)
        {
            throw new NotImplementedException();
        }

        public void Union(IPrimitiveConditionData value)
        {
            throw new NotImplementedException();
        }

        public string ToGDL()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}