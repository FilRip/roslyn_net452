Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Structure MemberModifiers
		Private ReadOnly _foundFlags As SourceMemberFlags

		Private ReadOnly _computedFlags As SourceMemberFlags

		Public ReadOnly Property AllFlags As SourceMemberFlags
			Get
				Return Me._foundFlags Or Me._computedFlags
			End Get
		End Property

		Public ReadOnly Property ComputedFlags As SourceMemberFlags
			Get
				Return Me._computedFlags
			End Get
		End Property

		Public ReadOnly Property FoundFlags As SourceMemberFlags
			Get
				Return Me._foundFlags
			End Get
		End Property

		Public Sub New(ByVal foundFlags As SourceMemberFlags, ByVal computedFlags As SourceMemberFlags)
			Me = New MemberModifiers() With
			{
				._foundFlags = foundFlags,
				._computedFlags = computedFlags
			}
		End Sub
	End Structure
End Namespace