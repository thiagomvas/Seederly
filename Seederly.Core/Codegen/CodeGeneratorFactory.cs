namespace Seederly.Core.Codegen;

public static class CodeGeneratorFactory
{
    public static ICodeGen Create(CodeLanguage language)
    {
        return language switch
        {
            CodeLanguage.Curl => new CurlCodeGenerator(),
            CodeLanguage.Httpie => new HttpieCodeGenerator(),
            CodeLanguage.JSFetch => new JsFetchCodeGen(),
            _ => throw new NotSupportedException($"Code generation for {language} is not supported.")
        };
    }
}