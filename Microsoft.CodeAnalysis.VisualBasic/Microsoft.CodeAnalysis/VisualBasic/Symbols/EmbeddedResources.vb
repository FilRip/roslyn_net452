Imports System
Imports System.IO
Imports System.Reflection

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class EmbeddedResources
		Private Shared s_embedded As String

		Private Shared s_internalXmlHelper As String

		Private Shared s_vbCoreSourceText As String

		Private Shared s_vbMyTemplateText As String

		Public ReadOnly Shared Property Embedded As String
			Get
				If (EmbeddedResources.s_embedded Is Nothing) Then
					EmbeddedResources.s_embedded = EmbeddedResources.GetManifestResourceString("Embedded.vb")
				End If
				Return EmbeddedResources.s_embedded
			End Get
		End Property

		Public ReadOnly Shared Property InternalXmlHelper As String
			Get
				If (EmbeddedResources.s_internalXmlHelper Is Nothing) Then
					EmbeddedResources.s_internalXmlHelper = EmbeddedResources.GetManifestResourceString("InternalXmlHelper.vb")
				End If
				Return EmbeddedResources.s_internalXmlHelper
			End Get
		End Property

		Public ReadOnly Shared Property VbCoreSourceText As String
			Get
				If (EmbeddedResources.s_vbCoreSourceText Is Nothing) Then
					EmbeddedResources.s_vbCoreSourceText = EmbeddedResources.GetManifestResourceString("VbCoreSourceText.vb")
				End If
				Return EmbeddedResources.s_vbCoreSourceText
			End Get
		End Property

		Public ReadOnly Shared Property VbMyTemplateText As String
			Get
				If (EmbeddedResources.s_vbMyTemplateText Is Nothing) Then
					EmbeddedResources.s_vbMyTemplateText = EmbeddedResources.GetManifestResourceString("VbMyTemplateText.vb")
				End If
				Return EmbeddedResources.s_vbMyTemplateText
			End Get
		End Property

		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Function GetManifestResourceString(ByVal name As String) As String
			Dim [end] As String
			Using streamReader As System.IO.StreamReader = New System.IO.StreamReader(GetType(EmbeddedResources).GetTypeInfo().Assembly.GetManifestResourceStream(name))
				[end] = streamReader.ReadToEnd()
			End Using
			Return [end]
		End Function
	End Class
End Namespace