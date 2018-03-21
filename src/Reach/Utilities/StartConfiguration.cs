using System;
using System.Collections.Generic;
using Dynamo.Extensions;
using Dynamo.Interfaces;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.Updates;
using Greg;

namespace Reach
{
    public struct StartConfiguration : DynamoModel.IStartConfiguration
    {
        public string Context { get; set; }
        public string DynamoCorePath { get; set; }
        public string DynamoHostPath { get; set; }
        public IPreferences Preferences { get; set; }
        public IPathResolver PathResolver { get; set; }
        public bool StartInTestMode { get; set; }
        public IUpdateManager UpdateManager { get; set; }
        public ISchedulerThread SchedulerThread { get; set; }
        public string GeometryFactoryPath { get; set; }
        public IAuthProvider AuthProvider { get; set; }
        public IEnumerable<IExtension> Extensions { get; set; }
        public TaskProcessMode ProcessMode { get; set; }
        public bool IsHeadless { get; set; }
    }
}
