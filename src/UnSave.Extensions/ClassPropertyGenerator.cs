using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnSave.Extensions
{
    [Generator]
    public class ClassPropertyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context) {
            context.RegisterForSyntaxNotifications(() =>
                new AttributeReceiver<SavePropertyAttribute, ClassDeclarationSyntax>(c => c.GetFullName()));
        }

        public void Execute(GeneratorExecutionContext context) {
            var receiver = (AttributeReceiver<SavePropertyAttribute, ClassDeclarationSyntax>)context.SyntaxReceiver;
            
            foreach (var classDeclarationSyntax in receiver.Candidates.Select(c => c.Value))
            {
                var model = context.Compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree, true);
                var declaredSymbol = model.GetDeclaredSymbol(classDeclarationSyntax);

                var field = declaredSymbol as ITypeSymbol;

                if (field is null)
                    continue;
                
                var code = GenerateClassPropertyCode(field, model);
                if (code is not null) {
                    context.AddSource($"{field.Name}_Generated.cs", code);
                }
            }
        }
        
        private static string? GenerateClassPropertyCode(ITypeSymbol classSymbol, SemanticModel model) {
            var ns = classSymbol.ContainingNamespace.ToString();
            var className = classSymbol.Name;
            var builder = new ClassBuilder(ns, className);

            var saveDataProp = classSymbol.GetMembers().Where(m => m is IPropertySymbol).Cast<IPropertySymbol>()
                .FirstOrDefault(p => p.Type.Name.Contains(nameof(GvasSaveData)));
            if (saveDataProp != null) {
                // var propCodes = new List<string>();
                // var nss = new List<string>();
                var attrs = classSymbol.GetAttributes();
                foreach (var attr in attrs) {
                    var savePropName = attr.ConstructorArguments.First(a => a.Type.Name == "String").Value as string;
                    var savePropType = attr.ConstructorArguments.First(a => a.Kind == TypedConstantKind.Type).Value as ITypeSymbol;
                    var baseType = savePropType.BaseType; //this is UnrealPropertyBase<something>
                    builder.AddImport(baseType.ContainingNamespace);
                    var targetPropertyType = baseType.TypeArguments.First();
                    builder.AddImport(targetPropertyType.ContainingNamespace);
                    var viewPropertyName = attr.GetValue(nameof(SavePropertyAttribute.PropertyName), savePropName);
                    var includeValueProperty = attr.GetFlag(nameof(SavePropertyAttribute.IncludeValueProperty));
                    
                    var propCode =
                        $@"public {savePropType.Name}? {viewPropertyName} => {saveDataProp.Name}.Properties.FindProperty<{savePropType.Name}>(p => p.Name == {('"' + savePropName + '"')});";
                    if (includeValueProperty) {
                        propCode = propCode.AddAfter(
                            $"public {targetPropertyType.Name}? {viewPropertyName}Value => {viewPropertyName}?.Value;");
                    }

                    builder.AddMember(propCode);
                }

                var gen = builder.Build();
                return gen;
            }

            return null;
        }
    }
}