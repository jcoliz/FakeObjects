using System;

namespace jcoliz.FakeObjects
{
    /// <summary>
    /// Factory to create fake objects of type T
    /// </summary>
    public static class FakeObjects<T> where T : class, new()
    {
        /// <summary>
        /// Make the initial set of fake objects
        /// </summary>
        /// <param name="count">How many objects</param>
        /// <param name="func">What changes to make on them</param>
        public static IFakeObjects<T> Make(int count, Action<T> func = null)
        {
            return new FakeObjectsInternal<T>().Add(count,func);
        }

        /// <summary>
        /// Make the initial set of fake objects, accepting an index parameter
        /// </summary>
        /// <param name="count">How many objects</param>
        /// <param name="func">What changes to make on them</param>
        public static IFakeObjects<T> Make(int count, Action<T,int> func)
        {
            return new FakeObjectsInternal<T>().Add(count, func);
        }
    }
}
