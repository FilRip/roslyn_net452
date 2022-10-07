Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundPropertyAccess
		Inherits BoundExpression
		Private ReadOnly _PropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol

		Private ReadOnly _PropertyGroupOpt As BoundPropertyGroup

		Private ReadOnly _AccessKind As PropertyAccessKind

		Private ReadOnly _IsWriteable As Boolean

		Private ReadOnly _IsLValue As Boolean

		Private ReadOnly _ReceiverOpt As BoundExpression

		Private ReadOnly _Arguments As ImmutableArray(Of BoundExpression)

		Private ReadOnly _DefaultArguments As BitVector

		Public ReadOnly Property AccessKind As PropertyAccessKind
			Get
				Return Me._AccessKind
			End Get
		End Property

		Public ReadOnly Property Arguments As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Arguments
			End Get
		End Property

		Public ReadOnly Property DefaultArguments As BitVector
			Get
				Return Me._DefaultArguments
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.PropertySymbol
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public ReadOnly Property IsWriteable As Boolean
			Get
				Return Me._IsWriteable
			End Get
		End Property

		Public ReadOnly Property PropertyGroupOpt As BoundPropertyGroup
			Get
				Return Me._PropertyGroupOpt
			End Get
		End Property

		Public ReadOnly Property PropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
			Get
				Return Me._PropertySymbol
			End Get
		End Property

		Public ReadOnly Property ReceiverOpt As BoundExpression
			Get
				Return Me._ReceiverOpt
			End Get
		End Property

		Public Overrides ReadOnly Property ResultKind As LookupResultKind
			Get
				Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind
				lookupResultKind = If(Me.PropertyGroupOpt Is Nothing, MyBase.ResultKind, Me.PropertyGroupOpt.ResultKind)
				Return lookupResultKind
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, ByVal propertyGroupOpt As BoundPropertyGroup, ByVal accessKind As PropertyAccessKind, ByVal isWriteable As Boolean, ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression), Optional ByVal defaultArguments As BitVector = Nothing, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, propertySymbol, propertyGroupOpt, accessKind, isWriteable, propertySymbol.ReturnsByRef, receiverOpt, arguments, defaultArguments, BoundPropertyAccess.GetTypeFromAccessKind(propertySymbol, accessKind), hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, ByVal propertyGroupOpt As BoundPropertyGroup, ByVal accessKind As PropertyAccessKind, ByVal isWriteable As Boolean, ByVal isLValue As Boolean, ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal defaultArguments As BitVector, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.PropertyAccess, syntax, type, If(hasErrors OrElse propertyGroupOpt.NonNullAndHasErrors() OrElse receiverOpt.NonNullAndHasErrors(), True, arguments.NonNullAndHasErrors()))
			Me._PropertySymbol = propertySymbol
			Me._PropertyGroupOpt = propertyGroupOpt
			Me._AccessKind = accessKind
			Me._IsWriteable = isWriteable
			Me._IsLValue = isLValue
			Me._ReceiverOpt = receiverOpt
			Me._Arguments = arguments
			Me._DefaultArguments = defaultArguments
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitPropertyAccess(Me)
		End Function

		Private Shared Function GetTypeFromAccessKind(ByVal [property] As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, ByVal accessKind As PropertyAccessKind) As TypeSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess::GetTypeFromAccessKind(Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.VisualBasic.PropertyAccessKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol GetTypeFromAccessKind(Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol,Microsoft.CodeAnalysis.VisualBasic.PropertyAccessKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess
			Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess
			boundPropertyAccess = If(Not Me._IsLValue, Me, Me.Update(Me.PropertySymbol, Me.PropertyGroupOpt, PropertyAccessKind.[Get], Me.IsWriteable, False, Me.ReceiverOpt, Me.Arguments, Me.DefaultArguments, MyBase.Type))
			Return boundPropertyAccess
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function SetAccessKind(ByVal newAccessKind As PropertyAccessKind) As BoundPropertyAccess
			Return Me.Update(Me.PropertySymbol, Me.PropertyGroupOpt, newAccessKind, Me.IsWriteable, Me.IsLValue, Me.ReceiverOpt, Me.Arguments, Me.DefaultArguments, BoundPropertyAccess.GetTypeFromAccessKind(Me.PropertySymbol, newAccessKind))
		End Function

		Public Function Update(ByVal propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol, ByVal propertyGroupOpt As BoundPropertyGroup, ByVal accessKind As PropertyAccessKind, ByVal isWriteable As Boolean, ByVal isLValue As Boolean, ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal defaultArguments As BitVector, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess
			Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess
			If (CObj(propertySymbol) <> CObj(Me.PropertySymbol) OrElse propertyGroupOpt <> Me.PropertyGroupOpt OrElse accessKind <> Me.AccessKind OrElse isWriteable <> Me.IsWriteable OrElse isLValue <> Me.IsLValue OrElse receiverOpt <> Me.ReceiverOpt OrElse arguments <> Me.Arguments OrElse defaultArguments <> Me.DefaultArguments OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundPropertyAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess(MyBase.Syntax, propertySymbol, propertyGroupOpt, accessKind, isWriteable, isLValue, receiverOpt, arguments, defaultArguments, type, MyBase.HasErrors)
				boundPropertyAccess1.CopyAttributes(Me)
				boundPropertyAccess = boundPropertyAccess1
			Else
				boundPropertyAccess = Me
			End If
			Return boundPropertyAccess
		End Function
	End Class
End Namespace