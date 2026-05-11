using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HammingApp.Models;
using HammingApp.Services;

namespace HammingApp.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly HammingService _hammingService = new();

    [ObservableProperty]
    private string inputText = string.Empty;

    [ObservableProperty]
    private bool isBinary;

    [ObservableProperty]
    private string sourceBits = string.Empty;

    [ObservableProperty]
    private string encodedResult = string.Empty;

    [ObservableProperty]
    private string checkInput = string.Empty;

    [ObservableProperty]
    private string errorResult = string.Empty;

    public ObservableCollection<SyndromeRow> SyndromeRows { get; } = new();

    [RelayCommand]
    private void Encode()
    {
        SyndromeRows.Clear();

        HammingResult result = _hammingService.Encode(InputText, IsBinary);

        SourceBits = result.SourceBits;

        EncodedResult = result.EncodedBits;

        foreach (SyndromeRow row in result.SyndromeTable)
        {
            SyndromeRows.Add(row);
        }
    }

    [RelayCommand]
    private void CheckError()
    {
        int errorPosition = _hammingService.FindErrorPosition(CheckInput);

        if (errorPosition == 0)
        {
            ErrorResult = "Ошибок нет";
        }
        else
        {
            ErrorResult = $"Ошибка в бите № {errorPosition}";
        }
    }
}