using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Seederly.Core;

public static class Utils
{
    public static void Write(Dictionary<string, Func<string>> generators)
    {
        const int padding = 30;

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Available Schema Generators:");
        Console.WriteLine(new string('-', 60));
        Console.ResetColor();

        foreach (var generator in generators.OrderBy(g => g.Key))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(generator.Key.PadRight(padding));

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"E.g.: {generator.Value()}");

            Console.ResetColor();
        }

        Console.WriteLine();
    }
    public static void Write(ApiResponse response)
    {
        // Status Code Coloring
        if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            Console.ForegroundColor = ConsoleColor.Green;
        else if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
            Console.ForegroundColor = ConsoleColor.Yellow;
        else if ((int)response.StatusCode >= 500)
            Console.ForegroundColor = ConsoleColor.Red;
        else
            Console.ForegroundColor = ConsoleColor.Gray;

        Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
        Console.ResetColor();
        Console.WriteLine();

        try
        {
            var json = JsonNode.Parse(response.Content);
            var pretty = json.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            WriteColorizedJson(pretty);
        }
        catch
        {
            // If not JSON, print as raw text
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(response.Content);
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    private static void WriteColorizedJson(string json)
    {
        foreach (var line in json.Split('\n'))
        {
            var trimmed = line.TrimStart();
            var indent = line.Length - trimmed.Length;
            Console.Write(new string(' ', indent));

            var tokens = Regex.Matches(trimmed, "\"(.*?)\"|\\{|\\}|\\[|\\]|:|,|\\d+|true|false|null");

            int lastIndex = 0;

            foreach (Match token in tokens)
            {
                // Write any in-between text (like whitespace or unexpected chars)
                if (token.Index > lastIndex)
                {
                    Console.ResetColor();
                    Console.Write(trimmed.Substring(lastIndex, token.Index - lastIndex));
                }

                var val = token.Value;
                if (val.StartsWith("\"") && token.Value.EndsWith("\""))
                {
                    if (IsPropertyName(token, trimmed))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow; // Key
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan; // String value
                    }
                }
                else if (val == "{" || val == "}" || val == "[" || val == "]" || val == ":")
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray; // Brackets / Colon
                }
                else if (val == ",")

                {
                    Console.ForegroundColor = ConsoleColor.DarkGray; // Comma
                }
                else if (Regex.IsMatch(val, @"^\d+$"))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta; // Numbers
                }
                else if (val is "true" or "false")
                {
                    Console.ForegroundColor = ConsoleColor.Green; // Booleans
                }
                else if (val == "null")
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan; // Null
                }

                Console.Write(val);
                Console.ResetColor();

                lastIndex = token.Index + token.Length;
            }

            // Write any remaining part of the line
            if (lastIndex < trimmed.Length)
            {
                Console.ResetColor();
                Console.Write(trimmed.Substring(lastIndex));
            }

            Console.WriteLine();
        }
    }

    private static bool IsPropertyName(Match token, string line)
    {
        // If the token is followed by a colon, it's a key
        var index = token.Index + token.Length;
        return index < line.Length && line[index..].TrimStart().StartsWith(":");
    }
}
