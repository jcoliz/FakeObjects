using jcoliz.FakeObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FakeObjects.Tests;

/// <summary>
/// These tests give concrete examples of how you might use the features of this library
/// </summary>
[TestClass]
public class UsageTests: IFakeObjectsSaveTarget
{
    private Repository repository { get; set; } = new Repository();

    public void AddRange(IEnumerable objects)
    {
        if (objects is IEnumerable<ModelItem> items)
            repository.AddRange(items);
        else
            throw new NotImplementedException();
    }

    /// <summary>
    /// Initialize tests
    /// </summary>
    /// <remarks>
    /// Create a fresh repository for every test
    /// </remarks>
    [TestInitialize]
    public void SetUp()
    {
        repository = new Repository();
    }

    /// <summary>
    /// Create objects
    /// </summary>
    /// <remarks>
    /// In its simplest form, you can simply make a number of objects. Each item will have a 
    /// unique value in the editable fields, and default values in the others.
    /// </remarks>
    [TestMethod]
    public void AddItems()
    {
        // Given: A set of objects
        var items = FakeObjects<ModelItem>.Make(20);

        // When: Adding objects to the system
        repository.AddRange(items);

        // Then: The items were added
        var results = repository.GetItems();
        Assert.IsTrue(results.SequenceEqual(items));
    }

    /// <summary>
    /// Modify certain objects
    /// </summary>
    /// <remarks>
    /// Commonly, we want a subset of objects to have a certain property we're looking for.
    /// Each call to Make or Add creates another group, starting at zero.
    /// In this example, Group(1) will contain the 10 items with "__test__" appended to the name.
    /// </remarks>
    [TestMethod]
    public void Search()
    {
        // Given: A set of objects, where a subset has {word} in the name
        var word = "__test__";
        var items = FakeObjects<ModelItem>.Make(20).Add(10, x => x.Name += word);

        // And: The objects are in the repository
        repository.AddRange(items);

        // When: Searching for {word}
        var results = repository.Search(word);
        
        // Then: Only the objects with {word} are returned
        Assert.IsTrue(results.SequenceEqual(items.Group(1)));
    }

    /// <summary>
    /// Add objects to the system under test
    /// </summary>
    /// <remarks>
    /// Any time we are adding objects to the system under test during the "Given" phase of a test,
    /// it's a good time to use the "SaveTo()" feature. In this example, we implement 
    /// IFakeObjectsSaveTarget directly in the test class. You can also implement it directly,
    /// for example in a mock repository of items.
    /// </remarks>
    [TestMethod]
    public void SearchV2()
    {
        // Given: A set of objects in the repository, where a subset has {word} in the name
        var word = "__test__";
        var items = FakeObjects<ModelItem>.Make(20).Add(10, x => x.Name += word).SaveTo(this);

        // When: Searching for {word}
        var results = repository.Search(word);
        
        // Then: Only the objects with {word} are returned
        Assert.IsTrue(results.SequenceEqual(items.Group(1)));
    }

    /// <summary>
    /// Add only some objects
    /// </summary>
    /// <remarks>
    /// Another common scenario is to start the test with some items in the database, and
    /// have others which were are not added during the "Given" phase. To support this,
    /// the SaveTo() method only saves objects which were already created at the time it was
    /// called.
    /// </remarks>
    [TestMethod]
    public void AddItemsToExisting()
    {
        // Given: A set of objects in the repository, plus more not yet added
        var items = FakeObjects<ModelItem>.Make(20).SaveTo(this).Add(10);

        // When: Adding the new items
        repository.AddRange(items.Group(1));

        // Then: The all the items are in the repository
        var results = repository.GetItems();
        Assert.IsTrue(results.SequenceEqual(items));
    }

    /// <summary>
    /// Choose multiple groups of items
    /// </summary>
    /// <remarks>
    /// Sometimes we need to create many different permutations of test data, then reference them
    /// all at once. Groups(X..Y) will handle this
    /// </remarks>

    [TestMethod]
    public void SearchMultiple()
    {
        // Given: A set of objects in the repository, some have a {word} in their name,
        // or their favorite child's comments, or some in their details
        var word = "__test__";
        var items = FakeObjects<ModelItem>
                        .Make(10)
                        .Add(5, x => x.Name += word)
                        .Add(5, x => x.Favorite!.Comment += word)
                        .Add(5, x => x.Details += word)
                        .SaveTo(this);

        // When: Searching for {word}
        var results = repository.Search(word);

        // Then: Only the objects with {word} in name or favorite comment are returned
        // (Note that items with {word} in Details are NOT returned
        Assert.IsTrue(results.SequenceEqual(items.Groups(1..3)));
    }

    /// <summary>
    /// Use additional objects as updated values
    /// </summary>
    /// <remarks>
    /// Sometimes it can by handy to create an extra object, not added to the database,
    /// to be used as new values for testing update cases
    /// </remarks>
    [TestMethod]
    public void Update()
    {
        // Given: A set of objects in the repository, one of which we care about
        // And: One more not yet added
        var items = FakeObjects<ModelItem>.Make(20).SaveTo(this).Add(1, x => x.Details = "Some Details");
        var id = items.Group(0).Last().ID;
        var newvalues = items.Group(1).Single();

        // When: Updating the selected item with new values
        repository.Update(id,newvalues.Name,newvalues.Details);

        // Then: The the item was updated
        var actual = repository.GetItems().Last();
        Assert.AreEqual(newvalues.Name,actual.Name);
        Assert.AreEqual(newvalues.Details,actual.Details);
    }
}