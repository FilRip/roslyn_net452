using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SourceFieldSymbol : FieldSymbol, IAttributeTargetSymbol
	{
		private struct DependencyInfo
		{
			public ImmutableHashSet<SourceFieldSymbol> Dependencies;

			public ImmutableHashSet<SourceFieldSymbol> DependedOnBy;
		}

		protected readonly SourceMemberFlags m_memberFlags;

		private readonly SourceMemberContainerTypeSymbol _containingType;

		private readonly string _name;

		private readonly SyntaxReference _syntaxRef;

		private string _lazyDocComment;

		private string _lazyExpandedDocComment;

		private CustomAttributesBag<VisualBasicAttributeData> _lazyCustomAttributesBag;

		private int _eventProduced;

		internal SyntaxTree SyntaxTree => _syntaxRef.SyntaxTree;

		internal VisualBasicSyntaxNode Syntax => VisualBasicExtensions.GetVisualBasicSyntax(_syntaxRef);

		internal abstract VisualBasicSyntaxNode DeclarationSyntax { get; }

		internal virtual VisualBasicSyntaxNode EqualsValueOrAsNewInitOpt => null;

		public sealed override string Name => _name;

		public sealed override Symbol ContainingSymbol => _containingType;

		public sealed override NamedTypeSymbol ContainingType => _containingType;

		public SourceMemberContainerTypeSymbol ContainingSourceType => _containingType;

		internal override bool HasDeclaredType => (m_memberFlags & SourceMemberFlags.InferredFieldType) == 0;

		public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override Symbol AssociatedSymbol => null;

		public override Accessibility DeclaredAccessibility => (Accessibility)(m_memberFlags & SourceMemberFlags.AccessibilityMask);

		public override bool IsReadOnly => (m_memberFlags & SourceMemberFlags.ReadOnly) != 0;

		public override bool IsConst => (m_memberFlags & SourceMemberFlags.Const) != 0;

		public override bool IsShared => (m_memberFlags & SourceMemberFlags.Shared) != 0;

		public override bool IsImplicitlyDeclared => _containingType.AreMembersImplicitlyDeclared;

		internal override bool ShadowsExplicitly => (m_memberFlags & SourceMemberFlags.Shadows) != 0;

		public override ImmutableArray<Location> Locations => ImmutableArray.Create(GetSymbolLocation(_syntaxRef));

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper(_syntaxRef);

		internal abstract OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations { get; }

		public AttributeLocation DefaultAttributeLocation => AttributeLocation.Field;

		internal sealed override bool HasSpecialName
		{
			get
			{
				if (HasRuntimeSpecialName)
				{
					return true;
				}
				return GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;
			}
		}

		internal sealed override bool HasRuntimeSpecialName => EmbeddedOperators.CompareString(Name, "value__", TextCompare: false) == 0;

		internal sealed override bool IsNotSerialized => GetDecodedWellKnownAttributeData()?.HasNonSerializedAttribute ?? false;

		internal sealed override MarshalPseudoCustomAttributeData MarshallingInformation => GetDecodedWellKnownAttributeData()?.MarshallingInformation;

		internal sealed override int? TypeLayoutOffset => GetDecodedWellKnownAttributeData()?.Offset;

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
					return ((CommonFieldEarlyWellKnownAttributeData)_lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
				}
				return ObsoleteAttributeData.Uninitialized;
			}
		}

		protected SourceFieldSymbol(SourceMemberContainerTypeSymbol container, SyntaxReference syntaxRef, string name, SourceMemberFlags memberFlags)
		{
			_name = name;
			_containingType = container;
			_syntaxRef = syntaxRef;
			m_memberFlags = memberFlags;
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			base.GenerateDeclarationErrors(cancellationToken);
			_ = Type;
			GetConstantValue(ConstantFieldsInProgress.Empty);
			SourceModuleSymbol sourceModuleSymbol = (SourceModuleSymbol)ContainingModule;
			if (Interlocked.CompareExchange(ref _eventProduced, 1, 0) == 0 && !IsImplicitlyDeclared)
			{
				sourceModuleSymbol.DeclaringCompilation.SymbolDeclaredEvent(this);
			}
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (expandIncludes)
			{
				return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, preferredCulture, expandIncludes, ref _lazyExpandedDocComment, cancellationToken);
			}
			return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, preferredCulture, expandIncludes, ref _lazyDocComment, cancellationToken);
		}

		internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress)
		{
			return null;
		}

		protected ConstantValue GetConstantValueImpl(ConstantFieldsInProgress inProgress)
		{
			EvaluatedConstant lazyConstantTuple = GetLazyConstantTuple();
			if (lazyConstantTuple != null)
			{
				return lazyConstantTuple.Value;
			}
			if (!inProgress.IsEmpty)
			{
				inProgress.AddDependency(this);
				return Microsoft.CodeAnalysis.ConstantValue.Bad;
			}
			ArrayBuilder<ConstantValueUtils.FieldInfo> instance = ArrayBuilder<ConstantValueUtils.FieldInfo>.GetInstance();
			OrderAllDependencies(instance);
			ArrayBuilder<ConstantValueUtils.FieldInfo>.Enumerator enumerator = instance.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ConstantValueUtils.FieldInfo current = enumerator.Current;
				current.Field.BindConstantTupleIfNecessary(current.StartsCycle);
			}
			instance.Free();
			return GetLazyConstantTuple().Value;
		}

		private void BindConstantTupleIfNecessary(bool startsCycle)
		{
			if (GetLazyConstantTuple() == null)
			{
				PooledHashSet<SourceFieldSymbol> instance = PooledHashSet<SourceFieldSymbol>.GetInstance();
				ConstantFieldsInProgress.Dependencies dependencies = new ConstantFieldsInProgress.Dependencies(instance);
				BindingDiagnosticBag instance2 = BindingDiagnosticBag.GetInstance();
				EvaluatedConstant constantTuple = MakeConstantTuple(dependencies, instance2);
				if (startsCycle)
				{
					instance2.Clear();
					instance2.Add(ERRID.ERR_CircularEvaluation1, Locations[0], CustomSymbolDisplayFormatter.ShortErrorName(this));
				}
				SetLazyConstantTuple(constantTuple, instance2);
				instance2.Free();
				instance.Free();
			}
		}

		private void OrderAllDependencies(ArrayBuilder<ConstantValueUtils.FieldInfo> order)
		{
			PooledDictionary<SourceFieldSymbol, DependencyInfo> instance = PooledDictionary<SourceFieldSymbol, DependencyInfo>.GetInstance();
			CreateGraph(instance);
			OrderGraph(instance, order);
			instance.Free();
		}

		private void CreateGraph(Dictionary<SourceFieldSymbol, DependencyInfo> graph)
		{
			ArrayBuilder<SourceFieldSymbol> instance = ArrayBuilder<SourceFieldSymbol>.GetInstance();
			instance.Push(this);
			while (instance.Count > 0)
			{
				SourceFieldSymbol sourceFieldSymbol = instance.Pop();
				DependencyInfo value = default(DependencyInfo);
				if (graph.TryGetValue(sourceFieldSymbol, out value))
				{
					if (value.Dependencies != null)
					{
						continue;
					}
				}
				else
				{
					value = default(DependencyInfo);
					value.DependedOnBy = ImmutableHashSet<SourceFieldSymbol>.Empty;
				}
				ImmutableHashSet<SourceFieldSymbol> immutableHashSet = (value.Dependencies = sourceFieldSymbol.GetConstantValueDependencies());
				graph[sourceFieldSymbol] = value;
				foreach (SourceFieldSymbol item in immutableHashSet)
				{
					instance.Push(item);
					if (!graph.TryGetValue(item, out value))
					{
						value = default(DependencyInfo);
						value.DependedOnBy = ImmutableHashSet<SourceFieldSymbol>.Empty;
					}
					value.DependedOnBy = value.DependedOnBy.Add(sourceFieldSymbol);
					graph[item] = value;
				}
			}
			instance.Free();
		}

		private ImmutableHashSet<SourceFieldSymbol> GetConstantValueDependencies()
		{
			EvaluatedConstant lazyConstantTuple = GetLazyConstantTuple();
			if (lazyConstantTuple != null)
			{
				return ImmutableHashSet<SourceFieldSymbol>.Empty;
			}
			PooledHashSet<SourceFieldSymbol> instance = PooledHashSet<SourceFieldSymbol>.GetInstance();
			ConstantFieldsInProgress.Dependencies dependencies = new ConstantFieldsInProgress.Dependencies(instance);
			BindingDiagnosticBag instance2 = BindingDiagnosticBag.GetInstance();
			lazyConstantTuple = MakeConstantTuple(dependencies, instance2);
			ImmutableHashSet<SourceFieldSymbol> result;
			if (instance.Count == 0 && !lazyConstantTuple.Value.IsBad && !instance2.HasAnyResolvedErrors())
			{
				SetLazyConstantTuple(lazyConstantTuple, instance2);
				result = ImmutableHashSet<SourceFieldSymbol>.Empty;
			}
			else
			{
				result = ImmutableHashSet<SourceFieldSymbol>.Empty.Union(instance);
			}
			instance2.Free();
			instance.Free();
			return result;
		}

		[Conditional("DEBUG")]
		private static void CheckGraph(Dictionary<SourceFieldSymbol, DependencyInfo> graph)
		{
			int num = 10;
			foreach (KeyValuePair<SourceFieldSymbol, DependencyInfo> item in graph)
			{
				_ = item.Key;
				DependencyInfo value = item.Value;
				foreach (SourceFieldSymbol dependency in value.Dependencies)
				{
					DependencyInfo value2 = default(DependencyInfo);
					graph.TryGetValue(dependency, out value2);
				}
				foreach (SourceFieldSymbol item2 in value.DependedOnBy)
				{
					DependencyInfo value3 = default(DependencyInfo);
					graph.TryGetValue(item2, out value3);
				}
				num--;
				if (num == 0)
				{
					break;
				}
			}
		}

		private static void OrderGraph(Dictionary<SourceFieldSymbol, DependencyInfo> graph, ArrayBuilder<ConstantValueUtils.FieldInfo> order)
		{
			PooledHashSet<SourceFieldSymbol> pooledHashSet = null;
			ArrayBuilder<SourceFieldSymbol> fieldsInvolvedInCycles = null;
			while (graph.Count > 0)
			{
				IEnumerable<SourceFieldSymbol> enumerable = (IEnumerable<SourceFieldSymbol>)(((object)pooledHashSet) ?? ((object)graph.Keys));
				ArrayBuilder<SourceFieldSymbol> instance = ArrayBuilder<SourceFieldSymbol>.GetInstance();
				foreach (SourceFieldSymbol item in enumerable)
				{
					DependencyInfo value = default(DependencyInfo);
					if (graph.TryGetValue(item, out value) && value.Dependencies.Count == 0)
					{
						instance.Add(item);
					}
				}
				pooledHashSet?.Free();
				pooledHashSet = null;
				if (instance.Count > 0)
				{
					PooledHashSet<SourceFieldSymbol> instance2 = PooledHashSet<SourceFieldSymbol>.GetInstance();
					ArrayBuilder<SourceFieldSymbol>.Enumerator enumerator2 = instance.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						SourceFieldSymbol current2 = enumerator2.Current;
						foreach (SourceFieldSymbol item2 in graph[current2].DependedOnBy)
						{
							DependencyInfo value2 = graph[item2];
							value2.Dependencies = value2.Dependencies.Remove(current2);
							graph[item2] = value2;
							instance2.Add(item2);
						}
						graph.Remove(current2);
					}
					ArrayBuilder<SourceFieldSymbol>.Enumerator enumerator4 = instance.GetEnumerator();
					while (enumerator4.MoveNext())
					{
						SourceFieldSymbol current4 = enumerator4.Current;
						order.Add(new ConstantValueUtils.FieldInfo(current4, startsCycle: false));
					}
					pooledHashSet = instance2;
				}
				else
				{
					SourceFieldSymbol startOfFirstCycle = GetStartOfFirstCycle(graph, ref fieldsInvolvedInCycles);
					foreach (SourceFieldSymbol dependency in graph[startOfFirstCycle].Dependencies)
					{
						DependencyInfo value3 = graph[dependency];
						value3.DependedOnBy = value3.DependedOnBy.Remove(startOfFirstCycle);
						graph[dependency] = value3;
					}
					DependencyInfo dependencyInfo = graph[startOfFirstCycle];
					PooledHashSet<SourceFieldSymbol> instance3 = PooledHashSet<SourceFieldSymbol>.GetInstance();
					foreach (SourceFieldSymbol item3 in dependencyInfo.DependedOnBy)
					{
						DependencyInfo value4 = graph[item3];
						value4.Dependencies = value4.Dependencies.Remove(startOfFirstCycle);
						graph[item3] = value4;
						instance3.Add(item3);
					}
					graph.Remove(startOfFirstCycle);
					order.Add(new ConstantValueUtils.FieldInfo(startOfFirstCycle, startsCycle: true));
					pooledHashSet = instance3;
				}
				instance.Free();
			}
			pooledHashSet?.Free();
			fieldsInvolvedInCycles?.Free();
		}

		private static SourceFieldSymbol GetStartOfFirstCycle(Dictionary<SourceFieldSymbol, DependencyInfo> graph, ref ArrayBuilder<SourceFieldSymbol> fieldsInvolvedInCycles)
		{
			if (fieldsInvolvedInCycles == null)
			{
				fieldsInvolvedInCycles = ArrayBuilder<SourceFieldSymbol>.GetInstance(graph.Count);
				fieldsInvolvedInCycles.AddRange((from f in graph.Keys
					group f by f.DeclaringCompilation).SelectMany(delegate(IGrouping<VisualBasicCompilation, SourceFieldSymbol> g)
				{
					_Closure_0024__48_002D0 arg = default(_Closure_0024__48_002D0);
					_Closure_0024__48_002D0 CS_0024_003C_003E8__locals0 = new _Closure_0024__48_002D0(arg);
					CS_0024_003C_003E8__locals0._0024VB_0024Local_g = g;
					return CS_0024_003C_003E8__locals0._0024VB_0024Local_g.OrderByDescending((SourceFieldSymbol f1, SourceFieldSymbol f2) => CS_0024_003C_003E8__locals0._0024VB_0024Local_g.Key.CompareSourceLocations(f1.Locations[0], f2.Locations[0]));
				}));
			}
			SourceFieldSymbol sourceFieldSymbol;
			do
			{
				sourceFieldSymbol = fieldsInvolvedInCycles.Pop();
			}
			while (!graph.ContainsKey(sourceFieldSymbol) || !IsPartOfCycle(graph, sourceFieldSymbol));
			return sourceFieldSymbol;
		}

		private static bool IsPartOfCycle(Dictionary<SourceFieldSymbol, DependencyInfo> graph, SourceFieldSymbol field)
		{
			PooledHashSet<SourceFieldSymbol> instance = PooledHashSet<SourceFieldSymbol>.GetInstance();
			ArrayBuilder<SourceFieldSymbol> instance2 = ArrayBuilder<SourceFieldSymbol>.GetInstance();
			SourceFieldSymbol item = field;
			bool result = false;
			instance2.Push(field);
			while (instance2.Count > 0)
			{
				field = instance2.Pop();
				DependencyInfo dependencyInfo = graph[field];
				if (dependencyInfo.Dependencies.Contains(item))
				{
					result = true;
					break;
				}
				foreach (SourceFieldSymbol dependency in dependencyInfo.Dependencies)
				{
					if (instance.Add(dependency))
					{
						instance2.Push(dependency);
					}
				}
			}
			instance2.Free();
			instance.Free();
			return result;
		}

		protected virtual EvaluatedConstant GetLazyConstantTuple()
		{
			throw ExceptionUtilities.Unreachable;
		}

		protected virtual void SetLazyConstantTuple(EvaluatedConstant constantTuple, BindingDiagnosticBag diagnostics)
		{
			throw ExceptionUtilities.Unreachable;
		}

		protected virtual EvaluatedConstant MakeConstantTuple(ConstantFieldsInProgress.Dependencies dependencies, BindingDiagnosticBag diagnostics)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return new LexicalSortKey(_syntaxRef, DeclaringCompilation);
		}

		public sealed override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return GetAttributesBag().Attributes;
		}

		private CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag()
		{
			if (_lazyCustomAttributesBag == null || !_lazyCustomAttributesBag.IsSealed)
			{
				LoadAndValidateAttributes(GetAttributeDeclarations, ref _lazyCustomAttributesBag);
			}
			return _lazyCustomAttributesBag;
		}

		private CommonFieldWellKnownAttributeData GetDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = _lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (CommonFieldWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		internal void SetCustomAttributeData(CustomAttributesBag<VisualBasicAttributeData> attributeData)
		{
			_lazyCustomAttributesBag = attributeData;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (IsConst && (object)GetConstantValue(ConstantFieldsInProgress.Empty) != null)
			{
				CommonFieldWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
				if (decodedWellKnownAttributeData == null || decodedWellKnownAttributeData.ConstValue == Microsoft.CodeAnalysis.ConstantValue.Unset)
				{
					if (Type.SpecialType == SpecialType.System_DateTime)
					{
						DateTime dateTime = (DateTime)ConstantValue;
						NamedTypeSymbol specialType = ContainingAssembly.GetSpecialType(SpecialType.System_Int64);
						VisualBasicCompilation declaringCompilation = DeclaringCompilation;
						Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DateTimeConstantAttribute__ctor, ImmutableArray.Create(new TypedConstant(specialType, TypedConstantKind.Primitive, dateTime.Ticks))));
					}
					else if (Type.SpecialType == SpecialType.System_Decimal)
					{
						decimal value = (decimal)ConstantValue;
						VisualBasicCompilation declaringCompilation2 = DeclaringCompilation;
						Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation2.SynthesizeDecimalConstantAttribute(value));
					}
				}
			}
			if (TypeSymbolExtensions.ContainsTupleNames(Type))
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeTupleNamesAttribute(Type));
			}
		}

		internal sealed override VisualBasicAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
		{
			VisualBasicAttributeData boundAttribute = null;
			ObsoleteAttributeData obsoleteData = null;
			if (EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out boundAttribute, out obsoleteData))
			{
				if (obsoleteData != null)
				{
					arguments.GetOrCreateData<CommonFieldEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
				}
				return boundAttribute;
			}
			return base.EarlyDecodeWellKnownAttribute(ref arguments);
		}

		internal sealed override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
			if (attribute.IsTargetAttribute(this, AttributeDescription.TupleElementNamesAttribute))
			{
				bindingDiagnosticBag.Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt!.Location);
			}
			if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
			{
				arguments.GetOrCreateData<CommonFieldWellKnownAttributeData>().HasSpecialNameAttribute = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.NonSerializedAttribute))
			{
				if (ContainingType.IsSerializable)
				{
					arguments.GetOrCreateData<CommonFieldWellKnownAttributeData>().HasNonSerializedAttribute = true;
				}
				else
				{
					bindingDiagnosticBag.Add(ERRID.ERR_InvalidNonSerializedUsage, arguments.AttributeSyntaxOpt!.GetLocation());
				}
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.FieldOffsetAttribute))
			{
				int num = attribute.CommonConstructorArguments[0].DecodeValue<int>(SpecialType.System_Int32);
				if (num < 0)
				{
					bindingDiagnosticBag.Add(ERRID.ERR_BadAttribute1, arguments.AttributeSyntaxOpt!.ArgumentList.Arguments[0].GetLocation(), attribute.AttributeClass);
					num = 0;
				}
				arguments.GetOrCreateData<CommonFieldWellKnownAttributeData>().SetFieldOffset(num);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.MarshalAsAttribute))
			{
				MarshalAsAttributeDecoder<CommonFieldWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation>.Decode(ref arguments, AttributeTargets.Field, MessageProvider.Instance);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.DateTimeConstantAttribute))
			{
				VerifyConstantValueMatches(attribute.DecodeDateTimeConstantValue(), ref arguments);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.DecimalConstantAttribute))
			{
				VerifyConstantValueMatches(attribute.DecodeDecimalConstantValue(), ref arguments);
			}
			else
			{
				base.DecodeWellKnownAttribute(ref arguments);
			}
		}

		private void VerifyConstantValueMatches(ConstantValue attrValue, ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			CommonFieldWellKnownAttributeData orCreateData = arguments.GetOrCreateData<CommonFieldWellKnownAttributeData>();
			BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
			ConstantValue constantValue;
			if (IsConst)
			{
				if (TypeSymbolExtensions.IsDecimalType(Type) || TypeSymbolExtensions.IsDateTimeType(Type))
				{
					constantValue = GetConstantValue(ConstantFieldsInProgress.Empty);
					if ((object)constantValue != null && !constantValue.IsBad && constantValue != attrValue)
					{
						bindingDiagnosticBag.Add(ERRID.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt!.GetLocation());
					}
				}
				else
				{
					bindingDiagnosticBag.Add(ERRID.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt!.GetLocation());
				}
				if (orCreateData.ConstValue == Microsoft.CodeAnalysis.ConstantValue.Unset)
				{
					orCreateData.ConstValue = attrValue;
				}
				return;
			}
			constantValue = orCreateData.ConstValue;
			if (constantValue != Microsoft.CodeAnalysis.ConstantValue.Unset)
			{
				if (constantValue != attrValue)
				{
					bindingDiagnosticBag.Add(ERRID.ERR_FieldHasMultipleDistinctConstantValues, arguments.AttributeSyntaxOpt!.GetLocation());
				}
			}
			else
			{
				orCreateData.ConstValue = attrValue;
			}
		}

		private static Location GetSymbolLocation(SyntaxReference syntaxRef)
		{
			SyntaxNode syntax = syntaxRef.GetSyntax();
			return syntaxRef.SyntaxTree.GetLocation(GetFieldLocationFromSyntax(((ModifiedIdentifierSyntax)syntax).Identifier));
		}

		private static TextSpan GetFieldLocationFromSyntax(SyntaxToken node)
		{
			return node.Span;
		}

		internal static Symbol FindFieldOrWithEventsSymbolFromSyntax(SyntaxToken variableName, SyntaxTree tree, NamedTypeSymbol container)
		{
			string valueText = variableName.ValueText;
			TextSpan fieldLocationFromSyntax = GetFieldLocationFromSyntax(variableName);
			return NamedTypeSymbolExtensions.FindFieldOrProperty(container, valueText, fieldLocationFromSyntax, tree);
		}
	}
}
