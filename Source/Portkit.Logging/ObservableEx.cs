using System;
using System.Threading;

namespace Portkit.Logging
{
    internal static class ObservableEx
    {
        /// <summary>
        ///     Creates a subscription to the source, where in every occurrence of the source stream, it transforms the input
        ///     item using the <paramref name="selector" /> and pushes the result to the <paramref name="observer" />.
        /// </summary>
        public static IDisposable CreateSubscription<TIn, TOut>(this IObservable<TIn> source, IObserver<TOut> observer,
            Func<TIn, TOut> selector)
            where TIn : class
        {
            var projection = new Projection<TIn, TOut>(observer, selector);
            return projection.Connect(source);
        }

        private sealed class Projection<TIn, TOut> : IObserver<TIn>
            where TIn : class
        {
            private readonly Func<TIn, TOut> _selector;
            private IObserver<TOut> _observer;
            private ProjectionSubscription _subscription;

            public Projection(IObserver<TOut> observer, Func<TIn, TOut> selector)
            {
                _observer = observer;
                _selector = selector;
            }

            void IObserver<TIn>.OnCompleted()
            {
                _observer.OnCompleted();
                using (_subscription)
                {
                }
            }

            void IObserver<TIn>.OnError(Exception error)
            {
                _observer.OnError(error);
                using (_subscription)
                {
                }
            }

            void IObserver<TIn>.OnNext(TIn value)
            {
                if (value != null)
                {
                    _observer.OnNext(_selector(value));
                }
            }

            public IDisposable Connect(IObservable<TIn> source)
            {
                _subscription = new ProjectionSubscription(this, source.Subscribe(this));
                return _subscription;
            }

            private class NoOpObserver : IObserver<TOut>
            {
                void IObserver<TOut>.OnCompleted()
                {
                }

                void IObserver<TOut>.OnError(Exception error)
                {
                }

                void IObserver<TOut>.OnNext(TOut value)
                {
                }
            }

            private sealed class ProjectionSubscription : IDisposable
            {
                private Projection<TIn, TOut> _parent;
                private IDisposable _subscription;

                public ProjectionSubscription(Projection<TIn, TOut> parent, IDisposable subscription)
                {
                    _parent = parent;
                    _subscription = subscription;
                }

                public void Dispose()
                {
                    Projection<TIn, TOut> currentParent = Interlocked.Exchange(ref _parent, null);
                    if (currentParent != null)
                    {
                        _subscription.Dispose();
                        currentParent._observer = new NoOpObserver();
                        _subscription = null;
                    }
                }
            }
        }
    }
}