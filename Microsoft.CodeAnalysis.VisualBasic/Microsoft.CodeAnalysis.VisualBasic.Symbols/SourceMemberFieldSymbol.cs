using System;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceMemberFieldSymbol : SourceFieldSymbol
	{
		private class SourceFieldSymbolWithInitializer : SourceMemberFieldSymbol
		{
			protected readonly SyntaxReference _equalsValueOrAsNewInit;

			internal sealed override VisualBasicSyntaxNode EqualsValueOrAsNewInitOpt => VisualBasicExtensions.GetVisualBasicSyntax(_equalsValueOrAsNewInit);

			public SourceFieldSymbolWithInitializer(SourceMemberContainerTypeSymbol container, SyntaxReference syntaxRef, string name, SourceMemberFlags memberFlags, SyntaxReference equalsValueOrAsNewInit)
				: base(container, syntaxRef, name, memberFlags)
			{
				_equalsValueOrAsNewInit = equalsValueOrAsNewInit;
			}
		}

		private sealed class SourceConstFieldSymbolWithInitializer : SourceFieldSymbolWithInitializer
		{
			private EvaluatedConstant _constantTuple;

			public SourceConstFieldSymbolWithInitializer(SourceMemberContainerTypeSymbol container, SyntaxReference syntaxRef, string name, SourceMemberFlags memberFlags, SyntaxReference equalsValueOrAsNewInit)
				: base(container, syntaxRef, name, memberFlags, equalsValueOrAsNewInit)
			{
			}

			protected override EvaluatedConstant GetLazyConstantTuple()
			{
				return _constantTuple;
			}

			internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
			{
				return GetConstantValueImpl(inProgress);
			}

			protected override EvaluatedConstant MakeConstantTuple(ConstantFieldsInProgress.Dependencies dependencies, BindingDiagnosticBag diagnostics)
			{
				return ConstantValueUtils.EvaluateFieldConstant(this, _equalsValueOrAsNewInit, dependencies, diagnostics);
			}

			protected override void SetLazyConstantTuple(EvaluatedConstant constantTuple, BindingDiagnosticBag diagnostics)
			{
				((SourceModuleSymbol)ContainingModule).AtomicStoreReferenceAndDiagnostics(ref _constantTuple, constantTuple, diagnostics);
			}

			protected override TypeSymbol GetInferredConstantType(ConstantFieldsInProgress inProgress)
			{
				GetConstantValueImpl(inProgress);
				EvaluatedConstant lazyConstantTuple = GetLazyConstantTuple();
				if (lazyConstantTuple != null)
				{
					return lazyConstantTuple.Type;
				}
				return new ErrorTypeSymbol();
			}
		}

		private sealed class SourceFieldSymbolSiblingInitializer : SourceMemberFieldSymbol
		{
			private readonly SourceMemberFieldSymbol _sibling;

			internal override VisualBasicSyntaxNode EqualsValueOrAsNewInitOpt => _sibling.EqualsValueOrAsNewInitOpt;

			public SourceFieldSymbolSiblingInitializer(SourceMemberContainerTypeSymbol container, SyntaxReference syntaxRef, string name, SourceMemberFlags memberFlags, SourceMemberFieldSymbol sibling)
				: base(container, syntaxRef, name, memberFlags)
			{
				_sibling = sibling;
			}

			internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
			{
				return _sibling.GetConstantValue(inProgress);
			}

			protected override TypeSymbol GetInferredConstantType(ConstantFieldsInProgress inProgress)
			{
				return _sibling.GetInferredConstantType(inProgress);
			}
		}

		private TypeSymbol _lazyType;

		private ParameterSymbol _lazyMeParameter;

		internal sealed override VisualBasicSyntaxNode DeclarationSyntax => base.Syntax.Parent.Parent;

		internal sealed override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations => OneOrMany.Create(((FieldDeclarationSyntax)base.Syntax.Parent.Parent).AttributeLists);

		internal override ParameterSymbol MeParameter
		{
			get
			{
				if (IsShared)
				{
					return null;
				}
				if ((object)_lazyMeParameter == null)
				{
					Interlocked.CompareExchange(ref _lazyMeParameter, new MeParameterSymbol(this), null);
				}
				return _lazyMeParameter;
			}
		}

		public override TypeSymbol Type
		{
			get
			{
				if ((object)_lazyType == null)
				{
					SourceModuleSymbol obj = (SourceModuleSymbol)ContainingModule;
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					obj.AtomicStoreReferenceAndDiagnostics(value: ComputeType(instance), variable: ref _lazyType, diagBag: instance);
					instance.Free();
				}
				return _lazyType;
			}
		}

		protected SourceMemberFieldSymbol(SourceMemberContainerTypeSymbol container, SyntaxReference syntaxRef, string name, SourceMemberFlags memberFlags)
			: base(container, syntaxRef, name, memberFlags)
		{
		}

		internal sealed override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Symbol.IsDefinedInSourceTree(DeclarationSyntax, tree, definedWithinSpan, cancellationToken);
		}

		private TypeSymbol ComputeType(BindingDiagnosticBag diagBag)
		{
			TypeSymbol declaredType = GetDeclaredType(diagBag);
			if (!HasDeclaredType)
			{
				return GetInferredType(ConstantFieldsInProgress.Empty);
			}
			return declaredType;
		}

		private TypeSymbol GetDeclaredType(BindingDiagnosticBag diagBag)
		{
			ModifiedIdentifierSyntax modifiedIdentifierSyntax = (ModifiedIdentifierSyntax)base.Syntax;
			VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)modifiedIdentifierSyntax.Parent;
			Binder containingBinder = BinderBuilder.CreateBinderForType((SourceModuleSymbol)ContainingModule, base.SyntaxTree, base.ContainingType);
			containingBinder = new LocationSpecificBinder(BindingLocation.FieldType, this, containingBinder);
			TypeSymbol typeSymbol = ComputeFieldType(modifiedIdentifierSyntax, containingBinder, diagBag, IsConst, isWithEvents: false, (m_memberFlags & SourceMemberFlags.FirstFieldDeclarationOfType) == 0);
			if (!TypeSymbolExtensions.IsErrorType(typeSymbol))
			{
				if (IsConst)
				{
					if (!TypeSymbolExtensions.IsValidTypeForConstField(typeSymbol))
					{
						if (TypeSymbolExtensions.IsArrayType(typeSymbol))
						{
							Binder.ReportDiagnostic(diagBag, modifiedIdentifierSyntax.Identifier, ERRID.ERR_ConstAsNonConstant);
						}
						else
						{
							Binder.ReportDiagnostic(diagBag, SyntaxExtensions.Type(variableDeclaratorSyntax.AsClause), ERRID.ERR_ConstAsNonConstant);
						}
					}
					else if (variableDeclaratorSyntax.Initializer == null)
					{
						Binder.ReportDiagnostic(diagBag, modifiedIdentifierSyntax, ERRID.ERR_ConstantWithNoValue);
					}
				}
				else
				{
					TypeSymbol restrictedType = null;
					if (TypeSymbolExtensions.IsRestrictedTypeOrArrayType(typeSymbol, out restrictedType))
					{
						Binder.ReportDiagnostic(diagBag, SyntaxExtensions.Type(variableDeclaratorSyntax.AsClause), ERRID.ERR_RestrictedType1, restrictedType);
					}
				}
				if (HasDeclaredType)
				{
					SyntaxNodeOrToken asClauseLocation = SourceSymbolHelpers.GetAsClauseLocation(modifiedIdentifierSyntax.Identifier, variableDeclaratorSyntax.AsClause);
					AccessCheck.VerifyAccessExposureForMemberType(this, asClauseLocation, typeSymbol, diagBag);
				}
			}
			return typeSymbol;
		}

		private static TypeSymbol ComputeFieldType(ModifiedIdentifierSyntax modifiedIdentifierSyntax, Binder binder, BindingDiagnosticBag diagnostics, bool isConst, bool isWithEvents, bool ignoreTypeSyntaxDiagnostics)
		{
			VariableDeclaratorSyntax obj = (VariableDeclaratorSyntax)modifiedIdentifierSyntax.Parent;
			AsClauseSyntax asClause = obj.AsClause;
			TypeSymbol asClauseOrValueType = null;
			VisualBasicSyntaxNode initializerSyntaxOpt = obj.Initializer;
			if (asClause != null)
			{
				if (asClause.Kind() != SyntaxKind.AsNewClause || ((AsNewClauseSyntax)asClause).NewExpression.Kind() != SyntaxKind.AnonymousObjectCreationExpression)
				{
					asClauseOrValueType = binder.BindTypeSyntax(SyntaxExtensions.Type(asClause), ignoreTypeSyntaxDiagnostics ? BindingDiagnosticBag.Discarded : diagnostics);
				}
				if (asClause.Kind() == SyntaxKind.AsNewClause)
				{
					initializerSyntaxOpt = asClause;
				}
			}
			bool flag = string.IsNullOrEmpty(modifiedIdentifierSyntax.Identifier.ValueText);
			if (asClause != null && asClause.Kind() == SyntaxKind.AsNewClause && ((AsNewClauseSyntax)asClause).NewExpression.Kind() == SyntaxKind.AnonymousObjectCreationExpression)
			{
				return ErrorTypeSymbol.UnknownResultType;
			}
			Func<DiagnosticInfo> getRequireTypeDiagnosticInfoFunc = null;
			if (!flag && (!isConst || !binder.OptionInfer))
			{
				if (isWithEvents)
				{
					getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_ERR_WithEventsRequiresClass;
				}
				else if (binder.OptionStrict == OptionStrict.On)
				{
					getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_ERR_StrictDisallowImplicitObject;
				}
				else if (binder.OptionStrict == OptionStrict.Custom)
				{
					getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_WRN_ObjectAssumedVar1_WRN_MissingAsClauseinVarDecl;
				}
			}
			return binder.DecodeModifiedIdentifierType(modifiedIdentifierSyntax, asClauseOrValueType, asClause, initializerSyntaxOpt, getRequireTypeDiagnosticInfoFunc, diagnostics, Binder.ModifiedIdentifierTypeDecoderContext.FieldType);
		}

		internal static TypeSymbol ComputeWithEventsFieldType(PropertySymbol propertySymbol, ModifiedIdentifierSyntax modifiedIdentifier, Binder binder, bool ignoreTypeSyntaxDiagnostics, BindingDiagnosticBag diagnostics)
		{
			TypeSymbol typeSymbol = ComputeFieldType(modifiedIdentifier, binder, diagnostics, isConst: false, isWithEvents: true, ignoreTypeSyntaxDiagnostics);
			if (!TypeSymbolExtensions.IsErrorType(typeSymbol))
			{
				VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)modifiedIdentifier.Parent;
				SyntaxToken identifier = modifiedIdentifier.Identifier;
				if (TypeSymbolExtensions.IsArrayType(typeSymbol))
				{
					Binder.ReportDiagnostic(diagnostics, modifiedIdentifier, ERRID.ERR_EventSourceIsArray);
				}
				else if (!TypeSymbolExtensions.IsClassOrInterfaceType(typeSymbol) && (typeSymbol.Kind != SymbolKind.TypeParameter || !typeSymbol.IsReferenceType) && variableDeclaratorSyntax.AsClause != null && SyntaxExtensions.Type(variableDeclaratorSyntax.AsClause) != null)
				{
					Binder.ReportDiagnostic(diagnostics, identifier, ERRID.ERR_WithEventsAsStruct);
				}
				if (variableDeclaratorSyntax.AsClause != null)
				{
					SyntaxNodeOrToken asClauseLocation = SourceSymbolHelpers.GetAsClauseLocation(identifier, variableDeclaratorSyntax.AsClause);
					AccessCheck.VerifyAccessExposureForMemberType(propertySymbol, asClauseLocation, typeSymbol, diagnostics);
				}
			}
			return typeSymbol;
		}

		internal override TypeSymbol GetInferredType(ConstantFieldsInProgress inProgress)
		{
			if (HasDeclaredType)
			{
				return Type;
			}
			GetConstantValue(inProgress);
			TypeSymbol inferredConstantType = GetInferredConstantType(inProgress);
			return ((object)inferredConstantType == null) ? ContainingAssembly.GetSpecialType(SpecialType.System_Object) : (TypeSymbolExtensions.IsValidTypeForConstField(inferredConstantType) ? TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(inferredConstantType) : ContainingAssembly.GetSpecialType(SpecialType.System_Object));
		}

		protected virtual TypeSymbol GetInferredConstantType(ConstantFieldsInProgress inProgress)
		{
			return null;
		}

		internal static void Create(SourceMemberContainerTypeSymbol container, FieldDeclarationSyntax syntax, Binder binder, SourceMemberContainerTypeSymbol.MembersAndInitializersBuilder members, ref ArrayBuilder<FieldOrPropertyInitializer> staticInitializers, ref ArrayBuilder<FieldOrPropertyInitializer> instanceInitializers, BindingDiagnosticBag diagBag)
		{
			SourceMemberFlags sourceMemberFlags = SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.Shared | SourceMemberFlags.Shadows | SourceMemberFlags.Dim | SourceMemberFlags.Const;
			ERRID eRRID = ERRID.ERR_BadDimFlags1;
			if (Microsoft.CodeAnalysis.VisualBasicExtensions.Any(syntax.Modifiers, SyntaxKind.WithEventsKeyword))
			{
				sourceMemberFlags |= SourceMemberFlags.WithEvents;
				eRRID = ERRID.ERR_BadWithEventsFlags1;
			}
			else
			{
				sourceMemberFlags |= SourceMemberFlags.ReadOnly;
				eRRID = ERRID.ERR_BadDimFlags1;
			}
			MemberModifiers modifiers = binder.DecodeModifiers(syntax.Modifiers, sourceMemberFlags, eRRID, (!container.IsValueType) ? Accessibility.Private : Accessibility.Public, diagBag.DiagnosticBag);
			if ((object)container != null)
			{
				switch (container.DeclarationKind)
				{
				case DeclarationKind.Structure:
					if ((modifiers.FoundFlags & SourceMemberFlags.Protected) != 0)
					{
						binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_StructCantUseVarSpecifier1, diagBag.DiagnosticBag, SyntaxKind.ProtectedKeyword);
						modifiers = new MemberModifiers(modifiers.FoundFlags & ~SourceMemberFlags.Protected, (modifiers.ComputedFlags & ~SourceMemberFlags.AccessibilityMask) | SourceMemberFlags.AccessibilityPrivate);
					}
					if ((modifiers.FoundFlags & SourceMemberFlags.WithEvents) != 0)
					{
						binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_StructCantUseVarSpecifier1, diagBag.DiagnosticBag, SyntaxKind.WithEventsKeyword);
						modifiers = new MemberModifiers(modifiers.FoundFlags & ~SourceMemberFlags.WithEvents, modifiers.ComputedFlags);
					}
					break;
				case DeclarationKind.Module:
					if ((modifiers.FoundFlags & SourceMemberFlags.InvalidInModule) != 0)
					{
						binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_ModuleCantUseVariableSpecifier1, diagBag.DiagnosticBag, SyntaxKind.SharedKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.DefaultKeyword, SyntaxKind.MustOverrideKeyword, SyntaxKind.OverridableKeyword, SyntaxKind.ShadowsKeyword, SyntaxKind.OverridesKeyword, SyntaxKind.NotOverridableKeyword);
					}
					modifiers = new MemberModifiers(modifiers.FoundFlags & ~SourceMemberFlags.InvalidInModule, modifiers.ComputedFlags | SourceMemberFlags.Shared);
					break;
				}
			}
			if ((modifiers.FoundFlags & SourceMemberFlags.Const) != 0 && (modifiers.FoundFlags & (SourceMemberFlags.Shared | SourceMemberFlags.ReadOnly | SourceMemberFlags.WithEvents | SourceMemberFlags.Dim)) != 0)
			{
				if ((modifiers.FoundFlags & SourceMemberFlags.Shared) != 0)
				{
					binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_BadConstFlags1, diagBag.DiagnosticBag, SyntaxKind.SharedKeyword);
				}
				if ((modifiers.FoundFlags & SourceMemberFlags.ReadOnly) != 0)
				{
					binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_BadConstFlags1, diagBag.DiagnosticBag, SyntaxKind.ReadOnlyKeyword);
				}
				if ((modifiers.FoundFlags & SourceMemberFlags.WithEvents) != 0)
				{
					binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_BadConstFlags1, diagBag.DiagnosticBag, SyntaxKind.WithEventsKeyword);
				}
				if ((modifiers.FoundFlags & SourceMemberFlags.Dim) != 0)
				{
					binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_BadConstFlags1, diagBag.DiagnosticBag, SyntaxKind.DimKeyword);
				}
				modifiers = new MemberModifiers(modifiers.FoundFlags & ~(SourceMemberFlags.Shared | SourceMemberFlags.ReadOnly | SourceMemberFlags.WithEvents | SourceMemberFlags.Dim), modifiers.ComputedFlags);
			}
			if ((modifiers.FoundFlags & SourceMemberFlags.Const) != 0 && (modifiers.FoundFlags & SourceMemberFlags.Shared) == 0)
			{
				modifiers = new MemberModifiers(modifiers.FoundFlags, modifiers.ComputedFlags | SourceMemberFlags.Shared);
			}
			SourceMemberFlags allFlags = modifiers.AllFlags;
			SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = syntax.Declarators.GetEnumerator();
			_Closure_0024__21_002D2 closure_0024__21_002D = default(_Closure_0024__21_002D2);
			_Closure_0024__21_002D1 closure_0024__21_002D2 = default(_Closure_0024__21_002D1);
			_Closure_0024__21_002D0 closure_0024__21_002D3 = default(_Closure_0024__21_002D0);
			while (enumerator.MoveNext())
			{
				VariableDeclaratorSyntax current = enumerator.Current;
				closure_0024__21_002D = new _Closure_0024__21_002D2(closure_0024__21_002D);
				if (current.Names.Count > 1 && current.Initializer != null)
				{
					Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_InitWithMultipleDeclarators);
				}
				AsClauseSyntax asClause = current.AsClause;
				EqualsValueSyntax initializer = current.Initializer;
				VisualBasicSyntaxNode visualBasicSyntaxNode = null;
				visualBasicSyntaxNode = ((asClause == null || asClause.Kind() != SyntaxKind.AsNewClause) ? ((VisualBasicSyntaxNode)initializer) : ((VisualBasicSyntaxNode)asClause));
				closure_0024__21_002D._0024VB_0024Local_initializerOptRef = ((visualBasicSyntaxNode == null) ? null : binder.GetSyntaxReference(visualBasicSyntaxNode));
				if (container.TypeKind == TypeKind.Struct && (allFlags & SourceMemberFlags.Shared) == 0)
				{
					if (initializer != null)
					{
						Binder.ReportDiagnostic(diagBag, (current.Names.Count > 0) ? ((VisualBasicSyntaxNode)current.Names.Last()) : ((VisualBasicSyntaxNode)current), ERRID.ERR_InitializerInStruct);
					}
					else if (asClause != null && asClause.Kind() == SyntaxKind.AsNewClause)
					{
						Binder.ReportDiagnostic(diagBag, ((AsNewClauseSyntax)asClause).NewExpression.NewKeyword, ERRID.ERR_SharedStructMemberCannotSpecifyNew);
					}
				}
				int count = current.Names.Count;
				closure_0024__21_002D._0024VB_0024Local_fieldOrWithEventSymbols = new Symbol[count - 1 + 1];
				int num = count - 1;
				for (int i = 0; i <= num; i++)
				{
					closure_0024__21_002D2 = new _Closure_0024__21_002D1(closure_0024__21_002D2);
					SourceMemberFlags sourceMemberFlags2 = allFlags;
					if (i == 0)
					{
						sourceMemberFlags2 |= SourceMemberFlags.FirstFieldDeclarationOfType;
					}
					ModifiedIdentifierSyntax modifiedIdentifierSyntax = current.Names[i];
					SyntaxToken identifier = modifiedIdentifierSyntax.Identifier;
					bool omitDiagnostics = string.IsNullOrEmpty(modifiedIdentifierSyntax.Identifier.ValueText);
					if ((modifiers.FoundFlags & SourceMemberFlags.Const) != 0 && VisualBasicExtensions.GetTypeCharacter(identifier) == TypeCharacter.None && modifiedIdentifierSyntax.Nullable.Node == null && modifiedIdentifierSyntax.ArrayBounds == null && modifiedIdentifierSyntax.ArrayRankSpecifiers.IsEmpty() && (!(asClause is SimpleAsClauseSyntax simpleAsClauseSyntax) || (simpleAsClauseSyntax.Type.Kind() == SyntaxKind.PredefinedType && VisualBasicExtensions.Kind(((PredefinedTypeSyntax)simpleAsClauseSyntax.Type).Keyword) == SyntaxKind.ObjectKeyword)))
					{
						sourceMemberFlags2 |= SourceMemberFlags.InferredFieldType;
					}
					closure_0024__21_002D2._0024VB_0024Local_modifiedIdentifierRef = binder.GetSyntaxReference(modifiedIdentifierSyntax);
					if ((modifiers.FoundFlags & SourceMemberFlags.WithEvents) == 0)
					{
						closure_0024__21_002D3 = new _Closure_0024__21_002D0(closure_0024__21_002D3);
						closure_0024__21_002D3._0024VB_0024NonLocal__0024VB_0024Closure_2 = closure_0024__21_002D2;
						if (closure_0024__21_002D._0024VB_0024Local_initializerOptRef == null)
						{
							closure_0024__21_002D3._0024VB_0024Local_fieldSymbol = new SourceMemberFieldSymbol(container, closure_0024__21_002D3._0024VB_0024NonLocal__0024VB_0024Closure_2._0024VB_0024Local_modifiedIdentifierRef, identifier.ValueText, sourceMemberFlags2);
						}
						else if (i == 0)
						{
							if ((sourceMemberFlags2 & SourceMemberFlags.Const) != 0)
							{
								closure_0024__21_002D3._0024VB_0024Local_fieldSymbol = new SourceConstFieldSymbolWithInitializer(container, closure_0024__21_002D3._0024VB_0024NonLocal__0024VB_0024Closure_2._0024VB_0024Local_modifiedIdentifierRef, identifier.ValueText, sourceMemberFlags2, closure_0024__21_002D._0024VB_0024Local_initializerOptRef);
							}
							else
							{
								closure_0024__21_002D3._0024VB_0024Local_fieldSymbol = new SourceFieldSymbolWithInitializer(container, closure_0024__21_002D3._0024VB_0024NonLocal__0024VB_0024Closure_2._0024VB_0024Local_modifiedIdentifierRef, identifier.ValueText, sourceMemberFlags2, closure_0024__21_002D._0024VB_0024Local_initializerOptRef);
							}
						}
						else
						{
							closure_0024__21_002D3._0024VB_0024Local_fieldSymbol = new SourceFieldSymbolSiblingInitializer(container, closure_0024__21_002D3._0024VB_0024NonLocal__0024VB_0024Closure_2._0024VB_0024Local_modifiedIdentifierRef, identifier.ValueText, sourceMemberFlags2, (SourceMemberFieldSymbol)closure_0024__21_002D._0024VB_0024Local_fieldOrWithEventSymbols[0]);
						}
						closure_0024__21_002D._0024VB_0024Local_fieldOrWithEventSymbols[i] = closure_0024__21_002D3._0024VB_0024Local_fieldSymbol;
						if (syntax.AttributeLists.Count == 0)
						{
							closure_0024__21_002D3._0024VB_0024Local_fieldSymbol.SetCustomAttributeData(CustomAttributesBag<VisualBasicAttributeData>.Empty);
						}
						if (modifiedIdentifierSyntax.ArrayBounds != null && modifiedIdentifierSyntax.ArrayBounds.Arguments.Count > 0)
						{
							if (TypeSymbolExtensions.IsStructureType(container) && !closure_0024__21_002D3._0024VB_0024Local_fieldSymbol.IsShared)
							{
								Binder.ReportDiagnostic(diagBag, modifiedIdentifierSyntax, ERRID.ERR_ArrayInitInStruct);
							}
							else if (initializer == null)
							{
								VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_ = closure_0024__21_002D3._Lambda_0024__0;
								if (closure_0024__21_002D3._0024VB_0024Local_fieldSymbol.IsShared)
								{
									VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_2 = vB_0024AnonymousDelegate_;
									SourceMemberContainerTypeSymbol.AddInitializer(ref staticInitializers, (vB_0024AnonymousDelegate_2 == null) ? null : new Func<int, FieldOrPropertyInitializer>(vB_0024AnonymousDelegate_2.Invoke), ref members.StaticSyntaxLength);
								}
								else
								{
									VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_2 = vB_0024AnonymousDelegate_;
									SourceMemberContainerTypeSymbol.AddInitializer(ref instanceInitializers, (vB_0024AnonymousDelegate_2 == null) ? null : new Func<int, FieldOrPropertyInitializer>(vB_0024AnonymousDelegate_2.Invoke), ref members.InstanceSyntaxLength);
								}
							}
							else
							{
								Binder.ReportDiagnostic(diagBag, modifiedIdentifierSyntax, ERRID.ERR_InitWithExplicitArraySizes);
							}
						}
						container.AddMember(closure_0024__21_002D3._0024VB_0024Local_fieldSymbol, binder, members, omitDiagnostics);
					}
					else
					{
						SourcePropertySymbol sourcePropertySymbol = SourcePropertySymbol.CreateWithEvents(container, binder, identifier, closure_0024__21_002D2._0024VB_0024Local_modifiedIdentifierRef, modifiers, i == 0, diagBag);
						closure_0024__21_002D._0024VB_0024Local_fieldOrWithEventSymbols[i] = sourcePropertySymbol;
						container.AddMember(sourcePropertySymbol, binder, members, omitDiagnostics);
						container.AddMember(sourcePropertySymbol.GetMethod, binder, members, omitDiagnostics: false);
						container.AddMember(sourcePropertySymbol.SetMethod, binder, members, omitDiagnostics: false);
						container.AddMember(sourcePropertySymbol.AssociatedField, binder, members, omitDiagnostics: false);
					}
				}
				if (closure_0024__21_002D._0024VB_0024Local_initializerOptRef != null)
				{
					VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_3 = closure_0024__21_002D._Lambda_0024__1;
					if (count > 0 && closure_0024__21_002D._0024VB_0024Local_fieldOrWithEventSymbols[0].IsShared)
					{
						VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_2 = vB_0024AnonymousDelegate_3;
						SourceMemberContainerTypeSymbol.AddInitializer(ref staticInitializers, (vB_0024AnonymousDelegate_2 == null) ? null : new Func<int, FieldOrPropertyInitializer>(vB_0024AnonymousDelegate_2.Invoke), ref members.StaticSyntaxLength);
					}
					else
					{
						VB_0024AnonymousDelegate_0<int, FieldOrPropertyInitializer> vB_0024AnonymousDelegate_2 = vB_0024AnonymousDelegate_3;
						SourceMemberContainerTypeSymbol.AddInitializer(ref instanceInitializers, (vB_0024AnonymousDelegate_2 == null) ? null : new Func<int, FieldOrPropertyInitializer>(vB_0024AnonymousDelegate_2.Invoke), ref members.InstanceSyntaxLength);
					}
				}
			}
		}
	}
}
