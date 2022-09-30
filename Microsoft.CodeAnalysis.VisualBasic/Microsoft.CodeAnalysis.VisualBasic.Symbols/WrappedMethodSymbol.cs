using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class WrappedMethodSymbol : MethodSymbol
	{
		public abstract MethodSymbol UnderlyingMethod { get; }

		public override bool IsVararg => UnderlyingMethod.IsVararg;

		public override bool IsGenericMethod => UnderlyingMethod.IsGenericMethod;

		public override int Arity => UnderlyingMethod.Arity;

		public override bool ReturnsByRef => UnderlyingMethod.ReturnsByRef;

		internal override int ParameterCount => UnderlyingMethod.ParameterCount;

		public override bool IsExtensionMethod => UnderlyingMethod.IsExtensionMethod;

		internal override bool IsHiddenBySignature => UnderlyingMethod.IsHiddenBySignature;

		public override ImmutableArray<Location> Locations => UnderlyingMethod.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => UnderlyingMethod.DeclaringSyntaxReferences;

		public override Accessibility DeclaredAccessibility => UnderlyingMethod.DeclaredAccessibility;

		public override bool IsShared => UnderlyingMethod.IsShared;

		public override bool IsExternalMethod => UnderlyingMethod.IsExternalMethod;

		public override bool IsAsync => UnderlyingMethod.IsAsync;

		public override bool IsOverrides => UnderlyingMethod.IsOverrides;

		public override bool IsMustOverride => UnderlyingMethod.IsMustOverride;

		public override bool IsNotOverridable => UnderlyingMethod.IsNotOverridable;

		public override bool IsImplicitlyDeclared => UnderlyingMethod.IsImplicitlyDeclared;

		internal override bool IsMetadataFinal => UnderlyingMethod.IsMetadataFinal;

		internal override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => UnderlyingMethod.ReturnTypeMarshallingInformation;

		internal override bool HasDeclarativeSecurity => UnderlyingMethod.HasDeclarativeSecurity;

		internal override ObsoleteAttributeData ObsoleteAttributeData => UnderlyingMethod.ObsoleteAttributeData;

		public override string Name => UnderlyingMethod.Name;

		internal override bool HasSpecialName => UnderlyingMethod.HasSpecialName;

		internal override MethodImplAttributes ImplementationAttributes => UnderlyingMethod.ImplementationAttributes;

		public override MethodKind MethodKind => UnderlyingMethod.MethodKind;

		internal override CallingConvention CallingConvention => UnderlyingMethod.CallingConvention;

		internal override bool IsAccessCheckedOnOverride => UnderlyingMethod.IsAccessCheckedOnOverride;

		internal override bool IsExternal => UnderlyingMethod.IsExternal;

		internal override bool HasRuntimeSpecialName => UnderlyingMethod.HasRuntimeSpecialName;

		internal override bool ReturnValueIsMarshalledExplicitly => UnderlyingMethod.ReturnValueIsMarshalledExplicitly;

		internal override ImmutableArray<byte> ReturnValueMarshallingDescriptor => UnderlyingMethod.ReturnValueMarshallingDescriptor;

		internal override bool IsMethodKindBasedOnSyntax => UnderlyingMethod.IsMethodKindBasedOnSyntax;

		public override bool IsIterator => UnderlyingMethod.IsIterator;

		public override bool IsInitOnly => UnderlyingMethod.IsInitOnly;

		internal override SyntaxNode Syntax => UnderlyingMethod.Syntax;

		public override bool IsOverloads => UnderlyingMethod.IsOverloads;

		internal override bool GenerateDebugInfoImpl => UnderlyingMethod.GenerateDebugInfoImpl;

		public override bool IsOverridable => UnderlyingMethod.IsOverridable;

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return UnderlyingMethod.IsMetadataNewSlot(ignoreInterfaceImplementationChanges);
		}

		public override DllImportData GetDllImportData()
		{
			return UnderlyingMethod.GetDllImportData();
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return UnderlyingMethod.GetSecurityInformation();
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return UnderlyingMethod.GetAppliedConditionalSymbols();
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UnderlyingMethod.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
