using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnSave.Types;

namespace UnSave.Extensions
{
    [Generator]
    public class ValuePropertyGenerator : ISourceGenerator
    {
        private static string GetName(PropertyDeclarationSyntax propSyntax) {
            var fullName = (propSyntax.Parent as ClassDeclarationSyntax)?.GetFullName() ?? string.Empty;
            return $"{fullName}.{propSyntax.Identifier}";
        }
        public void Initialize(GeneratorInitializationContext context) {
            // context.RegisterForSyntaxNotifications(() => new AttributeReceiver<ValuePropertyAttribute, PropertyDeclarationSyntax(s => s.));
            context.RegisterForSyntaxNotifications(() => new AttributeReceiver<ValuePropertyAttribute, PropertyDeclarationSyntax>(GetName));
#if DEBUGGEN
            if (!Debugger.IsAttached)
            {
                Debugger.Launch();
            }
#endif 
        }

        public void Execute(GeneratorExecutionContext context) {
            var receiver = (AttributeReceiver<ValuePropertyAttribute, PropertyDeclarationSyntax>)context.SyntaxReceiver;

            foreach (var propertyDeclarationSyntax in receiver.Candidates.Select(c => c.Value)) {
                var model = context.Compilation.GetSemanticModel(propertyDeclarationSyntax.SyntaxTree, true);
                var declaredSymbol = model.GetDeclaredSymbol(propertyDeclarationSyntax);
                var property = declaredSymbol as IPropertySymbol;
                if (property is null) continue;
                var code = GenerateValuePropertyCode(property, model, context);
                var classSym = property.ContainingType.Name;
                context.AddSource($"{classSym}_{property.Name}_Generated.cs", code);
            }
        }

        private static string GenerateValuePropertyCode(IPropertySymbol property, SemanticModel model, GeneratorExecutionContext context) {
            
            
            var ns = property.ContainingNamespace.ToString();
            var name = property.Name;
            var targetType = property.Type;
            var baseType = property.Type.BaseType; //this is UnrealPropertyBase<something>
            var targetPropertyType = baseType.TypeArguments.First();
            var classSym = property.ContainingType.Name;
            
            var builder = new ClassBuilder(ns, classSym)
                .AddImport(targetPropertyType.ContainingNamespace)
                .AddImport(targetType.ContainingNamespace);
            
            var attr = property.GetAttributes().First();
            // var viewPropName = attr.ConstructorArguments.First(a => a.Type.Name == "String").Value as string;
            var viewPropertySpecifier =
                attr.NamedArguments.FirstOrDefault(na => na.Key == nameof(ValuePropertyAttribute.ValuePropertyName));
            var viewPropertyName = string.IsNullOrWhiteSpace(viewPropertySpecifier.Key)
                ? $"{name}Value"
                : viewPropertySpecifier.Value.Value as string;
            var readOnlyProperty = attr.GetFlag(nameof(ValuePropertyAttribute.ReadOnly));
            var allowDefault = attr.GetFlag(nameof(ValuePropertyAttribute.AllowDefault));
            var createProp = attr.GetValue(nameof(ValuePropertyAttribute.CreateProperty));
            var getter = @$"get {{ return {name}?.Value; }}";
            var setter = string.Empty;
            if (!readOnlyProperty) {
                if (!string.IsNullOrWhiteSpace(createProp)) {
                    var defaultCondition = allowDefault ? string.Empty : $" && flValue != default({targetPropertyType}";
                    var classProps = property.ContainingType.GetProperties(nameof(GvasSaveData));
                    var saveDataProp = classProps.FirstOrDefault();
                    if (saveDataProp != null) {
                        setter = $@"set
    {{
            if ({name}?.Value is not null && value is not null)
            {{
                {name}.Value = ({targetPropertyType})value;
            }}
            else if ({name}?.Value is null && value is {targetPropertyType} flValue{defaultCondition}))
            {{
                //value is good, but property isn't
                {saveDataProp.Name}.Properties.Insert({saveDataProp.Name}.Properties.Count - 1, new {targetType.Name}
                {{
                    Name = ""{createProp}"",
                    Value = ({targetPropertyType})value
                }});
            }}
    }}";
                    }
                    else {
                        var descriptor = new DiagnosticDescriptor(
                            "USVP:InvalidCreation",
                            "Couldn't find a valid GvasSaveData property",
                            "Couldn't find a valid GvasSaveData proeprty in '{0}'",
                            nameof(ValuePropertyGenerator),
                            DiagnosticSeverity.Warning,
                            true);
                        context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None, property.ContainingType.Name));
                    }
                }
                else {
                        setter =
                            $"set {{ if ({name}?.Value is not null) {name}.Value = ({targetPropertyType})value; }}";
                }
            }
            builder.AddProperty(targetPropertyType, viewPropertyName, getter, setter);
            var gen = builder.Build();
            return gen;

        }

        
    }
}