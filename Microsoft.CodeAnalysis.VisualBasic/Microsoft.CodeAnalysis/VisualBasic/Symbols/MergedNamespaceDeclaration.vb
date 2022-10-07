Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class MergedNamespaceDeclaration
		Inherits MergedNamespaceOrTypeDeclaration
		Private ReadOnly _declarations As ImmutableArray(Of SingleNamespaceDeclaration)

		Private ReadOnly _multipleSpellings As Boolean

		Private _children As ImmutableArray(Of MergedNamespaceOrTypeDeclaration)

		Public Shadows ReadOnly Property Children As ImmutableArray(Of MergedNamespaceOrTypeDeclaration)
			Get
				If (Me._children.IsDefault) Then
					ImmutableInterlocked.InterlockedInitialize(Of MergedNamespaceOrTypeDeclaration)(Me._children, Me.MakeChildren())
				End If
				Return Me._children
			End Get
		End Property

		Public ReadOnly Property Declarations As ImmutableArray(Of SingleNamespaceDeclaration)
			Get
				Return Me._declarations
			End Get
		End Property

		Public ReadOnly Property HasMultipleSpellings As Boolean
			Get
				Return Me._multipleSpellings
			End Get
		End Property

		Public Overrides ReadOnly Property Kind As DeclarationKind
			Get
				Return DeclarationKind.[Namespace]
			End Get
		End Property

		Public ReadOnly Property NameLocations As ImmutableArray(Of Location)
			Get
				Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.Location)
				If (Me._declarations.Length <> 1) Then
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.Location) = ArrayBuilder(Of Microsoft.CodeAnalysis.Location).GetInstance()
					Dim enumerator As ImmutableArray(Of SingleNamespaceDeclaration).Enumerator = Me._declarations.GetEnumerator()
					While enumerator.MoveNext()
						Dim nameLocation As Microsoft.CodeAnalysis.Location = enumerator.Current.NameLocation
						If (nameLocation Is Nothing) Then
							Continue While
						End If
						instance.Add(nameLocation)
					End While
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					Dim location As Microsoft.CodeAnalysis.Location = Me._declarations(0).NameLocation
					If (location IsNot Nothing) Then
						immutableAndFree = ImmutableArray.Create(Of Microsoft.CodeAnalysis.Location)(location)
					Else
						immutableAndFree = ImmutableArray(Of Microsoft.CodeAnalysis.Location).Empty
					End If
				End If
				Return immutableAndFree
			End Get
		End Property

		Public ReadOnly Property SyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Dim instance As ArrayBuilder(Of SyntaxReference) = ArrayBuilder(Of SyntaxReference).GetInstance()
				Dim enumerator As ImmutableArray(Of SingleNamespaceDeclaration).Enumerator = Me._declarations.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SingleNamespaceDeclaration = enumerator.Current
					If (current.SyntaxReference Is Nothing) Then
						Continue While
					End If
					instance.Add(current.SyntaxReference)
				End While
				Return instance.ToImmutableAndFree()
			End Get
		End Property

		Private Sub New(ByVal declarations As ImmutableArray(Of SingleNamespaceDeclaration))
			MyBase.New([String].Empty)
			If (System.Linq.ImmutableArrayExtensions.Any(Of SingleNamespaceDeclaration)(declarations)) Then
				MyBase.Name = SingleNamespaceDeclaration.BestName(Of SingleNamespaceDeclaration)(declarations, Me._multipleSpellings)
			End If
			Me._declarations = declarations
		End Sub

		Public Shared Function Create(ByVal declarations As IEnumerable(Of SingleNamespaceDeclaration)) As MergedNamespaceDeclaration
			Return New MergedNamespaceDeclaration(ImmutableArray.CreateRange(Of SingleNamespaceDeclaration)(declarations))
		End Function

		Public Shared Function Create(ByVal ParamArray declarations As SingleNamespaceDeclaration()) As MergedNamespaceDeclaration
			Return New MergedNamespaceDeclaration(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of SingleNamespaceDeclaration)(declarations))
		End Function

		Protected Overrides Function GetDeclarationChildren() As ImmutableArray(Of Declaration)
			Return StaticCast(Of Declaration).From(Of MergedNamespaceOrTypeDeclaration)(Me.Children)
		End Function

		Public Function GetLexicalSortKey(ByVal compilation As VisualBasicCompilation) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey
			Dim singleNamespaceDeclarations As ImmutableArray(Of SingleNamespaceDeclaration) = Me._declarations
			Dim lexicalSortKey As Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey = New Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey(singleNamespaceDeclarations(0).NameLocation, compilation)
			Dim length As Integer = Me._declarations.Length - 1
			Dim num As Integer = 1
			Do
				singleNamespaceDeclarations = Me._declarations
				lexicalSortKey = Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey.First(lexicalSortKey, New Microsoft.CodeAnalysis.VisualBasic.Symbols.LexicalSortKey(singleNamespaceDeclarations(num).NameLocation, compilation))
				num = num + 1
			Loop While num <= length
			Return lexicalSortKey
		End Function

		Private Function MakeChildren() As ImmutableArray(Of MergedNamespaceOrTypeDeclaration)
			Dim enumerator As IEnumerator(Of IGrouping(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration, Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration)) = Nothing
			Dim func As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration, Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration).GetInstance()
			Dim singleTypeDeclarations As ArrayBuilder(Of SingleTypeDeclaration) = ArrayBuilder(Of SingleTypeDeclaration).GetInstance()
			Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration).Enumerator = Me._declarations.GetEnumerator()
			While enumerator1.MoveNext()
				Dim enumerator2 As ImmutableArray(Of SingleNamespaceOrTypeDeclaration).Enumerator = enumerator1.Current.Children.GetEnumerator()
				While enumerator2.MoveNext()
					Dim current As SingleNamespaceOrTypeDeclaration = enumerator2.Current
					Dim singleNamespaceDeclaration As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration)
					If (singleNamespaceDeclaration Is Nothing) Then
						singleTypeDeclarations.Add(DirectCast(current, SingleTypeDeclaration))
					Else
						instance.Add(singleNamespaceDeclaration)
					End If
				End While
			End While
			Dim mergedNamespaceOrTypeDeclarations As ArrayBuilder(Of MergedNamespaceOrTypeDeclaration) = ArrayBuilder(Of MergedNamespaceOrTypeDeclaration).GetInstance()
			Select Case instance.Count
				Case 0
					instance.Free()
					If (singleTypeDeclarations.Count <> 0) Then
						mergedNamespaceOrTypeDeclarations.AddRange(MergedTypeDeclaration.MakeMergedTypes(singleTypeDeclarations))
					End If
					singleTypeDeclarations.Free()
					Return mergedNamespaceOrTypeDeclarations.ToImmutableAndFree()
				Case 1
					mergedNamespaceOrTypeDeclarations.Add(MergedNamespaceDeclaration.Create(instance))
					instance.Free()
					If (singleTypeDeclarations.Count <> 0) Then
						mergedNamespaceOrTypeDeclarations.AddRange(MergedTypeDeclaration.MakeMergedTypes(singleTypeDeclarations))
					End If
					singleTypeDeclarations.Free()
					Return mergedNamespaceOrTypeDeclarations.ToImmutableAndFree()
				Case 2
					If (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration.EqualityComparer.Equals(instance(0), instance(1))) Then
						mergedNamespaceOrTypeDeclarations.Add(MergedNamespaceDeclaration.Create(New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration() { instance(0) }))
						mergedNamespaceOrTypeDeclarations.Add(MergedNamespaceDeclaration.Create(New Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration() { instance(1) }))
						instance.Free()
						If (singleTypeDeclarations.Count <> 0) Then
							mergedNamespaceOrTypeDeclarations.AddRange(MergedTypeDeclaration.MakeMergedTypes(singleTypeDeclarations))
						End If
						singleTypeDeclarations.Free()
						Return mergedNamespaceOrTypeDeclarations.ToImmutableAndFree()
					Else
						mergedNamespaceOrTypeDeclarations.Add(MergedNamespaceDeclaration.Create(instance))
						instance.Free()
						If (singleTypeDeclarations.Count <> 0) Then
							mergedNamespaceOrTypeDeclarations.AddRange(MergedTypeDeclaration.MakeMergedTypes(singleTypeDeclarations))
						End If
						singleTypeDeclarations.Free()
						Return mergedNamespaceOrTypeDeclarations.ToImmutableAndFree()
					End If
				Case Else
					Using enumerator
						Dim singleNamespaceDeclarations As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration) = instance
						If (MergedNamespaceDeclaration._Closure$__.$I16-0 Is Nothing) Then
							func = Function(n As Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration) n
							MergedNamespaceDeclaration._Closure$__.$I16-0 = func
						Else
							func = MergedNamespaceDeclaration._Closure$__.$I16-0
						End If
						enumerator = singleNamespaceDeclarations.GroupBy(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration)(func, Microsoft.CodeAnalysis.VisualBasic.Symbols.SingleNamespaceDeclaration.EqualityComparer).GetEnumerator()
						While enumerator.MoveNext()
							mergedNamespaceOrTypeDeclarations.Add(MergedNamespaceDeclaration.Create(enumerator.Current))
						End While
						instance.Free()
						If (singleTypeDeclarations.Count <> 0) Then
							mergedNamespaceOrTypeDeclarations.AddRange(MergedTypeDeclaration.MakeMergedTypes(singleTypeDeclarations))
						End If
						singleTypeDeclarations.Free()
						Return mergedNamespaceOrTypeDeclarations.ToImmutableAndFree()
					End Using

			End Select
		End Function
	End Class
End Namespace