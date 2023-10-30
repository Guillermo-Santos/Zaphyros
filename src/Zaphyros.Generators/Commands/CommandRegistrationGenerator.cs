using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using System.Diagnostics;

namespace Zaphyros.Generators.Commands
{
    [Generator]
    internal sealed class CommandRegistrationGenerator : ISourceGenerator
    {

        public void Initialize(GeneratorInitializationContext context)
        {
        }
        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = (CSharpCompilation)context.Compilation;
            var types = compilation.Assembly.GetAllTypes();
            var commandBaseType = compilation.GetTypeByMetadataName("Zaphyros.Core.Commands.CommandBase");
            var commandHandlerType = compilation.GetTypeByMetadataName("Zaphyros.Core.CommandHandler");
            var commandBaseTypes = types.Where(t => !t.IsAbstract && t.IsDerivedFrom(commandBaseType)).OrderBy(t => t.ContainingNamespace.MetadataName).ThenBy(t => t.MetadataName);
            SourceText sourceText;

            using (StringWriter stringWriter = new StringWriter())
            using (IndentedTextWriter indentedTextWriter = new IndentedTextWriter(stringWriter, "\t"))
            {
                indentedTextWriter.WriteNameSpaceHead(commandHandlerType);
                indentedTextWriter.StartBlock();// NameSpace Block
                indentedTextWriter.WriteLine($"public partial class CommandHandler");
                indentedTextWriter.StartBlock();// Class Block
                indentedTextWriter.WriteLine($"private partial void RegisterDefaultCommands()");
                indentedTextWriter.StartBlock();// Method Body

                foreach (var type in commandBaseTypes)
                {
                    indentedTextWriter.WriteLine($"RegisterCommand( new {type.ContainingNamespace}.{type.Name}());");
                }

                indentedTextWriter.CloseBlock();// Method Body
                indentedTextWriter.CloseBlock();// Class Block
                indentedTextWriter.CloseBlock();// NameSpace Block
                indentedTextWriter.Flush();

                sourceText = SourceText.From(stringWriter.ToString(), Encoding.UTF8);
            }
            context.AddSource($"{nameof(CommandRegistrationGenerator)}.cs", sourceText.ToString());
        }
    }
}