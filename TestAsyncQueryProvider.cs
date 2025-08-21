// TestAsyncQueryProvider.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;

namespace Tests.TestingAsync
{
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        internal TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

        // IMPORTANT: construct the correct element type from the expression
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.GetGenericArguments().First();
            var queryableType = typeof(TestAsyncEnumerable<>).MakeGenericType(elementType);
            return (IQueryable)Activator.CreateInstance(queryableType, expression)!;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression)
            => _inner.Execute(expression)!;

        public TResult Execute<TResult>(Expression expression)
            => _inner.Execute<TResult>(expression)!;

        // EF Core uses this to get IAsyncEnumerable<TResult>
        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            => new TestAsyncEnumerable<TResult>(expression);

        // NOTE: signature returns TResult (NOT Task<TResult>)
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            => Execute<TResult>(expression);
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync() => new(_inner.MoveNext());

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}
