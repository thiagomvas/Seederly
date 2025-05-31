using Cocona;
using Seederly.Cli;
using Seederly.Core;
using Seederly.Core.Automation;
using Seederly.Core.Codegen;
using Seederly.Core.OpenApi;

var request = new ApiRequest
{
    Method = HttpMethod.Post,
    Url = "https://httpbin.org/post",
    QueryParameters =
    {
        { "param1", "value1" },
        { "param2", "value2" }
    },
    Body = "{ \"key\": \"value\" }",
}.WithJwt("token");

var codeGen = CodeGeneratorFactory.Create(CodeLanguage.Httpie).GenerateCode(request);
Console.WriteLine(codeGen);
    