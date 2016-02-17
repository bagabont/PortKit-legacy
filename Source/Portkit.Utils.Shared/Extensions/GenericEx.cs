using System;

namespace Portkit.Utils.Extensions
{
    public static class GenericEx
    {
        public static TResult With<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator)
            where TResult : class
            where TInput : class
        {
            return o == null ? null : evaluator(o);
        }

        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue) where TInput : class
        {
            return o == null ? failureValue : evaluator(o);
        }

        public static TInput Do<TInput>(this TInput o, Action<TInput> action) where TInput : class
        {
            if (o == null)
            {
                return null;
            }
            action(o);
            return o;
        }

        public static TInput If<TInput>(this TInput o, Func<TInput, bool> evaluator) where TInput : class
        {
            return o == null ? null : (evaluator(o) ? o : null);
        }

        public static TInput Unless<TInput>(this TInput o, Func<TInput, bool> evaluator) where TInput : class
        {
            return o == null ? null : (evaluator(o) ? null : o);
        }

        /// <summary>
        /// Executes an action when an input object is null.
        /// </summary>
        /// <typeparam name="TInput">Input object type.</typeparam>
        /// <param name="o">Object instance</param>
        /// <param name="action">Action to be executed when input is null.</param>
        /// <returns>Input object</returns>
        public static TInput OnNull<TInput>(this TInput o, Action action) where TInput : class
        {
            if (o == null)
            {
                action?.Invoke();
            }
            return o;
        }
    }
}
