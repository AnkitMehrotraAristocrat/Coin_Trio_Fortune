using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace GameBackend.Helpers
{
    public static class DebugHelper
    {
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Log Helpers

        public static void LogStep(object caller)
        {
            LogLine(caller.GetType().FullName);
        }

        public static void LogLine(string text)
        {
            #if !LOGGING_OFF
            Debug.WriteLine(text);
            #endif
        }

        public static void LogLine()
        {
            #if !LOGGING_OFF
            Debug.WriteLine(string.Empty);
            #endif
        }

        public static void LogText(string text)
        {
            #if !LOGGING_OFF
            Debug.Write(text);
            #endif
        }

        public static void LogError(string text)
        {
            #if !LOGGING_OFF
            Debug.WriteLine(text);
            #endif
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Unit Test Helpers

        public static void AssertAreEqual<T>(string label, T value1, T value2)
        {
            LogLine($"[AssertAreEqual] {label} {value1}=={value2} {value1.Equals(value2)}");
        }

        public static void AssertAreAllEqual<T>(string label, List<T> value1, List<T> value2)
        {
            var valStr1 = value1 != null ? string.Join<T>(',', value1) : "null";
            var valStr2 = value2 != null ? string.Join<T>(',', value2) : "null";
            var batch = new StringBuilder();
            batch.AppendLine();
            batch.AppendLine($"[AssertAreAllEqual] {label}");
            batch.AppendLine($"[{valStr1}]==[{valStr2}]");
            batch.AppendLine($"{valStr1.Equals(valStr2)}");
            batch.AppendLine();
            LogText(batch.ToString());
        }
    }
}
