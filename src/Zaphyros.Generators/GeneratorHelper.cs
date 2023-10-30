using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Zaphyros.Generators
{
    internal static class GeneratorHelper
    {
        internal static void WriteNameSpaceHead(this IndentedTextWriter indentedTextWriter, INamedTypeSymbol type)
        {
            indentedTextWriter.WriteLine($"namespace {type.ContainingNamespace}");
        }
        internal static void StartBlock(this IndentedTextWriter indentedTextWriter)
        {
            indentedTextWriter.WriteLine("{");
            indentedTextWriter.Indent++;
        }
        internal static void CloseBlock(this IndentedTextWriter indentedTextWriter)
        {
            indentedTextWriter.Indent--;
            indentedTextWriter.WriteLine("}");
        }

        internal static IReadOnlyList<INamedTypeSymbol> GetAllTypes(this IAssemblySymbol symbol)
        {
            var result = symbol.GlobalNamespace.GetAllTypes();
            result.OrderBy(t => t.ContainingNamespace.MetadataName).ThenBy(x => x.MetadataName);
            return result;
        }

        internal static List<INamedTypeSymbol> GetAllTypes(this INamespaceOrTypeSymbol symbol)
        {
            var result = new List<INamedTypeSymbol>();

            if (symbol is INamedTypeSymbol type)
            {
                result.Add(type);
            }

            foreach (var child in symbol.GetMembers())
            {
                if (child is INamespaceOrTypeSymbol nsChild)
                {
                    result.AddRange(nsChild.GetAllTypes());
                }
            }

            return result;
        }
        internal static bool IsDerivedFrom(this ITypeSymbol type, INamedTypeSymbol baseType)
        {
            while (type != null)
            {
                if (SymbolEqualityComparer.Default.Equals(type, baseType))
                {
                    return true;
                }

                type = type.BaseType;
            }
            return false;
        }

        internal static bool IsPartial(this INamedTypeSymbol type)
        {
            return MatchModifier(type, "partial");
        }
        internal static bool IsPublic(this INamedTypeSymbol type)
        {
            return MatchModifier(type, "public");
        }
        internal static bool IsSealed(this INamedTypeSymbol type)
        {
            return MatchModifier(type, "sealed");
        }

        private static bool MatchModifier(INamedTypeSymbol type, string modifierString)
        {
            foreach (var declaration in type.DeclaringSyntaxReferences)
            {
                var syntax = declaration.GetSyntax();
                if (syntax is ClassDeclarationSyntax typeDeclaration)
                {
                    return typeDeclaration.Modifiers.Any(t => t.ValueText == modifierString);
                }
            }
            return false;
        }
    }
}