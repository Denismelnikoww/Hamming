using System.Collections.Generic;

namespace HammingApp.Models;

public class HammingResult
{
    public string SourceBits { get; set; } = string.Empty;

    public string EncodedBits { get; set; } = string.Empty;

    public List<SyndromeRow> SyndromeTable { get; set; } = new();
}
