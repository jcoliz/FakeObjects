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
    public void Int()
    {
        // When: Making some items each with an int property
        var items = FakeObjects<ModelItem>.Make(3);

        // Then: The property was filled in correctly
        int i = 1;
        Assert.IsTrue(items.All(x=>x.Index == i++));
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

    [TestMethod]
    public void AddCount()
    {
        // When: Making some items and adding more
        var items = FakeObjects<ModelItem>.Make(5).Add(10);

        // Then: Correct number of items were made
        Assert.AreEqual(15,items.Count);        
    }

    [TestMethod]
    public void AddGroups()
    {
        // When: Making some items and adding more
        var items = FakeObjects<ModelItem>.Make(5).Add(10);

        // Then: Correct number of items were made in each group
        Assert.AreEqual(5,items.Group(0).Count);
        Assert.AreEqual(10,items.Group(1).Count);
    }

    [TestMethod]
    public void ChangeAll()
    {
        // When: Making some items, adding more, and apply a change to all
        var value = 12345.67m;
        var items = FakeObjects<ModelItem>.Make(5).Add(10).ApplyToAll(x=>x.Amount = value);

        // Then: All properties were filled in correctly
        Assert.IsTrue(items.All(x => x.Amount == value));
    }

    [TestMethod]
    public void AddGroupsRange()
    {
        // Given: Some items and adding many more groups
        var items = FakeObjects<ModelItem>.Make(1).Add(2).Add(3).Add(4).Add(5);

        // When: Getting a group range 
        // (This returns 1,2,3,4)
        var groups = items.Groups(0..4);

        // Then: Correct number of items were included
        Assert.AreEqual(1+2+3+4,groups.Count());
    }

    [TestMethod]
    public void ModifyString()
    {
        // When: Making an item while modifying the string property
        var word = "__TEST__";
        var item = FakeObjects<ModelItem>.Make(1,x=>x.Name+=word).Single();

        // Then: The property was filled in correctly
        Assert.IsTrue(item.Name!.Contains(word));
    }

    [TestMethod]
    public void ModifyDecimal()
    {
        // When: Making many items while explicitly setting the decimal property
        var value = 12345.67m;
        var items = FakeObjects<ModelItem>.Make(10,x=>x.Amount = value);

        // Then: All properties were filled in correctly
        Assert.IsTrue(items.All(x=>x.Amount == value));
    }

    [DataRow(10)]
    [DataTestMethod]
    public void MakeIndex(int count)
    {
        // When: Making {count} items, and adjusting the amount based on an index
        var items = FakeObjects<ModelItem>.Make(count, (x, i) => x.Amount = i * 123m);

        // Then: All items have expected amount
        Assert.IsTrue(items.All(x => x.Index == x.Amount / 123m));
    }

    [TestMethod]
    public void AddGroupsIndex()
    {
        // When: Making some items and adding more, and adjusting the amount based on an index
        var items = FakeObjects<ModelItem>.Make(5).Add(10, (x, i) => x.Amount = i * 123m);

        // Then: All items have expected amount
        Assert.IsTrue(items.Group(0).All(x => x.Index == x.Amount / 100m));
        Assert.IsTrue(items.Group(1).All(x => x.Index == x.Amount / 123m));

        // NOTE: Under normal conditions, amount will be 100x Index.
        // For the second group, we're changing the factor to 123x
    }

    [TestMethod]
    public void SaveTo()
    {
        // When: Making items and saving them to a save target
        var repository = new Repository();
        var items = FakeObjects<ModelItem>.Make(10).SaveTo(repository);

        // Then: Items are correctly in the repository
        Assert.IsTrue(repository.SequenceEqual(items));
    }

    [TestMethod]
    public void SaveFirstGroup()
    {
        // When: Making items and saving some to a save target, and adding more
        var repository = new Repository();
        var items = FakeObjects<ModelItem>.Make(10).SaveTo(repository).Add(5);

        // Then: Only first group are in the repository
        Assert.IsTrue(repository.SequenceEqual(items.Group(0)));
    }
}