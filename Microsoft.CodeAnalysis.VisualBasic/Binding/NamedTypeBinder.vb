' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Collections.Immutable
Imports System.Runtime.InteropServices

Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Microsoft.CodeAnalysis.VisualBasic

    ''' <summary>
    ''' A NamedTypeBinder provides the context for a type symbol; e.g., looking up names
    ''' inside the type.
    ''' </summary>
    Friend Class NamedTypeBinder
        Inherits Binder

        Private ReadOnly _typeSymbol As NamedTypeSymbol

        Public Sub New(containingBinder As Binder, typeSymbol As NamedTypeSymbol)
            MyBase.New(containingBinder)
            Debug.Assert(typeSymbol IsNot Nothing)
            ' The constraints apply during normal binding, but not in the expression compiler.
            '   Debug.Assert(Not (TypeOf typeSymbol Is Symbols.Metadata.PE.PENamedTypeSymbol))
            '   Debug.Assert(typeSymbol.IsFromCompilation(Me.Compilation))
            _typeSymbol = typeSymbol
        End Sub

        ''' <summary>
        ''' Some nodes have special binder's for their contents 
        ''' </summary>
        Public Overrides Function GetBinder(node As SyntaxNode) As Binder
            ' TODO (tomat): this is a temporary workaround, we need a special script class binder
            ' Return Me so that identifiers in top-level statements bind to the members of the script class.
            Return If(_typeSymbol.IsScriptClass, Me, m_containingBinder.GetBinder(node))
        End Function

        Public Overrides Function GetBinder(stmtList As SyntaxList(Of StatementSyntax)) As Binder
            ' TODO (tomat): this is a temporary workaround, we need a special script class binder
            ' Return Me so that identifiers in top-level statements bind to the members of the script class.
            Return If(_typeSymbol.IsScriptClass, Me, m_containingBinder.GetBinder(stmtList))
        End Function

        Public Overrides ReadOnly Property ContainingNamespaceOrType As NamespaceOrTypeSymbol
            Get
                Return _typeSymbol
            End Get
        End Property

        ''' <summary>
        ''' Gets all symbols of the particular name as 
        ''' a) members of this type
        ''' b) members of base types 
        ''' c) type parameters in this type (but not outer or base types)
        ''' In that order.
        ''' 
        ''' Note, that section "11.4.4 Simple Name Expression" of VB Language spec 
        ''' implies that type parameters are examined first, and only then members 
        ''' of the type are examined. But this is inconsistent with Dev10 behavior.
        ''' 
        ''' Returns all members of that name, or empty list if none.
        ''' </summary>
        Friend Overrides Sub LookupInSingleBinder(lookupResult As LookupResult,
                                                      name As String,
                                                      arity As Integer,
                                                      options As LookupOptions,
                                                      originalBinder As Binder,
                                                      <[In], Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
            Debug.Assert(lookupResult.IsClear)

            ' 1. look for members. This call automatically gets members of base types.
            originalBinder.LookupMember(lookupResult, _typeSymbol, name, arity, options, useSiteInfo)
            If lookupResult.StopFurtherLookup Then
                Return ' short cut result
            End If

            ' 2. Lookup type parameter.
            Dim typeParameterLookupResult = LookupResult.GetInstance()
            LookupTypeParameter(typeParameterLookupResult, name, arity, options, originalBinder, useSiteInfo)
            lookupResult.MergePrioritized(typeParameterLookupResult)
            typeParameterLookupResult.Free()

        End Sub

        ''' <summary>
        ''' Collect extension methods with the given name that are in scope in this binder.
        ''' The passed in ArrayBuilder must be empty. Extension methods from the same containing type
        ''' must be grouped together. 
        ''' </summary>
        Protected Overrides Sub CollectProbableExtensionMethodsInSingleBinder(name As String,
                                                                      methods As ArrayBuilder(Of MethodSymbol),
                                                                      originalBinder As Binder)
            Debug.Assert(methods.Count = 0)
            _typeSymbol.AppendProbableExtensionMethods(name, methods)
        End Sub

        Protected Overrides Sub AddExtensionMethodLookupSymbolsInfoInSingleBinder(nameSet As LookupSymbolsInfo,
                                                                                   options As LookupOptions,
                                                                                   originalBinder As Binder)
            _typeSymbol.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder)
        End Sub

        Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(nameSet As LookupSymbolsInfo,
                                                                    options As LookupOptions,
                                                                    originalBinder As Binder)
            ' 1. Add all type parameters.
            ' UNDONE: filter using options.
            If _typeSymbol.Arity > 0 Then
                For Each tp In _typeSymbol.TypeParameters
                    If originalBinder.CanAddLookupSymbolInfo(tp, options, nameSet, Nothing) Then
                        nameSet.AddSymbol(tp, tp.Name, 0)
                    End If
                Next
            End If

            ' 2. Add member names on the type.
            originalBinder.AddMemberLookupSymbolsInfo(nameSet, _typeSymbol, options)
        End Sub

        ' Look in all type parameters of the given name in the instance type.
        ' Since there are typically just one or two, using a dictionary/ILookup would be overkill.
        Private Sub LookupTypeParameter(lookupResult As LookupResult,
                                        name As String,
                                        arity As Integer,
                                        options As LookupOptions,
                                        originalBinder As Binder,
                                        <[In], Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
            Debug.Assert(name IsNot Nothing)
            Debug.Assert(lookupResult.IsClear)

            If _typeSymbol.Arity > 0 Then
                For Each tp In _typeSymbol.TypeParameters
                    If CaseInsensitiveComparison.Equals(tp.Name, name) Then
                        lookupResult.SetFrom(originalBinder.CheckViability(tp, arity, options, Nothing, useSiteInfo))
                    End If
                Next
            End If

            Return
        End Sub

        Public Overrides Function CheckAccessibility(sym As Symbol,
                                                     <[In], Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol),
                                                     Optional accessThroughType As TypeSymbol = Nothing,
                                                     Optional basesBeingResolved As BasesBeingResolved = Nothing) As AccessCheckResult
            Return If(IgnoresAccessibility,
                AccessCheckResult.Accessible,
                AccessCheck.CheckSymbolAccessibility(sym, _typeSymbol, accessThroughType, useSiteInfo, basesBeingResolved))
        End Function

        Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
            Get
                Return _typeSymbol
            End Get
        End Property

        Public Overrides ReadOnly Property ContainingMember As Symbol
            Get
                Return _typeSymbol
            End Get
        End Property

        Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
            Get
                Return ImmutableArray(Of Symbol).Empty
            End Get
        End Property

        Public Overrides ReadOnly Property IsInQuery As Boolean
            Get
                Return False
            End Get
        End Property
    End Class

End Namespace
