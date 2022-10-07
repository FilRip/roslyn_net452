Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedEventBackingFieldSymbol
		Inherits SynthesizedBackingFieldBase(Of SourceEventSymbol)
		Private _lazyType As TypeSymbol

		Friend Overrides ReadOnly Property IsNotSerialized As Boolean
			Get
				Dim decodedWellKnownAttributeData As EventWellKnownAttributeData = Me._propertyOrEvent.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasNonSerializedAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				If (Me._lazyType Is Nothing) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me._propertyOrEvent.Type
					If (Me._propertyOrEvent.IsWindowsRuntimeEvent) Then
						Dim wellKnownType As NamedTypeSymbol = Me.DeclaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T)
						Dim useSiteInfoForWellKnownType As UseSiteInfo(Of AssemblySymbol) = Binder.GetUseSiteInfoForWellKnownType(wellKnownType)
						Dim locations As ImmutableArray(Of Location) = Me._propertyOrEvent.Locations
						instance.Add(useSiteInfoForWellKnownType, locations(0))
						typeSymbol = wellKnownType.Construct(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol() { typeSymbol })
					End If
					DirectCast(Me.ContainingModule, SourceModuleSymbol).AtomicStoreReferenceAndDiagnostics(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(Me._lazyType, typeSymbol, instance, Nothing)
					instance.Free()
				End If
				Return Me._lazyType
			End Get
		End Property

		Public Sub New(ByVal propertyOrEvent As SourceEventSymbol, ByVal name As String, ByVal isShared As Boolean)
			MyBase.New(propertyOrEvent, name, isShared)
		End Sub

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.GenerateDeclarationErrors(cancellationToken)
			cancellationToken.ThrowIfCancellationRequested()
			Dim type As TypeSymbol = Me.Type
		End Sub
	End Class
End Namespace