using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

// This attribute will be used in consuming code
[Generator]
public class HiFromGeneratorAttributeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Step 1: Register the attribute source (so consuming projects get the attribute automatically)
        context.RegisterPostInitializationOutput(ctx =>
        {
            const string attributeCode = @"
                namespace HiGenerator
                {
                    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
                    public sealed class HiFromGeneratorAttribute : System.Attribute { }
                }
                ";
            ctx.AddSource(
                "HiFromGeneratorAttribute.g.cs",
                SourceText.From(attributeCode, Encoding.UTF8));
        });

        // Step 2: Find classes decorated with our attribute (incremental style - modern & efficient)
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax classDecl &&
                    classDecl.AttributeLists.Count > 0,
                transform: static (ctx, _) =>
                {
                    var classDecl = (Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax)ctx.Node;
                    var semanticModel = ctx.SemanticModel;

                    foreach (var attrList in classDecl.AttributeLists)
                    {
                        foreach (var attr in attrList.Attributes)
                        {
                            var symbol = semanticModel.GetSymbolInfo(attr).Symbol;
                            if (symbol is IMethodSymbol method &&
                                method.ContainingType?.ToDisplayString() == "HiGenerator.HiFromGeneratorAttribute")
                            {
                                return semanticModel.GetDeclaredSymbol(classDecl);
                            }
                        }
                    }
                    return null;
                })
            .Where(static m => m is not null);

        // Step 3: Generate code for each matching class
        context.RegisterSourceOutput(provider, static (spc, classSymbol) =>
        {
            if (classSymbol is null) return;

            var ns = classSymbol.ContainingNamespace?.ToDisplayString() ?? "Global";
            var className = classSymbol.Name;

            var generatedCode = $@"
                namespace {ns}
                {{

                    partial class {className}
                    {{
                        public string HiFromGeneratedCode()
                        {{
                            return ""Hi from Source Generator! (generated for {className} at compile-time)"";
                        }}
                    }}
                }}
                ";

            spc.AddSource(
                $"{className}_Hi.g.cs",
                SourceText.From(generatedCode, Encoding.UTF8));
        });
    }
}