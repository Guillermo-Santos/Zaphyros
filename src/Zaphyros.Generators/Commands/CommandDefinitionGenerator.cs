using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Zaphyros.Generators;

namespace Zaphyros.Generators.Commands
{
    [Generator]
    internal sealed class CommandDefinitionGenerator : ISourceGenerator
    {
        private const string command = "command";
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = (CSharpCompilation)context.Compilation;
            var types = compilation.Assembly.GetAllTypes();
            var commandAttributeSymbol = compilation.GetTypeByMetadataName("Zaphyros.Core.Commands.CommandAttribute");
            var commandBaseType = compilation.GetTypeByMetadataName("Zaphyros.Core.Commands.CommandBase");
            var commandBaseTypes = types.Where(t => !t.IsAbstract && t.IsPartial() && t.IsDerivedFrom(commandBaseType)).OrderBy(t => t.ContainingNamespace.MetadataName).ThenBy(t => t.MetadataName);
            SourceText sourceText;
            string commandName;
            string commandDescription;
            using (StringWriter stringWriter = new StringWriter())
            using (IndentedTextWriter indentedTextWriter = new IndentedTextWriter(stringWriter, "\t"))
            {
                var lastNameSpace = commandBaseTypes.First().ContainingNamespace;

                indentedTextWriter.WriteLine("using Zaphyros.Core.Commands;");
                indentedTextWriter.WriteLine();
                indentedTextWriter.WriteNameSpaceHead(commandBaseTypes.First());
                indentedTextWriter.StartBlock();

                foreach (var type in commandBaseTypes)
                {
                    if (!SymbolEqualityComparer.Default.Equals(type.ContainingNamespace, lastNameSpace))
                    {
                        indentedTextWriter.CloseBlock();

                        indentedTextWriter.WriteLine();

                        indentedTextWriter.WriteNameSpaceHead(type);
                        indentedTextWriter.StartBlock();
                        lastNameSpace = type.ContainingNamespace;
                    }

                    var attribute = type.GetAttributes().FirstOrDefault(attr => attr.AttributeClass.Equals(commandAttributeSymbol, SymbolEqualityComparer.Default));

                    if (attribute is null)
                    {
                        commandName = type.Name.ToLower();
                        commandName = commandName.EndsWith(command) ? commandName.Substring(0, commandName.Length - command.Length) : commandName;
                        commandDescription = commandName;
                    }
                    else
                    {
                        commandName = attribute.ConstructorArguments[0].Value.ToString();
                        commandDescription = attribute.ConstructorArguments[1].Value.ToString();
                    }

                    indentedTextWriter.WriteLine($"internal partial class {type.Name} : {commandBaseType.Name}");
                    indentedTextWriter.StartBlock();
                    indentedTextWriter.WriteLine($"private {type.Name}(string name, string description, CommandType type) : base(new(name, description, type)) {{}}");
                    indentedTextWriter.WriteLine($"public {type.Name}() : this(\"{commandName}\",\"{commandDescription}\", CommandType.{type.ContainingNamespace.MetadataName}) {{}}");
                    indentedTextWriter.CloseBlock();
                }
                indentedTextWriter.CloseBlock();
                indentedTextWriter.Flush();
                sourceText = SourceText.From(stringWriter.ToString(), Encoding.UTF8);
            }
            context.AddSource($"{nameof(CommandDefinitionGenerator)}.cs", sourceText.ToString());
        }
    }
}
