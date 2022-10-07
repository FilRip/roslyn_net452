Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	<DebuggerDisplay("{GetDebuggerDisplay(), nq}")>
	Public NotInheritable Class GlobalImport
		Implements IEquatable(Of GlobalImport)
		Private ReadOnly _clause As SyntaxReference

		Private ReadOnly _importedName As String

		Public ReadOnly Property Clause As ImportsClauseSyntax
			Get
				Return DirectCast(Me._clause.GetSyntax(New CancellationToken()), ImportsClauseSyntax)
			End Get
		End Property

		Friend ReadOnly Property IsXmlClause As Boolean
			Get
				Return Me.Clause.IsKind(SyntaxKind.XmlNamespaceImportsClause)
			End Get
		End Property

		Public ReadOnly Property Name As String
			Get
				Return Me._importedName
			End Get
		End Property

		Friend Sub New(ByVal clause As ImportsClauseSyntax, ByVal importedName As String)
			MyBase.New()
			Me._clause = clause.SyntaxTree.GetReference(clause)
			Me._importedName = importedName
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of GlobalImport).Equals
			Return Me.ExplicitEquals(TryCast(obj, GlobalImport))
		End Function

		Public Function ExplicitEquals(ByVal other As GlobalImport) As Boolean Implements IEquatable(Of GlobalImport).Equals
			Dim flag As Boolean
			If (CObj(Me) = CObj(other)) Then
				flag = True
			ElseIf (other IsNot Nothing) Then
				flag = If(Not [String].Equals(Me.Name, other.Name, StringComparison.Ordinal), False, [String].Equals(Me.Clause.ToFullString(), other.Clause.ToFullString(), StringComparison.Ordinal))
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function GetDebuggerDisplay() As String
			Return Me.Name
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me.Name.GetHashCode(), StringComparer.Ordinal.GetHashCode(Me.Clause.ToFullString()))
		End Function

		Friend Function MapDiagnostic(ByVal unmappedDiag As Diagnostic) As Diagnostic
			Dim vBDiagnostic As Diagnostic
			If (unmappedDiag.Code <> 40056) Then
				Dim sourceSpan As TextSpan = unmappedDiag.Location.SourceSpan
				Dim start As Integer = sourceSpan.Start - Me._clause.Span.Start
				Dim length As Integer = sourceSpan.Length
				If (start < 0 OrElse length <= 0 OrElse start >= Me._importedName.Length) Then
					start = 0
					length = Me._importedName.Length
				End If
				length = Math.Min(Me._importedName.Length - start, length)
				vBDiagnostic = New Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic(New GlobalImport.ImportDiagnosticInfo(DirectCast(unmappedDiag, DiagnosticWithInfo).Info, Me._importedName, start, length), NoLocation.Singleton, False)
			Else
				vBDiagnostic = New Microsoft.CodeAnalysis.VisualBasic.VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.WRN_UndefinedOrEmptyProjectNamespaceOrClass1, New [Object]() { Me._importedName }), NoLocation.Singleton, False)
			End If
			Return vBDiagnostic
		End Function

		Public Shared Operator =(ByVal left As GlobalImport, ByVal right As GlobalImport) As Boolean
			Return [Object].Equals(left, right)
		End Operator

		Public Shared Operator <>(ByVal left As GlobalImport, ByVal right As GlobalImport) As Boolean
			Return Not [Object].Equals(left, right)
		End Operator

		Public Shared Function Parse(ByVal importedNames As String) As GlobalImport
			Return GlobalImport.Parse(New [String]() { importedNames }).ElementAtOrDefault(0)
		End Function

		Public Shared Function Parse(ByVal importedNames As String, <Out> ByRef diagnostics As ImmutableArray(Of Diagnostic)) As GlobalImport
			Return GlobalImport.Parse(New [String]() { importedNames }, diagnostics).ElementAtOrDefault(0)
		End Function

		Public Shared Function Parse(ByVal importedNames As IEnumerable(Of String)) As IEnumerable(Of GlobalImport)
			Dim severity As Func(Of Microsoft.CodeAnalysis.Diagnostic, Boolean)
			Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
			Dim globalImportArray As GlobalImport() = OptionsValidator.ParseImports(importedNames, instance)
			Dim diagnostics As IEnumerable(Of Microsoft.CodeAnalysis.Diagnostic) = instance.AsEnumerable()
			If (GlobalImport._Closure$__.$I12-0 Is Nothing) Then
				severity = Function(diag As Microsoft.CodeAnalysis.Diagnostic) diag.Severity = DiagnosticSeverity.[Error]
				GlobalImport._Closure$__.$I12-0 = severity
			Else
				severity = GlobalImport._Closure$__.$I12-0
			End If
			Dim diagnostic As Microsoft.CodeAnalysis.Diagnostic = diagnostics.FirstOrDefault(severity)
			instance.Free()
			If (diagnostic IsNot Nothing) Then
				Throw New ArgumentException(diagnostic.GetMessage(CultureInfo.CurrentUICulture))
			End If
			Return globalImportArray
		End Function

		Public Shared Function Parse(ByVal ParamArray importedNames As String()) As IEnumerable(Of GlobalImport)
			Return GlobalImport.Parse(DirectCast(importedNames, IEnumerable(Of String)))
		End Function

		Public Shared Function Parse(ByVal importedNames As IEnumerable(Of String), <Out> ByRef diagnostics As ImmutableArray(Of Diagnostic)) As IEnumerable(Of GlobalImport)
			Dim instance As DiagnosticBag = DiagnosticBag.GetInstance()
			Dim globalImportArray As GlobalImport() = OptionsValidator.ParseImports(importedNames, instance)
			diagnostics = instance.ToReadOnlyAndFree(Of Diagnostic)()
			Return globalImportArray
		End Function

		Private Class ImportDiagnosticInfo
			Inherits DiagnosticInfo
			Private ReadOnly _importText As String

			Private ReadOnly _startIndex As Integer

			Private ReadOnly _length As Integer

			Private ReadOnly _wrappedDiagnostic As DiagnosticInfo

			Shared Sub New()
				ObjectBinder.RegisterTypeReader(GetType(GlobalImport.ImportDiagnosticInfo), Function(r As ObjectReader) New GlobalImport.ImportDiagnosticInfo(r))
			End Sub

			Private Sub New(ByVal reader As ObjectReader)
				MyBase.New(reader)
				Me._importText = reader.ReadString()
				Me._startIndex = reader.ReadInt32()
				Me._length = reader.ReadInt32()
				Me._wrappedDiagnostic = DirectCast(reader.ReadValue(), DiagnosticInfo)
			End Sub

			Public Sub New(ByVal wrappedDiagnostic As DiagnosticInfo, ByVal importText As String, ByVal startIndex As Integer, ByVal length As Integer)
				MyBase.New(MessageProvider.Instance, wrappedDiagnostic.Code)
				Me._wrappedDiagnostic = wrappedDiagnostic
				Me._importText = importText
				Me._startIndex = startIndex
				Me._length = length
			End Sub

			Public Overrides Function GetMessage(Optional ByVal formatProvider As IFormatProvider = Nothing) As String
				Dim str As String = ErrorFactory.IdToString(ERRID.ERR_GeneralProjectImportsError3, TryCast(formatProvider, CultureInfo))
				Return [String].Format(formatProvider, str, Me._importText, Me._importText.Substring(Me._startIndex, Me._length), Me._wrappedDiagnostic.GetMessage(formatProvider))
			End Function

			Protected Overrides Sub WriteTo(ByVal writer As ObjectWriter)
				MyBase.WriteTo(writer)
				writer.WriteString(Me._importText)
				writer.WriteInt32(Me._startIndex)
				writer.WriteInt32(Me._length)
				writer.WriteValue(DirectCast(Me._wrappedDiagnostic, IObjectWritable))
			End Sub
		End Class
	End Class
End Namespace