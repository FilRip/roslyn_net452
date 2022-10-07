Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class GenericNameSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax
		Friend ReadOnly _typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property TypeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax
			Get
				Return Me._typeArgumentList
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal identifier As IdentifierTokenSyntax, ByVal typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)
			MyBase.New(kind, identifier)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(typeArgumentList)
			Me._typeArgumentList = typeArgumentList
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal identifier As IdentifierTokenSyntax, ByVal typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, identifier)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(typeArgumentList)
			Me._typeArgumentList = typeArgumentList
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal identifier As IdentifierTokenSyntax, ByVal typeArgumentList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)
			MyBase.New(kind, errors, annotations, identifier)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(typeArgumentList)
			Me._typeArgumentList = typeArgumentList
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim typeArgumentListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax)
			If (typeArgumentListSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeArgumentListSyntax)
				Me._typeArgumentList = typeArgumentListSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitGenericName(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.GenericNameSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._identifier
			ElseIf (num = 1) Then
				greenNode = Me._typeArgumentList
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._identifier, Me._typeArgumentList)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._identifier, Me._typeArgumentList)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._typeArgumentList, IObjectWritable))
		End Sub
	End Class
End Namespace