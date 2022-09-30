﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Collections.Immutable
Imports System.ComponentModel
Imports System.Threading
Imports Microsoft.CodeAnalysis.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic

    Public Class VisualBasicGeneratorDriver
        Inherits GeneratorDriver

        Private Sub New(state As GeneratorDriverState)
            MyBase.New(state)
        End Sub

        Friend Sub New(parseOptions As VisualBasicParseOptions, generators As ImmutableArray(Of ISourceGenerator), optionsProvider As AnalyzerConfigOptionsProvider, additionalTexts As ImmutableArray(Of AdditionalText), driverOptions As GeneratorDriverOptions)
            MyBase.New(parseOptions, generators, optionsProvider, additionalTexts, driverOptions)
        End Sub

        Public Overrides ReadOnly Property MessageProvider As CommonMessageProvider
            Get
                Return VisualBasic.MessageProvider.Instance
            End Get
        End Property

        Public Overrides Function FromState(state As GeneratorDriverState) As GeneratorDriver
            Return New VisualBasicGeneratorDriver(state)
        End Function

        Public Overrides Function ParseGeneratedSourceText(input As GeneratedSourceText, fileName As String, cancellationToken As CancellationToken) As SyntaxTree
            Return VisualBasicSyntaxTree.ParseTextLazy(input.Text, CType(_state.ParseOptions, VisualBasicParseOptions), fileName)
        End Function

        Public Shared Function Create(generators As ImmutableArray(Of ISourceGenerator), Optional additionalTexts As ImmutableArray(Of AdditionalText) = Nothing, Optional parseOptions As VisualBasicParseOptions = Nothing, Optional analyzerConfigOptionsProvider As AnalyzerConfigOptionsProvider = Nothing, Optional driverOptions As GeneratorDriverOptions = Nothing) As VisualBasicGeneratorDriver
            Return New VisualBasicGeneratorDriver(parseOptions, generators, If(analyzerConfigOptionsProvider, CompilerAnalyzerConfigOptionsProvider.Empty), additionalTexts.NullToEmpty(), driverOptions)
        End Function

        ' 3.11 BACK COMPAT OVERLOAD -- DO NOT TOUCH
        <EditorBrowsable(EditorBrowsableState.Never)>
        Public Shared Function Create(generators As ImmutableArray(Of ISourceGenerator), additionalTexts As ImmutableArray(Of AdditionalText), parseOptions As VisualBasicParseOptions, analyzerConfigOptionsProvider As AnalyzerConfigOptionsProvider) As VisualBasicGeneratorDriver
            Return Create(generators, additionalTexts, parseOptions, analyzerConfigOptionsProvider, driverOptions:=Nothing)
        End Function

        Public Overrides ReadOnly Property SourceExtension As String
            Get
                Return ".vb"
            End Get
        End Property

    End Class

End Namespace
