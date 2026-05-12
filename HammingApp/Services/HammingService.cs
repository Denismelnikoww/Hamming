using HammingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HammingApp.Services;

public class HammingService
{
    public HammingFullDecodeResult DecodeToAscii(string encodedBits)
    {
        // Сначала находим и исправляем ошибку
        var decodeResult = Decode(encodedBits);
        
        // Создаем результат полного декодирования
        var fullResult = new HammingFullDecodeResult
        {
            SyndromeTable = decodeResult.SyndromeTable,
            SyndromeRows = decodeResult.SyndromeRows,
            SyndromeBits = decodeResult.SyndromeBits,
            ErrorPosition = decodeResult.ErrorPosition
        };
        
        // Исправляем ошибку, если она есть
        List<int> correctedBits = encodedBits.Select(x => x - '0').ToList();
        
        if (decodeResult.ErrorPosition > 0 && decodeResult.ErrorPosition <= correctedBits.Count)
        {
            // Инвертируем бит в позиции ошибки
            correctedBits[decodeResult.ErrorPosition - 1] ^= 1;
            fullResult.ErrorCorrected = true;
            fullResult.CorrectedCode = string.Join("", correctedBits);
        }
        else
        {
            fullResult.ErrorCorrected = false;
            fullResult.CorrectedCode = encodedBits;
        }
        
        // Извлекаем информационные биты (все позиции, которые не являются степенями двойки)
        List<int> dataBits = new List<int>();
        int totalLength = correctedBits.Count;
        
        for (int i = 1; i <= totalLength; i++)
        {
            if (!IsPowerOfTwo(i))
            {
                dataBits.Add(correctedBits[i - 1]);
            }
        }
        
        // Сохраняем бинарную строку
        fullResult.BinaryString = string.Join("", dataBits);
        
        // Конвертируем бинарную строку в ASCII текст
        fullResult.AsciiText = ConvertBitsToAscii(fullResult.BinaryString);
        
        return fullResult;
    }
    
    // Замените метод ConvertToBits
    private string ConvertToBits(string input)
    {
        StringBuilder builder = new();
    
        // Используем ASCII кодировку (7 бит)
        byte[] bytes = Encoding.ASCII.GetBytes(input);
    
        foreach (byte b in bytes)
        {
            // Конвертируем в 7-битный ASCII (добавляем ведущий 0 для 8-битного представления)
            builder.Append(Convert.ToString(b, 2).PadLeft(7, '0'));
        }
    
        return builder.ToString();
    }

// Замените метод ConvertBitsToAscii
    private string ConvertBitsToAscii(string bits)
    {
        if (string.IsNullOrEmpty(bits))
            return string.Empty;
    
        // ASCII использует 7 бит на символ
        int bitsPerChar = 7;
    
        // Если количество бит не кратно 7, пробуем 8 бит (на случай если старые данные)
        if (bits.Length % bitsPerChar != 0)
        {
            bitsPerChar = 8;
            if (bits.Length % bitsPerChar != 0)
                return "Ошибка: неверный формат данных";
        }
    
        StringBuilder result = new();
    
        for (int i = 0; i < bits.Length; i += bitsPerChar)
        {
            string byteStr = bits.Substring(i, bitsPerChar);
            int charCode = Convert.ToInt32(byteStr, 2);
        
            // Проверяем, что это печатный ASCII символ
            if (charCode >= 32 && charCode <= 126)
            {
                result.Append((char)charCode);
            }
            else if (charCode == 10) // LF
            {
                result.Append('\n');
            }
            else if (charCode == 13) // CR
            {
                result.Append('\r');
            }
            else if (charCode == 9) // TAB
            {
                result.Append('\t');
            }
            else
            {
                result.Append('?'); // Непечатный символ
            }
        }
    
        return result.ToString();
    }
    
    public HammingEncodeResult Encode(string input, bool isBinary)
    {
        string bits = isBinary
            ? input
            : ConvertToBits(input);

        List<int> dataBits = bits
            .Select(x => x - '0')
            .ToList();

        int parityCount = CalculateParityBitsCount(dataBits.Count);

        int totalLength = dataBits.Count + parityCount;

        int[] code = new int[totalLength + 1];

        int dataIndex = 0;

        for (int i = 1; i <= totalLength; i++)
        {
            if (IsPowerOfTwo(i))
            {
                code[i] = 0;
            }
            else
            {
                code[i] = dataBits[dataIndex++];
            }
        }

        var result = new HammingEncodeResult();

        result.InitialTable = BuildInitialTable(code, totalLength);

        result.MatrixTable = BuildMatrixTable(code, totalLength, parityCount);

        result.Calculations = BuildCalculations(code, totalLength, parityCount);

        for (int i = 0; i < parityCount; i++)
        {
            int parityPosition = 1 << i;

            int parity = 0;

            for (int j = 1; j <= totalLength; j++)
            {
                if ((j & parityPosition) != 0)
                {
                    parity ^= code[j];
                }
            }

            code[parityPosition] = parity;
        }

        result.FinalCode = string.Join("", code.Skip(1));

        return result;
    }

    private int CalculateParityBitsCount(int dataLength)
    {
        int s = 0;

        while (Math.Pow(2, s) < dataLength + s + 1)
        {
            s++;
        }

        return s;
    }

    private bool IsPowerOfTwo(int x)
    {
        return (x & (x - 1)) == 0;
    }

    private HammingVisualTable BuildInitialTable(
        int[] code,
        int totalLength)
    {
        var table = new HammingVisualTable();

        for (int i = 1; i <= totalLength; i++)
        {
            table.Headers.Add(i.ToString());
        }

        var names = new HammingVisualRow
        {
            Name = "Бит"
        };

        int infoIndex = 1;
        int parityIndex = 0;

        for (int i = 1; i <= totalLength; i++)
        {
            if (IsPowerOfTwo(i))
            {
                names.Values.Add($"s{parityIndex}");
                parityIndex++;
            }
            else
            {
                names.Values.Add($"x{infoIndex}");
                infoIndex++;
            }
        }

        var values = new HammingVisualRow
        {
            Name = "Значение"
        };

        for (int i = 1; i <= totalLength; i++)
        {
            values.Values.Add(code[i].ToString());
        }

        table.Rows.Add(names);
        table.Rows.Add(values);

        return table;
    }

    private HammingVisualTable BuildMatrixTable(
        int[] code,
        int totalLength,
        int parityCount)
    {
        var table = new HammingVisualTable();

        for (int i = 1; i <= totalLength; i++)
        {
            table.Headers.Add(i.ToString());
        }

        var valuesRow = new HammingVisualRow
        {
            Name = "Код"
        };

        for (int i = 1; i <= totalLength; i++)
        {
            valuesRow.Values.Add(code[i].ToString());
        }

        table.Rows.Add(valuesRow);

        for (int s = 0; s < parityCount; s++)
        {
            var row = new HammingVisualRow
            {
                Name = $"s{s}"
            };

            int mask = 1 << s;

            for (int col = 1; col <= totalLength; col++)
            {
                row.Values.Add(
                    (col & mask) != 0 ? "1" : "0"
                );
            }

            table.Rows.Add(row);
        }

        return table;
    }

    private List<ParityCalculation> BuildCalculations(
        int[] code,
        int totalLength,
        int parityCount)
    {
        var calculations = new List<ParityCalculation>();

        for (int s = 0; s < parityCount; s++)
        {
            int mask = 1 << s;

            List<string> parts = new();

            int sum = 0;

            for (int i = 1; i <= totalLength; i++)
            {
                if ((i & mask) != 0)
                {
                    parts.Add(code[i].ToString());

                    sum += code[i];
                }
            }

            int result = sum % 2;

            calculations.Add(new ParityCalculation
            {
                Name = $"s{s}",
                Formula =
                    $"({string.Join(" + ", parts)}) mod 2 = {result}",
                Result = result
            });
        }

        return calculations;
    }

    public HammingDecodeResult Decode(string encodedBits)
    {
        List<int> bits = encodedBits
            .Select(x => x - '0')
            .ToList();

        int totalLength = bits.Count;

        int parityCount = 0;

        while (Math.Pow(2, parityCount) < totalLength + 1)
        {
            parityCount++;
        }

        var result = new HammingDecodeResult();

        result.SyndromeTable =
            BuildSyndromeTable(bits, parityCount);

        List<int> syndromeBits = new();

        for (int s = 0; s < parityCount; s++)
        {
            int mask = 1 << s;

            List<string> expression = new();

            int sum = 0;

            for (int i = 1; i <= totalLength; i++)
            {
                if ((i & mask) != 0)
                {
                    expression.Add(bits[i - 1].ToString());

                    sum += bits[i - 1];
                }
            }

            int resultBit = sum % 2;

            syndromeBits.Add(resultBit);

            result.SyndromeRows.Add(new SyndromeRow
            {
                Name = $"s{s}",

                Formula =
                    $"({string.Join(" + ", expression)}) mod 2 = {resultBit}",

                Result = resultBit
            });
        }

        string syndromeBinary =
            string.Join("", syndromeBits.Reverse<int>());

        result.SyndromeBits = syndromeBinary;

        result.ErrorPosition =
            Convert.ToInt32(syndromeBinary, 2);

        return result;
    }

    private HammingVisualTable BuildSyndromeTable(
    List<int> bits,
    int parityCount)
    {
        var table = new HammingVisualTable();

        for (int i = 1; i <= bits.Count; i++)
        {
            table.Headers.Add(i.ToString());
        }

        var codeRow = new HammingVisualRow
        {
            Name = "Код"
        };

        foreach (int bit in bits)
        {
            codeRow.Values.Add(bit.ToString());
        }

        table.Rows.Add(codeRow);

        for (int s = 0; s < parityCount; s++)
        {
            var row = new HammingVisualRow
            {
                Name = $"s{s}"
            };

            int mask = 1 << s;

            for (int i = 1; i <= bits.Count; i++)
            {
                row.Values.Add(
                    (i & mask) != 0
                        ? "1"
                        : "0"
                );
            }

            table.Rows.Add(row);
        }

        return table;
    }
}