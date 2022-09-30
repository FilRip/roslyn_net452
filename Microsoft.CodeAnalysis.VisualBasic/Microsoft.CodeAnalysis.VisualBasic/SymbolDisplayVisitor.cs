using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.SymbolDisplay;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SymbolDisplayVisitor : AbstractSymbolDisplayVisitor
	{
		private readonly bool _escapeKeywordIdentifiers;

		public override void VisitField(IFieldSymbol symbol)
		{
			AddAccessibilityIfRequired(symbol);
			AddMemberModifiersIfRequired(symbol);
			AddFieldModifiersIfRequired(symbol);
			bool noEscaping = false;
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeContainingType) && symbol.ContainingSymbol is INamedTypeSymbol namedTypeSymbol)
			{
				namedTypeSymbol.Accept(base.NotFirstVisitor);
				AddOperator(SyntaxKind.DotToken);
				noEscaping = true;
			}
			if (symbol.ContainingType.TypeKind == TypeKind.Enum)
			{
				builder.Add(CreatePart(SymbolDisplayPartKind.EnumMemberName, symbol, symbol.Name, noEscaping));
			}
			else if (symbol.IsConst)
			{
				builder.Add(CreatePart(SymbolDisplayPartKind.ConstantName, symbol, symbol.Name, noEscaping));
			}
			else
			{
				builder.Add(CreatePart(SymbolDisplayPartKind.FieldName, symbol, symbol.Name, noEscaping));
			}
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType) && isFirstSymbolVisited && !IsEnumMember(symbol))
			{
				AddSpace();
				AddKeyword(SyntaxKind.AsKeyword);
				AddSpace();
				symbol.Type.Accept(base.NotFirstVisitor);
				AddCustomModifiersIfRequired(symbol.CustomModifiers);
			}
			if (isFirstSymbolVisited && format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeConstantValue) && symbol.IsConst && symbol.HasConstantValue)
			{
				AddSpace();
				AddPunctuation(SyntaxKind.EqualsToken);
				AddSpace();
				AddConstantValue(symbol.Type, RuntimeHelpers.GetObjectValue(symbol.ConstantValue), IsEnumMember(symbol));
			}
		}

		public override void VisitProperty(IPropertySymbol symbol)
		{
			AddAccessibilityIfRequired(symbol);
			AddMemberModifiersIfRequired(symbol);
			if (format.PropertyStyle == SymbolDisplayPropertyStyle.ShowReadWriteDescriptor)
			{
				if (symbol.IsReadOnly)
				{
					AddKeyword(SyntaxKind.ReadOnlyKeyword);
					AddSpace();
				}
				else if (symbol.IsWriteOnly)
				{
					AddKeyword(SyntaxKind.WriteOnlyKeyword);
					AddSpace();
				}
			}
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeModifiers) && symbol.IsIndexer)
			{
				AddKeyword(SyntaxKind.DefaultKeyword);
				AddSpace();
			}
			if (symbol.ReturnsByRef && format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeRef))
			{
				AddKeyword(SyntaxKind.ByRefKeyword);
				AddCustomModifiersIfRequired(symbol.RefCustomModifiers);
				AddSpace();
			}
			if (format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeMemberKeyword))
			{
				if (IsWithEventsProperty(symbol))
				{
					AddKeyword(SyntaxKind.WithEventsKeyword);
				}
				else
				{
					AddKeyword(SyntaxKind.PropertyKeyword);
				}
				AddSpace();
			}
			bool noEscaping = false;
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeContainingType) && IncludeNamedType(symbol.ContainingType))
			{
				symbol.ContainingType.Accept(base.NotFirstVisitor);
				AddOperator(SyntaxKind.DotToken);
				noEscaping = true;
			}
			builder.Add(CreatePart(SymbolDisplayPartKind.PropertyName, symbol, symbol.Name, noEscaping));
			if (symbol.Parameters.Length > 0 && format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeParameters))
			{
				AddPunctuation(SyntaxKind.OpenParenToken);
				AddParametersIfRequired(isExtensionMethod: false, symbol.Parameters);
				AddPunctuation(SyntaxKind.CloseParenToken);
			}
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType))
			{
				AddSpace();
				AddKeyword(SyntaxKind.AsKeyword);
				AddSpace();
				symbol.Type.Accept(base.NotFirstVisitor);
				AddCustomModifiersIfRequired(symbol.TypeCustomModifiers);
			}
		}

		public override void VisitEvent(IEventSymbol symbol)
		{
			AddAccessibilityIfRequired(symbol);
			AddMemberModifiersIfRequired(symbol);
			if (format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeMemberKeyword))
			{
				AddKeyword(SyntaxKind.EventKeyword);
				AddSpace();
			}
			bool noEscaping = false;
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeContainingType) && IncludeNamedType(symbol.ContainingType))
			{
				symbol.ContainingType.Accept(base.NotFirstVisitor);
				AddOperator(SyntaxKind.DotToken);
				noEscaping = true;
			}
			builder.Add(CreatePart(SymbolDisplayPartKind.EventName, symbol, symbol.Name, noEscaping));
			SourceEventSymbol sourceEventSymbol = symbol as SourceEventSymbol;
			if ((object)sourceEventSymbol != null && sourceEventSymbol.IsTypeInferred && format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeParameters))
			{
				AddPunctuation(SyntaxKind.OpenParenToken);
				AddParametersIfRequired(isExtensionMethod: false, StaticCast<IParameterSymbol>.From(sourceEventSymbol.DelegateParameters));
				AddPunctuation(SyntaxKind.CloseParenToken);
			}
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType) && ((object)sourceEventSymbol == null || !sourceEventSymbol.IsTypeInferred))
			{
				AddSpace();
				AddKeyword(SyntaxKind.AsKeyword);
				AddSpace();
				symbol.Type.Accept(base.NotFirstVisitor);
			}
		}

		private void AddAccessor(ISymbol property, IMethodSymbol method, SyntaxKind keyword)
		{
			if (method != null)
			{
				AddSpace();
				if (method.DeclaredAccessibility != property.DeclaredAccessibility)
				{
					AddAccessibilityIfRequired(method);
				}
				AddKeyword(keyword);
				AddPunctuation(SyntaxKind.SemicolonToken);
			}
		}

		public override void VisitMethod(IMethodSymbol symbol)
		{
			if (IsDeclareMethod(symbol))
			{
				VisitDeclareMethod(symbol);
				return;
			}
			if (symbol.IsExtensionMethod && format.ExtensionMethodStyle != 0)
			{
				if (symbol.MethodKind == MethodKind.ReducedExtension && format.ExtensionMethodStyle == SymbolDisplayExtensionMethodStyle.StaticMethod)
				{
					symbol = symbol.GetConstructedReducedFrom();
				}
				else if (symbol.MethodKind != MethodKind.ReducedExtension && format.ExtensionMethodStyle == SymbolDisplayExtensionMethodStyle.InstanceMethod)
				{
					symbol = symbol.ReduceExtensionMethod(symbol.Parameters.First().Type) ?? symbol;
				}
			}
			AddAccessibilityIfRequired(symbol);
			AddMemberModifiersIfRequired(symbol);
			if (symbol.ReturnsByRef && format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeRef))
			{
				AddKeyword(SyntaxKind.ByRefKeyword);
				AddCustomModifiersIfRequired(symbol.RefCustomModifiers);
				AddSpace();
			}
			AddMethodKind(symbol);
			AddMethodName(symbol);
			AddMethodGenericParameters(symbol);
			AddMethodParameters(symbol);
			AddMethodReturnType(symbol);
		}

		private void AddMethodKind(IMethodSymbol symbol)
		{
			if (!format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeMemberKeyword))
			{
				return;
			}
			switch (symbol.MethodKind)
			{
			case MethodKind.Constructor:
			case MethodKind.StaticConstructor:
				AddKeyword(SyntaxKind.SubKeyword);
				AddSpace();
				break;
			case MethodKind.PropertyGet:
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					AddKeyword(SyntaxKind.FunctionKeyword);
					AddSpace();
					break;
				}
				AddKeyword(SyntaxKind.PropertyKeyword);
				AddSpace();
				AddKeyword(SyntaxKind.GetKeyword);
				AddSpace();
				break;
			case MethodKind.PropertySet:
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					AddKeyword(SyntaxKind.SubKeyword);
					AddSpace();
					break;
				}
				AddKeyword(SyntaxKind.PropertyKeyword);
				AddSpace();
				AddKeyword(SyntaxKind.SetKeyword);
				AddSpace();
				break;
			case MethodKind.EventAdd:
			case MethodKind.EventRaise:
			case MethodKind.EventRemove:
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					AddKeyword(SyntaxKind.SubKeyword);
					AddSpace();
					break;
				}
				AddKeyword((symbol.MethodKind == MethodKind.EventAdd) ? SyntaxKind.AddHandlerKeyword : ((symbol.MethodKind == MethodKind.EventRemove) ? SyntaxKind.RemoveHandlerKeyword : SyntaxKind.RaiseEventKeyword));
				AddSpace();
				AddKeyword(SyntaxKind.EventKeyword);
				AddSpace();
				break;
			case MethodKind.Conversion:
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					AddKeyword(SyntaxKind.FunctionKeyword);
					AddSpace();
					break;
				}
				if (CaseInsensitiveComparison.Equals(symbol.Name, "op_Implicit"))
				{
					AddKeyword(SyntaxKind.WideningKeyword);
					AddSpace();
				}
				else
				{
					AddKeyword(SyntaxKind.NarrowingKeyword);
					AddSpace();
				}
				AddKeyword(SyntaxKind.OperatorKeyword);
				AddSpace();
				break;
			case MethodKind.UserDefinedOperator:
			case MethodKind.BuiltinOperator:
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					AddKeyword(SyntaxKind.FunctionKeyword);
					AddSpace();
				}
				else
				{
					AddKeyword(SyntaxKind.OperatorKeyword);
					AddSpace();
				}
				break;
			case MethodKind.AnonymousFunction:
			case MethodKind.DelegateInvoke:
			case MethodKind.Ordinary:
			case MethodKind.ReducedExtension:
				if (symbol.ReturnsVoid)
				{
					AddKeyword(SyntaxKind.SubKeyword);
					AddSpace();
				}
				else
				{
					AddKeyword(SyntaxKind.FunctionKeyword);
					AddSpace();
				}
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(symbol.MethodKind);
			}
		}

		private void AddMethodName(IMethodSymbol symbol)
		{
			bool noEscaping = false;
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeContainingType))
			{
				ITypeSymbol typeSymbol = ((symbol.MethodKind != MethodKind.ReducedExtension) ? (symbol.ContainingSymbol as INamedTypeSymbol) : symbol.ReceiverType);
				if (typeSymbol != null)
				{
					typeSymbol.Accept(base.NotFirstVisitor);
					AddOperator(SyntaxKind.DotToken);
					noEscaping = true;
				}
			}
			switch (symbol.MethodKind)
			{
			case MethodKind.DelegateInvoke:
			case MethodKind.Ordinary:
			case MethodKind.DeclareMethod:
				builder.Add(CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, noEscaping));
				break;
			case MethodKind.ReducedExtension:
				builder.Add(CreatePart(SymbolDisplayPartKind.ExtensionMethodName, symbol, symbol.Name, noEscaping));
				break;
			case MethodKind.EventAdd:
			case MethodKind.EventRaise:
			case MethodKind.EventRemove:
			case MethodKind.PropertyGet:
			case MethodKind.PropertySet:
			{
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					builder.Add(CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, noEscaping));
					break;
				}
				ISymbol associatedSymbol = symbol.AssociatedSymbol;
				if (associatedSymbol.Kind == SymbolKind.Property)
				{
					builder.Add(CreatePart(SymbolDisplayPartKind.PropertyName, associatedSymbol, associatedSymbol.Name, noEscaping));
				}
				else
				{
					builder.Add(CreatePart(SymbolDisplayPartKind.EventName, associatedSymbol, associatedSymbol.Name, noEscaping));
				}
				break;
			}
			case MethodKind.Constructor:
			case MethodKind.StaticConstructor:
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					builder.Add(CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, noEscaping));
				}
				else
				{
					AddKeyword(SyntaxKind.NewKeyword);
				}
				break;
			case MethodKind.UserDefinedOperator:
			case MethodKind.BuiltinOperator:
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					builder.Add(CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, noEscaping));
				}
				else
				{
					AddKeyword(OverloadResolution.GetOperatorTokenKind(symbol.Name));
				}
				break;
			case MethodKind.Conversion:
				if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames))
				{
					builder.Add(CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, noEscaping));
				}
				else
				{
					AddKeyword(SyntaxKind.CTypeKeyword);
				}
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(symbol.MethodKind);
			case MethodKind.AnonymousFunction:
				break;
			}
		}

		private void AddMethodGenericParameters(IMethodSymbol method)
		{
			if (method.Arity > 0 && format.GenericsOptions.IncludesOption(SymbolDisplayGenericsOptions.IncludeTypeParameters))
			{
				AddTypeArguments(method.TypeArguments);
			}
		}

		private void AddMethodParameters(IMethodSymbol method)
		{
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeParameters))
			{
				AddPunctuation(SyntaxKind.OpenParenToken);
				AddParametersIfRequired(method.IsExtensionMethod && method.MethodKind != MethodKind.ReducedExtension, method.Parameters);
				AddPunctuation(SyntaxKind.CloseParenToken);
			}
		}

		private void AddMethodReturnType(IMethodSymbol method)
		{
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType))
			{
				MethodKind methodKind = method.MethodKind;
				if (methodKind != MethodKind.Constructor && methodKind != MethodKind.StaticConstructor && !method.ReturnsVoid)
				{
					AddSpace();
					AddKeyword(SyntaxKind.AsKeyword);
					AddSpace();
					method.ReturnType.Accept(base.NotFirstVisitor);
				}
				AddCustomModifiersIfRequired(method.ReturnTypeCustomModifiers);
			}
		}

		private void VisitDeclareMethod(IMethodSymbol method)
		{
			DllImportData dllImportData = method.GetDllImportData();
			AddAccessibilityIfRequired(method);
			if (format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeMemberKeyword))
			{
				AddKeyword(SyntaxKind.DeclareKeyword);
				AddSpace();
				switch (dllImportData.CharacterSet)
				{
				case CharSet.None:
				case CharSet.Ansi:
					AddKeyword(SyntaxKind.AnsiKeyword);
					AddSpace();
					break;
				case CharSet.Auto:
					AddKeyword(SyntaxKind.AutoKeyword);
					AddSpace();
					break;
				case CharSet.Unicode:
					AddKeyword(SyntaxKind.UnicodeKeyword);
					AddSpace();
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(dllImportData.CharacterSet);
				}
				AddKeyword(method.ReturnsVoid ? SyntaxKind.SubKeyword : SyntaxKind.FunctionKeyword);
				AddSpace();
			}
			AddMethodName(method);
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType))
			{
				bool flag = false;
				if (dllImportData.ModuleName != null)
				{
					AddSpace();
					AddKeyword(SyntaxKind.LibKeyword);
					AddSpace();
					builder.Add(CreatePart(SymbolDisplayPartKind.StringLiteral, null, Quote(dllImportData.ModuleName), noEscaping: true));
					flag = true;
				}
				if (dllImportData.EntryPointName != null)
				{
					AddSpace();
					AddKeyword(SyntaxKind.AliasKeyword);
					AddSpace();
					builder.Add(CreatePart(SymbolDisplayPartKind.StringLiteral, null, Quote(dllImportData.EntryPointName), noEscaping: true));
					flag = true;
				}
				if (flag && format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeParameters))
				{
					AddSpace();
				}
			}
			AddMethodParameters(method);
			AddMethodReturnType(method);
		}

		private static string Quote(string str)
		{
			return "\"" + str.Replace("\"", "\"\"") + "\"";
		}

		public override void VisitParameter(IParameterSymbol symbol)
		{
			if (format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeOptionalBrackets) && symbol.IsOptional)
			{
				AddPseudoPunctuation("[");
			}
			if (format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeParamsRefOut))
			{
				if (symbol.RefKind != 0 && IsExplicitByRefParameter(symbol))
				{
					AddKeyword(SyntaxKind.ByRefKeyword);
					AddSpace();
					AddCustomModifiersIfRequired(symbol.RefCustomModifiers, leadingSpace: false, trailingSpace: true);
				}
				if (symbol.IsParams)
				{
					AddKeyword(SyntaxKind.ParamArrayKeyword);
					AddSpace();
				}
			}
			if (format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeName))
			{
				SymbolDisplayPartKind kind = (symbol.IsThis ? SymbolDisplayPartKind.Keyword : SymbolDisplayPartKind.ParameterName);
				builder.Add(CreatePart(kind, symbol, symbol.Name, noEscaping: false));
			}
			if (format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeType))
			{
				if (format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeName))
				{
					AddSpace();
					AddKeyword(SyntaxKind.AsKeyword);
					AddSpace();
				}
				symbol.Type.Accept(base.NotFirstVisitor);
				AddCustomModifiersIfRequired(symbol.CustomModifiers);
			}
			if (format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeDefaultValue) && symbol.HasExplicitDefaultValue)
			{
				AddSpace();
				AddPunctuation(SyntaxKind.EqualsToken);
				AddSpace();
				AddConstantValue(symbol.Type, RuntimeHelpers.GetObjectValue(symbol.ExplicitDefaultValue));
			}
			if (format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeOptionalBrackets) && symbol.IsOptional)
			{
				AddPseudoPunctuation("]");
			}
		}

		private void AddCustomModifiersIfRequired(ImmutableArray<CustomModifier> customModifiers, bool leadingSpace = true, bool trailingSpace = false)
		{
			if (!format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.IncludeCustomModifiers) || customModifiers.IsEmpty)
			{
				return;
			}
			bool flag = true;
			ImmutableArray<CustomModifier>.Enumerator enumerator = customModifiers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CustomModifier current = enumerator.Current;
				if (!flag || leadingSpace)
				{
					AddSpace();
				}
				flag = false;
				builder.Add(CreatePart((SymbolDisplayPartKind)34, null, current.IsOptional ? "modopt" : "modreq", noEscaping: true));
				AddPunctuation(SyntaxKind.OpenParenToken);
				current.Modifier.Accept(base.NotFirstVisitor);
				AddPunctuation(SyntaxKind.CloseParenToken);
			}
			if (trailingSpace)
			{
				AddSpace();
			}
		}

		private void AddFieldModifiersIfRequired(IFieldSymbol symbol)
		{
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeModifiers) && !IsEnumMember(symbol))
			{
				if (symbol.IsConst)
				{
					AddKeyword(SyntaxKind.ConstKeyword);
					AddSpace();
				}
				if (symbol.IsReadOnly)
				{
					AddKeyword(SyntaxKind.ReadOnlyKeyword);
					AddSpace();
				}
			}
		}

		private void AddMemberModifiersIfRequired(ISymbol symbol)
		{
			INamedTypeSymbol namedTypeSymbol = symbol.ContainingSymbol as INamedTypeSymbol;
			if (!format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeModifiers) || (namedTypeSymbol != null && (namedTypeSymbol.TypeKind == TypeKind.Interface || IsEnumMember(symbol))))
			{
				return;
			}
			bool flag = symbol.Kind == SymbolKind.Field && ((IFieldSymbol)symbol).IsConst;
			if (symbol.IsStatic && (namedTypeSymbol == null || namedTypeSymbol.TypeKind != TypeKind.Module) && !flag)
			{
				AddKeyword(SyntaxKind.SharedKeyword);
				AddSpace();
			}
			if (!IsWithEventsProperty(symbol))
			{
				if (symbol.IsAbstract)
				{
					AddKeyword(SyntaxKind.MustOverrideKeyword);
					AddSpace();
				}
				if (symbol.IsSealed)
				{
					AddKeyword(SyntaxKind.NotOverridableKeyword);
					AddSpace();
				}
				if (symbol.IsVirtual)
				{
					AddKeyword(SyntaxKind.OverridableKeyword);
					AddSpace();
				}
				if (IsOverloads(symbol) && !symbol.IsOverride)
				{
					AddKeyword(SyntaxKind.OverloadsKeyword);
					AddSpace();
				}
				if (symbol.IsOverride)
				{
					AddKeyword(SyntaxKind.OverridesKeyword);
					AddSpace();
				}
			}
		}

		private void AddParametersIfRequired(bool isExtensionMethod, ImmutableArray<IParameterSymbol> parameters)
		{
			if (format.ParameterOptions == SymbolDisplayParameterOptions.None)
			{
				return;
			}
			bool flag = true;
			ImmutableArray<IParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				IParameterSymbol current = enumerator.Current;
				if (!flag)
				{
					AddPunctuation(SyntaxKind.CommaToken);
					AddSpace();
				}
				flag = false;
				current.Accept(base.NotFirstVisitor);
			}
		}

		private bool IsWithEventsProperty(ISymbol symbol)
		{
			if (symbol is PropertySymbol propertySymbol)
			{
				return propertySymbol.IsWithEvents;
			}
			return false;
		}

		private bool IsOverloads(ISymbol symbol)
		{
			if (symbol is Symbol sym)
			{
				return SymbolExtensions.IsOverloads(sym);
			}
			return false;
		}

		private bool IsDeclareMethod(IMethodSymbol method)
		{
			if (method is MethodSymbol methodSymbol)
			{
				return methodSymbol.MethodKind == MethodKind.DeclareMethod;
			}
			return false;
		}

		private bool IsExplicitByRefParameter(IParameterSymbol parameter)
		{
			if (parameter is ParameterSymbol parameterSymbol)
			{
				return parameterSymbol.IsExplicitByRef;
			}
			return false;
		}

		[Conditional("DEBUG")]
		private void AssertContainingSymbol(ISymbol symbol)
		{
		}

		public override void VisitArrayType(IArrayTypeSymbol symbol)
		{
			if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.ReverseArrayRankSpecifiers))
			{
				symbol.ElementType.Accept(this);
				AddCustomModifiersIfRequired(symbol.CustomModifiers, leadingSpace: true, trailingSpace: true);
				AddArrayRank(symbol);
				return;
			}
			ITypeSymbol elementType = symbol.ElementType;
			while (elementType.Kind == SymbolKind.ArrayType)
			{
				elementType = ((IArrayTypeSymbol)elementType).ElementType;
			}
			elementType.Accept(base.NotFirstVisitor);
			for (IArrayTypeSymbol arrayTypeSymbol = symbol; arrayTypeSymbol != null; arrayTypeSymbol = arrayTypeSymbol.ElementType as IArrayTypeSymbol)
			{
				AddCustomModifiersIfRequired(arrayTypeSymbol.CustomModifiers, leadingSpace: true, trailingSpace: true);
				AddArrayRank(arrayTypeSymbol);
			}
		}

		private void AddArrayRank(IArrayTypeSymbol symbol)
		{
			bool flag = format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.UseAsterisksInMultiDimensionalArrays);
			AddPunctuation(SyntaxKind.OpenParenToken);
			if (symbol.Rank > 1)
			{
				if (flag)
				{
					AddPunctuation(SyntaxKind.AsteriskToken);
				}
			}
			else if (symbol is ArrayTypeSymbol arrayTypeSymbol && !arrayTypeSymbol.IsSZArray)
			{
				AddPunctuation(SyntaxKind.AsteriskToken);
			}
			for (int i = 0; i < symbol.Rank - 1; i++)
			{
				AddPunctuation(SyntaxKind.CommaToken);
				if (flag)
				{
					AddPunctuation(SyntaxKind.AsteriskToken);
				}
			}
			AddPunctuation(SyntaxKind.CloseParenToken);
		}

		public override void VisitDynamicType(IDynamicTypeSymbol symbol)
		{
			AddKeyword(SyntaxKind.ObjectKeyword);
		}

		public override void VisitPointerType(IPointerTypeSymbol symbol)
		{
			symbol.PointedAtType.Accept(base.NotFirstVisitor);
			AddPunctuation(SyntaxKind.AsteriskToken);
		}

		public override void VisitTypeParameter(ITypeParameterSymbol symbol)
		{
			if (isFirstSymbolVisited)
			{
				AddTypeParameterVarianceIfRequired(symbol);
			}
			builder.Add(CreatePart(SymbolDisplayPartKind.TypeParameterName, symbol, symbol.Name, noEscaping: false));
		}

		public override void VisitNamedType(INamedTypeSymbol symbol)
		{
			if (base.IsMinimizing && TryAddAlias(symbol, builder))
			{
				return;
			}
			if (format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.UseSpecialTypes))
			{
				if (AddSpecialTypeKeyword(symbol))
				{
					return;
				}
				if (!format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.ExpandNullable) && ITypeSymbolHelpers.IsNullableType(symbol) && symbol != symbol.OriginalDefinition)
				{
					symbol.TypeArguments[0].Accept(base.NotFirstVisitor);
					AddPunctuation(SyntaxKind.QuestionToken);
					return;
				}
			}
			if (base.IsMinimizing || symbol.IsTupleType)
			{
				MinimallyQualify(symbol);
				return;
			}
			AddTypeKind(symbol);
			if (CanShowDelegateSignature(symbol) && (symbol.IsAnonymousType || format.DelegateStyle == SymbolDisplayDelegateStyle.NameAndSignature))
			{
				IMethodSymbol delegateInvokeMethod = symbol.DelegateInvokeMethod;
				if (delegateInvokeMethod.ReturnsVoid)
				{
					AddKeyword(SyntaxKind.SubKeyword);
				}
				else
				{
					if (delegateInvokeMethod.ReturnsByRef && format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeRef))
					{
						AddKeyword(SyntaxKind.ByRefKeyword);
						AddSpace();
					}
					AddKeyword(SyntaxKind.FunctionKeyword);
				}
				AddSpace();
			}
			bool noEscaping = false;
			ISymbol containingSymbol = symbol.ContainingSymbol;
			if (ShouldVisitNamespace(containingSymbol))
			{
				noEscaping = true;
				INamespaceSymbol symbol2 = (INamespaceSymbol)containingSymbol;
				string emittedName = ((symbol is NamedTypeSymbol namedTypeSymbol) ? (namedTypeSymbol.GetEmittedNamespaceName() ?? string.Empty) : string.Empty);
				VisitNamespace(symbol2, emittedName);
				AddOperator(SyntaxKind.DotToken);
			}
			if (format.TypeQualificationStyle == SymbolDisplayTypeQualificationStyle.NameAndContainingTypes || format.TypeQualificationStyle == SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
			{
				INamedTypeSymbol containingType = symbol.ContainingType;
				if (containingType != null)
				{
					noEscaping = true;
					containingType.Accept(base.NotFirstVisitor);
					AddOperator(SyntaxKind.DotToken);
				}
			}
			AddNameAndTypeArgumentsOrParameters(symbol, noEscaping);
			if (!CanShowDelegateSignature(symbol))
			{
				return;
			}
			if (symbol.IsAnonymousType || format.DelegateStyle == SymbolDisplayDelegateStyle.NameAndSignature || format.DelegateStyle == SymbolDisplayDelegateStyle.NameAndParameters)
			{
				IMethodSymbol delegateInvokeMethod2 = symbol.DelegateInvokeMethod;
				AddPunctuation(SyntaxKind.OpenParenToken);
				AddParametersIfRequired(isExtensionMethod: false, delegateInvokeMethod2.Parameters);
				AddPunctuation(SyntaxKind.CloseParenToken);
			}
			if (symbol.IsAnonymousType || format.DelegateStyle == SymbolDisplayDelegateStyle.NameAndSignature)
			{
				IMethodSymbol delegateInvokeMethod3 = symbol.DelegateInvokeMethod;
				if (!delegateInvokeMethod3.ReturnsVoid)
				{
					AddSpace();
					AddKeyword(SyntaxKind.AsKeyword);
					AddSpace();
					delegateInvokeMethod3.ReturnType.Accept(base.NotFirstVisitor);
				}
			}
		}

		private bool CanShowDelegateSignature(INamedTypeSymbol symbol)
		{
			if (isFirstSymbolVisited && symbol.TypeKind == TypeKind.Delegate && (symbol.IsAnonymousType || format.DelegateStyle != 0))
			{
				return symbol.DelegateInvokeMethod != null;
			}
			return false;
		}

		private void AddNameAndTypeArgumentsOrParameters(INamedTypeSymbol symbol, bool noEscaping)
		{
			string text = null;
			bool flag = false;
			if (symbol.IsAnonymousType)
			{
				AddAnonymousTypeName(symbol);
				return;
			}
			if (symbol.IsTupleType)
			{
				if (HasNonDefaultTupleElements(symbol) || CanUseTupleTypeName(symbol))
				{
					AddTupleTypeName(symbol);
					return;
				}
				symbol = symbol.TupleUnderlyingType;
			}
			if (symbol is NoPiaIllegalGenericInstantiationSymbol noPiaIllegalGenericInstantiationSymbol)
			{
				symbol = noPiaIllegalGenericInstantiationSymbol.UnderlyingSymbol;
			}
			else if (symbol is NoPiaAmbiguousCanonicalTypeSymbol noPiaAmbiguousCanonicalTypeSymbol)
			{
				symbol = noPiaAmbiguousCanonicalTypeSymbol.FirstCandidate;
			}
			else if (symbol is NoPiaMissingCanonicalTypeSymbol noPiaMissingCanonicalTypeSymbol)
			{
				text = noPiaMissingCanonicalTypeSymbol.FullTypeName;
			}
			if (text == null)
			{
				text = symbol.Name;
			}
			text = ((!format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName) || !string.IsNullOrEmpty(text)) ? RemoveAttributeSuffixIfNecessary(symbol, text) : "?");
			SymbolDisplayPartKind kind;
			switch (symbol.TypeKind)
			{
			case TypeKind.Class:
			case TypeKind.Submission:
				kind = SymbolDisplayPartKind.ClassName;
				break;
			case TypeKind.Delegate:
				kind = SymbolDisplayPartKind.DelegateName;
				break;
			case TypeKind.Enum:
				kind = SymbolDisplayPartKind.EnumName;
				break;
			case TypeKind.Interface:
				kind = SymbolDisplayPartKind.InterfaceName;
				break;
			case TypeKind.Module:
				kind = SymbolDisplayPartKind.ModuleName;
				break;
			case TypeKind.Struct:
				kind = SymbolDisplayPartKind.StructName;
				break;
			case TypeKind.Error:
				kind = SymbolDisplayPartKind.ErrorTypeName;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(symbol.TypeKind);
			}
			builder.Add(CreatePart(kind, symbol, text, noEscaping));
			bool flag2 = symbol is MissingMetadataTypeSymbol;
			if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseArityForGenericTypes))
			{
				if (((NamedTypeSymbol)symbol).MangleName)
				{
					builder.Add(CreatePart((SymbolDisplayPartKind)33, null, "`" + symbol.Arity, noEscaping: false));
				}
			}
			else if (symbol.Arity > 0 && format.GenericsOptions.IncludesOption(SymbolDisplayGenericsOptions.IncludeTypeParameters) && !flag)
			{
				if (flag2 || symbol.IsUnboundGenericType)
				{
					AddPunctuation(SyntaxKind.OpenParenToken);
					AddKeyword(SyntaxKind.OfKeyword);
					AddSpace();
					for (int i = 0; i < symbol.Arity - 1; i++)
					{
						AddPunctuation(SyntaxKind.CommaToken);
					}
					AddPunctuation(SyntaxKind.CloseParenToken);
				}
				else
				{
					AddTypeArguments(symbol.TypeArguments, symbol);
				}
			}
			if (format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.FlagMissingMetadataTypes) && (flag2 || (!symbol.IsDefinition && symbol.OriginalDefinition is MissingMetadataTypeSymbol)))
			{
				builder.Add(CreatePart(SymbolDisplayPartKind.Punctuation, null, "[", noEscaping: false));
				builder.Add(CreatePart((SymbolDisplayPartKind)34, symbol, "missing", noEscaping: false));
				builder.Add(CreatePart(SymbolDisplayPartKind.Punctuation, null, "]", noEscaping: false));
			}
		}

		private void AddAnonymousTypeName(INamedTypeSymbol symbol)
		{
			switch (symbol.TypeKind)
			{
			case TypeKind.Class:
			{
				string text = string.Join(", ", from p in symbol.GetMembers().OfType<IPropertySymbol>()
					select CreateAnonymousTypeMember(p));
				if (text.Length == 0)
				{
					builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.ClassName, symbol, "<empty anonymous type>"));
					break;
				}
				string text2 = $"<anonymous type: {text}>";
				builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.ClassName, symbol, text2));
				break;
			}
			case TypeKind.Delegate:
				builder.Add(CreatePart(SymbolDisplayPartKind.DelegateName, symbol, "<generated method>", noEscaping: true));
				break;
			}
		}

		private static bool CanUseTupleTypeName(INamedTypeSymbol tupleSymbol)
		{
			INamedTypeSymbol tupleUnderlyingTypeOrSelf = GetTupleUnderlyingTypeOrSelf(tupleSymbol);
			if (tupleUnderlyingTypeOrSelf.Arity == 1)
			{
				return false;
			}
			while (tupleUnderlyingTypeOrSelf.Arity == 8)
			{
				tupleSymbol = (INamedTypeSymbol)tupleUnderlyingTypeOrSelf.TypeArguments[7];
				if (HasNonDefaultTupleElements(tupleSymbol))
				{
					return false;
				}
				tupleUnderlyingTypeOrSelf = GetTupleUnderlyingTypeOrSelf(tupleSymbol);
			}
			return true;
		}

		private static INamedTypeSymbol GetTupleUnderlyingTypeOrSelf(INamedTypeSymbol tupleSymbol)
		{
			return tupleSymbol.TupleUnderlyingType ?? tupleSymbol;
		}

		private static bool HasNonDefaultTupleElements(INamedTypeSymbol tupleSymbol)
		{
			return tupleSymbol.TupleElements.Any((IFieldSymbol e) => e.IsExplicitlyNamedTupleElement);
		}

		private void AddTupleTypeName(INamedTypeSymbol symbol)
		{
			ImmutableArray<IFieldSymbol> tupleElements = symbol.TupleElements;
			AddPunctuation(SyntaxKind.OpenParenToken);
			int num = tupleElements.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				IFieldSymbol fieldSymbol = tupleElements[i];
				if (i != 0)
				{
					AddPunctuation(SyntaxKind.CommaToken);
					AddSpace();
				}
				if (fieldSymbol.IsExplicitlyNamedTupleElement)
				{
					builder.Add(CreatePart(SymbolDisplayPartKind.FieldName, symbol, fieldSymbol.Name, noEscaping: false));
					AddSpace();
					AddKeyword(SyntaxKind.AsKeyword);
					AddSpace();
				}
				fieldSymbol.Type.Accept(base.NotFirstVisitor);
			}
			AddPunctuation(SyntaxKind.CloseParenToken);
		}

		private string CreateAnonymousTypeMember(IPropertySymbol prop)
		{
			string text = CreateAnonymousTypeMemberWorker(prop);
			if (!prop.IsReadOnly)
			{
				return text;
			}
			return "Key " + text;
		}

		private string CreateAnonymousTypeMemberWorker(IPropertySymbol prop)
		{
			return prop.Name + " As " + prop.Type.ToDisplayString(format);
		}

		private bool AddSpecialTypeKeyword(INamedTypeSymbol symbol)
		{
			string text = Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.TryGetKeywordText(symbol.SpecialType);
			if (text == null)
			{
				return false;
			}
			builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, symbol, text, noEscaping: false));
			return true;
		}

		private void AddTypeKind(INamedTypeSymbol symbol)
		{
			if (!isFirstSymbolVisited || !format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeTypeKeyword))
			{
				return;
			}
			if (symbol.IsAnonymousType)
			{
				builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.AnonymousTypeIndicator, null, "AnonymousType"));
				AddSpace();
				return;
			}
			if (symbol.IsTupleType)
			{
				builder.Add(new SymbolDisplayPart(SymbolDisplayPartKind.AnonymousTypeIndicator, null, "Tuple"));
				AddSpace();
				return;
			}
			SyntaxKind typeKindKeyword = GetTypeKindKeyword(symbol.TypeKind);
			if (typeKindKeyword != 0)
			{
				AddKeyword(typeKindKeyword);
				AddSpace();
			}
		}

		private static SyntaxKind GetTypeKindKeyword(TypeKind typeKind)
		{
			return typeKind switch
			{
				TypeKind.Enum => SyntaxKind.EnumKeyword, 
				TypeKind.Class => SyntaxKind.ClassKeyword, 
				TypeKind.Delegate => SyntaxKind.DelegateKeyword, 
				TypeKind.Interface => SyntaxKind.InterfaceKeyword, 
				TypeKind.Module => SyntaxKind.ModuleKeyword, 
				TypeKind.Struct => SyntaxKind.StructureKeyword, 
				_ => SyntaxKind.None, 
			};
		}

		private void AddTypeParameterVarianceIfRequired(ITypeParameterSymbol symbol)
		{
			if (format.GenericsOptions.IncludesOption(SymbolDisplayGenericsOptions.IncludeVariance))
			{
				switch (symbol.Variance)
				{
				case VarianceKind.In:
					AddKeyword(SyntaxKind.InKeyword);
					AddSpace();
					break;
				case VarianceKind.Out:
					AddKeyword(SyntaxKind.OutKeyword);
					AddSpace();
					break;
				}
			}
		}

		private void AddTypeArguments(ImmutableArray<ITypeSymbol> typeArguments, INamedTypeSymbol modifiersSource = null)
		{
			AddPunctuation(SyntaxKind.OpenParenToken);
			AddKeyword(SyntaxKind.OfKeyword);
			AddSpace();
			bool flag = true;
			int num = typeArguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				ITypeSymbol typeSymbol = typeArguments[i];
				if (!flag)
				{
					AddPunctuation(SyntaxKind.CommaToken);
					AddSpace();
				}
				flag = false;
				if (typeSymbol.Kind == SymbolKind.TypeParameter)
				{
					ITypeParameterSymbol typeParameterSymbol = (ITypeParameterSymbol)typeSymbol;
					AddTypeParameterVarianceIfRequired(typeParameterSymbol);
					typeParameterSymbol.Accept(base.NotFirstVisitor);
					if (format.GenericsOptions.IncludesOption(SymbolDisplayGenericsOptions.IncludeTypeConstraints))
					{
						AddTypeParameterConstraints(typeParameterSymbol);
					}
				}
				else
				{
					typeSymbol.Accept(base.NotFirstVisitorNamespaceOrType);
				}
				if (modifiersSource != null)
				{
					AddCustomModifiersIfRequired(modifiersSource.GetTypeArgumentCustomModifiers(i));
				}
			}
			AddPunctuation(SyntaxKind.CloseParenToken);
		}

		private static int TypeParameterSpecialConstraintCount(ITypeParameterSymbol typeParam)
		{
			return (typeParam.HasReferenceTypeConstraint ? 1 : 0) + (typeParam.HasValueTypeConstraint ? 1 : 0) + (typeParam.HasConstructorConstraint ? 1 : 0);
		}

		private void AddTypeParameterConstraints(ITypeParameterSymbol typeParam)
		{
			if (!isFirstSymbolVisited)
			{
				return;
			}
			ImmutableArray<ITypeSymbol> constraintTypes = typeParam.ConstraintTypes;
			int num = TypeParameterSpecialConstraintCount(typeParam) + constraintTypes.Length;
			if (num == 0)
			{
				return;
			}
			AddSpace();
			AddKeyword(SyntaxKind.AsKeyword);
			AddSpace();
			if (num > 1)
			{
				AddPunctuation(SyntaxKind.OpenBraceToken);
			}
			bool flag = false;
			if (typeParam.HasReferenceTypeConstraint)
			{
				AddKeyword(SyntaxKind.ClassKeyword);
				flag = true;
			}
			else if (typeParam.HasValueTypeConstraint)
			{
				AddKeyword(SyntaxKind.StructureKeyword);
				flag = true;
			}
			ImmutableArray<ITypeSymbol>.Enumerator enumerator = constraintTypes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ITypeSymbol current = enumerator.Current;
				if (flag)
				{
					AddPunctuation(SyntaxKind.CommaToken);
					AddSpace();
				}
				current.Accept(base.NotFirstVisitor);
				flag = true;
			}
			if (typeParam.HasConstructorConstraint)
			{
				if (flag)
				{
					AddPunctuation(SyntaxKind.CommaToken);
					AddSpace();
				}
				AddKeyword(SyntaxKind.NewKeyword);
			}
			if (num > 1)
			{
				AddPunctuation(SyntaxKind.CloseBraceToken);
			}
		}

		internal SymbolDisplayVisitor(ArrayBuilder<SymbolDisplayPart> builder, SymbolDisplayFormat format, SemanticModel semanticModelOpt, int positionOpt)
			: base(builder, format, isFirstSymbolVisited: true, semanticModelOpt, positionOpt)
		{
			_escapeKeywordIdentifiers = format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);
		}

		private SymbolDisplayVisitor(ArrayBuilder<SymbolDisplayPart> builder, SymbolDisplayFormat format, SemanticModel semanticModelOpt, int positionOpt, bool escapeKeywordIdentifiers, bool isFirstSymbolVisited, bool inNamespaceOrType = false)
			: base(builder, format, isFirstSymbolVisited, semanticModelOpt, positionOpt, inNamespaceOrType)
		{
			_escapeKeywordIdentifiers = escapeKeywordIdentifiers;
		}

		protected override AbstractSymbolDisplayVisitor MakeNotFirstVisitor(bool inNamespaceOrType = false)
		{
			return new SymbolDisplayVisitor(builder, format, semanticModelOpt, positionOpt, _escapeKeywordIdentifiers, isFirstSymbolVisited: false, inNamespaceOrType);
		}

		internal SymbolDisplayPart CreatePart(SymbolDisplayPartKind kind, ISymbol symbol, string text, bool noEscaping)
		{
			bool flag = (AlwaysEscape(kind, text) || !noEscaping) && _escapeKeywordIdentifiers && IsEscapable(kind);
			return new SymbolDisplayPart(kind, symbol, flag ? EscapeIdentifier(text) : text);
		}

		private static bool AlwaysEscape(SymbolDisplayPartKind kind, string text)
		{
			if (kind != SymbolDisplayPartKind.Keyword && (CaseInsensitiveComparison.Equals(SyntaxFacts.GetText(SyntaxKind.REMKeyword), text) || CaseInsensitiveComparison.Equals(SyntaxFacts.GetText(SyntaxKind.NewKeyword), text)))
			{
				return true;
			}
			return false;
		}

		private static bool IsEscapable(SymbolDisplayPartKind kind)
		{
			switch (kind)
			{
			case SymbolDisplayPartKind.AliasName:
			case SymbolDisplayPartKind.ClassName:
			case SymbolDisplayPartKind.DelegateName:
			case SymbolDisplayPartKind.EnumName:
			case SymbolDisplayPartKind.ErrorTypeName:
			case SymbolDisplayPartKind.EventName:
			case SymbolDisplayPartKind.FieldName:
			case SymbolDisplayPartKind.InterfaceName:
			case SymbolDisplayPartKind.LabelName:
			case SymbolDisplayPartKind.LocalName:
			case SymbolDisplayPartKind.MethodName:
			case SymbolDisplayPartKind.ModuleName:
			case SymbolDisplayPartKind.NamespaceName:
			case SymbolDisplayPartKind.ParameterName:
			case SymbolDisplayPartKind.PropertyName:
			case SymbolDisplayPartKind.StructName:
			case SymbolDisplayPartKind.TypeParameterName:
			case SymbolDisplayPartKind.RangeVariableName:
				return true;
			default:
				return false;
			}
		}

		private string EscapeIdentifier(string identifier)
		{
			if (SyntaxFacts.GetKeywordKind(identifier) != 0)
			{
				return $"[{identifier}]";
			}
			if (base.IsMinimizing)
			{
				switch (SyntaxFacts.GetContextualKeywordKind(identifier))
				{
				case SyntaxKind.InKeyword:
				case SyntaxKind.LetKeyword:
				case SyntaxKind.OnKeyword:
				case SyntaxKind.SelectKeyword:
				case SyntaxKind.AggregateKeyword:
				case SyntaxKind.AscendingKeyword:
				case SyntaxKind.DescendingKeyword:
				case SyntaxKind.DistinctKeyword:
				case SyntaxKind.FromKeyword:
				case SyntaxKind.GroupKeyword:
				case SyntaxKind.IntoKeyword:
				case SyntaxKind.JoinKeyword:
				case SyntaxKind.OrderKeyword:
				case SyntaxKind.PreserveKeyword:
				case SyntaxKind.SkipKeyword:
				case SyntaxKind.TakeKeyword:
				case SyntaxKind.WhereKeyword:
					return $"[{identifier}]";
				}
			}
			return identifier;
		}

		public override void VisitAssembly(IAssemblySymbol symbol)
		{
			string text = ((format.TypeQualificationStyle == SymbolDisplayTypeQualificationStyle.NameOnly) ? symbol.Identity.Name : symbol.Identity.GetDisplayName());
			builder.Add(CreatePart(SymbolDisplayPartKind.AssemblyName, symbol, text, noEscaping: false));
		}

		public override void VisitLabel(ILabelSymbol symbol)
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.LabelName, symbol, symbol.Name, noEscaping: false));
		}

		public override void VisitAlias(IAliasSymbol symbol)
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.LocalName, symbol, symbol.Name, noEscaping: false));
			if (format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType))
			{
				AddPunctuation(SyntaxKind.EqualsToken);
				symbol.Target.Accept(this);
			}
		}

		public override void VisitModule(IModuleSymbol symbol)
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.ModuleName, symbol, symbol.Name, noEscaping: false));
		}

		public override void VisitNamespace(INamespaceSymbol symbol)
		{
			if (isFirstSymbolVisited && format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeNamespaceKeyword))
			{
				AddKeyword(SyntaxKind.NamespaceKeyword);
				AddSpace();
			}
			VisitNamespace(symbol, string.Empty);
		}

		private void VisitNamespace(INamespaceSymbol symbol, string emittedName)
		{
			string text = symbol.Name;
			string text2 = string.Empty;
			if (!emittedName.IsEmpty())
			{
				int num = emittedName.LastIndexOf('.');
				if (num > -1)
				{
					text = emittedName.Substring(num + 1);
					text2 = emittedName.Substring(0, num);
				}
				else
				{
					text = emittedName;
				}
			}
			if (base.IsMinimizing)
			{
				if (!TryAddAlias(symbol, builder))
				{
					MinimallyQualify(symbol, text, text2);
				}
				return;
			}
			bool noEscaping = false;
			if (format.TypeQualificationStyle == SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
			{
				INamespaceSymbol containingNamespace = symbol.ContainingNamespace;
				if (containingNamespace == null && format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeNamespaceKeyword))
				{
					AddKeyword(SyntaxKind.NamespaceKeyword);
					AddSpace();
				}
				if (ShouldVisitNamespace(containingNamespace))
				{
					VisitNamespace(containingNamespace, text2);
					AddOperator(SyntaxKind.DotToken);
					noEscaping = true;
				}
			}
			if (symbol.IsGlobalNamespace)
			{
				AddGlobalNamespace(symbol);
			}
			else
			{
				builder.Add(CreatePart(SymbolDisplayPartKind.NamespaceName, symbol, text, noEscaping));
			}
		}

		private void AddGlobalNamespace(INamespaceSymbol symbol)
		{
			switch (format.GlobalNamespaceStyle)
			{
			case SymbolDisplayGlobalNamespaceStyle.Included:
				builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, symbol, SyntaxFacts.GetText(SyntaxKind.GlobalKeyword), noEscaping: true));
				break;
			case SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining:
				builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, symbol, SyntaxFacts.GetText(SyntaxKind.GlobalKeyword), noEscaping: true));
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(format.GlobalNamespaceStyle);
			case SymbolDisplayGlobalNamespaceStyle.Omitted:
				break;
			}
		}

		public override void VisitLocal(ILocalSymbol symbol)
		{
			string text = symbol.Name ?? "<anonymous local>";
			if (symbol.IsConst)
			{
				builder.Add(CreatePart(SymbolDisplayPartKind.ConstantName, symbol, text, noEscaping: false));
			}
			else
			{
				builder.Add(CreatePart(SymbolDisplayPartKind.LocalName, symbol, text, noEscaping: false));
			}
			if (format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType))
			{
				AddSpace();
				AddKeyword(SyntaxKind.AsKeyword);
				AddSpace();
				symbol.Type.Accept(this);
			}
			if (symbol.IsConst && symbol.HasConstantValue && format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeConstantValue))
			{
				AddSpace();
				AddPunctuation(SyntaxKind.EqualsToken);
				AddSpace();
				AddConstantValue(symbol.Type, RuntimeHelpers.GetObjectValue(symbol.ConstantValue));
			}
		}

		public override void VisitRangeVariable(IRangeVariableSymbol symbol)
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.RangeVariableName, symbol, symbol.Name, noEscaping: false));
			if (format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType) && symbol is RangeVariableSymbol rangeVariableSymbol)
			{
				AddSpace();
				AddKeyword(SyntaxKind.AsKeyword);
				AddSpace();
				((ISymbol)rangeVariableSymbol.Type).Accept((SymbolVisitor)this);
			}
		}

		protected override void AddSpace()
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.Space, null, " ", noEscaping: false));
		}

		private void AddOperator(SyntaxKind operatorKind)
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.Operator, null, SyntaxFacts.GetText(operatorKind), noEscaping: false));
		}

		private void AddPunctuation(SyntaxKind punctuationKind)
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.Punctuation, null, SyntaxFacts.GetText(punctuationKind), noEscaping: false));
		}

		private void AddPseudoPunctuation(string text)
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.Punctuation, null, text, noEscaping: false));
		}

		private void AddKeyword(SyntaxKind keywordKind)
		{
			builder.Add(CreatePart(SymbolDisplayPartKind.Keyword, null, SyntaxFacts.GetText(keywordKind), noEscaping: false));
		}

		private void AddAccessibilityIfRequired(ISymbol symbol)
		{
			INamedTypeSymbol containingType = symbol.ContainingType;
			if (format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeAccessibility) && (containingType == null || (containingType.TypeKind != TypeKind.Interface && !IsEnumMember(symbol))))
			{
				switch (symbol.DeclaredAccessibility)
				{
				case Accessibility.Private:
					AddKeyword(SyntaxKind.PrivateKeyword);
					break;
				case Accessibility.Internal:
					AddKeyword(SyntaxKind.FriendKeyword);
					break;
				case Accessibility.Protected:
					AddKeyword(SyntaxKind.ProtectedKeyword);
					break;
				case Accessibility.ProtectedAndInternal:
					AddKeyword(SyntaxKind.PrivateKeyword);
					AddSpace();
					AddKeyword(SyntaxKind.ProtectedKeyword);
					break;
				case Accessibility.ProtectedOrInternal:
					AddKeyword(SyntaxKind.ProtectedKeyword);
					AddSpace();
					AddKeyword(SyntaxKind.FriendKeyword);
					break;
				case Accessibility.Public:
					AddKeyword(SyntaxKind.PublicKeyword);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility);
				}
				AddSpace();
			}
		}

		private bool ShouldVisitNamespace(ISymbol containingSymbol)
		{
			if (containingSymbol != null && containingSymbol.Kind == SymbolKind.Namespace && format.TypeQualificationStyle == SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
			{
				if (((INamespaceSymbol)containingSymbol).IsGlobalNamespace)
				{
					return format.GlobalNamespaceStyle == SymbolDisplayGlobalNamespaceStyle.Included;
				}
				return true;
			}
			return false;
		}

		private bool IncludeNamedType(INamedTypeSymbol namedType)
		{
			if (namedType != null)
			{
				if (namedType.IsScriptClass)
				{
					return format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.IncludeScriptType);
				}
				return true;
			}
			return false;
		}

		private static bool IsEnumMember(ISymbol symbol)
		{
			if (symbol != null && symbol.Kind == SymbolKind.Field && symbol.ContainingType != null && symbol.ContainingType.TypeKind == TypeKind.Enum)
			{
				return EmbeddedOperators.CompareString(symbol.Name, "value__", TextCompare: false) != 0;
			}
			return false;
		}

		protected override void AddBitwiseOr()
		{
			AddKeyword(SyntaxKind.OrKeyword);
		}

		protected override void AddExplicitlyCastedLiteralValue(INamedTypeSymbol namedType, SpecialType type, object value)
		{
			AddLiteralValue(type, RuntimeHelpers.GetObjectValue(value));
		}

		protected override void AddLiteralValue(SpecialType type, object value)
		{
			switch (type)
			{
			case SpecialType.System_String:
				SymbolDisplay.AddSymbolDisplayParts(builder, (string)value);
				break;
			case SpecialType.System_Char:
				SymbolDisplay.AddSymbolDisplayParts(builder, (char)value);
				break;
			default:
			{
				string text = SymbolDisplay.FormatPrimitive(RuntimeHelpers.GetObjectValue(value), quoteStrings: true, useHexadecimalNumbers: false);
				builder.Add(CreatePart(SymbolDisplayPartKind.NumericLiteral, null, text, noEscaping: false));
				break;
			}
			}
		}

		private void AddConstantValue(ITypeSymbol type, object constantValue, bool preferNumericValueOrExpandedFlagsForEnum = false)
		{
			if (constantValue != null)
			{
				AddNonNullConstantValue(type, RuntimeHelpers.GetObjectValue(constantValue), preferNumericValueOrExpandedFlagsForEnum);
			}
			else
			{
				AddKeyword(SyntaxKind.NothingKeyword);
			}
		}

		protected override bool ShouldRestrictMinimallyQualifyLookupToNamespacesAndTypes()
		{
			if (!SyntaxFacts.IsInNamespaceOrTypeContext(semanticModelOpt.SyntaxTree.GetRoot().FindToken(positionOpt).Parent as ExpressionSyntax))
			{
				return inNamespaceOrType;
			}
			return true;
		}

		private void MinimallyQualify(INamespaceSymbol symbol, string emittedName, string parentsEmittedName)
		{
			if (symbol.IsGlobalNamespace)
			{
				return;
			}
			bool noEscaping = false;
			ImmutableArray<ISymbol> immutableArray = (ShouldRestrictMinimallyQualifyLookupToNamespacesAndTypes() ? semanticModelOpt.LookupNamespacesAndTypes(positionOpt, null, symbol.Name) : semanticModelOpt.LookupSymbols(positionOpt, null, symbol.Name));
			NamespaceSymbol namespaceSymbol = null;
			if (immutableArray.Length == 1)
			{
				namespaceSymbol = immutableArray[0] as NamespaceSymbol;
			}
			if ((object)namespaceSymbol != null && namespaceSymbol != symbol && namespaceSymbol.Extent.Kind == NamespaceKind.Compilation && symbol is NamespaceSymbol namespaceSymbol2 && namespaceSymbol2.Extent.Kind != NamespaceKind.Compilation && (object)namespaceSymbol.Extent.Compilation.GetCompilationNamespace(namespaceSymbol2) == namespaceSymbol)
			{
				namespaceSymbol = namespaceSymbol2;
			}
			if (namespaceSymbol != symbol)
			{
				INamespaceSymbol containingNamespace = symbol.ContainingNamespace;
				if (containingNamespace != null)
				{
					if (containingNamespace.IsGlobalNamespace)
					{
						if (format.GlobalNamespaceStyle == SymbolDisplayGlobalNamespaceStyle.Included)
						{
							AddGlobalNamespace(containingNamespace);
							AddOperator(SyntaxKind.DotToken);
							noEscaping = true;
						}
					}
					else
					{
						VisitNamespace(containingNamespace, parentsEmittedName);
						AddOperator(SyntaxKind.DotToken);
						noEscaping = true;
					}
				}
			}
			builder.Add(CreatePart(SymbolDisplayPartKind.NamespaceName, symbol, emittedName, noEscaping));
		}

		private void MinimallyQualify(INamedTypeSymbol symbol)
		{
			bool noEscaping = false;
			if (!symbol.IsAnonymousType && !symbol.IsTupleType && !NameBoundSuccessfullyToSameSymbol(symbol))
			{
				if (IncludeNamedType(symbol.ContainingType))
				{
					symbol.ContainingType.Accept(base.NotFirstVisitor);
					AddOperator(SyntaxKind.DotToken);
				}
				else if (symbol.ContainingNamespace != null)
				{
					if (symbol.ContainingNamespace.IsGlobalNamespace)
					{
						if (symbol.TypeKind != TypeKind.Error)
						{
							AddKeyword(SyntaxKind.GlobalKeyword);
							AddOperator(SyntaxKind.DotToken);
						}
					}
					else
					{
						symbol.ContainingNamespace.Accept(base.NotFirstVisitor);
						AddOperator(SyntaxKind.DotToken);
					}
				}
				noEscaping = true;
			}
			AddNameAndTypeArgumentsOrParameters(symbol, noEscaping);
		}

		private bool TryAddAlias(INamespaceOrTypeSymbol symbol, ArrayBuilder<SymbolDisplayPart> builder)
		{
			IAliasSymbol aliasSymbol = GetAliasSymbol(symbol);
			if (aliasSymbol != null)
			{
				string name = aliasSymbol.Name;
				ImmutableArray<ISymbol> immutableArray = semanticModelOpt.LookupNamespacesAndTypes(positionOpt, null, name);
				if (immutableArray.Length == 1 && immutableArray[0] is IAliasSymbol aliasSymbol2 && aliasSymbol2.Target.Equals(symbol))
				{
					builder.Add(CreatePart(SymbolDisplayPartKind.AliasName, aliasSymbol, name, noEscaping: false));
					return true;
				}
			}
			return false;
		}

		private IAliasSymbol GetAliasSymbol(INamespaceOrTypeSymbol symbol)
		{
			if (!base.IsMinimizing)
			{
				return null;
			}
			if (semanticModelOpt.SyntaxTree.GetRoot().FindToken(positionOpt).Parent!.AncestorsAndSelf().OfType<ImportsClauseSyntax>().FirstOrDefault() != null)
			{
				return null;
			}
			SourceFile sourceFile = ((SourceModuleSymbol)semanticModelOpt.Compilation.SourceModule).TryGetSourceFile((VisualBasicSyntaxTree)GetSyntaxTree(semanticModelOpt));
			if (sourceFile.AliasImportsOpt != null)
			{
				foreach (AliasAndImportsClausePosition value in sourceFile.AliasImportsOpt.Values)
				{
					if (value.Alias.Target == (NamespaceOrTypeSymbol)symbol)
					{
						return value.Alias;
					}
				}
			}
			return null;
		}

		private SyntaxTree GetSyntaxTree(SemanticModel semanticModel)
		{
			if (!semanticModel.IsSpeculativeSemanticModel)
			{
				return semanticModel.SyntaxTree;
			}
			return semanticModel.ParentModel!.SyntaxTree;
		}

		private string RemoveAttributeSuffixIfNecessary(INamedTypeSymbol symbol, string symbolName)
		{
			if (format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix) && IsDerivedFromAttributeType(symbol))
			{
				string result = null;
				if (symbolName.TryGetWithoutAttributeSuffix(isCaseSensitive: false, out result) && VisualBasicExtensions.Kind(SyntaxFactory.ParseToken(result)) == SyntaxKind.IdentifierToken)
				{
					symbolName = result;
				}
			}
			return symbolName;
		}

		private bool IsDerivedFromAttributeType(INamedTypeSymbol derivedType)
		{
			if (semanticModelOpt != null)
			{
				NamedTypeSymbol derivedType2 = (NamedTypeSymbol)derivedType;
				VisualBasicCompilation compilation = (VisualBasicCompilation)semanticModelOpt.Compilation;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				return TypeSymbolExtensions.IsOrDerivedFromWellKnownClass(derivedType2, WellKnownType.System_Attribute, compilation, ref useSiteInfo);
			}
			return false;
		}
	}
}
