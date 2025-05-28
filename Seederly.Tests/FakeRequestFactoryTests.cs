using System.Text.Json.Nodes;
using Seederly.Core;

namespace Seederly.Tests;

public class FakeRequestFactoryTests
{
    [Test]
    public void Generate_ShouldGenerate_Something()
    {
        var data = FakeRequestFactory.Instance.Generate(new() {{"name", "name.fullName"}});

        Assert.That(data, Is.Not.Null);
        Assert.That(data["name"]?.GetValue<string>(), Is.Not.Null);
        Assert.That(data["name"]?.GetValue<string>(), Is.Not.Empty);
    }
    
    [Test]
    public void CreateFakeRequest_ShouldReturnValidRequest()
    {
        var request = FakeRequestFactory.Instance.GenerateRequest("https://example.com/api/test", HttpMethod.Get, new() {{"Foo", "name.fullName"}});

        Assert.That(request, Is.Not.Null);
        Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
        Assert.That(request.Url, Is.EqualTo("https://example.com/api/test"));
        Assert.That(request.Headers, Is.Not.Null);
        Assert.That(request.Body, Is.Not.Null);
    }

    [Test]
    public void Generate_ShouldGenerateArrays()
    {
        var data = FakeRequestFactory.Instance.Generate(new() { { "arr[3]", "name.fullName" } });
        Assert.Multiple(() =>
        {
            Assert.That(data["arr"]?.AsArray(), Is.Not.Null);
            Assert.That(data["arr"]?.AsArray(), Has.Count.EqualTo(3));
        });
    }

    [Test]
    public void Generate_ShouldGenerateNestedObjects()
    {
        var data = FakeRequestFactory.Instance.Generate(new() {{"obj.nested", "name.fullName" }});
        
        Assert.That(data["obj"]?.AsObject(), Is.Not.Null);
    }

    [Test]
    public void Generate_ShouldGenerateObjectArrays()
    {
        var data = FakeRequestFactory.Instance.Generate(new()
        {
            {"objArr[2]", "object" },
            {"objArr[].name", "name.fullName"}
        });
        
        Assert.That(data["objArr"]?.AsArray(), Is.Not.Null);
        Assert.That(data["objArr"]?.AsArray(), Has.Count.EqualTo(2));
        Assert.That(data["objArr"]?.AsArray()[0]?.AsObject()?["name"], Is.Not.Null);
        Assert.That(data["objArr"]?.AsArray()[1]?.AsObject()?["name"], Is.Not.Null);
    }

    [Test]
    public void Generate_ShouldGenerateDeepNestedObjects()
    {
        var data = FakeRequestFactory.Instance.Generate(new() {{"a.b.c.d.e", "name.fullName" }});
        
        Assert.That(data["a"]?.AsObject(), Is.Not.Null, "Did not generate 'a'");
        Assert.That(data["a"]?["b"]?.AsObject(), Is.Not.Null, "Did not generate 'b'");
        Assert.That(data["a"]?["b"]?["c"]?.AsObject(), Is.Not.Null, "Did not generate 'c'");
        Assert.That(data["a"]?["b"]?["c"]?["d"]?.AsObject(), Is.Not.Null, "Did not generate 'd'");
        Assert.That(data["a"]?["b"]?["c"]?["d"]?["e"]?.GetValue<string>(), Is.Not.Null, "Did not generate 'e'");
    }

    [Test]
    public void Generate_WhenGeneratingObjectArrays_ShouldIgnoreDeclarationOrder()
    {
        var data = FakeRequestFactory.Instance.Generate(new()
        {
            {"objArr[].name", "name.fullName"},
            {"objArr[2]", "object" },
        });   
        
        Assert.That(data["objArr"]?.AsArray(), Is.Not.Null);
        Assert.That(data["objArr"]?.AsArray(), Has.Count.EqualTo(2));
        Assert.That(data["objArr"]?.AsArray()[0]?.AsObject()?["name"], Is.Not.Null);
        Assert.That(data["objArr"]?.AsArray()[1]?.AsObject()?["name"], Is.Not.Null);
    }

    [Test]
    public void GenerateBody_ShouldGenerateFromBody()
    {
        var request = new ApiRequest()
        {
            Body =
                @"
{
    name: {{name.fullName}},
"
        };
        FakeRequestFactory.Instance.GenerateBody(request);
        
        Assert.That(request.Body, Is.Not.Null);
        Assert.That(request.Body, Is.Not.Empty);
        Assert.That(request.Body, Does.Contain("name: "));
    }
}