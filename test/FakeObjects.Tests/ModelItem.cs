using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ModelItem
{
    public int ID { get; set; }

    [Editable(true)]
    public string? Name { get; set; }

    [Editable(true)]
    public DateTime Date { get; set; }

    [Editable(true)]
    public decimal Amount { get; set; }

    public string? Details { get; set; }

    public ICollection<ChildItem>? Children { get; set; }

    [Editable(true)]
    public ChildItem? Favorite { get; set; }
}