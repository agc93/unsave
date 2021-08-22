using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnSave.Extensions
{
    public static class CoreExtensions
    {
        public static string ExtractName(this TypeSyntax? type)
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
        
        internal static string FirstCharToUpper(this string input) =>
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

        public static bool GetFlag(this AttributeData attr, string flagKey) {
            var readOnlyPropertySpecifier = 
                attr.NamedArguments.FirstOrDefault(na => na.Key == flagKey);
            var readOnlyProperty = !string.IsNullOrWhiteSpace(readOnlyPropertySpecifier.Key) && (bool.TryParse(readOnlyPropertySpecifier.Value.Value.ToString(), out var roProp) && roProp);
            return readOnlyProperty;
        }

        public static string? GetValue(this AttributeData attr, string key, string? defaultValue = null) {
            var viewPropertySpecifier =
                attr.NamedArguments.FirstOrDefault(na => na.Key == key);
            var viewPropertyName = string.IsNullOrWhiteSpace(viewPropertySpecifier.Key)
                ? defaultValue
                : viewPropertySpecifier.Value.Value as string;
            return viewPropertyName;
        }

        public static IEnumerable<IPropertySymbol> GetProperties(this ITypeSymbol classSymbol, string? propertyTypeMatch = null) {
            var classProps = classSymbol.GetMembers().Where(m => m is IPropertySymbol).Cast<IPropertySymbol>().ToList();
            var parent = classSymbol.BaseType;
            while (parent != null) {
                var baseProps = parent.GetMembers().Where(m => m is IPropertySymbol).Cast<IPropertySymbol>();
                classProps.AddRange(baseProps);
                parent = parent.BaseType;
            }

            return classProps.Where(p =>
                string.IsNullOrWhiteSpace(propertyTypeMatch) || p.Type.Name.Contains(propertyTypeMatch));
        }

        public static IEnumerable<ITypeSymbol> GetTypes(this ITypeSymbol classSymbol, string typeNameMatch = null) {
            var classProps = new List<ITypeSymbol>() { classSymbol };
            var parent = classSymbol.BaseType;
            while (parent != null) {
                classProps.Add(parent);
                parent = parent.BaseType;
            }

            return classProps.Where(p =>
                string.IsNullOrWhiteSpace(typeNameMatch) || p.Name.Contains(typeNameMatch));
        }
    }
}