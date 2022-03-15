using jcoliz.FakeObjects;
using System.Collections;
using System.Collections.Generic;

namespace FakeObjects.Tests;

public class Repository : List<ModelItem>, IFakeObjectsSaveTarget
{
    public void AddRange(IEnumerable objects)
    {
        if (objects is IEnumerable<ModelItem> items)
            base.AddRange(items);
        else
            throw new System.NotImplementedException();
    }
}
