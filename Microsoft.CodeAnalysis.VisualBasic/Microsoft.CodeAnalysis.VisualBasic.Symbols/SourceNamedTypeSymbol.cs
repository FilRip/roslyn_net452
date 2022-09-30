using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceNamedTypeSymbol : SourceMemberContainerTypeSymbol, IAttributeTargetSymbol
	{
		private struct TypeParameterInfo
		{
			public readonly VarianceKind Variance;

			public readonly ImmutableArray<TypeParameterConstraint> Constraints;

			public bool Initialized => !Constraints.IsDefault;

			public TypeParameterInfo(VarianceKind variance, ImmutableArray<TypeParameterConstraint> constraints)
			{
				this = default(TypeParameterInfo);
				Variance = variance;
				Constraints = constraints;
			}
		}

		private class ComClassData
		{
			private enum ReservedDispId
			{
				None = -1,
				DISPID_VALUE = 0,
				DISPID_NEWENUM = -4
			}

			private sealed class SynthesizedComInterface : NamedTypeSymbol
			{
				private readonly SourceNamedTypeSymbol _comClass;

				private readonly bool _isEventInterface;

				private readonly ImmutableArray<Symbol> _members;

				private readonly string _defaultMemberName;

				public bool IsEventInterface => _isEventInterface;

				public SourceNamedTypeSymbol ComClass => _comClass;

				public override int Arity => 0;

				internal override bool CanConstruct => false;

				public override NamedTypeSymbol ConstructedFrom => this;

				public override Symbol ContainingSymbol => _comClass;

				public override Accessibility DeclaredAccessibility => Accessibility.Public;

				public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				internal override string DefaultPropertyName
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				internal override ObsoleteAttributeData ObsoleteAttributeData => null;

				internal override bool HasCodeAnalysisEmbeddedAttribute
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				internal override bool HasVisualBasicEmbeddedAttribute
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => false;

				internal override bool IsWindowsRuntimeImport => false;

				internal override bool ShouldAddWinRTMembers => false;

				internal override bool IsComImport => false;

				internal override TypeSymbol CoClassType => null;

				internal override bool HasDeclarativeSecurity => false;

				public override bool IsMustInherit => false;

				public override bool IsNotInheritable => false;

				public override ImmutableArray<Location> Locations
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				internal override bool MangleName => false;

				public override IEnumerable<string> MemberNames
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override bool MightContainExtensionMethods
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override string Name => (_isEventInterface ? "__" : "_") + _comClass.Name;

				internal override bool HasSpecialName => false;

				public override bool IsSerializable => false;

				internal override TypeLayout Layout => default(TypeLayout);

				internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

				internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => ImmutableArray<TypeSymbol>.Empty;

				internal override bool HasTypeArgumentsCustomModifiers => false;

				public override TypeKind TypeKind => TypeKind.Interface;

				internal override bool IsInterface => true;

				public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

				internal override TypeSubstitution TypeSubstitution => null;

				public SynthesizedComInterface(SourceNamedTypeSymbol comClass, ArrayBuilder<KeyValuePair<Symbol, int>> interfaceMembers)
				{
					_comClass = comClass;
					_isEventInterface = false;
					HashSet<int> hashSet = new HashSet<int>();
					ArrayBuilder<KeyValuePair<Symbol, int>>.Enumerator enumerator = interfaceMembers.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<Symbol, int> current = enumerator.Current;
						if (current.Value != -1)
						{
							hashSet.Add(current.Value);
						}
					}
					ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
					int nextDispId = 1;
					int num = interfaceMembers.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						KeyValuePair<Symbol, int> keyValuePair = interfaceMembers[i];
						Symbol key = keyValuePair.Key;
						int value = keyValuePair.Value;
						switch (key.Kind)
						{
						case SymbolKind.Method:
						{
							MethodSymbol methodSymbol = (MethodSymbol)key;
							int num2;
							if (value == -1)
							{
								num2 = GetNextAvailableDispId(hashSet, ref nextDispId);
								if (CaseInsensitiveComparison.Equals(methodSymbol.Name, "GetEnumerator") && methodSymbol.ParameterCount == 0 && methodSymbol.ReturnType.SpecialType == SpecialType.System_Collections_IEnumerator)
								{
									num2 = -4;
								}
							}
							else
							{
								num2 = -1;
							}
							instance.Add(new SynthesizedComMethod(this, methodSymbol, num2));
							break;
						}
						case SymbolKind.Property:
						{
							PropertySymbol propertySymbol = (PropertySymbol)key;
							SynthesizedComMethod synthesizedComMethod = null;
							SynthesizedComMethod synthesizedComMethod2 = null;
							if (_defaultMemberName == null && propertySymbol.IsDefault)
							{
								_defaultMemberName = propertySymbol.Name;
							}
							i++;
							KeyValuePair<Symbol, int> keyValuePair2 = interfaceMembers[i];
							i++;
							KeyValuePair<Symbol, int> keyValuePair3 = interfaceMembers[i];
							int num2;
							if (value == -1 || ((object)keyValuePair2.Key != null && keyValuePair2.Value == -1) || ((object)keyValuePair3.Key != null && keyValuePair3.Value == -1))
							{
								num2 = GetNextAvailableDispId(hashSet, ref nextDispId);
								if (CaseInsensitiveComparison.Equals(propertySymbol.Name, "GetEnumerator") && propertySymbol.ParameterCount == 0 && propertySymbol.Type.SpecialType == SpecialType.System_Collections_IEnumerator)
								{
									num2 = -4;
								}
								else if (propertySymbol.IsDefault)
								{
									num2 = 0;
								}
								else if (value != -1)
								{
									num2 = value;
								}
							}
							else
							{
								num2 = -1;
							}
							if ((object)keyValuePair2.Key != null)
							{
								synthesizedComMethod = new SynthesizedComMethod(this, (MethodSymbol)keyValuePair2.Key, (keyValuePair2.Value == -1) ? num2 : (-1));
							}
							if ((object)keyValuePair3.Key != null)
							{
								synthesizedComMethod2 = new SynthesizedComMethod(this, (MethodSymbol)keyValuePair3.Key, (keyValuePair3.Value == -1) ? num2 : (-1));
							}
							if ((object)synthesizedComMethod != null)
							{
								if ((object)synthesizedComMethod2 != null)
								{
									if (LexicalOrderSymbolComparer.Instance.Compare(propertySymbol.GetMethod, propertySymbol.SetMethod) <= 0)
									{
										instance.Add(synthesizedComMethod);
										instance.Add(synthesizedComMethod2);
									}
									else
									{
										instance.Add(synthesizedComMethod2);
										instance.Add(synthesizedComMethod);
									}
								}
								else
								{
									instance.Add(synthesizedComMethod);
								}
							}
							else
							{
								instance.Add(synthesizedComMethod2);
							}
							instance.Add(new SynthesizedComProperty(this, propertySymbol, synthesizedComMethod, synthesizedComMethod2, (value == -1) ? num2 : (-1)));
							break;
						}
						default:
							throw ExceptionUtilities.UnexpectedValue(key.Kind);
						}
					}
					_members = instance.ToImmutableAndFree();
				}

				public override int GetHashCode()
				{
					return RuntimeHelpers.GetHashCode(this);
				}

				public override bool Equals(TypeSymbol other, TypeCompareKind comparison)
				{
					return (object)this == other;
				}

				private static int GetNextAvailableDispId(HashSet<int> usedDispIds, [In][Out] ref int nextDispId)
				{
					int i;
					for (i = nextDispId; usedDispIds.Contains(i); i++)
					{
					}
					nextDispId = i + 1;
					return i;
				}

				public SynthesizedComInterface(SourceNamedTypeSymbol comClass, ArrayBuilder<KeyValuePair<EventSymbol, int>> interfaceMembers)
				{
					_comClass = comClass;
					_isEventInterface = true;
					HashSet<int> hashSet = new HashSet<int>();
					ArrayBuilder<KeyValuePair<EventSymbol, int>>.Enumerator enumerator = interfaceMembers.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<EventSymbol, int> current = enumerator.Current;
						if (current.Value != -1)
						{
							hashSet.Add(current.Value);
						}
					}
					ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
					int nextDispId = 1;
					ArrayBuilder<KeyValuePair<EventSymbol, int>>.Enumerator enumerator2 = interfaceMembers.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						KeyValuePair<EventSymbol, int> current2 = enumerator2.Current;
						EventSymbol key = current2.Key;
						if (TypeSymbolExtensions.IsDelegateType(key.Type))
						{
							MethodSymbol delegateInvokeMethod = ((NamedTypeSymbol)key.Type).DelegateInvokeMethod;
							if ((object)delegateInvokeMethod != null)
							{
								int synthesizedDispId = ((current2.Value != -1) ? (-1) : GetNextAvailableDispId(hashSet, ref nextDispId));
								instance.Add(new SynthesizedComEventMethod(this, key, delegateInvokeMethod, synthesizedDispId));
							}
						}
					}
					_members = instance.ToImmutableAndFree();
				}

				public override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
				{
					throw ExceptionUtilities.Unreachable;
				}

				internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
				{
					throw ExceptionUtilities.Unreachable;
				}

				public override ImmutableArray<Symbol> GetMembers()
				{
					return _members;
				}

				public override ImmutableArray<Symbol> GetMembers(string name)
				{
					throw ExceptionUtilities.Unreachable;
				}

				public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
				{
					return ImmutableArray<NamedTypeSymbol>.Empty;
				}

				public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
				{
					throw ExceptionUtilities.Unreachable;
				}

				public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
				{
					throw ExceptionUtilities.Unreachable;
				}

				internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
				{
					return SpecializedCollections.EmptyEnumerable<FieldSymbol>();
				}

				internal override ImmutableArray<string> GetAppliedConditionalSymbols()
				{
					return ImmutableArray<string>.Empty;
				}

				internal override AttributeUsageInfo GetAttributeUsageInfo()
				{
					throw ExceptionUtilities.Unreachable;
				}

				internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
				{
					throw ExceptionUtilities.Unreachable;
				}

				internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
				{
					throw ExceptionUtilities.Unreachable;
				}

				internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
				{
					return null;
				}

				internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
				{
					return ImmutableArray<NamedTypeSymbol>.Empty;
				}

				internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
				{
					return null;
				}

				internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
				{
					return ImmutableArray<NamedTypeSymbol>.Empty;
				}

				public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
				{
					return GetEmptyTypeArgumentCustomModifiers(ordinal);
				}

				public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
				{
					return ImmutableArray<VisualBasicAttributeData>.Empty;
				}

				internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
				{
					base.AddSynthesizedAttributes(compilationState, ref attributes);
					VisualBasicCompilation declaringCompilation = _comClass.DeclaringCompilation;
					string text = (_isEventInterface ? _comClass._comClassData.EventId : _comClass._comClassData.InterfaceId);
					if (text != null)
					{
						Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_GuidAttribute__ctor, ImmutableArray.Create(new TypedConstant(_comClass.GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, text))));
					}
					if (_isEventInterface)
					{
						Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_InterfaceTypeAttribute__ctorInt16, ImmutableArray.Create(new TypedConstant(_comClass.GetSpecialType(SpecialType.System_Int16), TypedConstantKind.Primitive, (short)2))));
					}
					Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_ComVisibleAttribute__ctor, ImmutableArray.Create(new TypedConstant(_comClass.GetSpecialType(SpecialType.System_Boolean), TypedConstantKind.Primitive, true))));
					if (_defaultMemberName != null)
					{
						Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, ImmutableArray.Create(new TypedConstant(_comClass.GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, _defaultMemberName))));
					}
				}

				internal override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
				{
					return null;
				}

				internal override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
				{
					return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
				}
			}

			private class SynthesizedComMethod : MethodSymbol
			{
				public readonly MethodSymbol ClonedFrom;

				private readonly int _synthesizedDispId;

				private readonly SynthesizedComInterface _interface;

				private readonly ImmutableArray<ParameterSymbol> _parameters;

				protected virtual Symbol NameAndAttributesSource => ClonedFrom;

				public override string Name => NameAndAttributesSource.Name;

				internal override bool HasSpecialName => ClonedFrom.HasSpecialName;

				public override int Arity => 0;

				public override Symbol AssociatedSymbol => null;

				internal override Microsoft.Cci.CallingConvention CallingConvention => ClonedFrom.CallingConvention;

				public override Symbol ContainingSymbol => _interface;

				public override NamedTypeSymbol ContainingType => _interface;

				public override Accessibility DeclaredAccessibility => Accessibility.Public;

				public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override bool IsExtensionMethod
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override bool IsExternalMethod
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override bool IsMustOverride => true;

				public override bool IsNotOverridable => false;

				public override bool IsOverloads => false;

				public override bool IsOverridable => false;

				public override bool IsOverrides => false;

				public override bool IsShared => false;

				public override bool IsSub => ClonedFrom.IsSub;

				public override bool IsAsync => false;

				public override bool IsIterator => false;

				public sealed override bool IsInitOnly => false;

				public override bool IsVararg => ClonedFrom.IsVararg;

				public override ImmutableArray<Location> Locations
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override MethodKind MethodKind => ClonedFrom.MethodKind switch
				{
					MethodKind.PropertyGet => MethodKind.PropertyGet, 
					MethodKind.PropertySet => MethodKind.PropertySet, 
					_ => MethodKind.Ordinary, 
				};

				internal sealed override bool IsMethodKindBasedOnSyntax => ClonedFrom.IsMethodKindBasedOnSyntax;

				internal sealed override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => ClonedFrom.ReturnTypeMarshallingInformation;

				internal sealed override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

				internal sealed override bool HasDeclarativeSecurity => false;

				internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

				public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

				public override bool ReturnsByRef => ClonedFrom.ReturnsByRef;

				public override TypeSymbol ReturnType => ClonedFrom.ReturnType;

				public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => ClonedFrom.ReturnTypeCustomModifiers;

				public override ImmutableArray<CustomModifier> RefCustomModifiers => ClonedFrom.RefCustomModifiers;

				internal override SyntaxNode Syntax
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override ImmutableArray<TypeSymbol> TypeArguments => ImmutableArray<TypeSymbol>.Empty;

				public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

				internal override bool GenerateDebugInfoImpl => false;

				public SynthesizedComMethod(SynthesizedComInterface container, MethodSymbol clone, int synthesizedDispId)
				{
					_interface = container;
					_synthesizedDispId = synthesizedDispId;
					ClonedFrom = clone;
					if (clone.ParameterCount == 0)
					{
						_parameters = ImmutableArray<ParameterSymbol>.Empty;
						return;
					}
					ParameterSymbol[] array = new ParameterSymbol[clone.ParameterCount - 1 + 1];
					int num = array.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = new SynthesizedComParameter(this, clone.Parameters[i]);
					}
					_parameters = array.AsImmutable();
				}

				internal override ImmutableArray<string> GetAppliedConditionalSymbols()
				{
					return ImmutableArray<string>.Empty;
				}

				public sealed override DllImportData GetDllImportData()
				{
					return null;
				}

				internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
				{
					throw ExceptionUtilities.Unreachable;
				}

				public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
				{
					Symbol nameAndAttributesSource = NameAndAttributesSource;
					ImmutableArray<VisualBasicAttributeData> attributes = nameAndAttributesSource.GetAttributes();
					if (nameAndAttributesSource.Kind == SymbolKind.Method)
					{
						return attributes;
					}
					ArrayBuilder<VisualBasicAttributeData> instance = ArrayBuilder<VisualBasicAttributeData>.GetInstance();
					ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = attributes.GetEnumerator();
					while (enumerator.MoveNext())
					{
						VisualBasicAttributeData current = enumerator.Current;
						if ((current.AttributeClass.GetAttributeUsageInfo().ValidTargets & AttributeTargets.Method) != 0)
						{
							instance.Add(current);
						}
					}
					if (instance.Count == attributes.Length)
					{
						instance.Free();
						return attributes;
					}
					return instance.ToImmutableAndFree();
				}

				internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
				{
					base.AddSynthesizedAttributes(compilationState, ref attributes);
					if (_synthesizedDispId != -1)
					{
						Symbol.AddSynthesizedAttribute(ref attributes, _interface.ComClass.DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_DispIdAttribute__ctor, ImmutableArray.Create(new TypedConstant(_interface.ComClass.GetSpecialType(SpecialType.System_Int32), TypedConstantKind.Primitive, _synthesizedDispId))));
					}
				}

				public override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
				{
					Symbol nameAndAttributesSource = NameAndAttributesSource;
					if (nameAndAttributesSource.Kind == SymbolKind.Method)
					{
						return ((MethodSymbol)nameAndAttributesSource).GetReturnTypeAttributes();
					}
					return ImmutableArray<VisualBasicAttributeData>.Empty;
				}

				internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
				{
					return true;
				}

				internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
				{
					throw ExceptionUtilities.Unreachable;
				}
			}

			private class SynthesizedComEventMethod : SynthesizedComMethod
			{
				private readonly EventSymbol _event;

				protected override Symbol NameAndAttributesSource => _event;

				internal override bool HasSpecialName => _event.HasSpecialName;

				public SynthesizedComEventMethod(SynthesizedComInterface container, EventSymbol @event, MethodSymbol clone, int synthesizedDispId)
					: base(container, clone, synthesizedDispId)
				{
					_event = @event;
				}
			}

			private sealed class SynthesizedComParameter : ParameterSymbol
			{
				private readonly Symbol _container;

				private readonly ParameterSymbol _clonedFrom;

				public override string Name => _clonedFrom.Name;

				public override Symbol ContainingSymbol => _container;

				public override ImmutableArray<CustomModifier> CustomModifiers => _clonedFrom.CustomModifiers;

				public override ImmutableArray<CustomModifier> RefCustomModifiers => _clonedFrom.RefCustomModifiers;

				public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public bool IsComEventParameter => ((SynthesizedComInterface)_container.ContainingSymbol).IsEventInterface;

				internal override ConstantValue ExplicitDefaultConstantValue
				{
					get
					{
						if (IsComEventParameter)
						{
							return null;
						}
						return _clonedFrom.get_ExplicitDefaultConstantValue(inProgress);
					}
				}

				public override bool HasExplicitDefaultValue
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.HasExplicitDefaultValue;
					}
				}

				public override bool IsByRef => _clonedFrom.IsByRef;

				internal override bool IsExplicitByRef => _clonedFrom.IsExplicitByRef;

				public override bool IsOptional
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsOptional;
					}
				}

				internal override bool IsMetadataOut
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsMetadataOut;
					}
				}

				internal override bool IsMetadataIn
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsMetadataIn;
					}
				}

				internal override bool HasOptionCompare
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.HasOptionCompare;
					}
				}

				internal override bool IsIDispatchConstant
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsIDispatchConstant;
					}
				}

				internal override bool IsIUnknownConstant
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsIUnknownConstant;
					}
				}

				internal override bool IsCallerLineNumber
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsCallerLineNumber;
					}
				}

				internal override bool IsCallerMemberName
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsCallerMemberName;
					}
				}

				internal override bool IsCallerFilePath
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsCallerFilePath;
					}
				}

				public override bool IsParamArray
				{
					get
					{
						if (IsComEventParameter)
						{
							return false;
						}
						return _clonedFrom.IsParamArray;
					}
				}

				internal override MarshalPseudoCustomAttributeData MarshallingInformation
				{
					get
					{
						if (IsComEventParameter)
						{
							return null;
						}
						return _clonedFrom.MarshallingInformation;
					}
				}

				public override ImmutableArray<Location> Locations
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override int Ordinal => _clonedFrom.Ordinal;

				public override TypeSymbol Type => _clonedFrom.Type;

				public SynthesizedComParameter(SynthesizedComMethod container, ParameterSymbol clone)
				{
					_container = container;
					_clonedFrom = clone;
				}

				public SynthesizedComParameter(SynthesizedComProperty container, ParameterSymbol clone)
				{
					_container = container;
					_clonedFrom = clone;
				}

				public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
				{
					if (IsComEventParameter)
					{
						return ImmutableArray<VisualBasicAttributeData>.Empty;
					}
					return _clonedFrom.GetAttributes();
				}

				internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
				{
					base.AddSynthesizedAttributes(compilationState, ref attributes);
					if (IsComEventParameter)
					{
						return;
					}
					ArrayBuilder<SynthesizedAttributeData> attributes2 = null;
					_clonedFrom.AddSynthesizedAttributes(compilationState, ref attributes2);
					VisualBasicCompilation declaringCompilation = DeclaringCompilation;
					NamedTypeSymbol wellKnownType = declaringCompilation.GetWellKnownType(WellKnownType.System_ParamArrayAttribute);
					NamedTypeSymbol wellKnownType2 = declaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_DateTimeConstantAttribute);
					NamedTypeSymbol wellKnownType3 = declaringCompilation.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_DecimalConstantAttribute);
					if (attributes2 == null)
					{
						return;
					}
					ArrayBuilder<SynthesizedAttributeData>.Enumerator enumerator = attributes2.GetEnumerator();
					while (enumerator.MoveNext())
					{
						SynthesizedAttributeData current = enumerator.Current;
						if ((object)current.AttributeClass == wellKnownType || (object)current.AttributeClass == wellKnownType2 || (object)current.AttributeClass == wellKnownType3)
						{
							Symbol.AddSynthesizedAttribute(ref attributes, current);
						}
					}
					attributes2.Free();
				}
			}

			private class SynthesizedComProperty : PropertySymbol
			{
				private readonly SynthesizedComInterface _interface;

				private readonly PropertySymbol _clonedFrom;

				private readonly int _synthesizedDispId;

				private readonly SynthesizedComMethod _getter;

				private readonly SynthesizedComMethod _setter;

				private readonly ImmutableArray<ParameterSymbol> _parameters;

				public override string Name => _clonedFrom.Name;

				internal override bool HasSpecialName => _clonedFrom.HasSpecialName;

				internal override Microsoft.Cci.CallingConvention CallingConvention => _clonedFrom.CallingConvention;

				public override Symbol ContainingSymbol => _interface;

				public override NamedTypeSymbol ContainingType => _interface;

				public override Accessibility DeclaredAccessibility => Accessibility.Public;

				public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImmutableArray<PropertySymbol>.Empty;

				public override MethodSymbol GetMethod => _getter;

				internal override FieldSymbol AssociatedField => null;

				public override bool IsDefault
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override bool IsMustOverride => true;

				public override bool IsNotOverridable => false;

				public override bool IsOverloads => false;

				public override bool IsOverridable => false;

				public override bool IsOverrides => false;

				public override bool IsShared => false;

				public override ImmutableArray<Location> Locations
				{
					get
					{
						throw ExceptionUtilities.Unreachable;
					}
				}

				public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

				public override MethodSymbol SetMethod => _setter;

				public override bool ReturnsByRef => _clonedFrom.ReturnsByRef;

				public override TypeSymbol Type => _clonedFrom.Type;

				internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

				public override ImmutableArray<CustomModifier> TypeCustomModifiers => _clonedFrom.TypeCustomModifiers;

				public override ImmutableArray<CustomModifier> RefCustomModifiers => _clonedFrom.RefCustomModifiers;

				internal override bool IsMyGroupCollectionProperty => false;

				public SynthesizedComProperty(SynthesizedComInterface container, PropertySymbol clone, SynthesizedComMethod getter, SynthesizedComMethod setter, int synthesizedDispId)
				{
					_interface = container;
					_clonedFrom = clone;
					_synthesizedDispId = synthesizedDispId;
					_getter = getter;
					_setter = setter;
					if (clone.ParameterCount == 0)
					{
						_parameters = ImmutableArray<ParameterSymbol>.Empty;
						return;
					}
					ParameterSymbol[] array = new ParameterSymbol[clone.ParameterCount - 1 + 1];
					int num = array.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = new SynthesizedComParameter(this, clone.Parameters[i]);
					}
					_parameters = array.AsImmutable();
				}

				public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
				{
					return _clonedFrom.GetAttributes();
				}

				internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
				{
					base.AddSynthesizedAttributes(compilationState, ref attributes);
					if (_synthesizedDispId != -1)
					{
						Symbol.AddSynthesizedAttribute(ref attributes, _interface.ComClass.DeclaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_DispIdAttribute__ctor, ImmutableArray.Create(new TypedConstant(_interface.ComClass.GetSpecialType(SpecialType.System_Int32), TypedConstantKind.Primitive, _synthesizedDispId))));
					}
				}
			}

			public readonly string ClassId;

			public readonly string InterfaceId;

			public readonly string EventId;

			public readonly bool InterfaceShadows;

			private ImmutableArray<NamedTypeSymbol> _syntheticInterfaces;

			public ComClassData(VisualBasicAttributeData attrData)
			{
				ImmutableArray<TypedConstant> commonConstructorArguments = attrData.CommonConstructorArguments;
				if (commonConstructorArguments.Length > 0)
				{
					string text = ((commonConstructorArguments[0].Kind != TypedConstantKind.Array) ? (commonConstructorArguments[0].ValueInternal as string) : null);
					if (!string.IsNullOrEmpty(text))
					{
						ClassId = text;
					}
					if (commonConstructorArguments.Length > 1)
					{
						text = ((commonConstructorArguments[1].Kind != TypedConstantKind.Array) ? (commonConstructorArguments[1].ValueInternal as string) : null);
						if (!string.IsNullOrEmpty(text))
						{
							InterfaceId = text;
						}
						if (commonConstructorArguments.Length > 2)
						{
							text = ((commonConstructorArguments[2].Kind != TypedConstantKind.Array) ? (commonConstructorArguments[2].ValueInternal as string) : null);
							if (!string.IsNullOrEmpty(text))
							{
								EventId = text;
							}
						}
					}
				}
				InterfaceShadows = attrData.DecodeNamedArgument("InterfaceShadows", SpecialType.System_Boolean, defaultValue: false);
			}

			public ImmutableArray<NamedTypeSymbol> GetSynthesizedInterfaces()
			{
				return _syntheticInterfaces;
			}

			public NamedTypeSymbol GetSynthesizedEventInterface()
			{
				if (_syntheticInterfaces.Length > 1)
				{
					return _syntheticInterfaces[1];
				}
				return null;
			}

			public IEnumerable<NamedTypeSymbol> GetSynthesizedImplements()
			{
				if (_syntheticInterfaces.IsEmpty)
				{
					return null;
				}
				return SpecializedCollections.SingletonEnumerable(_syntheticInterfaces[0]);
			}

			public MethodSymbol GetCorrespondingComClassInterfaceMethod(MethodSymbol method)
			{
				if (_syntheticInterfaces.IsEmpty)
				{
					return null;
				}
				ImmutableArray<Symbol>.Enumerator enumerator = _syntheticInterfaces[0].GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.Method)
					{
						SynthesizedComMethod synthesizedComMethod = (SynthesizedComMethod)current;
						if ((object)synthesizedComMethod.ClonedFrom == method)
						{
							return synthesizedComMethod;
						}
					}
				}
				return null;
			}

			public void PerformComClassAnalysis(SourceNamedTypeSymbol comClass)
			{
				if (!_syntheticInterfaces.IsDefault)
				{
					return;
				}
				BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
				ImmutableArray<NamedTypeSymbol> value = ImmutableArray<NamedTypeSymbol>.Empty;
				ArrayBuilder<KeyValuePair<Symbol, int>> instance2 = ArrayBuilder<KeyValuePair<Symbol, int>>.GetInstance();
				ArrayBuilder<KeyValuePair<EventSymbol, int>> instance3 = ArrayBuilder<KeyValuePair<EventSymbol, int>>.GetInstance();
				string classId = ClassId;
				Guid guidVal = default(Guid);
				ValidateComClassGuid(comClass, classId, instance, out guidVal);
				if ((ValidateComClassGuid(comClass, InterfaceId, instance, out var guidVal2) & ValidateComClassGuid(comClass, EventId, instance, out var guidVal3)) && InterfaceId != null && EventId != null && guidVal2 == guidVal3)
				{
					Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.ERR_ComClassDuplicateGuids1, comClass.Name);
				}
				if (comClass.HasGuidAttribute())
				{
					Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.ERR_ComClassAndReservedAttribute1, AttributeDescription.GuidAttribute.Name);
				}
				if (comClass.HasClassInterfaceAttribute())
				{
					Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.ERR_ComClassAndReservedAttribute1, AttributeDescription.ClassInterfaceAttribute.Name);
				}
				if (comClass.HasComSourceInterfacesAttribute())
				{
					Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.ERR_ComClassAndReservedAttribute1, AttributeDescription.ComSourceInterfacesAttribute.Name);
				}
				if (!GetComVisibleState(comClass))
				{
					Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.ERR_ComClassAndReservedAttribute1, AttributeDescription.ComVisibleAttribute.Name + "(False)");
				}
				if (comClass.DeclaredAccessibility != Accessibility.Public)
				{
					Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.ERR_ComClassRequiresPublicClass1, comClass.Name);
				}
				else
				{
					NamedTypeSymbol containingType = comClass.ContainingType;
					while ((object)containingType != null)
					{
						if (containingType.DeclaredAccessibility != Accessibility.Public)
						{
							Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.ERR_ComClassRequiresPublicClass2, comClass.Name, containingType.Name);
							break;
						}
						containingType = containingType.ContainingType;
					}
				}
				if (comClass.IsMustInherit)
				{
					Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.ERR_ComClassCantBeAbstract0);
				}
				CheckForNameCollisions(comClass, instance);
				GetComClassMembers(comClass, instance2, instance3, out var haveDefaultProperty, instance);
				if (instance2.Count == 0 && instance3.Count == 0)
				{
					Binder.ReportDiagnostic(instance, comClass.Locations[0], ERRID.WRN_ComClassNoMembers1, comClass.Name);
				}
				else if (!instance.HasAnyErrors())
				{
					NamedTypeSymbol namedTypeSymbol = new SynthesizedComInterface(comClass, instance2);
					value = ((instance3.Count != 0) ? ImmutableArray.Create(namedTypeSymbol, new SynthesizedComInterface(comClass, instance3)) : ImmutableArray.Create(namedTypeSymbol));
				}
				if (ClassId != null || (InterfaceId != null && value.Length > 0) || (EventId != null && value.Length > 1))
				{
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_GuidAttribute__ctor, comClass.DeclaringCompilation, comClass.Locations[0], instance);
				}
				Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_ClassInterfaceAttribute__ctorClassInterfaceType, comClass.DeclaringCompilation, comClass.Locations[0], instance);
				if (value.Length > 1)
				{
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_ComSourceInterfacesAttribute__ctorString, comClass.DeclaringCompilation, comClass.Locations[0], instance);
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_InterfaceTypeAttribute__ctorInt16, comClass.DeclaringCompilation, comClass.Locations[0], instance);
				}
				if (value.Length > 0)
				{
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_ComVisibleAttribute__ctor, comClass.DeclaringCompilation, comClass.Locations[0], instance);
				}
				bool flag = false;
				ArrayBuilder<KeyValuePair<Symbol, int>>.Enumerator enumerator = instance2.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<Symbol, int> current = enumerator.Current;
					if ((object)current.Key != null && current.Value == -1)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ArrayBuilder<KeyValuePair<EventSymbol, int>>.Enumerator enumerator2 = instance3.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Value == -1)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_InteropServices_DispIdAttribute__ctor, comClass.DeclaringCompilation, comClass.Locations[0], instance);
				}
				if (haveDefaultProperty)
				{
					Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, comClass.DeclaringCompilation, comClass.Locations[0], instance);
				}
				instance2.Free();
				instance3.Free();
				comClass.ContainingSourceModule.AtomicStoreArrayAndDiagnostics(ref _syntheticInterfaces, value, instance);
				instance.Free();
			}

			private static bool ValidateComClassGuid(SourceNamedTypeSymbol comClass, string id, BindingDiagnosticBag diagnostics, out Guid guidVal = default(Guid))
			{
				if (id != null)
				{
					if (!Guid.TryParseExact(id, "D", out guidVal))
					{
						Binder.ReportDiagnostic(diagnostics, comClass.Locations[0], ERRID.ERR_BadAttributeUuid2, AttributeDescription.VisualBasicComClassAttribute.Name, id);
						return false;
					}
				}
				else
				{
					guidVal = default(Guid);
				}
				return true;
			}

			private static bool GetComVisibleState(Symbol target)
			{
				ImmutableArray<VisualBasicAttributeData> attributes = target.GetAttributes();
				int num = AttributeDataExtensions.IndexOfAttribute(attributes, target, AttributeDescription.ComVisibleAttribute);
				if (num > -1)
				{
					TypedConstant typedConstant = attributes[num].CommonConstructorArguments[0];
					object objectValue = RuntimeHelpers.GetObjectValue((typedConstant.Kind != TypedConstantKind.Array) ? typedConstant.ValueInternal : null);
					if (objectValue == null || (objectValue is bool && !(bool)objectValue))
					{
						return false;
					}
					return true;
				}
				return true;
			}

			private void CheckForNameCollisions(SourceNamedTypeSymbol comClass, BindingDiagnosticBag diagnostics)
			{
				int num = 0;
				do
				{
					string text = ((num == 0) ? "_" : "__") + comClass.Name;
					ImmutableArray<Symbol>.Enumerator enumerator = comClass.GetMembers(text).GetEnumerator();
					while (enumerator.MoveNext())
					{
						Symbol current = enumerator.Current;
						Binder.ReportDiagnostic(diagnostics, current.Locations[0], ERRID.ERR_MemberConflictWithSynth4, SyntaxFacts.GetText(SyntaxKind.InterfaceKeyword) + " " + text, AttributeDescription.VisualBasicComClassAttribute.Name, SyntaxFacts.GetText(SyntaxKind.ClassKeyword), comClass.Name);
					}
					if (!InterfaceShadows)
					{
						NamedTypeSymbol baseTypeNoUseSiteDiagnostics = comClass.BaseTypeNoUseSiteDiagnostics;
						while ((object)baseTypeNoUseSiteDiagnostics != null)
						{
							ImmutableArray<Symbol>.Enumerator enumerator2 = baseTypeNoUseSiteDiagnostics.GetMembers(text).GetEnumerator();
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.DeclaredAccessibility != Accessibility.Private)
								{
									Binder.ReportDiagnostic(diagnostics, comClass.Locations[0], ERRID.WRN_ComClassInterfaceShadows5, comClass.Name, SyntaxFacts.GetText(SyntaxKind.InterfaceKeyword), text, SyntaxFacts.GetText(SyntaxKind.ClassKeyword), baseTypeNoUseSiteDiagnostics);
								}
							}
							baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
						}
					}
					num++;
				}
				while (num <= 1);
			}

			private void GetComClassMembers(SourceNamedTypeSymbol comClass, ArrayBuilder<KeyValuePair<Symbol, int>> interfaceMembers, ArrayBuilder<KeyValuePair<EventSymbol, int>> eventMembers, out bool haveDefaultProperty, BindingDiagnosticBag diagnostics)
			{
				haveDefaultProperty = false;
				ImmutableArray<Symbol>.Enumerator enumerator = comClass.GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.IsShared || current.DeclaredAccessibility != Accessibility.Public || current.IsImplicitlyDeclared)
					{
						continue;
					}
					SymbolKind kind = current.Kind;
					if (kind == SymbolKind.Field)
					{
						continue;
					}
					if (kind != SymbolKind.Method)
					{
						if (kind == SymbolKind.NamedType)
						{
							continue;
						}
					}
					else if (((MethodSymbol)current).MethodKind != MethodKind.Ordinary)
					{
						continue;
					}
					if (!GetComVisibleState(current))
					{
						continue;
					}
					switch (kind)
					{
					case SymbolKind.Property:
					{
						PropertySymbol propertySymbol = (PropertySymbol)current;
						if (propertySymbol.IsWithEvents)
						{
							break;
						}
						MethodSymbol methodSymbol = propertySymbol.GetMethod;
						MethodSymbol methodSymbol2 = propertySymbol.SetMethod;
						if ((object)methodSymbol != null)
						{
							if (methodSymbol.IsImplicitlyDeclared)
							{
								break;
							}
							if (methodSymbol.DeclaredAccessibility != Accessibility.Public || !GetComVisibleState(methodSymbol))
							{
								methodSymbol = null;
							}
						}
						if ((object)methodSymbol2 != null)
						{
							if (methodSymbol2.IsImplicitlyDeclared)
							{
								break;
							}
							if (methodSymbol2.DeclaredAccessibility != Accessibility.Public || !GetComVisibleState(methodSymbol2))
							{
								methodSymbol2 = null;
							}
						}
						if ((object)methodSymbol != null || (object)methodSymbol2 != null)
						{
							if (TypeSymbolExtensions.IsObjectType(propertySymbol.Type) && (object)propertySymbol.SetMethod != null)
							{
								Binder.ReportDiagnostic(diagnostics, propertySymbol.Locations[0], ERRID.WRN_ComClassPropertySetObject1, propertySymbol);
							}
							interfaceMembers.Add(new KeyValuePair<Symbol, int>(propertySymbol, GetUserSpecifiedDispId(propertySymbol, diagnostics)));
							if (propertySymbol.IsDefault)
							{
								haveDefaultProperty = true;
							}
							interfaceMembers.Add(new KeyValuePair<Symbol, int>(methodSymbol, ((object)methodSymbol == null) ? (-1) : GetUserSpecifiedDispId(methodSymbol, diagnostics)));
							interfaceMembers.Add(new KeyValuePair<Symbol, int>(methodSymbol2, ((object)methodSymbol2 == null) ? (-1) : GetUserSpecifiedDispId(methodSymbol2, diagnostics)));
						}
						break;
					}
					case SymbolKind.Event:
						eventMembers.Add(new KeyValuePair<EventSymbol, int>((EventSymbol)current, GetUserSpecifiedDispId(current, diagnostics)));
						break;
					case SymbolKind.Method:
						if (((MethodSymbol)current).IsGenericMethod)
						{
							Binder.ReportDiagnostic(diagnostics, current.Locations[0], ERRID.ERR_ComClassGenericMethod);
						}
						interfaceMembers.Add(new KeyValuePair<Symbol, int>(current, GetUserSpecifiedDispId(current, diagnostics)));
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(kind);
					}
				}
			}

			private static int GetUserSpecifiedDispId(Symbol target, BindingDiagnosticBag diagnostics)
			{
				ImmutableArray<VisualBasicAttributeData> attributes = target.GetAttributes();
				int num = AttributeDataExtensions.IndexOfAttribute(attributes, target, AttributeDescription.DispIdAttribute);
				if (num > -1)
				{
					TypedConstant typedConstant = attributes[num].CommonConstructorArguments[0];
					object objectValue = RuntimeHelpers.GetObjectValue((typedConstant.Kind != TypedConstantKind.Array) ? typedConstant.ValueInternal : null);
					if (objectValue != null && objectValue is int num2)
					{
						if (num2 == 0)
						{
							if (target.Kind != SymbolKind.Property || !((PropertySymbol)target).IsDefault)
							{
								Binder.ReportDiagnostic(diagnostics, target.Locations[0], ERRID.ERR_ComClassReservedDispIdZero1, target.Name);
							}
						}
						else if (num2 < 0)
						{
							Binder.ReportDiagnostic(diagnostics, target.Locations[0], ERRID.ERR_ComClassReservedDispId1, target.Name);
						}
						return num2;
					}
				}
				return -1;
			}
		}

		private class GroupCollectionComparer : IComparer<KeyValuePair<NamedTypeSymbol, int>>
		{
			public static readonly GroupCollectionComparer Singleton = new GroupCollectionComparer();

			private GroupCollectionComparer()
			{
			}

			public int Compare(KeyValuePair<NamedTypeSymbol, int> x, KeyValuePair<NamedTypeSymbol, int> y)
			{
				return CaseInsensitiveComparison.Compare(x.Key.Name, y.Key.Name);
			}

			int IComparer<KeyValuePair<NamedTypeSymbol, int>>.Compare(KeyValuePair<NamedTypeSymbol, int> x, KeyValuePair<NamedTypeSymbol, int> y)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Compare
				return this.Compare(x, y);
			}
		}

		private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

		protected CustomAttributesBag<VisualBasicAttributeData> m_lazyCustomAttributesBag;

		private readonly SpecialType _corTypeId;

		private string _lazyDocComment;

		private string _lazyExpandedDocComment;

		private NamedTypeSymbol _lazyEnumUnderlyingType;

		private ConcurrentDictionary<PropertySymbol, SynthesizedOverridingWithEventsProperty> _lazyWithEventsOverrides;

		private bool _withEventsOverridesAreFrozen;

		internal const SourceMemberFlags DelegateConstructorMethodFlags = SourceMemberFlags.Static;

		internal const SourceMemberFlags DelegateCommonMethodFlags = SourceMemberFlags.Overridable;

		private LexicalSortKey _lazyLexicalSortKey;

		private ThreeState _lazyIsExtensibleInterface;

		private ThreeState _lazyIsExplicitDefinitionOfNoPiaLocalType;

		private ComClassData _comClassData;

		private TypeSymbol _lazyCoClassType;

		protected DiagnosticInfo m_baseCycleDiagnosticInfo;

		public override SpecialType SpecialType => _corTypeId;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters
		{
			get
			{
				if (_lazyTypeParameters.IsDefault)
				{
					ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeParameters, MakeTypeParameters());
				}
				return _lazyTypeParameters;
			}
		}

		public override NamedTypeSymbol EnumUnderlyingType
		{
			get
			{
				if (!TypeSymbolExtensions.IsEnumType(this))
				{
					return null;
				}
				NamedTypeSymbol namedTypeSymbol = _lazyEnumUnderlyingType;
				if ((object)namedTypeSymbol == null)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					SyntaxReference syntaxReference = base.SyntaxReferences[0];
					SyntaxTree syntaxTree = syntaxReference.SyntaxTree;
					EnumBlockSyntax syntax = (EnumBlockSyntax)syntaxReference.GetSyntax();
					Binder bodyBinder = BinderBuilder.CreateBinderForType(base.ContainingSourceModule, syntaxTree, this);
					namedTypeSymbol = BindEnumUnderlyingType(syntax, bodyBinder, instance);
					if ((object)Interlocked.CompareExchange(ref _lazyEnumUnderlyingType, namedTypeSymbol, null) == null)
					{
						base.ContainingSourceModule.AddDeclarationDiagnostics(instance);
					}
					else
					{
						namedTypeSymbol = _lazyEnumUnderlyingType;
					}
					instance.Free();
				}
				return namedTypeSymbol;
			}
		}

		public AttributeLocation DefaultAttributeLocation => AttributeLocation.Type;

		internal override bool HasCodeAnalysisEmbeddedAttribute => GetEarlyDecodedWellKnownAttributeData()?.HasCodeAnalysisEmbeddedAttribute ?? false;

		internal override bool HasVisualBasicEmbeddedAttribute => GetEarlyDecodedWellKnownAttributeData()?.HasVisualBasicEmbeddedAttribute ?? false;

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics
		{
			get
			{
				if (_lazyIsExtensibleInterface == ThreeState.Unknown)
				{
					_lazyIsExtensibleInterface = DecodeIsExtensibleInterface().ToThreeState();
				}
				return _lazyIsExtensibleInterface.Value();
			}
		}

		internal override bool IsComImport => GetEarlyDecodedWellKnownAttributeData()?.HasComImportAttribute ?? false;

		internal override TypeSymbol CoClassType
		{
			get
			{
				if ((object)_lazyCoClassType == ErrorTypeSymbol.UnknownResultType)
				{
					if (!IsInterface)
					{
						Interlocked.CompareExchange(ref _lazyCoClassType, null, ErrorTypeSymbol.UnknownResultType);
					}
					else
					{
						GetDecodedWellKnownAttributeData();
						if ((object)_lazyCoClassType == ErrorTypeSymbol.UnknownResultType)
						{
							Interlocked.CompareExchange(ref _lazyCoClassType, null, ErrorTypeSymbol.UnknownResultType);
						}
					}
				}
				return _lazyCoClassType;
			}
		}

		internal override bool IsWindowsRuntimeImport => GetDecodedWellKnownAttributeData()?.HasWindowsRuntimeImportAttribute ?? false;

		internal override bool ShouldAddWinRTMembers => false;

		internal bool HasSecurityCriticalAttributes => GetDecodedWellKnownAttributeData()?.HasSecurityCriticalAttributes ?? false;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				CustomAttributesBag<VisualBasicAttributeData> lazyCustomAttributesBag = m_lazyCustomAttributesBag;
				if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
				{
					return ((CommonTypeEarlyWellKnownAttributeData)lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
				}
				ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = base.TypeDeclaration.Declarations.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.HasAnyAttributes)
					{
						return ObsoleteAttributeData.Uninitialized;
					}
				}
				return null;
			}
		}

		internal sealed override bool HasDeclarativeSecurity => GetDecodedWellKnownAttributeData()?.HasDeclarativeSecurity ?? false;

		internal override bool IsExplicitDefinitionOfNoPiaLocalType
		{
			get
			{
				if (_lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.Unknown)
				{
					CheckPresenceOfTypeIdentifierAttribute();
					if (_lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.Unknown)
					{
						_lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.False;
					}
				}
				return _lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.True;
			}
		}

		internal sealed override bool IsDirectlyExcludedFromCodeCoverage => GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute ?? false;

		internal sealed override bool HasSpecialName => GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;

		public sealed override bool IsSerializable => GetDecodedWellKnownAttributeData()?.HasSerializableAttribute ?? false;

		internal sealed override TypeLayout Layout
		{
			get
			{
				CommonTypeWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
				if (decodedWellKnownAttributeData != null && decodedWellKnownAttributeData.HasStructLayoutAttribute)
				{
					return decodedWellKnownAttributeData.Layout;
				}
				TypeLayout result = ((TypeKind != TypeKind.Struct) ? default(TypeLayout) : new TypeLayout(LayoutKind.Sequential, (!HasInstanceFields()) ? 1 : 0, 0));
				return result;
			}
		}

		internal bool HasStructLayoutAttribute => GetDecodedWellKnownAttributeData()?.HasStructLayoutAttribute ?? false;

		internal override CharSet MarshallingCharSet
		{
			get
			{
				CommonTypeWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
				if (decodedWellKnownAttributeData == null || !decodedWellKnownAttributeData.HasStructLayoutAttribute)
				{
					return base.DefaultMarshallingCharSet;
				}
				return decodedWellKnownAttributeData.MarshallingCharSet;
			}
		}

		internal SourceNamedTypeSymbol(MergedTypeDeclaration declaration, NamespaceOrTypeSymbol containingSymbol, SourceModuleSymbol containingModule)
			: base(declaration, containingSymbol, containingModule)
		{
			_lazyLexicalSortKey = LexicalSortKey.NotInitialized;
			_lazyIsExtensibleInterface = ThreeState.Unknown;
			_lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.Unknown;
			_lazyCoClassType = ErrorTypeSymbol.UnknownResultType;
			m_baseCycleDiagnosticInfo = null;
			if (containingSymbol.Kind == SymbolKind.Namespace && containingSymbol.ContainingAssembly.KeepLookingForDeclaredSpecialTypes && DeclaredAccessibility == Accessibility.Public)
			{
				string qualifier = GetEmittedNamespaceName() ?? base.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
				qualifier = MetadataHelpers.BuildQualifiedName(qualifier, MetadataName);
				_corTypeId = SpecialTypes.GetTypeFromMetadataName(qualifier);
			}
			else
			{
				_corTypeId = SpecialType.None;
			}
			if (containingSymbol.Kind == SymbolKind.NamedType)
			{
				_lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.False;
			}
		}

		protected override void GenerateAllDeclarationErrorsImpl(CancellationToken cancellationToken)
		{
			base.GenerateAllDeclarationErrorsImpl(cancellationToken);
			_withEventsOverridesAreFrozen = true;
			cancellationToken.ThrowIfCancellationRequested();
			PerformComClassAnalysis();
			cancellationToken.ThrowIfCancellationRequested();
			CheckBaseConstraints();
			cancellationToken.ThrowIfCancellationRequested();
			CheckInterfacesConstraints();
		}

		internal SyntaxToken GetTypeIdentifierToken(VisualBasicSyntaxNode node)
		{
			switch (node.Kind())
			{
			case SyntaxKind.ModuleBlock:
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
				return ((TypeBlockSyntax)node).BlockStatement.Identifier;
			case SyntaxKind.EnumBlock:
				return ((EnumBlockSyntax)node).EnumStatement.Identifier;
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
				return ((DelegateStatementSyntax)node).Identifier;
			default:
				throw ExceptionUtilities.UnexpectedValue(node.Kind());
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

		private Binder CreateLocationSpecificBinderForType(SyntaxTree tree, BindingLocation location)
		{
			Binder containingBinder = BinderBuilder.CreateBinderForType(base.ContainingSourceModule, tree, this);
			return new LocationSpecificBinder(location, containingBinder);
		}

		protected override void AddDeclaredNonTypeMembers(MembersAndInitializersBuilder membersBuilder, BindingDiagnosticBag diagnostics)
		{
			DeclarationModifiers declarationModifiers = DeclarationModifiers.None;
			bool flag = false;
			bool nodeNameIsAlreadyDefined = false;
			VisualBasicSyntaxNode visualBasicSyntaxNode = null;
			int num = 0;
			ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(current);
				Binder binder = BinderBuilder.CreateBinderForType(base.ContainingSourceModule, current.SyntaxTree, this);
				ArrayBuilder<FieldOrPropertyInitializer> staticInitializers = null;
				ArrayBuilder<FieldOrPropertyInitializer> instanceInitializers = null;
				DeclarationModifiers declarationModifiers2 = AddMembersInPart(binder, visualBasicSyntax, diagnostics, declarationModifiers, membersBuilder, ref staticInitializers, ref instanceInitializers, ref nodeNameIsAlreadyDefined);
				if (declarationModifiers == DeclarationModifiers.None)
				{
					declarationModifiers = declarationModifiers2 & DeclarationModifiers.AllAccessibilityModifiers;
				}
				if ((declarationModifiers2 & DeclarationModifiers.Partial) != 0)
				{
					if (!flag)
					{
						visualBasicSyntaxNode = visualBasicSyntax;
						flag = true;
					}
				}
				else
				{
					num++;
					if (visualBasicSyntaxNode == null)
					{
						visualBasicSyntaxNode = visualBasicSyntax;
					}
				}
				ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>> allInitializers = membersBuilder.StaticInitializers;
				SourceMemberContainerTypeSymbol.AddInitializers(ref allInitializers, staticInitializers);
				membersBuilder.StaticInitializers = allInitializers;
				allInitializers = membersBuilder.InstanceInitializers;
				SourceMemberContainerTypeSymbol.AddInitializers(ref allInitializers, instanceInitializers);
				membersBuilder.InstanceInitializers = allInitializers;
			}
			if (!nodeNameIsAlreadyDefined && num >= 2)
			{
				ImmutableArray<SyntaxReference>.Enumerator enumerator2 = base.SyntaxReferences.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SyntaxReference current2 = enumerator2.Current;
					CheckDeclarationPart(current2.SyntaxTree, VisualBasicExtensions.GetVisualBasicSyntax(current2), visualBasicSyntaxNode, flag, diagnostics);
				}
			}
		}

		private DeclarationModifiers AddMembersInPart(Binder binder, VisualBasicSyntaxNode node, BindingDiagnosticBag diagBag, DeclarationModifiers accessModifiers, MembersAndInitializersBuilder members, ref ArrayBuilder<FieldOrPropertyInitializer> staticInitializers, ref ArrayBuilder<FieldOrPropertyInitializer> instanceInitializers, ref bool nodeNameIsAlreadyDefined)
		{
			CheckDeclarationNameAndTypeParameters(node, binder, diagBag, ref nodeNameIsAlreadyDefined);
			DeclarationModifiers result = CheckDeclarationModifiers(node, binder, diagBag.DiagnosticBag, accessModifiers);
			if (TypeKind == TypeKind.Delegate)
			{
				if (members.Members.Count == 0)
				{
					MethodSymbol constructor = null;
					MethodSymbol beginInvoke = null;
					MethodSymbol endInvoke = null;
					MethodSymbol invoke = null;
					ParameterListSyntax parameterList = ((DelegateStatementSyntax)node).ParameterList;
					SourceDelegateMethodSymbol.MakeDelegateMembers(this, node, parameterList, binder, out constructor, out beginInvoke, out endInvoke, out invoke, diagBag);
					AddSymbolToMembers(constructor, members.Members);
					if ((object)beginInvoke != null)
					{
						AddSymbolToMembers(beginInvoke, members.Members);
					}
					if ((object)endInvoke != null)
					{
						AddSymbolToMembers(endInvoke, members.Members);
					}
					AddSymbolToMembers(invoke, members.Members);
				}
			}
			else if (TypeKind == TypeKind.Enum)
			{
				EnumBlockSyntax syntax = (EnumBlockSyntax)node;
				AddEnumMembers(syntax, binder, diagBag, members);
			}
			else
			{
				SyntaxList<StatementSyntax>.Enumerator enumerator = ((TypeBlockSyntax)node).Members.GetEnumerator();
				while (enumerator.MoveNext())
				{
					StatementSyntax current = enumerator.Current;
					AddMember(current, binder, diagBag, members, ref staticInitializers, ref instanceInitializers, reportAsInvalid: false);
				}
			}
			return result;
		}

		private DeclarationModifiers CheckDeclarationModifiers(VisualBasicSyntaxNode node, Binder binder, DiagnosticBag diagBag, DeclarationModifiers accessModifiers)
		{
			SyntaxTokenList modifiers = default(SyntaxTokenList);
			SyntaxToken id = default(SyntaxToken);
			DeclarationModifiers declarationModifiers = DecodeDeclarationModifiers(node, binder, diagBag, ref modifiers, ref id);
			if (accessModifiers != 0)
			{
				DeclarationModifiers declarationModifiers2 = declarationModifiers & DeclarationModifiers.AllAccessibilityModifiers & ~accessModifiers;
				if (declarationModifiers2 != 0)
				{
					Binder.ReportDiagnostic(diagBag, id, ERRID.ERR_PartialTypeAccessMismatch3, ErrorMessageHelpers.ToDisplay(DeclarationModifiersExtensions.ToAccessibility(declarationModifiers2)), id.ToString(), ErrorMessageHelpers.ToDisplay(DeclarationModifiersExtensions.ToAccessibility(accessModifiers)));
				}
			}
			if (IsNotInheritable && (declarationModifiers & DeclarationModifiers.MustInherit) != 0 && (declarationModifiers & DeclarationModifiers.NotInheritable) == 0)
			{
				Binder.ReportDiagnostic(diagBag, id, ERRID.ERR_PartialTypeBadMustInherit1, id.ToString());
			}
			SourceNamedTypeSymbol sourceNamedTypeSymbol = ContainingType as SourceNamedTypeSymbol;
			bool flag = (object)sourceNamedTypeSymbol != null && !sourceNamedTypeSymbol.IsNamespace;
			if (flag)
			{
				switch (sourceNamedTypeSymbol.DeclarationKind)
				{
				case DeclarationKind.Module:
					if ((declarationModifiers & DeclarationModifiers.InvalidInModule) != 0)
					{
						binder.ReportModifierError(modifiers, ERRID.ERR_ModuleCantUseTypeSpecifier1, diagBag, InvalidModifiers.InvalidModifiersInModule);
						declarationModifiers &= ~DeclarationModifiers.InvalidInModule;
					}
					break;
				case DeclarationKind.Interface:
				{
					if ((declarationModifiers & DeclarationModifiers.InvalidInInterface) == 0)
					{
						break;
					}
					ERRID eRRID = ERRID.ERR_None;
					switch (base.DeclarationKind)
					{
					case DeclarationKind.Class:
						eRRID = ERRID.ERR_BadInterfaceClassSpecifier1;
						break;
					case DeclarationKind.Delegate:
						eRRID = ERRID.ERR_BadInterfaceDelegateSpecifier1;
						break;
					case DeclarationKind.Structure:
						eRRID = ERRID.ERR_BadInterfaceStructSpecifier1;
						break;
					case DeclarationKind.Enum:
						eRRID = ERRID.ERR_BadInterfaceEnumSpecifier1;
						break;
					case DeclarationKind.Interface:
					{
						DeclarationModifiers declarationModifiers3 = DeclarationModifiers.Private | DeclarationModifiers.Protected | DeclarationModifiers.Shared;
						if ((declarationModifiers & declarationModifiers3) != 0)
						{
							binder.ReportModifierError(modifiers, ERRID.ERR_BadInterfaceInterfaceSpecifier1, diagBag, SyntaxKind.PrivateKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.SharedKeyword);
							declarationModifiers &= ~declarationModifiers3;
						}
						break;
					}
					}
					if (eRRID != 0)
					{
						binder.ReportModifierError(modifiers, eRRID, diagBag, SyntaxKind.PrivateKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.FriendKeyword, SyntaxKind.PublicKeyword, SyntaxKind.SharedKeyword);
						declarationModifiers &= ~DeclarationModifiers.InvalidInInterface;
					}
					break;
				}
				}
			}
			else
			{
				if ((declarationModifiers & DeclarationModifiers.Private) != 0)
				{
					Binder.ReportDiagnostic(diagBag, id, ERRID.ERR_PrivateTypeOutsideType);
				}
				if ((declarationModifiers & DeclarationModifiers.Shadows) != 0)
				{
					Binder.ReportDiagnostic(diagBag, id, ERRID.ERR_ShadowingTypeOutsideClass1, id.ToString());
					declarationModifiers &= ~DeclarationModifiers.Shadows;
				}
			}
			if ((declarationModifiers & DeclarationModifiers.Protected) != 0 && (!flag || sourceNamedTypeSymbol.DeclarationKind != DeclarationKind.Class))
			{
				Binder.ReportDiagnostic(diagBag, id, ERRID.ERR_ProtectedTypeOutsideClass);
				declarationModifiers &= ~DeclarationModifiers.Protected;
			}
			return declarationModifiers;
		}

		private DeclarationModifiers DecodeDeclarationModifiers(VisualBasicSyntaxNode node, Binder binder, DiagnosticBag diagBag, ref SyntaxTokenList modifiers, ref SyntaxToken id)
		{
			SourceMemberFlags allowableModifiers = SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.Shadows;
			ERRID eRRID = ERRID.ERR_None;
			switch (node.Kind())
			{
			case SyntaxKind.ModuleBlock:
			{
				eRRID = ERRID.ERR_BadModuleFlags1;
				allowableModifiers = SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.Partial;
				TypeBlockSyntax typeBlockSyntax = (TypeBlockSyntax)node;
				modifiers = typeBlockSyntax.BlockStatement.Modifiers;
				id = typeBlockSyntax.BlockStatement.Identifier;
				break;
			}
			case SyntaxKind.ClassBlock:
			{
				eRRID = ERRID.ERR_BadClassFlags1;
				allowableModifiers = SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.TypeInheritModifiers | SourceMemberFlags.Shadows | SourceMemberFlags.Partial;
				TypeBlockSyntax typeBlockSyntax = (TypeBlockSyntax)node;
				modifiers = typeBlockSyntax.BlockStatement.Modifiers;
				id = typeBlockSyntax.BlockStatement.Identifier;
				break;
			}
			case SyntaxKind.StructureBlock:
			{
				eRRID = ERRID.ERR_BadRecordFlags1;
				allowableModifiers = SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.Shadows | SourceMemberFlags.Partial;
				TypeBlockSyntax typeBlockSyntax = (TypeBlockSyntax)node;
				modifiers = typeBlockSyntax.BlockStatement.Modifiers;
				id = typeBlockSyntax.BlockStatement.Identifier;
				break;
			}
			case SyntaxKind.InterfaceBlock:
			{
				eRRID = ERRID.ERR_BadInterfaceFlags1;
				allowableModifiers = SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.Shadows | SourceMemberFlags.Partial;
				TypeBlockSyntax typeBlockSyntax = (TypeBlockSyntax)node;
				modifiers = typeBlockSyntax.BlockStatement.Modifiers;
				id = typeBlockSyntax.BlockStatement.Identifier;
				break;
			}
			case SyntaxKind.EnumBlock:
			{
				eRRID = ERRID.ERR_BadEnumFlags1;
				EnumBlockSyntax enumBlockSyntax = (EnumBlockSyntax)node;
				modifiers = enumBlockSyntax.EnumStatement.Modifiers;
				id = enumBlockSyntax.EnumStatement.Identifier;
				break;
			}
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
				eRRID = ERRID.ERR_BadDelegateFlags1;
				modifiers = ((DelegateStatementSyntax)node).Modifiers;
				id = ((DelegateStatementSyntax)node).Identifier;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(node.Kind());
			}
			if (modifiers.Count != 0)
			{
				return (DeclarationModifiers)((int)(binder.DecodeModifiers(modifiers, allowableModifiers, eRRID, Accessibility.NotApplicable, diagBag).FoundFlags & SourceMemberFlags.DeclarationModifierFlagMask) >> 3);
			}
			return DeclarationModifiers.None;
		}

		private void CheckDeclarationNameAndTypeParameters(VisualBasicSyntaxNode node, Binder binder, BindingDiagnosticBag diagBag, ref bool nodeNameIsAlreadyDeclared)
		{
			SyntaxToken typeIdentifierToken = GetTypeIdentifierToken(node);
			Binder.DisallowTypeCharacter(typeIdentifierToken, diagBag);
			bool isEmbedded = base.IsEmbedded;
			NamespaceOrTypeSymbol namespaceOrTypeSymbol = ContainingSymbol as NamespaceOrTypeSymbol;
			if ((object)namespaceOrTypeSymbol != null)
			{
				ImmutableArray<Symbol> immutableArray = ((!namespaceOrTypeSymbol.IsNamespace) ? StaticCast<Symbol>.From(namespaceOrTypeSymbol.GetTypeMembers(base.Name)) : namespaceOrTypeSymbol.GetMembers(base.Name));
				int arity = base.Arity;
				ImmutableArray<Symbol>.Enumerator enumerator = immutableArray.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if ((object)current == this)
					{
						continue;
					}
					SymbolKind kind = current.Kind;
					object kindText;
					if (kind != SymbolKind.NamedType)
					{
						if (kind != SymbolKind.Namespace || arity > 0)
						{
							continue;
						}
						kindText = SymbolExtensions.GetKindText((NamespaceSymbol)current);
					}
					else
					{
						NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)current;
						if (namedTypeSymbol.Arity != arity)
						{
							continue;
						}
						kindText = SymbolExtensions.GetKindText(namedTypeSymbol);
					}
					if (current.IsEmbedded)
					{
						Binder.ReportDiagnostic(diagBag, typeIdentifierToken, ERRID.ERR_TypeClashesWithVbCoreType4, SymbolExtensions.GetKindText(this), typeIdentifierToken.ToString(), kindText, current.Name);
					}
					else
					{
						if (isEmbedded)
						{
							if (current.Kind != SymbolKind.Namespace)
							{
								continue;
							}
							bool flag = false;
							ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								Location current2 = enumerator2.Current;
								if (current2.IsInSource && !EmbeddedSymbolExtensions.IsEmbeddedSyntaxTree((VisualBasicSyntaxTree)current2.SourceTree))
								{
									Binder.ReportDiagnostic(diagBag, current2, ERRID.ERR_TypeClashesWithVbCoreType4, kindText, current.Name, SymbolExtensions.GetKindText(this), typeIdentifierToken.ToString());
									flag = true;
									break;
								}
							}
							if (flag)
							{
								break;
							}
							continue;
						}
						if ((object)ContainingType == null || namespaceOrTypeSymbol.Locations.Length == 1 || !(namespaceOrTypeSymbol is SourceMemberContainerTypeSymbol) || ((SourceMemberContainerTypeSymbol)namespaceOrTypeSymbol).IsPartial)
						{
							Binder.ReportDiagnostic(diagBag, typeIdentifierToken, ERRID.ERR_TypeConflict6, SymbolExtensions.GetKindText(this), typeIdentifierToken.ToString(), kindText, current.Name, SymbolExtensions.GetKindText(namespaceOrTypeSymbol), SymbolExtensions.ToErrorMessageArgument(ContainingSymbol, ERRID.ERR_TypeConflict6));
						}
					}
					nodeNameIsAlreadyDeclared = true;
					break;
				}
				if (!nodeNameIsAlreadyDeclared && namespaceOrTypeSymbol.IsNamespace && ContainingAssembly.Modules.Length > 1)
				{
					NamespaceSymbol namespaceSymbol = (NamespaceSymbol)namespaceOrTypeSymbol;
					if (ContainingAssembly.GetAssemblyNamespace(namespaceSymbol) is MergedNamespaceSymbol mergedNamespaceSymbol)
					{
						string a = GetEmittedNamespaceName() ?? namespaceSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
						NamedTypeSymbol namedTypeSymbol2 = null;
						ImmutableArray<NamespaceSymbol>.Enumerator enumerator3 = mergedNamespaceSymbol.ConstituentNamespaces.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							NamespaceSymbol current3 = enumerator3.Current;
							if ((object)current3 == namespaceOrTypeSymbol || ((object)namedTypeSymbol2 != null && namedTypeSymbol2.ContainingModule.Ordinal < current3.ContainingModule.Ordinal))
							{
								continue;
							}
							ImmutableArray<NamedTypeSymbol> typeMembers = current3.GetTypeMembers(base.Name, arity);
							if (typeMembers.Length == 0)
							{
								continue;
							}
							string text = current3.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
							ImmutableArray<NamedTypeSymbol>.Enumerator enumerator4 = typeMembers.GetEnumerator();
							while (enumerator4.MoveNext())
							{
								NamedTypeSymbol current4 = enumerator4.Current;
								if (current4.DeclaredAccessibility == Accessibility.Public && current4.MangleName == base.MangleName && string.Equals(base.Name, current4.Name, StringComparison.Ordinal) && string.Equals(a, current4.GetEmittedNamespaceName() ?? text, StringComparison.Ordinal))
								{
									namedTypeSymbol2 = current4;
									break;
								}
							}
						}
						if ((object)namedTypeSymbol2 != null)
						{
							Binder.ReportDiagnostic(diagBag, typeIdentifierToken, ERRID.ERR_CollisionWithPublicTypeInModule, this, namedTypeSymbol2.ContainingModule);
						}
					}
				}
			}
			if (namespaceOrTypeSymbol is SourceNamedTypeSymbol sourceNamedTypeSymbol && SymbolExtensions.MatchesAnyName(sourceNamedTypeSymbol.TypeParameters, base.Name))
			{
				Binder.ReportDiagnostic(diagBag, typeIdentifierToken, ERRID.ERR_ShadowingGenericParamWithMember1, base.Name);
			}
			CheckForDuplicateTypeParameters(TypeParameters, diagBag);
		}

		private void CheckDeclarationPart(SyntaxTree tree, VisualBasicSyntaxNode node, VisualBasicSyntaxNode firstNode, bool foundPartial, BindingDiagnosticBag diagBag)
		{
			if (node != firstNode)
			{
				Binder binder = BinderBuilder.CreateBinderForType(base.ContainingSourceModule, tree, this);
				SyntaxTokenList syntaxTokenList = default(SyntaxTokenList);
				switch (node.Kind())
				{
				case SyntaxKind.DelegateSubStatement:
				case SyntaxKind.DelegateFunctionStatement:
					syntaxTokenList = ((DelegateStatementSyntax)node).Modifiers;
					break;
				case SyntaxKind.EnumBlock:
					syntaxTokenList = ((EnumBlockSyntax)node).EnumStatement.Modifiers;
					break;
				case SyntaxKind.ModuleBlock:
				case SyntaxKind.StructureBlock:
				case SyntaxKind.InterfaceBlock:
				case SyntaxKind.ClassBlock:
					syntaxTokenList = ((TypeBlockSyntax)node).BlockStatement.Modifiers;
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(node.Kind());
				}
				SyntaxToken id = default(SyntaxToken);
				DiagnosticBag instance = DiagnosticBag.GetInstance();
				DeclarationModifiers num = DecodeDeclarationModifiers(node, binder, instance, ref syntaxTokenList, ref id);
				instance.Free();
				if ((num & DeclarationModifiers.Partial) == 0)
				{
					ERRID eRRID = (foundPartial ? ERRID.WRN_TypeConflictButMerged6 : ERRID.ERR_TypeConflict6);
					object objectValue = RuntimeHelpers.GetObjectValue(SymbolExtensions.ToErrorMessageArgument(ContainingSymbol, eRRID));
					string text = GetTypeIdentifierToken(firstNode).ToString();
					string kindText = SymbolExtensions.GetKindText(this);
					Binder.ReportDiagnostic(diagBag, id, eRRID, kindText, id.ToString(), kindText, text, SymbolExtensions.GetKindText(ContainingSymbol), objectValue);
				}
			}
		}

		private void AddEnumMembers(EnumBlockSyntax syntax, Binder bodyBinder, BindingDiagnosticBag diagnostics, MembersAndInitializersBuilder members)
		{
			SynthesizedFieldSymbol sym = new SynthesizedFieldSymbol(this, this, EnumUnderlyingType, "value__", Accessibility.Public, isReadOnly: false, isShared: false, isSpecialNameAndRuntimeSpecial: true);
			AddMember(sym, bodyBinder, members, omitDiagnostics: false);
			SourceEnumConstantSymbol sourceEnumConstantSymbol = null;
			int num = 0;
			if (syntax.Members.Count == 0)
			{
				Binder.ReportDiagnostic(diagnostics, syntax.EnumStatement.Identifier, ERRID.ERR_BadEmptyEnum1, syntax.EnumStatement.Identifier.ValueText);
				return;
			}
			SyntaxList<StatementSyntax>.Enumerator enumerator = syntax.Members.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StatementSyntax current = enumerator.Current;
				if (current.Kind() == SyntaxKind.EnumMemberDeclaration)
				{
					EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = (EnumMemberDeclarationSyntax)current;
					EqualsValueSyntax initializer = enumMemberDeclarationSyntax.Initializer;
					SourceEnumConstantSymbol sourceEnumConstantSymbol2 = ((initializer == null) ? SourceEnumConstantSymbol.CreateImplicitValuedConstant(this, bodyBinder, enumMemberDeclarationSyntax, sourceEnumConstantSymbol, num, diagnostics) : SourceEnumConstantSymbol.CreateExplicitValuedConstant(this, bodyBinder, enumMemberDeclarationSyntax, diagnostics));
					if (initializer != null || (object)sourceEnumConstantSymbol == null)
					{
						sourceEnumConstantSymbol = sourceEnumConstantSymbol2;
						num = 1;
					}
					else
					{
						num++;
					}
					AddMember(sourceEnumConstantSymbol2, bodyBinder, members, omitDiagnostics: false);
				}
			}
		}

		internal void BindTypeParameterConstraints(SourceTypeParameterOnTypeSymbol typeParameter, out VarianceKind variance, out ImmutableArray<TypeParameterConstraint> constraints, BindingDiagnosticBag diagnostics)
		{
			GetTypeMembersDictionary();
			TypeParameterInfo info = default(TypeParameterInfo);
			ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				SyntaxTree syntaxTree = current.SyntaxTree;
				VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(current);
				bool allowVarianceSpecifier = false;
				SyntaxKind syntaxKind = visualBasicSyntax.Kind();
				if (syntaxKind == SyntaxKind.InterfaceBlock || syntaxKind - 98 <= SyntaxKind.List)
				{
					allowVarianceSpecifier = true;
				}
				TypeParameterListSyntax typeParameterListSyntax = GetTypeParameterListSyntax(visualBasicSyntax);
				CreateTypeParameterInfoInPart(syntaxTree, typeParameter, typeParameterListSyntax, allowVarianceSpecifier, ref info, diagnostics);
			}
			variance = info.Variance;
			constraints = info.Constraints;
		}

		private void CreateTypeParameterInfoInPart(SyntaxTree tree, SourceTypeParameterOnTypeSymbol typeParameter, TypeParameterListSyntax typeParamListSyntax, bool allowVarianceSpecifier, ref TypeParameterInfo info, BindingDiagnosticBag diagBag)
		{
			Binder binder = CreateLocationSpecificBinderForType(tree, BindingLocation.GenericConstraintsClause);
			TypeParameterSyntax typeParameterSyntax = typeParamListSyntax.Parameters[typeParameter.Ordinal];
			SyntaxToken identifier = typeParameterSyntax.Identifier;
			Binder.DisallowTypeCharacter(identifier, diagBag, ERRID.ERR_TypeCharOnGenericParam);
			string valueText = identifier.ValueText;
			SyntaxToken varianceKeyword = typeParameterSyntax.VarianceKeyword;
			VarianceKind variance = VarianceKind.None;
			if (VisualBasicExtensions.Kind(varianceKeyword) != 0)
			{
				if (allowVarianceSpecifier)
				{
					variance = Binder.DecodeVariance(varianceKeyword);
				}
				else
				{
					Binder.ReportDiagnostic(diagBag, varianceKeyword, ERRID.ERR_VarianceDisallowedHere);
				}
			}
			ImmutableArray<TypeParameterConstraint> immutableArray = binder.BindTypeParameterConstraintClause(this, typeParameterSyntax.TypeParameterConstraintClause, diagBag);
			if (info.Initialized)
			{
				if (!CaseInsensitiveComparison.Equals(typeParameter.Name, valueText))
				{
					Binder.ReportDiagnostic(diagBag, identifier, ERRID.ERR_PartialTypeTypeParamNameMismatch3, valueText, typeParameter.Name, base.Name);
				}
				if (!HaveSameConstraints(info.Constraints, immutableArray))
				{
					Binder.ReportDiagnostic(diagBag, identifier, ERRID.ERR_PartialTypeConstraintMismatch1, base.Name);
				}
			}
			else
			{
				info = new TypeParameterInfo(variance, immutableArray);
			}
		}

		private static bool HaveSameConstraints(ImmutableArray<TypeParameterConstraint> constraints1, ImmutableArray<TypeParameterConstraint> constraints2)
		{
			int length = constraints1.Length;
			int length2 = constraints2.Length;
			if (length != length2)
			{
				return false;
			}
			if (length == 0 && length2 == 0)
			{
				return true;
			}
			if (GetConstraintKind(constraints1) != GetConstraintKind(constraints2))
			{
				return false;
			}
			HashSet<TypeSymbol> hashSet = new HashSet<TypeSymbol>();
			ImmutableArray<TypeParameterConstraint>.Enumerator enumerator = constraints1.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol typeConstraint = enumerator.Current.TypeConstraint;
				if ((object)typeConstraint != null)
				{
					hashSet.Add(typeConstraint);
				}
			}
			ImmutableArray<TypeParameterConstraint>.Enumerator enumerator2 = constraints2.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				TypeSymbol typeConstraint2 = enumerator2.Current.TypeConstraint;
				if ((object)typeConstraint2 != null && !hashSet.Contains(typeConstraint2))
				{
					return false;
				}
			}
			return true;
		}

		private static TypeParameterConstraintKind GetConstraintKind(ImmutableArray<TypeParameterConstraint> constraints)
		{
			TypeParameterConstraintKind typeParameterConstraintKind = TypeParameterConstraintKind.None;
			ImmutableArray<TypeParameterConstraint>.Enumerator enumerator = constraints.GetEnumerator();
			while (enumerator.MoveNext())
			{
				typeParameterConstraintKind |= enumerator.Current.Kind;
			}
			return typeParameterConstraintKind;
		}

		private ImmutableArray<TypeParameterSymbol> MakeTypeParameters()
		{
			int arity = base.TypeDeclaration.Arity;
			if (arity == 0)
			{
				return ImmutableArray<TypeParameterSymbol>.Empty;
			}
			TypeParameterSymbol[] array = new TypeParameterSymbol[arity - 1 + 1];
			int num = arity - 1;
			for (int i = 0; i <= num; i++)
			{
				ArrayBuilder<SyntaxReference> instance = ArrayBuilder<SyntaxReference>.GetInstance();
				string text = null;
				ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxReference current = enumerator.Current;
					SyntaxTree syntaxTree = current.SyntaxTree;
					TypeParameterSyntax typeParameterSyntax = GetTypeParameterListSyntax(VisualBasicExtensions.GetVisualBasicSyntax(current)).Parameters[i];
					if (text == null)
					{
						text = typeParameterSyntax.Identifier.ValueText;
					}
					instance.Add(syntaxTree.GetReference(typeParameterSyntax));
				}
				array[i] = new SourceTypeParameterOnTypeSymbol(this, i, text, instance.ToImmutableAndFree());
			}
			return array.AsImmutableOrNull();
		}

		private static TypeParameterListSyntax GetTypeParameterListSyntax(VisualBasicSyntaxNode syntax)
		{
			switch (syntax.Kind())
			{
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
				return ((TypeBlockSyntax)syntax).BlockStatement.TypeParameterList;
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
				return ((DelegateStatementSyntax)syntax).TypeParameterList;
			default:
				return null;
			}
		}

		internal void CheckForDuplicateTypeParameters(ImmutableArray<TypeParameterSymbol> typeParameters, BindingDiagnosticBag diagBag)
		{
			if (typeParameters.IsDefault)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>(CaseInsensitiveComparison.Comparer);
			int num = typeParameters.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeParameterSymbol typeParameterSymbol = typeParameters[i];
				if (!hashSet.Contains(typeParameterSymbol.Name))
				{
					hashSet.Add(typeParameterSymbol.Name);
					if (ShadowsTypeParameter(typeParameterSymbol))
					{
						Binder.ReportDiagnostic(diagBag, typeParameterSymbol.Locations[0], ERRID.WRN_ShadowingGenericParamWithParam1, typeParameterSymbol.Name);
					}
				}
				else
				{
					Binder.ReportDiagnostic(diagBag, typeParameterSymbol.Locations[0], ERRID.ERR_DuplicateTypeParamName1, typeParameterSymbol.Name);
				}
			}
		}

		private bool ShadowsTypeParameter(TypeParameterSymbol typeParameter)
		{
			string name = typeParameter.Name;
			SourceNamedTypeSymbol sourceNamedTypeSymbol = ((typeParameter.TypeParameterKind != TypeParameterKind.Method) ? (ContainingType as SourceNamedTypeSymbol) : this);
			while ((object)sourceNamedTypeSymbol != null)
			{
				if (SymbolExtensions.MatchesAnyName(sourceNamedTypeSymbol.TypeParameters, name))
				{
					return true;
				}
				sourceNamedTypeSymbol = sourceNamedTypeSymbol.ContainingType as SourceNamedTypeSymbol;
			}
			return false;
		}

		private void MakeDeclaredBaseInPart(SyntaxTree tree, VisualBasicSyntaxNode syntaxNode, ref NamedTypeSymbol baseType, BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagBag)
		{
			Binder binder = CreateLocationSpecificBinderForType(tree, BindingLocation.BaseTypes);
			switch (syntaxNode.Kind())
			{
			case SyntaxKind.ClassBlock:
			{
				SyntaxList<InheritsStatementSyntax> inherits3 = ((TypeBlockSyntax)syntaxNode).Inherits;
				NamedTypeSymbol namedTypeSymbol = ValidateClassBase(inherits3, baseType, basesBeingResolved, binder, diagBag);
				if ((object)baseType == null)
				{
					baseType = namedTypeSymbol;
				}
				break;
			}
			case SyntaxKind.StructureBlock:
			{
				SyntaxList<InheritsStatementSyntax> inherits2 = ((TypeBlockSyntax)syntaxNode).Inherits;
				CheckNoBase(inherits2, ERRID.ERR_StructCantInherit, diagBag);
				break;
			}
			case SyntaxKind.ModuleBlock:
			{
				SyntaxList<InheritsStatementSyntax> inherits = ((TypeBlockSyntax)syntaxNode).Inherits;
				CheckNoBase(inherits, ERRID.ERR_ModuleCantInherit, diagBag);
				break;
			}
			case SyntaxKind.InterfaceBlock:
				break;
			}
		}

		private void MakeDeclaredInterfacesInPart(SyntaxTree tree, VisualBasicSyntaxNode syntaxNode, SetWithInsertionOrder<NamedTypeSymbol> interfaces, BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagBag)
		{
			Binder binder = CreateLocationSpecificBinderForType(tree, BindingLocation.BaseTypes);
			switch (syntaxNode.Kind())
			{
			case SyntaxKind.ClassBlock:
			{
				SyntaxList<ImplementsStatementSyntax> implements3 = ((TypeBlockSyntax)syntaxNode).Implements;
				ValidateImplementedInterfaces(implements3, interfaces, basesBeingResolved, binder, diagBag);
				break;
			}
			case SyntaxKind.StructureBlock:
			{
				SyntaxList<ImplementsStatementSyntax> implements2 = ((TypeBlockSyntax)syntaxNode).Implements;
				ValidateImplementedInterfaces(implements2, interfaces, basesBeingResolved, binder, diagBag);
				break;
			}
			case SyntaxKind.InterfaceBlock:
			{
				SyntaxList<InheritsStatementSyntax> inherits = ((TypeBlockSyntax)syntaxNode).Inherits;
				ValidateInheritedInterfaces(inherits, interfaces, basesBeingResolved, binder, diagBag);
				break;
			}
			case SyntaxKind.ModuleBlock:
			{
				SyntaxList<ImplementsStatementSyntax> implements = ((TypeBlockSyntax)syntaxNode).Implements;
				CheckNoBase(implements, ERRID.ERR_ModuleCantImplement, diagBag);
				break;
			}
			}
		}

		private void CheckNoBase<T>(SyntaxList<T> baseDeclList, ERRID errId, BindingDiagnosticBag diagBag) where T : InheritsOrImplementsStatementSyntax
		{
			if (baseDeclList.Count > 0)
			{
				SyntaxList<T>.Enumerator enumerator = baseDeclList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					Binder.ReportDiagnostic(diagBag, current, errId);
				}
			}
		}

		private NamedTypeSymbol ValidateClassBase(SyntaxList<InheritsStatementSyntax> inheritsSyntax, NamedTypeSymbol baseInOtherPartial, BasesBeingResolved basesBeingResolved, Binder binder, BindingDiagnosticBag diagBag)
		{
			if (inheritsSyntax.Count == 0)
			{
				return null;
			}
			basesBeingResolved = basesBeingResolved.PrependInheritsBeingResolved(this);
			binder = new BasesBeingResolvedBinder(binder, basesBeingResolved);
			TypeSyntax typeSyntax = null;
			SyntaxList<InheritsStatementSyntax>.Enumerator enumerator = inheritsSyntax.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InheritsStatementSyntax current = enumerator.Current;
				if (current.Kind() == SyntaxKind.InheritsStatement)
				{
					InheritsStatementSyntax inheritsStatementSyntax = current;
					if (typeSyntax != null || inheritsStatementSyntax.Types.Count > 1)
					{
						Binder.ReportDiagnostic(diagBag, inheritsStatementSyntax, ERRID.ERR_MultipleExtends);
					}
					if (typeSyntax == null && inheritsStatementSyntax.Types.Count > 0)
					{
						typeSyntax = inheritsStatementSyntax.Types[0];
					}
				}
			}
			if (typeSyntax == null)
			{
				return null;
			}
			TypeSymbol typeSymbol = binder.BindTypeSyntax(typeSyntax, diagBag, suppressUseSiteError: true, inGetTypeContext: false, resolvingBaseType: true);
			if ((object)typeSymbol == null)
			{
				return null;
			}
			switch (typeSymbol.TypeKind)
			{
			case TypeKind.TypeParameter:
				Binder.ReportDiagnostic(diagBag, typeSyntax, ERRID.ERR_GenericParamBase2, "Class", base.Name);
				return null;
			case TypeKind.Array:
			case TypeKind.Delegate:
			case TypeKind.Enum:
			case TypeKind.Interface:
			case TypeKind.Module:
			case TypeKind.Struct:
				Binder.ReportDiagnostic(diagBag, typeSyntax, ERRID.ERR_InheritsFromNonClass);
				return null;
			case TypeKind.Unknown:
			case TypeKind.Error:
				return (NamedTypeSymbol)typeSymbol;
			case TypeKind.Class:
				if (IsRestrictedBaseClass(typeSymbol.SpecialType))
				{
					Binder.ReportDiagnostic(diagBag, typeSyntax, ERRID.ERR_InheritsFromRestrictedType1, typeSymbol);
					return null;
				}
				if (((NamedTypeSymbol)typeSymbol).IsNotInheritable)
				{
					Binder.ReportDiagnostic(diagBag, typeSyntax, ERRID.ERR_InheritsFromCantInherit3, base.Name, typeSymbol.Name, SymbolExtensions.GetKindText(typeSymbol));
					return null;
				}
				break;
			}
			if ((object)baseInOtherPartial != null)
			{
				if (!typeSymbol.Equals(baseInOtherPartial))
				{
					Binder.ReportDiagnostic(diagBag, typeSyntax, ERRID.ERR_BaseMismatchForPartialClass3, typeSymbol, base.Name, baseInOtherPartial);
					return null;
				}
			}
			else if (!TypeSymbolExtensions.IsErrorType(typeSymbol))
			{
				AccessCheck.VerifyAccessExposureOfBaseClassOrInterface(this, typeSyntax, typeSymbol, diagBag);
			}
			return (NamedTypeSymbol)typeSymbol;
		}

		private void ValidateInheritedInterfaces(SyntaxList<InheritsStatementSyntax> baseSyntax, SetWithInsertionOrder<NamedTypeSymbol> basesInOtherPartials, BasesBeingResolved basesBeingResolved, Binder binder, BindingDiagnosticBag diagBag)
		{
			if (baseSyntax.Count == 0)
			{
				return;
			}
			basesBeingResolved = basesBeingResolved.PrependInheritsBeingResolved(this);
			binder = new BasesBeingResolvedBinder(binder, basesBeingResolved);
			HashSet<NamedTypeSymbol> hashSet = new HashSet<NamedTypeSymbol>();
			SyntaxList<InheritsStatementSyntax>.Enumerator enumerator = baseSyntax.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SeparatedSyntaxList<TypeSyntax>.Enumerator enumerator2 = enumerator.Current.Types.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TypeSyntax current = enumerator2.Current;
					TypeSymbol typeSymbol = binder.BindTypeSyntax(current, diagBag, suppressUseSiteError: true);
					NamedTypeSymbol namedTypeSymbol = typeSymbol as NamedTypeSymbol;
					if ((object)namedTypeSymbol != null && hashSet.Contains(namedTypeSymbol))
					{
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateInInherits1, typeSymbol);
						continue;
					}
					if ((object)namedTypeSymbol != null)
					{
						hashSet.Add(namedTypeSymbol);
					}
					switch (typeSymbol.TypeKind)
					{
					case TypeKind.TypeParameter:
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_GenericParamBase2, "Interface", base.Name);
						break;
					case TypeKind.Error:
					case TypeKind.Interface:
						basesInOtherPartials.Add(namedTypeSymbol);
						if (!TypeSymbolExtensions.IsErrorType(typeSymbol))
						{
							AccessCheck.VerifyAccessExposureOfBaseClassOrInterface(this, current, typeSymbol, diagBag);
						}
						break;
					default:
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_InheritsFromNonInterface);
						break;
					case TypeKind.Unknown:
						break;
					}
				}
			}
		}

		private void ValidateImplementedInterfaces(SyntaxList<ImplementsStatementSyntax> baseSyntax, SetWithInsertionOrder<NamedTypeSymbol> basesInOtherPartials, BasesBeingResolved basesBeingResolved, Binder binder, BindingDiagnosticBag diagBag)
		{
			if (baseSyntax.Count == 0)
			{
				return;
			}
			basesBeingResolved = basesBeingResolved.PrependImplementsBeingResolved(this);
			binder = new BasesBeingResolvedBinder(binder, basesBeingResolved);
			HashSet<TypeSymbol> hashSet = new HashSet<TypeSymbol>();
			SyntaxList<ImplementsStatementSyntax>.Enumerator enumerator = baseSyntax.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SeparatedSyntaxList<TypeSyntax>.Enumerator enumerator2 = enumerator.Current.Types.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					TypeSyntax current = enumerator2.Current;
					TypeSymbol typeSymbol = binder.BindTypeSyntax(current, diagBag, suppressUseSiteError: true);
					if (!hashSet.Add(typeSymbol))
					{
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_InterfaceImplementedTwice1, typeSymbol);
						continue;
					}
					switch (typeSymbol.TypeKind)
					{
					case TypeKind.TypeParameter:
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_ImplementsGenericParam, "Interface", base.Name);
						break;
					case TypeKind.Error:
					case TypeKind.Interface:
						basesInOtherPartials.Add((NamedTypeSymbol)typeSymbol);
						break;
					default:
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_BadImplementsType);
						break;
					case TypeKind.Unknown:
						break;
					}
				}
			}
		}

		private bool IsRestrictedBaseClass(SpecialType type)
		{
			SpecialType specialType = type;
			if ((uint)(specialType - 2) <= 3u || specialType == SpecialType.System_Array)
			{
				return true;
			}
			return false;
		}

		private NamedTypeSymbol AsPeOrRetargetingType(TypeSymbol potentialBaseType)
		{
			NamedTypeSymbol namedTypeSymbol = potentialBaseType as PENamedTypeSymbol;
			if ((object)namedTypeSymbol == null)
			{
				namedTypeSymbol = potentialBaseType as RetargetingNamedTypeSymbol;
			}
			return namedTypeSymbol;
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			if (ContainingSymbol is SourceNamedTypeSymbol sourceNamedTypeSymbol)
			{
				sourceNamedTypeSymbol.GetDeclaredBaseSafe(basesBeingResolved.PrependInheritsBeingResolved(this));
			}
			NamedTypeSymbol baseType = null;
			ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = base.TypeDeclaration.Declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SingleTypeDeclaration current = enumerator.Current;
				if (current.HasBaseDeclarations)
				{
					SyntaxReference syntaxReference = current.SyntaxReference;
					MakeDeclaredBaseInPart(syntaxReference.SyntaxTree, VisualBasicExtensions.GetVisualBasicSyntax(syntaxReference), ref baseType, basesBeingResolved, diagnostics);
				}
			}
			return baseType;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			SourceNamedTypeSymbol sourceNamedTypeSymbol = ContainingSymbol as SourceNamedTypeSymbol;
			if (IsInterface && (object)sourceNamedTypeSymbol != null && sourceNamedTypeSymbol.IsInterface)
			{
				sourceNamedTypeSymbol.GetDeclaredBaseInterfacesSafe(basesBeingResolved.PrependInheritsBeingResolved(this));
			}
			SetWithInsertionOrder<NamedTypeSymbol> setWithInsertionOrder = new SetWithInsertionOrder<NamedTypeSymbol>();
			ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				MakeDeclaredInterfacesInPart(current.SyntaxTree, VisualBasicExtensions.GetVisualBasicSyntax(current), setWithInsertionOrder, basesBeingResolved, diagnostics);
			}
			return setWithInsertionOrder.AsImmutable();
		}

		private Location GetInheritsLocation(NamedTypeSymbol @base)
		{
			return GetInheritsOrImplementsLocation(@base, getInherits: true);
		}

		protected override Location GetInheritsOrImplementsLocation(NamedTypeSymbol @base, bool getInherits)
		{
			Location location = null;
			ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				TypeBlockSyntax typeBlockSyntax = (TypeBlockSyntax)current.GetSyntax();
				object obj;
				if (!getInherits)
				{
					obj = typeBlockSyntax.Implements;
				}
				else
				{
					IEnumerable<InheritsOrImplementsStatementSyntax> enumerable = typeBlockSyntax.Inherits;
					obj = enumerable;
				}
				IEnumerable<InheritsOrImplementsStatementSyntax> enumerable2 = (IEnumerable<InheritsOrImplementsStatementSyntax>)obj;
				Binder containingBinder = CreateLocationSpecificBinderForType(current.SyntaxTree, BindingLocation.BaseTypes);
				BasesBeingResolved basesBeingResolved = default(BasesBeingResolved);
				basesBeingResolved = ((!getInherits) ? basesBeingResolved.PrependImplementsBeingResolved(this) : basesBeingResolved.PrependInheritsBeingResolved(this));
				containingBinder = new BasesBeingResolvedBinder(containingBinder, basesBeingResolved);
				foreach (InheritsOrImplementsStatementSyntax item in enumerable2)
				{
					if ((object)location == null)
					{
						location = item.GetLocation();
					}
					SeparatedSyntaxList<TypeSyntax>.Enumerator enumerator3 = (getInherits ? ((InheritsStatementSyntax)item).Types : ((ImplementsStatementSyntax)item).Types).GetEnumerator();
					while (enumerator3.MoveNext())
					{
						TypeSyntax current3 = enumerator3.Current;
						if (TypeSymbol.Equals(containingBinder.BindTypeSyntax(current3, BindingDiagnosticBag.Discarded, suppressUseSiteError: true), @base, TypeCompareKind.ConsiderEverything))
						{
							return current3.GetLocation();
						}
					}
				}
			}
			return location;
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			_ = DeclaringCompilation;
			NamedTypeSymbol declaredBase = GetDeclaredBase(default(BasesBeingResolved));
			if ((object)declaredBase != null)
			{
				DiagnosticInfo diagnosticInfo = m_baseCycleDiagnosticInfo ?? BaseTypeAnalysis.GetDependenceDiagnosticForBase(this, declaredBase);
				if (diagnosticInfo != null)
				{
					Location inheritsLocation = GetInheritsLocation(declaredBase);
					diagnostics.Add(new VBDiagnostic(diagnosticInfo, inheritsLocation));
					return new ExtendedErrorTypeSymbol(diagnosticInfo);
				}
			}
			NamedTypeSymbol namedTypeSymbol = declaredBase;
			if ((object)namedTypeSymbol == null && SpecialType != SpecialType.System_Object)
			{
				switch (TypeKind)
				{
				case TypeKind.Submission:
					ReportUseSiteInfoForBaseType(DeclaringCompilation.GetSpecialType(SpecialType.System_Object), declaredBase, diagnostics);
					namedTypeSymbol = null;
					break;
				case TypeKind.Class:
					namedTypeSymbol = GetSpecialType(SpecialType.System_Object);
					break;
				case TypeKind.Interface:
					namedTypeSymbol = null;
					break;
				case TypeKind.Enum:
					namedTypeSymbol = GetSpecialType(SpecialType.System_Enum);
					break;
				case TypeKind.Struct:
					namedTypeSymbol = GetSpecialType(SpecialType.System_ValueType);
					break;
				case TypeKind.Delegate:
					namedTypeSymbol = GetSpecialType(SpecialType.System_MulticastDelegate);
					break;
				case TypeKind.Module:
					namedTypeSymbol = GetSpecialType(SpecialType.System_Object);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(TypeKind);
				}
			}
			if ((object)namedTypeSymbol != null)
			{
				ReportUseSiteInfoForBaseType(namedTypeSymbol, declaredBase, diagnostics);
			}
			return namedTypeSymbol;
		}

		private NamedTypeSymbol GetSpecialType(SpecialType type)
		{
			return ContainingModule.ContainingAssembly.GetSpecialType(type);
		}

		private void ReportUseSiteInfoForBaseType(NamedTypeSymbol baseType, NamedTypeSymbol declaredBase, BindingDiagnosticBag diagnostics)
		{
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
			NamedTypeSymbol namedTypeSymbol = baseType;
			while (namedTypeSymbol.DeclaringCompilation != DeclaringCompilation)
			{
				TypeSymbolExtensions.AddUseSiteInfo(namedTypeSymbol, ref useSiteInfo);
				namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
				if ((object)namedTypeSymbol == null)
				{
					break;
				}
			}
			if (!useSiteInfo.Diagnostics.IsNullOrEmpty())
			{
				Location location;
				if ((object)declaredBase == baseType)
				{
					location = GetInheritsLocation(baseType);
				}
				else
				{
					VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(base.SyntaxReferences.First());
					location = ((visualBasicSyntax.Kind() == SyntaxKind.CompilationUnit || visualBasicSyntax.Kind() == SyntaxKind.NamespaceBlock) ? base.Locations[0] : GetTypeIdentifierToken(visualBasicSyntax).GetLocation());
				}
				diagnostics.Add(location, useSiteInfo);
			}
			else
			{
				diagnostics.AddDependencies(useSiteInfo);
			}
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<NamedTypeSymbol> declaredInterfacesNoUseSiteDiagnostics = GetDeclaredInterfacesNoUseSiteDiagnostics(default(BasesBeingResolved));
			bool flag = TypeSymbolExtensions.IsInterfaceType(this);
			ArrayBuilder<NamedTypeSymbol> arrayBuilder = (flag ? ArrayBuilder<NamedTypeSymbol>.GetInstance() : null);
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = declaredInterfacesNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol current = enumerator.Current;
				DiagnosticInfo diagnosticInfo = ((flag && !TypeSymbolExtensions.IsErrorType(current)) ? BaseTypeAnalysis.GetDependenceDiagnosticForBase(this, current) : null);
				if (diagnosticInfo != null)
				{
					Location inheritsLocation = GetInheritsLocation(current);
					diagnostics.Add(new VBDiagnostic(diagnosticInfo, inheritsLocation));
					arrayBuilder.Add(new ExtendedErrorTypeSymbol(diagnosticInfo));
					continue;
				}
				if (!TypeSymbolExtensions.IsErrorType(current))
				{
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
					if (current.DeclaringCompilation != DeclaringCompilation)
					{
						TypeSymbolExtensions.AddUseSiteInfo(current, ref useSiteInfo);
						ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = current.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							NamedTypeSymbol current2 = enumerator2.Current;
							if (current2.DeclaringCompilation != DeclaringCompilation)
							{
								TypeSymbolExtensions.AddUseSiteInfo(current2, ref useSiteInfo);
							}
						}
					}
					if (!useSiteInfo.Diagnostics.IsNullOrEmpty())
					{
						Location location = (flag ? GetInheritsLocation(current) : GetInheritsOrImplementsLocation(current, getInherits: false));
						diagnostics.Add(location, useSiteInfo);
					}
					else
					{
						diagnostics.AddDependencies(useSiteInfo);
					}
				}
				if (flag)
				{
					arrayBuilder.Add(current);
				}
			}
			if (!flag)
			{
				return declaredInterfacesNoUseSiteDiagnostics;
			}
			return arrayBuilder.ToImmutableAndFree();
		}

		internal override NamedTypeSymbol GetDirectBaseTypeNoUseSiteDiagnostics(BasesBeingResolved basesBeingResolved)
		{
			if (TypeKind == TypeKind.Enum)
			{
				return GetSpecialType(SpecialType.System_Enum);
			}
			if (TypeKind == TypeKind.Delegate)
			{
				return GetSpecialType(SpecialType.System_MulticastDelegate);
			}
			if (basesBeingResolved.InheritsBeingResolvedOpt == null)
			{
				return base.BaseTypeNoUseSiteDiagnostics;
			}
			return GetDeclaredBaseSafe(basesBeingResolved);
		}

		private NamedTypeSymbol GetDeclaredBaseSafe(BasesBeingResolved basesBeingResolved)
		{
			if (m_baseCycleDiagnosticInfo != null)
			{
				return null;
			}
			if ((object)this == basesBeingResolved.InheritsBeingResolvedOpt.Head)
			{
				return null;
			}
			DiagnosticInfo dependenceDiagnosticForBase = BaseTypeAnalysis.GetDependenceDiagnosticForBase(this, basesBeingResolved);
			if (dependenceDiagnosticForBase == null)
			{
				NamedTypeSymbol declaredBase = GetDeclaredBase(basesBeingResolved);
				return (m_baseCycleDiagnosticInfo == null) ? declaredBase : null;
			}
			Interlocked.CompareExchange(ref m_baseCycleDiagnosticInfo, dependenceDiagnosticForBase, null);
			return null;
		}

		internal override ImmutableArray<NamedTypeSymbol> GetDeclaredBaseInterfacesSafe(BasesBeingResolved basesBeingResolved)
		{
			ImmutableArray<NamedTypeSymbol> result;
			if (m_baseCycleDiagnosticInfo != null)
			{
				result = default(ImmutableArray<NamedTypeSymbol>);
			}
			else if ((object)this == basesBeingResolved.InheritsBeingResolvedOpt.Head)
			{
				result = default(ImmutableArray<NamedTypeSymbol>);
			}
			else
			{
				DiagnosticInfo dependenceDiagnosticForBase = BaseTypeAnalysis.GetDependenceDiagnosticForBase(this, basesBeingResolved);
				if (dependenceDiagnosticForBase == null)
				{
					ImmutableArray<NamedTypeSymbol> declaredInterfacesNoUseSiteDiagnostics = GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved);
					return (m_baseCycleDiagnosticInfo == null) ? declaredInterfacesNoUseSiteDiagnostics : ImmutableArray<NamedTypeSymbol>.Empty;
				}
				Interlocked.CompareExchange(ref m_baseCycleDiagnosticInfo, dependenceDiagnosticForBase, null);
				result = default(ImmutableArray<NamedTypeSymbol>);
			}
			return result;
		}

		private void CheckBaseConstraints()
		{
			if (((uint)m_lazyState & 4u) != 0)
			{
				return;
			}
			BindingDiagnosticBag bindingDiagnosticBag = null;
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = base.BaseTypeNoUseSiteDiagnostics;
			if ((object)baseTypeNoUseSiteDiagnostics != null)
			{
				SingleTypeDeclaration singleTypeDeclaration = FirstDeclarationWithExplicitBases();
				if (singleTypeDeclaration != null)
				{
					Location nameLocation = singleTypeDeclaration.NameLocation;
					bindingDiagnosticBag = BindingDiagnosticBag.GetInstance();
					ConstraintsHelper.CheckAllConstraints(baseTypeNoUseSiteDiagnostics, nameLocation, bindingDiagnosticBag, new CompoundUseSiteInfo<AssemblySymbol>(bindingDiagnosticBag, m_containingModule.ContainingAssembly));
					if (base.IsGenericType)
					{
						CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(bindingDiagnosticBag, m_containingModule.ContainingAssembly);
						bool num = TypeSymbolExtensions.IsBaseTypeOf(DeclaringCompilation.GetWellKnownType(WellKnownType.System_Attribute), baseTypeNoUseSiteDiagnostics, ref useSiteInfo);
						bindingDiagnosticBag.Add(nameLocation, useSiteInfo);
						if (num)
						{
							Binder.ReportDiagnostic(bindingDiagnosticBag, nameLocation, ERRID.ERR_GenericClassCannotInheritAttr);
						}
					}
				}
			}
			m_containingModule.AtomicSetFlagAndStoreDiagnostics(ref m_lazyState, 4, 0, bindingDiagnosticBag);
			bindingDiagnosticBag?.Free();
		}

		private void CheckInterfacesConstraints()
		{
			if (((uint)m_lazyState & 8u) != 0)
			{
				return;
			}
			BindingDiagnosticBag bindingDiagnosticBag = null;
			ImmutableArray<NamedTypeSymbol> interfacesNoUseSiteDiagnostics = base.InterfacesNoUseSiteDiagnostics;
			if (!interfacesNoUseSiteDiagnostics.IsEmpty)
			{
				SingleTypeDeclaration singleTypeDeclaration = FirstDeclarationWithExplicitInterfaces();
				if (singleTypeDeclaration != null)
				{
					Location nameLocation = singleTypeDeclaration.NameLocation;
					bindingDiagnosticBag = BindingDiagnosticBag.GetInstance();
					ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = interfacesNoUseSiteDiagnostics.GetEnumerator();
					while (enumerator.MoveNext())
					{
						ConstraintsHelper.CheckAllConstraints(enumerator.Current, nameLocation, bindingDiagnosticBag, new CompoundUseSiteInfo<AssemblySymbol>(bindingDiagnosticBag, m_containingModule.ContainingAssembly));
					}
				}
			}
			if (m_containingModule.AtomicSetFlagAndStoreDiagnostics(ref m_lazyState, 8, 0, bindingDiagnosticBag))
			{
				DeclaringCompilation.SymbolDeclaredEvent(this);
			}
			bindingDiagnosticBag?.Free();
		}

		private SingleTypeDeclaration FirstDeclarationWithExplicitBases()
		{
			ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = base.TypeDeclaration.Declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SingleTypeDeclaration current = enumerator.Current;
				VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(current.SyntaxReference);
				SyntaxKind syntaxKind = visualBasicSyntax.Kind();
				if (syntaxKind == SyntaxKind.ClassBlock && ((TypeBlockSyntax)visualBasicSyntax).Inherits.Count > 0)
				{
					return current;
				}
			}
			return null;
		}

		private SingleTypeDeclaration FirstDeclarationWithExplicitInterfaces()
		{
			ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = base.TypeDeclaration.Declarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SingleTypeDeclaration current = enumerator.Current;
				VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(current.SyntaxReference);
				switch (visualBasicSyntax.Kind())
				{
				case SyntaxKind.StructureBlock:
				case SyntaxKind.ClassBlock:
					if (((TypeBlockSyntax)visualBasicSyntax).Implements.Count > 0)
					{
						return current;
					}
					break;
				case SyntaxKind.InterfaceBlock:
					if (((TypeBlockSyntax)visualBasicSyntax).Inherits.Count > 0)
					{
						return current;
					}
					break;
				}
			}
			return null;
		}

		private NamedTypeSymbol BindEnumUnderlyingType(EnumBlockSyntax syntax, Binder bodyBinder, BindingDiagnosticBag diagnostics)
		{
			AsClauseSyntax underlyingType = syntax.EnumStatement.UnderlyingType;
			if (underlyingType != null && !SyntaxExtensions.Type(underlyingType).IsMissing)
			{
				TypeSymbol typeSymbol = bodyBinder.BindTypeSyntax(SyntaxExtensions.Type(underlyingType), diagnostics);
				if (TypeSymbolExtensions.IsValidEnumUnderlyingType(typeSymbol))
				{
					return (NamedTypeSymbol)typeSymbol;
				}
				Binder.ReportDiagnostic(diagnostics, SyntaxExtensions.Type(underlyingType), ERRID.ERR_InvalidEnumBase);
			}
			return bodyBinder.GetSpecialType(SpecialType.System_Int32, syntax.EnumStatement.Identifier, diagnostics);
		}

		private ImmutableArray<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			return base.TypeDeclaration.GetAttributeDeclarations();
		}

		private CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag()
		{
			if (m_lazyCustomAttributesBag == null || !m_lazyCustomAttributesBag.IsSealed)
			{
				LoadAndValidateAttributes(OneOrMany.Create(GetAttributeDeclarations()), ref m_lazyCustomAttributesBag);
			}
			return m_lazyCustomAttributesBag;
		}

		public sealed override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return GetAttributesBag().Attributes;
		}

		private CommonTypeWellKnownAttributeData GetDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = m_lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (CommonTypeWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		private bool DecodeIsExtensibleInterface()
		{
			if (TypeSymbolExtensions.IsInterfaceType(this))
			{
				TypeEarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = GetEarlyDecodedWellKnownAttributeData();
				if (earlyDecodedWellKnownAttributeData != null && earlyDecodedWellKnownAttributeData.HasAttributeForExtensibleInterface)
				{
					return true;
				}
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = base.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsExtensibleInterfaceNoUseSiteDiagnostics)
					{
						return true;
					}
				}
			}
			return false;
		}

		private TypeEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = m_lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (TypeEarlyWellKnownAttributeData)customAttributesBag.EarlyDecodedWellKnownAttributeData;
		}

		internal bool HasGuidAttribute()
		{
			return AttributeDataExtensions.IndexOfAttribute(GetAttributes(), this, AttributeDescription.GuidAttribute) > -1;
		}

		internal bool HasClassInterfaceAttribute()
		{
			return AttributeDataExtensions.IndexOfAttribute(GetAttributes(), this, AttributeDescription.ClassInterfaceAttribute) > -1;
		}

		internal bool HasComSourceInterfacesAttribute()
		{
			return AttributeDataExtensions.IndexOfAttribute(GetAttributes(), this, AttributeDescription.ComSourceInterfacesAttribute) > -1;
		}

		internal override VisualBasicAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
		{
			bool generatedDiagnostics = false;
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.VisualBasicEmbeddedAttribute))
			{
				SourceAttributeData attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
				if (!attribute.HasErrors)
				{
					arguments.GetOrCreateData<TypeEarlyWellKnownAttributeData>().HasVisualBasicEmbeddedAttribute = true;
					return (!generatedDiagnostics) ? attribute : null;
				}
				return null;
			}
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CodeAnalysisEmbeddedAttribute))
			{
				SourceAttributeData attribute2 = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
				if (!attribute2.HasErrors)
				{
					arguments.GetOrCreateData<TypeEarlyWellKnownAttributeData>().HasCodeAnalysisEmbeddedAttribute = true;
					return (!generatedDiagnostics) ? attribute2 : null;
				}
				return null;
			}
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ComImportAttribute))
			{
				SourceAttributeData attribute3 = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
				if (!attribute3.HasErrors)
				{
					arguments.GetOrCreateData<TypeEarlyWellKnownAttributeData>().HasComImportAttribute = true;
					return (!generatedDiagnostics) ? attribute3 : null;
				}
				return null;
			}
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ConditionalAttribute))
			{
				SourceAttributeData attribute4 = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
				if (!attribute4.HasErrors)
				{
					string constructorArgument = attribute4.GetConstructorArgument<string>(0, SpecialType.System_String);
					arguments.GetOrCreateData<TypeEarlyWellKnownAttributeData>().AddConditionalSymbol(constructorArgument);
					return (!generatedDiagnostics) ? attribute4 : null;
				}
				return null;
			}
			VisualBasicAttributeData boundAttribute = null;
			ObsoleteAttributeData obsoleteData = null;
			if (EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out boundAttribute, out obsoleteData))
			{
				if (obsoleteData != null)
				{
					arguments.GetOrCreateData<TypeEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
				}
				return boundAttribute;
			}
			if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.AttributeUsageAttribute))
			{
				if (!arguments.HasDecodedData || ((TypeEarlyWellKnownAttributeData)arguments.DecodedData).AttributeUsageInfo.IsNull)
				{
					SourceAttributeData attribute5 = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
					if (!attribute5.HasErrors)
					{
						arguments.GetOrCreateData<TypeEarlyWellKnownAttributeData>().AttributeUsageInfo = attribute5.DecodeAttributeUsageAttribute();
						return (!generatedDiagnostics) ? attribute5 : null;
					}
				}
				return null;
			}
			if (TypeSymbolExtensions.IsInterfaceType(this))
			{
				if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.InterfaceTypeAttribute))
				{
					SourceAttributeData attribute6 = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
					if (!attribute6.HasErrors)
					{
						ComInterfaceType interfaceType = ComInterfaceType.InterfaceIsDual;
						if (attribute6.DecodeInterfaceTypeAttribute(out interfaceType) && (interfaceType & ComInterfaceType.InterfaceIsIDispatch) != 0)
						{
							arguments.GetOrCreateData<TypeEarlyWellKnownAttributeData>().HasAttributeForExtensibleInterface = true;
						}
						return (!generatedDiagnostics) ? attribute6 : null;
					}
					return null;
				}
				if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.TypeLibTypeAttribute))
				{
					SourceAttributeData attribute7 = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
					if (!attribute7.HasErrors)
					{
						if ((attribute7.DecodeTypeLibTypeAttribute() & Microsoft.Cci.TypeLibTypeFlags.FNonExtensible) == 0)
						{
							arguments.GetOrCreateData<TypeEarlyWellKnownAttributeData>().HasAttributeForExtensibleInterface = true;
						}
						return (!generatedDiagnostics) ? attribute7 : null;
					}
					return null;
				}
			}
			return base.EarlyDecodeWellKnownAttribute(ref arguments);
		}

		internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return GetEarlyDecodedWellKnownAttributeData()?.ConditionalSymbols ?? ImmutableArray<string>.Empty;
		}

		internal sealed override AttributeUsageInfo GetAttributeUsageInfo()
		{
			TypeEarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = GetEarlyDecodedWellKnownAttributeData();
			if (earlyDecodedWellKnownAttributeData != null && !earlyDecodedWellKnownAttributeData.AttributeUsageInfo.IsNull)
			{
				return earlyDecodedWellKnownAttributeData.AttributeUsageInfo;
			}
			return base.BaseTypeNoUseSiteDiagnostics?.GetAttributeUsageInfo() ?? AttributeUsageInfo.Default;
		}

		internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			CustomAttributesBag<VisualBasicAttributeData> attributesBag = GetAttributesBag();
			CommonTypeWellKnownAttributeData commonTypeWellKnownAttributeData = (CommonTypeWellKnownAttributeData)attributesBag.DecodedWellKnownAttributeData;
			if (commonTypeWellKnownAttributeData != null)
			{
				SecurityWellKnownAttributeData securityInformation = commonTypeWellKnownAttributeData.SecurityInformation;
				if (securityInformation != null)
				{
					return securityInformation.GetSecurityAttributes(attributesBag.Attributes);
				}
			}
			return SpecializedCollections.EmptyEnumerable<SecurityAttribute>();
		}

		internal sealed override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
			if (attribute.IsTargetAttribute(this, AttributeDescription.TupleElementNamesAttribute))
			{
				bindingDiagnosticBag.Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt!.Location);
			}
			bool flag = false;
			switch (TypeKind)
			{
			case TypeKind.Class:
				if (attribute.IsTargetAttribute(this, AttributeDescription.CaseInsensitiveExtensionAttribute))
				{
					bindingDiagnosticBag.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ExtensionOnlyAllowedOnModuleSubOrFunction), base.Locations[0]);
					flag = true;
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.VisualBasicComClassAttribute))
				{
					if (base.IsGenericType)
					{
						bindingDiagnosticBag.Add(ERRID.ERR_ComClassOnGeneric, base.Locations[0]);
					}
					else
					{
						Interlocked.CompareExchange(ref _comClassData, new ComClassData(attribute), null);
					}
					flag = true;
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.DefaultEventAttribute))
				{
					if (attribute.CommonConstructorArguments.Length == 1 && attribute.CommonConstructorArguments[0].Kind == TypedConstantKind.Primitive && attribute.CommonConstructorArguments[0].ValueInternal is string text && text.Length > 0 && !FindDefaultEvent(text))
					{
						bindingDiagnosticBag.Add(ERRID.ERR_DefaultEventNotFound1, arguments.AttributeSyntaxOpt!.GetLocation(), text);
					}
					flag = true;
				}
				break;
			case TypeKind.Interface:
				if (attribute.IsTargetAttribute(this, AttributeDescription.CoClassAttribute))
				{
					Interlocked.CompareExchange(value: (TypeSymbol)attribute.CommonConstructorArguments[0].ValueInternal, location1: ref _lazyCoClassType, comparand: ErrorTypeSymbol.UnknownResultType);
					flag = true;
				}
				break;
			case TypeKind.Module:
				if (ContainingSymbol.Kind == SymbolKind.Namespace && attribute.IsTargetAttribute(this, AttributeDescription.CaseInsensitiveExtensionAttribute))
				{
					SuppressExtensionAttributeSynthesis();
					flag = true;
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.VisualBasicComClassAttribute))
				{
					bindingDiagnosticBag.Add(ErrorFactory.ErrorInfo(ERRID.ERR_InvalidAttributeUsage2, AttributeDescription.VisualBasicComClassAttribute.Name, base.Name), base.Locations[0]);
					flag = true;
				}
				break;
			}
			if (!flag)
			{
				if (attribute.IsTargetAttribute(this, AttributeDescription.DefaultMemberAttribute))
				{
					arguments.GetOrCreateData<CommonTypeWellKnownAttributeData>().HasDefaultMemberAttribute = true;
					string right = attribute.DecodeDefaultMemberAttribute();
					string defaultPropertyName = DefaultPropertyName;
					if (!string.IsNullOrEmpty(defaultPropertyName) && !CaseInsensitiveComparison.Equals(defaultPropertyName, right))
					{
						bindingDiagnosticBag.Add(ERRID.ERR_ConflictDefaultPropertyAttribute, base.Locations[0], this);
					}
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.SerializableAttribute))
				{
					arguments.GetOrCreateData<CommonTypeWellKnownAttributeData>().HasSerializableAttribute = true;
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.ExcludeFromCodeCoverageAttribute))
				{
					arguments.GetOrCreateData<CommonTypeWellKnownAttributeData>().HasExcludeFromCodeCoverageAttribute = true;
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
				{
					arguments.GetOrCreateData<CommonTypeWellKnownAttributeData>().HasSpecialNameAttribute = true;
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.StructLayoutAttribute))
				{
					int defaultAutoLayoutSize = ((TypeKind == TypeKind.Struct) ? 1 : 0);
					AttributeData.DecodeStructLayoutAttribute<CommonTypeWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation>(ref arguments, base.DefaultMarshallingCharSet, defaultAutoLayoutSize, MessageProvider.Instance);
					if (base.IsGenericType)
					{
						bindingDiagnosticBag.Add(ERRID.ERR_StructLayoutAttributeNotAllowed, arguments.AttributeSyntaxOpt!.GetLocation(), this);
					}
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.SuppressUnmanagedCodeSecurityAttribute))
				{
					arguments.GetOrCreateData<CommonTypeWellKnownAttributeData>().HasSuppressUnmanagedCodeSecurityAttribute = true;
				}
				else if (attribute.IsSecurityAttribute(DeclaringCompilation))
				{
					attribute.DecodeSecurityAttribute<CommonTypeWellKnownAttributeData>(this, DeclaringCompilation, ref arguments);
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.ClassInterfaceAttribute))
				{
					attribute.DecodeClassInterfaceAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.InterfaceTypeAttribute))
				{
					attribute.DecodeInterfaceTypeAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.GuidAttribute))
				{
					attribute.DecodeGuidAttribute(arguments.AttributeSyntaxOpt, bindingDiagnosticBag);
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.WindowsRuntimeImportAttribute))
				{
					arguments.GetOrCreateData<CommonTypeWellKnownAttributeData>().HasWindowsRuntimeImportAttribute = true;
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.SecurityCriticalAttribute) || attribute.IsTargetAttribute(this, AttributeDescription.SecuritySafeCriticalAttribute))
				{
					arguments.GetOrCreateData<CommonTypeWellKnownAttributeData>().HasSecurityCriticalAttributes = true;
				}
				else if (_lazyIsExplicitDefinitionOfNoPiaLocalType == ThreeState.Unknown && attribute.IsTargetAttribute(this, AttributeDescription.TypeIdentifierAttribute))
				{
					_lazyIsExplicitDefinitionOfNoPiaLocalType = ThreeState.True;
				}
				else if (attribute.IsTargetAttribute(this, AttributeDescription.RequiredAttributeAttribute))
				{
					bindingDiagnosticBag.Add(ERRID.ERR_CantUseRequiredAttribute, arguments.AttributeSyntaxOpt!.GetLocation(), this);
				}
			}
			base.DecodeWellKnownAttribute(ref arguments);
		}

		private void CheckPresenceOfTypeIdentifierAttribute()
		{
			CustomAttributesBag<VisualBasicAttributeData> lazyCustomAttributesBag = m_lazyCustomAttributesBag;
			if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				return;
			}
			ImmutableArray<SyntaxList<AttributeListSyntax>>.Enumerator enumerator = GetAttributeDeclarations().GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxList<AttributeListSyntax> current = enumerator.Current;
				SourceFile sourceFile = base.ContainingSourceModule.TryGetSourceFile(current.Node!.SyntaxTree);
				SyntaxList<AttributeListSyntax>.Enumerator enumerator2 = current.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SeparatedSyntaxList<AttributeSyntax>.Enumerator enumerator3 = enumerator2.Current.Attributes.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						AttributeSyntax current2 = enumerator3.Current;
						if ((sourceFile.QuickAttributeChecker.CheckAttribute(current2) & QuickAttributes.TypeIdentifier) != 0)
						{
							GetAttributes();
							return;
						}
					}
				}
			}
		}

		private bool FindDefaultEvent(string eventName)
		{
			NamedTypeSymbol namedTypeSymbol = this;
			do
			{
				ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.GetMembers(eventName).GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.Event && (current.DeclaredAccessibility == Accessibility.Public || current.DeclaredAccessibility == Accessibility.Internal))
					{
						return true;
					}
				}
				namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
			}
			while ((object)namedTypeSymbol != null);
			return false;
		}

		internal override void PostDecodeWellKnownAttributes(ImmutableArray<VisualBasicAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
		{
			ValidateStandardModuleAttribute(diagnostics);
			base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
		}

		private void ValidateStandardModuleAttribute(BindingDiagnosticBag diagnostics)
		{
			if (TypeKind == TypeKind.Module)
			{
				Binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute__ctor, DeclaringCompilation, base.Locations[0], diagnostics);
			}
		}

		private bool HasInstanceFields()
		{
			ImmutableArray<Symbol> membersUnordered = GetMembersUnordered();
			int num = membersUnordered.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				Symbol symbol = membersUnordered[i];
				if (!symbol.IsShared & (symbol.Kind == SymbolKind.Field))
				{
					return true;
				}
			}
			return false;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			if (!string.IsNullOrEmpty(DefaultPropertyName) && !HasDefaultMemberAttribute())
			{
				NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_String);
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, ImmutableArray.Create(new TypedConstant(specialType, TypedConstantKind.Primitive, DefaultPropertyName))));
			}
			if (TypeKind == TypeKind.Module)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.Microsoft_VisualBasic_CompilerServices_StandardModuleAttribute__ctor));
			}
			if (_comClassData != null)
			{
				if (_comClassData.ClassId != null)
				{
					Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_GuidAttribute__ctor, ImmutableArray.Create(new TypedConstant(GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, _comClassData.ClassId))));
				}
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_ClassInterfaceAttribute__ctorClassInterfaceType, ImmutableArray.Create(new TypedConstant(GetSpecialType(SpecialType.System_Int32), TypedConstantKind.Enum, 0))));
				NamedTypeSymbol synthesizedEventInterface = _comClassData.GetSynthesizedEventInterface();
				if ((object)synthesizedEventInterface != null)
				{
					string text = synthesizedEventInterface.Name;
					NamedTypeSymbol namedTypeSymbol = this;
					NamedTypeSymbol containingType = namedTypeSymbol.ContainingType;
					while ((object)containingType != null)
					{
						text = namedTypeSymbol.Name + "+" + text;
						namedTypeSymbol = containingType;
						containingType = namedTypeSymbol.ContainingType;
					}
					text = namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat) + "+" + text;
					Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_InteropServices_ComSourceInterfacesAttribute__ctorString, ImmutableArray.Create(new TypedConstant(GetSpecialType(SpecialType.System_String), TypedConstantKind.Primitive, text))));
				}
			}
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = base.BaseTypeNoUseSiteDiagnostics;
			if ((object)baseTypeNoUseSiteDiagnostics != null && TypeSymbolExtensions.ContainsTupleNames(baseTypeNoUseSiteDiagnostics))
			{
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(baseTypeNoUseSiteDiagnostics));
			}
		}

		private bool HasDefaultMemberAttribute()
		{
			return ((CommonTypeWellKnownAttributeData)GetAttributesBag().DecodedWellKnownAttributeData)?.HasDefaultMemberAttribute ?? false;
		}

		internal SynthesizedOverridingWithEventsProperty GetOrAddWithEventsOverride(PropertySymbol baseProperty)
		{
			_Closure_0024__124_002D0 CS_0024_003C_003E8__locals0 = new _Closure_0024__124_002D0();
			CS_0024_003C_003E8__locals0._0024VB_0024Me = this;
			CS_0024_003C_003E8__locals0._0024VB_0024Local_baseProperty = baseProperty;
			ConcurrentDictionary<PropertySymbol, SynthesizedOverridingWithEventsProperty> lazyWithEventsOverrides = _lazyWithEventsOverrides;
			if (lazyWithEventsOverrides == null)
			{
				Interlocked.CompareExchange(ref _lazyWithEventsOverrides, new ConcurrentDictionary<PropertySymbol, SynthesizedOverridingWithEventsProperty>(), null);
				lazyWithEventsOverrides = _lazyWithEventsOverrides;
			}
			SynthesizedOverridingWithEventsProperty value = null;
			if (lazyWithEventsOverrides.TryGetValue(CS_0024_003C_003E8__locals0._0024VB_0024Local_baseProperty, out value))
			{
				return value;
			}
			return lazyWithEventsOverrides.GetOrAdd(CS_0024_003C_003E8__locals0._0024VB_0024Local_baseProperty, (PropertySymbol a0) => CS_0024_003C_003E8__locals0._Lambda_0024__0());
		}

		internal sealed override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			EnsureAllHandlesAreBound();
			ConcurrentDictionary<PropertySymbol, SynthesizedOverridingWithEventsProperty> lazyWithEventsOverrides = _lazyWithEventsOverrides;
			if (lazyWithEventsOverrides != null)
			{
				return lazyWithEventsOverrides.Values;
			}
			return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
		}

		private void EnsureAllHandlesAreBound()
		{
			if (_withEventsOverridesAreFrozen)
			{
				return;
			}
			ImmutableArray<Symbol>.Enumerator enumerator = GetMembersUnordered().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind == SymbolKind.Method)
				{
					_ = ((MethodSymbol)current).HandledEvents;
				}
			}
			_withEventsOverridesAreFrozen = true;
		}

		protected override void AddEntryPointIfNeeded(MembersAndInitializersBuilder membersBuilder)
		{
			if (TypeKind != TypeKind.Class || base.IsGenericType)
			{
				return;
			}
			string mainTypeName = DeclaringCompilation.Options.MainTypeName;
			if (mainTypeName == null || !CaseInsensitiveComparison.EndsWith(mainTypeName, base.Name) || !CaseInsensitiveComparison.Equals(mainTypeName, ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat)))
			{
				return;
			}
			NamedTypeSymbol wellKnownType = DeclaringCompilation.GetWellKnownType(WellKnownType.System_Windows_Forms_Form);
			if (TypeSymbolExtensions.IsErrorType(wellKnownType))
			{
				return;
			}
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			if (!TypeSymbolExtensions.IsOrDerivedFrom(this, wellKnownType, ref useSiteInfo))
			{
				return;
			}
			string key = "Main";
			if (membersBuilder.Members.ContainsKey(key) || GetTypeMembersDictionary().ContainsKey(key))
			{
				return;
			}
			ArrayBuilder<Symbol> value = null;
			bool flag = false;
			if (membersBuilder.Members.TryGetValue(".ctor", out value))
			{
				ArrayBuilder<Symbol>.Enumerator enumerator = value.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MethodSymbol methodSymbol = (MethodSymbol)enumerator.Current;
					if (methodSymbol.MethodKind == MethodKind.Constructor && methodSymbol.ParameterCount == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					ArrayBuilder<Symbol>.Enumerator enumerator2 = value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						MethodSymbol methodSymbol2 = (MethodSymbol)enumerator2.Current;
						if (methodSymbol2.MethodKind == MethodKind.Constructor && MethodSymbolExtensions.CanBeCalledWithNoParameters(methodSymbol2))
						{
							flag = true;
							break;
						}
					}
				}
			}
			if (flag)
			{
				SyntaxReference syntaxReference = base.SyntaxReferences.First();
				Binder binder = BinderBuilder.CreateBinderForType(base.ContainingSourceModule, syntaxReference.SyntaxTree, this);
				SynthesizedMainTypeEntryPoint sym = new SynthesizedMainTypeEntryPoint(VisualBasicExtensions.GetVisualBasicSyntax(syntaxReference), this);
				AddMember(sym, binder, membersBuilder, omitDiagnostics: true);
			}
		}

		private void PerformComClassAnalysis()
		{
			if (_comClassData != null)
			{
				_comClassData.PerformComClassAnalysis(this);
			}
		}

		internal override IEnumerable<INestedTypeDefinition> GetSynthesizedNestedTypes()
		{
			if (_comClassData == null)
			{
				return null;
			}
			ImmutableArray<NamedTypeSymbol> synthesizedInterfaces = _comClassData.GetSynthesizedInterfaces();
			if (synthesizedInterfaces.IsEmpty)
			{
				return null;
			}
			return synthesizedInterfaces.AsEnumerable();
		}

		internal override IEnumerable<NamedTypeSymbol> GetSynthesizedImplements()
		{
			if (_comClassData == null)
			{
				return null;
			}
			return _comClassData.GetSynthesizedImplements();
		}

		internal MethodSymbol GetCorrespondingComClassInterfaceMethod(MethodSymbol method)
		{
			GetAttributes();
			if (_comClassData == null)
			{
				return null;
			}
			_comClassData.PerformComClassAnalysis(this);
			return _comClassData.GetCorrespondingComClassInterfaceMethod(method);
		}

		protected override void AddGroupClassMembersIfNeeded(MembersAndInitializersBuilder membersBuilder, BindingDiagnosticBag diagnostics)
		{
			if (TypeKind != TypeKind.Class || base.IsGenericType)
			{
				return;
			}
			Binder binder = null;
			AttributeSyntax attributeSyntax = null;
			VisualBasicAttributeData myGroupCollectionAttributeData = GetMyGroupCollectionAttributeData(diagnostics, out binder, out attributeSyntax);
			if (myGroupCollectionAttributeData == null)
			{
				return;
			}
			char[] separator = new char[1] { ',' };
			char[] separator2 = new char[1] { '.' };
			string[] array = (myGroupCollectionAttributeData.GetConstructorArgument<string>(0, SpecialType.System_String) ?? "")!.Split(separator, StringSplitOptions.None);
			string[] array2 = (myGroupCollectionAttributeData.GetConstructorArgument<string>(1, SpecialType.System_String) ?? "")!.Split(separator, StringSplitOptions.None);
			string[] array3 = (myGroupCollectionAttributeData.GetConstructorArgument<string>(2, SpecialType.System_String) ?? "")!.Split(separator, StringSplitOptions.None);
			string[] array4 = ((!VisualBasicExtensions.IsMyTemplate(attributeSyntax.SyntaxTree)) ? Array.Empty<string>() : (myGroupCollectionAttributeData.GetConstructorArgument<string>(3, SpecialType.System_String) ?? "")!.Split(separator, StringSplitOptions.None));
			ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
			bool flag = false;
			int num = Math.Min(array.Length, array2.Length) - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = array[i].Trim();
				array2[i] = array2[i].Trim();
				if (array[i].Length == 0 || array2[i].Length == 0)
				{
					break;
				}
				if (i < array3.Length)
				{
					array3[i] = array3[i].Trim();
				}
				if (i < array4.Length)
				{
					array4[i] = array4[i].Trim();
				}
				FindGroupClassBaseTypes(array[i].Split(separator2, StringSplitOptions.None), DeclaringCompilation.GlobalNamespace, 0, instance);
				if (!flag && instance.Count > 0 && (object)instance.Last() != null)
				{
					flag = true;
				}
				instance.Add(null);
			}
			if (flag)
			{
				ArrayBuilder<KeyValuePair<NamedTypeSymbol, int>> instance2 = ArrayBuilder<KeyValuePair<NamedTypeSymbol, int>>.GetInstance();
				GetMyGroupCollectionTypes(ContainingModule.GlobalNamespace, instance, binder, instance2);
				if (instance2.Count > 0)
				{
					instance2.Sort(GroupCollectionComparer.Singleton);
					int num2 = instance2.Count - 1;
					for (int j = 0; j <= num2; j++)
					{
						KeyValuePair<NamedTypeSymbol, int> keyValuePair = instance2[j];
						bool mangleNames = (j > 0 && CaseInsensitiveComparison.Equals(keyValuePair.Key.Name, instance2[j - 1].Key.Name)) || (j < instance2.Count - 1 && CaseInsensitiveComparison.Equals(keyValuePair.Key.Name, instance2[j + 1].Key.Name));
						AddSyntheticMyGroupCollectionProperty(keyValuePair.Key, mangleNames, array2[keyValuePair.Value], (keyValuePair.Value < array3.Length) ? array3[keyValuePair.Value] : "", (keyValuePair.Value < array4.Length) ? array4[keyValuePair.Value] : "", membersBuilder, binder, attributeSyntax, diagnostics);
					}
				}
				instance2.Free();
			}
			instance.Free();
		}

		private VisualBasicAttributeData GetMyGroupCollectionAttributeData(BindingDiagnosticBag diagnostics, out Binder binder, out AttributeSyntax attributeSyntax)
		{
			ImmutableArray<SyntaxList<AttributeListSyntax>> attributeDeclarations = GetAttributeDeclarations();
			VisualBasicAttributeData visualBasicAttributeData = null;
			ImmutableArray<SyntaxList<AttributeListSyntax>>.Enumerator enumerator = attributeDeclarations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxList<AttributeListSyntax> current = enumerator.Current;
				if (!current.Any())
				{
					continue;
				}
				binder = GetAttributeBinder(current, base.ContainingSourceModule);
				QuickAttributeChecker quickAttributeChecker = binder.QuickAttributeChecker;
				SyntaxList<AttributeListSyntax>.Enumerator enumerator2 = current.GetEnumerator();
				AttributeSyntax current2;
				ExpressionSyntax expressionSyntax;
				while (enumerator2.MoveNext())
				{
					SeparatedSyntaxList<AttributeSyntax>.Enumerator enumerator3 = enumerator2.Current.Attributes.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						current2 = enumerator3.Current;
						if ((quickAttributeChecker.CheckAttribute(current2) & QuickAttributes.MyGroupCollection) == 0)
						{
							continue;
						}
						NamedTypeSymbol namedTypeSymbol = Binder.BindAttributeType(binder, current2, this, BindingDiagnosticBag.Discarded);
						if (TypeSymbolExtensions.IsErrorType(namedTypeSymbol) || !VisualBasicAttributeData.IsTargetEarlyAttribute(namedTypeSymbol, current2, AttributeDescription.MyGroupCollectionAttribute))
						{
							continue;
						}
						if ((object)namedTypeSymbol == this)
						{
							goto IL_00ba;
						}
						SeparatedSyntaxList<ArgumentSyntax>.Enumerator enumerator4 = current2.ArgumentList.Arguments.GetEnumerator();
						while (enumerator4.MoveNext())
						{
							ArgumentSyntax current3 = enumerator4.Current;
							expressionSyntax = current3.Kind() switch
							{
								SyntaxKind.SimpleArgument => ((SimpleArgumentSyntax)current3).Expression, 
								SyntaxKind.OmittedArgument => null, 
								_ => throw ExceptionUtilities.UnexpectedValue(current3.Kind()), 
							};
							if (expressionSyntax == null || expressionSyntax is LiteralExpressionSyntax)
							{
								continue;
							}
							goto IL_0142;
						}
						bool generatedDiagnostics = false;
						VisualBasicAttributeData attribute = new EarlyWellKnownAttributeBinder(this, binder).GetAttribute(current2, namedTypeSymbol, out generatedDiagnostics);
						if (attribute.HasErrors || generatedDiagnostics || !attribute.IsTargetAttribute(this, AttributeDescription.MyGroupCollectionAttribute))
						{
							continue;
						}
						if (visualBasicAttributeData == null)
						{
							visualBasicAttributeData = attribute;
							attributeSyntax = current2;
							continue;
						}
						goto IL_0198;
					}
				}
				continue;
				IL_00ba:
				Binder.ReportDiagnostic(diagnostics, current2, ERRID.ERR_MyGroupCollectionAttributeCycle);
				break;
				IL_0198:
				visualBasicAttributeData = null;
				break;
				IL_0142:
				Binder.ReportDiagnostic(diagnostics, expressionSyntax, ERRID.ERR_LiteralExpected);
				visualBasicAttributeData = null;
				break;
			}
			if (visualBasicAttributeData == null)
			{
				binder = null;
				attributeSyntax = null;
			}
			return visualBasicAttributeData;
		}

		private static void FindGroupClassBaseTypes(string[] nameParts, NamespaceOrTypeSymbol current, int nextPart, ArrayBuilder<NamedTypeSymbol> candidates)
		{
			if (nextPart == nameParts.Length)
			{
				if (current.Kind == SymbolKind.NamedType)
				{
					NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)current;
					if (namedTypeSymbol.TypeKind == TypeKind.Class && !namedTypeSymbol.IsNotInheritable)
					{
						candidates.Add(namedTypeSymbol);
					}
				}
				return;
			}
			string name = nameParts[nextPart];
			nextPart++;
			ImmutableArray<Symbol>.Enumerator enumerator = current.GetMembers(name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current2 = enumerator.Current;
				SymbolKind kind = current2.Kind;
				if ((uint)(kind - 11) <= 1u)
				{
					FindGroupClassBaseTypes(nameParts, (NamespaceOrTypeSymbol)current2, nextPart, candidates);
				}
			}
		}

		private static void GetMyGroupCollectionTypes(NamespaceSymbol ns, ArrayBuilder<NamedTypeSymbol> baseTypes, Binder binder, ArrayBuilder<KeyValuePair<NamedTypeSymbol, int>> collectionTypes)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = ns.GetMembersUnordered().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				switch (current.Kind)
				{
				case SymbolKind.NamedType:
				{
					if (!(current is SourceNamedTypeSymbol sourceNamedTypeSymbol) || sourceNamedTypeSymbol.IsImplicitlyDeclared || sourceNamedTypeSymbol.TypeKind != TypeKind.Class || sourceNamedTypeSymbol.IsGenericType || sourceNamedTypeSymbol.IsMustInherit)
					{
						break;
					}
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					if (binder.IsAccessible(sourceNamedTypeSymbol, ref useSiteInfo))
					{
						int num = FindBaseInMyGroupCollection(sourceNamedTypeSymbol, baseTypes);
						if (num >= 0 && MyGroupCollectionCandidateHasPublicParameterlessConstructor(sourceNamedTypeSymbol))
						{
							collectionTypes.Add(new KeyValuePair<NamedTypeSymbol, int>(sourceNamedTypeSymbol, num));
						}
					}
					break;
				}
				case SymbolKind.Namespace:
					GetMyGroupCollectionTypes((NamespaceSymbol)current, baseTypes, binder, collectionTypes);
					break;
				}
			}
		}

		private static int FindBaseInMyGroupCollection(NamedTypeSymbol classType, ArrayBuilder<NamedTypeSymbol> bases)
		{
			classType = classType.BaseTypeNoUseSiteDiagnostics;
			while ((object)classType != null && !TypeSymbolExtensions.IsObjectType(classType))
			{
				int num = 0;
				ArrayBuilder<NamedTypeSymbol>.Enumerator enumerator = bases.GetEnumerator();
				while (enumerator.MoveNext())
				{
					NamedTypeSymbol current = enumerator.Current;
					if ((object)current == null)
					{
						num++;
					}
					else if ((object)classType.OriginalDefinition == current)
					{
						return num;
					}
				}
				classType = classType.BaseTypeNoUseSiteDiagnostics;
			}
			return -1;
		}

		private static bool MyGroupCollectionCandidateHasPublicParameterlessConstructor(SourceNamedTypeSymbol candidate)
		{
			if (candidate.MembersHaveBeenCreated)
			{
				return ConstraintsHelper.HasPublicParameterlessConstructor(candidate);
			}
			return candidate.InferFromSyntaxIfClassWillHavePublicParameterlessConstructor();
		}

		internal bool InferFromSyntaxIfClassWillHavePublicParameterlessConstructor()
		{
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			bool flag = false;
			ImmutableArray<SyntaxReference>.Enumerator enumerator = base.SyntaxReferences.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxReference current = enumerator.Current;
				SyntaxNode syntax = current.GetSyntax();
				Binder binder = BinderBuilder.CreateBinderForType(base.ContainingSourceModule, current.SyntaxTree, this);
				SyntaxList<StatementSyntax>.Enumerator enumerator2 = ((TypeBlockSyntax)syntax).Members.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					StatementSyntax current2 = enumerator2.Current;
					SubNewStatementSyntax subNewStatementSyntax = current2.Kind() switch
					{
						SyntaxKind.ConstructorBlock => ((ConstructorBlockSyntax)current2).SubNewStatement, 
						SyntaxKind.SubNewStatement => (SubNewStatementSyntax)current2, 
						_ => null, 
					};
					if (subNewStatementSyntax == null)
					{
						continue;
					}
					SourceMemberFlags allFlags = SourceMethodSymbol.DecodeConstructorModifiers(subNewStatementSyntax.Modifiers, this, binder, instance).AllFlags;
					if ((allFlags & SourceMemberFlags.Shared) == 0)
					{
						if (subNewStatementSyntax.ParameterList == null || subNewStatementSyntax.ParameterList.Parameters.Count == 0)
						{
							instance.Free();
							return (allFlags & SourceMemberFlags.AccessibilityMask) == SourceMemberFlags.AccessibilityPublic;
						}
						flag = true;
					}
				}
			}
			instance.Free();
			return !flag && !IsMustInherit;
		}

		private void AddSyntheticMyGroupCollectionProperty(NamedTypeSymbol targetType, bool mangleNames, string createMethod, string disposeMethod, string defaultInstanceAlias, MembersAndInitializersBuilder membersBuilder, Binder binder, AttributeSyntax attributeSyntax, BindingDiagnosticBag diagnostics)
		{
			string text;
			if (mangleNames)
			{
				text = targetType.ToDisplayString();
				text = text.Replace('.', '_');
			}
			else
			{
				text = targetType.Name;
			}
			string text2 = "m_" + text;
			Symbol conflictsWith = null;
			Dictionary<string, ImmutableArray<NamedTypeSymbol>> typeMembersDictionary = GetTypeMembersDictionary();
			bool isWinMd = SymbolExtensions.IsCompilationOutputWinMdObj(this);
			if (ConflictsWithExistingMemberOrType(text, membersBuilder, typeMembersDictionary, out conflictsWith) || ConflictsWithExistingMemberOrType(Binder.GetAccessorName(text, MethodKind.PropertyGet, isWinMd: false), membersBuilder, typeMembersDictionary, out conflictsWith) || (disposeMethod.Length > 0 && ConflictsWithExistingMemberOrType(Binder.GetAccessorName(text, MethodKind.PropertySet, isWinMd), membersBuilder, typeMembersDictionary, out conflictsWith)) || ConflictsWithExistingMemberOrType(text2, membersBuilder, typeMembersDictionary, out conflictsWith))
			{
				Binder.ReportDiagnostic(diagnostics, attributeSyntax, ERRID.ERR_PropertyNameConflictInMyCollection, conflictsWith, targetType);
				return;
			}
			SynthesizedMyGroupCollectionPropertySymbol synthesizedMyGroupCollectionPropertySymbol = new SynthesizedMyGroupCollectionPropertySymbol(this, attributeSyntax, text, text2, targetType, createMethod, disposeMethod, defaultInstanceAlias);
			AddMember(synthesizedMyGroupCollectionPropertySymbol.AssociatedField, binder, membersBuilder, omitDiagnostics: true);
			AddMember(synthesizedMyGroupCollectionPropertySymbol, binder, membersBuilder, omitDiagnostics: true);
			AddMember(synthesizedMyGroupCollectionPropertySymbol.GetMethod, binder, membersBuilder, omitDiagnostics: true);
			if ((object)synthesizedMyGroupCollectionPropertySymbol.SetMethod != null)
			{
				AddMember(synthesizedMyGroupCollectionPropertySymbol.SetMethod, binder, membersBuilder, omitDiagnostics: true);
			}
		}

		private static bool ConflictsWithExistingMemberOrType(string name, MembersAndInitializersBuilder membersBuilder, Dictionary<string, ImmutableArray<NamedTypeSymbol>> nestedTypes, out Symbol conflictsWith)
		{
			ArrayBuilder<Symbol> value = null;
			ImmutableArray<NamedTypeSymbol> value2 = default(ImmutableArray<NamedTypeSymbol>);
			if (membersBuilder.Members.TryGetValue(name, out value))
			{
				conflictsWith = value[0];
			}
			else if (nestedTypes.TryGetValue(name, out value2))
			{
				conflictsWith = value2[0];
			}
			else
			{
				conflictsWith = null;
			}
			return (object)conflictsWith != null;
		}
	}
}
