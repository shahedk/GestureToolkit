using System;
using TouchToolkit.GestureProcessor.ReturnTypes;
using TouchToolkit.GestureProcessor.Objects;

namespace LanguageParser.TestApp.Extenstions.Return_Types
{
    public class ReturnType1CalculatorCalculator : IReturnTypeCalculator
    {
        #region IReturnTypeCalculator Members

        public IReturnType Calculate(ValidSetOfTouchPoints set)
        {
            return new ReturnType1();
        }

        #endregion
    }
}
