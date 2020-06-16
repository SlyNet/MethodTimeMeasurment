using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace TestProject.Tests.Logger
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class LoggerAdapter
    {
        private const string NullString = "<NULL>";
        private readonly Type _type;

        public LoggerAdapter(Type type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }
        
        public void TraceEnter(string methodName, 
            Tuple<string, string>[] methodParameters,
            string[] paramNames,
            object[] paramValues)
        {
            string message;
            
            if (paramNames != null)
            {
                StringBuilder parameters = ObjectPoolHelpers.StringBuilder.Get();
                try
                {
                    for (int i = 0; i < paramNames.Length; i++)
                    {
                        parameters.AppendFormat("\t{0} = {1}", paramNames[i], paramValues[i] ?? NullString);
                        if (i < paramNames.Length - 1) parameters.Append(Environment.NewLine);
                    }
                    string argInfo = parameters.ToString();
                    message = $"{Environment.NewLine}{argInfo}";
                }
                finally
                {
                    ObjectPoolHelpers.StringBuilder.Return(parameters);
                }
            }
            else
            {
                message = string.Empty;
            }

            Debug(TestLog.SectionType.Opening, methodName, message);
            TestLog.Depth.Value++;
        }

        public void TraceLeave(string methodName, Tuple<string, string>[] methodParameters, long startTicks, long endTicks, string[] paramNames, object[] paramValues)
        {
            string returnValue = null;

            if (paramNames != null)
            {
                StringBuilder parameters = ObjectPoolHelpers.StringBuilder.Get();
                try
                {
                    for (int i = 0; i < paramNames.Length; i++)
                    {
                        string value = (paramValues[i] ?? NullString).ToString();
                        value = value.Substring(0, Math.Min(value.Length, 900));

                        parameters.Append($"{paramNames[i] ?? "$return"}={value}");
                        if (i < paramNames.Length - 1) parameters.Append(", ");
                    }

                    returnValue = parameters.ToString();
                }
                finally
                {
                    ObjectPoolHelpers.StringBuilder.Return(parameters);
                }
            }

            double timeTaken = ConvertTicksToMilliseconds(endTicks - startTicks);

            string message = string.IsNullOrWhiteSpace(returnValue) 
                ? $" Time taken: {timeTaken:0.00} ms"
                : $"{returnValue}\r\n Time taken: {timeTaken:0.00} ms";

            if (timeTaken > 2000)
            {
                message = "!!" + message;
            }

            TestLog.Depth.Value--;
            Debug(TestLog.SectionType.Closing, methodName, message);
        }

        void Debug(TestLog.SectionType sectionType, string methodName, string message)
        {
            var prefix = $"{ (_type.Namespace ?? "").Split('.').LastOrDefault() ?? ""}.{ _type.Name}.{methodName}";
         
            if (sectionType == TestLog.SectionType.Opening)
            {
                message = prefix + message;
            }

            var messageLines = message.ToLines().ToList();

            var result = messageLines.Select((l, i) =>
            {
                if (messageLines.Count > 0)
                {
                    return sectionType switch
                    {
                        TestLog.SectionType.Closing => i == messageLines.Count - 1
                            ? prefix + l
                            : l,
                        TestLog.SectionType.Opening => l,
                        _ => prefix + l
                    };
                }

                return prefix + l;
            });
            
            TestLog.Debug(string.Join(Environment.NewLine, result), sectionType);
        }

        private static double ConvertTicksToMilliseconds(long ticks)
        {
            //ticks * tickFrequency * 10000
            return ticks * (10000000 / (double)Stopwatch.Frequency) / 10000L;
        }
    }
}