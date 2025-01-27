using System;
using System.Collections.Generic;
using System.Linq;
using wikrypted.Models;

namespace wikrypted;

public static class Gameplay
{
    public static void DecodeGameplay(WikipediaArticle originalArticle)
    {
        Krypter krypter = Krypter.Instance;
        krypter.SetKey();
        WikipediaArticle encodedArticle = new WikipediaArticle
        {
            Url = originalArticle.Url,
            Title = krypter.Encode(originalArticle.Title),
            Description = krypter.Encode(originalArticle.Description)
        };

        Dictionary<char, char> lookupTable = new Dictionary<char, char>();
        int cursorX = 0, cursorY = 0;
        List<string> lines = WrapLineToConsoleWidth(originalArticle.ToString());

        while (true)
        {
            Console.Clear();
            DisplayArticle(originalArticle, encodedArticle, lookupTable, cursorX, cursorY);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.UpArrow) cursorY = Math.Max(0, cursorY - 1);
            else if (keyInfo.Key == ConsoleKey.DownArrow) cursorY = Math.Min(lines.Count - 1, cursorY + 1);
            else if (keyInfo.Key == ConsoleKey.LeftArrow) cursorX = Math.Clamp(cursorX-1, 0, lines[cursorY].Length - 1);
            else if (keyInfo.Key == ConsoleKey.RightArrow) cursorX = Math.Clamp(cursorX+1, 0, lines[cursorY].Length - 1);
            else if (char.IsLetterOrDigit(keyInfo.KeyChar) || char.IsPunctuation(keyInfo.KeyChar))
            {
                char originalChar = lines[cursorY][cursorX];
                lookupTable[originalChar] = keyInfo.KeyChar;
            }
            else if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                char originalChar = lines[cursorY][cursorX];
                lookupTable[originalChar] = ' ';
            }
            else if (keyInfo.Key == ConsoleKey.Backspace)
            {
                foreach (var letter in originalArticle.Description)
                {
                    if (!lookupTable.ContainsKey(letter) || char.ToLower(lookupTable[letter]) != char.ToLower(letter))
                    {
                        lookupTable[letter] = letter;
                        break;
                    }
                }
            }
            else if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                Console.WriteLine(originalArticle.ToString());
                Console.WriteLine($"\n{originalArticle.Url}\nArticle not decoded!");
                break;
            }

            if (IsDecoded(originalArticle.ToString(), encodedArticle.ToString(), lookupTable))
            {
                Console.Clear();
                Console.WriteLine(originalArticle.ToString());
                Console.WriteLine($"\n{originalArticle.Url}\nArticle decoded successfully!");
                break;
            }
        }
    }

    private static void DisplayArticle(WikipediaArticle originalArticle, WikipediaArticle encodedArticle, Dictionary<char, char> lookupTable, int cursorX, int cursorY)
    {
        List<string> linesOriginal = WrapLineToConsoleWidth(originalArticle.ToString());
        List<string> linesEncoded = WrapLineToConsoleWidth(encodedArticle.ToString());
        
        string buffer = "";
        
        for (int y = 0; y < linesEncoded.Count; y++)
        {
            for (int x = 0; x < linesEncoded[y].Length; x++)
            {
                char originalChar = linesOriginal[y][x];
                char displayChar = linesEncoded[y][x];
                if (lookupTable.ContainsKey(originalChar))
                {
                    displayChar = lookupTable[originalChar];
                }

                if (char.IsControl(displayChar))
                {
                    displayChar = '?';
                }

                if (x == cursorX && y == cursorY)
                {
                    Console.Write(buffer);
                    buffer = "";
                }
                
                buffer += displayChar;
                
                if (x == cursorX && y == cursorY)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(buffer);
                    Console.ResetColor();
                    buffer = "";
                }
            }
            buffer += "\n";
        }
        Console.Write(buffer);
    }

    private static bool IsDecoded(string original, string encoded, Dictionary<char, char> lookupTable)
    {
        for (int i = 0; i < encoded.Length; i++)
        {
            char encodedChar = encoded[i];
            char originalChar = original[i];
            if (lookupTable.ContainsKey(originalChar))
            {
                if (lookupTable[originalChar] != originalChar && char.ToLower(lookupTable[originalChar]) != char.ToLower(originalChar))
                {
                    return false;
                }
            }
            else if (encodedChar != originalChar && char.ToLower(encodedChar) != char.ToLower(originalChar))
            {
                return false;
            }
        }
        return true;
    }

    private static List<string> WrapLineToConsoleWidth(string content)
    {
        int consoleWidth = Console.BufferWidth;
        string[] lines = content.Split('\n');
        
        List<string> result = new List<string>();

        foreach (var line in lines)
        {
            if (line.Length <= consoleWidth)
            {
                result.Add(line);
            }
            else
            {
                int start = 0;
                while (start < line.Length)
                {
                    int end = start + consoleWidth;
                    if (end >= line.Length)
                    {
                        result.Add(line.Substring(start));
                        break;
                    }
                    else
                    {
                        result.Add(line.Substring(start, consoleWidth));
                        start = end;
                    }
                }
            }
        }

        return result;
    }
}