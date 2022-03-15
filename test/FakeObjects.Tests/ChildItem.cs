using System.ComponentModel.DataAnnotations;

namespace FakeObjects.Tests;

public class ChildItem
{
    public int ID { get; set; }

    [Editable(true)]
    public string? Comment { get; set; }
}