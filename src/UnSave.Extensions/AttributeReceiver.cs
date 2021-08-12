using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnSave.Extensions
{
    public class AttributeReceiver : ISyntaxReceiver
    {
        public List<PropertyDeclarationSyntax> ViewCandidates { get; } = new();
        public Dictionary<string, ClassDeclarationSyntax> Candidates { get; } = new();
        
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is not AttributeSyntax attribute) {
                return;
            }

            var name = attribute.Name.ExtractName();
            if (name is "SaveProperty" or "SavePropertyAttribute") {
                // "attribute.Parent" is "AttributeListSyntax"
                // "attribute.Parent.Parent" is a C# fragment the attribute is applied to
                if (attribute.Parent?.Parent is ClassDeclarationSyntax classDeclarationSyntax) {
                    // NamedCandidates.Add(attribute.ArgumentList);
                    var className = classDeclarationSyntax.GetFullName();
                    if (!Candidates.ContainsKey(className)) {
                        Candidates.Add(className, classDeclarationSyntax);
                    }
                    // Candidates.Add(classDeclarationSyntax);
                }
            }

            if (name is "ViewProperty" or "ViewPropertyAttribute") {
                if (attribute.Parent?.Parent is PropertyDeclarationSyntax propertyDeclaration) {
                    ViewCandidates.Add(propertyDeclaration);
                }
            }

        }
    }
}