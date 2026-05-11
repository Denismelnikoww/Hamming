using System.Collections.Generic;

namespace HammingApp.Models;

public class HammingDecodeResult
{
    public HammingVisualTable SyndromeTable { get; set; } = new();

    public List<SyndromeRow> SyndromeRows { get; set; } = new();

    public string SyndromeBits { get; set; } = string.Empty;

    public int ErrorPosition { get; set; }
}