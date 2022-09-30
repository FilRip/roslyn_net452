using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal struct ExpressionEvaluator
	{
		private readonly ImmutableDictionary<string, CConst> _symbols;

		private static readonly byte[,] s_dominantType;

		static ExpressionEvaluator()
		{
			s_dominantType = new byte[16, 16]
			{
				{
					10, 1, 11, 12, 13, 14, 15, 16, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					1, 9, 11, 1, 13, 1, 15, 1, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					11, 11, 11, 1, 13, 1, 15, 1, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					12, 1, 1, 12, 13, 14, 15, 16, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					13, 13, 13, 13, 13, 1, 15, 1, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					14, 1, 1, 14, 1, 14, 15, 16, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					15, 15, 15, 15, 15, 15, 15, 1, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					16, 1, 1, 16, 1, 16, 1, 16, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					18, 18, 18, 18, 18, 18, 18, 18, 18, 19,
					18, 1, 1, 1, 1, 1
				},
				{
					19, 19, 19, 19, 19, 19, 19, 19, 19, 19,
					19, 1, 1, 1, 1, 1
				},
				{
					17, 17, 17, 17, 17, 17, 17, 17, 18, 19,
					17, 1, 1, 1, 1, 1
				},
				{
					1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
					1, 33, 1, 1, 1, 1
				},
				{
					1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
					1, 1, 8, 1, 20, 1
				},
				{
					1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
					1, 1, 1, 7, 1, 1
				},
				{
					1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
					1, 1, 20, 1, 20, 1
				},
				{
					1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
					1, 1, 1, 1, 1, 1
				}
			};
		}

		private static int TypeCodeToDominantTypeIndex(SpecialType specialType)
		{
			return specialType switch
			{
				SpecialType.System_Byte => 0, 
				SpecialType.System_SByte => 1, 
				SpecialType.System_Int16 => 2, 
				SpecialType.System_UInt16 => 3, 
				SpecialType.System_Int32 => 4, 
				SpecialType.System_UInt32 => 5, 
				SpecialType.System_Int64 => 6, 
				SpecialType.System_UInt64 => 7, 
				SpecialType.System_Single => 8, 
				SpecialType.System_Double => 9, 
				SpecialType.System_Decimal => 10, 
				SpecialType.System_DateTime => 11, 
				SpecialType.System_Char => 12, 
				SpecialType.System_Boolean => 13, 
				SpecialType.System_String => 14, 
				SpecialType.System_Object => 15, 
				_ => throw ExceptionUtilities.UnexpectedValue(specialType), 
			};
		}

		private ExpressionEvaluator(ImmutableDictionary<string, CConst> symbols)
		{
			this = default(ExpressionEvaluator);
			_symbols = symbols;
		}

		public static CConst EvaluateCondition(ExpressionSyntax expr, ImmutableDictionary<string, CConst> symbols = null)
		{
			if (expr.ContainsDiagnostics)
			{
				return new BadCConst(ERRID.ERR_None);
			}
			CConst cConst = EvaluateExpression(expr, symbols);
			if (cConst.IsBad)
			{
				return cConst;
			}
			return ConvertToBool(cConst, expr);
		}

		public static CConst EvaluateExpression(ExpressionSyntax expr, ImmutableDictionary<string, CConst> symbols = null)
		{
			if (expr.ContainsDiagnostics)
			{
				return new BadCConst(ERRID.ERR_None);
			}
			return new ExpressionEvaluator(symbols).EvaluateExpressionInternal(expr);
		}

		private CConst EvaluateExpressionInternal(ExpressionSyntax expr)
		{
			switch (expr.Kind)
			{
			case SyntaxKind.CharacterLiteralExpression:
			case SyntaxKind.TrueLiteralExpression:
			case SyntaxKind.FalseLiteralExpression:
			case SyntaxKind.NumericLiteralExpression:
			case SyntaxKind.DateLiteralExpression:
			case SyntaxKind.StringLiteralExpression:
			case SyntaxKind.NothingLiteralExpression:
				return EvaluateLiteralExpression((LiteralExpressionSyntax)expr);
			case SyntaxKind.ParenthesizedExpression:
				return EvaluateParenthesizedExpression((ParenthesizedExpressionSyntax)expr);
			case SyntaxKind.IdentifierName:
				return EvaluateIdentifierNameExpression((IdentifierNameSyntax)expr);
			case SyntaxKind.PredefinedCastExpression:
				return EvaluatePredefinedCastExpression((PredefinedCastExpressionSyntax)expr);
			case SyntaxKind.CTypeExpression:
				return EvaluateCTypeExpression((CastExpressionSyntax)expr);
			case SyntaxKind.DirectCastExpression:
				return EvaluateDirectCastExpression((CastExpressionSyntax)expr);
			case SyntaxKind.TryCastExpression:
				return EvaluateTryCastExpression((CastExpressionSyntax)expr);
			case SyntaxKind.UnaryPlusExpression:
			case SyntaxKind.UnaryMinusExpression:
			case SyntaxKind.NotExpression:
				return EvaluateUnaryExpression((UnaryExpressionSyntax)expr);
			case SyntaxKind.AddExpression:
			case SyntaxKind.SubtractExpression:
			case SyntaxKind.MultiplyExpression:
			case SyntaxKind.DivideExpression:
			case SyntaxKind.IntegerDivideExpression:
			case SyntaxKind.ExponentiateExpression:
			case SyntaxKind.LeftShiftExpression:
			case SyntaxKind.RightShiftExpression:
			case SyntaxKind.ConcatenateExpression:
			case SyntaxKind.ModuloExpression:
			case SyntaxKind.EqualsExpression:
			case SyntaxKind.NotEqualsExpression:
			case SyntaxKind.LessThanExpression:
			case SyntaxKind.LessThanOrEqualExpression:
			case SyntaxKind.GreaterThanOrEqualExpression:
			case SyntaxKind.GreaterThanExpression:
			case SyntaxKind.OrExpression:
			case SyntaxKind.ExclusiveOrExpression:
			case SyntaxKind.AndExpression:
			case SyntaxKind.OrElseExpression:
			case SyntaxKind.AndAlsoExpression:
				return EvaluateBinaryExpression((BinaryExpressionSyntax)expr);
			case SyntaxKind.BinaryConditionalExpression:
				return EvaluateBinaryIfExpression((BinaryConditionalExpressionSyntax)expr);
			case SyntaxKind.TernaryConditionalExpression:
				return EvaluateTernaryIfExpression((TernaryConditionalExpressionSyntax)expr);
			default:
				return ReportSemanticError(ERRID.ERR_BadCCExpression, expr);
			}
		}

		private static BadCConst ReportSemanticError(ERRID id, VisualBasicSyntaxNode node)
		{
			return ReportSemanticError(id, node, Array.Empty<object>());
		}

		private static BadCConst ReportSemanticError(ERRID id, VisualBasicSyntaxNode node, params object[] args)
		{
			return new BadCConst(id, args);
		}

		private static CConst EvaluateLiteralExpression(LiteralExpressionSyntax expr)
		{
			SyntaxToken token = expr.Token;
			if (expr.ContainsDiagnostics)
			{
				return ReportSemanticError(ERRID.ERR_BadCCExpression, expr);
			}
			return token.Kind switch
			{
				SyntaxKind.TrueKeyword => CConst.Create(value: true), 
				SyntaxKind.FalseKeyword => CConst.Create(value: false), 
				SyntaxKind.CharacterLiteralToken => CConst.Create(((CharacterLiteralTokenSyntax)token).Value), 
				SyntaxKind.DateLiteralToken => CConst.Create(((DateLiteralTokenSyntax)token).Value), 
				SyntaxKind.DecimalLiteralToken => CConst.Create(((DecimalLiteralTokenSyntax)token).Value), 
				SyntaxKind.FloatingLiteralToken => CConst.CreateChecked(RuntimeHelpers.GetObjectValue(((FloatingLiteralTokenSyntax)token).ObjectValue)), 
				SyntaxKind.IntegerLiteralToken => CConst.CreateChecked(RuntimeHelpers.GetObjectValue(((IntegerLiteralTokenSyntax)token).ObjectValue)), 
				SyntaxKind.NothingKeyword => CConst.CreateNothing(), 
				SyntaxKind.StringLiteralToken => CConst.Create(((StringLiteralTokenSyntax)token).Value), 
				_ => throw ExceptionUtilities.UnexpectedValue(token.Kind), 
			};
		}

		private CConst EvaluateParenthesizedExpression(ParenthesizedExpressionSyntax expr)
		{
			return EvaluateExpressionInternal(expr.Expression);
		}

		private CConst EvaluateIdentifierNameExpression(IdentifierNameSyntax expr)
		{
			if (_symbols == null)
			{
				return CConst.CreateNothing();
			}
			IdentifierTokenSyntax identifier = expr.Identifier;
			CConst value = null;
			if (!_symbols.TryGetValue(identifier.IdentifierText, out value))
			{
				return CConst.CreateNothing();
			}
			if (value.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_None, expr);
			}
			TypeCharacter typeCharacter = identifier.TypeCharacter;
			if (typeCharacter != 0 && typeCharacter != AsTypeCharacter(value.SpecialType))
			{
				return ReportSemanticError(ERRID.ERR_TypecharNoMatch2, expr, GetDisplayString(typeCharacter), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(value.SpecialType));
			}
			return value;
		}

		private static string GetDisplayString(TypeCharacter typeChar)
		{
			return typeChar switch
			{
				TypeCharacter.Integer => "%", 
				TypeCharacter.Long => "&", 
				TypeCharacter.Decimal => "@", 
				TypeCharacter.Single => "!", 
				TypeCharacter.Double => "#", 
				TypeCharacter.String => "$", 
				_ => throw ExceptionUtilities.UnexpectedValue(typeChar), 
			};
		}

		private static TypeCharacter AsTypeCharacter(SpecialType specialType)
		{
			return specialType switch
			{
				SpecialType.System_Int32 => TypeCharacter.Integer, 
				SpecialType.System_Int64 => TypeCharacter.Long, 
				SpecialType.System_Decimal => TypeCharacter.Decimal, 
				SpecialType.System_Single => TypeCharacter.Single, 
				SpecialType.System_Double => TypeCharacter.Double, 
				SpecialType.System_String => TypeCharacter.String, 
				_ => TypeCharacter.None, 
			};
		}

		private static SpecialType GetSpecialType(PredefinedTypeSyntax predefinedType)
		{
			SyntaxKind kind = predefinedType.Keyword.Kind;
			switch (kind)
			{
			case SyntaxKind.ShortKeyword:
				return SpecialType.System_Int16;
			case SyntaxKind.UShortKeyword:
				return SpecialType.System_UInt16;
			case SyntaxKind.IntegerKeyword:
				return SpecialType.System_Int32;
			case SyntaxKind.UIntegerKeyword:
				return SpecialType.System_UInt32;
			case SyntaxKind.LongKeyword:
				return SpecialType.System_Int64;
			case SyntaxKind.ULongKeyword:
				return SpecialType.System_UInt64;
			case SyntaxKind.DecimalKeyword:
				return SpecialType.System_Decimal;
			case SyntaxKind.SingleKeyword:
				return SpecialType.System_Single;
			case SyntaxKind.DoubleKeyword:
				return SpecialType.System_Double;
			case SyntaxKind.SByteKeyword:
				return SpecialType.System_SByte;
			case SyntaxKind.ByteKeyword:
				return SpecialType.System_Byte;
			case SyntaxKind.BooleanKeyword:
				return SpecialType.System_Boolean;
			case SyntaxKind.CharKeyword:
				return SpecialType.System_Char;
			case SyntaxKind.DateKeyword:
				return SpecialType.System_DateTime;
			case SyntaxKind.StringKeyword:
				return SpecialType.System_String;
			case SyntaxKind.ObjectKeyword:
			case SyntaxKind.VariantKeyword:
				return SpecialType.System_Object;
			default:
				throw ExceptionUtilities.UnexpectedValue(kind);
			}
		}

		private CConst EvaluateTryCastExpression(CastExpressionSyntax expr)
		{
			CConst cConst = EvaluateExpressionInternal(expr.Expression);
			if (!(expr.Type is PredefinedTypeSyntax predefinedType))
			{
				return ReportSemanticError(ERRID.ERR_BadTypeInCCExpression, expr.Type);
			}
			SpecialType specialType = GetSpecialType(predefinedType);
			if (specialType != SpecialType.System_Object && specialType != SpecialType.System_String)
			{
				return ReportSemanticError(ERRID.ERR_TryCastOfValueType1, expr.Type);
			}
			if (cConst.SpecialType == SpecialType.System_Object || cConst.SpecialType == SpecialType.System_String)
			{
				return Convert(cConst, specialType, expr);
			}
			if (cConst.SpecialType == specialType)
			{
				if (specialType == SpecialType.System_Double || specialType == SpecialType.System_Single)
				{
					return ReportSemanticError(ERRID.ERR_IdentityDirectCastForFloat, expr.Type);
				}
				return ReportSemanticError(ERRID.WRN_ObsoleteIdentityDirectCastForValueType, expr.Type);
			}
			return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(cConst.SpecialType), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(specialType));
		}

		private CConst EvaluateDirectCastExpression(CastExpressionSyntax expr)
		{
			CConst cConst = EvaluateExpressionInternal(expr.Expression);
			if (!(expr.Type is PredefinedTypeSyntax predefinedType))
			{
				return ReportSemanticError(ERRID.ERR_BadTypeInCCExpression, expr.Type);
			}
			SpecialType specialType = GetSpecialType(predefinedType);
			if (cConst.SpecialType == SpecialType.System_Object || cConst.SpecialType == SpecialType.System_String)
			{
				return Convert(cConst, specialType, expr);
			}
			if (cConst.SpecialType == specialType)
			{
				if (specialType == SpecialType.System_Double || specialType == SpecialType.System_Single)
				{
					return ReportSemanticError(ERRID.ERR_IdentityDirectCastForFloat, expr.Type);
				}
				return Convert(cConst, specialType, expr).WithError(ERRID.WRN_ObsoleteIdentityDirectCastForValueType);
			}
			return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr.Type, cConst.SpecialType, specialType);
		}

		private CConst EvaluateCTypeExpression(CastExpressionSyntax expr)
		{
			CConst value = EvaluateExpressionInternal(expr.Expression);
			if (!(expr.Type is PredefinedTypeSyntax predefinedType))
			{
				return ReportSemanticError(ERRID.ERR_BadTypeInCCExpression, expr.Type);
			}
			SpecialType specialType = GetSpecialType(predefinedType);
			return Convert(value, specialType, expr);
		}

		private CConst EvaluatePredefinedCastExpression(PredefinedCastExpressionSyntax expr)
		{
			CConst value = EvaluateExpressionInternal(expr.Expression);
			SpecialType toSpecialType;
			switch (expr.Keyword.Kind)
			{
			case SyntaxKind.CBoolKeyword:
				toSpecialType = SpecialType.System_Boolean;
				break;
			case SyntaxKind.CDateKeyword:
				toSpecialType = SpecialType.System_DateTime;
				break;
			case SyntaxKind.CDblKeyword:
				toSpecialType = SpecialType.System_Double;
				break;
			case SyntaxKind.CSByteKeyword:
				toSpecialType = SpecialType.System_SByte;
				break;
			case SyntaxKind.CByteKeyword:
				toSpecialType = SpecialType.System_Byte;
				break;
			case SyntaxKind.CCharKeyword:
				toSpecialType = SpecialType.System_Char;
				break;
			case SyntaxKind.CShortKeyword:
				toSpecialType = SpecialType.System_Int16;
				break;
			case SyntaxKind.CUShortKeyword:
				toSpecialType = SpecialType.System_UInt16;
				break;
			case SyntaxKind.CIntKeyword:
				toSpecialType = SpecialType.System_Int32;
				break;
			case SyntaxKind.CUIntKeyword:
				toSpecialType = SpecialType.System_UInt32;
				break;
			case SyntaxKind.CLngKeyword:
				toSpecialType = SpecialType.System_Int64;
				break;
			case SyntaxKind.CULngKeyword:
				toSpecialType = SpecialType.System_UInt64;
				break;
			case SyntaxKind.CSngKeyword:
				toSpecialType = SpecialType.System_Single;
				break;
			case SyntaxKind.CStrKeyword:
				toSpecialType = SpecialType.System_String;
				break;
			case SyntaxKind.CDecKeyword:
				toSpecialType = SpecialType.System_Decimal;
				break;
			case SyntaxKind.CObjKeyword:
				return ConvertToObject(value, expr);
			default:
				throw ExceptionUtilities.UnexpectedValue(expr.Keyword.Kind);
			}
			return Convert(value, toSpecialType, expr);
		}

		private CConst EvaluateBinaryIfExpression(BinaryConditionalExpressionSyntax expr)
		{
			CConst cConst = EvaluateExpressionInternal(expr.FirstExpression);
			object objectValue = RuntimeHelpers.GetObjectValue(cConst.ValueAsObject);
			if (objectValue != null)
			{
				if (objectValue.GetType().GetTypeInfo().IsValueType)
				{
					return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
				}
				return cConst;
			}
			return EvaluateExpressionInternal(expr.SecondExpression);
		}

		private CConst EvaluateTernaryIfExpression(TernaryConditionalExpressionSyntax expr)
		{
			CConst cConst = EvaluateExpressionInternal(expr.Condition);
			if (cConst.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			CConst cConst2 = ConvertToBool(cConst, expr);
			if (cConst2.IsBad)
			{
				return cConst2;
			}
			CConst cConst3 = EvaluateExpressionInternal(expr.WhenTrue);
			CConst cConst4 = EvaluateExpressionInternal(expr.WhenFalse);
			if (!cConst3.IsBad && !cConst4.IsBad)
			{
				if (IsNothing(cConst3))
				{
					if (!IsNothing(cConst4) && cConst4.SpecialType != SpecialType.System_Object)
					{
						cConst3 = Convert(cConst3, cConst4.SpecialType, expr.WhenTrue);
					}
				}
				else if (IsNothing(cConst4))
				{
					if (cConst3.SpecialType != SpecialType.System_Object)
					{
						cConst4 = Convert(cConst4, cConst3.SpecialType, expr.WhenFalse);
					}
				}
				else
				{
					SpecialType specialType = (SpecialType)s_dominantType[TypeCodeToDominantTypeIndex(cConst3.SpecialType), TypeCodeToDominantTypeIndex(cConst4.SpecialType)];
					if (specialType != cConst3.SpecialType)
					{
						cConst3 = Convert(cConst3, specialType, expr.WhenTrue);
					}
					if (specialType != cConst4.SpecialType)
					{
						cConst4 = Convert(cConst4, specialType, expr.WhenFalse);
					}
				}
			}
			if (cConst3.IsBad)
			{
				return cConst3;
			}
			if (cConst4.IsBad)
			{
				return cConst4;
			}
			return ((CConst<bool>)cConst2).Value ? cConst3 : cConst4;
		}

		private static CConst ConvertToBool(CConst value, ExpressionSyntax expr)
		{
			if (value.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			SpecialType specialType = value.SpecialType;
			if (specialType == SpecialType.System_Boolean)
			{
				return (CConst<bool>)value;
			}
			if (specialType.IsNumericType())
			{
				return CConst.Create(Microsoft.VisualBasic.CompilerServices.Conversions.ToBoolean(value.ValueAsObject));
			}
			switch (specialType)
			{
			case SpecialType.System_Char:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Char), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Boolean));
			case SpecialType.System_DateTime:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_DateTime), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Boolean));
			case SpecialType.System_Object:
				if (value.ValueAsObject == null)
				{
					return CConst.Create(value: false);
				}
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			case SpecialType.System_String:
				return ReportSemanticError(ERRID.ERR_RequiredConstConversion2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_String), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Boolean));
			default:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Boolean));
			}
		}

		private static CConst ConvertToNumeric(CConst value, SpecialType toSpecialType, ExpressionSyntax expr)
		{
			if (value.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			if (IsNothing(value))
			{
				value = CConst.Create(0);
			}
			SpecialType specialType = value.SpecialType;
			if (specialType == toSpecialType)
			{
				return value;
			}
			if (specialType.IsNumericType())
			{
				return ConvertNumericToNumeric(value, toSpecialType, expr);
			}
			switch (specialType)
			{
			case SpecialType.System_Boolean:
			{
				long num = 0 - (((CConst<bool>)value).Value ? 1 : 0);
				if (toSpecialType.IsUnsignedIntegralType())
				{
					long sourceValue = num;
					bool overflow = false;
					num = CompileTimeCalculations.NarrowIntegralResult(sourceValue, SpecialType.System_Int64, toSpecialType, ref overflow);
				}
				return CConst.CreateChecked(RuntimeHelpers.GetObjectValue(System.Convert.ChangeType(num, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.ToRuntimeType(toSpecialType), CultureInfo.InvariantCulture)));
			}
			case SpecialType.System_Char:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Char), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(toSpecialType));
			case SpecialType.System_DateTime:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_DateTime), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(toSpecialType));
			case SpecialType.System_Object:
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			case SpecialType.System_String:
				return ReportSemanticError(ERRID.ERR_RequiredConstConversion2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_String), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(toSpecialType));
			default:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(specialType), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(toSpecialType));
			}
		}

		private static CConst ConvertNumericToNumeric(CConst value, SpecialType toSpecialType, ExpressionSyntax expr)
		{
			try
			{
				return CConst.CreateChecked(RuntimeHelpers.GetObjectValue(System.Convert.ChangeType(RuntimeHelpers.GetObjectValue(value.ValueAsObject), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.ToRuntimeType(toSpecialType), CultureInfo.InvariantCulture)));
			}
			catch (OverflowException ex)
			{
				ProjectData.SetProjectError(ex);
				OverflowException ex2 = ex;
				CConst result = ReportSemanticError(ERRID.ERR_ExpressionOverflow1, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(toSpecialType));
				ProjectData.ClearProjectError();
				return result;
			}
		}

		private static CConst Convert(CConst value, SpecialType toSpecialType, ExpressionSyntax expr)
		{
			if (value.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			SpecialType specialType = value.SpecialType;
			if (specialType == toSpecialType)
			{
				return value;
			}
			if (toSpecialType.IsNumericType())
			{
				return ConvertToNumeric(value, toSpecialType, expr);
			}
			return toSpecialType switch
			{
				SpecialType.System_Boolean => ConvertToBool(value, expr), 
				SpecialType.System_Char => ConvertToChar(value, expr), 
				SpecialType.System_DateTime => ConvertToDate(value, expr), 
				SpecialType.System_Object => ConvertToObject(value, expr), 
				SpecialType.System_String => ConvertToString(value, expr), 
				_ => ReportSemanticError(ERRID.ERR_CannotConvertValue2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(specialType), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(toSpecialType)), 
			};
		}

		private static CConst ConvertToChar(CConst value, ExpressionSyntax expr)
		{
			if (value.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			SpecialType specialType = value.SpecialType;
			if (specialType == SpecialType.System_Char)
			{
				return (CConst<char>)value;
			}
			if (specialType.IsIntegralType())
			{
				return ReportSemanticError(ERRID.ERR_IntegralToCharTypeMismatch1, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(specialType));
			}
			switch (specialType)
			{
			case SpecialType.System_Boolean:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Char));
			case SpecialType.System_DateTime:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Char));
			case SpecialType.System_Object:
				if (value.ValueAsObject == null)
				{
					return CConst.Create('\0');
				}
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			case SpecialType.System_String:
			{
				CConst<string> cConst = (CConst<string>)value;
				return CConst.Create((cConst.Value != null) ? Microsoft.VisualBasic.CompilerServices.Conversions.ToChar(cConst.Value) : '\0');
			}
			default:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Char));
			}
		}

		private static CConst ConvertToDate(CConst value, ExpressionSyntax expr)
		{
			if (value.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			SpecialType specialType = value.SpecialType;
			if (specialType == SpecialType.System_DateTime)
			{
				return (CConst<DateTime>)value;
			}
			if (specialType.IsIntegralType())
			{
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_DateTime));
			}
			switch (specialType)
			{
			case SpecialType.System_Boolean:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_DateTime));
			case SpecialType.System_Char:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_DateTime));
			case SpecialType.System_String:
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			case SpecialType.System_Object:
				if (value.ValueAsObject == null)
				{
					return CConst.Create(DateTime.MinValue);
				}
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			default:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_DateTime));
			}
		}

		private static CConst ConvertToString(CConst value, ExpressionSyntax expr)
		{
			if (value.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			SpecialType specialType = value.SpecialType;
			if (specialType == SpecialType.System_String)
			{
				return (CConst<string>)value;
			}
			if (specialType.IsIntegralType())
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			switch (specialType)
			{
			case SpecialType.System_Boolean:
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			case SpecialType.System_Char:
				return CConst.Create(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(((CConst<char>)value).Value));
			case SpecialType.System_DateTime:
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			case SpecialType.System_Object:
				if (value.ValueAsObject == null)
				{
					return CConst.Create(null);
				}
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			default:
				return ReportSemanticError(ERRID.ERR_TypeMismatch2, expr, specialType, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_String));
			}
		}

		private static CConst ConvertToObject(CConst value, ExpressionSyntax expr)
		{
			if (value.IsBad)
			{
				return value;
			}
			if (IsNothing(value))
			{
				return ReportSemanticError(ERRID.ERR_RequiredConstExpr, expr);
			}
			return ReportSemanticError(ERRID.ERR_RequiredConstConversion2, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(value.SpecialType), Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(SpecialType.System_Object));
		}

		private CConst EvaluateUnaryExpression(UnaryExpressionSyntax expr)
		{
			CConst cConst = EvaluateExpressionInternal(expr.Operand);
			SpecialType specialType = cConst.SpecialType;
			if (specialType == SpecialType.None)
			{
				return ReportSemanticError(ERRID.ERR_BadCCExpression, expr);
			}
			if (specialType == SpecialType.System_String || (specialType == SpecialType.System_Object && !IsNothing(cConst)) || specialType == SpecialType.System_Char || specialType == SpecialType.System_DateTime)
			{
				return ReportSemanticError(ERRID.ERR_UnaryOperand2, expr, expr.OperatorToken.ValueText, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(specialType));
			}
			try
			{
				switch (expr.Kind)
				{
				case SyntaxKind.UnaryMinusExpression:
					if (IsNothing(cConst))
					{
						return CConst.Create(0);
					}
					return specialType switch
					{
						SpecialType.System_Boolean => CConst.Create((short)(-(short)(0 - (((CConst<bool>)cConst).Value ? 1 : 0)))), 
						SpecialType.System_Byte => CConst.Create((short)(-((CConst<byte>)cConst).Value)), 
						SpecialType.System_Decimal => CConst.Create(decimal.Negate(((CConst<decimal>)cConst).Value)), 
						SpecialType.System_Double => CConst.Create(0.0 - ((CConst<double>)cConst).Value), 
						SpecialType.System_Int16 => CConst.Create((short)(-((CConst<short>)cConst).Value)), 
						SpecialType.System_Int32 => CConst.Create(-((CConst<int>)cConst).Value), 
						SpecialType.System_Int64 => CConst.Create(-((CConst<long>)cConst).Value), 
						SpecialType.System_SByte => CConst.Create((sbyte)(-((CConst<sbyte>)cConst).Value)), 
						SpecialType.System_Single => CConst.Create(0f - ((CConst<float>)cConst).Value), 
						SpecialType.System_UInt16 => CConst.Create(-((CConst<ushort>)cConst).Value), 
						SpecialType.System_UInt32 => CConst.Create(0L - (long)((CConst<uint>)cConst).Value), 
						SpecialType.System_UInt64 => CConst.Create(decimal.Negate(new decimal(((CConst<ulong>)cConst).Value))), 
						_ => throw ExceptionUtilities.UnexpectedValue(specialType), 
					};
				case SyntaxKind.UnaryPlusExpression:
					if (specialType == SpecialType.System_Boolean)
					{
						return CConst.Create((short)(0 - (((CConst<bool>)cConst).Value ? 1 : 0)));
					}
					return cConst;
				case SyntaxKind.NotExpression:
					if (IsNothing(cConst))
					{
						return CConst.Create(-1);
					}
					return specialType switch
					{
						SpecialType.System_Boolean => CConst.Create(!((CConst<bool>)cConst).Value), 
						SpecialType.System_Byte => CConst.Create((byte)(~((CConst<byte>)cConst).Value)), 
						SpecialType.System_Decimal => CConst.Create(~System.Convert.ToInt64(((CConst<decimal>)cConst).Value)), 
						SpecialType.System_Double => CConst.Create(~(long)Math.Round(((CConst<double>)cConst).Value)), 
						SpecialType.System_Int16 => CConst.Create((short)(~((CConst<short>)cConst).Value)), 
						SpecialType.System_Int32 => CConst.Create(~((CConst<int>)cConst).Value), 
						SpecialType.System_Int64 => CConst.Create(~((CConst<long>)cConst).Value), 
						SpecialType.System_SByte => CConst.Create((sbyte)(~((CConst<sbyte>)cConst).Value)), 
						SpecialType.System_Single => CConst.Create(~(long)Math.Round(((CConst<float>)cConst).Value)), 
						SpecialType.System_UInt16 => CConst.Create((ushort)(~((CConst<ushort>)cConst).Value)), 
						SpecialType.System_UInt32 => CConst.Create(~((CConst<uint>)cConst).Value), 
						SpecialType.System_UInt64 => CConst.Create(~((CConst<ulong>)cConst).Value), 
						_ => throw ExceptionUtilities.UnexpectedValue(specialType), 
					};
				}
			}
			catch (OverflowException ex)
			{
				ProjectData.SetProjectError(ex);
				OverflowException ex2 = ex;
				CConst result = ReportSemanticError(ERRID.ERR_ExpressionOverflow1, expr);
				ProjectData.ClearProjectError();
				return result;
			}
			throw ExceptionUtilities.UnexpectedValue(expr);
		}

		private static bool IsNothing(CConst val)
		{
			if (val.SpecialType == SpecialType.System_Object)
			{
				return val.ValueAsObject == null;
			}
			return false;
		}

		private CConst EvaluateBinaryExpression(BinaryExpressionSyntax expr)
		{
			CConst cConst = EvaluateExpressionInternal(expr.Left);
			CConst cConst2 = EvaluateExpressionInternal(expr.Right);
			SyntaxKind syntaxKind = expr.Kind;
			if (cConst.IsBad || cConst2.IsBad)
			{
				return ReportSemanticError(ERRID.ERR_BadCCExpression, expr);
			}
			SpecialType specialType = SpecialType.None;
			if (IsNothing(cConst) || IsNothing(cConst2))
			{
				if (IsNothing(cConst) && IsNothing(cConst2))
				{
					switch (syntaxKind)
					{
					case SyntaxKind.ConcatenateExpression:
					case SyntaxKind.LikeExpression:
						cConst2 = ConvertToString(cConst2, expr.Right);
						break;
					case SyntaxKind.OrElseExpression:
					case SyntaxKind.AndAlsoExpression:
						cConst2 = ConvertToBool(cConst2, expr.Right);
						break;
					case SyntaxKind.AddExpression:
					case SyntaxKind.SubtractExpression:
					case SyntaxKind.MultiplyExpression:
					case SyntaxKind.DivideExpression:
					case SyntaxKind.IntegerDivideExpression:
					case SyntaxKind.ExponentiateExpression:
					case SyntaxKind.LeftShiftExpression:
					case SyntaxKind.RightShiftExpression:
					case SyntaxKind.ModuloExpression:
					case SyntaxKind.EqualsExpression:
					case SyntaxKind.NotEqualsExpression:
					case SyntaxKind.LessThanExpression:
					case SyntaxKind.LessThanOrEqualExpression:
					case SyntaxKind.GreaterThanOrEqualExpression:
					case SyntaxKind.GreaterThanExpression:
					case SyntaxKind.IsExpression:
					case SyntaxKind.IsNotExpression:
						cConst2 = ConvertToNumeric(cConst2, SpecialType.System_Int32, expr.Right);
						break;
					case SyntaxKind.OrExpression:
					case SyntaxKind.ExclusiveOrExpression:
					case SyntaxKind.AndExpression:
						cConst2 = ConvertToNumeric(cConst2, SpecialType.System_Int32, expr.Right);
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(expr);
					}
				}
				if (IsNothing(cConst))
				{
					specialType = cConst2.SpecialType;
					switch (syntaxKind)
					{
					case SyntaxKind.ConcatenateExpression:
					case SyntaxKind.LikeExpression:
						specialType = SpecialType.System_String;
						break;
					case SyntaxKind.LeftShiftExpression:
					case SyntaxKind.RightShiftExpression:
						specialType = SpecialType.System_Int32;
						break;
					}
					cConst = Convert(cConst, specialType, expr.Left);
				}
				else if (IsNothing(cConst2))
				{
					specialType = cConst.SpecialType;
					if (syntaxKind == SyntaxKind.ConcatenateExpression || syntaxKind == SyntaxKind.LikeExpression)
					{
						specialType = SpecialType.System_String;
					}
					cConst2 = Convert(cConst2, specialType, expr.Right);
				}
			}
			SpecialType specialType2 = OperatorResolution.LookupInOperatorTables(syntaxKind, cConst.SpecialType, cConst2.SpecialType);
			if (specialType2 == SpecialType.None)
			{
				return ReportSemanticError(ERRID.ERR_BadTypeInCCExpression, expr);
			}
			specialType = specialType2;
			cConst = Convert(cConst, specialType, expr.Left);
			if (cConst.IsBad)
			{
				return cConst;
			}
			cConst2 = Convert(cConst2, specialType, expr.Right);
			if (cConst2.IsBad)
			{
				return cConst2;
			}
			switch (syntaxKind)
			{
			case SyntaxKind.AddExpression:
				if (specialType2 == SpecialType.System_String)
				{
					syntaxKind = SyntaxKind.ConcatenateExpression;
				}
				break;
			case SyntaxKind.EqualsExpression:
			case SyntaxKind.NotEqualsExpression:
			case SyntaxKind.LessThanExpression:
			case SyntaxKind.LessThanOrEqualExpression:
			case SyntaxKind.GreaterThanOrEqualExpression:
			case SyntaxKind.GreaterThanExpression:
				specialType2 = SpecialType.System_Boolean;
				break;
			}
			return PerformCompileTimeBinaryOperation(syntaxKind, specialType2, cConst, cConst2, expr);
		}

		private static CConst PerformCompileTimeBinaryOperation(SyntaxKind opcode, SpecialType resultType, CConst left, CConst right, ExpressionSyntax expr)
		{
			if (left.SpecialType.IsIntegralType() || left.SpecialType == SpecialType.System_Char || left.SpecialType == SpecialType.System_DateTime)
			{
				long num = TypeHelpers.UncheckedCLng(left);
				long num2 = TypeHelpers.UncheckedCLng(right);
				if (resultType == SpecialType.System_Boolean)
				{
					bool flag = false;
					return CConst.Create(opcode switch
					{
						SyntaxKind.EqualsExpression => left.SpecialType.IsUnsignedIntegralType() ? (CompileTimeCalculations.UncheckedCULng(num) == CompileTimeCalculations.UncheckedCULng(num2)) : (num == num2), 
						SyntaxKind.NotEqualsExpression => left.SpecialType.IsUnsignedIntegralType() ? (CompileTimeCalculations.UncheckedCULng(num) != CompileTimeCalculations.UncheckedCULng(num2)) : (num != num2), 
						SyntaxKind.LessThanOrEqualExpression => left.SpecialType.IsUnsignedIntegralType() ? (CompileTimeCalculations.UncheckedCULng(num) <= CompileTimeCalculations.UncheckedCULng(num2)) : (num <= num2), 
						SyntaxKind.GreaterThanOrEqualExpression => left.SpecialType.IsUnsignedIntegralType() ? (CompileTimeCalculations.UncheckedCULng(num) >= CompileTimeCalculations.UncheckedCULng(num2)) : (num >= num2), 
						SyntaxKind.LessThanExpression => left.SpecialType.IsUnsignedIntegralType() ? (CompileTimeCalculations.UncheckedCULng(num) < CompileTimeCalculations.UncheckedCULng(num2)) : (num < num2), 
						SyntaxKind.GreaterThanExpression => left.SpecialType.IsUnsignedIntegralType() ? (CompileTimeCalculations.UncheckedCULng(num) > CompileTimeCalculations.UncheckedCULng(num2)) : (num > num2), 
						_ => throw ExceptionUtilities.UnexpectedValue(opcode), 
					});
				}
				long num3 = 0L;
				bool overflow = false;
				switch (opcode)
				{
				case SyntaxKind.AddExpression:
					num3 = CompileTimeCalculations.NarrowIntegralResult(num + num2, left.SpecialType, resultType, ref overflow);
					if (!resultType.IsUnsignedIntegralType())
					{
						if ((num2 > 0 && num3 < num) || (num2 < 0 && num3 > num))
						{
							overflow = true;
						}
					}
					else if (CompileTimeCalculations.UncheckedCULng(num3) < CompileTimeCalculations.UncheckedCULng(num))
					{
						overflow = true;
					}
					break;
				case SyntaxKind.SubtractExpression:
					num3 = CompileTimeCalculations.NarrowIntegralResult(num - num2, left.SpecialType, resultType, ref overflow);
					if (!resultType.IsUnsignedIntegralType())
					{
						if ((num2 > 0 && num3 > num) || (num2 < 0 && num3 < num))
						{
							overflow = true;
						}
					}
					else if (CompileTimeCalculations.UncheckedCULng(num3) > CompileTimeCalculations.UncheckedCULng(num))
					{
						overflow = true;
					}
					break;
				case SyntaxKind.MultiplyExpression:
					num3 = CompileTimeCalculations.Multiply(num, num2, left.SpecialType, resultType, ref overflow);
					break;
				case SyntaxKind.IntegerDivideExpression:
					if (num2 == 0L)
					{
						return ReportSemanticError(ERRID.ERR_ZeroDivide, expr);
					}
					num3 = CompileTimeCalculations.NarrowIntegralResult(resultType.IsUnsignedIntegralType() ? CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(num) / CompileTimeCalculations.UncheckedCULng(num2)) : CompileTimeCalculations.UncheckedIntegralDiv(num, num2), left.SpecialType, resultType, ref overflow);
					if (!resultType.IsUnsignedIntegralType() && num == long.MinValue && num2 == -1)
					{
						overflow = true;
					}
					break;
				case SyntaxKind.ModuloExpression:
					if (num2 == 0L)
					{
						return ReportSemanticError(ERRID.ERR_ZeroDivide, expr);
					}
					num3 = ((!resultType.IsUnsignedIntegralType()) ? ((num2 == -1) ? 0 : (num % num2)) : CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(num) % CompileTimeCalculations.UncheckedCULng(num2)));
					break;
				case SyntaxKind.ExclusiveOrExpression:
					num3 = num ^ num2;
					break;
				case SyntaxKind.OrExpression:
					num3 = num | num2;
					break;
				case SyntaxKind.AndExpression:
					num3 = num & num2;
					break;
				case SyntaxKind.LeftShiftExpression:
				{
					num2 &= Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetShiftSizeMask(left.SpecialType);
					num3 = num << (int)num2;
					bool overflow2 = false;
					num3 = CompileTimeCalculations.NarrowIntegralResult(num3, left.SpecialType, resultType, ref overflow2);
					break;
				}
				case SyntaxKind.RightShiftExpression:
					num2 &= Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetShiftSizeMask(left.SpecialType);
					num3 = ((!resultType.IsUnsignedIntegralType()) ? (num >> (int)num2) : CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(num) >> (int)num2));
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(opcode);
				}
				if (overflow)
				{
					return ReportSemanticError(ERRID.ERR_ExpressionOverflow1, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(resultType));
				}
				return Convert(CConst.Create(num3), resultType, expr);
			}
			if (Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.IsFloatingType(left.SpecialType))
			{
				double num4 = Microsoft.VisualBasic.CompilerServices.Conversions.ToDouble(left.ValueAsObject);
				double num5 = Microsoft.VisualBasic.CompilerServices.Conversions.ToDouble(right.ValueAsObject);
				if (resultType == SpecialType.System_Boolean)
				{
					bool flag2 = false;
					return CConst.Create((opcode switch
					{
						SyntaxKind.EqualsExpression => num4 == num5, 
						SyntaxKind.NotEqualsExpression => num4 != num5, 
						SyntaxKind.LessThanOrEqualExpression => num4 <= num5, 
						SyntaxKind.GreaterThanOrEqualExpression => num4 >= num5, 
						SyntaxKind.LessThanExpression => num4 < num5, 
						SyntaxKind.GreaterThanExpression => num4 > num5, 
						_ => throw ExceptionUtilities.UnexpectedValue(opcode), 
					}) ? true : false);
				}
				double num6 = 0.0;
				bool overflow3 = false;
				switch (opcode)
				{
				case SyntaxKind.AddExpression:
					num6 = num4 + num5;
					break;
				case SyntaxKind.SubtractExpression:
					num6 = num4 - num5;
					break;
				case SyntaxKind.MultiplyExpression:
					num6 = num4 * num5;
					break;
				case SyntaxKind.ExponentiateExpression:
					if (double.IsInfinity(num5))
					{
						if (num4 == 1.0)
						{
							num6 = num4;
							break;
						}
						if (num4 == -1.0)
						{
							num6 = double.NaN;
							break;
						}
					}
					else if (double.IsNaN(num5))
					{
						num6 = double.NaN;
						break;
					}
					num6 = Math.Pow(num4, num5);
					break;
				case SyntaxKind.DivideExpression:
					num6 = num4 / num5;
					break;
				case SyntaxKind.ModuloExpression:
					num6 = Math.IEEERemainder(num4, num5);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(opcode);
				}
				num6 = CompileTimeCalculations.NarrowFloatingResult(num6, resultType, ref overflow3);
				return Convert(CConst.Create(num6), resultType, expr);
			}
			if (left.SpecialType == SpecialType.System_Decimal)
			{
				decimal pdecLeft = Microsoft.VisualBasic.CompilerServices.Conversions.ToDecimal(left.ValueAsObject);
				decimal num7 = Microsoft.VisualBasic.CompilerServices.Conversions.ToDecimal(right.ValueAsObject);
				if (resultType == SpecialType.System_Boolean)
				{
					bool flag3 = false;
					int num8 = pdecLeft.CompareTo(num7);
					return CConst.Create(opcode switch
					{
						SyntaxKind.EqualsExpression => num8 == 0, 
						SyntaxKind.NotEqualsExpression => num8 != 0, 
						SyntaxKind.LessThanOrEqualExpression => num8 <= 0, 
						SyntaxKind.GreaterThanOrEqualExpression => num8 >= 0, 
						SyntaxKind.LessThanExpression => num8 < 0, 
						SyntaxKind.GreaterThanExpression => num8 > 0, 
						_ => throw ExceptionUtilities.UnexpectedValue(opcode), 
					});
				}
				bool flag4 = false;
				decimal pdecResult = default(decimal);
				switch (opcode)
				{
				case SyntaxKind.AddExpression:
					flag4 = TypeHelpers.VarDecAdd(pdecLeft, num7, ref pdecResult);
					break;
				case SyntaxKind.SubtractExpression:
					flag4 = TypeHelpers.VarDecSub(pdecLeft, num7, ref pdecResult);
					break;
				case SyntaxKind.MultiplyExpression:
					flag4 = TypeHelpers.VarDecMul(pdecLeft, num7, ref pdecResult);
					break;
				case SyntaxKind.DivideExpression:
					if (decimal.Compare(num7, 0m) == 0)
					{
						return ReportSemanticError(ERRID.ERR_ZeroDivide, expr);
					}
					flag4 = TypeHelpers.VarDecDiv(pdecLeft, num7, ref pdecResult);
					break;
				case SyntaxKind.ModuloExpression:
					if (decimal.Compare(num7, 0m) == 0)
					{
						return ReportSemanticError(ERRID.ERR_ZeroDivide, expr);
					}
					flag4 = TypeHelpers.VarDecDiv(pdecLeft, num7, ref pdecResult);
					if (!flag4)
					{
						pdecResult = decimal.Truncate(pdecResult);
						flag4 = TypeHelpers.VarDecMul(pdecResult, num7, ref pdecResult);
						if (!flag4)
						{
							flag4 = TypeHelpers.VarDecSub(pdecLeft, pdecResult, ref pdecResult);
						}
					}
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(opcode);
				}
				if (flag4)
				{
					return ReportSemanticError(ERRID.ERR_ExpressionOverflow1, expr, Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.GetDisplayName(resultType));
				}
				return CConst.Create(pdecResult);
			}
			if (left.SpecialType == SpecialType.System_String)
			{
				string text = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(left.ValueAsObject) ?? "";
				string text2 = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(right.ValueAsObject) ?? "";
				switch (opcode)
				{
				case SyntaxKind.ConcatenateExpression:
					return CConst.Create(text + text2);
				case SyntaxKind.EqualsExpression:
				case SyntaxKind.NotEqualsExpression:
				case SyntaxKind.LessThanExpression:
				case SyntaxKind.LessThanOrEqualExpression:
				case SyntaxKind.GreaterThanOrEqualExpression:
				case SyntaxKind.GreaterThanExpression:
				{
					bool value = false;
					int num9 = StringComparer.Ordinal.Compare(text, text2);
					switch (opcode)
					{
					case SyntaxKind.EqualsExpression:
						value = num9 == 0;
						break;
					case SyntaxKind.NotEqualsExpression:
						value = num9 != 0;
						break;
					case SyntaxKind.GreaterThanExpression:
						value = num9 > 0;
						break;
					case SyntaxKind.GreaterThanOrEqualExpression:
						value = num9 >= 0;
						break;
					case SyntaxKind.LessThanExpression:
						value = num9 < 0;
						break;
					case SyntaxKind.LessThanOrEqualExpression:
						value = num9 <= 0;
						break;
					}
					return CConst.Create(value);
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(opcode);
				}
			}
			if (left.SpecialType == SpecialType.System_Boolean)
			{
				bool flag5 = Microsoft.VisualBasic.CompilerServices.Conversions.ToBoolean(left.ValueAsObject);
				bool flag6 = Microsoft.VisualBasic.CompilerServices.Conversions.ToBoolean(right.ValueAsObject);
				bool flag7 = false;
				switch (opcode)
				{
				case SyntaxKind.EqualsExpression:
					flag7 = flag5 == flag6;
					break;
				case SyntaxKind.NotEqualsExpression:
					flag7 = flag5 != flag6;
					break;
				case SyntaxKind.GreaterThanExpression:
					flag7 = !flag5 && flag6;
					break;
				case SyntaxKind.GreaterThanOrEqualExpression:
					flag7 = !flag5 || flag6;
					break;
				case SyntaxKind.LessThanExpression:
					flag7 = flag5 && !flag6;
					break;
				case SyntaxKind.LessThanOrEqualExpression:
					flag7 = flag5 || !flag6;
					break;
				case SyntaxKind.ExclusiveOrExpression:
					flag7 = flag5 ^ flag6;
					break;
				case SyntaxKind.OrExpression:
				case SyntaxKind.OrElseExpression:
					flag7 = flag5 || flag6;
					break;
				case SyntaxKind.AndExpression:
				case SyntaxKind.AndAlsoExpression:
					flag7 = flag5 && flag6;
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(opcode);
				}
				return CConst.Create(flag7);
			}
			throw ExceptionUtilities.Unreachable;
		}
	}
}
