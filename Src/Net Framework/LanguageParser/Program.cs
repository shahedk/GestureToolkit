using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.M;
using System.IO;
using System.Dataflow;
using MGraphXamlReader;
using System.Runtime.Serialization.Json;
using TouchToolkit.GestureProcessor.Objects.LanguageTokens;
using TouchToolkit.GestureProcessor.Rules.Objects;

using System.Threading;
using System.Reflection;
using TouchToolkit.Framework;


namespace LanguageParser
{
    class Program
    {
        private static readonly string[] AssembliesToSkip = { "MGraphXamlReader", "Microsoft.Build.Framework", "Microsoft.M", "PresentationCore", "PresentationFramework", "System", "System.Core", "System.Data", "System.Data.DataSetExtensions", "System.Dataflow", "System.Drawing", "System.Runtime.Serialization", "System.Windows.Forms", "System.Xml", "System.Xml.Linq", "WindowsBase", "Xaml", "LanguageParser", "TouchToolkit.Framework", "TouchToolkit.GestureProcessor" };

        private static string RuleNamesFilePath { get; set; }
        private static string CompiledGestureDefPath { get; set; }
        private static string GestureDefSourcePath { get; set; }
        private static string LanguageDefPath { get; set; }
        private static string _language = "";
        private static string AppBinPath { get; set; }

        static void Main(string[] args)
        {
            if (args.Length == 5)
            {
                CompiledGestureDefPath = args[0];
                RuleNamesFilePath = args[1];
                GestureDefSourcePath = args[2];
                LanguageDefPath = args[3];
                LanguageDefPath = args[4];

                // Display arg values
                WriteMessage("CompiledGestureDefPath:");
                WriteMessage(CompiledGestureDefPath + Environment.NewLine);

                WriteMessage("RuleNamesFilePath:");
                WriteMessage(RuleNamesFilePath + Environment.NewLine);

                WriteMessage("GestureDefSourcePath:");
                WriteMessage(GestureDefSourcePath + Environment.NewLine);

                WriteMessage("LanguageDefPath:");
                WriteMessage(LanguageDefPath + Environment.NewLine);

                WriteMessage("Local Application Bin Path:");
                WriteMessage(AppBinPath + Environment.NewLine);
            }
            else if (args.Length == 0)
            {
                WriteMessage("No args provided. Using default paths!");

                CompiledGestureDefPath = @"../../../../Silverlight/Gestures/Bin/gestures.gx";
                RuleNamesFilePath = @"../../../../Silverlight/Gestures/Bin/rulenames.gx";
                GestureDefSourcePath = @"../../../../Silverlight/Gestures/Gesture Definitions";
                LanguageDefPath = "../GDL.mg";
                AppBinPath = @"../../../../Net Framework/TestApplication/Bin/Debug/";
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

            Type[] types = GetAllTypes();
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

        private static Type[] GetAllTypes()
        {

            List<Type> types = new List<Type>();
            DirectoryInfo dirInfo = new DirectoryInfo(AppBinPath);

            // Scan all user defined dlls
            if (dirInfo.Exists)
            {
                FileInfo[] assemblyFiles = dirInfo.GetFiles("*.dll");
                foreach (var assemblyFile in assemblyFiles)
                {
                    string assName = assemblyFile.Name.Replace(".dll", string.Empty);
                    if (AssembliesToSkip.Contains(assName))
                        continue;

                    Assembly assembly = Assembly.LoadFile(assemblyFile.FullName);
                    types.AddRange(assembly.GetTypes());
                }
            }

            // Scan framework dlls
            Assembly framework = typeof(GestureFramework).Assembly;
            types.AddRange(framework.GetTypes());
            
            Assembly gestureProcessor = typeof(IRuleData).Assembly;
            types.AddRange(gestureProcessor.GetTypes());

            return types.ToArray();
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
