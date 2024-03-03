using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace jcoliz.FakeObjects
{
    /// <summary>
    /// Implementation of the fluid composer of fake objects operations
    /// </summary>
    internal class FakeObjectsInternal<T> : IFakeObjects<T> where T : class, new()
    {
        /// <summary>
        /// All items created to date
        /// </summary>
        private readonly List<List<T>> Items = new();

        /// <summary>
        /// Total number of items
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Add another group of fake objects to this one
        /// </summary>
        /// <param name="count">How many objects</param>
        /// <param name="func">What changes to make on them</param>
        public IFakeObjects<T> Add(int count, Action<T> func)
        {
            var adding = GivenFakeItems<T>(count, func, 1 + Count).ToList();
            Items.Add(adding);
            Count += adding.Count;

            return this;
        }
        public IFakeObjects<T> Add(int count, Func<T,T> func)
        {
            var adding = GivenFakeItems<T>(count, func, 1 + Count).ToList();
            Items.Add(adding);
            Count += adding.Count;

            return this;
        }

        /// <summary>
        /// Add another group of fake objects to this one, accepting an index parameter
        /// </summary>
        /// <param name="count">How many objects</param>
        /// <param name="func">What changes to make on them</param>
        public IFakeObjects<T> Add(int count, Action<T,int> func)
        {
            var adding = GivenFakeItems<T>(count, func, 1 + Count).ToList();
            Items.Add(adding);
            Count += adding.Count;

            return this;
        }

        /// <summary>
        /// Apply an operation to all items
        /// </summary>
        /// <param name="func">What changes to make</param>
        public IFakeObjects<T> ApplyToAll(Action<T> func)
        {
            foreach (var groups in Items)
            {
                foreach (var item in groups)
                {
                    func(item);
                }
            }

            return this;
        }

        /// <summary>
        /// Apply an operation to all items, accepting an index
        /// </summary>
        /// <param name="func">What changes to make</param>
        public IFakeObjects<T> ApplyToAll(Action<T,int> func)
        {
            var index = 1;
            foreach (var groups in Items)
            {
                foreach (var item in groups)
                {
                    func(item,index++);
                }
            }

            return this;
        }

        /// <summary>
        /// Pick one particular series
        /// </summary>
        /// <param name="index">Which series</param>
        /// <returns>A list of all items in the chosen series</returns>
        public IList<T> Group(int index)
        {
            if (index >= Items.Count)
                throw new IndexOutOfRangeException();

            return Items.Skip(index).First();
        }

        /// <summary>
        /// Pick many series
        /// </summary>
        /// <param name="index">What series range</param>
        /// <returns>All items in the chosen series</returns>
        public IEnumerable<T> Groups(Range index)
        {
            if (index.Start.IsFromEnd)
                throw new IndexOutOfRangeException("Start from End not supported");

            var skip = index.Start.Value;

            if (skip >= Items.Count)
                throw new IndexOutOfRangeException();

            var take = index.End.IsFromEnd ? Items.Count - index.End.Value - skip : index.End.Value - skip;

            if (take <= 0)
                throw new IndexOutOfRangeException("Start must be before end");

            return Items.Skip(skip).Take(take).SelectMany(x=>x);
        }

        /// <summary>
        /// An iterator to step through ALL items across ALL groups
        /// </summary>
        /// <remarks>
        /// Implements IEnumerable
        /// </remarks>
        public IEnumerator GetEnumerator()
        {
            return Items.SelectMany(x => x).GetEnumerator();
        }

        /// <summary>
        /// An iterator to step through ALL items across ALL groups
        /// </summary>
        /// <remarks>
        /// Implements IEnumerable<T>
        /// </remarks>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Items.SelectMany(x => x).GetEnumerator();
        }

        /// <summary>
        /// Create the set of fake objects
        /// </summary>
        /// <param name="num">How many items to create</param>
        /// <param name="func">Action to take on each created item</param>
        /// <param name="from">What index number are we starting at?</param>
        protected static List<TItem> GivenFakeItems<TItem>(int num, Action<TItem> func = null, int from = 1) where TItem : class, new()
        {
            var result = Enumerable
                .Range(from, num)
                .Select(x => GivenFakeItem<TItem>(x))
                .ToList();

            if (func != null)
                foreach (var i in result)
                    func(i);

            return result;
        }

        protected static List<TItem> GivenFakeItems<TItem>(int num, Func<TItem,TItem> func, int from = 1) where TItem : class, new()
        {
            var result = Enumerable
                .Range(from, num)
                .Select(x => GivenFakeItem<TItem>(x))
                .ToList();

            if (func != null)
            {
                result = result.Select(func).ToList();            
            }

            return result;
        }

        /// <summary>
        /// Create the set of fake objects, passing in an index
        /// </summary>
        /// <param name="num">How many items to create</param>
        /// <param name="func">Action to take on each created item</param>
        /// <param name="from">What index number are we starting at?</param>
        protected static List<TItem> GivenFakeItems<TItem>(int num, Action<TItem,int> func, int from) where TItem : class, new()
        {
            var result = Enumerable
                .Range(from, num)
                .Select(x => GivenFakeItem<TItem>(x))
                .ToList();

            int index = from;
            if (func != null)
                foreach (var i in result)
                    func(i,index++);

            return result;
        }

        /// <summary>
        /// Create a single fake object
        /// </summary>
        /// <param name="index">Which index# are we creating</param>
        protected static TItem GivenFakeItem<TItem>(int index) where TItem : class, new()
        {
            return (TItem)GivenFakeItem_t(typeof(TItem), index);
        }

        /// <summary>
        /// Create a single fake object
        /// </summary>
        /// <param name="tfirst">What type of object to create</param>
        /// <param name="index">Which index# are we creating</param>
        protected static object GivenFakeItem_t(Type tfirst, int index)
        {
            var result = Activator.CreateInstance(tfirst);
            var properties = tfirst.GetProperties();
            var chosen = properties.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(System.ComponentModel.DataAnnotations.EditableAttribute)));

            foreach (var property in chosen)
            {
                var t = property.PropertyType;
                object o = default;

                if (t == typeof(string))
                    o = $"{property.Name} {index:D5}";
                else if (t == typeof(decimal) || t == typeof(decimal?))
                    o = index * 100m;
                else if (t == typeof(int) || t == typeof(int?))
                    o = index;
                else if (t == typeof(DateTime))
                    // Note that datetimes should descend, because anything which sorts by a datetime
                    // will typically sort descending
                    o = new DateTime(2001, 12, 31) - TimeSpan.FromDays(index);
                else if (t.IsClass)
                    o = GivenFakeItem_t(t, index);
                else
                    throw new NotImplementedException();

                property.SetValue(result, o);
            }

            // Also fill in any IEnumerables whether or not they're flagged
            // We're looking for IEnumerable<>, for which we'll create a new List<> of
            // the matching type
            foreach (var property in properties)
            {
                var t = property.PropertyType;
                if (t != typeof(string))
                    foreach (var ti in t.GetInterfaces())
                        if (ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                        {
                            var containedtype = ti.GetGenericArguments()[0];

                            var listType = typeof(List<>);
                            var constructedListType = listType.MakeGenericType(containedtype);

                            var o = Activator.CreateInstance(constructedListType);
                            property.SetValue(result, o);
                            break;
                        }
            }

            return result;
        }

        /// <summary>
        /// Save all objects created sofar to the chosen target
        /// </summary>
        /// <param name="target">Where to save them</param>
        public IFakeObjects<T> SaveTo(IFakeObjectsSaveTarget target)
        {
            target.AddRange(this);

            return this;
        }
    }

}
