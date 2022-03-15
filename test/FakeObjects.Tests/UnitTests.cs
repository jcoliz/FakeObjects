using jcoliz.FakeObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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

    [TestMethod]
    public void String()
    {
        // When: Making an item with a string property
        var item = FakeObjects<ModelItem>.Make(1).Single();

        // Then: The property was filled in correctly
        Assert.IsTrue(item.Name!.Contains(nameof(ModelItem.Name)));
    }

    [TestMethod]
    public void DateTime()
    {
        // When: Making two items with datetime properties
        var items = FakeObjects<ModelItem>.Make(2).Group(0);

        // Then: The difference between the two is one day.
        var dayone = items[0].Date;
        var daytwo = items[1].Date;
        var diff = Math.Abs((dayone - daytwo).Days);
        Assert.AreEqual(1,diff);
    }

    [TestMethod]
    public void Decimal()
    {
        // When: Making an item with a decimal property
        var item = FakeObjects<ModelItem>.Make(1).Single();

        // Then: The property was filled in correctly
        Assert.AreNotEqual(0,item.Amount);
    }

    [TestMethod]
    public void Unmarked()
    {
        // When: Making an item with an unmarked property
        var item = FakeObjects<ModelItem>.Make(1).Single();

        // Then: The property was NOT filled in
        Assert.AreEqual(0,item.ID);
    }

    [TestMethod]
    public void Children()
    {
        // When: Making an item with a child item property, which has a string field
        var item = FakeObjects<ModelItem>.Make(1).Single();

        // Then: The child property was filled in correctly
        Assert.IsTrue(item.Favorite!.Comment!.Contains(nameof(ChildItem.Comment)));
    }

    [TestMethod]
    public void Enumerable()
    {
        // When: Making an item with a collection property
        var item = FakeObjects<ModelItem>.Make(1).Single();

        // Then: The collection was not left null
        Assert.IsNotNull(item.Children);
    }

}