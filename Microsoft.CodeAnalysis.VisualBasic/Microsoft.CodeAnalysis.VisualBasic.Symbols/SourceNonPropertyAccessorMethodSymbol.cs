using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SourceNonPropertyAccessorMethodSymbol : SourceMethodSymbol
	{
		private ImmutableArray<ParameterSymbol> _lazyParameters;

		private TypeSymbol _lazyReturnType;

		private OverriddenMembersResult<MethodSymbol> _lazyOverriddenMethods;

		internal sealed override int ParameterCount
		{
			get
			{
				if (!_lazyParameters.IsDefault)
				{
					return _lazyParameters.Length;
				}
				MethodBaseSyntax declarationSyntax = base.DeclarationSyntax;
				ParameterListSyntax parameterList;
				switch (declarationSyntax.Kind())
				{
				case SyntaxKind.SubNewStatement:
					parameterList = ((SubNewStatementSyntax)declarationSyntax).ParameterList;
					break;
				case SyntaxKind.SubStatement:
				case SyntaxKind.FunctionStatement:
					parameterList = ((MethodStatementSyntax)declarationSyntax).ParameterList;
					break;
				default:
					return base.ParameterCount;
				}
				return parameterList?.Parameters.Count ?? 0;
			}
		}

		public sealed override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				EnsureSignature();
				return _lazyParameters;
			}
		}

		public sealed override TypeSymbol ReturnType
		{
			get
			{
				EnsureSignature();
				return _lazyReturnType;
			}
		}

		internal sealed override OverriddenMembersResult<MethodSymbol> OverriddenMembers
		{
			get
			{
				EnsureSignature();
				return _lazyOverriddenMethods;
			}
		}

		protected SourceNonPropertyAccessorMethodSymbol(NamedTypeSymbol containingType, SourceMemberFlags flags, SyntaxReference syntaxRef, ImmutableArray<Location> locations = default(ImmutableArray<Location>))
			: base(containingType, flags, syntaxRef, locations)
		{
		}

		private void EnsureSignature()
		{
			if (!_lazyParameters.IsDefault)
			{
				return;
			}
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			SourceModuleSymbol containingSourceModule = base.ContainingSourceModule;
			ImmutableArray<ParameterSymbol> parameters = GetParameters(containingSourceModule, instance);
			SyntaxNodeOrToken errorLocation = default(SyntaxNodeOrToken);
			TypeSymbol destinationReturnType = GetReturnType(containingSourceModule, ref errorLocation, instance);
			OverriddenMembersResult<MethodSymbol> overriddenMembersResult;
			if (!base.IsOverrides || !OverrideHidingHelper.CanOverrideOrHide(this))
			{
				overriddenMembersResult = OverriddenMembersResult<MethodSymbol>.Empty;
			}
			else
			{
				ImmutableArray<TypeParameterSymbol> immutableArray;
				TypeSubstitution substitution;
				if (Arity > 0)
				{
					immutableArray = IndexedTypeParameterSymbol.Take(Arity);
					substitution = TypeSubstitution.Create(this, TypeParameters, StaticCast<TypeSymbol>.From(immutableArray));
				}
				else
				{
					immutableArray = ImmutableArray<TypeParameterSymbol>.Empty;
					substitution = null;
				}
				ArrayBuilder<ParameterSymbol> instance2 = ArrayBuilder<ParameterSymbol>.GetInstance(parameters.Length);
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					instance2.Add(new SignatureOnlyParameterSymbol(current.Type.InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly(), ImmutableArray<CustomModifier>.Empty, ImmutableArray<CustomModifier>.Empty, null, isParamArray: false, current.IsByRef, isOut: false, current.IsOptional));
				}
				overriddenMembersResult = OverrideHidingHelper<MethodSymbol>.MakeOverriddenMembers(new SignatureOnlyMethodSymbol(Name, m_containingType, MethodKind, base.CallingConvention, immutableArray, instance2.ToImmutableAndFree(), returnsByRef: false, destinationReturnType.InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly(), ImmutableArray<CustomModifier>.Empty, ImmutableArray<CustomModifier>.Empty, ImmutableArray<MethodSymbol>.Empty, isOverrides: true));
			}
			MethodSymbol overriddenMember = overriddenMembersResult.OverriddenMember;
			if ((object)overriddenMember != null)
			{
				CustomModifierUtils.CopyMethodCustomModifiers(overriddenMember, base.TypeArguments, ref destinationReturnType, ref parameters);
			}
			Interlocked.CompareExchange(ref _lazyOverriddenMethods, overriddenMembersResult, null);
			Interlocked.CompareExchange(ref _lazyReturnType, destinationReturnType, null);
			destinationReturnType = _lazyReturnType;
			ImmutableArray<ParameterSymbol>.Enumerator enumerator2 = parameters.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				ParameterSymbol current2 = enumerator2.Current;
				if (current2.Locations.Length > 0)
				{
					ConstraintsHelper.CheckAllConstraints(current2.Type, current2.Locations[0], instance, new CompoundUseSiteInfo<AssemblySymbol>(instance, containingSourceModule.ContainingAssembly));
				}
			}
			if (!Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(errorLocation, SyntaxKind.None))
			{
				ArrayBuilder<TypeParameterDiagnosticInfo> instance3 = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
				ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
				ConstraintsHelper.CheckAllConstraints(destinationReturnType, instance3, ref useSiteDiagnosticsBuilder, new CompoundUseSiteInfo<AssemblySymbol>(instance, containingSourceModule.ContainingAssembly));
				if (useSiteDiagnosticsBuilder != null)
				{
					instance3.AddRange(useSiteDiagnosticsBuilder);
				}
				ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator3 = instance3.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					instance.Add(enumerator3.Current.UseSiteInfo, errorLocation.GetLocation());
				}
				instance3.Free();
			}
			containingSourceModule.AtomicStoreArrayAndDiagnostics(ref _lazyParameters, parameters, instance);
			instance.Free();
		}

		private Binder CreateBinderForMethodDeclaration(SourceModuleSymbol sourceModule)
		{
			Binder containingBinder = BinderBuilder.CreateBinderForMethodDeclaration(sourceModule, base.SyntaxTree, this);
			return new LocationSpecificBinder(BindingLocation.MethodSignature, this, containingBinder);
		}

		protected virtual ImmutableArray<ParameterSymbol> GetParameters(SourceModuleSymbol sourceModule, BindingDiagnosticBag diagBag)
		{
			MethodBaseSyntax declarationSyntax = base.DeclarationSyntax;
			Binder binder = CreateBinderForMethodDeclaration(sourceModule);
			ParameterListSyntax parameterList;
			switch (declarationSyntax.Kind())
			{
			case SyntaxKind.SubNewStatement:
				parameterList = ((SubNewStatementSyntax)declarationSyntax).ParameterList;
				break;
			case SyntaxKind.OperatorStatement:
				parameterList = ((OperatorStatementSyntax)declarationSyntax).ParameterList;
				break;
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
				parameterList = ((MethodStatementSyntax)declarationSyntax).ParameterList;
				break;
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
				parameterList = ((DeclareStatementSyntax)declarationSyntax).ParameterList;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(declarationSyntax.Kind());
			}
			return binder.DecodeParameterList(this, isFromLambda: false, m_flags, parameterList, diagBag);
		}

		private static SyntaxToken GetNameToken(MethodBaseSyntax methodStatement)
		{
			switch (methodStatement.Kind())
			{
			case SyntaxKind.OperatorStatement:
				return ((OperatorStatementSyntax)methodStatement).OperatorToken;
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
				return ((MethodStatementSyntax)methodStatement).Identifier;
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
				return ((DeclareStatementSyntax)methodStatement).Identifier;
			default:
				throw ExceptionUtilities.UnexpectedValue(methodStatement.Kind());
			}
		}

		private TypeSymbol GetReturnType(SourceModuleSymbol sourceModule, ref SyntaxNodeOrToken errorLocation, BindingDiagnosticBag diagBag)
		{
			Binder binder = CreateBinderForMethodDeclaration(sourceModule);
			switch (MethodKind)
			{
			case MethodKind.Constructor:
			case MethodKind.EventRaise:
			case MethodKind.EventRemove:
			case MethodKind.StaticConstructor:
				return binder.GetSpecialType(SpecialType.System_Void, Syntax, diagBag);
			case MethodKind.EventAdd:
				return ((EventSymbol)AssociatedSymbol).IsWindowsRuntimeEvent ? binder.GetWellKnownType(WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken, Syntax, diagBag) : binder.GetSpecialType(SpecialType.System_Void, Syntax, diagBag);
			case MethodKind.PropertyGet:
			case MethodKind.PropertySet:
				throw ExceptionUtilities.Unreachable;
			default:
			{
				MethodBaseSyntax declarationSyntax = base.DeclarationSyntax;
				SyntaxKind syntaxKind = declarationSyntax.Kind();
				TypeSymbol typeSymbol;
				if (syntaxKind == SyntaxKind.SubStatement || syntaxKind == SyntaxKind.DeclareSubStatement)
				{
					Binder.DisallowTypeCharacter(GetNameToken(declarationSyntax), diagBag, ERRID.ERR_TypeCharOnSub);
					typeSymbol = binder.GetSpecialType(SpecialType.System_Void, Syntax, diagBag);
					errorLocation = declarationSyntax.DeclarationKeyword;
				}
				else
				{
					Func<DiagnosticInfo> getRequireTypeDiagnosticInfoFunc = null;
					if (binder.OptionStrict == OptionStrict.On)
					{
						getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitProc;
					}
					else if (binder.OptionStrict == OptionStrict.Custom)
					{
						getRequireTypeDiagnosticInfoFunc = ((MethodKind != MethodKind.UserDefinedOperator) ? ErrorFactory.GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinFunction : ErrorFactory.GetErrorInfo_WRN_ObjectAssumed1_WRN_MissingAsClauseinOperator);
					}
					AsClauseSyntax asClauseInternal = declarationSyntax.AsClauseInternal;
					typeSymbol = binder.DecodeIdentifierType(GetNameToken(declarationSyntax), asClauseInternal, getRequireTypeDiagnosticInfoFunc, diagBag);
					if (asClauseInternal != null)
					{
						errorLocation = SyntaxExtensions.Type(asClauseInternal);
					}
					else
					{
						errorLocation = declarationSyntax.DeclarationKeyword;
					}
				}
				if (!TypeSymbolExtensions.IsErrorType(typeSymbol))
				{
					AccessCheck.VerifyAccessExposureForMemberType(this, errorLocation, typeSymbol, diagBag);
					TypeSymbol restrictedType = null;
					if (TypeSymbolExtensions.IsRestrictedArrayType(typeSymbol, out restrictedType))
					{
						Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_RestrictedType1, restrictedType);
					}
					if (!base.IsAsync || !base.IsIterator)
					{
						if (IsSub)
						{
							if (base.IsIterator)
							{
								Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_BadIteratorReturn);
							}
						}
						else
						{
							if (base.IsAsync)
							{
								VisualBasicCompilation declaringCompilation = DeclaringCompilation;
								if (!typeSymbol.OriginalDefinition.Equals(declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T)) && !typeSymbol.Equals(declaringCompilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task)))
								{
									Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_BadAsyncReturn);
								}
							}
							if (base.IsIterator)
							{
								TypeSymbol originalDefinition = typeSymbol.OriginalDefinition;
								if (originalDefinition.SpecialType != SpecialType.System_Collections_Generic_IEnumerable_T && originalDefinition.SpecialType != SpecialType.System_Collections_Generic_IEnumerator_T && typeSymbol.SpecialType != SpecialType.System_Collections_IEnumerable && typeSymbol.SpecialType != SpecialType.System_Collections_IEnumerator)
								{
									Binder.ReportDiagnostic(diagBag, errorLocation, ERRID.ERR_BadIteratorReturn);
								}
							}
						}
					}
				}
				return typeSymbol;
			}
			}
		}
	}
}
