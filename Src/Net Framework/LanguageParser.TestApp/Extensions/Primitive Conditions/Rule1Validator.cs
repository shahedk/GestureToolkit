using System;
using System.Collections.Generic;
using TouchToolkit.GestureProcessor.Objects;
using TouchToolkit.GestureProcessor.PrimitiveConditions.Validators;

namespace LanguageParser.TestApp.Extensions.Primitive_Conditions
{
    public class Rule1ValidatorValidator : IPrimitiveConditionValidator
    {
        public void Init(TouchToolkit.GestureProcessor.PrimitiveConditions.Objects.IPrimitiveConditionData ruleData)
        {
            
        }

        public bool Equals(IPrimitiveConditionValidator rule)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }

        public ValidSetOfPointsCollection Validate(ValidSetOfPointsCollection sets)
        {
            throw new NotImplementedException();
        }

        public TouchToolkit.GestureProcessor.PrimitiveConditions.Objects.IPrimitiveConditionData GenerateRuleData(List<TouchPoint2> points)
        {
            throw new NotImplementedException();
        }
    }
}
