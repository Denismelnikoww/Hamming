using System.Collections.Generic;

namespace HammingApp.Models;

public class HammingEncodeResult
{
    public HammingVisualTable InitialTable { get; set; } = new();

    public HammingVisualTable MatrixTable { get; set; } = new();

    public List<ParityCalculation> Calculations { get; set; } = new();

    public string FinalCode { get; set; } = string.Empty;
}