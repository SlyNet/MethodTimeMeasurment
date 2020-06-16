using System.Collections.Generic;
using System.IO;

namespace TestProject.Tests.Logger
{
    public static class StringExtensions
    {
        public static IEnumerable<string> ToLines(this string text)
        {
            using StringReader sr = new StringReader(text);
            string line;

            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }
        
        public static string Multiply(this string source, int multiplier)
        {
            var sb = ObjectPoolHelpers.StringBuilder.Get();
            try
            {
                for (int i = 0; i < multiplier; i++)
                {
                    sb.Append(source);
                }

                return sb.ToString();
            }
            finally
            {
                ObjectPoolHelpers.StringBuilder.Return(sb);
            }
        }
    }
}