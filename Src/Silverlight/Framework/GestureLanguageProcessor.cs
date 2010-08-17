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
using TouchToolkit.Framework.Utility;
using System.Collections.Generic;

using TouchToolkit.GestureProcessor.Objects.LanguageTokens;
using TouchToolkit.GestureProcessor.ReturnTypes;
using TouchToolkit.GestureProcessor.PrimitiveConditions.Validators;
using TouchToolkit.GestureProcessor.Objects;
using TouchToolkit.GestureProcessor.PrimitiveConditions.Objects;
using System.Reflection;
using System.IO;

namespace TouchToolkit.Framework
{
    internal class GestureLanguageProcessor
    {
        //private static readonly string[] assembliesToSkip = { "BlueTools", "Conoto.net" };

        private static List<Type> _allRules = new List<Type>();
        private static List<Tuple<IPrimitiveConditionValidator, Gesture>> _preConGestureMap = new List<Tuple<IPrimitiveConditionValidator, Gesture>>();
        private static List<IPrimitiveConditionValidator> _rules = new List<IPrimitiveConditionValidator>();
        private static List<IPrimitiveConditionValidator> _preCons = new List<IPrimitiveConditionValidator>();

        private static List<Gesture> _gestures = new List<Gesture>();
        private static Dictionary<string, Gesture> activeGestures = new Dictionary<string, Gesture>();
        internal static Dictionary<string, Gesture> ActiveGestures
        {
            get { return activeGestures; }
        }

        private static List<IPrimitiveConditionValidator> activePreConditions = new List<IPrimitiveConditionValidator>();
        internal static List<IPrimitiveConditionValidator> ActivePreConditions
        {
            get { return activePreConditions; }
        }

        private static List<IPrimitiveConditionValidator> activeTouchPatterns = new List<IPrimitiveConditionValidator>();
        internal static List<IPrimitiveConditionValidator> ActiveTouchPatterns
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
                    if (t.IsTypeOf(typeof(IPrimitiveConditionValidator)))
                        _allRules.Add(t);
                }

            }

            return _allRules;
        }

        internal static void LoadGestureDefinitions()
        {
            // Loading both pre-defined gestures from framework assembly and user defined gestures (if any)
            List<GestureToken> gestureTokens = GetGestureTokens();

            // Load Gestures
            foreach (var gToken in gestureTokens)
            {
                // Name
                Gesture g = new Gesture() { Name = gToken.Name };

                // Validate blocks
                foreach (var validateToken in gToken.ValidateTokens)
                {
                    ValidationBlock vb = new ValidationBlock() { Name = validateToken.Name };

                    // Primitive conditions
                    foreach (var priConData in validateToken.PrimitiveConditions)
                    {
                        IPrimitiveConditionValidator primitiveCondition = GetPrimitiveConditionValidator(priConData);
                        vb.PrimitiveConditions.Add(primitiveCondition);
                    }

                    g.ValidationBlocks.Add(vb);
                }

                // Returns
                foreach (var retToken in gToken.Returns)
                {
                    ReturnTypeInfo info = GetReturnTypeInfo(retToken);
                    g.ReturnTypes.Add(info);
                }

                // If a gesture definition with same name already exists (probably from the pre-defined list)
                // then override that with the latest one (defined by the user)

                var existingGesture = _gestures.Get(gToken.Name);
                if (existingGesture != null)
                {
                    _gestures.Remove(existingGesture);
                }

                _gestures.Add(g);
            }
        }

        private static List<GestureToken> GetGestureTokens()
        {
            List<GestureToken> tokens = new List<GestureToken>();

            /* 
             * TODO: Temporary work around
             * 
             * Currently, we can only load custom gestures from the main-app assembly
             */

            /*load definitions from the framework assembly */
            FrameInfo objectFromFramework = new FrameInfo();
            List<GestureToken> preDefinedGestureTokens = ContentHelper.GetEmbeddedGestureDefinition(objectFromFramework.GetType().Assembly, "gestures.gx");
            if (preDefinedGestureTokens != null && preDefinedGestureTokens.Count > 0)
                tokens.AddRange(preDefinedGestureTokens);

            /*load definitions from the executing assembly (user's app for custom gestures)*/
            if (GestureFramework.HostAssembly != null)
            {
                List<GestureToken> userDefinedGestureTokens = ContentHelper.GetEmbeddedGestureDefinition(GestureFramework.HostAssembly, "gestures.gx");
                if (userDefinedGestureTokens != null && userDefinedGestureTokens.Count > 0)
                    tokens.AddRange(userDefinedGestureTokens);
            }
            /*
            // Go through all assemblies (both framework and client) and load the gesture definitions
            DirectoryInfo dirInfo = new DirectoryInfo(Environment.CurrentDirectory);
            var dlls = dirInfo.GetFiles("*.dll");
            var exes = dirInfo.GetFiles("*.exe");

            List<FileInfo> allAssemblies = new List<FileInfo>(dlls.Length + exes.Length);
            allAssemblies.AddRange(dlls);
            allAssemblies.AddRange(exes);

            // First: Load the pre-defined definitions from TouchToolkit.GestureProcessor.dll
            FileInfo frameworkDll = null;
            foreach (FileInfo assemblyFile in allAssemblies)
            {
                if (IsExternalAssembly(assemblyFile))
                    continue;

                if (assemblyFile.Name.Contains("TouchToolkit.GestureProcessor"))
                {
                    Assembly assembly = Assembly.LoadFile(assemblyFile.FullName);
                    List<GestureToken> gestureTokens = ContentHelper.GetEmbeddedGestureDefinition(assembly, "gestures.gx");

                    if (gestureTokens != null && gestureTokens.Count > 0)
                    {
                        tokens.AddRange(gestureTokens);
                    }

                    frameworkDll = assemblyFile;
                    break;
                }
            }

            // Then: Load the remaining gesture definitions
            foreach (var assemblyFile in allAssemblies)
            {
                if (IsExternalAssembly(assemblyFile))
                    continue;

                if (assemblyFile.Name.Contains("TouchToolkit.GestureProcessor") || assemblyFile.Name.Contains("TouchToolkit.Framework"))
                    continue; // we already processed the framework dlls

                Assembly assembly = Assembly.LoadFile(assemblyFile.FullName);
                List<GestureToken> gestureTokens = ContentHelper.GetEmbeddedGestureDefinition(assembly, "gestures.gx");

                if (gestureTokens != null && gestureTokens.Count > 0)
                {
                    tokens.AddRange(gestureTokens);
                }
            }
*/

            return tokens;
        }

        //private static bool IsExternalAssembly(FileInfo assemblyFile)
        //{
        //    foreach (var name in assembliesToSkip)
        //    {
        //        if (assemblyFile.FullName.Contains(name))
        //            return true;
        //    }

        //    return false;
        //}

        private static IPrimitiveConditionValidator GetPreCondition(IPrimitiveConditionData ruleData, Gesture gesture)
        {
            IPrimitiveConditionValidator preCondition = null;
            IPrimitiveConditionValidator newPreCondition = GetPrimitiveConditionValidator(ruleData);

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
            var map = Tuple.Create<IPrimitiveConditionValidator, Gesture>(preCondition, gesture);
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

        private static IPrimitiveConditionValidator GetPrimitiveConditionValidator(IPrimitiveConditionData data)
        {
            // Get the rule validator class name using the ruleObject name
            string className = data.GetType().Name + "Validator";


            IPrimitiveConditionValidator rule = GetInstanceByTypeName(className) as IPrimitiveConditionValidator;

            rule.Init(data);

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

        private static IPrimitiveConditionValidator GetInstanceByTypeName(string className)
        {
            Type type = GetType(className);
            return Activator.CreateInstance(type) as IPrimitiveConditionValidator;
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
