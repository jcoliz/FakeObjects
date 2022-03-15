using jcoliz.FakeObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FakeObjects.Tests;

/// <summary>
/// Repository of items for use in testing
/// </summary>
public class Repository : List<ModelItem>, IFakeObjectsSaveTarget
{
    public new void AddRange(IEnumerable<ModelItem> items)
    {
        // Find the next ID to assign
        var nextid = 1;
        if (this.Any())
            nextid = 1 + this.Max(x=>x.ID);

        // Assign them all IDs
        foreach(var item in items)
            item.ID = nextid++;

        // Add the given items
        base.AddRange(items);
    }

    public void AddRange(IEnumerable objects)
    {
        if (objects is IEnumerable<ModelItem> items)
            base.AddRange(items);
        else
            throw new System.NotImplementedException();
    }
    
    public IEnumerable<ModelItem> GetItems() => this as IEnumerable<ModelItem>;

    public IEnumerable<ModelItem> Search(string term) => this
        .Where(x => 
            (x.Name?.Contains(term) ?? false) ||
            (x.Favorite?.Comment?.Contains(term) ?? false)
        );

    internal void Update(int id, string? name, string? details)
    {
        var selected = this.Where(x=>x.ID == id).Single();
        selected.Name = name;
        selected.Details = details;
    }
}
