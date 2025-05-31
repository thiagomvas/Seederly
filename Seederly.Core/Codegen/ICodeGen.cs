namespace Seederly.Core.Codegen;

public interface ICodeGen
{
    string GenerateCode(ApiRequest request);
}