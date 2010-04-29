using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization.Json;
using Framework.Utility;
using System.Collections.Generic;

using Gestures.Objects.LanguageTokens;
using Gestures.ReturnTypes;
using Gestures.Rules.RuleValidators;
using Gestures.Objects;
using Gestures.Rules.Objects;

namespace Framework
{
    internal class GestureLanguageProcessor
    {
        private static List<Type> _allRules = new List<Type>();
        private static List<Tuple<IRuleValidator, Gesture>> _preConGestureMap = new List<Tuple<IRuleValidator, Gesture>>();
        private static List<IRuleValidator> _rules = new List<IRuleValidator>();
        private static List<IRuleValidator> _preCons = new List<IRuleValidator>();

        private static List<Gesture> _gestures = new List<Gesture>();
        private static Dictionary<string, Gesture> activeGestures = new Dictionary<string, Gesture>();
        internal static Dictionary<string, Gesture> ActiveGestures
        {
            get { return activeGestures; }
        }

        private static List<IRuleValidator> activePreConditions = new List<IRuleValidator>();
        internal static List<IRuleValidator> ActivePreConditions
        {
            get { return activePreConditions; }
        }

        private static List<IRuleValidator> activeTouchPatterns = new List<IRuleValidator>();
        internal static List<IRuleValidator> ActiveTouchPatterns
        {
            get { return activeTouchPatterns; }
        }

        public static Gesture GetGesture(string name)
        {
            if (activeGestures.ContainsKey(name))
                return activeGestures[name];
            else
            {
                Gesture gesture = _gestures.Get(name);

                if (gesture != null)
                    activeGestures.Add(name, gesture);

                return gesture;
            }
        }

        /// <summary>
        /// Returns all unique rules defined in the "Gestures" assembly
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetAllRules()
        {
            if (_allRules.Count == 0)
            {
                // Find all classes in "Gestures" assembly that implements IRuleValidator interface
                Gesture g = new Gesture();
                var types = g.GetType().Assembly.GetTypes();
                foreach (Type t in types)
                {
                    if (t.IsTypeOf(typeof(IRuleValidator)))
                        _allRules.Add(t);
                }

            }

            return _allRules;
        }

        internal static void LoadGestureDefinitions()
        {
            // TODO: for test only, find a better way to get current assembly reference 
            Position dummy = new Position();

            // Get the gesture definition from embeded resources
            List<GestureToken> gestureTokens = ContentHelper.GetEmbeddedGestureDefinition(dummy, "gestures.gx");

            // Load Gestures
            foreach (var gToken in gestureTokens)
            {
                // Name
                Gesture g = new Gesture() { Name = gToken.Name };

                // PreConditions
                foreach (IRuleData ruleData in gToken.PreConditions)
                {
                    IRuleValidator preConRule = GetPreCondition(ruleData, g);
                    g.PreConditions.Add(preConRule);
                }

                // Conditions
                foreach (var ruleData in gToken.Conditions)
                {
                    IRuleValidator conRule = GetRule(ruleData);
                    g.Rules.Add(conRule);
                }

                // Returns
                foreach (var retToken in gToken.Returns)
                {
                    ReturnTypeInfo info = GetReturnTypeInfo(retToken);
                    g.ReturnTypes.Add(info);
                }

                _gestures.Add(g);
            }
        }

        private static IRuleValidator GetPreCondition(IRuleData ruleData, Gesture gesture)
        {
            IRuleValidator preCondition = null;
            IRuleValidator newPreCondition = GetRule(ruleData);

            // Check if same preCondition already exists
            foreach (var rule in _preCons)
            {
                if (newPreCondition.Equals(rule))
                {
                    preCondition = rule;
                    break;
                }
            }
            // Update preCondition to gesture mapping
            if (preCondition == null)
                preCondition = newPreCondition;
            var map = Tuple.Create<IRuleValidator, Gesture>(preCondition, gesture);
            _preConGestureMap.Add(map);

            return preCondition;
        }

        /// <summary>
        /// Creates object from gesture assembly using reflection
        /// </summary>
        /// <param name="retToken"></param>
        /// <returns></returns>
        private static ReturnTypeInfo GetReturnTypeInfo(ReturnToken retToken)
        {
            string className = retToken.Name.Replace(" ", string.Empty);

            //TODO: temp work around for 'Info' return type. This is the only return type that 
            // can carry parameters
            string infoMsg = string.Empty;
            if (className.StartsWith("Info:"))
            {
                string[] tokens = className.Split(":".ToCharArray());
                if (tokens.Length == 2)
                {
                    className = tokens[0];
                    infoMsg = tokens[1];
                }
            }

            string calculatorClassName = className + "Calculator";
            ReturnTypeInfo info = new ReturnTypeInfo()
            {
                ReturnType = GetType(className),
                CalculatorType = GetType(calculatorClassName),
                AdditionalInfo = infoMsg
            };

            return info;
        }

        private static IRuleValidator GetRule(IRuleData ruleData)
        {
            // Get the rule validator class name using the ruleObject name
            string className = ruleData.GetType().Name + "Validator";


            IRuleValidator rule = GetInstanceByTypeName(className) as IRuleValidator;
            rule.Init(ruleData);

            bool matchFound = false;
            foreach (var r in _rules)
            {

                //TODO: temp work-around... since the 'Equals' method is not yet implemented for all rules
                break;

                if (rule.Equals(r))
                {
                    rule = r;
                    matchFound = true;
                    break;
                }
            }

            if (!matchFound)
            {
                _rules.Add(rule);
            }

            return rule;
        }

        private static IRuleValidator GetInstanceByTypeName(string className)
        {
            Type type = GetType(className);

            return Activator.CreateInstance(type) as IRuleValidator;
        }

        private static Type GetType(string className)
        {
            Type type = null;
            Gesture g = new Gesture();
            var types = g.GetType().Assembly.GetTypes();
            foreach (var t in types)
            {
                if (t.Name.ToLower() == className.ToLower())
                {
                    type = t;
                    break;
                }
            }

            return type;
        }
    }
}
