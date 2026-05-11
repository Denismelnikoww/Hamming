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

    [RelayCommand]
    private void Encode()
    {
        var result = _service.Encode(InputText, IsBinary);

        InitialTable = result.InitialTable;

        MatrixTable = result.MatrixTable;

        Calculations = new ObservableCollection<ParityCalculation>(
            result.Calculations
        );

        FinalCode = result.FinalCode;
    }

    [RelayCommand]
    private void Decode()
    {
        var result = _service.Decode(DecodeInput);

        SyndromeTable = result.SyndromeTable;

        SyndromeRows = new ObservableCollection<SyndromeRow>(
            result.SyndromeRows
        );

        SyndromeBits = result.SyndromeBits;

        DecodeResult =
            result.ErrorPosition == 0
                ? "Ошибок не обнаружено"
                : $"Ошибка в бите №{result.ErrorPosition}";
    }
}

