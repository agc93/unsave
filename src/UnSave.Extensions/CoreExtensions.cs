using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnSave.Extensions
{
    public static class CoreExtensions
    {
        internal static string ExtractName(this TypeSyntax type)
        {
            while (type != null)
            {
                switch (type)
                {
                    case IdentifierNameSyntax ins:
                        return ins.Identifier.Text;

                    case QualifiedNameSyntax qns:
                        type = qns.Right;
                        break;

                    default:
                        return null;
                }
            }

            return null;
        }
        
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => input.First().ToString().ToUpper() + input.Substring(1)
            };
        
        static bool TypeSymbolMatchesType(ITypeSymbol typeSymbol, Type type, SemanticModel semanticModel)
        {
            return GetTypeSymbolForType(type, semanticModel).Equals(typeSymbol);
        }

        internal static INamedTypeSymbol GetTypeSymbolForType(this Type type, SemanticModel semanticModel)
        {

            if (!type.IsConstructedGenericType)
            {
                return semanticModel.Compilation.GetTypeByMetadataName(type.FullName);
            }

            // get all typeInfo's for the Type arguments 
            var typeArgumentsTypeInfos = type.GenericTypeArguments.Select(a => GetTypeSymbolForType(a, semanticModel));

            var openType = type.GetGenericTypeDefinition();
            var typeSymbol = semanticModel.Compilation.GetTypeByMetadataName(openType.FullName);
            return typeSymbol.Construct(typeArgumentsTypeInfos.ToArray<ITypeSymbol>());
        }
    }
}