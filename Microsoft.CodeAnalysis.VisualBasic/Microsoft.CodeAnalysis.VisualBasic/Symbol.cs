using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
	internal abstract class Symbol : IReference, ISymbol, ISymbolInternal, IFormattable
	{
		internal Symbol AdaptedSymbol => this;

		public virtual string Name => string.Empty;

		public virtual string MetadataName => Name;

		public abstract SymbolKind Kind { get; }

		public abstract Symbol ContainingSymbol { get; }

		public NamespaceSymbol ContainingNamespace
		{
			get
			{
				Symbol containingSymbol = ContainingSymbol;
				while ((object)containingSymbol != null)
				{
					if (containingSymbol is NamespaceSymbol result)
					{
						return result;
					}
					containingSymbol = containingSymbol.ContainingSymbol;
				}
				return null;
			}
		}

		public virtual NamedTypeSymbol ContainingType
		{
			get
			{
				Symbol containingSymbol = ContainingSymbol;
				NamedTypeSymbol namedTypeSymbol = containingSymbol as NamedTypeSymbol;
				if ((object)namedTypeSymbol == containingSymbol)
				{
					return namedTypeSymbol;
				}
				return containingSymbol.ContainingType;
			}
		}

		internal NamespaceOrTypeSymbol ContainingNamespaceOrType
		{
			get
			{
				Symbol containingSymbol = ContainingSymbol;
				if ((object)containingSymbol != null)
				{
					switch (containingSymbol.Kind)
					{
					case SymbolKind.Namespace:
						return (NamespaceSymbol)ContainingSymbol;
					case SymbolKind.NamedType:
						return (NamedTypeSymbol)ContainingSymbol;
					}
				}
				return null;
			}
		}

		public virtual AssemblySymbol ContainingAssembly => ContainingSymbol?.ContainingAssembly;

		internal virtual VisualBasicCompilation DeclaringCompilation => Kind switch
		{
			SymbolKind.ErrorType => null, 
			SymbolKind.Assembly => null, 
			SymbolKind.NetModule => null, 
			_ => (!(ContainingModule is SourceModuleSymbol sourceModuleSymbol)) ? null : sourceModuleSymbol.DeclaringCompilation, 
		};

		public Compilation ISymbolInternal_DeclaringCompilation => DeclaringCompilation;

		public virtual ModuleSymbol ContainingModule => ContainingSymbol?.ContainingModule;

		public Symbol OriginalDefinition => OriginalSymbolDefinition;

		protected virtual Symbol OriginalSymbolDefinition => this;

		public bool IsDefinition => (object)OriginalDefinition == this;

		public abstract ImmutableArray<Location> Locations { get; }

		public abstract ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }

		public abstract Accessibility DeclaredAccessibility { get; }

		public abstract bool IsShared { get; }

		public abstract bool IsOverridable { get; }

		public abstract bool IsOverrides { get; }

		public abstract bool IsMustOverride { get; }

		public abstract bool IsNotOverridable { get; }

		public virtual bool IsImplicitlyDeclared => false;

		internal ThreeState ObsoleteState
		{
			get
			{
				switch (ObsoleteKind)
				{
				case ObsoleteAttributeKind.None:
				case ObsoleteAttributeKind.Experimental:
					return ThreeState.False;
				case ObsoleteAttributeKind.Uninitialized:
					return ThreeState.Unknown;
				default:
					return ThreeState.True;
				}
			}
		}

		internal ObsoleteAttributeKind ObsoleteKind => ObsoleteAttributeData?.Kind ?? ObsoleteAttributeKind.None;

		internal abstract ObsoleteAttributeData ObsoleteAttributeData { get; }

		internal virtual Symbol ImplicitlyDefinedBy => null;

		internal virtual bool ShadowsExplicitly => false;

		public bool CanBeReferencedByName
		{
			get
			{
				switch (Kind)
				{
				case SymbolKind.Alias:
				case SymbolKind.Label:
				case SymbolKind.Local:
					return Name.Length > 0;
				case SymbolKind.NamedType:
					if (((NamedTypeSymbol)this).IsSubmissionClass)
					{
						return false;
					}
					break;
				case SymbolKind.Method:
					switch (((MethodSymbol)this).MethodKind)
					{
					case MethodKind.Conversion:
					case MethodKind.DelegateInvoke:
					case MethodKind.UserDefinedOperator:
						return true;
					default:
						return false;
					case MethodKind.Ordinary:
					case MethodKind.ReducedExtension:
					case MethodKind.DeclareMethod:
						break;
					}
					break;
				case SymbolKind.ArrayType:
				case SymbolKind.Assembly:
				case SymbolKind.NetModule:
					return false;
				default:
					throw ExceptionUtilities.UnexpectedValue(Kind);
				case SymbolKind.ErrorType:
				case SymbolKind.Event:
				case SymbolKind.Field:
				case SymbolKind.Namespace:
				case SymbolKind.Parameter:
				case SymbolKind.Property:
				case SymbolKind.RangeVariable:
				case SymbolKind.TypeParameter:
					break;
				}
				if (Dangerous_IsFromSomeCompilationIncludingRetargeting)
				{
					return !string.IsNullOrEmpty(Name) && SyntaxFacts.IsIdentifierStartCharacter(Name[0]);
				}
				return SyntaxFacts.IsValidIdentifier(Name);
			}
		}

		internal bool CanBeReferencedByNameIgnoringIllegalCharacters
		{
			get
			{
				if (Kind == SymbolKind.Method)
				{
					switch (((MethodSymbol)this).MethodKind)
					{
					case MethodKind.Conversion:
					case MethodKind.DelegateInvoke:
					case MethodKind.UserDefinedOperator:
					case MethodKind.Ordinary:
					case MethodKind.ReducedExtension:
					case MethodKind.DeclareMethod:
						return true;
					default:
						return false;
					}
				}
				return true;
			}
		}

		internal bool IsEmbedded => EmbeddedSymbolKind != EmbeddedSymbolKind.None;

		internal virtual EmbeddedSymbolKind EmbeddedSymbolKind => EmbeddedSymbolKind.None;

		internal CharSet? EffectiveDefaultMarshallingCharSet
		{
			get
			{
				if (!IsEmbedded)
				{
					return ContainingModule.DefaultMarshallingCharSet;
				}
				return null;
			}
		}

		internal bool Dangerous_IsFromSomeCompilationIncludingRetargeting
		{
			get
			{
				if (DeclaringCompilation != null)
				{
					return true;
				}
				if (Kind == SymbolKind.Assembly)
				{
					return this is RetargetingAssemblySymbol retargetingAssemblySymbol && retargetingAssemblySymbol.UnderlyingAssembly.DeclaringCompilation != null;
				}
				return ((Kind == SymbolKind.NetModule) ? this : ContainingModule) is RetargetingModuleSymbol retargetingModuleSymbol && retargetingModuleSymbol.UnderlyingModule.DeclaringCompilation != null;
			}
		}

		internal virtual bool IsLambdaMethod => false;

		internal virtual bool IsMyGroupCollectionProperty => false;

		internal virtual bool IsQueryLambdaMethod => false;

		internal AssemblySymbol PrimaryDependency
		{
			get
			{
				AssemblySymbol containingAssembly = ContainingAssembly;
				if ((object)containingAssembly != null && containingAssembly.CorLibrary == containingAssembly)
				{
					return null;
				}
				return containingAssembly;
			}
		}

		public virtual bool HasUnsupportedMetadata => false;

		protected virtual int HighestPriorityUseSiteError => int.MaxValue;

		private IAssemblySymbol ISymbol_ContainingAssembly => ContainingAssembly;

		private IAssemblySymbolInternal ISymbolInternal_ContainingAssembly => ContainingAssembly;

		private IModuleSymbol ISymbol_ContainingModule => ContainingModule;

		private IModuleSymbolInternal ISymbolInternal_ContainingModule => ContainingModule;

		private INamespaceSymbol ISymbol_ContainingNamespace => ContainingNamespace;

		private INamespaceSymbolInternal ISymbolInternal_ContainingNamespace => ContainingNamespace;

		private ISymbol ISymbol_ContainingSymbol => ContainingSymbol;

		private ISymbolInternal ISymbolInternal_ContainingSymbol => ContainingSymbol;

		private INamedTypeSymbol ISymbol_ContainingType => ContainingType;

		private INamedTypeSymbolInternal ISymbolInternal_ContainingType => ContainingType;

		private Accessibility ISymbol_DeclaredAccessibility => DeclaredAccessibility;

		protected virtual bool ISymbol_IsAbstract => IsMustOverride;

		private bool ISymbol_IsDefinition => IsDefinition;

		private bool ISymbol_IsOverride => IsOverrides;

		protected virtual bool ISymbol_IsSealed => IsNotOverridable;

		protected virtual bool ISymbol_IsStatic => IsShared;

		private bool ISymbol_IsImplicitlyDeclared => IsImplicitlyDeclared;

		private bool ISymbol_IsVirtual => IsOverridable;

		private bool ISymbol_CanBeReferencedByName => CanBeReferencedByName;

		public string Language => "Visual Basic";

		private ImmutableArray<Location> ISymbol_Locations => Locations;

		private ImmutableArray<SyntaxReference> ISymbol_DeclaringSyntaxReferences => DeclaringSyntaxReferences;

		private string ISymbol_Name => Name;

		private ISymbol ISymbol_OriginalDefinition => OriginalDefinition;

		private SymbolKind ISymbol_Kind => Kind;

		private bool ISymbol_IsExtern => false;

		internal virtual IDefinition IReferenceAsDefinition(EmitContext context)
		{
			throw ExceptionUtilities.Unreachable;
		}

		IDefinition IReference.AsDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceAsDefinition
			return this.IReferenceAsDefinition(context);
		}

		private ISymbolInternal IReferenceGetInternalSymbol()
		{
			return AdaptedSymbol;
		}

		ISymbolInternal IReference.GetInternalSymbol()
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceGetInternalSymbol
			return this.IReferenceGetInternalSymbol();
		}

		internal virtual void IReferenceDispatch(MetadataVisitor visitor)
		{
			throw ExceptionUtilities.Unreachable;
		}

		void IReference.Dispatch(MetadataVisitor visitor)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceDispatch
			this.IReferenceDispatch(visitor);
		}

		private IEnumerable<ICustomAttribute> IReferenceGetAttributes(EmitContext context)
		{
			return AdaptedSymbol.GetCustomAttributesToEmit(((PEModuleBuilder)context.Module).CompilationState);
		}

		IEnumerable<ICustomAttribute> IReference.GetAttributes(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IReferenceGetAttributes
			return this.IReferenceGetAttributes(context);
		}

		internal Symbol GetCciAdapter()
		{
			return this;
		}

		private IReference ISymbolInternalGetCciAdapter()
		{
			return GetCciAdapter();
		}

		IReference ISymbolInternal.GetCciAdapter()
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbolInternalGetCciAdapter
			return this.ISymbolInternalGetCciAdapter();
		}

		internal bool IsDefinitionOrDistinct()
		{
			if (!IsDefinition)
			{
				return !Equals(OriginalDefinition);
			}
			return true;
		}

		internal virtual IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return GetCustomAttributesToEmit(compilationState, emittingAssemblyAttributesInNetModule: false);
		}

		internal IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState, bool emittingAssemblyAttributesInNetModule)
		{
			ArrayBuilder<SynthesizedAttributeData> attributes = null;
			AddSynthesizedAttributes(compilationState, ref attributes);
			return GetCustomAttributesToEmit(GetAttributes(), attributes, isReturnType: false, emittingAssemblyAttributesInNetModule);
		}

		internal IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ImmutableArray<VisualBasicAttributeData> userDefined, ArrayBuilder<SynthesizedAttributeData> synthesized, bool isReturnType, bool emittingAssemblyAttributesInNetModule)
		{
			if (userDefined.IsEmpty && synthesized == null)
			{
				return SpecializedCollections.EmptyEnumerable<VisualBasicAttributeData>();
			}
			return GetCustomAttributesToEmitIterator(userDefined, synthesized, isReturnType, emittingAssemblyAttributesInNetModule);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_12_GetCustomAttributesToEmitIterator))]
		private IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmitIterator(ImmutableArray<VisualBasicAttributeData> userDefined, ArrayBuilder<SynthesizedAttributeData> synthesized, bool isReturnType, bool emittingAssemblyAttributesInNetModule)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_12_GetCustomAttributesToEmitIterator(-2)
			{
				_0024VB_0024Me = this,
				_0024P_userDefined = userDefined,
				_0024P_synthesized = synthesized,
				_0024P_isReturnType = isReturnType,
				_0024P_emittingAssemblyAttributesInNetModule = emittingAssemblyAttributesInNetModule
			};
		}

		[Conditional("DEBUG")]
		protected internal void CheckDefinitionInvariant()
		{
		}

		internal static bool HaveSameSignature(MethodSymbol method1, MethodSymbol method2)
		{
			return MethodSignatureComparer.DetailedCompare(method1, method2, (SymbolComparisonResults)115525) == (SymbolComparisonResults)0;
		}

		internal static bool HaveSameSignatureAndConstraintsAndReturnType(MethodSymbol method1, MethodSymbol method2)
		{
			return MethodSignatureComparer.VisualBasicSignatureAndConstraintsAndReturnTypeComparer.Equals(method1, method2);
		}

		public static bool IsSymbolAccessible(Symbol symbol, NamedTypeSymbol within, NamedTypeSymbol throughTypeOpt = null)
		{
			if ((object)symbol == null)
			{
				throw new ArgumentNullException("symbol");
			}
			if ((object)within == null)
			{
				throw new ArgumentNullException("within");
			}
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			return AccessCheck.IsSymbolAccessible(symbol, within, throughTypeOpt, ref useSiteInfo);
		}

		public static bool IsSymbolAccessible(Symbol symbol, AssemblySymbol within)
		{
			if ((object)symbol == null)
			{
				throw new ArgumentNullException("symbol");
			}
			if ((object)within == null)
			{
				throw new ArgumentNullException("within");
			}
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			return AccessCheck.IsSymbolAccessible(symbol, within, ref useSiteInfo);
		}

		internal virtual void SetMetadataName(string metadataName)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal virtual LexicalSortKey GetLexicalSortKey()
		{
			ImmutableArray<Location> locations = Locations;
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			if (locations.Length <= 0)
			{
				return LexicalSortKey.NotInSource;
			}
			return new LexicalSortKey(locations[0], declaringCompilation);
		}

		internal static ImmutableArray<VisualBasicSyntaxNode> GetDeclaringSyntaxNodeHelper<TNode>(ImmutableArray<Location> locations) where TNode : VisualBasicSyntaxNode
		{
			if (locations.IsEmpty)
			{
				return ImmutableArray<VisualBasicSyntaxNode>.Empty;
			}
			ArrayBuilder<VisualBasicSyntaxNode> instance = ArrayBuilder<VisualBasicSyntaxNode>.GetInstance();
			ImmutableArray<Location>.Enumerator enumerator = locations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Location current = enumerator.Current;
				if (!current.IsInSource || current.SourceTree == null)
				{
					continue;
				}
				SyntaxToken token = current.SourceTree!.GetRoot().FindToken(current.SourceSpan.Start);
				if (VisualBasicExtensions.Kind(token) != 0)
				{
					VisualBasicSyntaxNode visualBasicSyntaxNode = token.Parent!.FirstAncestorOrSelf<TNode>();
					if (visualBasicSyntaxNode != null)
					{
						instance.Add(visualBasicSyntaxNode);
					}
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal static ImmutableArray<SyntaxReference> GetDeclaringSyntaxReferenceHelper<TNode>(ImmutableArray<Location> locations) where TNode : VisualBasicSyntaxNode
		{
			ImmutableArray<VisualBasicSyntaxNode> declaringSyntaxNodeHelper = GetDeclaringSyntaxNodeHelper<TNode>(locations);
			if (declaringSyntaxNodeHelper.IsEmpty)
			{
				return ImmutableArray<SyntaxReference>.Empty;
			}
			ArrayBuilder<SyntaxReference> instance = ArrayBuilder<SyntaxReference>.GetInstance();
			ImmutableArray<VisualBasicSyntaxNode>.Enumerator enumerator = declaringSyntaxNodeHelper.GetEnumerator();
			while (enumerator.MoveNext())
			{
				VisualBasicSyntaxNode current = enumerator.Current;
				instance.Add(current.GetReference());
			}
			return instance.ToImmutableAndFree();
		}

		internal static ImmutableArray<SyntaxReference> GetDeclaringSyntaxReferenceHelper(ImmutableArray<SyntaxReference> references)
		{
			if (references.Length == 1)
			{
				return GetDeclaringSyntaxReferenceHelper(references[0]);
			}
			ArrayBuilder<SyntaxReference> instance = ArrayBuilder<SyntaxReference>.GetInstance();
			ImmutableArray<SyntaxReference>.Enumerator enumerator = references.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				if (!EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(current.SyntaxTree))
				{
					instance.Add(new BeginOfBlockSyntaxReference(current));
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal static ImmutableArray<SyntaxReference> GetDeclaringSyntaxReferenceHelper(SyntaxReference reference)
		{
			if (reference != null && !EmbeddedSymbolExtensions.IsEmbeddedOrMyTemplateTree(reference.SyntaxTree))
			{
				return ImmutableArray.Create((SyntaxReference)new BeginOfBlockSyntaxReference(reference));
			}
			return ImmutableArray<SyntaxReference>.Empty;
		}

		internal bool IsFromCompilation(VisualBasicCompilation compilation)
		{
			return compilation == DeclaringCompilation;
		}

		internal bool GetGuidStringDefaultImplementation(out string guidString)
		{
			ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = GetAttributes().GetEnumerator();
			while (enumerator.MoveNext())
			{
				VisualBasicAttributeData current = enumerator.Current;
				if (current.IsTargetAttribute(this, AttributeDescription.GuidAttribute) && current.TryGetGuidAttributeValue(out guidString))
				{
					return true;
				}
			}
			guidString = null;
			return false;
		}

		public virtual string GetDocumentationCommentId()
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			DocumentationCommentIdVisitor.Instance.Visit(this, instance.Builder);
			string text = instance.ToStringAndFree();
			if (text.Length != 0)
			{
				return text;
			}
			return null;
		}

		string ISymbol.GetDocumentationCommentId()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetDocumentationCommentId
			return this.GetDocumentationCommentId();
		}

		public virtual string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return "";
		}

		string ISymbol.GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetDocumentationCommentXml
			return this.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		public static bool operator ==(Symbol left, Symbol right)
		{
			if ((object)right == null)
			{
				return (object)left == null;
			}
			return (object)left == right || right.Equals(left);
		}

		public static bool operator !=(Symbol left, Symbol right)
		{
			if ((object)right == null)
			{
				return (object)left != null;
			}
			return (object)left != right && !right.Equals(left);
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		private bool IEquatable_Equals(ISymbol other)
		{
			return Equals(other as Symbol, SymbolEqualityComparer.Default.CompareKind);
		}

		bool IEquatable<ISymbol>.Equals(ISymbol other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IEquatable_Equals
			return this.IEquatable_Equals(other);
		}

		private bool ISymbol_Equals(ISymbol other, SymbolEqualityComparer equalityComparer)
		{
			return Equals(other as Symbol, equalityComparer.CompareKind);
		}

		bool ISymbol.Equals(ISymbol other, SymbolEqualityComparer equalityComparer)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbol_Equals
			return this.ISymbol_Equals(other, equalityComparer);
		}

		private bool ISymbolInternal_Equals(ISymbolInternal other, TypeCompareKind compareKind)
		{
			return Equals(other as Symbol, compareKind);
		}

		bool ISymbolInternal.Equals(ISymbolInternal other, TypeCompareKind compareKind)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbolInternal_Equals
			return this.ISymbolInternal_Equals(other, compareKind);
		}

		public virtual bool Equals(Symbol other, TypeCompareKind compareKind)
		{
			return Equals(other);
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public sealed override string ToString()
		{
			return ToDisplayString(SymbolDisplayFormat.VisualBasicErrorMessageFormat);
		}

		public string ToDisplayString(SymbolDisplayFormat format = null)
		{
			return SymbolDisplay.ToDisplayString(this, format);
		}

		public ImmutableArray<SymbolDisplayPart> ToDisplayParts(SymbolDisplayFormat format = null)
		{
			return SymbolDisplay.ToDisplayParts(this, format);
		}

		public string ToMinimalDisplayString(SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
		{
			return SymbolDisplay.ToMinimalDisplayString(this, semanticModel, position, format);
		}

		public ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
		{
			return SymbolDisplay.ToMinimalDisplayParts(this, semanticModel, position, format);
		}

		private string GetDebuggerDisplay()
		{
			return $"{Kind} {ToDisplayString(SymbolDisplayFormat.TestFormat)}";
		}

		internal abstract TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg);

		internal Symbol()
		{
		}

		internal virtual bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			ImmutableArray<SyntaxReference> declaringSyntaxReferences = DeclaringSyntaxReferences;
			if (IsImplicitlyDeclared && declaringSyntaxReferences.Length == 0)
			{
				return ContainingSymbol.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken);
			}
			ImmutableArray<SyntaxReference>.Enumerator enumerator = declaringSyntaxReferences.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				cancellationToken.ThrowIfCancellationRequested();
				if (current.SyntaxTree == tree && (!definedWithinSpan.HasValue || current.Span.IntersectsWith(definedWithinSpan.Value)))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsDefinedInSourceTree(SyntaxNode syntaxNode, SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (syntaxNode != null && syntaxNode.SyntaxTree == tree)
			{
				if (definedWithinSpan.HasValue)
				{
					return definedWithinSpan.Value.IntersectsWith(syntaxNode.FullSpan);
				}
				return true;
			}
			return false;
		}

		internal virtual void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
		}

		internal virtual UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			return default(UseSiteInfo<AssemblySymbol>);
		}

		internal UseSiteInfo<AssemblySymbol> DeriveUseSiteInfoFromType(TypeSymbol type)
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = type.GetUseSiteInfo();
			if (useSiteInfo.DiagnosticInfo != null)
			{
				int code = useSiteInfo.DiagnosticInfo!.Code;
				if (code == 30649)
				{
					GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(ref useSiteInfo);
				}
			}
			return useSiteInfo;
		}

		private void GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(ref UseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			switch (Kind)
			{
			case SymbolKind.Field:
				useSiteInfo = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedField1, CustomSymbolDisplayFormatter.ShortErrorName(this)));
				break;
			case SymbolKind.Method:
				useSiteInfo = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, CustomSymbolDisplayFormatter.ShortErrorName(this)));
				break;
			case SymbolKind.Property:
				useSiteInfo = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedProperty1, CustomSymbolDisplayFormatter.ShortErrorName(this)));
				break;
			}
		}

		internal UseSiteInfo<AssemblySymbol> MergeUseSiteInfo(UseSiteInfo<AssemblySymbol> first, UseSiteInfo<AssemblySymbol> second)
		{
			MergeUseSiteInfo(ref first, second, HighestPriorityUseSiteError);
			return first;
		}

		internal static bool MergeUseSiteInfo(ref UseSiteInfo<AssemblySymbol> result, UseSiteInfo<AssemblySymbol> other, int highestPriorityUseSiteError)
		{
			DiagnosticInfo? diagnosticInfo = other.DiagnosticInfo;
			if (diagnosticInfo != null && diagnosticInfo!.Code == highestPriorityUseSiteError)
			{
				result = other;
				return true;
			}
			if (result.DiagnosticInfo == null)
			{
				if (other.DiagnosticInfo != null)
				{
					result = other;
				}
				else
				{
					AssemblySymbol primaryDependency = result.PrimaryDependency;
					ImmutableHashSet<AssemblySymbol> secondaryDependencies = result.SecondaryDependencies;
					other.MergeDependencies(ref primaryDependency, ref secondaryDependencies);
					result = new UseSiteInfo<AssemblySymbol>(null, primaryDependency, secondaryDependencies);
				}
				return false;
			}
			return result.DiagnosticInfo!.Code == highestPriorityUseSiteError;
		}

		internal UseSiteInfo<AssemblySymbol> DeriveUseSiteInfoFromParameter(ParameterSymbol param, int highestPriorityUseSiteError)
		{
			UseSiteInfo<AssemblySymbol> result = DeriveUseSiteInfoFromType(param.Type);
			DiagnosticInfo? diagnosticInfo = result.DiagnosticInfo;
			if (diagnosticInfo != null && diagnosticInfo!.Code == highestPriorityUseSiteError)
			{
				return result;
			}
			UseSiteInfo<AssemblySymbol> result2 = DeriveUseSiteInfoFromCustomModifiers(param.RefCustomModifiers);
			DiagnosticInfo? diagnosticInfo2 = result2.DiagnosticInfo;
			if (diagnosticInfo2 != null && diagnosticInfo2!.Code == highestPriorityUseSiteError)
			{
				return result2;
			}
			UseSiteInfo<AssemblySymbol> result3 = DeriveUseSiteInfoFromCustomModifiers(param.CustomModifiers);
			DiagnosticInfo? diagnosticInfo3 = result3.DiagnosticInfo;
			if (diagnosticInfo3 != null && diagnosticInfo3!.Code == highestPriorityUseSiteError)
			{
				return result3;
			}
			DiagnosticInfo diagnosticInfo4 = result.DiagnosticInfo ?? result2.DiagnosticInfo ?? result3.DiagnosticInfo;
			UseSiteInfo<AssemblySymbol> result4;
			if (diagnosticInfo4 != null)
			{
				result4 = new UseSiteInfo<AssemblySymbol>(diagnosticInfo4);
			}
			else
			{
				AssemblySymbol primaryDependency = result.PrimaryDependency;
				ImmutableHashSet<AssemblySymbol> secondaryDependencies = result.SecondaryDependencies;
				result2.MergeDependencies(ref primaryDependency, ref secondaryDependencies);
				result3.MergeDependencies(ref primaryDependency, ref secondaryDependencies);
				result4 = new UseSiteInfo<AssemblySymbol>(null, primaryDependency, secondaryDependencies);
			}
			return result4;
		}

		internal UseSiteInfo<AssemblySymbol> DeriveUseSiteInfoFromParameters(ImmutableArray<ParameterSymbol> parameters)
		{
			UseSiteInfo<AssemblySymbol> result = default(UseSiteInfo<AssemblySymbol>);
			int highestPriorityUseSiteError = HighestPriorityUseSiteError;
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				if (MergeUseSiteInfo(ref result, DeriveUseSiteInfoFromParameter(current, highestPriorityUseSiteError), highestPriorityUseSiteError))
				{
					break;
				}
			}
			return result;
		}

		internal UseSiteInfo<AssemblySymbol> DeriveUseSiteInfoFromCustomModifiers(ImmutableArray<CustomModifier> customModifiers, bool allowIsExternalInit = false)
		{
			UseSiteInfo<AssemblySymbol> result = default(UseSiteInfo<AssemblySymbol>);
			int highestPriorityUseSiteError = HighestPriorityUseSiteError;
			ImmutableArray<CustomModifier>.Enumerator enumerator = customModifiers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CustomModifier current = enumerator.Current;
				UseSiteInfo<AssemblySymbol> useSiteInfo;
				if (!current.IsOptional && (!allowIsExternalInit || !TypeSymbolExtensions.IsWellKnownTypeIsExternalInit(((VisualBasicCustomModifier)current).ModifierSymbol)))
				{
					useSiteInfo = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, string.Empty));
					GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(ref useSiteInfo);
					if (MergeUseSiteInfo(ref result, useSiteInfo, highestPriorityUseSiteError))
					{
						break;
					}
				}
				useSiteInfo = DeriveUseSiteInfoFromType(((VisualBasicCustomModifier)current).ModifierSymbol);
				if (MergeUseSiteInfo(ref result, useSiteInfo, highestPriorityUseSiteError))
				{
					break;
				}
			}
			return result;
		}

		internal static DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive<T>(ImmutableArray<T> types, Symbol owner, ref HashSet<TypeSymbol> checkedTypes) where T : TypeSymbol
		{
			ImmutableArray<T>.Enumerator enumerator = types.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DiagnosticInfo unificationUseSiteDiagnosticRecursive = enumerator.Current.GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes);
				if (unificationUseSiteDiagnosticRecursive != null)
				{
					return unificationUseSiteDiagnosticRecursive;
				}
			}
			return null;
		}

		internal static DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(ImmutableArray<CustomModifier> modifiers, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			ImmutableArray<CustomModifier>.Enumerator enumerator = modifiers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DiagnosticInfo unificationUseSiteDiagnosticRecursive = ((TypeSymbol)enumerator.Current.Modifier).GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes);
				if (unificationUseSiteDiagnosticRecursive != null)
				{
					return unificationUseSiteDiagnosticRecursive;
				}
			}
			return null;
		}

		internal static DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(ImmutableArray<ParameterSymbol> parameters, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				DiagnosticInfo diagnosticInfo = current.Type.GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes) ?? GetUnificationUseSiteDiagnosticRecursive(current.RefCustomModifiers, owner, ref checkedTypes) ?? GetUnificationUseSiteDiagnosticRecursive(current.CustomModifiers, owner, ref checkedTypes);
				if (diagnosticInfo != null)
				{
					return diagnosticInfo;
				}
			}
			return null;
		}

		internal static DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(ImmutableArray<TypeParameterSymbol> typeParameters, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				DiagnosticInfo unificationUseSiteDiagnosticRecursive = GetUnificationUseSiteDiagnosticRecursive(enumerator.Current.ConstraintTypesNoUseSiteDiagnostics, owner, ref checkedTypes);
				if (unificationUseSiteDiagnosticRecursive != null)
				{
					return unificationUseSiteDiagnosticRecursive;
				}
			}
			return null;
		}

		public abstract void Accept(SymbolVisitor visitor);

		void ISymbol.Accept(SymbolVisitor visitor)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Accept
			this.Accept(visitor);
		}

		public abstract TResult Accept<TResult>(SymbolVisitor<TResult> visitor);

		TResult ISymbol.Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Accept
			return this.Accept<TResult>(visitor);
		}

		public abstract void Accept(VisualBasicSymbolVisitor visitor);

		public abstract TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor);

		private string ISymbol_ToDisplayString(SymbolDisplayFormat format = null)
		{
			return SymbolDisplay.ToDisplayString(this, format);
		}

		string ISymbol.ToDisplayString(SymbolDisplayFormat format = null)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbol_ToDisplayString
			return this.ISymbol_ToDisplayString(format);
		}

		private ImmutableArray<SymbolDisplayPart> ISymbol_ToDisplayParts(SymbolDisplayFormat format = null)
		{
			return SymbolDisplay.ToDisplayParts(this, format);
		}

		ImmutableArray<SymbolDisplayPart> ISymbol.ToDisplayParts(SymbolDisplayFormat format = null)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbol_ToDisplayParts
			return this.ISymbol_ToDisplayParts(format);
		}

		private string ISymbol_ToMinimalDisplayString(SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
		{
			return SymbolDisplay.ToMinimalDisplayString(this, semanticModel, position, format);
		}

		string ISymbol.ToMinimalDisplayString(SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbol_ToMinimalDisplayString
			return this.ISymbol_ToMinimalDisplayString(semanticModel, position, format);
		}

		private ImmutableArray<SymbolDisplayPart> ISymbol_ToMinimalDisplayParts(SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
		{
			return SymbolDisplay.ToMinimalDisplayParts(this, semanticModel, position, format);
		}

		ImmutableArray<SymbolDisplayPart> ISymbol.ToMinimalDisplayParts(SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbol_ToMinimalDisplayParts
			return this.ISymbol_ToMinimalDisplayParts(semanticModel, position, format);
		}

		private ImmutableArray<AttributeData> ISymbol_GetAttributes()
		{
			return StaticCast<AttributeData>.From(GetAttributes());
		}

		ImmutableArray<AttributeData> ISymbol.GetAttributes()
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbol_GetAttributes
			return this.ISymbol_GetAttributes();
		}

		protected static ImmutableArray<TypeSymbol> ConstructTypeArguments(params ITypeSymbol[] typeArguments)
		{
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance(typeArguments.Length);
			foreach (ITypeSymbol symbol in typeArguments)
			{
				instance.Add(SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(symbol, "typeArguments"));
			}
			return instance.ToImmutableAndFree();
		}

		protected static ImmutableArray<TypeSymbol> ConstructTypeArguments(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<NullableAnnotation> typeArgumentNullableAnnotations)
		{
			if (typeArguments.IsDefault)
			{
				throw new ArgumentException("typeArguments");
			}
			int length = typeArguments.Length;
			if (!typeArgumentNullableAnnotations.IsDefault && typeArgumentNullableAnnotations.Length != length)
			{
				throw new ArgumentException("typeArgumentNullableAnnotations");
			}
			return typeArguments.SelectAsArray((ITypeSymbol typeArg) => SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(typeArg, "typeArguments"));
		}

		private string IFormattable_ToString(string format, IFormatProvider formatProvider)
		{
			return ToString();
		}

		string IFormattable.ToString(string format, IFormatProvider formatProvider)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IFormattable_ToString
			return this.IFormattable_ToString(format, formatProvider);
		}

		private ISymbol ISymbolInternal_GetISymbol()
		{
			return this;
		}

		ISymbol ISymbolInternal.GetISymbol()
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISymbolInternal_GetISymbol
			return this.ISymbolInternal_GetISymbol();
		}

		public virtual ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal virtual void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
		}

		internal static void AddSynthesizedAttribute(ref ArrayBuilder<SynthesizedAttributeData> attributes, SynthesizedAttributeData attribute)
		{
			if (attribute != null)
			{
				if (attributes == null)
				{
					attributes = ArrayBuilder<SynthesizedAttributeData>.GetInstance(4);
				}
				attributes.Add(attribute);
			}
		}

		internal AttributeTargets GetAttributeTarget()
		{
			switch (Kind)
			{
			case SymbolKind.Assembly:
				return AttributeTargets.Assembly;
			case SymbolKind.Event:
				return AttributeTargets.Event;
			case SymbolKind.Field:
				return AttributeTargets.Field;
			case SymbolKind.Method:
				switch (((MethodSymbol)this).MethodKind)
				{
				case MethodKind.Constructor:
				case MethodKind.StaticConstructor:
					return AttributeTargets.Constructor;
				case MethodKind.Conversion:
				case MethodKind.DelegateInvoke:
				case MethodKind.EventAdd:
				case MethodKind.EventRaise:
				case MethodKind.EventRemove:
				case MethodKind.UserDefinedOperator:
				case MethodKind.Ordinary:
				case MethodKind.PropertyGet:
				case MethodKind.PropertySet:
				case MethodKind.DeclareMethod:
					return AttributeTargets.Method;
				}
				break;
			case SymbolKind.Property:
				return AttributeTargets.Property;
			case SymbolKind.NamedType:
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)this;
				switch (namedTypeSymbol.TypeKind)
				{
				case TypeKind.Class:
				case TypeKind.Module:
					return AttributeTargets.Class;
				case TypeKind.Struct:
					return AttributeTargets.Struct;
				case TypeKind.Interface:
					return AttributeTargets.Interface;
				case TypeKind.Enum:
					return AttributeTargets.Struct | AttributeTargets.Enum;
				case TypeKind.Delegate:
					return AttributeTargets.Delegate;
				case TypeKind.Submission:
					throw ExceptionUtilities.UnexpectedValue(namedTypeSymbol.TypeKind);
				}
				break;
			}
			case SymbolKind.NetModule:
				return AttributeTargets.Module;
			case SymbolKind.Parameter:
				return AttributeTargets.Parameter;
			case SymbolKind.TypeParameter:
				return AttributeTargets.GenericParameter;
			}
			return (AttributeTargets)0;
		}

		internal virtual VisualBasicAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
		{
			return null;
		}

		internal bool EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments, out VisualBasicAttributeData boundAttribute, out ObsoleteAttributeData obsoleteData)
		{
			NamedTypeSymbol attributeType = arguments.AttributeType;
			AttributeSyntax attributeSyntax = arguments.AttributeSyntax;
			ObsoleteAttributeKind kind;
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.ObsoleteAttribute))
			{
				kind = ObsoleteAttributeKind.Obsolete;
			}
			else if (VisualBasicAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.DeprecatedAttribute))
			{
				kind = ObsoleteAttributeKind.Deprecated;
			}
			else
			{
				if (!VisualBasicAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.ExperimentalAttribute))
				{
					boundAttribute = null;
					obsoleteData = null;
					return false;
				}
				kind = ObsoleteAttributeKind.Experimental;
			}
			bool generatedDiagnostics = false;
			boundAttribute = arguments.Binder.GetAttribute(attributeSyntax, attributeType, out generatedDiagnostics);
			if (!boundAttribute.HasErrors)
			{
				obsoleteData = boundAttribute.DecodeObsoleteAttribute(kind);
				if (generatedDiagnostics)
				{
					boundAttribute = null;
				}
			}
			else
			{
				obsoleteData = null;
				boundAttribute = null;
			}
			return true;
		}

		internal virtual void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			MarkEmbeddedAttributeTypeReference(arguments.Attribute, arguments.AttributeSyntaxOpt, declaringCompilation);
			ReportExtensionAttributeUseSiteInfo(arguments.Attribute, arguments.AttributeSyntaxOpt, declaringCompilation, (BindingDiagnosticBag)arguments.Diagnostics);
			if (arguments.Attribute.IsTargetAttribute(this, AttributeDescription.SkipLocalsInitAttribute))
			{
				((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.WRN_AttributeNotSupportedInVB, arguments.AttributeSyntaxOpt!.Location, AttributeDescription.SkipLocalsInitAttribute.FullName);
			}
		}

		internal virtual void PostDecodeWellKnownAttributes(ImmutableArray<VisualBasicAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
		{
		}

		internal void LoadAndValidateAttributes(OneOrMany<SyntaxList<AttributeListSyntax>> attributeBlockSyntaxList, ref CustomAttributesBag<VisualBasicAttributeData> lazyCustomAttributesBag, AttributeLocation symbolPart = AttributeLocation.None)
		{
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			SourceAssemblySymbol obj = (SourceAssemblySymbol)((Kind == SymbolKind.Assembly) ? this : ContainingAssembly);
			SourceModuleSymbol sourceModule = obj.SourceModule;
			VisualBasicCompilation declaringCompilation = obj.DeclaringCompilation;
			ImmutableArray<Binder> binders = default(ImmutableArray<Binder>);
			ImmutableArray<AttributeSyntax> attributesToBind = GetAttributesToBind(attributeBlockSyntaxList, symbolPart, declaringCompilation, out binders);
			ImmutableArray<VisualBasicAttributeData> immutableArray;
			WellKnownAttributeData wellKnownAttributeData;
			if (attributesToBind.Any())
			{
				if (lazyCustomAttributesBag == null)
				{
					Interlocked.CompareExchange(ref lazyCustomAttributesBag, new CustomAttributesBag<VisualBasicAttributeData>(), null);
				}
				ImmutableArray<NamedTypeSymbol> boundAttributeTypes = Binder.BindAttributeTypes(binders, attributesToBind, this, instance);
				VisualBasicAttributeData[] array = new VisualBasicAttributeData[boundAttributeTypes.Length - 1 + 1];
				EarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = EarlyDecodeWellKnownAttributes(binders, boundAttributeTypes, attributesToBind, array, symbolPart);
				lazyCustomAttributesBag.SetEarlyDecodedWellKnownAttributeData(earlyDecodedWellKnownAttributeData);
				Binder.GetAttributes(binders, attributesToBind, boundAttributeTypes, array, this, instance);
				immutableArray = array.AsImmutableOrNull();
				wellKnownAttributeData = ValidateAttributeUsageAndDecodeWellKnownAttributes(binders, attributesToBind, immutableArray, instance, symbolPart);
				lazyCustomAttributesBag.SetDecodedWellKnownAttributeData(wellKnownAttributeData);
			}
			else
			{
				immutableArray = ImmutableArray<VisualBasicAttributeData>.Empty;
				wellKnownAttributeData = null;
				Interlocked.CompareExchange(ref lazyCustomAttributesBag, CustomAttributesBag<VisualBasicAttributeData>.WithEmptyData(), null);
			}
			PostDecodeWellKnownAttributes(immutableArray, attributesToBind, instance, symbolPart, wellKnownAttributeData);
			sourceModule.AtomicStoreAttributesAndDiagnostics(lazyCustomAttributesBag, immutableArray, instance);
			instance.Free();
		}

		private ImmutableArray<AttributeSyntax> GetAttributesToBind(OneOrMany<SyntaxList<AttributeListSyntax>> attributeDeclarationSyntaxLists, AttributeLocation symbolPart, VisualBasicCompilation compilation, out ImmutableArray<Binder> binders)
		{
			IAttributeTargetSymbol attributeTarget = (IAttributeTargetSymbol)this;
			SourceModuleSymbol sourceModule = (SourceModuleSymbol)compilation.SourceModule;
			ArrayBuilder<AttributeSyntax> arrayBuilder = null;
			ArrayBuilder<Binder> arrayBuilder2 = null;
			int num = 0;
			int num2 = attributeDeclarationSyntaxLists.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				SyntaxList<AttributeListSyntax> syntaxList = attributeDeclarationSyntaxLists[i];
				if (!syntaxList.Any())
				{
					continue;
				}
				int num3 = num;
				SyntaxList<AttributeListSyntax>.Enumerator enumerator = syntaxList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SeparatedSyntaxList<AttributeSyntax>.Enumerator enumerator2 = enumerator.Current.Attributes.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						AttributeSyntax current = enumerator2.Current;
						if (MatchAttributeTarget(attributeTarget, symbolPart, current.Target))
						{
							if (arrayBuilder == null)
							{
								arrayBuilder = new ArrayBuilder<AttributeSyntax>();
								arrayBuilder2 = new ArrayBuilder<Binder>();
							}
							arrayBuilder.Add(current);
							num++;
						}
					}
				}
				if (num != num3)
				{
					Binder attributeBinder = GetAttributeBinder(syntaxList, sourceModule);
					int num4 = num - num3 - 1;
					for (int j = 0; j <= num4; j++)
					{
						arrayBuilder2.Add(attributeBinder);
					}
				}
			}
			if (arrayBuilder != null)
			{
				binders = arrayBuilder2.ToImmutableAndFree();
				return arrayBuilder.ToImmutableAndFree();
			}
			binders = ImmutableArray<Binder>.Empty;
			return ImmutableArray<AttributeSyntax>.Empty;
		}

		internal Binder GetAttributeBinder(SyntaxList<AttributeListSyntax> syntaxList, SourceModuleSymbol sourceModule)
		{
			SyntaxTree syntaxTree = syntaxList.Node!.SyntaxTree;
			SyntaxNode parent = syntaxList.Node!.Parent;
			if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.AttributesStatement) && Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent.Parent, SyntaxKind.CompilationUnit))
			{
				return BinderBuilder.CreateBinderForProjectLevelNamespace(sourceModule, syntaxTree);
			}
			return BinderBuilder.CreateBinderForAttribute(sourceModule, syntaxTree, this);
		}

		private static bool MatchAttributeTarget(IAttributeTargetSymbol attributeTarget, AttributeLocation symbolPart, AttributeTargetSyntax targetOpt)
		{
			if (targetOpt == null)
			{
				return true;
			}
			AttributeLocation attributeLocation = VisualBasicExtensions.Kind(targetOpt.AttributeModifier) switch
			{
				SyntaxKind.AssemblyKeyword => AttributeLocation.Assembly, 
				SyntaxKind.ModuleKeyword => AttributeLocation.Module, 
				_ => throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(targetOpt.AttributeModifier)), 
			};
			if (symbolPart == AttributeLocation.None)
			{
				return attributeLocation == attributeTarget.DefaultAttributeLocation;
			}
			return attributeLocation == symbolPart;
		}

		private static ImmutableArray<AttributeSyntax> GetAttributesToBind(SyntaxList<AttributeListSyntax> attributeBlockSyntaxList)
		{
			ArrayBuilder<AttributeSyntax> attributeSyntaxBuilder = null;
			GetAttributesToBind(attributeBlockSyntaxList, ref attributeSyntaxBuilder);
			return attributeSyntaxBuilder?.ToImmutableAndFree() ?? ImmutableArray<AttributeSyntax>.Empty;
		}

		internal static void GetAttributesToBind(SyntaxList<AttributeListSyntax> attributeBlockSyntaxList, ref ArrayBuilder<AttributeSyntax> attributeSyntaxBuilder)
		{
			if (attributeBlockSyntaxList.Count > 0)
			{
				if (attributeSyntaxBuilder == null)
				{
					attributeSyntaxBuilder = ArrayBuilder<AttributeSyntax>.GetInstance();
				}
				SyntaxList<AttributeListSyntax>.Enumerator enumerator = attributeBlockSyntaxList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AttributeListSyntax current = enumerator.Current;
					attributeSyntaxBuilder.AddRange(current.Attributes);
				}
			}
		}

		private EarlyWellKnownAttributeData EarlyDecodeWellKnownAttributes(ImmutableArray<Binder> binders, ImmutableArray<NamedTypeSymbol> boundAttributeTypes, ImmutableArray<AttributeSyntax> attributesToBind, VisualBasicAttributeData[] attributeBuilder, AttributeLocation symbolPart)
		{
			EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments = default(EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation>);
			arguments.SymbolPart = symbolPart;
			int num = boundAttributeTypes.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				NamedTypeSymbol namedTypeSymbol = boundAttributeTypes[i];
				if (!TypeSymbolExtensions.IsErrorType(namedTypeSymbol))
				{
					arguments.Binder = new EarlyWellKnownAttributeBinder(this, binders[i]);
					arguments.AttributeType = namedTypeSymbol;
					arguments.AttributeSyntax = attributesToBind[i];
					attributeBuilder[i] = EarlyDecodeWellKnownAttribute(ref arguments);
				}
			}
			if (!arguments.HasDecodedData)
			{
				return null;
			}
			return arguments.DecodedData;
		}

		internal WellKnownAttributeData ValidateAttributeUsageAndDecodeWellKnownAttributes(ImmutableArray<Binder> binders, ImmutableArray<AttributeSyntax> attributeSyntaxList, ImmutableArray<VisualBasicAttributeData> boundAttributes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart)
		{
			int length = boundAttributes.Length;
			HashSet<NamedTypeSymbol> uniqueAttributeTypes = new HashSet<NamedTypeSymbol>();
			DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments = default(DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation>);
			arguments.AttributesCount = length;
			arguments.Diagnostics = diagnostics;
			arguments.SymbolPart = symbolPart;
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				VisualBasicAttributeData visualBasicAttributeData = boundAttributes[i];
				AttributeSyntax attributeSyntax = attributeSyntaxList[i];
				Binder binder = binders[i];
				if (!visualBasicAttributeData.HasErrors && ValidateAttributeUsage(visualBasicAttributeData, attributeSyntax, binder.Compilation, symbolPart, diagnostics, uniqueAttributeTypes))
				{
					arguments.Attribute = visualBasicAttributeData;
					arguments.AttributeSyntaxOpt = attributeSyntax;
					arguments.Index = i;
					DecodeWellKnownAttribute(ref arguments);
				}
			}
			if (!arguments.HasDecodedData)
			{
				return null;
			}
			return arguments.DecodedData;
		}

		private bool ValidateAttributeUsage(VisualBasicAttributeData attribute, AttributeSyntax node, VisualBasicCompilation compilation, AttributeLocation symbolPart, BindingDiagnosticBag diagnostics, HashSet<NamedTypeSymbol> uniqueAttributeTypes)
		{
			NamedTypeSymbol attributeClass = attribute.AttributeClass;
			AttributeUsageInfo attributeUsageInfo = attributeClass.GetAttributeUsageInfo();
			if (!uniqueAttributeTypes.Add(attributeClass) && !attributeUsageInfo.AllowMultiple)
			{
				diagnostics.Add(ERRID.ERR_InvalidMultipleAttributeUsage1, node.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass));
				return false;
			}
			AttributeTargets attributeTargets = ((symbolPart != AttributeLocation.Return) ? GetAttributeTarget() : AttributeTargets.ReturnValue);
			bool flag;
			if ((object)attributeClass == compilation.GetWellKnownType(WellKnownType.System_NonSerializedAttribute) && Kind == SymbolKind.Event && (object)((SourceEventSymbol)this).AssociatedField != null)
			{
				flag = true;
			}
			else
			{
				AttributeTargets validTargets = attributeUsageInfo.ValidTargets;
				flag = attributeTargets != 0 && (validTargets & attributeTargets) != 0;
			}
			if (!flag)
			{
				switch (attributeTargets)
				{
				case AttributeTargets.Assembly:
					diagnostics.Add(ERRID.ERR_InvalidAssemblyAttribute1, node.Name.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass));
					break;
				case AttributeTargets.Module:
					diagnostics.Add(ERRID.ERR_InvalidModuleAttribute1, node.Name.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass));
					break;
				case AttributeTargets.Method:
					if (Kind == SymbolKind.Method)
					{
						SourceMethodSymbol sourceMethodSymbol = (SourceMethodSymbol)this;
						string text2 = MethodKindExtensions.TryGetAccessorDisplayName(sourceMethodSymbol.MethodKind);
						if (text2 != null)
						{
							diagnostics.Add(ERRID.ERR_InvalidAttributeUsageOnAccessor, node.Name.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), text2, CustomSymbolDisplayFormatter.ShortErrorName(sourceMethodSymbol.AssociatedSymbol));
							break;
						}
					}
					diagnostics.Add(ERRID.ERR_InvalidAttributeUsage2, node.Name.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), CustomSymbolDisplayFormatter.ShortErrorName(this).ToString());
					break;
				case AttributeTargets.Field:
				{
					string text = ((!(this is SourceWithEventsBackingFieldSymbol sourceWithEventsBackingFieldSymbol)) ? CustomSymbolDisplayFormatter.ShortErrorName(this).ToString() : CustomSymbolDisplayFormatter.ShortErrorName(sourceWithEventsBackingFieldSymbol.AssociatedSymbol).ToString());
					diagnostics.Add(ERRID.ERR_InvalidAttributeUsage2, node.Name.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), text);
					break;
				}
				case AttributeTargets.ReturnValue:
					diagnostics.Add(ERRID.ERR_InvalidAttributeUsage2, node.Name.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), new LocalizableErrorArgument(ERRID.IDS_FunctionReturnType));
					break;
				default:
					diagnostics.Add(ERRID.ERR_InvalidAttributeUsage2, node.Name.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), CustomSymbolDisplayFormatter.ShortErrorName(this).ToString());
					break;
				}
				return false;
			}
			if (attribute.IsSecurityAttribute(compilation))
			{
				SymbolKind kind = Kind;
				if (kind != SymbolKind.Assembly && kind != SymbolKind.Method && kind != SymbolKind.NamedType)
				{
					diagnostics.Add(ERRID.ERR_SecurityAttributeInvalidTarget, node.Name.GetLocation(), CustomSymbolDisplayFormatter.ShortErrorName(attributeClass));
					return false;
				}
			}
			return true;
		}

		private void ReportExtensionAttributeUseSiteInfo(VisualBasicAttributeData attribute, AttributeSyntax nodeOpt, VisualBasicCompilation compilation, BindingDiagnosticBag diagnostics)
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = default(UseSiteInfo<AssemblySymbol>);
			if ((object)attribute.AttributeConstructor != null && (object)attribute.AttributeConstructor == compilation.GetExtensionAttributeConstructor(out useSiteInfo))
			{
				diagnostics.Add(useSiteInfo, (nodeOpt != null) ? nodeOpt.GetLocation() : NoLocation.Singleton);
			}
		}

		private void MarkEmbeddedAttributeTypeReference(VisualBasicAttributeData attribute, AttributeSyntax nodeOpt, VisualBasicCompilation compilation)
		{
			if (!IsEmbedded && attribute.AttributeClass.IsEmbedded && nodeOpt != null && compilation.ContainsSyntaxTree(nodeOpt.SyntaxTree))
			{
				compilation.EmbeddedSymbolManager.MarkSymbolAsReferenced(attribute.AttributeClass);
			}
		}

		internal void ForceCompleteObsoleteAttribute()
		{
			if (ObsoleteState == ThreeState.Unknown)
			{
				GetAttributes();
			}
		}
	}
}
