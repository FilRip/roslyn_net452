Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.SymbolDisplay
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SymbolDisplayVisitor
		Inherits AbstractSymbolDisplayVisitor
		Private ReadOnly _escapeKeywordIdentifiers As Boolean

		Friend Sub New(ByVal builder As ArrayBuilder(Of SymbolDisplayPart), ByVal format As SymbolDisplayFormat, ByVal semanticModelOpt As SemanticModel, ByVal positionOpt As Integer)
			MyBase.New(builder, format, True, semanticModelOpt, positionOpt, False)
			Me._escapeKeywordIdentifiers = format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers)
		End Sub

		Private Sub New(ByVal builder As ArrayBuilder(Of SymbolDisplayPart), ByVal format As SymbolDisplayFormat, ByVal semanticModelOpt As SemanticModel, ByVal positionOpt As Integer, ByVal escapeKeywordIdentifiers As Boolean, ByVal isFirstSymbolVisited As Boolean, Optional ByVal inNamespaceOrType As Boolean = False)
			MyBase.New(builder, format, isFirstSymbolVisited, semanticModelOpt, positionOpt, inNamespaceOrType)
			Me._escapeKeywordIdentifiers = escapeKeywordIdentifiers
		End Sub

		Private Sub AddAccessibilityIfRequired(ByVal symbol As ISymbol)
			Dim containingType As INamedTypeSymbol = symbol.ContainingType
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeAccessibility) AndAlso (containingType Is Nothing OrElse containingType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Interface] AndAlso Not SymbolDisplayVisitor.IsEnumMember(symbol))) Then
				Select Case symbol.DeclaredAccessibility
					Case Accessibility.[Private]
						Me.AddKeyword(SyntaxKind.PrivateKeyword)
						Exit Select
					Case Accessibility.ProtectedAndInternal
						Me.AddKeyword(SyntaxKind.PrivateKeyword)
						Me.AddSpace()
						Me.AddKeyword(SyntaxKind.ProtectedKeyword)
						Exit Select
					Case Accessibility.[Protected]
						Me.AddKeyword(SyntaxKind.ProtectedKeyword)
						Exit Select
					Case Accessibility.Internal
						Me.AddKeyword(SyntaxKind.FriendKeyword)
						Exit Select
					Case Accessibility.ProtectedOrInternal
						Me.AddKeyword(SyntaxKind.ProtectedKeyword)
						Me.AddSpace()
						Me.AddKeyword(SyntaxKind.FriendKeyword)
						Exit Select
					Case Accessibility.[Public]
						Me.AddKeyword(SyntaxKind.PublicKeyword)
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility)
				End Select
				Me.AddSpace()
			End If
		End Sub

		Private Sub AddAccessor(ByVal [property] As ISymbol, ByVal method As IMethodSymbol, ByVal keyword As SyntaxKind)
			If (method IsNot Nothing) Then
				Me.AddSpace()
				If (method.DeclaredAccessibility <> [property].DeclaredAccessibility) Then
					Me.AddAccessibilityIfRequired(method)
				End If
				Me.AddKeyword(keyword)
				Me.AddPunctuation(SyntaxKind.SemicolonToken)
			End If
		End Sub

		Private Sub AddAnonymousTypeName(ByVal symbol As INamedTypeSymbol)
			Dim typeKind As Microsoft.CodeAnalysis.TypeKind = symbol.TypeKind
			If (typeKind <> Microsoft.CodeAnalysis.TypeKind.[Class]) Then
				If (typeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
					Return
				End If
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.DelegateName, symbol, "<generated method>", True))
				Return
			End If
			Dim members As ImmutableArray(Of ISymbol) = symbol.GetMembers()
			Dim str As String = [String].Join(", ", members.OfType(Of IPropertySymbol)().[Select](Of String)(Function(p As IPropertySymbol) Me.CreateAnonymousTypeMember(p)))
			If (str.Length = 0) Then
				Me.builder.Add(New SymbolDisplayPart(SymbolDisplayPartKind.ClassName, symbol, "<empty anonymous type>"))
				Return
			End If
			Dim str1 As String = [String].Format("<anonymous type: {0}>", str)
			Me.builder.Add(New SymbolDisplayPart(SymbolDisplayPartKind.ClassName, symbol, str1))
		End Sub

		Private Sub AddArrayRank(ByVal symbol As IArrayTypeSymbol)
			Dim flag As Boolean = Me.format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.UseAsterisksInMultiDimensionalArrays)
			Me.AddPunctuation(SyntaxKind.OpenParenToken)
			If (symbol.Rank <= 1) Then
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				If (arrayTypeSymbol IsNot Nothing AndAlso Not arrayTypeSymbol.IsSZArray) Then
					Me.AddPunctuation(SyntaxKind.AsteriskToken)
				End If
			ElseIf (flag) Then
				Me.AddPunctuation(SyntaxKind.AsteriskToken)
			End If
			Dim num As Integer = 0
			Do
				Me.AddPunctuation(SyntaxKind.CommaToken)
				If (flag) Then
					Me.AddPunctuation(SyntaxKind.AsteriskToken)
				End If
				num = num + 1
			Loop While num < symbol.Rank - 1
			Me.AddPunctuation(SyntaxKind.CloseParenToken)
		End Sub

		Protected Overrides Sub AddBitwiseOr()
			Me.AddKeyword(SyntaxKind.OrKeyword)
		End Sub

		Private Sub AddConstantValue(ByVal type As ITypeSymbol, ByVal constantValue As Object, Optional ByVal preferNumericValueOrExpandedFlagsForEnum As Boolean = False)
			If (constantValue Is Nothing) Then
				Me.AddKeyword(SyntaxKind.NothingKeyword)
				Return
			End If
			MyBase.AddNonNullConstantValue(type, RuntimeHelpers.GetObjectValue(constantValue), preferNumericValueOrExpandedFlagsForEnum)
		End Sub

		Private Sub AddCustomModifiersIfRequired(ByVal customModifiers As ImmutableArray(Of CustomModifier), Optional ByVal leadingSpace As Boolean = True, Optional ByVal trailingSpace As Boolean = False)
			If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.IncludeCustomModifiers) AndAlso Not customModifiers.IsEmpty) Then
				Dim flag As Boolean = True
				Dim enumerator As ImmutableArray(Of CustomModifier).Enumerator = customModifiers.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As CustomModifier = enumerator.Current
					If (Not flag OrElse leadingSpace) Then
						Me.AddSpace()
					End If
					flag = False
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.ClassName Or SymbolDisplayPartKind.RecordStructName, Nothing, If(current.IsOptional, "modopt", "modreq"), True))
					Me.AddPunctuation(SyntaxKind.OpenParenToken)
					current.Modifier.Accept(MyBase.NotFirstVisitor)
					Me.AddPunctuation(SyntaxKind.CloseParenToken)
				End While
				If (trailingSpace) Then
					Me.AddSpace()
				End If
			End If
		End Sub

		Protected Overrides Sub AddExplicitlyCastedLiteralValue(ByVal namedType As INamedTypeSymbol, ByVal type As SpecialType, ByVal value As Object)
			Me.AddLiteralValue(type, RuntimeHelpers.GetObjectValue(value))
		End Sub

		Private Sub AddFieldModifiersIfRequired(ByVal symbol As IFieldSymbol)
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeModifiers) AndAlso Not SymbolDisplayVisitor.IsEnumMember(symbol)) Then
				If (symbol.IsConst) Then
					Me.AddKeyword(SyntaxKind.ConstKeyword)
					Me.AddSpace()
				End If
				If (symbol.IsReadOnly) Then
					Me.AddKeyword(SyntaxKind.ReadOnlyKeyword)
					Me.AddSpace()
				End If
			End If
		End Sub

		Private Sub AddGlobalNamespace(ByVal symbol As INamespaceSymbol)
			Select Case Me.format.GlobalNamespaceStyle
				Case SymbolDisplayGlobalNamespaceStyle.Omitted
					Return
				Case SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.Keyword, symbol, SyntaxFacts.GetText(SyntaxKind.GlobalKeyword), True))
					Return
				Case SymbolDisplayGlobalNamespaceStyle.Included
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.Keyword, symbol, SyntaxFacts.GetText(SyntaxKind.GlobalKeyword), True))
					Return
			End Select
			Throw ExceptionUtilities.UnexpectedValue(Me.format.GlobalNamespaceStyle)
		End Sub

		Private Sub AddKeyword(ByVal keywordKind As SyntaxKind)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.Keyword, Nothing, SyntaxFacts.GetText(keywordKind), False))
		End Sub

		Protected Overrides Sub AddLiteralValue(ByVal type As Microsoft.CodeAnalysis.SpecialType, ByVal value As Object)
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = type
			If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Char) Then
				Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.AddSymbolDisplayParts(Me.builder, CChar(value))
				Return
			End If
			If (specialType = Microsoft.CodeAnalysis.SpecialType.System_String) Then
				Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.AddSymbolDisplayParts(Me.builder, CStr(value))
				Return
			End If
			Dim str As String = Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.FormatPrimitive(RuntimeHelpers.GetObjectValue(value), True, False)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.NumericLiteral, Nothing, str, False))
		End Sub

		Private Sub AddMemberModifiersIfRequired(ByVal symbol As ISymbol)
			Dim containingSymbol As INamedTypeSymbol = TryCast(symbol.ContainingSymbol, INamedTypeSymbol)
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeModifiers) AndAlso (containingSymbol Is Nothing OrElse containingSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Interface] AndAlso Not SymbolDisplayVisitor.IsEnumMember(symbol))) Then
				Dim flag As Boolean = If(symbol.Kind <> SymbolKind.Field, False, DirectCast(symbol, IFieldSymbol).IsConst)
				If (symbol.IsStatic AndAlso (containingSymbol Is Nothing OrElse containingSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Module]) AndAlso Not flag) Then
					Me.AddKeyword(SyntaxKind.SharedKeyword)
					Me.AddSpace()
				End If
				If (Not Me.IsWithEventsProperty(symbol)) Then
					If (symbol.IsAbstract) Then
						Me.AddKeyword(SyntaxKind.MustOverrideKeyword)
						Me.AddSpace()
					End If
					If (symbol.IsSealed) Then
						Me.AddKeyword(SyntaxKind.NotOverridableKeyword)
						Me.AddSpace()
					End If
					If (symbol.IsVirtual) Then
						Me.AddKeyword(SyntaxKind.OverridableKeyword)
						Me.AddSpace()
					End If
					If (Me.IsOverloads(symbol) AndAlso Not symbol.IsOverride) Then
						Me.AddKeyword(SyntaxKind.OverloadsKeyword)
						Me.AddSpace()
					End If
					If (symbol.IsOverride) Then
						Me.AddKeyword(SyntaxKind.OverridesKeyword)
						Me.AddSpace()
					End If
				End If
			End If
		End Sub

		Private Sub AddMethodGenericParameters(ByVal method As IMethodSymbol)
			If (method.Arity > 0 AndAlso Me.format.GenericsOptions.IncludesOption(SymbolDisplayGenericsOptions.IncludeTypeParameters)) Then
				Me.AddTypeArguments(method.TypeArguments, Nothing)
			End If
		End Sub

		Private Sub AddMethodKind(ByVal symbol As IMethodSymbol)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			If (Not Me.format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeMemberKeyword)) Then
				Return
			End If
			Select Case symbol.MethodKind
				Case MethodKind.AnonymousFunction
				Case MethodKind.DelegateInvoke
				Case MethodKind.Ordinary
				Case MethodKind.ReducedExtension
					If (symbol.ReturnsVoid) Then
						Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
						Me.AddSpace()
						Return
					End If
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword)
					Me.AddSpace()
					Return
				Case MethodKind.Constructor
				Case MethodKind.StaticConstructor
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
					Me.AddSpace()
					Return
				Case MethodKind.Conversion
					If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword)
						Me.AddSpace()
						Return
					End If
					If (Not CaseInsensitiveComparison.Equals(symbol.Name, "op_Implicit")) Then
						Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NarrowingKeyword)
						Me.AddSpace()
					Else
						Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword)
						Me.AddSpace()
					End If
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword)
					Me.AddSpace()
					Return
				Case MethodKind.Destructor
				Case MethodKind.ExplicitInterfaceImplementation
					Throw ExceptionUtilities.UnexpectedValue(symbol.MethodKind)
				Case MethodKind.EventAdd
				Case MethodKind.EventRaise
				Case MethodKind.EventRemove
					If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
						Me.AddSpace()
						Return
					End If
					If (symbol.MethodKind = MethodKind.EventAdd) Then
						syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerKeyword
					Else
						syntaxKind = If(symbol.MethodKind = MethodKind.EventRemove, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword, Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword)
					End If
					Me.AddKeyword(syntaxKind)
					Me.AddSpace()
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventKeyword)
					Me.AddSpace()
					Return
				Case MethodKind.UserDefinedOperator
				Case MethodKind.BuiltinOperator
					If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword)
						Me.AddSpace()
						Return
					End If
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword)
					Me.AddSpace()
					Return
				Case MethodKind.PropertyGet
					If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionKeyword)
						Me.AddSpace()
						Return
					End If
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword)
					Me.AddSpace()
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetKeyword)
					Me.AddSpace()
					Return
				Case MethodKind.PropertySet
					If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword)
						Me.AddSpace()
						Return
					End If
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword)
					Me.AddSpace()
					Me.AddKeyword(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword)
					Me.AddSpace()
					Return
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(symbol.MethodKind)
			End Select
		End Sub

		Private Sub AddMethodName(ByVal symbol As IMethodSymbol)
			Dim containingSymbol As ITypeSymbol
			Dim flag As Boolean = False
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeContainingType)) Then
				If (symbol.MethodKind <> MethodKind.ReducedExtension) Then
					containingSymbol = TryCast(symbol.ContainingSymbol, INamedTypeSymbol)
				Else
					containingSymbol = symbol.ReceiverType
				End If
				If (containingSymbol IsNot Nothing) Then
					containingSymbol.Accept(MyBase.NotFirstVisitor)
					Me.AddOperator(SyntaxKind.DotToken)
					flag = True
				End If
			End If
			Select Case symbol.MethodKind
				Case MethodKind.AnonymousFunction
					Return
				Case MethodKind.Constructor
				Case MethodKind.StaticConstructor
					If (Not Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.AddKeyword(SyntaxKind.NewKeyword)
						Return
					End If
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, flag))
					Return
				Case MethodKind.Conversion
					If (Not Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.AddKeyword(SyntaxKind.CTypeKeyword)
						Return
					End If
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, flag))
					Return
				Case MethodKind.DelegateInvoke
				Case MethodKind.Ordinary
				Case MethodKind.DeclareMethod
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, flag))
					Return
				Case MethodKind.Destructor
				Case MethodKind.ExplicitInterfaceImplementation
					Throw ExceptionUtilities.UnexpectedValue(symbol.MethodKind)
				Case MethodKind.EventAdd
				Case MethodKind.EventRaise
				Case MethodKind.EventRemove
				Case MethodKind.PropertyGet
				Case MethodKind.PropertySet
					If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, flag))
						Return
					End If
					Dim associatedSymbol As ISymbol = symbol.AssociatedSymbol
					If (associatedSymbol.Kind = SymbolKind.[Property]) Then
						Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.PropertyName, associatedSymbol, associatedSymbol.Name, flag))
						Return
					End If
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.EventName, associatedSymbol, associatedSymbol.Name, flag))
					Return
				Case MethodKind.UserDefinedOperator
				Case MethodKind.BuiltinOperator
					If (Not Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames)) Then
						Me.AddKeyword(OverloadResolution.GetOperatorTokenKind(symbol.Name))
						Return
					End If
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.MethodName, symbol, symbol.Name, flag))
					Return
				Case MethodKind.ReducedExtension
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.ExtensionMethodName, symbol, symbol.Name, flag))
					Return
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(symbol.MethodKind)
			End Select
		End Sub

		Private Sub AddMethodParameters(ByVal method As IMethodSymbol)
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeParameters)) Then
				Me.AddPunctuation(SyntaxKind.OpenParenToken)
				Me.AddParametersIfRequired(If(Not method.IsExtensionMethod, False, method.MethodKind <> MethodKind.ReducedExtension), method.Parameters)
				Me.AddPunctuation(SyntaxKind.CloseParenToken)
			End If
		End Sub

		Private Sub AddMethodReturnType(ByVal method As IMethodSymbol)
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType)) Then
				Dim methodKind As Microsoft.CodeAnalysis.MethodKind = method.MethodKind
				If (methodKind <> Microsoft.CodeAnalysis.MethodKind.Constructor AndAlso methodKind <> Microsoft.CodeAnalysis.MethodKind.StaticConstructor AndAlso Not method.ReturnsVoid) Then
					Me.AddSpace()
					Me.AddKeyword(SyntaxKind.AsKeyword)
					Me.AddSpace()
					method.ReturnType.Accept(MyBase.NotFirstVisitor)
				End If
				Me.AddCustomModifiersIfRequired(method.ReturnTypeCustomModifiers, True, False)
			End If
		End Sub

		Private Sub AddNameAndTypeArgumentsOrParameters(ByVal symbol As INamedTypeSymbol, ByVal noEscaping As Boolean)
			Dim symbolDisplayPartKind As Microsoft.CodeAnalysis.SymbolDisplayPartKind
			Dim fullTypeName As String = Nothing
			Dim flag As Boolean = False
			If (symbol.IsAnonymousType) Then
				Me.AddAnonymousTypeName(symbol)
				Return
			End If
			If (symbol.IsTupleType) Then
				If (SymbolDisplayVisitor.HasNonDefaultTupleElements(symbol) OrElse SymbolDisplayVisitor.CanUseTupleTypeName(symbol)) Then
					Me.AddTupleTypeName(symbol)
					Return
				End If
				symbol = symbol.TupleUnderlyingType
			End If
			Dim noPiaIllegalGenericInstantiationSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaIllegalGenericInstantiationSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaIllegalGenericInstantiationSymbol)
			If (noPiaIllegalGenericInstantiationSymbol Is Nothing) Then
				Dim noPiaAmbiguousCanonicalTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaAmbiguousCanonicalTypeSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaAmbiguousCanonicalTypeSymbol)
				If (noPiaAmbiguousCanonicalTypeSymbol Is Nothing) Then
					Dim noPiaMissingCanonicalTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaMissingCanonicalTypeSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaMissingCanonicalTypeSymbol)
					If (noPiaMissingCanonicalTypeSymbol IsNot Nothing) Then
						fullTypeName = noPiaMissingCanonicalTypeSymbol.FullTypeName
					End If
				Else
					symbol = noPiaAmbiguousCanonicalTypeSymbol.FirstCandidate
				End If
			Else
				symbol = noPiaIllegalGenericInstantiationSymbol.UnderlyingSymbol
			End If
			If (fullTypeName Is Nothing) Then
				fullTypeName = symbol.Name
			End If
			fullTypeName = If(Not Me.format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName) OrElse Not [String].IsNullOrEmpty(fullTypeName), Me.RemoveAttributeSuffixIfNecessary(symbol, fullTypeName), "?")
			Select Case symbol.TypeKind
				Case Microsoft.CodeAnalysis.TypeKind.[Class]
				Case Microsoft.CodeAnalysis.TypeKind.Submission
					symbolDisplayPartKind = Microsoft.CodeAnalysis.SymbolDisplayPartKind.ClassName
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
					symbolDisplayPartKind = Microsoft.CodeAnalysis.SymbolDisplayPartKind.DelegateName
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.Dynamic
				Case Microsoft.CodeAnalysis.TypeKind.Pointer
				Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
					Throw ExceptionUtilities.UnexpectedValue(symbol.TypeKind)
				Case Microsoft.CodeAnalysis.TypeKind.[Enum]
					symbolDisplayPartKind = Microsoft.CodeAnalysis.SymbolDisplayPartKind.EnumName
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Error]
					symbolDisplayPartKind = Microsoft.CodeAnalysis.SymbolDisplayPartKind.ErrorTypeName
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Interface]
					symbolDisplayPartKind = Microsoft.CodeAnalysis.SymbolDisplayPartKind.InterfaceName
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Module]
					symbolDisplayPartKind = Microsoft.CodeAnalysis.SymbolDisplayPartKind.ModuleName
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.Struct
					symbolDisplayPartKind = Microsoft.CodeAnalysis.SymbolDisplayPartKind.StructName
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(symbol.TypeKind)
			End Select
			Me.builder.Add(Me.CreatePart(symbolDisplayPartKind, symbol, fullTypeName, noEscaping))
			Dim flag1 As Boolean = TypeOf symbol Is MissingMetadataTypeSymbol
			If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.UseArityForGenericTypes)) Then
				If (DirectCast(symbol, NamedTypeSymbol).MangleName) Then
					Dim symbolDisplayParts As ArrayBuilder(Of SymbolDisplayPart) = Me.builder
					Dim arity As Integer = symbol.Arity
					symbolDisplayParts.Add(Me.CreatePart(Microsoft.CodeAnalysis.SymbolDisplayPartKind.AssemblyName Or Microsoft.CodeAnalysis.SymbolDisplayPartKind.RecordStructName, Nothing, [String].Concat("`", arity.ToString()), False))
				End If
			ElseIf (symbol.Arity > 0 AndAlso Me.format.GenericsOptions.IncludesOption(SymbolDisplayGenericsOptions.IncludeTypeParameters) AndAlso Not flag) Then
				If (flag1 OrElse symbol.IsUnboundGenericType) Then
					Me.AddPunctuation(SyntaxKind.OpenParenToken)
					Me.AddKeyword(SyntaxKind.OfKeyword)
					Me.AddSpace()
					Dim num As Integer = 0
					Do
						Me.AddPunctuation(SyntaxKind.CommaToken)
						num = num + 1
					Loop While num < symbol.Arity - 1
					Me.AddPunctuation(SyntaxKind.CloseParenToken)
				Else
					Me.AddTypeArguments(symbol.TypeArguments, symbol)
				End If
			End If
			If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.FlagMissingMetadataTypes) AndAlso (flag1 OrElse Not symbol.IsDefinition AndAlso TypeOf symbol.OriginalDefinition Is MissingMetadataTypeSymbol)) Then
				Me.builder.Add(Me.CreatePart(Microsoft.CodeAnalysis.SymbolDisplayPartKind.Punctuation, Nothing, "[", False))
				Me.builder.Add(Me.CreatePart(Microsoft.CodeAnalysis.SymbolDisplayPartKind.ClassName Or Microsoft.CodeAnalysis.SymbolDisplayPartKind.RecordStructName, symbol, "missing", False))
				Me.builder.Add(Me.CreatePart(Microsoft.CodeAnalysis.SymbolDisplayPartKind.Punctuation, Nothing, "]", False))
			End If
		End Sub

		Private Sub AddOperator(ByVal operatorKind As SyntaxKind)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.[Operator], Nothing, SyntaxFacts.GetText(operatorKind), False))
		End Sub

		Private Sub AddParametersIfRequired(ByVal isExtensionMethod As Boolean, ByVal parameters As ImmutableArray(Of IParameterSymbol))
			If (Me.format.ParameterOptions <> SymbolDisplayParameterOptions.None) Then
				Dim flag As Boolean = True
				Dim enumerator As ImmutableArray(Of IParameterSymbol).Enumerator = parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As IParameterSymbol = enumerator.Current
					If (Not flag) Then
						Me.AddPunctuation(SyntaxKind.CommaToken)
						Me.AddSpace()
					End If
					flag = False
					current.Accept(MyBase.NotFirstVisitor)
				End While
			End If
		End Sub

		Private Sub AddPseudoPunctuation(ByVal text As String)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.Punctuation, Nothing, text, False))
		End Sub

		Private Sub AddPunctuation(ByVal punctuationKind As SyntaxKind)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.Punctuation, Nothing, SyntaxFacts.GetText(punctuationKind), False))
		End Sub

		Protected Overrides Sub AddSpace()
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.Space, Nothing, " ", False))
		End Sub

		Private Function AddSpecialTypeKeyword(ByVal symbol As INamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim str As String = symbol.SpecialType.TryGetKeywordText()
			If (str IsNot Nothing) Then
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.Keyword, symbol, str, False))
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Sub AddTupleTypeName(ByVal symbol As INamedTypeSymbol)
			Dim tupleElements As ImmutableArray(Of IFieldSymbol) = symbol.TupleElements
			Me.AddPunctuation(SyntaxKind.OpenParenToken)
			Dim length As Integer = tupleElements.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As IFieldSymbol = tupleElements(num)
				If (num <> 0) Then
					Me.AddPunctuation(SyntaxKind.CommaToken)
					Me.AddSpace()
				End If
				If (item.IsExplicitlyNamedTupleElement) Then
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.FieldName, symbol, item.Name, False))
					Me.AddSpace()
					Me.AddKeyword(SyntaxKind.AsKeyword)
					Me.AddSpace()
				End If
				item.Type.Accept(MyBase.NotFirstVisitor)
				num = num + 1
			Loop While num <= length
			Me.AddPunctuation(SyntaxKind.CloseParenToken)
		End Sub

		Private Sub AddTypeArguments(ByVal typeArguments As ImmutableArray(Of ITypeSymbol), Optional ByVal modifiersSource As INamedTypeSymbol = Nothing)
			Me.AddPunctuation(SyntaxKind.OpenParenToken)
			Me.AddKeyword(SyntaxKind.OfKeyword)
			Me.AddSpace()
			Dim flag As Boolean = True
			Dim length As Integer = typeArguments.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As ITypeSymbol = typeArguments(num)
				If (Not flag) Then
					Me.AddPunctuation(SyntaxKind.CommaToken)
					Me.AddSpace()
				End If
				flag = False
				If (item.Kind <> SymbolKind.TypeParameter) Then
					item.Accept(MyBase.NotFirstVisitorNamespaceOrType)
				Else
					Dim typeParameterSymbol As ITypeParameterSymbol = DirectCast(item, ITypeParameterSymbol)
					Me.AddTypeParameterVarianceIfRequired(typeParameterSymbol)
					typeParameterSymbol.Accept(MyBase.NotFirstVisitor)
					If (Me.format.GenericsOptions.IncludesOption(SymbolDisplayGenericsOptions.IncludeTypeConstraints)) Then
						Me.AddTypeParameterConstraints(typeParameterSymbol)
					End If
				End If
				If (modifiersSource IsNot Nothing) Then
					Me.AddCustomModifiersIfRequired(modifiersSource.GetTypeArgumentCustomModifiers(num), True, False)
				End If
				num = num + 1
			Loop While num <= length
			Me.AddPunctuation(SyntaxKind.CloseParenToken)
		End Sub

		Private Sub AddTypeKind(ByVal symbol As INamedTypeSymbol)
			If (Me.isFirstSymbolVisited AndAlso Me.format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeTypeKeyword)) Then
				If (symbol.IsAnonymousType) Then
					Me.builder.Add(New SymbolDisplayPart(SymbolDisplayPartKind.AnonymousTypeIndicator, Nothing, "AnonymousType"))
					Me.AddSpace()
					Return
				End If
				If (symbol.IsTupleType) Then
					Me.builder.Add(New SymbolDisplayPart(SymbolDisplayPartKind.AnonymousTypeIndicator, Nothing, "Tuple"))
					Me.AddSpace()
					Return
				End If
				Dim typeKindKeyword As SyntaxKind = SymbolDisplayVisitor.GetTypeKindKeyword(symbol.TypeKind)
				If (typeKindKeyword <> SyntaxKind.None) Then
					Me.AddKeyword(typeKindKeyword)
					Me.AddSpace()
				End If
			End If
		End Sub

		Private Sub AddTypeParameterConstraints(ByVal typeParam As ITypeParameterSymbol)
			If (Me.isFirstSymbolVisited) Then
				Dim constraintTypes As ImmutableArray(Of ITypeSymbol) = typeParam.ConstraintTypes
				Dim num As Integer = SymbolDisplayVisitor.TypeParameterSpecialConstraintCount(typeParam) + constraintTypes.Length
				If (num <> 0) Then
					Me.AddSpace()
					Me.AddKeyword(SyntaxKind.AsKeyword)
					Me.AddSpace()
					If (num > 1) Then
						Me.AddPunctuation(SyntaxKind.OpenBraceToken)
					End If
					Dim flag As Boolean = False
					If (typeParam.HasReferenceTypeConstraint) Then
						Me.AddKeyword(SyntaxKind.ClassKeyword)
						flag = True
					ElseIf (typeParam.HasValueTypeConstraint) Then
						Me.AddKeyword(SyntaxKind.StructureKeyword)
						flag = True
					End If
					Dim enumerator As ImmutableArray(Of ITypeSymbol).Enumerator = constraintTypes.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As ITypeSymbol = enumerator.Current
						If (flag) Then
							Me.AddPunctuation(SyntaxKind.CommaToken)
							Me.AddSpace()
						End If
						current.Accept(MyBase.NotFirstVisitor)
						flag = True
					End While
					If (typeParam.HasConstructorConstraint) Then
						If (flag) Then
							Me.AddPunctuation(SyntaxKind.CommaToken)
							Me.AddSpace()
						End If
						Me.AddKeyword(SyntaxKind.NewKeyword)
					End If
					If (num > 1) Then
						Me.AddPunctuation(SyntaxKind.CloseBraceToken)
					End If
				End If
			End If
		End Sub

		Private Sub AddTypeParameterVarianceIfRequired(ByVal symbol As ITypeParameterSymbol)
			If (Me.format.GenericsOptions.IncludesOption(SymbolDisplayGenericsOptions.IncludeVariance)) Then
				Dim variance As VarianceKind = symbol.Variance
				If (variance = VarianceKind.Out) Then
					Me.AddKeyword(SyntaxKind.OutKeyword)
					Me.AddSpace()
				ElseIf (variance = VarianceKind.[In]) Then
					Me.AddKeyword(SyntaxKind.InKeyword)
					Me.AddSpace()
					Return
				End If
			End If
		End Sub

		Private Shared Function AlwaysEscape(ByVal kind As SymbolDisplayPartKind, ByVal text As String) As Boolean
			Dim flag As Boolean
			flag = If(kind = SymbolDisplayPartKind.Keyword OrElse Not CaseInsensitiveComparison.Equals(SyntaxFacts.GetText(SyntaxKind.REMKeyword), text) AndAlso Not CaseInsensitiveComparison.Equals(SyntaxFacts.GetText(SyntaxKind.NewKeyword), text), False, True)
			Return flag
		End Function

		<Conditional("DEBUG")>
		Private Sub AssertContainingSymbol(ByVal symbol As ISymbol)
		End Sub

		Private Function CanShowDelegateSignature(ByVal symbol As INamedTypeSymbol) As Boolean
			If (Not Me.isFirstSymbolVisited OrElse symbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate] OrElse Not symbol.IsAnonymousType AndAlso Me.format.DelegateStyle = SymbolDisplayDelegateStyle.NameOnly) Then
				Return False
			End If
			Return symbol.DelegateInvokeMethod IsNot Nothing
		End Function

		Private Shared Function CanUseTupleTypeName(ByVal tupleSymbol As INamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim tupleUnderlyingTypeOrSelf As INamedTypeSymbol = SymbolDisplayVisitor.GetTupleUnderlyingTypeOrSelf(tupleSymbol)
			If (tupleUnderlyingTypeOrSelf.Arity <> 1) Then
				While tupleUnderlyingTypeOrSelf.Arity = 8
					tupleSymbol = DirectCast(tupleUnderlyingTypeOrSelf.TypeArguments(7), INamedTypeSymbol)
					If (Not SymbolDisplayVisitor.HasNonDefaultTupleElements(tupleSymbol)) Then
						tupleUnderlyingTypeOrSelf = SymbolDisplayVisitor.GetTupleUnderlyingTypeOrSelf(tupleSymbol)
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function CreateAnonymousTypeMember(ByVal prop As IPropertySymbol) As String
			Dim str As String = Me.CreateAnonymousTypeMemberWorker(prop)
			If (Not prop.IsReadOnly) Then
				Return str
			End If
			Return [String].Concat("Key ", str)
		End Function

		Private Function CreateAnonymousTypeMemberWorker(ByVal prop As IPropertySymbol) As String
			Return [String].Concat(prop.Name, " As ", prop.Type.ToDisplayString(Me.format))
		End Function

		Friend Function CreatePart(ByVal kind As SymbolDisplayPartKind, ByVal symbol As ISymbol, ByVal text As String, ByVal noEscaping As Boolean) As SymbolDisplayPart
			Dim flag As Boolean = If((SymbolDisplayVisitor.AlwaysEscape(kind, text) OrElse Not noEscaping) AndAlso Me._escapeKeywordIdentifiers, SymbolDisplayVisitor.IsEscapable(kind), False)
			Return New SymbolDisplayPart(kind, symbol, If(flag, Me.EscapeIdentifier(text), text))
		End Function

		Private Function EscapeIdentifier(ByVal identifier As String) As String
			Dim str As String
			If (SyntaxFacts.GetKeywordKind(identifier) = SyntaxKind.None) Then
				If (MyBase.IsMinimizing) Then
					Dim contextualKeywordKind As SyntaxKind = SyntaxFacts.GetContextualKeywordKind(identifier)
					If (contextualKeywordKind <= SyntaxKind.DescendingKeyword) Then
						If (contextualKeywordKind <= SyntaxKind.OnKeyword) Then
							If (contextualKeywordKind = SyntaxKind.InKeyword OrElse contextualKeywordKind = SyntaxKind.LetKeyword OrElse contextualKeywordKind = SyntaxKind.OnKeyword) Then
								str = [String].Format("[{0}]", identifier)
								Return str
							End If
							GoTo Label1
						ElseIf (contextualKeywordKind > SyntaxKind.AggregateKeyword) Then
							If (contextualKeywordKind = SyntaxKind.AscendingKeyword OrElse contextualKeywordKind = SyntaxKind.DescendingKeyword) Then
								str = [String].Format("[{0}]", identifier)
								Return str
							End If
							GoTo Label1
						Else
							If (contextualKeywordKind = SyntaxKind.SelectKeyword OrElse contextualKeywordKind = SyntaxKind.AggregateKeyword) Then
								str = [String].Format("[{0}]", identifier)
								Return str
							End If
							GoTo Label1
						End If
					ElseIf (contextualKeywordKind <= SyntaxKind.OrderKeyword) Then
						If (contextualKeywordKind <> SyntaxKind.DistinctKeyword) Then
							Select Case contextualKeywordKind
								Case SyntaxKind.FromKeyword
								Case SyntaxKind.GroupKeyword
								Case SyntaxKind.IntoKeyword
								Case SyntaxKind.JoinKeyword
									Exit Select
								Case SyntaxKind.InferKeyword
								Case SyntaxKind.IsFalseKeyword
								Case SyntaxKind.IsTrueKeyword
									GoTo Label1
								Case Else
									If (contextualKeywordKind = SyntaxKind.OrderKeyword) Then
										Exit Select
									End If
									GoTo Label1
							End Select
						End If
					ElseIf (contextualKeywordKind <= SyntaxKind.SkipKeyword) Then
						If (contextualKeywordKind = SyntaxKind.PreserveKeyword OrElse contextualKeywordKind = SyntaxKind.SkipKeyword) Then
							str = [String].Format("[{0}]", identifier)
							Return str
						End If
						GoTo Label1
					ElseIf (contextualKeywordKind <> SyntaxKind.TakeKeyword AndAlso contextualKeywordKind <> SyntaxKind.WhereKeyword) Then
						GoTo Label1
					End If
					str = [String].Format("[{0}]", identifier)
					Return str
				End If
			Label1:
				str = identifier
			Else
				str = [String].Format("[{0}]", identifier)
			End If
			Return str
		End Function

		Private Function GetAliasSymbol(ByVal symbol As INamespaceOrTypeSymbol) As IAliasSymbol
			Dim [alias] As IAliasSymbol
			Dim enumerator As IEnumerator(Of AliasAndImportsClausePosition) = Nothing
			If (MyBase.IsMinimizing) Then
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me.semanticModelOpt.SyntaxTree
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				If (syntaxTree.GetRoot(cancellationToken).FindToken(Me.positionOpt, False).Parent.AncestorsAndSelf(True).OfType(Of ImportsClauseSyntax)().FirstOrDefault() Is Nothing) Then
					Dim sourceFile As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceFile = DirectCast(Me.semanticModelOpt.Compilation.SourceModule, SourceModuleSymbol).TryGetSourceFile(DirectCast(Me.GetSyntaxTree(Me.semanticModelOpt), VisualBasicSyntaxTree))
					If (sourceFile.AliasImportsOpt IsNot Nothing) Then
						Try
							enumerator = sourceFile.AliasImportsOpt.Values.GetEnumerator()
							While enumerator.MoveNext()
								Dim current As AliasAndImportsClausePosition = enumerator.Current
								If (current.[Alias].Target <> DirectCast(symbol, NamespaceOrTypeSymbol)) Then
									Continue While
								End If
								[alias] = current.[Alias]
								Return [alias]
							End While
						Finally
							If (enumerator IsNot Nothing) Then
								enumerator.Dispose()
							End If
						End Try
					End If
					[alias] = Nothing
				Else
					[alias] = Nothing
				End If
			Else
				[alias] = Nothing
			End If
			Return [alias]
		End Function

		Private Function GetSyntaxTree(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel) As Microsoft.CodeAnalysis.SyntaxTree
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			syntaxTree = If(semanticModel.IsSpeculativeSemanticModel, semanticModel.ParentModel.SyntaxTree, semanticModel.SyntaxTree)
			Return syntaxTree
		End Function

		Private Shared Function GetTupleUnderlyingTypeOrSelf(ByVal tupleSymbol As INamedTypeSymbol) As INamedTypeSymbol
			Return If(tupleSymbol.TupleUnderlyingType, tupleSymbol)
		End Function

		Private Shared Function GetTypeKindKeyword(ByVal typeKind As Microsoft.CodeAnalysis.TypeKind) As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind
			Select Case typeKind
				Case Microsoft.CodeAnalysis.TypeKind.[Class]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassKeyword
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateKeyword
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.Dynamic
				Case Microsoft.CodeAnalysis.TypeKind.[Error]
				Case Microsoft.CodeAnalysis.TypeKind.Pointer
				Label0:
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Enum]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumKeyword
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Interface]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceKeyword
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Module]
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.Struct
					syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxKind
		End Function

		Private Shared Function HasNonDefaultTupleElements(ByVal tupleSymbol As INamedTypeSymbol) As Boolean
			Dim isExplicitlyNamedTupleElement As Func(Of IFieldSymbol, Boolean)
			Dim tupleElements As ImmutableArray(Of IFieldSymbol) = tupleSymbol.TupleElements
			If (SymbolDisplayVisitor._Closure$__.$I33-0 Is Nothing) Then
				isExplicitlyNamedTupleElement = Function(e As IFieldSymbol) e.IsExplicitlyNamedTupleElement
				SymbolDisplayVisitor._Closure$__.$I33-0 = isExplicitlyNamedTupleElement
			Else
				isExplicitlyNamedTupleElement = SymbolDisplayVisitor._Closure$__.$I33-0
			End If
			Return tupleElements.Any(isExplicitlyNamedTupleElement)
		End Function

		Private Function IncludeNamedType(ByVal namedType As INamedTypeSymbol) As Boolean
			If (namedType Is Nothing) Then
				Return False
			End If
			If (Not namedType.IsScriptClass) Then
				Return True
			End If
			Return Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.IncludeScriptType)
		End Function

		Private Function IsDeclareMethod(ByVal method As IMethodSymbol) As Boolean
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(method, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (methodSymbol Is Nothing) Then
				Return False
			End If
			Return methodSymbol.MethodKind = MethodKind.DeclareMethod
		End Function

		Private Function IsDerivedFromAttributeType(ByVal derivedType As INamedTypeSymbol) As Boolean
			If (Me.semanticModelOpt Is Nothing) Then
				Return False
			End If
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(derivedType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim compilation As VisualBasicCompilation = DirectCast(Me.semanticModelOpt.Compilation, VisualBasicCompilation)
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
			Return namedTypeSymbol.IsOrDerivedFromWellKnownClass(WellKnownType.System_Attribute, compilation, discarded)
		End Function

		Private Shared Function IsEnumMember(ByVal symbol As ISymbol) As Boolean
			If (symbol Is Nothing OrElse symbol.Kind <> SymbolKind.Field OrElse symbol.ContainingType Is Nothing OrElse symbol.ContainingType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
				Return False
			End If
			Return EmbeddedOperators.CompareString(symbol.Name, "value__", False) <> 0
		End Function

		Private Shared Function IsEscapable(ByVal kind As SymbolDisplayPartKind) As Boolean
			Dim flag As Boolean
			Select Case kind
				Case SymbolDisplayPartKind.AliasName
				Case SymbolDisplayPartKind.ClassName
				Case SymbolDisplayPartKind.DelegateName
				Case SymbolDisplayPartKind.EnumName
				Case SymbolDisplayPartKind.ErrorTypeName
				Case SymbolDisplayPartKind.EventName
				Case SymbolDisplayPartKind.FieldName
				Case SymbolDisplayPartKind.InterfaceName
				Case SymbolDisplayPartKind.LabelName
				Case SymbolDisplayPartKind.LocalName
				Case SymbolDisplayPartKind.MethodName
				Case SymbolDisplayPartKind.ModuleName
				Case SymbolDisplayPartKind.NamespaceName
				Case SymbolDisplayPartKind.ParameterName
				Case SymbolDisplayPartKind.PropertyName
				Case SymbolDisplayPartKind.StructName
				Case SymbolDisplayPartKind.TypeParameterName
				Case SymbolDisplayPartKind.RangeVariableName
					flag = True
					Exit Select
				Case SymbolDisplayPartKind.AssemblyName
				Case SymbolDisplayPartKind.Keyword
				Case SymbolDisplayPartKind.LineBreak
				Case SymbolDisplayPartKind.NumericLiteral
				Case SymbolDisplayPartKind.StringLiteral
				Case SymbolDisplayPartKind.[Operator]
				Case SymbolDisplayPartKind.Punctuation
				Case SymbolDisplayPartKind.Space
				Case SymbolDisplayPartKind.AnonymousTypeIndicator
				Case SymbolDisplayPartKind.Text
				Label0:
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		Private Function IsExplicitByRefParameter(ByVal parameter As IParameterSymbol) As Boolean
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = TryCast(parameter, Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
			If (parameterSymbol Is Nothing) Then
				Return False
			End If
			Return parameterSymbol.IsExplicitByRef
		End Function

		Private Function IsOverloads(ByVal symbol As ISymbol) As Boolean
			Dim symbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (symbol1 Is Nothing) Then
				Return False
			End If
			Return symbol1.IsOverloads()
		End Function

		Private Function IsWithEventsProperty(ByVal symbol As ISymbol) As Boolean
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
			If (propertySymbol Is Nothing) Then
				Return False
			End If
			Return propertySymbol.IsWithEvents
		End Function

		Protected Overrides Function MakeNotFirstVisitor(Optional ByVal inNamespaceOrType As Boolean = False) As AbstractSymbolDisplayVisitor
			Return New SymbolDisplayVisitor(Me.builder, Me.format, Me.semanticModelOpt, Me.positionOpt, Me._escapeKeywordIdentifiers, False, inNamespaceOrType)
		End Function

		Private Sub MinimallyQualify(ByVal symbol As INamespaceSymbol, ByVal emittedName As String, ByVal parentsEmittedName As String)
			If (Not symbol.IsGlobalNamespace) Then
				Dim flag As Boolean = False
				Dim symbols As ImmutableArray(Of ISymbol) = If(Me.ShouldRestrictMinimallyQualifyLookupToNamespacesAndTypes(), Me.semanticModelOpt.LookupNamespacesAndTypes(Me.positionOpt, Nothing, symbol.Name), Me.semanticModelOpt.LookupSymbols(Me.positionOpt, Nothing, symbol.Name, False))
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = Nothing
				If (symbols.Length = 1) Then
					item = TryCast(symbols(0), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
				End If
				If (item IsNot Nothing AndAlso item <> symbol AndAlso item.Extent.Kind = NamespaceKind.Compilation) Then
					Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
					If (namespaceSymbol IsNot Nothing AndAlso namespaceSymbol.Extent.Kind <> NamespaceKind.Compilation AndAlso CObj(item.Extent.Compilation.GetCompilationNamespace(namespaceSymbol)) = CObj(item)) Then
						item = namespaceSymbol
					End If
				End If
				If (item <> symbol) Then
					Dim containingNamespace As INamespaceSymbol = symbol.ContainingNamespace
					If (containingNamespace IsNot Nothing) Then
						If (Not containingNamespace.IsGlobalNamespace) Then
							Me.VisitNamespace(containingNamespace, parentsEmittedName)
							Me.AddOperator(SyntaxKind.DotToken)
							flag = True
						ElseIf (Me.format.GlobalNamespaceStyle = SymbolDisplayGlobalNamespaceStyle.Included) Then
							Me.AddGlobalNamespace(containingNamespace)
							Me.AddOperator(SyntaxKind.DotToken)
							flag = True
						End If
					End If
				End If
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.NamespaceName, symbol, emittedName, flag))
			End If
		End Sub

		Private Sub MinimallyQualify(ByVal symbol As INamedTypeSymbol)
			Dim flag As Boolean = False
			If (Not symbol.IsAnonymousType AndAlso Not symbol.IsTupleType AndAlso Not MyBase.NameBoundSuccessfullyToSameSymbol(symbol)) Then
				If (Me.IncludeNamedType(symbol.ContainingType)) Then
					symbol.ContainingType.Accept(MyBase.NotFirstVisitor)
					Me.AddOperator(SyntaxKind.DotToken)
				ElseIf (symbol.ContainingNamespace IsNot Nothing) Then
					If (Not symbol.ContainingNamespace.IsGlobalNamespace) Then
						symbol.ContainingNamespace.Accept(MyBase.NotFirstVisitor)
						Me.AddOperator(SyntaxKind.DotToken)
					ElseIf (symbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Error]) Then
						Me.AddKeyword(SyntaxKind.GlobalKeyword)
						Me.AddOperator(SyntaxKind.DotToken)
					End If
				End If
				flag = True
			End If
			Me.AddNameAndTypeArgumentsOrParameters(symbol, flag)
		End Sub

		Private Shared Function Quote(ByVal str As String) As String
			Return [String].Concat("""", str.Replace("""", """"""), """")
		End Function

		Private Function RemoveAttributeSuffixIfNecessary(ByVal symbol As INamedTypeSymbol, ByVal symbolName As String) As String
			If (Me.format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.RemoveAttributeSuffix) AndAlso Me.IsDerivedFromAttributeType(symbol)) Then
				Dim str As String = Nothing
				If (symbolName.TryGetWithoutAttributeSuffix(False, str) AndAlso SyntaxFactory.ParseToken(str, 0, False).Kind() = SyntaxKind.IdentifierToken) Then
					symbolName = str
				End If
			End If
			Return symbolName
		End Function

		Protected Overrides Function ShouldRestrictMinimallyQualifyLookupToNamespacesAndTypes() As Boolean
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me.semanticModelOpt.SyntaxTree
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			If (SyntaxFacts.IsInNamespaceOrTypeContext(TryCast(syntaxTree.GetRoot(cancellationToken).FindToken(Me.positionOpt, False).Parent, ExpressionSyntax))) Then
				Return True
			End If
			Return Me.inNamespaceOrType
		End Function

		Private Function ShouldVisitNamespace(ByVal containingSymbol As ISymbol) As Boolean
			If (containingSymbol Is Nothing OrElse containingSymbol.Kind <> SymbolKind.[Namespace] OrElse Me.format.TypeQualificationStyle <> SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces) Then
				Return False
			End If
			If (Not DirectCast(containingSymbol, INamespaceSymbol).IsGlobalNamespace) Then
				Return True
			End If
			Return Me.format.GlobalNamespaceStyle = SymbolDisplayGlobalNamespaceStyle.Included
		End Function

		Private Function TryAddAlias(ByVal symbol As INamespaceOrTypeSymbol, ByVal builder As ArrayBuilder(Of SymbolDisplayPart)) As Boolean
			Dim flag As Boolean
			Dim aliasSymbol As IAliasSymbol = Me.GetAliasSymbol(symbol)
			If (aliasSymbol IsNot Nothing) Then
				Dim name As String = aliasSymbol.Name
				Dim symbols As ImmutableArray(Of ISymbol) = Me.semanticModelOpt.LookupNamespacesAndTypes(Me.positionOpt, Nothing, name)
				If (symbols.Length = 1) Then
					Dim item As IAliasSymbol = TryCast(symbols(0), IAliasSymbol)
					If (item Is Nothing OrElse Not item.Target.Equals(symbol)) Then
						flag = False
						Return flag
					End If
					builder.Add(Me.CreatePart(SymbolDisplayPartKind.AliasName, aliasSymbol, name, False))
					flag = True
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Shared Function TypeParameterSpecialConstraintCount(ByVal typeParam As ITypeParameterSymbol) As Integer
			Return If(typeParam.HasReferenceTypeConstraint, 1, 0) + If(typeParam.HasValueTypeConstraint, 1, 0) + If(typeParam.HasConstructorConstraint, 1, 0)
		End Function

		Public Overrides Sub VisitAlias(ByVal symbol As IAliasSymbol)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.LocalName, symbol, symbol.Name, False))
			If (Me.format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType)) Then
				Me.AddPunctuation(SyntaxKind.EqualsToken)
				symbol.Target.Accept(Me)
			End If
		End Sub

		Public Overrides Sub VisitArrayType(ByVal symbol As IArrayTypeSymbol)
			If (Me.format.CompilerInternalOptions.IncludesOption(SymbolDisplayCompilerInternalOptions.ReverseArrayRankSpecifiers)) Then
				symbol.ElementType.Accept(Me)
				Me.AddCustomModifiersIfRequired(symbol.CustomModifiers, True, True)
				Me.AddArrayRank(symbol)
				Return
			End If
			Dim elementType As ITypeSymbol = symbol.ElementType
			While elementType.Kind = SymbolKind.ArrayType
				elementType = DirectCast(elementType, IArrayTypeSymbol).ElementType
			End While
			elementType.Accept(MyBase.NotFirstVisitor)
			Dim arrayTypeSymbol As IArrayTypeSymbol = symbol
			While arrayTypeSymbol IsNot Nothing
				Me.AddCustomModifiersIfRequired(arrayTypeSymbol.CustomModifiers, True, True)
				Me.AddArrayRank(arrayTypeSymbol)
				arrayTypeSymbol = TryCast(arrayTypeSymbol.ElementType, IArrayTypeSymbol)
			End While
		End Sub

		Public Overrides Sub VisitAssembly(ByVal symbol As IAssemblySymbol)
			Dim str As String = If(Me.format.TypeQualificationStyle = SymbolDisplayTypeQualificationStyle.NameOnly, symbol.Identity.Name, symbol.Identity.GetDisplayName(False))
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.AssemblyName, symbol, str, False))
		End Sub

		Private Sub VisitDeclareMethod(ByVal method As IMethodSymbol)
			Dim dllImportData As Microsoft.CodeAnalysis.DllImportData = method.GetDllImportData()
			Me.AddAccessibilityIfRequired(method)
			If (Me.format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeMemberKeyword)) Then
				Me.AddKeyword(SyntaxKind.DeclareKeyword)
				Me.AddSpace()
				Select Case dllImportData.CharacterSet
					Case CharSet.None
					Case CharSet.Ansi
						Me.AddKeyword(SyntaxKind.AnsiKeyword)
						Me.AddSpace()
						Exit Select
					Case CharSet.Unicode
						Me.AddKeyword(SyntaxKind.UnicodeKeyword)
						Me.AddSpace()
						Exit Select
					Case CharSet.Auto
						Me.AddKeyword(SyntaxKind.AutoKeyword)
						Me.AddSpace()
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(dllImportData.CharacterSet)
				End Select
				Me.AddKeyword(If(method.ReturnsVoid, SyntaxKind.SubKeyword, SyntaxKind.FunctionKeyword))
				Me.AddSpace()
			End If
			Me.AddMethodName(method)
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType)) Then
				Dim flag As Boolean = False
				If (dllImportData.ModuleName IsNot Nothing) Then
					Me.AddSpace()
					Me.AddKeyword(SyntaxKind.LibKeyword)
					Me.AddSpace()
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.StringLiteral, Nothing, SymbolDisplayVisitor.Quote(dllImportData.ModuleName), True))
					flag = True
				End If
				If (dllImportData.EntryPointName IsNot Nothing) Then
					Me.AddSpace()
					Me.AddKeyword(SyntaxKind.AliasKeyword)
					Me.AddSpace()
					Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.StringLiteral, Nothing, SymbolDisplayVisitor.Quote(dllImportData.EntryPointName), True))
					flag = True
				End If
				If (flag AndAlso Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeParameters)) Then
					Me.AddSpace()
				End If
			End If
			Me.AddMethodParameters(method)
			Me.AddMethodReturnType(method)
		End Sub

		Public Overrides Sub VisitDynamicType(ByVal symbol As IDynamicTypeSymbol)
			Me.AddKeyword(SyntaxKind.ObjectKeyword)
		End Sub

		Public Overrides Sub VisitEvent(ByVal symbol As IEventSymbol)
			Me.AddAccessibilityIfRequired(symbol)
			Me.AddMemberModifiersIfRequired(symbol)
			If (Me.format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeMemberKeyword)) Then
				Me.AddKeyword(SyntaxKind.EventKeyword)
				Me.AddSpace()
			End If
			Dim flag As Boolean = False
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeContainingType) AndAlso Me.IncludeNamedType(symbol.ContainingType)) Then
				symbol.ContainingType.Accept(MyBase.NotFirstVisitor)
				Me.AddOperator(SyntaxKind.DotToken)
				flag = True
			End If
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.EventName, symbol, symbol.Name, flag))
			Dim sourceEventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceEventSymbol)
			If (sourceEventSymbol IsNot Nothing AndAlso sourceEventSymbol.IsTypeInferred AndAlso Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeParameters)) Then
				Me.AddPunctuation(SyntaxKind.OpenParenToken)
				Me.AddParametersIfRequired(False, StaticCast(Of IParameterSymbol).From(Of ParameterSymbol)(sourceEventSymbol.DelegateParameters))
				Me.AddPunctuation(SyntaxKind.CloseParenToken)
			End If
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType) AndAlso (sourceEventSymbol Is Nothing OrElse Not sourceEventSymbol.IsTypeInferred)) Then
				Me.AddSpace()
				Me.AddKeyword(SyntaxKind.AsKeyword)
				Me.AddSpace()
				symbol.Type.Accept(MyBase.NotFirstVisitor)
			End If
		End Sub

		Public Overrides Sub VisitField(ByVal symbol As IFieldSymbol)
			Me.AddAccessibilityIfRequired(symbol)
			Me.AddMemberModifiersIfRequired(symbol)
			Me.AddFieldModifiersIfRequired(symbol)
			Dim flag As Boolean = False
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeContainingType)) Then
				Dim containingSymbol As INamedTypeSymbol = TryCast(symbol.ContainingSymbol, INamedTypeSymbol)
				If (containingSymbol IsNot Nothing) Then
					containingSymbol.Accept(MyBase.NotFirstVisitor)
					Me.AddOperator(SyntaxKind.DotToken)
					flag = True
				End If
			End If
			If (symbol.ContainingType.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.EnumMemberName, symbol, symbol.Name, flag))
			ElseIf (Not symbol.IsConst) Then
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.FieldName, symbol, symbol.Name, flag))
			Else
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.ConstantName, symbol, symbol.Name, flag))
			End If
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType) AndAlso Me.isFirstSymbolVisited AndAlso Not SymbolDisplayVisitor.IsEnumMember(symbol)) Then
				Me.AddSpace()
				Me.AddKeyword(SyntaxKind.AsKeyword)
				Me.AddSpace()
				symbol.Type.Accept(MyBase.NotFirstVisitor)
				Me.AddCustomModifiersIfRequired(symbol.CustomModifiers, True, False)
			End If
			If (Me.isFirstSymbolVisited AndAlso Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeConstantValue) AndAlso symbol.IsConst AndAlso symbol.HasConstantValue) Then
				Me.AddSpace()
				Me.AddPunctuation(SyntaxKind.EqualsToken)
				Me.AddSpace()
				Me.AddConstantValue(symbol.Type, RuntimeHelpers.GetObjectValue(symbol.ConstantValue), SymbolDisplayVisitor.IsEnumMember(symbol))
			End If
		End Sub

		Public Overrides Sub VisitLabel(ByVal symbol As ILabelSymbol)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.LabelName, symbol, symbol.Name, False))
		End Sub

		Public Overrides Sub VisitLocal(ByVal symbol As ILocalSymbol)
			Dim name As String = If(symbol.Name, "<anonymous local>")
			If (Not symbol.IsConst) Then
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.LocalName, symbol, name, False))
			Else
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.ConstantName, symbol, name, False))
			End If
			If (Me.format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType)) Then
				Me.AddSpace()
				Me.AddKeyword(SyntaxKind.AsKeyword)
				Me.AddSpace()
				symbol.Type.Accept(Me)
			End If
			If (symbol.IsConst AndAlso symbol.HasConstantValue AndAlso Me.format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeConstantValue)) Then
				Me.AddSpace()
				Me.AddPunctuation(SyntaxKind.EqualsToken)
				Me.AddSpace()
				Me.AddConstantValue(symbol.Type, RuntimeHelpers.GetObjectValue(symbol.ConstantValue), False)
			End If
		End Sub

		Public Overrides Sub VisitMethod(ByVal symbol As IMethodSymbol)
			If (Me.IsDeclareMethod(symbol)) Then
				Me.VisitDeclareMethod(symbol)
				Return
			End If
			If (symbol.IsExtensionMethod AndAlso Me.format.ExtensionMethodStyle <> SymbolDisplayExtensionMethodStyle.[Default]) Then
				If (symbol.MethodKind = MethodKind.ReducedExtension AndAlso Me.format.ExtensionMethodStyle = SymbolDisplayExtensionMethodStyle.StaticMethod) Then
					symbol = symbol.GetConstructedReducedFrom()
				ElseIf (symbol.MethodKind <> MethodKind.ReducedExtension AndAlso Me.format.ExtensionMethodStyle = SymbolDisplayExtensionMethodStyle.InstanceMethod) Then
					symbol = If(symbol.ReduceExtensionMethod(symbol.Parameters.First().Type), symbol)
				End If
			End If
			Me.AddAccessibilityIfRequired(symbol)
			Me.AddMemberModifiersIfRequired(symbol)
			If (symbol.ReturnsByRef AndAlso Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeRef)) Then
				Me.AddKeyword(SyntaxKind.ByRefKeyword)
				Me.AddCustomModifiersIfRequired(symbol.RefCustomModifiers, True, False)
				Me.AddSpace()
			End If
			Me.AddMethodKind(symbol)
			Me.AddMethodName(symbol)
			Me.AddMethodGenericParameters(symbol)
			Me.AddMethodParameters(symbol)
			Me.AddMethodReturnType(symbol)
		End Sub

		Public Overrides Sub VisitModule(ByVal symbol As IModuleSymbol)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.ModuleName, symbol, symbol.Name, False))
		End Sub

		Public Overrides Sub VisitNamedType(ByVal symbol As INamedTypeSymbol)
			If (Not MyBase.IsMinimizing OrElse Not Me.TryAddAlias(symbol, Me.builder)) Then
				If (Me.format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.UseSpecialTypes)) Then
					If (Me.AddSpecialTypeKeyword(symbol)) Then
						Return
					End If
					If (Not Me.format.MiscellaneousOptions.IncludesOption(SymbolDisplayMiscellaneousOptions.ExpandNullable) AndAlso ITypeSymbolHelpers.IsNullableType(symbol) AndAlso symbol <> symbol.OriginalDefinition) Then
						symbol.TypeArguments(0).Accept(MyBase.NotFirstVisitor)
						Me.AddPunctuation(SyntaxKind.QuestionToken)
						Return
					End If
				End If
				If (MyBase.IsMinimizing OrElse symbol.IsTupleType) Then
					Me.MinimallyQualify(symbol)
					Return
				End If
				Me.AddTypeKind(symbol)
				If (Me.CanShowDelegateSignature(symbol) AndAlso (symbol.IsAnonymousType OrElse Me.format.DelegateStyle = SymbolDisplayDelegateStyle.NameAndSignature)) Then
					Dim delegateInvokeMethod As IMethodSymbol = symbol.DelegateInvokeMethod
					If (Not delegateInvokeMethod.ReturnsVoid) Then
						If (delegateInvokeMethod.ReturnsByRef AndAlso Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeRef)) Then
							Me.AddKeyword(SyntaxKind.ByRefKeyword)
							Me.AddSpace()
						End If
						Me.AddKeyword(SyntaxKind.FunctionKeyword)
					Else
						Me.AddKeyword(SyntaxKind.SubKeyword)
					End If
					Me.AddSpace()
				End If
				Dim flag As Boolean = False
				Dim containingSymbol As ISymbol = symbol.ContainingSymbol
				If (Me.ShouldVisitNamespace(containingSymbol)) Then
					flag = True
					Dim namespaceSymbol As INamespaceSymbol = DirectCast(containingSymbol, INamespaceSymbol)
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Me.VisitNamespace(namespaceSymbol, If(namedTypeSymbol IsNot Nothing, If(namedTypeSymbol.GetEmittedNamespaceName(), [String].Empty), [String].Empty))
					Me.AddOperator(SyntaxKind.DotToken)
				End If
				If (Me.format.TypeQualificationStyle = SymbolDisplayTypeQualificationStyle.NameAndContainingTypes OrElse Me.format.TypeQualificationStyle = SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces) Then
					Dim containingType As INamedTypeSymbol = symbol.ContainingType
					If (containingType IsNot Nothing) Then
						flag = True
						containingType.Accept(MyBase.NotFirstVisitor)
						Me.AddOperator(SyntaxKind.DotToken)
					End If
				End If
				Me.AddNameAndTypeArgumentsOrParameters(symbol, flag)
				If (Me.CanShowDelegateSignature(symbol)) Then
					If (symbol.IsAnonymousType OrElse Me.format.DelegateStyle = SymbolDisplayDelegateStyle.NameAndSignature OrElse Me.format.DelegateStyle = SymbolDisplayDelegateStyle.NameAndParameters) Then
						Dim methodSymbol As IMethodSymbol = symbol.DelegateInvokeMethod
						Me.AddPunctuation(SyntaxKind.OpenParenToken)
						Me.AddParametersIfRequired(False, methodSymbol.Parameters)
						Me.AddPunctuation(SyntaxKind.CloseParenToken)
					End If
					If (symbol.IsAnonymousType OrElse Me.format.DelegateStyle = SymbolDisplayDelegateStyle.NameAndSignature) Then
						Dim delegateInvokeMethod1 As IMethodSymbol = symbol.DelegateInvokeMethod
						If (Not delegateInvokeMethod1.ReturnsVoid) Then
							Me.AddSpace()
							Me.AddKeyword(SyntaxKind.AsKeyword)
							Me.AddSpace()
							delegateInvokeMethod1.ReturnType.Accept(MyBase.NotFirstVisitor)
						End If
					End If
				End If
			End If
		End Sub

		Public Overrides Sub VisitNamespace(ByVal symbol As INamespaceSymbol)
			If (Me.isFirstSymbolVisited AndAlso Me.format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeNamespaceKeyword)) Then
				Me.AddKeyword(SyntaxKind.NamespaceKeyword)
				Me.AddSpace()
			End If
			Me.VisitNamespace(symbol, [String].Empty)
		End Sub

		Private Sub VisitNamespace(ByVal symbol As INamespaceSymbol, ByVal emittedName As String)
			Dim name As String = symbol.Name
			Dim empty As String = [String].Empty
			If (Not emittedName.IsEmpty()) Then
				Dim num As Integer = emittedName.LastIndexOf("."C)
				If (num <= -1) Then
					name = emittedName
				Else
					name = emittedName.Substring(num + 1)
					empty = emittedName.Substring(0, num)
				End If
			End If
			If (Not MyBase.IsMinimizing) Then
				Dim flag As Boolean = False
				If (Me.format.TypeQualificationStyle = SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces) Then
					Dim containingNamespace As INamespaceSymbol = symbol.ContainingNamespace
					If (containingNamespace Is Nothing AndAlso Me.format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeNamespaceKeyword)) Then
						Me.AddKeyword(SyntaxKind.NamespaceKeyword)
						Me.AddSpace()
					End If
					If (Me.ShouldVisitNamespace(containingNamespace)) Then
						Me.VisitNamespace(containingNamespace, empty)
						Me.AddOperator(SyntaxKind.DotToken)
						flag = True
					End If
				End If
				If (symbol.IsGlobalNamespace) Then
					Me.AddGlobalNamespace(symbol)
					Return
				End If
				Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.NamespaceName, symbol, name, flag))
			ElseIf (Not Me.TryAddAlias(symbol, Me.builder)) Then
				Me.MinimallyQualify(symbol, name, empty)
				Return
			End If
		End Sub

		Public Overrides Sub VisitParameter(ByVal symbol As IParameterSymbol)
			If (Me.format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeOptionalBrackets) AndAlso symbol.IsOptional) Then
				Me.AddPseudoPunctuation("[")
			End If
			If (Me.format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeParamsRefOut)) Then
				If (symbol.RefKind <> RefKind.None AndAlso Me.IsExplicitByRefParameter(symbol)) Then
					Me.AddKeyword(SyntaxKind.ByRefKeyword)
					Me.AddSpace()
					Me.AddCustomModifiersIfRequired(symbol.RefCustomModifiers, False, True)
				End If
				If (symbol.IsParams) Then
					Me.AddKeyword(SyntaxKind.ParamArrayKeyword)
					Me.AddSpace()
				End If
			End If
			If (Me.format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeName)) Then
				Dim symbolDisplayPartKind As Microsoft.CodeAnalysis.SymbolDisplayPartKind = If(symbol.IsThis, Microsoft.CodeAnalysis.SymbolDisplayPartKind.Keyword, Microsoft.CodeAnalysis.SymbolDisplayPartKind.ParameterName)
				Me.builder.Add(Me.CreatePart(symbolDisplayPartKind, symbol, symbol.Name, False))
			End If
			If (Me.format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeType)) Then
				If (Me.format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeName)) Then
					Me.AddSpace()
					Me.AddKeyword(SyntaxKind.AsKeyword)
					Me.AddSpace()
				End If
				symbol.Type.Accept(MyBase.NotFirstVisitor)
				Me.AddCustomModifiersIfRequired(symbol.CustomModifiers, True, False)
			End If
			If (Me.format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeDefaultValue) AndAlso symbol.HasExplicitDefaultValue) Then
				Me.AddSpace()
				Me.AddPunctuation(SyntaxKind.EqualsToken)
				Me.AddSpace()
				Me.AddConstantValue(symbol.Type, RuntimeHelpers.GetObjectValue(symbol.ExplicitDefaultValue), False)
			End If
			If (Me.format.ParameterOptions.IncludesOption(SymbolDisplayParameterOptions.IncludeOptionalBrackets) AndAlso symbol.IsOptional) Then
				Me.AddPseudoPunctuation("]")
			End If
		End Sub

		Public Overrides Sub VisitPointerType(ByVal symbol As IPointerTypeSymbol)
			symbol.PointedAtType.Accept(MyBase.NotFirstVisitor)
			Me.AddPunctuation(SyntaxKind.AsteriskToken)
		End Sub

		Public Overrides Sub VisitProperty(ByVal symbol As IPropertySymbol)
			Me.AddAccessibilityIfRequired(symbol)
			Me.AddMemberModifiersIfRequired(symbol)
			If (Me.format.PropertyStyle = SymbolDisplayPropertyStyle.ShowReadWriteDescriptor) Then
				If (symbol.IsReadOnly) Then
					Me.AddKeyword(SyntaxKind.ReadOnlyKeyword)
					Me.AddSpace()
				ElseIf (symbol.IsWriteOnly) Then
					Me.AddKeyword(SyntaxKind.WriteOnlyKeyword)
					Me.AddSpace()
				End If
			End If
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeModifiers) AndAlso symbol.IsIndexer) Then
				Me.AddKeyword(SyntaxKind.DefaultKeyword)
				Me.AddSpace()
			End If
			If (symbol.ReturnsByRef AndAlso Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeRef)) Then
				Me.AddKeyword(SyntaxKind.ByRefKeyword)
				Me.AddCustomModifiersIfRequired(symbol.RefCustomModifiers, True, False)
				Me.AddSpace()
			End If
			If (Me.format.KindOptions.IncludesOption(SymbolDisplayKindOptions.IncludeMemberKeyword)) Then
				If (Not Me.IsWithEventsProperty(symbol)) Then
					Me.AddKeyword(SyntaxKind.PropertyKeyword)
				Else
					Me.AddKeyword(SyntaxKind.WithEventsKeyword)
				End If
				Me.AddSpace()
			End If
			Dim flag As Boolean = False
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeContainingType) AndAlso Me.IncludeNamedType(symbol.ContainingType)) Then
				symbol.ContainingType.Accept(MyBase.NotFirstVisitor)
				Me.AddOperator(SyntaxKind.DotToken)
				flag = True
			End If
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.PropertyName, symbol, symbol.Name, flag))
			If (symbol.Parameters.Length > 0 AndAlso Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeParameters)) Then
				Me.AddPunctuation(SyntaxKind.OpenParenToken)
				Me.AddParametersIfRequired(False, symbol.Parameters)
				Me.AddPunctuation(SyntaxKind.CloseParenToken)
			End If
			If (Me.format.MemberOptions.IncludesOption(SymbolDisplayMemberOptions.IncludeType)) Then
				Me.AddSpace()
				Me.AddKeyword(SyntaxKind.AsKeyword)
				Me.AddSpace()
				symbol.Type.Accept(MyBase.NotFirstVisitor)
				Me.AddCustomModifiersIfRequired(symbol.TypeCustomModifiers, True, False)
			End If
		End Sub

		Public Overrides Sub VisitRangeVariable(ByVal symbol As IRangeVariableSymbol)
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.RangeVariableName, symbol, symbol.Name, False))
			If (Me.format.LocalOptions.IncludesOption(SymbolDisplayLocalOptions.IncludeType)) Then
				Dim rangeVariableSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.RangeVariableSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.RangeVariableSymbol)
				If (rangeVariableSymbol IsNot Nothing) Then
					Me.AddSpace()
					Me.AddKeyword(SyntaxKind.AsKeyword)
					Me.AddSpace()
					DirectCast(rangeVariableSymbol.Type, ISymbol).Accept(Me)
				End If
			End If
		End Sub

		Public Overrides Sub VisitTypeParameter(ByVal symbol As ITypeParameterSymbol)
			If (Me.isFirstSymbolVisited) Then
				Me.AddTypeParameterVarianceIfRequired(symbol)
			End If
			Me.builder.Add(Me.CreatePart(SymbolDisplayPartKind.TypeParameterName, symbol, symbol.Name, False))
		End Sub
	End Class
End Namespace