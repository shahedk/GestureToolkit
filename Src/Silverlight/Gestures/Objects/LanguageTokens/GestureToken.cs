using System.Collections.Generic;
using TouchToolkit.GestureProcessor.Rules.Objects;


namespace TouchToolkit.GestureProcessor.Objects.LanguageTokens
{
    public class GestureToken : LanguageToken
    {
        public List<IRuleData> _preConditions = new List<IRuleData>();
        public List<IRuleData> PreConditions
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

        public List<IRuleData> _rules = new List<IRuleData>();
        public List<IRuleData> Conditions
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