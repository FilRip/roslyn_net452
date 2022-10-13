' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Namespace Microsoft.CodeAnalysis.VisualBasic

    Friend Structure XmlNamespaceAndImportsClausePosition
        Public ReadOnly XmlNamespace As String
        Public ReadOnly ImportsClausePosition As Integer

        Public Sub New(xmlNamespace As String, importsClausePosition As Integer)
            Me.XmlNamespace = xmlNamespace
            Me.ImportsClausePosition = importsClausePosition
        End Sub
    End Structure
End Namespace
