Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class EarlyWellKnownAttributeBinder
		Inherits Binder
		Private ReadOnly _owner As Symbol

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Return ImmutableArray(Of Symbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property BindingLocation As Microsoft.CodeAnalysis.VisualBasic.BindingLocation
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.BindingLocation.Attribute
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Return If(Me._owner, MyBase.ContainingMember)
			End Get
		End Property

		Friend Sub New(ByVal owner As Symbol, ByVal containingBinder As Binder)
			MyBase.New(containingBinder, New Nullable(Of Boolean)(True), Nothing)
			Me._owner = owner
		End Sub

		Friend Overrides Function BinderSpecificLookupOptions(ByVal options As LookupOptions) As LookupOptions
			Return MyBase.ContainingBinder.BinderSpecificLookupOptions(options) Or LookupOptions.IgnoreExtensionMethods
		End Function

		Friend Shared Function CanBeValidAttributeArgument(ByVal node As ExpressionSyntax, ByVal memberAccessBinder As Binder) As Boolean
			Dim flag As Boolean
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			Select Case syntaxKind
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FalseLiteralExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NumericLiteralExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringLiteralExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CallStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FalseLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DateLiteralExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MeExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyBaseExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyClassExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfIsExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfIsNotExpression
				Case 288
				Case 289
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlElementAccessExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlDescendantAccessExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlAttributeAccessExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateAssignmentStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimPreserveStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetXmlNamespaceExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnonymousObjectCreationExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DictionaryAccessExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CharacterLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingLiteralExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParenthesizedExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCreationExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastExpression Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsNotExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddressOfExpression
				Label1:
					flag = False
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParenthesizedExpression
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetTypeExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ArrayCreationExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CollectionInitializer
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleMemberAccessExpression
				Label0:
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InvocationExpression
					Dim expression As MemberAccessExpressionSyntax = TryCast(DirectCast(node, InvocationExpressionSyntax).Expression, MemberAccessExpressionSyntax)
					If (expression IsNot Nothing) Then
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = memberAccessBinder.BindExpression(expression, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded)
						If (Not boundExpression.HasErrors) Then
							Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = TryCast(boundExpression, Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup)
							If (boundMethodGroup IsNot Nothing AndAlso boundMethodGroup.Methods.Length = 1) Then
								Dim item As MethodSymbol = boundMethodGroup.Methods(0)
								Dim compilation As VisualBasicCompilation = memberAccessBinder.Compilation
								If (item = compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__ChrWInt32Char) OrElse item = compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__ChrInt32Char) OrElse item = compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__AscWCharInt32) OrElse item = compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Strings__AscCharInt32)) Then
									flag = True
									Exit Select
								End If
							End If
						Else
							flag = False
							Exit Select
						End If
					End If
					flag = False
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DirectCastExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedCastExpression
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubtractExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiplyExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DivideExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntegerDivideExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExponentiateExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LeftShiftExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RightShiftExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConcatenateExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuloExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotEqualsExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanOrEqualExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanOrEqualExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclusiveOrExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndAlsoExpression
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnaryPlusExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnaryMinusExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotExpression
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryConditionalExpression
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TernaryConditionalExpression
					flag = True
					Exit Select
				Case Else
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PredefinedType) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List) OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GlobalName) Then
						GoTo Label0
					End If
					GoTo Label1
			End Select
			Return flag
		End Function

		Friend Function GetAttribute(ByVal node As AttributeSyntax, ByVal boundAttributeType As NamedTypeSymbol, <Out> ByRef generatedDiagnostics As Boolean) As SourceAttributeData
			Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(True, False)
			Dim attribute As SourceAttributeData = MyBase.GetAttribute(node, boundAttributeType, instance)
			generatedDiagnostics = Not instance.DiagnosticBag.IsEmptyWithoutResolution
			instance.Free()
			Return attribute
		End Function
	End Class
End Namespace