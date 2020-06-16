using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace TestProject.Tests.Logger
{
    public static class TestLog
    {
        public static readonly AsyncLocal<int> Depth = new AsyncLocal<int>();
        public enum SectionType
        {
            Normal,
            Opening,
            Closing
        }
        
        public static void Debug(string line, SectionType sectionType = SectionType.Normal) =>
            Write("DBG", line, sectionType);

        private static void Write(string level, string message, SectionType sectionType = SectionType.Normal)
        {
            foreach (string line in MessageToLogLines(level, message, sectionType))
            {
                System.Diagnostics.Debug.WriteLine(line);
            }

            var logMessages = MessageToLogLines(level, message, sectionType, false).ToList();
            logMessages.ForEach(TestContext.Out.WriteLine);
        }
        
        private static IEnumerable<string> MessageToLogLines(string level, string message,
            SectionType sectionType, bool addWorker = true)
        {
            var ident = "| ".Multiply(TestLog.Depth.Value);

            List<string> lines = message.ToLines().ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                string m = lines[i];
                string prefix = $"[{level} {DateTime.UtcNow:G}]{(addWorker ? " " + CurrentContextWorkerId : "")} ";

                var corner = sectionType switch
                {
                    SectionType.Normal => "| ",
                    SectionType.Opening when i == 0 => "┌",
                    SectionType.Opening => "| ",
                    SectionType.Closing when i == lines.Count -1 && lines.Count > 1 => "└",
                    SectionType.Closing when i == lines.Count - 1 => "└",
                    SectionType.Closing when i == 0 && lines.Count > 1 => "├─",
                    SectionType.Closing when i == 0 => "|",
                    SectionType.Closing when i != lines.Count -1 => "| ",
                    _ => ""
                };

                yield return $"{prefix}{ident}{corner}{m}";
            }
        }
        
        private static string CurrentContextWorkerId => TestContext.CurrentContext.WorkerId ?? "noId";
    }
}