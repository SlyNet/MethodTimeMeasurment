using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace TestProject.Tests.Logger
{
    public static class ObjectPoolHelpers
    {
        public static ObjectPoolProvider ObjectPoolProvider = new DefaultObjectPoolProvider();
        public static ObjectPool<StringBuilder> StringBuilder = ObjectPoolProvider.CreateStringBuilderPool();
    }
}