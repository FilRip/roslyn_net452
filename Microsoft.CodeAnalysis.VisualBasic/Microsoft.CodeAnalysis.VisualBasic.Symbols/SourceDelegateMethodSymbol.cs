using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceDelegateMethodSymbol : SourceMethodSymbol
	{
		private sealed class Constructor : SourceDelegateMethodSymbol
		{
			public override string Name => ".ctor";

			public Constructor(NamedTypeSymbol delegateType, TypeSymbol voidType, TypeSymbol objectType, TypeSymbol intPtrType, VisualBasicSyntaxNode syntax, Binder binder)
				: base(delegateType, syntax, binder, SourceMemberFlags.AccessibilityPublic | SourceMemberFlags.Dim | SourceMemberFlags.Static, voidType)
			{
				InitializeParameters(ImmutableArray.Create((ParameterSymbol)new SynthesizedParameterSymbol(this, objectType, 0, isByRef: false, "TargetObject"), (ParameterSymbol)new SynthesizedParameterSymbol(this, intPtrType, 1, isByRef: false, "TargetMethod")));
			}

			protected override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
			{
				return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
			}
		}

		private sealed class InvokeMethod : SourceDelegateMethodSymbol
		{
			public override string Name => "Invoke";

			public InvokeMethod(NamedTypeSymbol delegateType, TypeSymbol returnType, VisualBasicSyntaxNode syntax, Binder binder, ParameterListSyntax parameterListOpt, BindingDiagnosticBag diagnostics)
				: base(delegateType, syntax, binder, SourceMemberFlags.AccessibilityPublic | SourceMemberFlags.MethodKindDelegateInvoke | SourceMemberFlags.Overridable | ((returnType.SpecialType == SpecialType.System_Void) ? SourceMemberFlags.Dim : SourceMemberFlags.None), returnType)
			{
				InitializeParameters(binder.DecodeParameterListOfDelegateDeclaration(this, parameterListOpt, diagnostics));
			}
		}

		private sealed class BeginInvokeMethod : SourceDelegateMethodSymbol
		{
			public override string Name => "BeginInvoke";

			public BeginInvokeMethod(InvokeMethod invoke, TypeSymbol iAsyncResultType, TypeSymbol objectType, TypeSymbol asyncCallbackType, VisualBasicSyntaxNode syntax, Binder binder)
				: base(invoke.ContainingType, syntax, binder, SourceMemberFlags.AccessibilityPublic | SourceMemberFlags.MethodKindOrdinary | SourceMemberFlags.Overridable, iAsyncResultType)
			{
				ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
				int num = 0;
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = invoke.Parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					instance.Add(new SourceClonedParameterSymbol((SourceParameterSymbol)current, this, num));
					num++;
				}
				instance.Add(new SynthesizedParameterSymbol(this, asyncCallbackType, num, isByRef: false, "DelegateCallback"));
				num++;
				instance.Add(new SynthesizedParameterSymbol(this, objectType, num, isByRef: false, "DelegateAsyncState"));
				InitializeParameters(instance.ToImmutableAndFree());
			}

			protected override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
			{
				return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
			}
		}

		private sealed class EndInvokeMethod : SourceDelegateMethodSymbol
		{
			public override string Name => "EndInvoke";

			public EndInvokeMethod(InvokeMethod invoke, TypeSymbol iAsyncResultType, VisualBasicSyntaxNode syntax, Binder binder)
				: base(invoke.ContainingType, syntax, binder, SourceMemberFlags.AccessibilityPublic | SourceMemberFlags.MethodKindOrdinary | SourceMemberFlags.Overridable | ((invoke.ReturnType.SpecialType == SpecialType.System_Void) ? SourceMemberFlags.Dim : SourceMemberFlags.None), invoke.ReturnType)
			{
				ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
				int num = 0;
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = invoke.Parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					if (current.IsByRef)
					{
						instance.Add(new SourceClonedParameterSymbol((SourceParameterSymbol)current, this, num));
						num++;
					}
				}
				instance.Add(new SynthesizedParameterSymbol(this, iAsyncResultType, instance.Count, isByRef: false, "DelegateAsyncResult"));
				InitializeParameters(instance.ToImmutableAndFree());
			}

			protected override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
			{
				return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
			}
		}

		private ImmutableArray<ParameterSymbol> _parameters;

		private readonly TypeSymbol _returnType;

		public override TypeSymbol ReturnType => _returnType;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		internal override OverriddenMembersResult<MethodSymbol> OverriddenMembers => OverriddenMembersResult<MethodSymbol>.Empty;

		public sealed override bool IsExtensionMethod => false;

		internal override bool MayBeReducibleExtensionMethod => false;

		public sealed override bool IsExternalMethod => true;

		internal sealed override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.CodeTypeMask;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public sealed override bool IsImplicitlyDeclared => true;

		internal override bool GenerateDebugInfoImpl => false;

		internal sealed override bool HasSpecialName => MethodKind == MethodKind.Constructor;

		protected SourceDelegateMethodSymbol(NamedTypeSymbol delegateType, VisualBasicSyntaxNode syntax, Binder binder, SourceMemberFlags flags, TypeSymbol returnType)
			: base(delegateType, flags, binder.GetSyntaxReference(syntax), delegateType.Locations)
		{
			_returnType = returnType;
		}

		protected void InitializeParameters(ImmutableArray<ParameterSymbol> parameters)
		{
			_parameters = parameters;
		}

		internal static void MakeDelegateMembers(NamedTypeSymbol delegateType, VisualBasicSyntaxNode syntax, ParameterListSyntax parameterListOpt, Binder binder, out MethodSymbol constructor, out MethodSymbol beginInvoke, out MethodSymbol endInvoke, out MethodSymbol invoke, BindingDiagnosticBag diagnostics)
		{
			TypeSymbol returnType = BindReturnType(syntax, binder, diagnostics);
			NamedTypeSymbol specialType = binder.GetSpecialType(SpecialType.System_Void, syntax, diagnostics);
			NamedTypeSymbol specialType2 = binder.GetSpecialType(SpecialType.System_IAsyncResult, syntax, diagnostics);
			NamedTypeSymbol specialType3 = binder.GetSpecialType(SpecialType.System_Object, syntax, diagnostics);
			NamedTypeSymbol specialType4 = binder.GetSpecialType(SpecialType.System_IntPtr, syntax, diagnostics);
			NamedTypeSymbol specialType5 = binder.GetSpecialType(SpecialType.System_AsyncCallback, syntax, diagnostics);
			InvokeMethod invoke2 = (InvokeMethod)(invoke = new InvokeMethod(delegateType, returnType, syntax, binder, parameterListOpt, diagnostics));
			constructor = new Constructor(delegateType, specialType, specialType3, specialType4, syntax, binder);
			if (SymbolExtensions.IsCompilationOutputWinMdObj(delegateType))
			{
				beginInvoke = null;
				endInvoke = null;
			}
			else
			{
				beginInvoke = new BeginInvokeMethod(invoke2, specialType2, specialType3, specialType5, syntax, binder);
				endInvoke = new EndInvokeMethod(invoke2, specialType2, syntax, binder);
			}
		}

		private static TypeSymbol BindReturnType(VisualBasicSyntaxNode syntax, Binder binder, BindingDiagnosticBag diagnostics)
		{
			if (syntax.Kind() == SyntaxKind.DelegateFunctionStatement)
			{
				DelegateStatementSyntax delegateStatementSyntax = (DelegateStatementSyntax)syntax;
				Func<DiagnosticInfo> getRequireTypeDiagnosticInfoFunc = null;
				if (binder.OptionStrict == OptionStrict.On)
				{
					getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitProc;
				}
				else if (binder.OptionStrict == OptionStrict.Custom)
				{
					getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinFunction;
				}
				SimpleAsClauseSyntax asClause = delegateStatementSyntax.AsClause;
				return binder.DecodeIdentifierType(delegateStatementSyntax.Identifier, asClause, getRequireTypeDiagnosticInfoFunc, diagnostics);
			}
			return binder.GetSpecialType(SpecialType.System_Void, syntax, diagnostics);
		}

		public sealed override DllImportData GetDllImportData()
		{
			return null;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return ImmutableArray<string>.Empty;
		}

		protected sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
		}
	}
}
