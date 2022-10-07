Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.RuntimeMembers
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class MetadataOrSourceAssemblySymbol
		Inherits NonMissingAssemblySymbol
		Private _lazySpecialTypes As NamedTypeSymbol()

		Private _cachedSpecialTypes As Integer

		Private _lazyTypeNames As ICollection(Of String)

		Private _lazyNamespaceNames As ICollection(Of String)

		Private _assembliesToWhichInternalAccessHasBeenAnalyzed As ConcurrentDictionary(Of AssemblySymbol, IVTConclusion)

		Private _lazySpecialTypeMembers As Symbol()

		Private ReadOnly Property AssembliesToWhichInternalAccessHasBeenDetermined As ConcurrentDictionary(Of AssemblySymbol, IVTConclusion)
			Get
				If (Me._assembliesToWhichInternalAccessHasBeenAnalyzed Is Nothing) Then
					Interlocked.CompareExchange(Of ConcurrentDictionary(Of AssemblySymbol, IVTConclusion))(Me._assembliesToWhichInternalAccessHasBeenAnalyzed, New ConcurrentDictionary(Of AssemblySymbol, IVTConclusion)(), Nothing)
				End If
				Return Me._assembliesToWhichInternalAccessHasBeenAnalyzed
			End Get
		End Property

		Friend Overrides ReadOnly Property KeepLookingForDeclaredSpecialTypes As Boolean
			Get
				If (MyBase.CorLibrary <> Me) Then
					Return False
				End If
				Return Me._cachedSpecialTypes < 45
			End Get
		End Property

		Public Overrides ReadOnly Property NamespaceNames As ICollection(Of String)
			Get
				Dim func As Func(Of ModuleSymbol, ICollection(Of String))
				If (Me._lazyNamespaceNames Is Nothing) Then
					Dim modules As ImmutableArray(Of ModuleSymbol) = Me.Modules
					If (MetadataOrSourceAssemblySymbol._Closure$__.$I12-0 Is Nothing) Then
						func = Function(m As ModuleSymbol) m.NamespaceNames
						MetadataOrSourceAssemblySymbol._Closure$__.$I12-0 = func
					Else
						func = MetadataOrSourceAssemblySymbol._Closure$__.$I12-0
					End If
					Interlocked.CompareExchange(Of ICollection(Of String))(Me._lazyNamespaceNames, UnionCollection(Of String).Create(Of ModuleSymbol)(modules, func), Nothing)
				End If
				Return Me._lazyNamespaceNames
			End Get
		End Property

		Public Overrides ReadOnly Property TypeNames As ICollection(Of String)
			Get
				Dim func As Func(Of ModuleSymbol, ICollection(Of String))
				If (Me._lazyTypeNames Is Nothing) Then
					Dim modules As ImmutableArray(Of ModuleSymbol) = Me.Modules
					If (MetadataOrSourceAssemblySymbol._Closure$__.$I10-0 Is Nothing) Then
						func = Function(m As ModuleSymbol) m.TypeNames
						MetadataOrSourceAssemblySymbol._Closure$__.$I10-0 = func
					Else
						func = MetadataOrSourceAssemblySymbol._Closure$__.$I10-0
					End If
					Interlocked.CompareExchange(Of ICollection(Of String))(Me._lazyTypeNames, UnionCollection(Of String).Create(Of ModuleSymbol)(modules, func), Nothing)
				End If
				Return Me._lazyTypeNames
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function GetDeclaredSpecialType(ByVal type As SpecialType) As NamedTypeSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.MetadataOrSourceAssemblySymbol::GetDeclaredSpecialType(Microsoft.CodeAnalysis.SpecialType)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol GetDeclaredSpecialType(Microsoft.CodeAnalysis.SpecialType)
			' 
			' The unary opperator AddressReference is not supported in VisualBasic
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineNotSupportedVBCodeStep.cs:ligne 22
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Overrides Function GetDeclaredSpecialTypeMember(ByVal member As SpecialMember) As Symbol
			If (Me._lazySpecialTypeMembers Is Nothing OrElse Me._lazySpecialTypeMembers(CInt(member)) = ErrorTypeSymbol.UnknownResultType) Then
				If (Me._lazySpecialTypeMembers Is Nothing) Then
					Dim unknownResultType(123) As Symbol
					Dim length As Integer = CInt(unknownResultType.Length) - 1
					Dim num As Integer = 0
					Do
						unknownResultType(num) = ErrorTypeSymbol.UnknownResultType
						num = num + 1
					Loop While num <= length
					Interlocked.CompareExchange(Of Symbol())(Me._lazySpecialTypeMembers, unknownResultType, Nothing)
				End If
				Dim descriptor As MemberDescriptor = SpecialMembers.GetDescriptor(member)
				Dim declaredSpecialType As NamedTypeSymbol = Me.GetDeclaredSpecialType(DirectCast(CSByte(descriptor.DeclaringTypeId), SpecialType))
				Dim runtimeMember As Symbol = Nothing
				If (Not declaredSpecialType.IsErrorType()) Then
					runtimeMember = VisualBasicCompilation.GetRuntimeMember(declaredSpecialType, descriptor, VisualBasicCompilation.SpecialMembersSignatureComparer.Instance, Nothing)
				End If
				Interlocked.CompareExchange(Of Symbol)(Me._lazySpecialTypeMembers(CInt(member)), runtimeMember, ErrorTypeSymbol.UnknownResultType)
			End If
			Return Me._lazySpecialTypeMembers(CInt(member))
		End Function

		Protected Function MakeFinalIVTDetermination(ByVal potentialGiverOfAccess As AssemblySymbol) As IVTConclusion
			Dim vTConclusion As IVTConclusion
			Dim enumerator As IEnumerator(Of ImmutableArray(Of Byte)) = Nothing
			Dim vTConclusion1 As IVTConclusion = IVTConclusion.NoRelationshipClaimed
			If (Not Me.AssembliesToWhichInternalAccessHasBeenDetermined.TryGetValue(potentialGiverOfAccess, vTConclusion1)) Then
				vTConclusion1 = IVTConclusion.NoRelationshipClaimed
				Dim internalsVisibleToPublicKeys As IEnumerable(Of ImmutableArray(Of Byte)) = potentialGiverOfAccess.GetInternalsVisibleToPublicKeys(Me.Name)
				If (Not internalsVisibleToPublicKeys.Any() OrElse Not Me.IsNetModule()) Then
					Using enumerator
						enumerator = internalsVisibleToPublicKeys.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As ImmutableArray(Of Byte) = enumerator.Current
							vTConclusion1 = ISymbolExtensions.PerformIVTCheck(potentialGiverOfAccess.Identity, Me.PublicKey, current)
							If (vTConclusion1 <> IVTConclusion.Match) Then
								Continue While
							End If
							GoTo Label0
						End While
					End Using
				Label0:
					Me.AssembliesToWhichInternalAccessHasBeenDetermined.TryAdd(potentialGiverOfAccess, vTConclusion1)
					vTConclusion = vTConclusion1
				Else
					vTConclusion = IVTConclusion.Match
				End If
			Else
				vTConclusion = vTConclusion1
			End If
			Return vTConclusion
		End Function

		Friend Overrides Sub RegisterDeclaredSpecialType(ByVal corType As NamedTypeSymbol)
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = corType.SpecialType
			If (Me._lazySpecialTypes Is Nothing) Then
				Interlocked.CompareExchange(Of NamedTypeSymbol())(Me._lazySpecialTypes, New NamedTypeSymbol(45) {}, Nothing)
			End If
			If (Interlocked.CompareExchange(Of NamedTypeSymbol)(Me._lazySpecialTypes(CInt(specialType)), corType, Nothing) Is Nothing) Then
				Interlocked.Increment(Me._cachedSpecialTypes)
			End If
		End Sub
	End Class
End Namespace