using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceEventSymbol : EventSymbol, IAttributeTargetSymbol
	{
		[Flags]
		private enum StateFlags
		{
			IsTypeInferred = 1,
			IsDelegateFromImplements = 2,
			ReportedExplicitImplementationDiagnostics = 4,
			SymbolDeclaredEvent = 8
		}

		private readonly SourceMemberContainerTypeSymbol _containingType;

		private readonly string _name;

		private readonly SyntaxReference _syntaxRef;

		private readonly Location _location;

		private readonly SourceMemberFlags _memberFlags;

		private readonly MethodSymbol _addMethod;

		private readonly MethodSymbol _removeMethod;

		private readonly MethodSymbol _raiseMethod;

		private readonly FieldSymbol _backingField;

		private int _lazyState;

		private TypeSymbol _lazyType;

		private ImmutableArray<EventSymbol> _lazyImplementedEvents;

		private ImmutableArray<ParameterSymbol> _lazyDelegateParameters;

		private string _lazyDocComment;

		private string _lazyExpandedDocComment;

		private CustomAttributesBag<VisualBasicAttributeData> _lazyCustomAttributesBag;

		internal bool IsTypeInferred
		{
			get
			{
				_ = Type;
				return (_lazyState & 1) != 0;
			}
		}

		internal override ImmutableArray<ParameterSymbol> DelegateParameters
		{
			get
			{
				if (_lazyDelegateParameters.IsDefault)
				{
					EventStatementSyntax eventStatementSyntax = (EventStatementSyntax)_syntaxRef.GetSyntax();
					if (eventStatementSyntax.AsClause != null)
					{
						_lazyDelegateParameters = base.DelegateParameters;
					}
					else
					{
						Binder binder = CreateBinderForTypeDeclaration();
						BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
						ContainingSourceModule.AtomicStoreArrayAndDiagnostics(ref _lazyDelegateParameters, binder.DecodeParameterListOfDelegateDeclaration(this, eventStatementSyntax.ParameterList, instance), instance);
						instance.Free();
					}
				}
				return _lazyDelegateParameters;
			}
		}

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public SourceModuleSymbol ContainingSourceModule => _containingType.ContainingSourceModule;

		public override ImmutableArray<Location> Locations => ImmutableArray.Create(_location);

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper(_syntaxRef);

		public override TypeSymbol Type
		{
			get
			{
				if ((object)_lazyType == null)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					bool isTypeInferred = false;
					bool isDelegateFromImplements = false;
					TypeSymbol value = ComputeType(instance, out isTypeInferred, out isDelegateFromImplements);
					int toSet = (isTypeInferred ? 1 : 0) | (isDelegateFromImplements ? 2 : 0);
					ThreadSafeFlagOperations.Set(ref _lazyState, toSet);
					ContainingSourceModule.AtomicStoreReferenceAndDiagnostics(ref _lazyType, value, instance);
					instance.Free();
				}
				return _lazyType;
			}
		}

		public override string Name => _name;

		public override MethodSymbol AddMethod => _addMethod;

		public override MethodSymbol RemoveMethod => _removeMethod;

		public override MethodSymbol RaiseMethod => _raiseMethod;

		internal override FieldSymbol AssociatedField => _backingField;

		public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyImplementedEvents.IsDefault)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					ContainingSourceModule.AtomicStoreArrayAndDiagnostics(ref _lazyImplementedEvents, ComputeImplementedEvents(instance), instance);
					instance.Free();
				}
				return _lazyImplementedEvents;
			}
		}

		internal SyntaxReference SyntaxReference => _syntaxRef;

		public override bool IsShared => (_memberFlags & SourceMemberFlags.Shared) != 0;

		public override bool IsMustOverride => (_memberFlags & SourceMemberFlags.MustOverride) != 0;

		public override bool IsOverridable => false;

		public override bool IsOverrides => false;

		public override bool IsNotOverridable => false;

		public override Accessibility DeclaredAccessibility => (Accessibility)(_memberFlags & SourceMemberFlags.AccessibilityMask);

		internal override bool ShadowsExplicitly => (_memberFlags & SourceMemberFlags.Shadows) != 0;

		internal SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList => ((EventStatementSyntax)_syntaxRef.GetSyntax()).AttributeLists;

		public AttributeLocation DefaultAttributeLocation => AttributeLocation.Event;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				if (!_containingType.AnyMemberHasAttributes)
				{
					return null;
				}
				CustomAttributesBag<VisualBasicAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
				if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
				{
					return ((CommonEventEarlyWellKnownAttributeData)_lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
				}
				return ObsoleteAttributeData.Uninitialized;
			}
		}

		internal sealed override bool IsDirectlyExcludedFromCodeCoverage => GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute ?? false;

		internal override bool HasSpecialName => GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;

		public override bool IsWindowsRuntimeEvent
		{
			get
			{
				ImmutableArray<EventSymbol> explicitInterfaceImplementations = ExplicitInterfaceImplementations;
				if (!explicitInterfaceImplementations.Any())
				{
					return SymbolExtensions.IsCompilationOutputWinMdObj(this);
				}
				return explicitInterfaceImplementations[0].IsWindowsRuntimeEvent;
			}
		}

		internal SourceEventSymbol(SourceMemberContainerTypeSymbol containingType, Binder binder, EventStatementSyntax syntax, EventBlockSyntax blockSyntaxOpt, DiagnosticBag diagnostics)
		{
			_containingType = containingType;
			_memberFlags = DecodeModifiers(syntax.Modifiers, containingType, binder, diagnostics).AllFlags;
			SyntaxToken identifier = syntax.Identifier;
			_name = identifier.ValueText;
			if (VisualBasicExtensions.GetTypeCharacter(identifier) != 0)
			{
				Binder.ReportDiagnostic(diagnostics, identifier, ERRID.ERR_TypecharNotallowed);
			}
			Location location = (_location = identifier.GetLocation());
			_syntaxRef = binder.GetSyntaxReference(syntax);
			binder = new LocationSpecificBinder(BindingLocation.EventSignature, this, binder);
			if (blockSyntaxOpt != null)
			{
				SyntaxList<AccessorBlockSyntax>.Enumerator enumerator = blockSyntaxOpt.Accessors.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AccessorBlockSyntax current = enumerator.Current;
					CustomEventAccessorSymbol customEventAccessorSymbol = BindEventAccessor(current, binder);
					switch (customEventAccessorSymbol.MethodKind)
					{
					case MethodKind.EventAdd:
						if ((object)_addMethod == null)
						{
							_addMethod = customEventAccessorSymbol;
						}
						else
						{
							DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_DuplicateAddHandlerDef, customEventAccessorSymbol.Locations[0]);
						}
						break;
					case MethodKind.EventRemove:
						if ((object)_removeMethod == null)
						{
							_removeMethod = customEventAccessorSymbol;
						}
						else
						{
							DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_DuplicateRemoveHandlerDef, customEventAccessorSymbol.Locations[0]);
						}
						break;
					case MethodKind.EventRaise:
						if ((object)_raiseMethod == null)
						{
							_raiseMethod = customEventAccessorSymbol;
						}
						else
						{
							DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_DuplicateRaiseEventDef, customEventAccessorSymbol.Locations[0]);
						}
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(customEventAccessorSymbol.MethodKind);
					}
				}
				if ((object)_addMethod == null)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_MissingAddHandlerDef1, location, this);
				}
				if ((object)_removeMethod == null)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_MissingRemoveHandlerDef1, location, this);
				}
				if ((object)_raiseMethod == null)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_MissingRaiseEventDef1, location, this);
				}
			}
			else
			{
				_addMethod = new SynthesizedAddAccessorSymbol(containingType, this);
				_removeMethod = new SynthesizedRemoveAccessorSymbol(containingType, this);
				if (!TypeSymbolExtensions.IsInterfaceType(containingType))
				{
					_backingField = new SynthesizedEventBackingFieldSymbol(this, Name + "Event", IsShared);
				}
			}
		}

		private TypeSymbol ComputeType(BindingDiagnosticBag diagnostics, out bool isTypeInferred, out bool isDelegateFromImplements)
		{
			Binder binder = CreateBinderForTypeDeclaration();
			EventStatementSyntax eventStatementSyntax = (EventStatementSyntax)_syntaxRef.GetSyntax();
			isTypeInferred = false;
			isDelegateFromImplements = false;
			bool flag = eventStatementSyntax.ImplementsClause == null && IsWindowsRuntimeEvent;
			TypeSymbol typeSymbol;
			if (eventStatementSyntax.AsClause != null)
			{
				typeSymbol = binder.DecodeIdentifierType(eventStatementSyntax.Identifier, eventStatementSyntax.AsClause, null, diagnostics);
				if (!eventStatementSyntax.AsClause.AsKeyword.IsMissing)
				{
					if (!TypeSymbolExtensions.IsDelegateType(typeSymbol))
					{
						Binder.ReportDiagnostic(diagnostics, eventStatementSyntax.AsClause.Type, ERRID.ERR_EventTypeNotDelegate);
					}
					else
					{
						MethodSymbol delegateInvokeMethod = ((NamedTypeSymbol)typeSymbol).DelegateInvokeMethod;
						if ((object)delegateInvokeMethod == null)
						{
							Binder.ReportDiagnostic(diagnostics, eventStatementSyntax.AsClause.Type, ERRID.ERR_UnsupportedType1, typeSymbol.Name);
						}
						else if (!delegateInvokeMethod.IsSub)
						{
							Binder.ReportDiagnostic(diagnostics, eventStatementSyntax.AsClause.Type, ERRID.ERR_EventDelegatesCantBeFunctions);
						}
					}
				}
				else if (flag)
				{
					Binder.ReportDiagnostic(diagnostics, eventStatementSyntax.Identifier, ERRID.ERR_WinRTEventWithoutDelegate);
				}
			}
			else
			{
				if (flag)
				{
					Binder.ReportDiagnostic(diagnostics, eventStatementSyntax.Identifier, ERRID.ERR_WinRTEventWithoutDelegate);
				}
				ImmutableArray<EventSymbol> explicitInterfaceImplementations = ExplicitInterfaceImplementations;
				if (!explicitInterfaceImplementations.IsEmpty)
				{
					TypeSymbol type = explicitInterfaceImplementations[0].Type;
					int num = explicitInterfaceImplementations.Length - 1;
					for (int i = 1; i <= num; i++)
					{
						EventSymbol eventSymbol = explicitInterfaceImplementations[i];
						if (!TypeSymbolExtensions.IsSameType(eventSymbol.Type, type, TypeCompareKind.IgnoreTupleNames))
						{
							Location implementingLocation = GetImplementingLocation(eventSymbol);
							Binder.ReportDiagnostic(diagnostics, implementingLocation, ERRID.ERR_MultipleEventImplMismatch3, this, eventSymbol, eventSymbol.ContainingType);
						}
					}
					typeSymbol = type;
					isDelegateFromImplements = true;
				}
				else
				{
					ImmutableArray<NamedTypeSymbol> typeMembers = _containingType.GetTypeMembers(Name + "EventHandler");
					typeSymbol = null;
					ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = typeMembers.GetEnumerator();
					while (enumerator.MoveNext())
					{
						NamedTypeSymbol current = enumerator.Current;
						if ((object)current.AssociatedSymbol == this)
						{
							typeSymbol = current;
							break;
						}
					}
					if ((object)typeSymbol == null)
					{
						typeSymbol = new SynthesizedEventDelegateSymbol(_syntaxRef, _containingType);
					}
					isTypeInferred = true;
				}
			}
			if (!TypeSymbolExtensions.IsErrorType(typeSymbol))
			{
				AccessCheck.VerifyAccessExposureForMemberType(this, eventStatementSyntax.Identifier, typeSymbol, diagnostics, isDelegateFromImplements);
			}
			return typeSymbol;
		}

		private ImmutableArray<EventSymbol> ComputeImplementedEvents(BindingDiagnosticBag diagnostics)
		{
			EventStatementSyntax eventStatementSyntax = (EventStatementSyntax)_syntaxRef.GetSyntax();
			ImplementsClauseSyntax implementsClause = eventStatementSyntax.ImplementsClause;
			if (implementsClause != null)
			{
				Binder binder = CreateBinderForTypeDeclaration();
				if (TypeSymbolExtensions.IsInterfaceType(_containingType))
				{
					SyntaxToken implementsKeyword = implementsClause.ImplementsKeyword;
					Binder.ReportDiagnostic(diagnostics, implementsKeyword, ERRID.ERR_InterfaceEventCantUse1, implementsKeyword.ValueText);
				}
				else
				{
					if (!IsShared || TypeSymbolExtensions.IsModuleType(_containingType))
					{
						return ImplementsHelper.ProcessImplementsClause(implementsClause, (EventSymbol)this, _containingType, binder, diagnostics);
					}
					Binder.ReportDiagnostic(diagnostics, Microsoft.CodeAnalysis.VisualBasicExtensions.First(eventStatementSyntax.Modifiers, SyntaxKind.SharedKeyword), ERRID.ERR_SharedOnProcThatImpl);
				}
			}
			return ImmutableArray<EventSymbol>.Empty;
		}

		private void CheckExplicitImplementationTypes()
		{
			if (((uint)_lazyState & 7u) != 0)
			{
				return;
			}
			BindingDiagnosticBag bindingDiagnosticBag = null;
			TypeSymbol type = Type;
			ImmutableArray<EventSymbol>.Enumerator enumerator = ExplicitInterfaceImplementations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EventSymbol current = enumerator.Current;
				if (!TypeSymbolExtensions.IsSameType(current.Type, type, TypeCompareKind.IgnoreTupleNames))
				{
					if (bindingDiagnosticBag == null)
					{
						bindingDiagnosticBag = BindingDiagnosticBag.GetInstance();
					}
					Location implementingLocation = GetImplementingLocation(current);
					bindingDiagnosticBag.Add(ERRID.ERR_EventImplMismatch5, implementingLocation, this, current, current.ContainingType, type, current.Type);
				}
			}
			if (bindingDiagnosticBag != null)
			{
				ContainingSourceModule.AtomicSetFlagAndStoreDiagnostics(ref _lazyState, 4, 0, bindingDiagnosticBag);
				bindingDiagnosticBag.Free();
			}
		}

		private CustomEventAccessorSymbol BindEventAccessor(AccessorBlockSyntax blockSyntax, Binder binder)
		{
			MethodBaseSyntax blockStatement = blockSyntax.BlockStatement;
			SourceMemberFlags sourceMemberFlags = _memberFlags;
			if (IsImplementing())
			{
				sourceMemberFlags = sourceMemberFlags | SourceMemberFlags.Overrides | SourceMemberFlags.NotOverridable;
			}
			sourceMemberFlags = blockSyntax.Kind() switch
			{
				SyntaxKind.AddHandlerAccessorBlock => sourceMemberFlags | SourceMemberFlags.MethodKindEventAdd | SourceMemberFlags.Dim, 
				SyntaxKind.RemoveHandlerAccessorBlock => sourceMemberFlags | SourceMemberFlags.MethodKindEventRemove | SourceMemberFlags.Dim, 
				SyntaxKind.RaiseEventAccessorBlock => sourceMemberFlags | SourceMemberFlags.MethodKindEventRaise | SourceMemberFlags.Dim, 
				_ => throw ExceptionUtilities.UnexpectedValue(blockSyntax.Kind()), 
			};
			Location location = blockStatement.GetLocation();
			return new CustomEventAccessorSymbol(_containingType, this, Binder.GetAccessorName(Name, SourceMemberFlagsExtensions.ToMethodKind(sourceMemberFlags), isWinMd: false), sourceMemberFlags, binder.GetSyntaxReference(blockStatement), location);
		}

		private bool IsImplementing()
		{
			return !ExplicitInterfaceImplementations.IsEmpty;
		}

		internal ImmutableArray<MethodSymbol> GetAccessorImplementations(MethodKind kind)
		{
			ImmutableArray<EventSymbol> explicitInterfaceImplementations = ExplicitInterfaceImplementations;
			if (explicitInterfaceImplementations.IsEmpty)
			{
				return ImmutableArray<MethodSymbol>.Empty;
			}
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			ImmutableArray<EventSymbol>.Enumerator enumerator = explicitInterfaceImplementations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EventSymbol current = enumerator.Current;
				MethodSymbol methodSymbol = kind switch
				{
					MethodKind.EventAdd => current.AddMethod, 
					MethodKind.EventRemove => current.RemoveMethod, 
					MethodKind.EventRaise => current.RaiseMethod, 
					_ => throw ExceptionUtilities.UnexpectedValue(kind), 
				};
				if ((object)methodSymbol != null)
				{
					instance.Add(methodSymbol);
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return new LexicalSortKey(_location, DeclaringCompilation);
		}

		internal sealed override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Symbol.IsDefinedInSourceTree(_syntaxRef.GetSyntax(cancellationToken).Parent, tree, definedWithinSpan, cancellationToken);
		}

		public sealed override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return GetAttributesBag().Attributes;
		}

		private CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag()
		{
			if (_lazyCustomAttributesBag == null || !_lazyCustomAttributesBag.IsSealed)
			{
				LoadAndValidateAttributes(OneOrMany.Create(AttributeDeclarationSyntaxList), ref _lazyCustomAttributesBag);
			}
			return _lazyCustomAttributesBag;
		}

		internal EventWellKnownAttributeData GetDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = _lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (EventWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		internal override VisualBasicAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
		{
			VisualBasicAttributeData boundAttribute = null;
			ObsoleteAttributeData obsoleteData = null;
			if (EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out boundAttribute, out obsoleteData))
			{
				if (obsoleteData != null)
				{
					arguments.GetOrCreateData<CommonEventEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
				}
				return boundAttribute;
			}
			return base.EarlyDecodeWellKnownAttribute(ref arguments);
		}

		internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			if (attribute.IsTargetAttribute(this, AttributeDescription.TupleElementNamesAttribute))
			{
				((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt!.Location);
			}
			if (attribute.IsTargetAttribute(this, AttributeDescription.NonSerializedAttribute))
			{
				if (ContainingType.IsSerializable)
				{
					arguments.GetOrCreateData<EventWellKnownAttributeData>().HasNonSerializedAttribute = true;
				}
				else
				{
					((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_InvalidNonSerializedUsage, arguments.AttributeSyntaxOpt!.GetLocation());
				}
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
			{
				arguments.GetOrCreateData<EventWellKnownAttributeData>().HasSpecialNameAttribute = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.ExcludeFromCodeCoverageAttribute))
			{
				arguments.GetOrCreateData<EventWellKnownAttributeData>().HasExcludeFromCodeCoverageAttribute = true;
			}
			base.DecodeWellKnownAttribute(ref arguments);
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (expandIncludes)
			{
				return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, preferredCulture, expandIncludes, ref _lazyExpandedDocComment, cancellationToken);
			}
			return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, preferredCulture, expandIncludes, ref _lazyDocComment, cancellationToken);
		}

		internal static MemberModifiers DecodeModifiers(SyntaxTokenList modifiers, SourceMemberContainerTypeSymbol container, Binder binder, DiagnosticBag diagBag)
		{
			MemberModifiers memberModifiers = binder.DecodeModifiers(modifiers, SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.Shared | SourceMemberFlags.Shadows, ERRID.ERR_BadEventFlags1, Accessibility.Public, diagBag);
			return binder.ValidateEventModifiers(modifiers, memberModifiers, container, diagBag);
		}

		internal Location GetImplementingLocation(EventSymbol implementedEvent)
		{
			EventStatementSyntax eventStatementSyntax = (EventStatementSyntax)_syntaxRef.GetSyntax();
			_ = _syntaxRef.SyntaxTree;
			if (eventStatementSyntax.ImplementsClause != null)
			{
				Binder binder = CreateBinderForTypeDeclaration();
				return ImplementsHelper.FindImplementingSyntax(eventStatementSyntax.ImplementsClause, this, implementedEvent, _containingType, binder).GetLocation();
			}
			return Locations.FirstOrDefault() ?? NoLocation.Singleton;
		}

		private Binder CreateBinderForTypeDeclaration()
		{
			Binder containingBinder = BinderBuilder.CreateBinderForType(ContainingSourceModule, _syntaxRef.SyntaxTree, _containingType);
			return new LocationSpecificBinder(BindingLocation.EventSignature, this, containingBinder);
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			base.GenerateDeclarationErrors(cancellationToken);
			_ = Type;
			_ = ExplicitInterfaceImplementations;
			CheckExplicitImplementationTypes();
			if (DeclaringCompilation.EventQueue != null)
			{
				ContainingSourceModule.AtomicSetFlagAndRaiseSymbolDeclaredEvent(ref _lazyState, 8, 0, this);
			}
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (TypeSymbolExtensions.ContainsTupleNames(Type))
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeTupleNamesAttribute(Type));
			}
		}
	}
}
