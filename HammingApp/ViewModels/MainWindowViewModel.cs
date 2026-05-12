using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HammingApp.Models;
using HammingApp.Services;
using System.Collections.ObjectModel;

namespace HammingApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly HammingService _service = new();

    [ObservableProperty]
    private string inputText = string.Empty;

    [ObservableProperty]
    private bool isBinary;

    [ObservableProperty]
    private HammingVisualTable initialTable = new();

    [ObservableProperty]
    private HammingVisualTable matrixTable = new();

    [ObservableProperty]
    private ObservableCollection<ParityCalculation> calculations = new();

    [ObservableProperty]
    private string finalCode = string.Empty;

    [ObservableProperty]
    private string decodeInput = string.Empty;

    [ObservableProperty]
    private HammingVisualTable syndromeTable = new();

    [ObservableProperty]
    private ObservableCollection<SyndromeRow> syndromeRows = new();

    [ObservableProperty]
    private string syndromeBits = string.Empty;

    [ObservableProperty]
    private string decodeResult = string.Empty;
    
    // Новые свойства для полного декодирования
    [ObservableProperty]
    private string correctedCode = string.Empty;
    
    [ObservableProperty]
    private string decodedBinary = string.Empty;
    
    [ObservableProperty]
    private string decodedAscii = string.Empty;
    
    [ObservableProperty]
    private bool hasError;

    [RelayCommand]
    private void Encode()
    {
        var result = _service.Encode(InputText, IsBinary);

        InitialTable = result.InitialTable;
        MatrixTable = result.MatrixTable;
        Calculations = new ObservableCollection<ParityCalculation>(result.Calculations);
        FinalCode = result.FinalCode;
    }

    [RelayCommand]
    private void Decode()
    {
        var result = _service.DecodeToAscii(DecodeInput);

        SyndromeTable = result.SyndromeTable;
        SyndromeRows = new ObservableCollection<SyndromeRow>(result.SyndromeRows);
        SyndromeBits = result.SyndromeBits;
        
        HasError = result.ErrorPosition != 0;
        
        if (HasError)
        {
            DecodeResult = result.ErrorCorrected 
                ? $"✅ Ошибка обнаружена и исправлена в бите №{result.ErrorPosition}"
                : $"❌ Ошибка в бите №{result.ErrorPosition} (не удалось исправить)";
            
            CorrectedCode = result.CorrectedCode;
        }
        else
        {
            DecodeResult = "✅ Ошибок не обнаружено";
            CorrectedCode = DecodeInput;
        }
        
        DecodedBinary = result.BinaryString;
        DecodedAscii = string.IsNullOrEmpty(result.AsciiText) 
            ? "Не удалось декодировать в ASCII (неверный формат данных)" 
            : result.AsciiText;
    }
    
    partial void OnDecodeInputChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            DecodeResult = string.Empty;
            DecodedBinary = string.Empty;
            DecodedAscii = string.Empty;
            CorrectedCode = string.Empty;
        }
    }
}