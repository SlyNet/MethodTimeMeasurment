using System;
using System.Diagnostics.CodeAnalysis;

namespace TestProject.Tests.Logger
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public static class LogManagerAdapter
    {
        public static LoggerAdapter GetLogger(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return new LoggerAdapter(type);
        }
    }
}