using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnSave.Extensions
{
    public class AttributeReceiver<TAttribute, TSyntax> : ISyntaxReceiver where TSyntax : MemberDeclarationSyntax
    {
        private readonly Func<TSyntax,string> _nameFunc;
        public Dictionary<string, TSyntax> Candidates { get; } = new();
        /*public AttributeReceiver() {
            
        }*/

        public AttributeReceiver(Func<TSyntax, string> nameFunc) {
            _nameFunc = nameFunc;
        }
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is not AttributeSyntax attribute) {
                return;
            }
            
            var name = attribute.Name.ExtractName();
            var attrTypeName = typeof(TAttribute).Name;
            if (name == attrTypeName || $"{name}Attribute" == attrTypeName ||
                name.Replace("Attribute", string.Empty) == attrTypeName) {
                if (attribute.Parent?.Parent is TSyntax declarationSyntax) {
                    // NamedCandidates.Add(attribute.ArgumentList);
                    var className = _nameFunc(declarationSyntax);
                    if (!Candidates.ContainsKey(className)) {
                        Candidates.Add(className, declarationSyntax);
                    }
                }
            }
        }
    }
}