namespace Seederly.Core.Codegen;

public static class CodeGenExtensions
{
    public static string ToFormattedString(this CodeLanguage language)
    {
        return language switch
        {
            CodeLanguage.Curl => "cURL",
            CodeLanguage.Httpie => "HTTPie",
            CodeLanguage.JSFetch => "JS - fetch",
            _ => throw new NotSupportedException($"Code generation for {language} is not supported.")
        };
    }
    
    public static CodeLanguage ToCodeLanguage(this string language)
    {
        return language.ToLowerInvariant() switch
        {
            "curl" => CodeLanguage.Curl,
            "httpie" => CodeLanguage.Httpie,
            "js - fetch" => CodeLanguage.JSFetch,
            _ => throw new NotSupportedException($"Code generation for {language} is not supported.")
        };
    }
    
}