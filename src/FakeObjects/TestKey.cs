using System;
using System.Linq;
using System.Reflection;

namespace jcoliz.FakeObjects
{
    /// <summary>
    /// Class independent test key
    /// </summary>
    /// <remarks>
    /// TODO: Will need to document and write test cases for this!
    /// </remarks>
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
