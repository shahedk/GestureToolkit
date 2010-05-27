using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.M;
using System.IO;
using System.Dataflow;
using MGraphXamlReader;
using System.Runtime.Serialization.Json;
using Gestures.Objects.LanguageTokens;
using Gestures.Rules.Objects;

using System.Threading;


namespace LanguageParser
{
    class Program
    {
        private static string RuleNamesFilePath { get; set; }
        private static string CompiledGestureDefPath { get; set; }
        private static string GestureDefSourcePath { get; set; }
        private static string LanguageDefPath { get; set; }
        private static string _language = "";

        static void Main(string[] args)
        {
            if (args.Length == 4)
            {
                CompiledGestureDefPath = args[0];
                RuleNamesFilePath = args[1];
                GestureDefSourcePath = args[2];
                LanguageDefPath = args[3];

                // Display arg values
                WriteMessage("CompiledGestureDefPath:");
                WriteMessage(CompiledGestureDefPath + Environment.NewLine);

                WriteMessage("RuleNamesFilePath:");
                WriteMessage(RuleNamesFilePath + Environment.NewLine);

                WriteMessage("GestureDefSourcePath:");
                WriteMessage(GestureDefSourcePath + Environment.NewLine);

                WriteMessage("LanguageDefPath:");
                WriteMessage(LanguageDefPath + Environment.NewLine);
            }
            else if (args.Length == 0)
            {
                WriteMessage("No args provided. Using default paths!");

                CompiledGestureDefPath = @"../../../../Silverlight/Gestures/Bin/gestures.gx";
                RuleNamesFilePath = @"../../../../Silverlight/Gestures/Bin/rulenames.gx";
                GestureDefSourcePath = @"../../../../Silverlight/Gestures/Gesture Definitions";
                LanguageDefPath = "../GDL.mg";
            }
            else
            {
                WriteMessage("Invalid args!");
                return;
            }

            // Load language grammar
            WriteMessage("Loading language grammar...");
            _language = File.ReadAllText(LanguageDefPath);

            // Build parser
            WriteMessage("Building parser from language...");
            Parser parser = GetParser();
            List<string> ruleNames = GetRuleNames(parser);

            // Build the dictionary with known types: all rule & return types
            WriteMessage("Building list of rule types...");
            IEnumerable<Type> ruleTypes = GetAllRuleDataTypes();

            var map = new Dictionary<Identifier, Type>();

            // Adding all types that implements 'IRuleData'
            foreach (var type in ruleTypes)
            {
                map.Add(type.Name, type);
            }

            // Adding additional types
            map.Add("Return", typeof(ReturnToken));
            map.Add("Gesture", typeof(GestureToken));
            //map.Add("States", typeof(List<string>));

            WriteMessage("Compiling gestures (*.g files) ...");
            List<GestureToken> gestures = new List<GestureToken>();
            DirectoryInfo gestureFolder = new DirectoryInfo(GestureDefSourcePath);
            FileInfo[] files = gestureFolder.GetFiles("*.g");
            foreach (var file in files)
            {
                string data = File.ReadAllText(file.FullName);

                // Parse
                var list = parser.Parse<List<object>>(data, map);
                foreach (GestureToken g in list)
                {
                    gestures.Add(g);
                }
            }

            WriteMessage("Updating deployment folder...");

            UpdateGestureDefFile(gestures);
            UpdateRuleNamesList(ruleNames);

            WriteMessage("Done.");

#if DEBUG
            // Let users read the console message
            Thread.Sleep(1000);
#endif
        }

        private static Parser GetParser()
        {
            using (var options = new CompilerOptions())
            {
                var sourceItems = new SourceItem[]{

                    new TextItem()
                    {
                        Name = "GdlGrammer", ContentType = TextItemType.MGrammar, Reader = new StringReader(_language)
                    }
                };

                options.AddSourceItems(sourceItems);

                CompilationResults results = Compiler.Compile(options);
                if (results.HasErrors)
                {
                    //TODO: show meaningful error message
                    throw new Exception("Failed to compile GDL ....");
                }
                else
                {
                    foreach (var parserFactory in results.ParserFactories)
                    {
                        //TODO: Why inside foreach loop!!!
                        return parserFactory.Value.Create();
                    }
                }
            }

            return null;
        }

        private static void WriteMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        #region Get the list of rules from assembly

        private static List<string> GetRuleNames(Parser parser)
        {
            List<string> ruleNames = new List<string>();
            foreach (var productionInfo in parser.ProductionTable)
            {
                // Get the rule name from production table
                if (productionInfo.Description.StartsWith("Rule ="))
                {
                    // Remove the prefix "rule =" from the description
                    string ruleName = productionInfo.Description.Substring(6).Trim();

                    ruleNames.Add(ruleName);
                }
            }

            return ruleNames;
        }

        private static IEnumerable<Type> GetAllRuleDataTypes()
        {
            List<Type> requiredTypes = new List<Type>();

            Type[] types = GetAllTypesInGestureAssembly();

            foreach (var type in types)
            {
                Type[] interfaces = type.GetInterfaces();
                foreach (var i in interfaces)
                {
                    if (i.IsAssignableFrom(typeof(IRuleData)))
                        requiredTypes.Add(type);
                }
            }

            return requiredTypes;
        }

        private static Type[] GetAllTypesInGestureAssembly()
        {
            GestureToken dummyObject = new GestureToken();
            Type t = dummyObject.GetType();

            return t.Assembly.GetTypes();
        }
        #endregion

        #region Update resources for executables
        private static void UpdateRuleNamesList(List<string> ruleNames)
        {
            string contentToWrite = string.Join("|", ruleNames);

            File.WriteAllText(RuleNamesFilePath, contentToWrite);
        }

        private static void UpdateGestureDefFile(List<GestureToken> gestures)
        {
            IEnumerable<Type> knownTypes = GetAllRuleDataTypes();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<GestureToken>), knownTypes);

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, gestures);

                ms.Position = 0;
                StreamReader sr = new StreamReader(ms);
                string json = sr.ReadToEnd();

                File.WriteAllText(CompiledGestureDefPath, json);
            }
        }
        #endregion
    }
}
