using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PuzzleSolverGenerateFactory
{

    [Generator]
    public class CreatePuzzleFactory : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // Create syntax provider which retrieves all classes which inherit from IPuzzle
            var provider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is ClassDeclarationSyntax,
                    transform: static (ctx, _) => ctx)
                .Select((ctx, cancellationToken) =>
                {
                    var classDeclarationSyntax = (ClassDeclarationSyntax)ctx.Node;
                    var semanticModel = ctx.SemanticModel;
                    var classSymbol = semanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken) as INamedTypeSymbol;

                    if (classSymbol == null || classSymbol.IsAbstract)
                    {
                        return null;
                    }


                    var interfaceSymbol = semanticModel.Compilation.GetTypeByMetadataName("PuzzleSolver.Puzzles.IPuzzle");

                    if (interfaceSymbol == null)
                    {
                        return null;
                    }

                    if (classSymbol.AllInterfaces.Contains(interfaceSymbol))
                    {
                        return classDeclarationSyntax;
                    }

                    return null;
                })
                .Where(m => m is not null);

            var compilation = context.CompilationProvider.Combine(provider.Collect());

            // Register the provider
            context.RegisterSourceOutput(compilation, OnCreateSource);
        }

        private void OnCreateSource(SourceProductionContext context, (Compilation Left, System.Collections.Immutable.ImmutableArray<ClassDeclarationSyntax> Right) tuple)
        {
            var (compilation, classDeclarations) = tuple;
            var sb = new StringBuilder();

            sb.AppendLine(@"using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using PuzzleSolver.Puzzles;
    using PuzzleSolver.Puzzles.Y2023;

    namespace PuzzleSolver.Services
    {
        public class PuzzleNotAvailable : IPuzzle
        {
            public string Solve()
            {
                Console.WriteLine(""Puzzle not available"");
                return null;
            }
            public string Solve(ReadOnlySpan<char> path)
            {
                return Solve();
            }
        }

        public partial class PuzzleFactoryService : IPuzzleFactoryService
        {
            public IPuzzle CreatePuzzle(int year, int day, int part)
            {");
            foreach (var classDeclaration in classDeclarations)
            {
                var semanticModel = compilation.GetSemanticModel(classDeclaration.SyntaxTree);
                var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
                if (classSymbol == null)
                {
                    continue;
                }
                if (!int.TryParse(classSymbol.ContainingNamespace.Name.Substring(1), out var year)) continue;

                var nameChars = classSymbol.Name.ToCharArray(); 
                var part = (int)(nameChars.Last() &~ 0x20) - (int)'A' + 1;
                if (!int.TryParse(classSymbol.Name.Substring(3, classSymbol.Name.Length - 4), out var day)) continue;

                sb.AppendLine();
                sb.AppendLine($"\t\t\tif (year == {year} && day == {day} && part == {part})");
                sb.AppendLine($"\t\t\t\treturn new {classSymbol.Name}();");
            }
            sb.AppendLine(@"

                return new PuzzleNotAvailable();

            }
        }
    }");
            context.AddSource("PuzzleFactoryService.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }
    }
}
