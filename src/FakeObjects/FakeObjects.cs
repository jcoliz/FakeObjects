﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace jcoliz.FakeObjects
{
    public static class FakeObjects<T> where T : class, new()
    {
        public static IFakeObjects<T> Make(int count, Action<T> func = null)
        {
            return new FakeObjectsInternal<T>().Add(count,func);
        }
    }

    public interface IFakeObjectsSaveTarget
    {
        void AddRange(IEnumerable objects);
    }

    public interface IFakeObjects<T>: IEnumerable<T> where T : class, new()
    {
        /// <summary>
        /// Pick one particular series
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IList<T> Group(int index);

        IEnumerable<T> Groups(Range index);

        /// <summary>
        /// Total number of items
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Add another group of fake objects to this one
        /// </summary>
        /// <param name="count">How many objects</param>
        /// <param name="func">What changes to make on them</param>
        /// <returns></returns>
        IFakeObjects<T> Add(int count, Action<T> func = null);

        IFakeObjects<T> SaveTo(IFakeObjectsSaveTarget target);
    }

    internal class FakeObjectsInternal<T> : IFakeObjects<T> where T : class, new()
    {
        private List<List<T>> Items = new List<List<T>>();

        public int Count { get; private set; }

        public IFakeObjects<T> Add(int count, Action<T> func)
        {
            var adding = GivenFakeItems<T>(count, func, 1 + Count).ToList();
            Items.Add(adding);
            Count += adding.Count;

            return this;
        }

        public IList<T> Group(int index)
        {
            if (index >= Items.Count())
                throw new IndexOutOfRangeException();

            return Items.Skip(index).First();
        }

        public IEnumerable<T> Groups(Range index)
        {
            if (index.Start.IsFromEnd)
                throw new IndexOutOfRangeException("Start from End not supported");

            var skip = index.Start.Value;

            if (skip >= Items.Count())
                throw new IndexOutOfRangeException();

            var take = index.End.IsFromEnd ? Items.Count - index.End.Value - skip : index.End.Value - skip;

            if (take <= 0)
                throw new IndexOutOfRangeException("Start must be before end");

            return Items.Skip(skip).Take(take).SelectMany(x=>x);
        }

        public IEnumerator GetEnumerator()
        {
            return Items.SelectMany(x => x).GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Items.SelectMany(x => x).GetEnumerator();
        }

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

        protected static TItem GivenFakeItem<TItem>(int index) where TItem : class, new()
        {
            return (TItem)GivenFakeItem_t(typeof(TItem), index);
        }

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

        public IFakeObjects<T> SaveTo(IFakeObjectsSaveTarget target)
        {
            target.AddRange(this);

            return this;
        }
    }

    public static class TestKey<T>
    {
        public static PropertyInfo Find()
        {
            // Find the test key on the object
            var properties = typeof(T).GetProperties();
            var chosen = properties.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(System.ComponentModel.CategoryAttribute)));
            if (!chosen.Any())
                throw new ApplicationException("Test Key not found");
            if (chosen.Skip(1).Any())
                throw new ApplicationException("More than one Test Key found");
            var property = chosen.Single();

            return property;
        }

        public static Func<T, object> Order()
        {
            return x => Find().GetValue(x);
        }
    }
}
