using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var tree = CSharpSyntaxTree.ParseText(
@"using System;
using System.Collections;
using System.Linq;
using System.Text;
 
// HelloWorld
namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
        }
    }
}");

var root = tree.GetCompilationUnitRoot();

var namespaceNode = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();
var classNode = root.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

if (namespaceNode is not null)
{
    Console.WriteLine(namespaceNode.GetLeadingTrivia().ToFullString());
}

var oldUsing = root.Usings[0];

NameSyntax name = SyntaxFactory.IdentifierName("System");

name = SyntaxFactory.QualifiedName(name, SyntaxFactory.IdentifierName("Collections"));

name = SyntaxFactory.QualifiedName(name, SyntaxFactory.IdentifierName("Generic"));

var newUsing = oldUsing
    .WithName(name)
    .WithTrailingTrivia(SyntaxFactory.Whitespace("\r\n\r\n"));

var newUsing2 = SyntaxFactory
    .UsingDirective(SyntaxFactory.ParseName("System.Threading.Tasks"))
    .NormalizeWhitespace()
    .WithTrailingTrivia(SyntaxFactory.Whitespace(Environment.NewLine));

root = root
    .ReplaceNode(oldUsing, newUsing)
    .AddUsings(newUsing2);

//var usings = root.Usings
//    .Replace(oldUsing, newUsing)
//    .Add(newUsing2);

//root = root.WithUsings(usings);

Console.WriteLine(root.ToFullString());
Console.ReadLine();
