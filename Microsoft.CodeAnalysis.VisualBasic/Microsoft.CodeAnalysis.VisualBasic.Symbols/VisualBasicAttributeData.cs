using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class VisualBasicAttributeData : AttributeData, ICustomAttribute
	{
		private ThreeState _lazyIsSecurityAttribute;

		private int ArgumentCount => CommonConstructorArguments.Length;

		private ushort NamedArgumentCount => (ushort)CommonNamedArguments.Length;

		private bool AllowMultiple1 => AttributeClass.GetAttributeUsageInfo().AllowMultiple;

		public new abstract NamedTypeSymbol AttributeClass { get; }

		public new abstract MethodSymbol AttributeConstructor { get; }

		public new abstract SyntaxReference ApplicationSyntaxReference { get; }

		public new IEnumerable<TypedConstant> ConstructorArguments => CommonConstructorArguments;

		public new IEnumerable<KeyValuePair<string, TypedConstant>> NamedArguments => CommonNamedArguments;

		protected override INamedTypeSymbol CommonAttributeClass => AttributeClass;

		protected override IMethodSymbol CommonAttributeConstructor => AttributeConstructor;

		protected override SyntaxReference CommonApplicationSyntaxReference => ApplicationSyntaxReference;

		protected VisualBasicAttributeData()
		{
			_lazyIsSecurityAttribute = ThreeState.Unknown;
		}

		private ImmutableArray<IMetadataExpression> GetArguments1(EmitContext context)
		{
			return CommonConstructorArguments.SelectAsArray((TypedConstant arg) => CreateMetadataExpression(arg, context));
		}

		ImmutableArray<IMetadataExpression> ICustomAttribute.GetArguments(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetArguments1
			return this.GetArguments1(context);
		}

		private IMethodReference Constructor1(EmitContext context, bool reportDiagnostics)
		{
			if (SymbolExtensions.IsDefaultValueTypeConstructor(AttributeConstructor))
			{
				if (reportDiagnostics)
				{
					DiagnosticBagExtensions.Add(context.Diagnostics, ERRID.ERR_AttributeMustBeClassNotStruct1, context.SyntaxNode?.GetLocation() ?? NoLocation.Singleton, AttributeClass);
				}
				return null;
			}
			return ((PEModuleBuilder)context.Module).Translate(AttributeConstructor, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		IMethodReference ICustomAttribute.Constructor(EmitContext context, bool reportDiagnostics)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Constructor1
			return this.Constructor1(context, reportDiagnostics);
		}

		private ImmutableArray<IMetadataNamedArgument> GetNamedArguments1(EmitContext context)
		{
			return CommonNamedArguments.SelectAsArray<KeyValuePair<string, TypedConstant>, IMetadataNamedArgument>((KeyValuePair<string, TypedConstant> namedArgument) => CreateMetadataNamedArgument(namedArgument.Key, namedArgument.Value, context));
		}

		ImmutableArray<IMetadataNamedArgument> ICustomAttribute.GetNamedArguments(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetNamedArguments1
			return this.GetNamedArguments1(context);
		}

		private ITypeReference GetType1(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(AttributeClass, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference ICustomAttribute.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetType1
			return this.GetType1(context);
		}

		private IMetadataExpression CreateMetadataExpression(TypedConstant argument, EmitContext context)
		{
			if (argument.IsNull)
			{
				return CreateMetadataConstant(argument.TypeInternal, null, context);
			}
			return argument.Kind switch
			{
				TypedConstantKind.Array => CreateMetadataArray(argument, context), 
				TypedConstantKind.Type => CreateType(argument, context), 
				_ => CreateMetadataConstant(argument.TypeInternal, RuntimeHelpers.GetObjectValue(argument.ValueInternal), context), 
			};
		}

		private MetadataCreateArray CreateMetadataArray(TypedConstant argument, EmitContext context)
		{
			ImmutableArray<TypedConstant> values = argument.Values;
			IArrayTypeReference arrayTypeReference = ((PEModuleBuilder)context.Module).Translate((ArrayTypeSymbol)argument.TypeInternal);
			if (values.Length == 0)
			{
				return new MetadataCreateArray(arrayTypeReference, arrayTypeReference.GetElementType(context), ImmutableArray<IMetadataExpression>.Empty);
			}
			IMetadataExpression[] array = new IMetadataExpression[values.Length - 1 + 1];
			int num = values.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				array[i] = CreateMetadataExpression(values[i], context);
			}
			return new MetadataCreateArray(arrayTypeReference, arrayTypeReference.GetElementType(context), array.AsImmutableOrNull());
		}

		private MetadataTypeOf CreateType(TypedConstant argument, EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			VisualBasicSyntaxNode syntaxNodeOpt = (VisualBasicSyntaxNode)context.SyntaxNode;
			DiagnosticBag diagnostics = context.Diagnostics;
			return new MetadataTypeOf(pEModuleBuilder.Translate((TypeSymbol)argument.ValueInternal, syntaxNodeOpt, diagnostics), pEModuleBuilder.Translate((TypeSymbol)argument.TypeInternal, syntaxNodeOpt, diagnostics));
		}

		private MetadataConstant CreateMetadataConstant(ITypeSymbolInternal type, object value, EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).CreateConstant((TypeSymbol)type, RuntimeHelpers.GetObjectValue(value), (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		private IMetadataNamedArgument CreateMetadataNamedArgument(string name, TypedConstant argument, EmitContext context)
		{
			Symbol symbol = LookupName(name);
			IMetadataExpression value = CreateMetadataExpression(argument, context);
			TypeSymbol typeSymbol = ((!(symbol is FieldSymbol fieldSymbol)) ? ((PropertySymbol)symbol).Type : fieldSymbol.Type);
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			return new MetadataNamedArgument(symbol, pEModuleBuilder.Translate(typeSymbol, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics), value);
		}

		private Symbol LookupName(string name)
		{
			NamedTypeSymbol namedTypeSymbol = AttributeClass;
			do
			{
				ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.GetMembers(name).GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.DeclaredAccessibility == Accessibility.Public)
					{
						return current;
					}
				}
				namedTypeSymbol = namedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
			}
			while ((object)namedTypeSymbol != null);
			return ErrorTypeSymbol.UnknownResultType;
		}

		internal virtual bool IsTargetAttribute(string namespaceName, string typeName, bool ignoreCase = false)
		{
			if (TypeSymbolExtensions.IsErrorType(AttributeClass) && !(AttributeClass is MissingMetadataTypeSymbol))
			{
				return false;
			}
			StringComparison stringComparison = (ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			return TypeSymbolExtensions.HasNameQualifier(AttributeClass, namespaceName, stringComparison) && AttributeClass.Name.Equals(typeName, stringComparison);
		}

		internal bool IsTargetAttribute(Symbol targetSymbol, AttributeDescription description)
		{
			return GetTargetAttributeSignatureIndex(targetSymbol, description) != -1;
		}

		internal abstract int GetTargetAttributeSignatureIndex(Symbol targetSymbol, AttributeDescription description);

		internal static bool IsTargetEarlyAttribute(NamedTypeSymbol attributeType, AttributeSyntax attributeSyntax, AttributeDescription description)
		{
			int attributeArgCount = ((attributeSyntax.ArgumentList != null) ? attributeSyntax.ArgumentList.Arguments.Where((ArgumentSyntax arg) => arg.Kind() == SyntaxKind.SimpleArgument && !arg.IsNamed).Count() : 0);
			return AttributeData.IsTargetEarlyAttribute(attributeType, attributeArgCount, description);
		}

		public override string ToString()
		{
			if ((object)AttributeClass != null)
			{
				string text = AttributeClass.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);
				if (!CommonConstructorArguments.Any() & !CommonNamedArguments.Any())
				{
					return text;
				}
				PooledStringBuilder instance = PooledStringBuilder.GetInstance();
				StringBuilder builder = instance.Builder;
				builder.Append(text);
				builder.Append("(");
				bool flag = true;
				ImmutableArray<TypedConstant>.Enumerator enumerator = CommonConstructorArguments.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypedConstant current = enumerator.Current;
					if (!flag)
					{
						builder.Append(", ");
					}
					builder.Append(TypedConstantExtensions.ToVisualBasicString(current));
					flag = false;
				}
				ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator2 = CommonNamedArguments.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					KeyValuePair<string, TypedConstant> current2 = enumerator2.Current;
					if (!flag)
					{
						builder.Append(", ");
					}
					builder.Append(current2.Key);
					builder.Append(":=");
					builder.Append(TypedConstantExtensions.ToVisualBasicString(current2.Value));
					flag = false;
				}
				builder.Append(")");
				return instance.ToStringAndFree();
			}
			return base.ToString();
		}

		internal bool IsSecurityAttribute(VisualBasicCompilation comp)
		{
			if (_lazyIsSecurityAttribute == ThreeState.Unknown)
			{
				NamedTypeSymbol attributeClass = AttributeClass;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				_lazyIsSecurityAttribute = TypeSymbolExtensions.IsOrDerivedFromWellKnownClass(attributeClass, WellKnownType.System_Security_Permissions_SecurityAttribute, comp, ref useSiteInfo).ToThreeState();
			}
			return _lazyIsSecurityAttribute.Value();
		}

		internal void DecodeSecurityAttribute<T>(Symbol targetSymbol, VisualBasicCompilation compilation, ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments) where T : WellKnownAttributeData, ISecurityAttributeTarget, new()
		{
			bool hasErrors = false;
			DeclarativeSecurityAction action = DecodeSecurityAttributeAction(targetSymbol, compilation, arguments.AttributeSyntaxOpt, ref hasErrors, (BindingDiagnosticBag)arguments.Diagnostics);
			if (hasErrors)
			{
				return;
			}
			SecurityWellKnownAttributeData orCreateData = arguments.GetOrCreateData<T>().GetOrCreateData();
			orCreateData.SetSecurityAttribute(arguments.Index, action, arguments.AttributesCount);
			if (IsTargetAttribute(targetSymbol, AttributeDescription.PermissionSetAttribute))
			{
				string text = DecodePermissionSetAttribute(compilation, ref arguments);
				if (text != null)
				{
					orCreateData.SetPathForPermissionSetAttributeFixup(arguments.Index, text, arguments.AttributesCount);
				}
			}
		}

		private DeclarativeSecurityAction DecodeSecurityAttributeAction(Symbol targetSymbol, VisualBasicCompilation compilation, AttributeSyntax nodeOpt, ref bool hasErrors, BindingDiagnosticBag diagnostics)
		{
			if (AttributeConstructor.ParameterCount == 0)
			{
				if (IsTargetAttribute(targetSymbol, AttributeDescription.HostProtectionAttribute))
				{
					return DeclarativeSecurityAction.LinkDemand;
				}
			}
			else
			{
				TypedConstant typedValue = CommonConstructorArguments.FirstOrDefault();
				TypeSymbol typeSymbol = (TypeSymbol)typedValue.TypeInternal;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, compilation.Assembly);
				if ((object)typeSymbol != null && TypeSymbolExtensions.IsOrDerivedFromWellKnownClass(typeSymbol, WellKnownType.System_Security_Permissions_SecurityAction, compilation, ref useSiteInfo))
				{
					return ValidateSecurityAction(typedValue, targetSymbol, nodeOpt, diagnostics, out hasErrors);
				}
				diagnostics.Add((nodeOpt != null) ? nodeOpt.Name.GetLocation() : NoLocation.Singleton, useSiteInfo);
			}
			diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_SecurityAttributeMissingAction, AttributeClass), (nodeOpt != null) ? nodeOpt.Name.GetLocation() : NoLocation.Singleton);
			hasErrors = true;
			return DeclarativeSecurityAction.None;
		}

		private DeclarativeSecurityAction ValidateSecurityAction(TypedConstant typedValue, Symbol targetSymbol, AttributeSyntax nodeOpt, BindingDiagnosticBag diagnostics, out bool hasErrors)
		{
			int num = Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(typedValue.ValueInternal);
			hasErrors = false;
			bool flag = default(bool);
			switch (num)
			{
			case 6:
			case 7:
				if (IsTargetAttribute(targetSymbol, AttributeDescription.PrincipalPermissionAttribute))
				{
					diagnostics.Add(ERRID.ERR_PrincipalPermissionInvalidAction, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].GetLocation() : NoLocation.Singleton, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].ToString() : "");
					hasErrors = true;
					return DeclarativeSecurityAction.None;
				}
				flag = false;
				break;
			case 2:
			case 3:
			case 4:
			case 5:
				flag = false;
				break;
			case 8:
			case 9:
			case 10:
				flag = true;
				break;
			default:
				diagnostics.Add(ERRID.ERR_SecurityAttributeInvalidActionTypeOrMethod, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].GetLocation() : NoLocation.Singleton, (nodeOpt != null) ? nodeOpt.Name.ToString() : "", (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].ToString() : "");
				hasErrors = true;
				return DeclarativeSecurityAction.None;
			case 1:
				break;
			}
			if (flag)
			{
				if (targetSymbol.Kind == SymbolKind.NamedType || targetSymbol.Kind == SymbolKind.Method)
				{
					diagnostics.Add(ERRID.ERR_SecurityAttributeInvalidActionTypeOrMethod, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].GetLocation() : NoLocation.Singleton, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].ToString() : "");
					hasErrors = true;
					return DeclarativeSecurityAction.None;
				}
			}
			else if (targetSymbol.Kind == SymbolKind.Assembly)
			{
				diagnostics.Add(ERRID.ERR_SecurityAttributeInvalidActionAssembly, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].GetLocation() : NoLocation.Singleton, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].ToString() : "");
				hasErrors = true;
				return DeclarativeSecurityAction.None;
			}
			return (DeclarativeSecurityAction)num;
		}

		internal string DecodePermissionSetAttribute(VisualBasicCompilation compilation, ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			string text = null;
			ImmutableArray<KeyValuePair<string, TypedConstant>> commonNamedArguments = CommonNamedArguments;
			if (commonNamedArguments.Length == 1)
			{
				KeyValuePair<string, TypedConstant> keyValuePair = commonNamedArguments[0];
				NamedTypeSymbol attributeClass = AttributeClass;
				string text2 = "File";
				string propName = "Hex";
				if (EmbeddedOperators.CompareString(keyValuePair.Key, text2, TextCompare: false) == 0 && PermissionSetAttributeTypeHasRequiredProperty(attributeClass, text2))
				{
					string text3 = (string)keyValuePair.Value.ValueInternal;
					text = compilation.Options.XmlReferenceResolver?.ResolveReference(text3, null);
					if (text == null)
					{
						Location location = ((arguments.AttributeSyntaxOpt != null) ? arguments.AttributeSyntaxOpt!.ArgumentList.Arguments[1].GetLocation() : NoLocation.Singleton);
						((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_PermissionSetAttributeInvalidFile, location, text3 ?? "<empty>", text2);
					}
					else if (!PermissionSetAttributeTypeHasRequiredProperty(attributeClass, propName))
					{
						return null;
					}
				}
			}
			return text;
		}

		private static bool PermissionSetAttributeTypeHasRequiredProperty(NamedTypeSymbol permissionSetType, string propName)
		{
			ImmutableArray<Symbol> members = permissionSetType.GetMembers(propName);
			if (members.Length == 1 && members[0].Kind == SymbolKind.Property)
			{
				PropertySymbol propertySymbol = (PropertySymbol)members[0];
				if ((object)propertySymbol.Type != null && propertySymbol.Type.SpecialType == SpecialType.System_String && propertySymbol.DeclaredAccessibility == Accessibility.Public && SymbolExtensions.GetArity(propertySymbol) == 0 && propertySymbol.HasSet && propertySymbol.SetMethod.DeclaredAccessibility == Accessibility.Public)
				{
					return true;
				}
			}
			return false;
		}

		internal void DecodeClassInterfaceAttribute(AttributeSyntax nodeOpt, BindingDiagnosticBag diagnostics)
		{
			TypedConstant typedConstant = CommonConstructorArguments[0];
			ClassInterfaceType classInterfaceType = ((typedConstant.Kind == TypedConstantKind.Enum) ? typedConstant.DecodeValue<ClassInterfaceType>(SpecialType.System_Enum) : ((ClassInterfaceType)typedConstant.DecodeValue<short>(SpecialType.System_Int16)));
			if ((uint)classInterfaceType > 2u)
			{
				diagnostics.Add(ERRID.ERR_BadAttribute1, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].GetLocation() : NoLocation.Singleton, AttributeClass);
			}
		}

		internal void DecodeInterfaceTypeAttribute(AttributeSyntax node, BindingDiagnosticBag diagnostics)
		{
			ComInterfaceType interfaceType = ComInterfaceType.InterfaceIsDual;
			if (!DecodeInterfaceTypeAttribute(out interfaceType))
			{
				diagnostics.Add(ERRID.ERR_BadAttribute1, node.ArgumentList.Arguments[0].GetLocation(), AttributeClass);
			}
		}

		internal bool DecodeInterfaceTypeAttribute(out ComInterfaceType interfaceType)
		{
			TypedConstant typedConstant = CommonConstructorArguments[0];
			interfaceType = ((typedConstant.Kind == TypedConstantKind.Enum) ? typedConstant.DecodeValue<ComInterfaceType>(SpecialType.System_Enum) : ((ComInterfaceType)typedConstant.DecodeValue<short>(SpecialType.System_Int16)));
			ComInterfaceType comInterfaceType = interfaceType;
			if ((uint)comInterfaceType <= 3u)
			{
				return true;
			}
			return false;
		}

		internal Microsoft.Cci.TypeLibTypeFlags DecodeTypeLibTypeAttribute()
		{
			TypedConstant typedConstant = CommonConstructorArguments[0];
			if (typedConstant.Kind != TypedConstantKind.Enum)
			{
				return (Microsoft.Cci.TypeLibTypeFlags)typedConstant.DecodeValue<short>(SpecialType.System_Int16);
			}
			return typedConstant.DecodeValue<Microsoft.Cci.TypeLibTypeFlags>(SpecialType.System_Enum);
		}

		internal void DecodeGuidAttribute(AttributeSyntax nodeOpt, BindingDiagnosticBag diagnostics)
		{
			string constructorArgument = GetConstructorArgument<string>(0, SpecialType.System_String);
			if (!Guid.TryParseExact(constructorArgument, "D", out var _))
			{
				diagnostics.Add(ERRID.ERR_BadAttributeUuid2, (nodeOpt != null) ? nodeOpt.ArgumentList.Arguments[0].GetLocation() : NoLocation.Singleton, AttributeClass, constructorArgument ?? Microsoft.CodeAnalysis.VisualBasic.ObjectDisplay.ObjectDisplay.NullLiteral);
			}
		}

		internal string DecodeDefaultMemberAttribute()
		{
			return GetConstructorArgument<string>(0, SpecialType.System_String);
		}

		private protected sealed override bool IsStringProperty(string memberName)
		{
			if ((object)AttributeClass != null)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = AttributeClass.GetMembers(memberName).GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is PropertySymbol propertySymbol && propertySymbol.Type.SpecialType == SpecialType.System_String)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal bool ShouldEmitAttribute(Symbol target, bool isReturnType, bool emittingAssemblyAttributesInNetModule)
		{
			if (HasErrors)
			{
				throw ExceptionUtilities.Unreachable;
			}
			if (IsConditionallyOmitted)
			{
				return false;
			}
			switch (target.Kind)
			{
			case SymbolKind.Assembly:
				if ((!emittingAssemblyAttributesInNetModule && (IsTargetAttribute(target, AttributeDescription.AssemblyCultureAttribute) || IsTargetAttribute(target, AttributeDescription.AssemblyVersionAttribute) || IsTargetAttribute(target, AttributeDescription.AssemblyFlagsAttribute) || IsTargetAttribute(target, AttributeDescription.AssemblyAlgorithmIdAttribute))) || (IsTargetAttribute(target, AttributeDescription.CLSCompliantAttribute) && target.DeclaringCompilation.Options.OutputKind == OutputKind.NetModule) || IsTargetAttribute(target, AttributeDescription.TypeForwardedToAttribute) || IsSecurityAttribute(target.DeclaringCompilation))
				{
					return false;
				}
				break;
			case SymbolKind.Event:
				if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) || IsTargetAttribute(target, AttributeDescription.NonSerializedAttribute))
				{
					return false;
				}
				break;
			case SymbolKind.Field:
				if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) || IsTargetAttribute(target, AttributeDescription.NonSerializedAttribute) || IsTargetAttribute(target, AttributeDescription.FieldOffsetAttribute) || IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute))
				{
					return false;
				}
				break;
			case SymbolKind.Method:
				if (isReturnType)
				{
					if (IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute))
					{
						return false;
					}
				}
				else if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) || IsTargetAttribute(target, AttributeDescription.MethodImplAttribute) || IsTargetAttribute(target, AttributeDescription.DllImportAttribute) || IsTargetAttribute(target, AttributeDescription.PreserveSigAttribute) || IsSecurityAttribute(target.DeclaringCompilation))
				{
					return false;
				}
				break;
			case SymbolKind.NetModule:
				if (IsTargetAttribute(target, AttributeDescription.CLSCompliantAttribute) && target.DeclaringCompilation.Options.OutputKind != OutputKind.NetModule)
				{
					return false;
				}
				break;
			case SymbolKind.NamedType:
				if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute) || IsTargetAttribute(target, AttributeDescription.ComImportAttribute) || IsTargetAttribute(target, AttributeDescription.SerializableAttribute) || IsTargetAttribute(target, AttributeDescription.StructLayoutAttribute) || IsTargetAttribute(target, AttributeDescription.WindowsRuntimeImportAttribute) || IsSecurityAttribute(target.DeclaringCompilation))
				{
					return false;
				}
				break;
			case SymbolKind.Parameter:
				if (IsTargetAttribute(target, AttributeDescription.OptionalAttribute) || IsTargetAttribute(target, AttributeDescription.MarshalAsAttribute) || IsTargetAttribute(target, AttributeDescription.InAttribute) || IsTargetAttribute(target, AttributeDescription.OutAttribute))
				{
					return false;
				}
				break;
			case SymbolKind.Property:
				if (IsTargetAttribute(target, AttributeDescription.SpecialNameAttribute))
				{
					return false;
				}
				break;
			}
			return true;
		}
	}
}
