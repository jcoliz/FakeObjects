using jcoliz.FakeObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FakeObjects.Tests;

[TestClass]
public class UnitTests
{
    [DataRow(1)]
    [DataRow(10)]
    [DataRow(100)]
    [DataTestMethod]
    public void Make(int count)
    {
        // When: Making {count} items
        var items = FakeObjects<ModelItem>.Make(count);

        // Then: {count} items were made
        Assert.AreEqual(count,items.Count);
    }
}