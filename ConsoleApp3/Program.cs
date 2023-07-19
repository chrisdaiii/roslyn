using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Text;

var content = """
              namespace ConsoleApp2
              {
                  public class HelloWorld
                  {
                      public void SayHi()
                      {
                          System.Diagnostics.Debugger.Break();

                          System.Console.WriteLine("Hello world!");
                      }
                  }
              } 
              """;

var filePath = $"{Path.GetTempPath()}\\generated.cs";

File.WriteAllText(filePath, content, Encoding.UTF8);

var syntaxTree = CSharpSyntaxTree.ParseText(content, CSharpParseOptions.Default, filePath, Encoding.UTF8);

var references = new MetadataReference[]
{
    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System")).Location),
    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location),
};

var compilation = CSharpCompilation.Create(
    "HelloWorld",
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

    dynamic instance = assembly.CreateInstance("ConsoleApp2.HelloWorld");

    instance.SayHi();
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

File.Delete(filePath);

Console.ReadLine();
