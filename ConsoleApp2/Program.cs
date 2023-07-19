using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var tree = CSharpSyntaxTree.ParseText("""
                                      namespace Test
                                      {
                                          public class A
                                          {
                                              public int someField;
                                          }
                                      }
                                      """);

var classNode = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();

var returnToken = SyntaxFactory.Token(SyntaxKind.IntKeyword);

var returnNode = SyntaxFactory.PredefinedType(returnToken);

// 方式一
var method = SyntaxFactory
    .MethodDeclaration(returnNode, "Calculate")
    .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PublicKeyword)))
    .WithBody(SyntaxFactory.Block())
    .NormalizeWhitespace();

var newClassNode = classNode.AddMembers(method).NormalizeWhitespace();

// 方式二
var members = classNode.Members.Insert(1, method);

var newClassNode2 = classNode.Update(
    classNode.AttributeLists,
    classNode.Modifiers,
    classNode.Keyword,
    classNode.Identifier,
    classNode.TypeParameterList,
    classNode.BaseList,
    classNode.ConstraintClauses,
    classNode.OpenBraceToken,
    members,
    classNode.CloseBraceToken,
    classNode.SemicolonToken)
    .NormalizeWhitespace();

Console.WriteLine(newClassNode.ToString());
Console.WriteLine();
Console.WriteLine(newClassNode2.ToString());

Console.ReadLine();
