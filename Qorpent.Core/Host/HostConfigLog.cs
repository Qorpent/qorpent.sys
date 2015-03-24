using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Config;
using Qorpent.IO;
using Qorpent.Log;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host {
    public partial class HostConfig {
        public void LogDebug(object message) {
            Trace.WriteLine(message, "debug");
        }

        public void LogInfo(object message) {
            Trace.WriteLine(message, "info");
        }

        public void LogWarning(object message) {
            Trace.WriteLine(message, "warning");
        }

        public void LogError(object message) {
            Trace.WriteLine(message, "error");
        }

        public void LogFatal(object message) {
            Trace.WriteLine(message, "fatal");
        }

    }


}