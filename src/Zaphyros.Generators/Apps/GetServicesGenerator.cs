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

namespace Zaphyros.Generators.Apps
{
    [Generator]
    internal sealed class GetServicesGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            /*
            var compilation = (CSharpCompilation)context.Compilation;
            var types = compilation.Assembly.GetAllTypes();
            var commandAttributeSymbol = compilation.GetTypeByMetadataName("Abyss.Core.Commands.CommandAttribute");
            var serviceBaseType = compilation.GetTypeByMetadataName("Abyss.Core.Apps.Service");
            var servicesTypes = types.Where(t => !t.IsAbstract && t.IsDerivedFrom(serviceBaseType)).OrderBy(t => t.ContainingNamespace.MetadataName).ThenBy(t => t.MetadataName);
            SourceText sourceText;
            string commandName;
            string commandDescription;
            using (StringWriter stringWriter = new StringWriter())
            using (IndentedTextWriter indentedTextWriter = new IndentedTextWriter(stringWriter, "\t"))
            {
                var lastNameSpace = servicesTypes.First().ContainingNamespace;

                indentedTextWriter.WriteLine("using Abyss.Core.Commands;");
                indentedTextWriter.WriteLine();
                indentedTextWriter.WriteNameSpaceHead(servicesTypes.First());
                indentedTextWriter.StartBlock();

                foreach (var type in servicesTypes)
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

                    indentedTextWriter.WriteLine($"internal partial class {type.Name} : {serviceBaseType.Name}");
                    indentedTextWriter.StartBlock();
                    indentedTextWriter.WriteLine($"private {type.Name}(string name, string description, CommandType type) : base(new(name, description, type)) {{}}");
                    indentedTextWriter.WriteLine($"public {type.Name}() : this(\"{commandName}\",\"{commandDescription}\", CommandType.{type.ContainingNamespace.MetadataName}) {{}}");
                    indentedTextWriter.CloseBlock();
                }
                indentedTextWriter.CloseBlock();
                indentedTextWriter.Flush();
                sourceText = SourceText.From(stringWriter.ToString(), Encoding.UTF8);
            }
            context.AddSource($"{nameof(GetServicesGenerator)}.cs", sourceText.ToString());
            */
        }
    }
}
