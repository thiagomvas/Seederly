using Seederly.Core;

namespace Seederly.Tests;

public class WorkspaceTests
{
    [Test]
    public void Workspace_ShouldSerializeCorrectly()
    {
        var workspace = new Workspace("Test")
        {
            Path = "/test/foo/bar.json",
            Endpoints =
            [
                new ApiEndpoint()
                {
                    Name = "Test Endpoint",
                    Request = new()
                    {
                        Method = HttpMethod.Patch,
                        Url = "https://example.com/api/test",
                        Headers = new Dictionary<string, string>
                        {
                            { "Authorization", "Bearer token" }
                        },
                        Body = "{\"key\":\"value\"}",
                    },
                }
            ]
        };
        
        var json = workspace.SerializeToJson();
        var deserializedWorkspace = Workspace.DeserializeFromJson(json);
        
        Assert.That(deserializedWorkspace, Is.Not.Null);
        Assert.That(deserializedWorkspace.Name, Is.EqualTo(workspace.Name));
        Assert.That(deserializedWorkspace.Path, Is.EqualTo(workspace.Path));
        Assert.That(deserializedWorkspace.Endpoints.Count, Is.EqualTo(workspace.Endpoints.Count));
    }
}