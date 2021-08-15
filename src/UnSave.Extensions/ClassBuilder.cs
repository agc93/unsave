﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnSave.Extensions
{
    public class ClassBuilder
    {
        public ClassBuilder(string ns, string className) {
            ClassName = className;
            Namespace = ns;
        }

        private string Namespace { get; }

        private string ClassName { get; }

        private List<string> Namespaces { get; } = new();
        public ClassBuilder AddImport(string ns) {
            Namespaces.Add(ns);
            return this;
        }

        public ClassBuilder AddImport(INamespaceSymbol nsSyntax) {
            Namespaces.Add(nsSyntax.ToString());
            return this;
        }
        
        public ClassBuilder AddImport(INamedTypeSymbol typeSymbol) {
            Namespaces.Add(typeSymbol.ContainingNamespace.ToString());
            return this;
        }

        public ClassBuilder AddImports(IEnumerable<string> ns) {
            Namespaces.AddRange(ns);
            return this;
        }

        public ClassBuilder AddMember(string memberCode) {
            Members.Add(memberCode);
            return this;
        }

        public ClassBuilder AddProperty(ITypeSymbol propertyType, string propertyName, string propertyBody) {
            Members.Add($@"public {propertyType.Name}? {propertyName} {{
    {propertyBody}
}}");
            return this;
        }

        public List<string> Members { get; } = new();

        public string Build() {
            var usings = string.Join(Environment.NewLine, Namespaces.Distinct().Select(ns => $"using {ns};"));
            var members = string.Join(Environment.NewLine, Members);
            var gen = $@"// <auto-generated />
#nullable enable annotations
using System.Collections.Generic;
{usings}

{(String.IsNullOrWhiteSpace(Namespace) ? null : $"namespace {Namespace}")}
{{
   public partial class {ClassName}
   {{
      {members}
   }}
}}
#nullable restore annotations
";
            return gen;
        }
        
        
    }
}