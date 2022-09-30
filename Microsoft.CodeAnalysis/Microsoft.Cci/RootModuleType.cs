using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.Cci
{
    public class RootModuleType : INamespaceTypeDefinition, INamedTypeDefinition, ITypeDefinition, IDefinition, IReference, ITypeReference, INamedTypeReference, INamedEntity, INamespaceTypeReference
    {
        private IReadOnlyList<IMethodDefinition>? _methods;

        public TypeDefinitionHandle TypeDef => default(TypeDefinitionHandle);

        public ITypeDefinition ResolvedType => this;

        public bool MangleName => false;

        public string Name => "<Module>";

        public ushort Alignment => 0;

        public bool HasDeclarativeSecurity => false;

        public bool IsAbstract => false;

        public bool IsBeforeFieldInit => false;

        public bool IsComObject => false;

        public bool IsGeneric => false;

        public bool IsInterface => false;

        public bool IsDelegate => false;

        public bool IsRuntimeSpecial => false;

        public bool IsSerializable => false;

        public bool IsSpecialName => false;

        public bool IsWindowsRuntimeImport => false;

        public bool IsSealed => false;

        public LayoutKind Layout => LayoutKind.Auto;

        public uint SizeOf => 0u;

        public CharSet StringFormat => CharSet.Ansi;

        public bool IsPublic => false;

        public bool IsNested => false;

        IEnumerable<IGenericTypeParameter> ITypeDefinition.GenericParameters
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        ushort ITypeDefinition.GenericParameterCount => 0;

        IEnumerable<SecurityAttribute> ITypeDefinition.SecurityAttributes
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        bool ITypeReference.IsEnum
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        bool ITypeReference.IsValueType
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        PrimitiveTypeCode ITypeReference.TypeCode
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        ushort INamedTypeReference.GenericParameterCount
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        string INamespaceTypeReference.NamespaceName => string.Empty;

        IGenericMethodParameterReference? ITypeReference.AsGenericMethodParameterReference => null;

        IGenericTypeInstanceReference? ITypeReference.AsGenericTypeInstanceReference => null;

        IGenericTypeParameterReference? ITypeReference.AsGenericTypeParameterReference => null;

        INamespaceTypeReference ITypeReference.AsNamespaceTypeReference => this;

        INestedTypeReference? ITypeReference.AsNestedTypeReference => null;

        ISpecializedNestedTypeReference? ITypeReference.AsSpecializedNestedTypeReference => null;

        public void SetStaticConstructorBody(ImmutableArray<byte> il)
        {
            _methods = SpecializedCollections.SingletonReadOnlyList(new RootModuleStaticConstructor(this, il));
        }

        public IEnumerable<IMethodDefinition> GetMethods(EmitContext context)
        {
            return _methods ?? (_methods = SpecializedCollections.EmptyReadOnlyList<IMethodDefinition>());
        }

        public IEnumerable<ICustomAttribute> GetAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<ICustomAttribute>();
        }

        public ITypeReference? GetBaseClass(EmitContext context)
        {
            return null;
        }

        public IEnumerable<IEventDefinition> GetEvents(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<IEventDefinition>();
        }

        public IEnumerable<MethodImplementation> GetExplicitImplementationOverrides(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<MethodImplementation>();
        }

        public IEnumerable<IFieldDefinition> GetFields(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<IFieldDefinition>();
        }

        public IEnumerable<TypeReferenceWithAttributes> Interfaces(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<TypeReferenceWithAttributes>();
        }

        public IEnumerable<INestedTypeDefinition> GetNestedTypes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<INestedTypeDefinition>();
        }

        public IEnumerable<IPropertyDefinition> GetProperties(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<IPropertyDefinition>();
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }

        ITypeDefinition ITypeReference.GetResolvedType(EmitContext context)
        {
            return this;
        }

        IUnitReference INamespaceTypeReference.GetUnit(EmitContext context)
        {
            throw ExceptionUtilities.Unreachable;
        }

        INamespaceTypeDefinition ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
        {
            return this;
        }

        INestedTypeDefinition? ITypeReference.AsNestedTypeDefinition(EmitContext context)
        {
            return null;
        }

        ITypeDefinition ITypeReference.AsTypeDefinition(EmitContext context)
        {
            return this;
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return this;
        }

        ISymbolInternal? IReference.GetInternalSymbol()
        {
            return null;
        }

        public sealed override bool Equals(object? obj)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override int GetHashCode()
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
