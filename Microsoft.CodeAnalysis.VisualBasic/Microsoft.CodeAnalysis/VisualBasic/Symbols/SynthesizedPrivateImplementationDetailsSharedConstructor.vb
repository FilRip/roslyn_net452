Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedPrivateImplementationDetailsSharedConstructor
		Inherits SynthesizedGlobalMethodBase
		Private ReadOnly _containingModule As SourceModuleSymbol

		Private ReadOnly _privateImplementationType As PrivateImplementationDetails

		Private ReadOnly _voidType As TypeSymbol

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me._containingModule
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.StaticConstructor
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._voidType
			End Get
		End Property

		Friend Sub New(ByVal containingModule As SourceModuleSymbol, ByVal privateImplementationType As PrivateImplementationDetails, ByVal voidType As NamedTypeSymbol)
			MyBase.New(containingModule, ".cctor", privateImplementationType)
			Me._containingModule = containingModule
			Me._privateImplementationType = privateImplementationType
			Me._voidType = voidType
		End Sub

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
			Dim enumerator As IEnumerator(Of KeyValuePair(Of Integer, InstrumentationPayloadRootField)) = Nothing
			methodBodyBinder = Nothing
			Dim dummy As VisualBasicSyntaxTree = VisualBasicSyntaxTree.Dummy
			Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me, Me, dummy.GetRoot(cancellationToken), compilationState, diagnostics)
			Using instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
				enumerator = Me._privateImplementationType.GetInstrumentationPayloadRoots().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of Integer, InstrumentationPayloadRootField) = enumerator.Current
					Dim key As Integer = current.Key
					Dim internalSymbol As ArrayTypeSymbol = DirectCast(current.Value.Type.GetInternalSymbol(), ArrayTypeSymbol)
					instance.Add(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.InstrumentationPayloadRoot(key, internalSymbol, True), syntheticBoundNodeFactory.Array(internalSymbol.ElementType, ImmutableArray.Create(Of BoundExpression)(syntheticBoundNodeFactory.MaximumMethodDefIndex()), ImmutableArray(Of BoundExpression).Empty)))
				End While
			End Using
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.System_Guid__ctor, False)
			If (methodSymbol IsNot Nothing) Then
				instance.Add(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.ModuleVersionId(True), syntheticBoundNodeFactory.[New](methodSymbol, New BoundExpression() { syntheticBoundNodeFactory.ModuleVersionIdString() })))
			End If
			instance.Add(syntheticBoundNodeFactory.[Return](Nothing))
			Return syntheticBoundNodeFactory.Block(instance.ToImmutableAndFree())
		End Function
	End Class
End Namespace