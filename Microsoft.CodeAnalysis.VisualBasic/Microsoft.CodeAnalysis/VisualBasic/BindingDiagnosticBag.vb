Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BindingDiagnosticBag
		Inherits BindingDiagnosticBag(Of AssemblySymbol)
		Public ReadOnly Shared Discarded As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Friend ReadOnly Property IsEmpty As Boolean
			Get
				If (Me.DiagnosticBag IsNot Nothing AndAlso Not Me.DiagnosticBag.IsEmptyWithoutResolution) Then
					Return False
				End If
				Return Me.DependenciesBag.IsNullOrEmpty()
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded = New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(Nothing, Nothing)
		End Sub

		Public Sub New()
			MyBase.New(False)
		End Sub

		Private Sub New(ByVal usePool As Boolean)
			MyBase.New(usePool)
		End Sub

		Public Sub New(ByVal diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag)
			MyBase.New(diagnosticBag, Nothing)
		End Sub

		Public Sub New(ByVal diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag, ByVal dependenciesBag As ICollection(Of AssemblySymbol))
			MyBase.New(diagnosticBag, dependenciesBag)
		End Sub

		Public Overloads Function Add(ByVal node As BoundNode, ByVal useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Return MyBase.Add(node.Syntax.Location, useSiteInfo)
		End Function

		Public Overloads Function Add(ByVal syntax As SyntaxNodeOrToken, ByVal useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Return MyBase.Add(syntax.GetLocation(), useSiteInfo)
		End Function

		Public Overloads Function Add(ByVal code As ERRID, ByVal location As Microsoft.CodeAnalysis.Location) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(code)
			Me.Add(diagnosticInfo, location)
			Return diagnosticInfo
		End Function

		Public Overloads Function Add(ByVal code As ERRID, ByVal location As Microsoft.CodeAnalysis.Location, ByVal ParamArray args As Object()) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = ErrorFactory.ErrorInfo(code, args)
			Me.Add(diagnosticInfo, location)
			Return diagnosticInfo
		End Function

		Public Overloads Sub Add(ByVal info As DiagnosticInfo, ByVal location As Microsoft.CodeAnalysis.Location)
			Dim diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag = Me.DiagnosticBag
			If (diagnosticBag Is Nothing) Then
				Return
			End If
			diagnosticBag.Add(New VBDiagnostic(info, location, False))
		End Sub

		Friend Sub AddAssembliesUsedByCrefTarget(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (Me.DependenciesBag IsNot Nothing) Then
				Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
				If (namespaceSymbol IsNot Nothing) Then
					Me.AddAssembliesUsedByNamespaceReference(namespaceSymbol)
					Return
				End If
				Dim containingType As Object = TryCast(symbol, TypeSymbol)
				If (containingType Is Nothing) Then
					containingType = symbol.ContainingType
				End If
				AddDependencies(If(TryCast(symbol, TypeSymbol), symbol.ContainingType))
			End If
		End Sub

		Friend Sub AddAssembliesUsedByNamespaceReference(ByVal ns As NamespaceSymbol)
			If (Me.DependenciesBag IsNot Nothing) Then
				Me.AddAssembliesUsedByNamespaceReferenceImpl(ns)
			End If
		End Sub

		Private Sub AddAssembliesUsedByNamespaceReferenceImpl(ByVal ns As NamespaceSymbol)
			If (ns.Extent.Kind = NamespaceKind.Compilation) Then
				Dim enumerator As ImmutableArray(Of NamespaceSymbol).Enumerator = ns.ConstituentNamespaces.GetEnumerator()
				While enumerator.MoveNext()
					Me.AddAssembliesUsedByNamespaceReferenceImpl(enumerator.Current)
				End While
				Return
			End If
			Dim containingAssembly As AssemblySymbol = ns.ContainingAssembly
			If (containingAssembly IsNot Nothing AndAlso Not containingAssembly.IsMissing) Then
				Me.DependenciesBag.Add(containingAssembly)
			End If
		End Sub

		Friend Overloads Sub AddDependencies(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (Me.DependenciesBag IsNot Nothing AndAlso symbol IsNot Nothing) Then
				MyBase.AddDependencies(symbol.GetUseSiteInfo())
			End If
		End Sub

		Friend Shared Function Create(ByVal withDiagnostics As Boolean, ByVal withDependencies As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			If (Not withDependencies) Then
				bindingDiagnosticBag = If(Not withDiagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(New DiagnosticBag()))
			Else
				bindingDiagnosticBag = If(Not withDiagnostics, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(Nothing, New HashSet(Of AssemblySymbol)()), New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag())
			End If
			Return bindingDiagnosticBag
		End Function

		Friend Shared Function Create(ByVal template As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Return Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Create(template.AccumulatesDiagnostics, template.AccumulatesDependencies)
		End Function

		Friend Shared Function GetInstance() As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Return New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(True)
		End Function

		Friend Shared Function GetInstance(ByVal withDiagnostics As Boolean, ByVal withDependencies As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Dim bindingDiagnosticBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			If (Not withDependencies) Then
				bindingDiagnosticBag = If(Not withDiagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(DiagnosticBag.GetInstance()))
			Else
				bindingDiagnosticBag = If(Not withDiagnostics, New Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag(Nothing, PooledHashSet(Of AssemblySymbol).GetInstance()), Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance())
			End If
			Return bindingDiagnosticBag
		End Function

		Friend Shared Function GetInstance(ByVal template As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag
			Return Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(template.AccumulatesDiagnostics, template.AccumulatesDependencies)
		End Function

		Friend Function ReportUseSite(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal node As SyntaxNode) As Boolean
			Return Me.ReportUseSite(symbol, node.Location)
		End Function

		Friend Function ReportUseSite(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal token As SyntaxToken) As Boolean
			Return Me.ReportUseSite(symbol, token.GetLocation())
		End Function

		Friend Function ReportUseSite(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal location As Microsoft.CodeAnalysis.Location) As Boolean
			Dim flag As Boolean
			flag = If(symbol Is Nothing, False, MyBase.Add(symbol.GetUseSiteInfo(), location))
			Return flag
		End Function

		Protected Overrides Function ReportUseSiteDiagnostic(ByVal diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo, ByVal diagnosticBag As Microsoft.CodeAnalysis.DiagnosticBag, ByVal location As Microsoft.CodeAnalysis.Location) As Boolean
			diagnosticBag.Add(New VBDiagnostic(diagnosticInfo, location, False))
			Return True
		End Function
	End Class
End Namespace