using System.ComponentModel.DataAnnotations;

namespace FakeObjects.Tests;

/// <summary>
/// Example item contained in another item
/// </summary>
public class ChildItem
{
    public int ID { get; set; }

    [Editable(true)]
    public string? Comment { get; set; }
}