using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Zaphyros.Generators.Commands
{
    [Generator]
    internal class CommandTypeGenerator : ISourceGenerator
    {
        private const string command = "command";
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = (CSharpCompilation)context.Compilation;
            var types = compilation.Assembly.GetAllTypes();
            var commandBaseType = compilation.GetTypeByMetadataName("Zaphyros.Core.Commands.CommandBase");
            var namespaces = commandBaseType.ContainingNamespace.GetNamespaceMembers();
            SourceText sourceText;
            using (StringWriter stringWriter = new StringWriter())
            using (IndentedTextWriter indentedTextWriter = new IndentedTextWriter(stringWriter, "\t"))
            {

                indentedTextWriter.WriteLine("using Zaphyros.Core.Commands;");
                indentedTextWriter.WriteLine();
                indentedTextWriter.WriteNameSpaceHead(commandBaseType);
                indentedTextWriter.StartBlock();

                indentedTextWriter.WriteLine("public enum CommandType");
                indentedTextWriter.StartBlock();
                indentedTextWriter.WriteLine($"Info,");
                indentedTextWriter.WriteLine($"Miscellaneous,");
                foreach (var @namespace in namespaces)
                {
                    indentedTextWriter.WriteLine($"{@namespace.Name},");
                }
                indentedTextWriter.CloseBlock();
                indentedTextWriter.CloseBlock();
                indentedTextWriter.Flush();
                sourceText = SourceText.From(stringWriter.ToString(), Encoding.UTF8);
            }
            context.AddSource($"{nameof(CommandDefinitionGenerator)}.cs", sourceText.ToString());
        }
    }
}
