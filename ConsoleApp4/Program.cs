using ConsoleApp3;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Text;

var content = """
              @using ConsoleApp3;

              @inherits TemplateBase
              @{
                System.Diagnostics.Debugger.Break();

                var a = 1;

                @a
              }
              """;

var rootDirPath = Path.GetTempPath();

var fileName = "generated.cshtml";

var filePath = $"{rootDirPath}{fileName}";

File.WriteAllText(filePath, content);

var engine = RazorProjectEngine.Create(RazorConfiguration.Default, RazorProjectFileSystem.Create(rootDirPath));

var sourceDocument = RazorSourceDocument.Create(content, fileName);

var codeDocument = engine.Process(sourceDocument, null, Array.Empty<RazorSourceDocument>(), Array.Empty<TagHelperDescriptor>());

var csharpDocument = codeDocument.GetCSharpDocument();

var syntaxTree = CSharpSyntaxTree.ParseText(csharpDocument.GeneratedCode);

var references = new MetadataReference[]
{
    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    MetadataReference.CreateFromFile(typeof(TemplateBase).Assembly.Location),
    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System")).Location),
    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
};

var compilation = CSharpCompilation.Create(
    "Hello",
    new[] { syntaxTree },
    references,
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

using var peStream = new MemoryStream();
using var pdbStream = new MemoryStream();

var result = compilation.Emit(peStream, pdbStream);

if (result.Success)
{
    peStream.Seek(0, SeekOrigin.Begin);
    pdbStream.Seek(0, SeekOrigin.Begin);

    var assembly = Assembly.Load(peStream.ToArray(), pdbStream.ToArray());

    dynamic instance = assembly.CreateInstance("Razor.Template");

    instance.ExecuteAsync();
}
else
{
    foreach (var diagnostic in result.Diagnostics.Where(
        diagnostic => diagnostic.IsWarningAsError ||
        diagnostic.Severity == DiagnosticSeverity.Error))
    {
        Console.WriteLine($"{diagnostic.Id}: {diagnostic.GetMessage()}");
    }
}

//File.Delete(filePath);

Console.ReadLine();
