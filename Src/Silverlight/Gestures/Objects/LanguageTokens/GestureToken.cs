using System.Collections.Generic;
using TouchToolkit.GestureProcessor.PrimitiveConditions.Objects;


namespace TouchToolkit.GestureProcessor.Objects.LanguageTokens
{
    public class GestureToken : LanguageToken
    {
        public List<IPrimitiveConditionData> _preConditions = new List<IPrimitiveConditionData>();
        public List<IPrimitiveConditionData> PreConditions
        {
            get
            {
                return _preConditions;
            }
            set
            {
                _preConditions = value;
            }
        }

        public List<IPrimitiveConditionData> _rules = new List<IPrimitiveConditionData>();
        public List<IPrimitiveConditionData> Conditions
        {
            get
            {
                return _rules;
            }
            set
            {
                _rules = value;
            }
        }

        public List<ReturnToken> _returns = new List<ReturnToken>();
        public List<ReturnToken> Returns
        {
            get
            {
                return _returns;
            }
            set
            {
                _returns = value;
            }
        }
    }
}