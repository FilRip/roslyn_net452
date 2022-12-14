' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic
    Partial Public Class VisualBasicCompilation

        Partial Friend Class DocumentationCommentCompiler
            Inherits VisualBasicSymbolVisitor

            Public Overrides Sub VisitField(symbol As FieldSymbol)
                Me._cancellationToken.ThrowIfCancellationRequested()

                If Not ShouldSkipSymbol(symbol) Then
                    Dim sourceField = TryCast(symbol, SourceFieldSymbol)
                    If sourceField IsNot Nothing Then
                        WriteDocumentationCommentForField(sourceField)
                    End If
                End If
            End Sub

            Private Sub WriteDocumentationCommentForField(field As SourceFieldSymbol)
                Dim docCommentTrivia As DocumentationCommentTriviaSyntax =
                    TryGetDocCommentTriviaAndGenerateDiagnostics(field.DeclarationSyntax)

                If docCommentTrivia Is Nothing Then
                    Return
                End If

                Dim wellKnownElementNodes As New Dictionary(Of WellKnownTag, ArrayBuilder(Of XmlNodeSyntax))

                Dim docCommentXml As String =
                    GetDocumentationCommentForSymbol(field, docCommentTrivia, wellKnownElementNodes)

                ' No further processing
                If docCommentXml Is Nothing Then
                    FreeWellKnownElementNodes(wellKnownElementNodes)
                    Return
                End If

                If docCommentTrivia.SyntaxTree.ReportDocumentationCommentDiagnostics OrElse _writer.IsSpecified Then
                    Dim symbolName As String = GetSymbolName(field)

                    ' Duplicate top-level well known tags
                    ReportWarningsForDuplicatedTags(wellKnownElementNodes)

                    ' <exception>
                    ReportIllegalWellKnownTagIfAny(WellKnownTag.Exception, wellKnownElementNodes, symbolName)

                    ' <returns>
                    ReportIllegalWellKnownTagIfAny(WellKnownTag.Returns, wellKnownElementNodes, symbolName)

                    ' <param>
                    ReportIllegalWellKnownTagIfAny(WellKnownTag.Param, wellKnownElementNodes, symbolName)

                    ' <paramref>
                    ReportIllegalWellKnownTagIfAny(WellKnownTag.ParamRef, wellKnownElementNodes, symbolName)

                    ' <value>
                    ReportIllegalWellKnownTagIfAny(WellKnownTag.Value, wellKnownElementNodes, symbolName)

                    ' <typeparam>
                    ReportIllegalWellKnownTagIfAny(WellKnownTag.TypeParam, wellKnownElementNodes, symbolName)

                    ' <typeparamref>
                    ReportWarningsForTypeParamRefTags(wellKnownElementNodes, symbolName, field, docCommentTrivia.SyntaxTree)
                End If

                FreeWellKnownElementNodes(wellKnownElementNodes)

                WriteDocumentationCommentForSymbol(docCommentXml)

            End Sub

        End Class
    End Class
End Namespace
