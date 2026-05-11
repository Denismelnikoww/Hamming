using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HammingApp.Models;

namespace HammingApp.Services;

public class HammingService
{
    public string TextToBits(string text)
    {
        StringBuilder builder = new();

        foreach (char ch in text)
        {
            builder.Append(Convert.ToString(ch, 2).PadLeft(8, '0'));
        }

        return builder.ToString();
    }

    public HammingResult Encode(string input, bool isBinary)
    {
        string bits = isBinary
            ? input
            : TextToBits(input);

        List<int> data = bits
            .Select(x => x - '0')
            .ToList();

        int controlBitsCount = 0;

        while (Math.Pow(2, controlBitsCount) < data.Count + controlBitsCount + 1)
        {
            controlBitsCount++;
        }

        int totalLength = data.Count + controlBitsCount;

        int[] result = new int[totalLength + 1];

        int dataIndex = 0;

        for (int i = 1; i <= totalLength; i++)
        {
            if (IsPowerOfTwo(i))
            {
                result[i] = 0;
            }
            else
            {
                result[i] = data[dataIndex++];
            }
        }

        for (int i = 0; i < controlBitsCount; i++)
        {
            int position = 1 << i;

            int parity = 0;

            for (int j = 1; j <= totalLength; j++)
            {
                if ((j & position) != 0)
                {
                    parity ^= result[j];
                }
            }

            result[position] = parity;
        }

        string encoded = string.Join("", result.Skip(1));

        HammingResult hammingResult = new()
        {
            SourceBits = bits,
            EncodedBits = encoded
        };

        for (int i = 1; i <= totalLength; i++)
        {
            hammingResult.SyndromeTable.Add(new SyndromeRow
            {
                Position = i,
                Value = result[i],
                IsControlBit = IsPowerOfTwo(i)
            });
        }

        return hammingResult;
    }

    public int FindErrorPosition(string encodedBits)
    {
        int[] bits = new int[encodedBits.Length + 1];

        for (int i = 0; i < encodedBits.Length; i++)
        {
            bits[i + 1] = encodedBits[i] - '0';
        }

        int syndrome = 0;

        int controlBits = (int)Math.Ceiling(Math.Log2(encodedBits.Length));

        for (int i = 0; i < controlBits; i++)
        {
            int position = 1 << i;

            int parity = 0;

            for (int j = 1; j <= encodedBits.Length; j++)
            {
                if ((j & position) != 0)
                {
                    parity ^= bits[j];
                }
            }

            if (parity != 0)
            {
                syndrome += position;
            }
        }

        return syndrome;
    }

    private bool IsPowerOfTwo(int number)
    {
        return (number & (number - 1)) == 0;
    }
}