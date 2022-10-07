Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class MergedTypeDeclaration
		Inherits MergedNamespaceOrTypeDeclaration
		Private _declarations As ImmutableArray(Of SingleTypeDeclaration)

		Private _children As MergedTypeDeclaration()

		Private _memberNames As ICollection(Of String)

		Private ReadOnly Shared s_identityFunc As Func(Of SingleTypeDeclaration, SingleTypeDeclaration)

		Private ReadOnly Shared s_mergeFunc As Func(Of IEnumerable(Of SingleTypeDeclaration), MergedTypeDeclaration)

		Public ReadOnly Property AnyMemberHasAttributes As Boolean
			Get
				Dim flag As Boolean
				Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = Me.Declarations.GetEnumerator()
				While True
					If (Not enumerator.MoveNext()) Then
						flag = False
						Exit While
					ElseIf (enumerator.Current.AnyMemberHasAttributes) Then
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Get
		End Property

		Public ReadOnly Property Arity As Integer
			Get
				Return Me.Declarations(0).Arity
			End Get
		End Property

		Public Shadows ReadOnly Property Children As ImmutableArray(Of MergedTypeDeclaration)
			Get
				If (Me._children Is Nothing) Then
					Interlocked.CompareExchange(Of MergedTypeDeclaration())(Me._children, Me.MakeChildren(), Nothing)
				End If
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of MergedTypeDeclaration)(Me._children)
			End Get
		End Property

		Public Property Declarations As ImmutableArray(Of SingleTypeDeclaration)
			Get
				Return Me._declarations
			End Get
			Private Set(ByVal value As ImmutableArray(Of SingleTypeDeclaration))
				Me._declarations = value
			End Set
		End Property

		Public Overrides ReadOnly Property Kind As DeclarationKind
			Get
				Return Me.Declarations(0).Kind
			End Get
		End Property

		Public ReadOnly Property MemberNames As ICollection(Of String)
			Get
				Dim func As Func(Of SingleTypeDeclaration, ICollection(Of String))
				If (Me._memberNames Is Nothing) Then
					Dim declarations As ImmutableArray(Of SingleTypeDeclaration) = Me.Declarations
					If (MergedTypeDeclaration._Closure$__.$I26-0 Is Nothing) Then
						func = Function(d As SingleTypeDeclaration) d.MemberNames
						MergedTypeDeclaration._Closure$__.$I26-0 = func
					Else
						func = MergedTypeDeclaration._Closure$__.$I26-0
					End If
					Dim strs As ICollection(Of String) = UnionCollection(Of String).Create(Of SingleTypeDeclaration)(declarations, func)
					Interlocked.CompareExchange(Of ICollection(Of String))(Me._memberNames, strs, Nothing)
				End If
				Return Me._memberNames
			End Get
		End Property

		Public ReadOnly Property NameLocations As ImmutableArray(Of Location)
			Get
				Dim immutableAndFree As ImmutableArray(Of Location)
				If (Me.Declarations.Length <> 1) Then
					Dim instance As ArrayBuilder(Of Location) = ArrayBuilder(Of Location).GetInstance()
					Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = Me.Declarations.GetEnumerator()
					While enumerator.MoveNext()
						Dim nameLocation As Location = enumerator.Current.NameLocation
						If (nameLocation Is Nothing) Then
							Continue While
						End If
						instance.Add(nameLocation)
					End While
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					Dim declarations As ImmutableArray(Of SingleTypeDeclaration) = Me.Declarations
					immutableAndFree = ImmutableArray.Create(Of Location)(declarations(0).NameLocation)
				End If
				Return immutableAndFree
			End Get
		End Property

		Public ReadOnly Property SyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Dim instance As ArrayBuilder(Of SyntaxReference) = ArrayBuilder(Of SyntaxReference).GetInstance()
				Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = Me.Declarations.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(enumerator.Current.SyntaxReference)
				End While
				Return instance.ToImmutableAndFree()
			End Get
		End Property

		Shared Sub New()
			MergedTypeDeclaration.s_identityFunc = Function(t As SingleTypeDeclaration) t
			MergedTypeDeclaration.s_mergeFunc = Function(g As IEnumerable(Of SingleTypeDeclaration)) New MergedTypeDeclaration(ImmutableArray.CreateRange(Of SingleTypeDeclaration)(g))
		End Sub

		Friend Sub New(ByVal declarations As ImmutableArray(Of SingleTypeDeclaration))
			MyBase.New(SingleNamespaceOrTypeDeclaration.BestName(Of SingleTypeDeclaration)(declarations))
			Me.Declarations = declarations
		End Sub

		Public Function GetAttributeDeclarations() As ImmutableArray(Of SyntaxList(Of AttributeListSyntax))
			Dim attributeLists As SyntaxList(Of AttributeListSyntax)
			Dim instance As ArrayBuilder(Of SyntaxList(Of AttributeListSyntax)) = ArrayBuilder(Of SyntaxList(Of AttributeListSyntax)).GetInstance()
			Dim enumerator As ImmutableArray(Of SingleTypeDeclaration).Enumerator = Me.Declarations.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SingleTypeDeclaration = enumerator.Current
				If (Not current.HasAnyAttributes) Then
					Continue While
				End If
				Dim syntax As SyntaxNode = current.SyntaxReference.GetSyntax(New CancellationToken())
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = syntax.Kind()
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock) <= CUShort((Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement))) Then
					attributeLists = DirectCast(syntax, TypeBlockSyntax).BlockStatement.AttributeLists
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock) Then
					attributeLists = DirectCast(syntax, EnumBlockSyntax).EnumStatement.AttributeLists
				Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						Throw ExceptionUtilities.UnexpectedValue(syntax.Kind())
					End If
					attributeLists = DirectCast(syntax, DelegateStatementSyntax).AttributeLists
				End If
				instance.Add(attributeLists)
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Protected Overrides Function GetDeclarationChildren() As ImmutableArray(Of Declaration)
			Return StaticCast(Of Declaration).From(Of MergedTypeDeclaration)(Me.Children)
		End Function

		Public Function GetLexicalSortKey(ByVal compilation As VisualBasicCompilation) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey
			Dim lexicalSortKey As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = New Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey(Me._declarations(0).NameLocation, compilation)
			Dim length As Integer = Me._declarations.Length - 1
			Dim num As Integer = 1
			Do
				lexicalSortKey = Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey.First(lexicalSortKey, New Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey(Me._declarations(num).NameLocation, compilation))
				num = num + 1
			Loop While num <= length
			Return lexicalSortKey
		End Function

		Private Function MakeChildren() As MergedTypeDeclaration()
			Dim singleTypeDeclarations As IEnumerable(Of SingleTypeDeclaration)
			Dim func As Func(Of SingleTypeDeclaration, IEnumerable(Of SingleTypeDeclaration))
			If (Me.Declarations.Length <> 1) Then
				Dim declarations As IEnumerable(Of SingleTypeDeclaration) = DirectCast(Me.Declarations, IEnumerable(Of SingleTypeDeclaration))
				If (MergedTypeDeclaration._Closure$__.$I20-0 Is Nothing) Then
					func = Function(d As SingleTypeDeclaration) d.Children.OfType(Of SingleTypeDeclaration)()
					MergedTypeDeclaration._Closure$__.$I20-0 = func
				Else
					func = MergedTypeDeclaration._Closure$__.$I20-0
				End If
				singleTypeDeclarations = declarations.SelectMany(Of SingleTypeDeclaration)(func)
			Else
				Dim children As ImmutableArray(Of SingleTypeDeclaration) = Me.Declarations
				children = children(0).Children
				singleTypeDeclarations = children.OfType(Of SingleTypeDeclaration)()
			End If
			Return MergedTypeDeclaration.MakeMergedTypes(singleTypeDeclarations).ToArray()
		End Function

		Friend Shared Function MakeMergedTypes(ByVal types As IEnumerable(Of SingleTypeDeclaration)) As IEnumerable(Of MergedTypeDeclaration)
			Return types.GroupBy(Of SingleTypeDeclaration)(MergedTypeDeclaration.s_identityFunc, SingleTypeDeclaration.EqualityComparer).[Select](Of MergedTypeDeclaration)(MergedTypeDeclaration.s_mergeFunc)
		End Function
	End Class
End Namespace