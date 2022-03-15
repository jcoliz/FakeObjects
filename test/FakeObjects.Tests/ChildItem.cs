using System.ComponentModel.DataAnnotations;

public class ChildItem
{
    public int ID { get; set; }

    [Editable(true)]
    public string? Comment { get; set; }
}