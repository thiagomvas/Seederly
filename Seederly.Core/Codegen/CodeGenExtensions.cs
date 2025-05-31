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
            CodeLanguage.CSharpHttpClient => "C# - HttpClient",
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
            "c# - httpclient" => CodeLanguage.CSharpHttpClient,
            _ => throw new NotSupportedException($"Code generation for {language} is not supported.")
        };
    }
    
}