# Usage

This is the complete reference for all features available in the library.

### Namespace

```c#
using jcoliz.FakeObjects;
```

### Mark Up Classes

First, you'll need to tell the library which properties you want filled in on your
fake objects. Do this by marking them "Editable(true)". In the example below, the
library will only fill in values for the Name and Date properties.

```c#
using System.ComponentModel.Annotations;

public class ModelItem
{
    public int ID { get; set; }

    [Editable(true)]
    public string Name { get; set; }

    [Editable(true)]
    public DateTime Date { get; set; }

    public string Details { get; set; }
}
```

### Create objects

In its simplest form, you can simply make a number of objects. Each item will have a 
unique value in the editable fields, and default values in the others.

```c#
[TestMethod]
public void AddItems()
{
    // Given: A set of objects
    var items = FakeObjects<ModelItem>.Make(20);

    // When: Adding objects to the system
    respository.AddRange(items);

    // Then: The items were added
    var results = repository.GetItems();
    Assert.IsTrue(results.SequenceEqual(items));
}
```

### Modify certain objects

Commonly, we want a subset of objects to have a certain property we're looking for.
Each call to Make or Add creates another group, starting at zero.
In this example, Group(1) will contain the 10 items with "__test__" appended to the name.

```c#
[TestMethod]
public void Search()
{
    // Given: A set of objects, where a subset has {word} in the name
    var word = "__test__";
    var items = FakeObjects<ModelItem>.Make(20).Add(10, x => x.Name += test);

    // And: The objects are in the repository
    respository.AddRange(items);

    // When: Searching for {word}
    var results = repository.Search(word);
    
    // Then: Only the objects with {word} are returned
    Assert.IsTrue(results.SequenceEqual(items.Group(1)));
}
```

### Add objects to the system under test

Any time we are adding objects to the system under test during the "Given" phase of a test,
it's a good time to use the "SaveTo()" feature. In this example, we implement 
IFakeObjectsSaveTarget directly in the test class. You can also implement it directly,
for example in a mock repository of items

```c#
using System.Collections;

[TestClass]
public class TestClass: IFakeObjectsSaveTarget
{
    public void AddRange(IEnumerable objects)
    {
        if (objects is IEnumerable<ModelItem> items)
        {
            repository.AddRange(items);
        }
        else
            throw new NotImplementedException();
    }

    [TestMethod]
    public void Search_V2()
    {
        // Given: A set of objects in the repository, where a subset has {word} in the name
        var word = "__test__";
        var items = FakeObjects<ModelItem>.Make(20).Add(10, x => x.Name += test).SaveTo(this);

        // When: Searching for {word}
        var results = repository.Search(word);
        
        // Then: Only the objects with {word} are returned
        Assert.IsTrue(results.SequenceEqual(items.Group(1)));
    }
}
```

### Add only some objects

Another common scenario is to start the test with some items in the database, and
have others which were are not added during the "Given" phase. To support this,
the SaveTo() method only saves objects which were already created at the time it was
called.

```c#
[TestMethod]
public void AddItemsToExisting()
{
    // Given: A set of objects in the repository, plus more not yet added
    var items = FakeObjects<ModelItem>.Make(20).SaveTo(this).Add(10);

    // When: Adding the new items
    respository.AddRange(items.Group(1));

    // Then: The all the items are in the repository
    var results = repository.GetItems();
    Assert.IsTrue(results.SequenceEqual(items));
}
```

### TODO: More!

More documentation on finer-grain controls to follow!