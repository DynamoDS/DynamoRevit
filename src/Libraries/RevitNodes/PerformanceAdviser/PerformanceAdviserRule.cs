using System;
using System.Collections.Generic;
using Autodesk.DesignScript.Runtime;
using Autodesk.Revit.DB;
using RevitServices.Persistence;
using RevitServices.Transactions;
using Revit.Elements;

namespace Revit.PerformanceAdviser
{
    /// <summary>
    /// Performance Adviser Rule
    /// </summary>
    public class PerformanceAdviserRule
    {
        internal Autodesk.Revit.DB.PerformanceAdviserRuleId InternalId
        { 
            get; set;
        }

        [SupressImportIntoVM]
        public PerformanceAdviserRule(Autodesk.Revit.DB.PerformanceAdviserRuleId id)
        {
            this.InternalId = id;
        }

        /// <summary>
        /// The name of the rule.
        /// </summary>
        public string Name
        {
            get
            {
                Autodesk.Revit.DB.PerformanceAdviser adviser = Autodesk.Revit.DB.PerformanceAdviser.GetPerformanceAdviser();
                return adviser.GetRuleName(this.InternalId);
            }
        }

        /// <summary>
        /// The description of the rule.
        /// </summary>
        public string Description
        {
            get
            {
                Autodesk.Revit.DB.PerformanceAdviser adviser = Autodesk.Revit.DB.PerformanceAdviser.GetPerformanceAdviser();
                return adviser.GetRuleDescription(this.InternalId);
            }
        }

        /// <summary>
        /// A boolean indicating if the rule is activated.
        /// </summary>
        public bool Enabled
        {
            get
            {
                Autodesk.Revit.DB.PerformanceAdviser adviser = Autodesk.Revit.DB.PerformanceAdviser.GetPerformanceAdviser();
                // Check if the rule is activated
                return adviser.IsRuleEnabled(this.InternalId);
            }
        }

        /// <summary>
        /// A boolean indicating if the rule is activated successfully.
        /// </summary>
        public bool SetEnabled
        {
            set
            {
                Autodesk.Revit.DB.PerformanceAdviser adviser = Autodesk.Revit.DB.PerformanceAdviser.GetPerformanceAdviser();
                // Activate or deactivate the rule
                adviser.SetRuleEnabled(this.InternalId, value);
            }
        }

        /// <summary>
        /// The id of the rule.
        /// </summary>
        public Guid RuleId
        {
            get
            {
                return this.InternalId.Guid;
            }
        }

        /// <summary>
        /// Execute Rules
        /// </summary>
        public static IList<FailureMessage> Execute(IEnumerable<PerformanceAdviserRule> rules)
        {
            List<FailureMessage> messages = new List<FailureMessage>();


            var document = DocumentManager.Instance.CurrentDBDocument;
            TransactionManager.Instance.EnsureInTransaction(document);

            Autodesk.Revit.DB.PerformanceAdviser adviser = Autodesk.Revit.DB.PerformanceAdviser.GetPerformanceAdviser();
            IList<PerformanceAdviserRuleId> rulesIdsToExecute = new List<PerformanceAdviserRuleId>();

            foreach (var rule in rules) rulesIdsToExecute.Add(new PerformanceAdviserRuleId(rule.RuleId));

            IList<Autodesk.Revit.DB.FailureMessage> failureMessages = adviser.ExecuteRules(document, rulesIdsToExecute);

            foreach (var message in failureMessages)
            {
                messages.Add(new FailureMessage(message));
            }

            TransactionManager.Instance.TransactionTaskDone();


            return messages;
        }

        /// <summary>
        /// Create Performance Adviser Rule by Id
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static PerformanceAdviserRule ById(string guid)
        {
            return new PerformanceAdviserRule(new PerformanceAdviserRuleId(new Guid(guid)));
        }

        /// <summary>
        /// Outputs rule name and rule description.
        /// </summary>
        /// <returns></returns>
        [SupressImportIntoVM]
        public override string ToString()
        {
            Autodesk.Revit.DB.PerformanceAdviser adviser = Autodesk.Revit.DB.PerformanceAdviser.GetPerformanceAdviser();
            return string.Format("{0} : {1}", adviser.GetRuleName(this.InternalId), adviser.GetRuleDescription(this.InternalId));
        }
        
    }
}
