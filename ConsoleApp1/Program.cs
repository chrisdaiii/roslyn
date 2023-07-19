using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var filePath = "C:\\Users\\June\\Desktop\\ConsoleApp1\\ConsoleApp1\\ServiceCollectionExtensions.cs";

            var content = File.ReadAllText(filePath);

            var syntaxTree = CSharpSyntaxTree.ParseText(content);

            var compilation = CSharpCompilation
                .Create("test")
                .AddSyntaxTrees(syntaxTree)
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var compilationUnit = syntaxTree.GetCompilationUnitRoot();

            var isMatch = false;

            MethodDeclarationSyntax? methodNode = null;

            foreach (var node in compilationUnit.DescendantNodes())
            {
                if (node is MethodDeclarationSyntax methodDeclarationSyntax && methodDeclarationSyntax.Identifier.Text == "AddServices")
                {
                    methodNode = methodDeclarationSyntax;

                    foreach (var item in methodDeclarationSyntax.DescendantNodes())
                    {
                        if (item is ExpressionStatementSyntax expressionStatementSyntax)
                        {
                            content = expressionStatementSyntax.ToString();

                            if (content == "services.AddSingleton<StudentService, StudentService>();")
                            {
                                isMatch = true;
                            }

                            Console.WriteLine(content);
                        }
                    }
                }
            }

            if (!isMatch && methodNode?.Body is not null)
            {
                var statement = SyntaxFactory.ParseStatement("services.AddSingleton<StudentService, StudentService>();");

                var statements = methodNode.Body.Statements.Insert(methodNode.Body.Statements.Count - 2, statement);

                var blockNode = methodNode.Body.Update(methodNode.Body.OpenBraceToken, statements, methodNode.Body.CloseBraceToken);

                var newMethodNode = methodNode.ReplaceNode(methodNode.Body, blockNode);

                compilationUnit = compilationUnit.ReplaceNode(methodNode, newMethodNode).NormalizeWhitespace();

                Console.WriteLine();
                Console.WriteLine("After modified result:");
                Console.WriteLine();
                Console.WriteLine(compilationUnit.ToString());
            }

            Console.Read();
        }
    }
}