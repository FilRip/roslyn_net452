Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedMyGroupCollectionPropertyAccessorSymbol
		Inherits SynthesizedPropertyAccessorBase(Of SynthesizedMyGroupCollectionPropertySymbol)
		Private ReadOnly _createOrDisposeMethod As String

		Friend Overrides ReadOnly Property BackingFieldSymbol As FieldSymbol
			Get
				Return MyBase.PropertyOrEvent.AssociatedField
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Public Sub New(ByVal container As SourceNamedTypeSymbol, ByVal [property] As SynthesizedMyGroupCollectionPropertySymbol, ByVal createOrDisposeMethod As String)
			MyBase.New(container, [property])
			Me._createOrDisposeMethod = createOrDisposeMethod
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeDebuggerHiddenAttribute())
		End Sub

		Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

        Friend Overrides Function GetBoundMethodBody(compilationState As TypeCompilationState, diagnostics As BindingDiagnosticBag, <Out()> Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock

            Dim containingType = DirectCast(Me.ContainingType, SourceNamedTypeSymbol)
            Dim containingTypeName As String = MakeSafeName(containingType.Name)

            Dim targetTypeName As String = PropertyOrEvent.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)

            Dim propertyName As String = MakeSafeName(PropertyOrEvent.Name)

            Dim fieldName As String = PropertyOrEvent.AssociatedField.Name

            Dim codeToParse As String =
                "Partial Class " & containingTypeName & vbCrLf &
                    "Property " & propertyName & vbCrLf &
                        GetMethodBlock(fieldName, MakeSafeName(_createOrDisposeMethod), targetTypeName) &
                    "End Property" & vbCrLf &
                "End Class" & vbCrLf

            ' TODO: It looks like Dev11 respects project level conditional compilation here.
            Dim tree = VisualBasicSyntaxTree.ParseText(codeToParse)
            Dim attributeSyntax = PropertyOrEvent.AttributeSyntax.GetVisualBasicSyntax()
            Dim diagnosticLocation As Location = attributeSyntax.GetLocation()
            Dim root As CompilationUnitSyntax = tree.GetCompilationUnitRoot()
            Dim hasErrors As Boolean = False

            For Each diag As Diagnostic In tree.GetDiagnostics(root)
                Dim vbdiag = DirectCast(diag, VBDiagnostic)

                diagnostics.Add(vbdiag.WithLocation(diagnosticLocation))

                If diag.Severity = DiagnosticSeverity.Error Then
                    hasErrors = True
                End If
            Next

            Dim classBlock = DirectCast(root.Members(0), ClassBlockSyntax)
            Dim propertyBlock = DirectCast(classBlock.Members(0), PropertyBlockSyntax)
            Dim accessorBlock As AccessorBlockSyntax = propertyBlock.Accessors(0)

            Dim boundStatement As BoundStatement

            If hasErrors Then
                boundStatement = New BoundBadStatement(accessorBlock, ImmutableArray(Of BoundNode).Empty)
            Else
                Dim typeBinder As Binder = BinderBuilder.CreateBinderForType(containingType.ContainingSourceModule, PropertyOrEvent.AttributeSyntax.SyntaxTree, containingType)
                methodBodyBinder = BinderBuilder.CreateBinderForMethodBody(Me, accessorBlock, typeBinder)

                Dim bindingDiagnostics = New BindingDiagnosticBag(DiagnosticBag.GetInstance(), diagnostics.DependenciesBag)

                boundStatement = methodBodyBinder.BindStatement(accessorBlock, bindingDiagnostics)


                For Each diag As VBDiagnostic In bindingDiagnostics.DiagnosticBag.AsEnumerable()
                    diagnostics.Add(diag.WithLocation(diagnosticLocation))
                Next

                bindingDiagnostics.DiagnosticBag.Free()

                If boundStatement.Kind = BoundKind.Block Then
                    Return DirectCast(boundStatement, BoundBlock)
                End If
            End If

            Return New BoundBlock(accessorBlock, Nothing, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(boundStatement))
        End Function

        Protected MustOverride Function GetMethodBlock(ByVal fieldName As String, ByVal createOrDisposeMethodName As String, ByVal targetTypeName As String) As String

		Private Shared Function MakeSafeName(ByVal name As String) As String
			Dim str As String
			str = If(SyntaxFacts.GetKeywordKind(name) = SyntaxKind.None, name, [String].Concat("[", name, "]"))
			Return str
		End Function
	End Class
End Namespace