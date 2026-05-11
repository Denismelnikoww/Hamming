using System.Collections.Generic;

namespace HammingApp.Models;

public class HammingVisualRow
{
    public string Name { get; set; } = string.Empty;

    public List<string> Values { get; set; } = new();
}