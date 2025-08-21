// Tests/TestingAsync/AsyncQueryableExtensions.cs
using System.Collections.Generic;
using System.Linq;
using UserManagement.Data.Tests.TestingAsync;

namespace Tests.TestingAsync
{
    internal static class AsyncQueryableExtensions
    {
        public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
            => new TestAsyncEnumerable<T>(source);
    }
}
