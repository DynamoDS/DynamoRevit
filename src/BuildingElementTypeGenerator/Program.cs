using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace BuildingElementTypeGenerator
{
    internal class MethodData
    {
        public List<string> ParamNames { get; set; }
        public List<string> ParamDescriptions { get; set; }
        public string Summary { get; set; }
        public List<string> ParamTypes { get; set; }

        public string MethodName
        {
            get
            {
                var methodName = "By";
                foreach (var p in ParamNames)
                {
                    // Document is available to every element
                    if (p == "Document") continue;
                    methodName += p;
                }
                return methodName;
            }
        }

        public MethodData(string summary = "No summary provided")
        {
            ParamTypes = new List<string>();
            ParamNames = new List<string>();
            ParamDescriptions = new List<string>();
            Summary = summary;
        }
    }

    class Program
    {
        private static readonly Dictionary<string, string> TypeMap = new Dictionary<string, string>()
        {
            {"XYZ", "Vector"},
            {"Double", "Number"},
            {"Int32", "Int"},
        };

        static void Main(string[] args)
        {
            // assembly path is the first argument
            if (args.Length < 1)
            {
                Console.WriteLine("You must supply the path to the RevitAPI.dll.");
                Console.ReadKey();
                return;
            }

            var apiPath = args[0];
            if (!File.Exists(apiPath))
            {
                Console.WriteLine("The specified path to the Revit API is invalid.");
                Console.ReadKey();
                return;
            }

            var doc = new XmlDocument();
            doc.Load(apiPath);

            var members = doc.GetElementsByTagName("member");

            if (members.Count == 0)
            {
                Console.WriteLine("No types could be found.");
                Console.ReadKey();
            }

            var memberData = new Dictionary<string, List<MethodData>>();

            foreach (XmlElement member in members)
            {
                var signature = member.Attributes["name"].Value;
                
                if (!signature.Contains("M:")) continue;
                if (!signature.Contains(".Create(")) continue;
                var children = member.ChildNodes;

                // Create the new method name
                var typeName = GetSimpleTypeNameFromCreateSignature(signature);

                var methodData = new MethodData();

                foreach (XmlElement child in children)
                {
                    switch (child.Name)
                    {
                        case "summary":
                            methodData.Summary = child.InnerText.Clean();
                            break;
                        case "param":
                            methodData.ParamNames.Add(child.Attributes["name"].Value.Clean());
                            methodData.ParamDescriptions.Add(child.InnerText.Clean());
                            break;
                    }
                }

                var parameters = GetParametersFromCreateSignature(signature).Select(GetMemberNameWithoutNamespace);

                foreach (var p in parameters)
                {
                    var cleanTypeName = p.Replace("}", "s");
                    methodData.ParamTypes.Add(TypeMap.ContainsKey(cleanTypeName) ? TypeMap[p] : cleanTypeName);
                }

                if (!memberData.ContainsKey(typeName))
                {
                    memberData.Add(typeName, new List<MethodData> {methodData});
                }
                else
                {
                    memberData[typeName].Add(methodData);
                }
                
            }

            Console.WriteLine("There are {0} available types", memberData.Count);

            foreach (var kvp in memberData)
            {
                Debug.WriteLine(kvp.Key);
                foreach (var methodData in kvp.Value)
                {
                    Debug.WriteLine("\t" + methodData.MethodName + " : " + methodData.Summary);
                }
            }
        }

        private static string GetSimpleTypeNameFromCreateSignature(string signature)
        {
            var methodNameParts = signature.Split('(').First().Split('.');
            var typeName = methodNameParts[methodNameParts.Length - 2];
            return typeName;
        }

        private static string[] GetParametersFromCreateSignature(string signature)
        {
            if (!signature.Contains("(") || !signature.Contains(")")) return new string[]{};

            var paramsStart = signature.IndexOf("(");
            var paramsEnd = signature.LastIndexOf(")");
            var paramsLength = paramsEnd - paramsStart;
            return signature.Substring(paramsStart, paramsLength).
                Split(',').
                Select(s=>s.Replace("(","").Replace(")","")).
                ToArray();
        }

        private static string GetMemberNameWithoutNamespace(string memberName)
        {
            return memberName.Split('.').Last();
        }
    }

    internal static class StringExtensions
    {
        internal static string Clean(this string target)
        {
            return target.Replace("\\r\\n", "").Trim();
        }
    }
}
