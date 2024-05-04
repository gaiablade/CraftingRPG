using System;
using System.Collections.Generic;

namespace CraftingRPG.Utility;

public static class StringMethods
{
    public static string[] BreakUpString(string s, int charsPerLine)
    {
        var list = new List<string>();
        int currentIndex;
        var lastWrap = 0;

        do
        {
            currentIndex = lastWrap + charsPerLine > s.Length
                ? s.Length
                : (s.LastIndexOfAny(new[] { ' ', ',', '.', '?', '!', ':', ';', '-', '\n', '\r', '\t' },
                    Math.Min(s.Length - 1, lastWrap + charsPerLine)) + 1);

            if (currentIndex <= lastWrap)
                currentIndex = Math.Min(lastWrap + charsPerLine, s.Length);

            list.Add(s.Substring(lastWrap, currentIndex - lastWrap).Trim(' '));
            lastWrap = currentIndex;
        } while (currentIndex < s.Length);

        return list.ToArray();
    }
}