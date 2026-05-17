using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class StringExtensions
{
    public static int IndexOfWithoutStringLiteral(this string input, char target)
    {
        bool inQuotes = false;
        bool escape = false;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == target && !inQuotes)
            {
                return i;
            }

            if (c == '"' && !escape)
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (inQuotes)
            {
                if (escape)
                {
                    escape = false;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
            }
        }
        return -1;
    }

    public static string[] SplitWithoutStringLiteral(this string input, char target)
    {
        if (string.IsNullOrEmpty(input)) return Array.Empty<string>();

        List<string> result = new();

        var sb = new StringBuilder();
        bool inQuotes = false;
        bool escape = false;
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (c == target && !inQuotes)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }

            if (c == '"' && !escape)
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (inQuotes)
            {
                if (escape)
                {
                    escape = false;
                }
                else if (c == '\\')
                {
                    escape = true;
                }
            }
        }

        result.Add(sb.ToString());

        return result.ToArray();
    }
}
