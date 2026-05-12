namespace HammingApp.Models;

using System.Collections.Generic;

public class HammingFullDecodeResult
{
    public HammingVisualTable SyndromeTable { get; set; } = new();
    public List<SyndromeRow> SyndromeRows { get; set; } = new();
    public string SyndromeBits { get; set; } = string.Empty;
    public int ErrorPosition { get; set; }
    public bool ErrorCorrected { get; set; }
    public string CorrectedCode { get; set; } = string.Empty;
    public string BinaryString { get; set; } = string.Empty;
    public string AsciiText { get; set; } = string.Empty;
}