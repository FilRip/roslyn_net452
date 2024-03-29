' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Globalization

Namespace Microsoft.CodeAnalysis.VisualBasic

    Partial Public Class GlobalImport

        ' A special Diagnostic info that wraps a particular diagnostic but customized the message with 
        ' the text of the import.
        Private Class ImportDiagnosticInfo
            Inherits DiagnosticInfo

            Shared Sub New()
                ObjectBinder.RegisterTypeReader(GetType(ImportDiagnosticInfo), Function(r) New ImportDiagnosticInfo(r))
            End Sub

            Private ReadOnly _importText As String
            Private ReadOnly _startIndex As Integer
            Private ReadOnly _length As Integer
            Private ReadOnly _wrappedDiagnostic As DiagnosticInfo

            Private Sub New(reader As ObjectReader)
                MyBase.New(reader)
                Me._importText = reader.ReadString()
                Me._startIndex = reader.ReadInt32()
                Me._length = reader.ReadInt32()
                Me._wrappedDiagnostic = DirectCast(reader.ReadValue(), DiagnosticInfo)
            End Sub

            Protected Overrides Sub WriteTo(writer As ObjectWriter)
                MyBase.WriteTo(writer)

                writer.WriteString(_importText)
                writer.WriteInt32(_startIndex)
                writer.WriteInt32(_length)
                writer.WriteValue(_wrappedDiagnostic)
            End Sub

            Public Overrides Function GetMessage(Optional formatProvider As IFormatProvider = Nothing) As String
                Dim msg = ErrorFactory.IdToString(ERRID.ERR_GeneralProjectImportsError3, TryCast(formatProvider, CultureInfo))
                Return string.Format(formatProvider, msg, _importText, _importText.Substring(_startIndex, _length), _wrappedDiagnostic.GetMessage(formatProvider))
            End Function

            Public Sub New(wrappedDiagnostic As DiagnosticInfo,
                           importText As String,
                           startIndex As Integer,
                           length As Integer)
                MyBase.New(VisualBasic.MessageProvider.Instance, wrappedDiagnostic.Code)
                _wrappedDiagnostic = wrappedDiagnostic
                _importText = importText
                _startIndex = startIndex
                _length = length
            End Sub
        End Class
    End Class
End Namespace
