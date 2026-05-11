using System.Collections.Generic;

namespace HammingApp.Models;

public class HammingVisualTable
{
    public List<string> Headers { get; set; } = new();
    public List<HammingVisualRow> Rows { get; set; } = new();
}