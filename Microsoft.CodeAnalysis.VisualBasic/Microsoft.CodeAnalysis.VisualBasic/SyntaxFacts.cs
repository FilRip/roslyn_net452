using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public class SyntaxFacts
	{
		private sealed class SyntaxKindEqualityComparer : IEqualityComparer<SyntaxKind>
		{
			public bool Equals(SyntaxKind x, SyntaxKind y)
			{
				return x == y;
			}

			bool IEqualityComparer<SyntaxKind>.Equals(SyntaxKind x, SyntaxKind y)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Equals
				return this.Equals(x, y);
			}

			public int GetHashCode(SyntaxKind obj)
			{
				return (int)obj;
			}

			int IEqualityComparer<SyntaxKind>.GetHashCode(SyntaxKind obj)
			{
				//ILSpy generated this explicit interface implementation from .override directive in GetHashCode
				return this.GetHashCode(obj);
			}
		}

		private const int s_fullwidth = 65248;

		internal const char CHARACTER_TABULATION = '\t';

		internal const char LINE_FEED = '\n';

		internal const char CARRIAGE_RETURN = '\r';

		internal const char SPACE = ' ';

		internal const char NO_BREAK_SPACE = '\u00a0';

		internal const char IDEOGRAPHIC_SPACE = '\u3000';

		internal const char LINE_SEPARATOR = '\u2028';

		internal const char PARAGRAPH_SEPARATOR = '\u2029';

		internal const char NEXT_LINE = '\u0085';

		internal const char LEFT_SINGLE_QUOTATION_MARK = '‘';

		internal const char RIGHT_SINGLE_QUOTATION_MARK = '’';

		internal const char LEFT_DOUBLE_QUOTATION_MARK = '“';

		internal const char RIGHT_DOUBLE_QUOTATION_MARK = '”';

		internal const char FULLWIDTH_APOSTROPHE = '＇';

		internal const char FULLWIDTH_QUOTATION_MARK = '＂';

		internal const char FULLWIDTH_DIGIT_ZERO = '０';

		internal const char FULLWIDTH_DIGIT_ONE = '１';

		internal const char FULLWIDTH_DIGIT_SEVEN = '７';

		internal const char FULLWIDTH_DIGIT_NINE = '９';

		internal const char FULLWIDTH_LOW_LINE = '\uff3f';

		internal const char FULLWIDTH_COLON = '：';

		internal const char FULLWIDTH_SOLIDUS = '／';

		internal const char FULLWIDTH_HYPHEN_MINUS = '－';

		internal const char FULLWIDTH_PLUS_SIGN = '＋';

		internal const char FULLWIDTH_NUMBER_SIGN = '＃';

		internal const char FULLWIDTH_EQUALS_SIGN = '＝';

		internal const char FULLWIDTH_LESS_THAN_SIGN = '＜';

		internal const char FULLWIDTH_GREATER_THAN_SIGN = '＞';

		internal const char FULLWIDTH_LEFT_PARENTHESIS = '（';

		internal const char FULLWIDTH_LEFT_SQUARE_BRACKET = '［';

		internal const char FULLWIDTH_RIGHT_SQUARE_BRACKET = '］';

		internal const char FULLWIDTH_LEFT_CURLY_BRACKET = '｛';

		internal const char FULLWIDTH_RIGHT_CURLY_BRACKET = '｝';

		internal const char FULLWIDTH_AMPERSAND = '＆';

		internal const char FULLWIDTH_DOLLAR_SIGN = '＄';

		internal const char FULLWIDTH_QUESTION_MARK = '？';

		internal const char FULLWIDTH_FULL_STOP = '．';

		internal const char FULLWIDTH_COMMA = '，';

		internal const char FULLWIDTH_PERCENT_SIGN = '％';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_B = 'Ｂ';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_H = 'Ｈ';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_O = 'Ｏ';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_E = 'Ｅ';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_A = 'Ａ';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_F = 'Ｆ';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_C = 'Ｃ';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_P = 'Ｐ';

		internal const char FULLWIDTH_LATIN_CAPITAL_LETTER_M = 'Ｍ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_B = 'ｂ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_H = 'ｈ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_O = 'ｏ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_E = 'ｅ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_A = 'ａ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_F = 'ｆ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_C = 'ｃ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_P = 'ｐ';

		internal const char FULLWIDTH_LATIN_SMALL_LETTER_M = 'ｍ';

		internal const string FULLWIDTH_LEFT_PARENTHESIS_STRING = "（";

		internal const string FULLWIDTH_RIGHT_PARENTHESIS_STRING = "）";

		internal const string FULLWIDTH_LEFT_CURLY_BRACKET_STRING = "｛";

		internal const string FULLWIDTH_RIGHT_CURLY_BRACKET_STRING = "｝";

		internal const string FULLWIDTH_FULL_STOP_STRING = "．";

		internal const string FULLWIDTH_COMMA_STRING = "，";

		internal const string FULLWIDTH_EQUALS_SIGN_STRING = "＝";

		internal const string FULLWIDTH_PLUS_SIGN_STRING = "＋";

		internal const string FULLWIDTH_HYPHEN_MINUS_STRING = "－";

		internal const string FULLWIDTH_ASTERISK_STRING = "＊";

		internal const string FULLWIDTH_SOLIDUS_STRING = "／";

		internal const string FULLWIDTH_REVERSE_SOLIDUS_STRING = "＼";

		internal const string FULLWIDTH_COLON_STRING = "：";

		internal const string FULLWIDTH_CIRCUMFLEX_ACCENT_STRING = "\uff3e";

		internal const string FULLWIDTH_AMPERSAND_STRING = "＆";

		internal const string FULLWIDTH_NUMBER_SIGN_STRING = "＃";

		internal const string FULLWIDTH_EXCLAMATION_MARK_STRING = "！";

		internal const string FULLWIDTH_QUESTION_MARK_STRING = "？";

		internal const string FULLWIDTH_COMMERCIAL_AT_STRING = "＠";

		internal const string FULLWIDTH_LESS_THAN_SIGN_STRING = "＜";

		internal const string FULLWIDTH_GREATER_THAN_SIGN_STRING = "＞";

		private static readonly bool[] s_isIDChar = new bool[128]
		{
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, false, false,
			false, false, false, false, false, false, false, false, true, true,
			true, true, true, true, true, true, true, true, false, false,
			false, false, false, false, false, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, false, false, false, false, true, false, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, true, true, true, true, true, true, true,
			true, true, true, false, false, false, false, false
		};

		internal static readonly int[] DaysToMonth365 = new int[13]
		{
			0, 31, 59, 90, 120, 151, 181, 212, 243, 273,
			304, 334, 365
		};

		internal static readonly int[] DaysToMonth366 = new int[13]
		{
			0, 31, 60, 91, 121, 152, 182, 213, 244, 274,
			305, 335, 366
		};

		private static readonly SyntaxKind[] s_reservedKeywords = new SyntaxKind[154]
		{
			SyntaxKind.AddressOfKeyword,
			SyntaxKind.AddHandlerKeyword,
			SyntaxKind.AliasKeyword,
			SyntaxKind.AndKeyword,
			SyntaxKind.AndAlsoKeyword,
			SyntaxKind.AsKeyword,
			SyntaxKind.BooleanKeyword,
			SyntaxKind.ByRefKeyword,
			SyntaxKind.ByteKeyword,
			SyntaxKind.ByValKeyword,
			SyntaxKind.CallKeyword,
			SyntaxKind.CaseKeyword,
			SyntaxKind.CatchKeyword,
			SyntaxKind.CBoolKeyword,
			SyntaxKind.CByteKeyword,
			SyntaxKind.CCharKeyword,
			SyntaxKind.CDateKeyword,
			SyntaxKind.CDecKeyword,
			SyntaxKind.CDblKeyword,
			SyntaxKind.CharKeyword,
			SyntaxKind.CIntKeyword,
			SyntaxKind.ClassKeyword,
			SyntaxKind.CLngKeyword,
			SyntaxKind.CObjKeyword,
			SyntaxKind.ConstKeyword,
			SyntaxKind.ReferenceKeyword,
			SyntaxKind.ContinueKeyword,
			SyntaxKind.CSByteKeyword,
			SyntaxKind.CShortKeyword,
			SyntaxKind.CSngKeyword,
			SyntaxKind.CStrKeyword,
			SyntaxKind.CTypeKeyword,
			SyntaxKind.CUIntKeyword,
			SyntaxKind.CULngKeyword,
			SyntaxKind.CUShortKeyword,
			SyntaxKind.DateKeyword,
			SyntaxKind.DecimalKeyword,
			SyntaxKind.DeclareKeyword,
			SyntaxKind.DefaultKeyword,
			SyntaxKind.DelegateKeyword,
			SyntaxKind.DimKeyword,
			SyntaxKind.DirectCastKeyword,
			SyntaxKind.DoKeyword,
			SyntaxKind.DoubleKeyword,
			SyntaxKind.EachKeyword,
			SyntaxKind.ElseKeyword,
			SyntaxKind.ElseIfKeyword,
			SyntaxKind.EndKeyword,
			SyntaxKind.EnumKeyword,
			SyntaxKind.EraseKeyword,
			SyntaxKind.ErrorKeyword,
			SyntaxKind.EventKeyword,
			SyntaxKind.ExitKeyword,
			SyntaxKind.FalseKeyword,
			SyntaxKind.FinallyKeyword,
			SyntaxKind.ForKeyword,
			SyntaxKind.FriendKeyword,
			SyntaxKind.FunctionKeyword,
			SyntaxKind.GetKeyword,
			SyntaxKind.GetTypeKeyword,
			SyntaxKind.GetXmlNamespaceKeyword,
			SyntaxKind.GlobalKeyword,
			SyntaxKind.GoToKeyword,
			SyntaxKind.HandlesKeyword,
			SyntaxKind.IfKeyword,
			SyntaxKind.ImplementsKeyword,
			SyntaxKind.ImportsKeyword,
			SyntaxKind.InKeyword,
			SyntaxKind.InheritsKeyword,
			SyntaxKind.IntegerKeyword,
			SyntaxKind.InterfaceKeyword,
			SyntaxKind.IsKeyword,
			SyntaxKind.IsNotKeyword,
			SyntaxKind.LetKeyword,
			SyntaxKind.LibKeyword,
			SyntaxKind.LikeKeyword,
			SyntaxKind.LongKeyword,
			SyntaxKind.LoopKeyword,
			SyntaxKind.MeKeyword,
			SyntaxKind.ModKeyword,
			SyntaxKind.ModuleKeyword,
			SyntaxKind.MustInheritKeyword,
			SyntaxKind.MustOverrideKeyword,
			SyntaxKind.MyBaseKeyword,
			SyntaxKind.MyClassKeyword,
			SyntaxKind.NameOfKeyword,
			SyntaxKind.NamespaceKeyword,
			SyntaxKind.NarrowingKeyword,
			SyntaxKind.NextKeyword,
			SyntaxKind.NewKeyword,
			SyntaxKind.NotKeyword,
			SyntaxKind.NothingKeyword,
			SyntaxKind.NotInheritableKeyword,
			SyntaxKind.NotOverridableKeyword,
			SyntaxKind.ObjectKeyword,
			SyntaxKind.OfKeyword,
			SyntaxKind.OnKeyword,
			SyntaxKind.OperatorKeyword,
			SyntaxKind.OptionKeyword,
			SyntaxKind.OptionalKeyword,
			SyntaxKind.OrKeyword,
			SyntaxKind.OrElseKeyword,
			SyntaxKind.OverloadsKeyword,
			SyntaxKind.OverridableKeyword,
			SyntaxKind.OverridesKeyword,
			SyntaxKind.ParamArrayKeyword,
			SyntaxKind.PartialKeyword,
			SyntaxKind.PrivateKeyword,
			SyntaxKind.PropertyKeyword,
			SyntaxKind.ProtectedKeyword,
			SyntaxKind.PublicKeyword,
			SyntaxKind.RaiseEventKeyword,
			SyntaxKind.ReadOnlyKeyword,
			SyntaxKind.ReDimKeyword,
			SyntaxKind.REMKeyword,
			SyntaxKind.RemoveHandlerKeyword,
			SyntaxKind.ResumeKeyword,
			SyntaxKind.ReturnKeyword,
			SyntaxKind.SByteKeyword,
			SyntaxKind.SelectKeyword,
			SyntaxKind.SetKeyword,
			SyntaxKind.ShadowsKeyword,
			SyntaxKind.SharedKeyword,
			SyntaxKind.ShortKeyword,
			SyntaxKind.SingleKeyword,
			SyntaxKind.StaticKeyword,
			SyntaxKind.StepKeyword,
			SyntaxKind.StopKeyword,
			SyntaxKind.StringKeyword,
			SyntaxKind.StructureKeyword,
			SyntaxKind.SubKeyword,
			SyntaxKind.SyncLockKeyword,
			SyntaxKind.ThenKeyword,
			SyntaxKind.ThrowKeyword,
			SyntaxKind.ToKeyword,
			SyntaxKind.TrueKeyword,
			SyntaxKind.TryKeyword,
			SyntaxKind.TryCastKeyword,
			SyntaxKind.TypeOfKeyword,
			SyntaxKind.UIntegerKeyword,
			SyntaxKind.ULongKeyword,
			SyntaxKind.UShortKeyword,
			SyntaxKind.UsingKeyword,
			SyntaxKind.WhenKeyword,
			SyntaxKind.WhileKeyword,
			SyntaxKind.WideningKeyword,
			SyntaxKind.WithKeyword,
			SyntaxKind.WithEventsKeyword,
			SyntaxKind.WriteOnlyKeyword,
			SyntaxKind.XorKeyword,
			SyntaxKind.EndIfKeyword,
			SyntaxKind.GosubKeyword,
			SyntaxKind.VariantKeyword,
			SyntaxKind.WendKeyword
		};

		private static readonly SyntaxKind[] s_contextualKeywords = new SyntaxKind[47]
		{
			SyntaxKind.AggregateKeyword,
			SyntaxKind.AllKeyword,
			SyntaxKind.AnsiKeyword,
			SyntaxKind.AscendingKeyword,
			SyntaxKind.AssemblyKeyword,
			SyntaxKind.AutoKeyword,
			SyntaxKind.BinaryKeyword,
			SyntaxKind.ByKeyword,
			SyntaxKind.CompareKeyword,
			SyntaxKind.CustomKeyword,
			SyntaxKind.DescendingKeyword,
			SyntaxKind.DisableKeyword,
			SyntaxKind.DistinctKeyword,
			SyntaxKind.EnableKeyword,
			SyntaxKind.EqualsKeyword,
			SyntaxKind.ExplicitKeyword,
			SyntaxKind.ExternalSourceKeyword,
			SyntaxKind.ExternalChecksumKeyword,
			SyntaxKind.FromKeyword,
			SyntaxKind.GroupKeyword,
			SyntaxKind.InferKeyword,
			SyntaxKind.IntoKeyword,
			SyntaxKind.IsFalseKeyword,
			SyntaxKind.IsTrueKeyword,
			SyntaxKind.JoinKeyword,
			SyntaxKind.KeyKeyword,
			SyntaxKind.MidKeyword,
			SyntaxKind.OffKeyword,
			SyntaxKind.OrderKeyword,
			SyntaxKind.OutKeyword,
			SyntaxKind.PreserveKeyword,
			SyntaxKind.RegionKeyword,
			SyntaxKind.ReferenceKeyword,
			SyntaxKind.SkipKeyword,
			SyntaxKind.StrictKeyword,
			SyntaxKind.TakeKeyword,
			SyntaxKind.TextKeyword,
			SyntaxKind.UnicodeKeyword,
			SyntaxKind.UntilKeyword,
			SyntaxKind.WarningKeyword,
			SyntaxKind.WhereKeyword,
			SyntaxKind.TypeKeyword,
			SyntaxKind.XmlKeyword,
			SyntaxKind.AsyncKeyword,
			SyntaxKind.AwaitKeyword,
			SyntaxKind.IteratorKeyword,
			SyntaxKind.YieldKeyword
		};

		private static readonly SyntaxKind[] s_punctuationKinds = new SyntaxKind[42]
		{
			SyntaxKind.ExclamationToken,
			SyntaxKind.AtToken,
			SyntaxKind.CommaToken,
			SyntaxKind.HashToken,
			SyntaxKind.AmpersandToken,
			SyntaxKind.SingleQuoteToken,
			SyntaxKind.OpenParenToken,
			SyntaxKind.CloseParenToken,
			SyntaxKind.OpenBraceToken,
			SyntaxKind.CloseBraceToken,
			SyntaxKind.SemicolonToken,
			SyntaxKind.AsteriskToken,
			SyntaxKind.PlusToken,
			SyntaxKind.MinusToken,
			SyntaxKind.DotToken,
			SyntaxKind.SlashToken,
			SyntaxKind.ColonToken,
			SyntaxKind.LessThanToken,
			SyntaxKind.LessThanEqualsToken,
			SyntaxKind.LessThanGreaterThanToken,
			SyntaxKind.EqualsToken,
			SyntaxKind.GreaterThanToken,
			SyntaxKind.GreaterThanEqualsToken,
			SyntaxKind.BackslashToken,
			SyntaxKind.CaretToken,
			SyntaxKind.ColonEqualsToken,
			SyntaxKind.AmpersandEqualsToken,
			SyntaxKind.AsteriskEqualsToken,
			SyntaxKind.PlusEqualsToken,
			SyntaxKind.MinusEqualsToken,
			SyntaxKind.SlashEqualsToken,
			SyntaxKind.BackslashEqualsToken,
			SyntaxKind.CaretEqualsToken,
			SyntaxKind.LessThanLessThanToken,
			SyntaxKind.GreaterThanGreaterThanToken,
			SyntaxKind.LessThanLessThanEqualsToken,
			SyntaxKind.GreaterThanGreaterThanEqualsToken,
			SyntaxKind.QuestionToken,
			SyntaxKind.DoubleQuoteToken,
			SyntaxKind.StatementTerminatorToken,
			SyntaxKind.EndOfFileToken,
			SyntaxKind.EmptyToken
		};

		private static readonly SyntaxKind[] s_preprocessorKeywords = new SyntaxKind[14]
		{
			SyntaxKind.IfKeyword,
			SyntaxKind.ThenKeyword,
			SyntaxKind.ElseIfKeyword,
			SyntaxKind.ElseKeyword,
			SyntaxKind.EndIfKeyword,
			SyntaxKind.EndKeyword,
			SyntaxKind.RegionKeyword,
			SyntaxKind.ConstKeyword,
			SyntaxKind.ReferenceKeyword,
			SyntaxKind.EnableKeyword,
			SyntaxKind.DisableKeyword,
			SyntaxKind.WarningKeyword,
			SyntaxKind.ExternalSourceKeyword,
			SyntaxKind.ExternalChecksumKeyword
		};

		private static readonly Dictionary<string, SyntaxKind> s_contextualKeywordToSyntaxKindMap = new Dictionary<string, SyntaxKind>(CaseInsensitiveComparison.Comparer)
		{
			{
				"aggregate",
				SyntaxKind.AggregateKeyword
			},
			{
				"all",
				SyntaxKind.AllKeyword
			},
			{
				"ansi",
				SyntaxKind.AnsiKeyword
			},
			{
				"ascending",
				SyntaxKind.AscendingKeyword
			},
			{
				"assembly",
				SyntaxKind.AssemblyKeyword
			},
			{
				"auto",
				SyntaxKind.AutoKeyword
			},
			{
				"binary",
				SyntaxKind.BinaryKeyword
			},
			{
				"by",
				SyntaxKind.ByKeyword
			},
			{
				"compare",
				SyntaxKind.CompareKeyword
			},
			{
				"custom",
				SyntaxKind.CustomKeyword
			},
			{
				"descending",
				SyntaxKind.DescendingKeyword
			},
			{
				"disable",
				SyntaxKind.DisableKeyword
			},
			{
				"distinct",
				SyntaxKind.DistinctKeyword
			},
			{
				"enable",
				SyntaxKind.EnableKeyword
			},
			{
				"equals",
				SyntaxKind.EqualsKeyword
			},
			{
				"explicit",
				SyntaxKind.ExplicitKeyword
			},
			{
				"externalsource",
				SyntaxKind.ExternalSourceKeyword
			},
			{
				"externalchecksum",
				SyntaxKind.ExternalChecksumKeyword
			},
			{
				"from",
				SyntaxKind.FromKeyword
			},
			{
				"group",
				SyntaxKind.GroupKeyword
			},
			{
				"infer",
				SyntaxKind.InferKeyword
			},
			{
				"into",
				SyntaxKind.IntoKeyword
			},
			{
				"isfalse",
				SyntaxKind.IsFalseKeyword
			},
			{
				"istrue",
				SyntaxKind.IsTrueKeyword
			},
			{
				"join",
				SyntaxKind.JoinKeyword
			},
			{
				"key",
				SyntaxKind.KeyKeyword
			},
			{
				"mid",
				SyntaxKind.MidKeyword
			},
			{
				"off",
				SyntaxKind.OffKeyword
			},
			{
				"order",
				SyntaxKind.OrderKeyword
			},
			{
				"out",
				SyntaxKind.OutKeyword
			},
			{
				"preserve",
				SyntaxKind.PreserveKeyword
			},
			{
				"region",
				SyntaxKind.RegionKeyword
			},
			{
				"r",
				SyntaxKind.ReferenceKeyword
			},
			{
				"skip",
				SyntaxKind.SkipKeyword
			},
			{
				"strict",
				SyntaxKind.StrictKeyword
			},
			{
				"take",
				SyntaxKind.TakeKeyword
			},
			{
				"text",
				SyntaxKind.TextKeyword
			},
			{
				"unicode",
				SyntaxKind.UnicodeKeyword
			},
			{
				"until",
				SyntaxKind.UntilKeyword
			},
			{
				"warning",
				SyntaxKind.WarningKeyword
			},
			{
				"where",
				SyntaxKind.WhereKeyword
			},
			{
				"type",
				SyntaxKind.TypeKeyword
			},
			{
				"xml",
				SyntaxKind.XmlKeyword
			},
			{
				"async",
				SyntaxKind.AsyncKeyword
			},
			{
				"await",
				SyntaxKind.AwaitKeyword
			},
			{
				"iterator",
				SyntaxKind.IteratorKeyword
			},
			{
				"yield",
				SyntaxKind.YieldKeyword
			}
		};

		private static readonly Dictionary<string, SyntaxKind> s_preprocessorKeywordToSyntaxKindMap = new Dictionary<string, SyntaxKind>(CaseInsensitiveComparison.Comparer)
		{
			{
				"if",
				SyntaxKind.IfKeyword
			},
			{
				"elseif",
				SyntaxKind.ElseIfKeyword
			},
			{
				"else",
				SyntaxKind.ElseKeyword
			},
			{
				"endif",
				SyntaxKind.EndIfKeyword
			},
			{
				"region",
				SyntaxKind.RegionKeyword
			},
			{
				"end",
				SyntaxKind.EndKeyword
			},
			{
				"const",
				SyntaxKind.ConstKeyword
			},
			{
				"externalsource",
				SyntaxKind.ExternalSourceKeyword
			},
			{
				"externalchecksum",
				SyntaxKind.ExternalChecksumKeyword
			},
			{
				"r",
				SyntaxKind.ReferenceKeyword
			},
			{
				"enable",
				SyntaxKind.EnableKeyword
			},
			{
				"disable",
				SyntaxKind.DisableKeyword
			}
		};

		public static IEqualityComparer<SyntaxKind> EqualityComparer { get; } = new SyntaxKindEqualityComparer();


		public static bool IsEndBlockStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 5 <= SyntaxKind.EndAddHandlerStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsEndBlockStatementBlockKeyword(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.AddHandlerKeyword:
			case SyntaxKind.ClassKeyword:
			case SyntaxKind.EnumKeyword:
			case SyntaxKind.EventKeyword:
			case SyntaxKind.FunctionKeyword:
			case SyntaxKind.GetKeyword:
			case SyntaxKind.IfKeyword:
			case SyntaxKind.InterfaceKeyword:
			case SyntaxKind.ModuleKeyword:
			case SyntaxKind.NamespaceKeyword:
			case SyntaxKind.OperatorKeyword:
			case SyntaxKind.PropertyKeyword:
			case SyntaxKind.RaiseEventKeyword:
			case SyntaxKind.RemoveHandlerKeyword:
			case SyntaxKind.SelectKeyword:
			case SyntaxKind.SetKeyword:
			case SyntaxKind.StructureKeyword:
			case SyntaxKind.SubKeyword:
			case SyntaxKind.SyncLockKeyword:
			case SyntaxKind.TryKeyword:
			case SyntaxKind.UsingKeyword:
			case SyntaxKind.WhileKeyword:
			case SyntaxKind.WithKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsOptionStatementNameKeyword(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.CompareKeyword:
			case SyntaxKind.ExplicitKeyword:
			case SyntaxKind.InferKeyword:
			case SyntaxKind.StrictKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsOptionStatementValueKeyword(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.OnKeyword:
			case SyntaxKind.BinaryKeyword:
			case SyntaxKind.OffKeyword:
			case SyntaxKind.TextKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsTypeParameterVarianceKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.InKeyword || syntaxKind == SyntaxKind.OutKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsSpecialConstraint(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 72 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsSpecialConstraintConstraintKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.ClassKeyword || syntaxKind == SyntaxKind.NewKeyword || syntaxKind == SyntaxKind.StructureKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsMethodBlock(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 79 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsAccessorBlock(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 83 <= (SyntaxKind)4)
			{
				return true;
			}
			return false;
		}

		public static bool IsMethodStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 93 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsMethodStatementSubOrFunctionKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.FunctionKeyword || syntaxKind == SyntaxKind.SubKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsDeclareStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 96 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsDeclareStatementCharsetKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.AnsiKeyword || syntaxKind == SyntaxKind.AutoKeyword || syntaxKind == SyntaxKind.UnicodeKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsDeclareStatementSubOrFunctionKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.FunctionKeyword || syntaxKind == SyntaxKind.SubKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsDelegateStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 98 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsDelegateStatementSubOrFunctionKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.FunctionKeyword || syntaxKind == SyntaxKind.SubKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsOperatorStatementOperatorToken(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.AndKeyword:
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.LikeKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.NotKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.IsFalseKeyword:
			case SyntaxKind.IsTrueKeyword:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
				return true;
			default:
				return false;
			}
		}

		public static bool IsAccessorStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 105 <= (SyntaxKind)3 || syntaxKind == SyntaxKind.RaiseEventAccessorStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsAccessorStatementAccessorKeyword(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.AddHandlerKeyword:
			case SyntaxKind.GetKeyword:
			case SyntaxKind.RaiseEventKeyword:
			case SyntaxKind.RemoveHandlerKeyword:
			case SyntaxKind.SetKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsKeywordEventContainerKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.MeKeyword || syntaxKind - 506 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsAttributeTargetAttributeModifier(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.ModuleKeyword || syntaxKind == SyntaxKind.AssemblyKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsLabelStatementLabelToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 700 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsLabel(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 150 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsLabelLabelToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.NextKeyword || syntaxKind - 700 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsStopOrEndStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.StopStatement || syntaxKind == SyntaxKind.EndStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsStopOrEndStatementStopOrEndKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.EndKeyword || syntaxKind == SyntaxKind.StopKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsExitStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 157 <= SyntaxKind.EndSelectStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsExitStatementBlockKeyword(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.DoKeyword:
			case SyntaxKind.ForKeyword:
			case SyntaxKind.FunctionKeyword:
			case SyntaxKind.OperatorKeyword:
			case SyntaxKind.PropertyKeyword:
			case SyntaxKind.SelectKeyword:
			case SyntaxKind.SubKeyword:
			case SyntaxKind.TryKeyword:
			case SyntaxKind.WhileKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsContinueStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 166 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsContinueStatementBlockKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.DoKeyword || syntaxKind == SyntaxKind.ForKeyword || syntaxKind == SyntaxKind.WhileKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsOnErrorGoToStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 196 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsResumeStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 200 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsCaseBlock(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.CaseBlock || syntaxKind == SyntaxKind.CaseElseBlock)
			{
				return true;
			}
			return false;
		}

		public static bool IsCaseStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 211 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsRelationalCaseClause(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 216 <= (SyntaxKind)3 || syntaxKind - 222 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsRelationalCaseClauseOperatorToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 653 <= SyntaxKind.EndIfStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsDoLoopBlock(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 756 <= (SyntaxKind)4)
			{
				return true;
			}
			return false;
		}

		public static bool IsDoStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 770 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsLoopStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 773 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsWhileOrUntilClause(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 776 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsWhileOrUntilClauseWhileOrUntilKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.WhileKeyword || syntaxKind == SyntaxKind.UntilKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsAssignmentStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 247 <= SyntaxKind.EndSelectStatement || syntaxKind - 258 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsAssignmentStatementOperatorToken(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.EqualsToken:
			case SyntaxKind.AmpersandEqualsToken:
			case SyntaxKind.AsteriskEqualsToken:
			case SyntaxKind.PlusEqualsToken:
			case SyntaxKind.MinusEqualsToken:
			case SyntaxKind.SlashEqualsToken:
			case SyntaxKind.BackslashEqualsToken:
			case SyntaxKind.CaretEqualsToken:
			case SyntaxKind.LessThanLessThanEqualsToken:
			case SyntaxKind.GreaterThanGreaterThanEqualsToken:
				return true;
			default:
				return false;
			}
		}

		public static bool IsAddRemoveHandlerStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 262 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsAddRemoveHandlerStatementAddHandlerOrRemoveHandlerKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.AddHandlerKeyword || syntaxKind == SyntaxKind.RemoveHandlerKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsReDimStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 266 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsLiteralExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 272 <= (SyntaxKind)4 || syntaxKind - 279 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsLiteralExpressionToken(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.FalseKeyword:
			case SyntaxKind.NothingKeyword:
			case SyntaxKind.TrueKeyword:
			case SyntaxKind.IntegerLiteralToken:
			case SyntaxKind.FloatingLiteralToken:
			case SyntaxKind.DecimalLiteralToken:
			case SyntaxKind.DateLiteralToken:
			case SyntaxKind.StringLiteralToken:
			case SyntaxKind.CharacterLiteralToken:
				return true;
			default:
				return false;
			}
		}

		public static bool IsTypeOfExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 286 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsTypeOfExpressionOperatorToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.IsKeyword || syntaxKind == SyntaxKind.IsNotKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsMemberAccessExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 291 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsMemberAccessExpressionOperatorToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.ExclamationToken || syntaxKind == SyntaxKind.DotToken)
			{
				return true;
			}
			return false;
		}

		public static bool IsXmlMemberAccessExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 293 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsXmlMemberAccessExpressionToken2(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.AtToken || syntaxKind == SyntaxKind.DotToken)
			{
				return true;
			}
			return false;
		}

		public static bool IsPredefinedCastExpressionKeyword(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.CBoolKeyword:
			case SyntaxKind.CByteKeyword:
			case SyntaxKind.CCharKeyword:
			case SyntaxKind.CDateKeyword:
			case SyntaxKind.CDecKeyword:
			case SyntaxKind.CDblKeyword:
			case SyntaxKind.CIntKeyword:
			case SyntaxKind.CLngKeyword:
			case SyntaxKind.CObjKeyword:
			case SyntaxKind.CSByteKeyword:
			case SyntaxKind.CShortKeyword:
			case SyntaxKind.CSngKeyword:
			case SyntaxKind.CStrKeyword:
			case SyntaxKind.CUIntKeyword:
			case SyntaxKind.CULngKeyword:
			case SyntaxKind.CUShortKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsBinaryExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 307 <= (SyntaxKind)4 || syntaxKind - 314 <= SyntaxKind.EndSetStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsBinaryExpressionOperatorToken(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.AndKeyword:
			case SyntaxKind.AndAlsoKeyword:
			case SyntaxKind.IsKeyword:
			case SyntaxKind.IsNotKeyword:
			case SyntaxKind.LikeKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.OrElseKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
				return true;
			default:
				return false;
			}
		}

		public static bool IsUnaryExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 333 <= (SyntaxKind)3)
			{
				return true;
			}
			return false;
		}

		public static bool IsUnaryExpressionOperatorToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.AddressOfKeyword || syntaxKind == SyntaxKind.NotKeyword || syntaxKind - 648 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsSingleLineLambdaExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SingleLineFunctionLambdaExpression || syntaxKind == SyntaxKind.SingleLineSubLambdaExpression)
			{
				return true;
			}
			return false;
		}

		public static bool IsMultiLineLambdaExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 343 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsLambdaHeader(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 345 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsLambdaHeaderSubOrFunctionKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.FunctionKeyword || syntaxKind == SyntaxKind.SubKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsPartitionWhileClause(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 364 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsPartitionWhileClauseSkipOrTakeKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SkipKeyword || syntaxKind == SyntaxKind.TakeKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsPartitionClause(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 366 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsPartitionClauseSkipOrTakeKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SkipKeyword || syntaxKind == SyntaxKind.TakeKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsOrdering(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 375 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsOrderingAscendingOrDescendingKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.AscendingKeyword || syntaxKind == SyntaxKind.DescendingKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsXmlStringStartQuoteToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SingleQuoteToken || syntaxKind == SyntaxKind.DoubleQuoteToken)
			{
				return true;
			}
			return false;
		}

		public static bool IsXmlStringEndQuoteToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SingleQuoteToken || syntaxKind == SyntaxKind.DoubleQuoteToken)
			{
				return true;
			}
			return false;
		}

		internal static bool IsPredefinedTypeKeyword(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.BooleanKeyword:
			case SyntaxKind.ByteKeyword:
			case SyntaxKind.CharKeyword:
			case SyntaxKind.DateKeyword:
			case SyntaxKind.DecimalKeyword:
			case SyntaxKind.DoubleKeyword:
			case SyntaxKind.IntegerKeyword:
			case SyntaxKind.LongKeyword:
			case SyntaxKind.ObjectKeyword:
			case SyntaxKind.SByteKeyword:
			case SyntaxKind.ShortKeyword:
			case SyntaxKind.SingleKeyword:
			case SyntaxKind.StringKeyword:
			case SyntaxKind.UIntegerKeyword:
			case SyntaxKind.ULongKeyword:
			case SyntaxKind.UShortKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsCrefSignaturePartModifier(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.ByRefKeyword || syntaxKind == SyntaxKind.ByValKeyword)
			{
				return true;
			}
			return false;
		}

		public static bool IsCrefOperatorReferenceOperatorToken(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.AndKeyword:
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.LikeKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.NotKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.IsFalseKeyword:
			case SyntaxKind.IsTrueKeyword:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
				return true;
			default:
				return false;
			}
		}

		public static bool IsKeywordKind(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.AddHandlerKeyword:
			case SyntaxKind.AddressOfKeyword:
			case SyntaxKind.AliasKeyword:
			case SyntaxKind.AndKeyword:
			case SyntaxKind.AndAlsoKeyword:
			case SyntaxKind.AsKeyword:
			case SyntaxKind.BooleanKeyword:
			case SyntaxKind.ByRefKeyword:
			case SyntaxKind.ByteKeyword:
			case SyntaxKind.ByValKeyword:
			case SyntaxKind.CallKeyword:
			case SyntaxKind.CaseKeyword:
			case SyntaxKind.CatchKeyword:
			case SyntaxKind.CBoolKeyword:
			case SyntaxKind.CByteKeyword:
			case SyntaxKind.CCharKeyword:
			case SyntaxKind.CDateKeyword:
			case SyntaxKind.CDecKeyword:
			case SyntaxKind.CDblKeyword:
			case SyntaxKind.CharKeyword:
			case SyntaxKind.CIntKeyword:
			case SyntaxKind.ClassKeyword:
			case SyntaxKind.CLngKeyword:
			case SyntaxKind.CObjKeyword:
			case SyntaxKind.ConstKeyword:
			case SyntaxKind.ReferenceKeyword:
			case SyntaxKind.ContinueKeyword:
			case SyntaxKind.CSByteKeyword:
			case SyntaxKind.CShortKeyword:
			case SyntaxKind.CSngKeyword:
			case SyntaxKind.CStrKeyword:
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.CUIntKeyword:
			case SyntaxKind.CULngKeyword:
			case SyntaxKind.CUShortKeyword:
			case SyntaxKind.DateKeyword:
			case SyntaxKind.DecimalKeyword:
			case SyntaxKind.DeclareKeyword:
			case SyntaxKind.DefaultKeyword:
			case SyntaxKind.DelegateKeyword:
			case SyntaxKind.DimKeyword:
			case SyntaxKind.DirectCastKeyword:
			case SyntaxKind.DoKeyword:
			case SyntaxKind.DoubleKeyword:
			case SyntaxKind.EachKeyword:
			case SyntaxKind.ElseKeyword:
			case SyntaxKind.ElseIfKeyword:
			case SyntaxKind.EndKeyword:
			case SyntaxKind.EnumKeyword:
			case SyntaxKind.EraseKeyword:
			case SyntaxKind.ErrorKeyword:
			case SyntaxKind.EventKeyword:
			case SyntaxKind.ExitKeyword:
			case SyntaxKind.FalseKeyword:
			case SyntaxKind.FinallyKeyword:
			case SyntaxKind.ForKeyword:
			case SyntaxKind.FriendKeyword:
			case SyntaxKind.FunctionKeyword:
			case SyntaxKind.GetKeyword:
			case SyntaxKind.GetTypeKeyword:
			case SyntaxKind.GetXmlNamespaceKeyword:
			case SyntaxKind.GlobalKeyword:
			case SyntaxKind.GoToKeyword:
			case SyntaxKind.HandlesKeyword:
			case SyntaxKind.IfKeyword:
			case SyntaxKind.ImplementsKeyword:
			case SyntaxKind.ImportsKeyword:
			case SyntaxKind.InKeyword:
			case SyntaxKind.InheritsKeyword:
			case SyntaxKind.IntegerKeyword:
			case SyntaxKind.InterfaceKeyword:
			case SyntaxKind.IsKeyword:
			case SyntaxKind.IsNotKeyword:
			case SyntaxKind.LetKeyword:
			case SyntaxKind.LibKeyword:
			case SyntaxKind.LikeKeyword:
			case SyntaxKind.LongKeyword:
			case SyntaxKind.LoopKeyword:
			case SyntaxKind.MeKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.ModuleKeyword:
			case SyntaxKind.MustInheritKeyword:
			case SyntaxKind.MustOverrideKeyword:
			case SyntaxKind.MyBaseKeyword:
			case SyntaxKind.MyClassKeyword:
			case SyntaxKind.NamespaceKeyword:
			case SyntaxKind.NarrowingKeyword:
			case SyntaxKind.NextKeyword:
			case SyntaxKind.NewKeyword:
			case SyntaxKind.NotKeyword:
			case SyntaxKind.NothingKeyword:
			case SyntaxKind.NotInheritableKeyword:
			case SyntaxKind.NotOverridableKeyword:
			case SyntaxKind.ObjectKeyword:
			case SyntaxKind.OfKeyword:
			case SyntaxKind.OnKeyword:
			case SyntaxKind.OperatorKeyword:
			case SyntaxKind.OptionKeyword:
			case SyntaxKind.OptionalKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.OrElseKeyword:
			case SyntaxKind.OverloadsKeyword:
			case SyntaxKind.OverridableKeyword:
			case SyntaxKind.OverridesKeyword:
			case SyntaxKind.ParamArrayKeyword:
			case SyntaxKind.PartialKeyword:
			case SyntaxKind.PrivateKeyword:
			case SyntaxKind.PropertyKeyword:
			case SyntaxKind.ProtectedKeyword:
			case SyntaxKind.PublicKeyword:
			case SyntaxKind.RaiseEventKeyword:
			case SyntaxKind.ReadOnlyKeyword:
			case SyntaxKind.ReDimKeyword:
			case SyntaxKind.REMKeyword:
			case SyntaxKind.RemoveHandlerKeyword:
			case SyntaxKind.ResumeKeyword:
			case SyntaxKind.ReturnKeyword:
			case SyntaxKind.SByteKeyword:
			case SyntaxKind.SelectKeyword:
			case SyntaxKind.SetKeyword:
			case SyntaxKind.ShadowsKeyword:
			case SyntaxKind.SharedKeyword:
			case SyntaxKind.ShortKeyword:
			case SyntaxKind.SingleKeyword:
			case SyntaxKind.StaticKeyword:
			case SyntaxKind.StepKeyword:
			case SyntaxKind.StopKeyword:
			case SyntaxKind.StringKeyword:
			case SyntaxKind.StructureKeyword:
			case SyntaxKind.SubKeyword:
			case SyntaxKind.SyncLockKeyword:
			case SyntaxKind.ThenKeyword:
			case SyntaxKind.ThrowKeyword:
			case SyntaxKind.ToKeyword:
			case SyntaxKind.TrueKeyword:
			case SyntaxKind.TryKeyword:
			case SyntaxKind.TryCastKeyword:
			case SyntaxKind.TypeOfKeyword:
			case SyntaxKind.UIntegerKeyword:
			case SyntaxKind.ULongKeyword:
			case SyntaxKind.UShortKeyword:
			case SyntaxKind.UsingKeyword:
			case SyntaxKind.WhenKeyword:
			case SyntaxKind.WhileKeyword:
			case SyntaxKind.WideningKeyword:
			case SyntaxKind.WithKeyword:
			case SyntaxKind.WithEventsKeyword:
			case SyntaxKind.WriteOnlyKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.EndIfKeyword:
			case SyntaxKind.GosubKeyword:
			case SyntaxKind.VariantKeyword:
			case SyntaxKind.WendKeyword:
			case SyntaxKind.AggregateKeyword:
			case SyntaxKind.AllKeyword:
			case SyntaxKind.AnsiKeyword:
			case SyntaxKind.AscendingKeyword:
			case SyntaxKind.AssemblyKeyword:
			case SyntaxKind.AutoKeyword:
			case SyntaxKind.BinaryKeyword:
			case SyntaxKind.ByKeyword:
			case SyntaxKind.CompareKeyword:
			case SyntaxKind.CustomKeyword:
			case SyntaxKind.DescendingKeyword:
			case SyntaxKind.DisableKeyword:
			case SyntaxKind.DistinctKeyword:
			case SyntaxKind.EnableKeyword:
			case SyntaxKind.EqualsKeyword:
			case SyntaxKind.ExplicitKeyword:
			case SyntaxKind.ExternalSourceKeyword:
			case SyntaxKind.ExternalChecksumKeyword:
			case SyntaxKind.FromKeyword:
			case SyntaxKind.GroupKeyword:
			case SyntaxKind.InferKeyword:
			case SyntaxKind.IntoKeyword:
			case SyntaxKind.IsFalseKeyword:
			case SyntaxKind.IsTrueKeyword:
			case SyntaxKind.JoinKeyword:
			case SyntaxKind.KeyKeyword:
			case SyntaxKind.MidKeyword:
			case SyntaxKind.OffKeyword:
			case SyntaxKind.OrderKeyword:
			case SyntaxKind.OutKeyword:
			case SyntaxKind.PreserveKeyword:
			case SyntaxKind.RegionKeyword:
			case SyntaxKind.SkipKeyword:
			case SyntaxKind.StrictKeyword:
			case SyntaxKind.TakeKeyword:
			case SyntaxKind.TextKeyword:
			case SyntaxKind.UnicodeKeyword:
			case SyntaxKind.UntilKeyword:
			case SyntaxKind.WarningKeyword:
			case SyntaxKind.WhereKeyword:
			case SyntaxKind.TypeKeyword:
			case SyntaxKind.XmlKeyword:
			case SyntaxKind.AsyncKeyword:
			case SyntaxKind.AwaitKeyword:
			case SyntaxKind.IteratorKeyword:
			case SyntaxKind.YieldKeyword:
			case SyntaxKind.NameOfKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsPunctuation(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.ExclamationToken:
			case SyntaxKind.AtToken:
			case SyntaxKind.CommaToken:
			case SyntaxKind.HashToken:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.SingleQuoteToken:
			case SyntaxKind.OpenParenToken:
			case SyntaxKind.CloseParenToken:
			case SyntaxKind.OpenBraceToken:
			case SyntaxKind.CloseBraceToken:
			case SyntaxKind.SemicolonToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.DotToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.ColonToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.ColonEqualsToken:
			case SyntaxKind.AmpersandEqualsToken:
			case SyntaxKind.AsteriskEqualsToken:
			case SyntaxKind.PlusEqualsToken:
			case SyntaxKind.MinusEqualsToken:
			case SyntaxKind.SlashEqualsToken:
			case SyntaxKind.BackslashEqualsToken:
			case SyntaxKind.CaretEqualsToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
			case SyntaxKind.LessThanLessThanEqualsToken:
			case SyntaxKind.GreaterThanGreaterThanEqualsToken:
			case SyntaxKind.QuestionToken:
			case SyntaxKind.DoubleQuoteToken:
			case SyntaxKind.StatementTerminatorToken:
			case SyntaxKind.EndOfFileToken:
			case SyntaxKind.EmptyToken:
			case SyntaxKind.SlashGreaterThanToken:
			case SyntaxKind.LessThanSlashToken:
			case SyntaxKind.LessThanExclamationMinusMinusToken:
			case SyntaxKind.MinusMinusGreaterThanToken:
			case SyntaxKind.LessThanQuestionToken:
			case SyntaxKind.QuestionGreaterThanToken:
			case SyntaxKind.LessThanPercentEqualsToken:
			case SyntaxKind.PercentGreaterThanToken:
			case SyntaxKind.BeginCDataToken:
			case SyntaxKind.EndCDataToken:
			case SyntaxKind.EndOfXmlToken:
			case SyntaxKind.DollarSignDoubleQuoteToken:
			case SyntaxKind.EndOfInterpolatedStringToken:
				return true;
			default:
				return false;
			}
		}

		public static bool IsXmlTextToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 695 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsXmlCrefAttributeStartQuoteToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SingleQuoteToken || syntaxKind == SyntaxKind.DoubleQuoteToken)
			{
				return true;
			}
			return false;
		}

		public static bool IsXmlCrefAttributeEndQuoteToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SingleQuoteToken || syntaxKind == SyntaxKind.DoubleQuoteToken)
			{
				return true;
			}
			return false;
		}

		public static bool IsXmlNameAttributeStartQuoteToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SingleQuoteToken || syntaxKind == SyntaxKind.DoubleQuoteToken)
			{
				return true;
			}
			return false;
		}

		public static bool IsXmlNameAttributeEndQuoteToken(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.SingleQuoteToken || syntaxKind == SyntaxKind.DoubleQuoteToken)
			{
				return true;
			}
			return false;
		}

		internal static bool IsSyntaxTrivia(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 729 <= SyntaxKind.EndUsingStatement || syntaxKind == SyntaxKind.ConflictMarkerTrivia)
			{
				return true;
			}
			return false;
		}

		public static bool IsIfDirectiveTrivia(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 737 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static bool IsIfDirectiveTriviaIfOrElseIfKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.ElseIfKeyword || syntaxKind == SyntaxKind.IfKeyword)
			{
				return true;
			}
			return false;
		}

		public static string GetText(SyntaxKind kind)
		{
			return kind switch
			{
				SyntaxKind.AddHandlerKeyword => "AddHandler", 
				SyntaxKind.AddressOfKeyword => "AddressOf", 
				SyntaxKind.AliasKeyword => "Alias", 
				SyntaxKind.AndKeyword => "And", 
				SyntaxKind.AndAlsoKeyword => "AndAlso", 
				SyntaxKind.AsKeyword => "As", 
				SyntaxKind.BooleanKeyword => "Boolean", 
				SyntaxKind.ByRefKeyword => "ByRef", 
				SyntaxKind.ByteKeyword => "Byte", 
				SyntaxKind.ByValKeyword => "ByVal", 
				SyntaxKind.CallKeyword => "Call", 
				SyntaxKind.CaseKeyword => "Case", 
				SyntaxKind.CatchKeyword => "Catch", 
				SyntaxKind.CBoolKeyword => "CBool", 
				SyntaxKind.CByteKeyword => "CByte", 
				SyntaxKind.CCharKeyword => "CChar", 
				SyntaxKind.CDateKeyword => "CDate", 
				SyntaxKind.CDecKeyword => "CDec", 
				SyntaxKind.CDblKeyword => "CDbl", 
				SyntaxKind.CharKeyword => "Char", 
				SyntaxKind.CIntKeyword => "CInt", 
				SyntaxKind.ClassKeyword => "Class", 
				SyntaxKind.CLngKeyword => "CLng", 
				SyntaxKind.CObjKeyword => "CObj", 
				SyntaxKind.ConstKeyword => "Const", 
				SyntaxKind.ReferenceKeyword => "R", 
				SyntaxKind.ContinueKeyword => "Continue", 
				SyntaxKind.CSByteKeyword => "CSByte", 
				SyntaxKind.CShortKeyword => "CShort", 
				SyntaxKind.CSngKeyword => "CSng", 
				SyntaxKind.CStrKeyword => "CStr", 
				SyntaxKind.CTypeKeyword => "CType", 
				SyntaxKind.CUIntKeyword => "CUInt", 
				SyntaxKind.CULngKeyword => "CULng", 
				SyntaxKind.CUShortKeyword => "CUShort", 
				SyntaxKind.DateKeyword => "Date", 
				SyntaxKind.DecimalKeyword => "Decimal", 
				SyntaxKind.DeclareKeyword => "Declare", 
				SyntaxKind.DefaultKeyword => "Default", 
				SyntaxKind.DelegateKeyword => "Delegate", 
				SyntaxKind.DimKeyword => "Dim", 
				SyntaxKind.DirectCastKeyword => "DirectCast", 
				SyntaxKind.DoKeyword => "Do", 
				SyntaxKind.DoubleKeyword => "Double", 
				SyntaxKind.EachKeyword => "Each", 
				SyntaxKind.ElseKeyword => "Else", 
				SyntaxKind.ElseIfKeyword => "ElseIf", 
				SyntaxKind.EndKeyword => "End", 
				SyntaxKind.EnumKeyword => "Enum", 
				SyntaxKind.EraseKeyword => "Erase", 
				SyntaxKind.ErrorKeyword => "Error", 
				SyntaxKind.EventKeyword => "Event", 
				SyntaxKind.ExitKeyword => "Exit", 
				SyntaxKind.FalseKeyword => "False", 
				SyntaxKind.FinallyKeyword => "Finally", 
				SyntaxKind.ForKeyword => "For", 
				SyntaxKind.FriendKeyword => "Friend", 
				SyntaxKind.FunctionKeyword => "Function", 
				SyntaxKind.GetKeyword => "Get", 
				SyntaxKind.GetTypeKeyword => "GetType", 
				SyntaxKind.GetXmlNamespaceKeyword => "GetXmlNamespace", 
				SyntaxKind.GlobalKeyword => "Global", 
				SyntaxKind.GoToKeyword => "GoTo", 
				SyntaxKind.HandlesKeyword => "Handles", 
				SyntaxKind.IfKeyword => "If", 
				SyntaxKind.ImplementsKeyword => "Implements", 
				SyntaxKind.ImportsKeyword => "Imports", 
				SyntaxKind.InKeyword => "In", 
				SyntaxKind.InheritsKeyword => "Inherits", 
				SyntaxKind.IntegerKeyword => "Integer", 
				SyntaxKind.InterfaceKeyword => "Interface", 
				SyntaxKind.IsKeyword => "Is", 
				SyntaxKind.IsNotKeyword => "IsNot", 
				SyntaxKind.LetKeyword => "Let", 
				SyntaxKind.LibKeyword => "Lib", 
				SyntaxKind.LikeKeyword => "Like", 
				SyntaxKind.LongKeyword => "Long", 
				SyntaxKind.LoopKeyword => "Loop", 
				SyntaxKind.MeKeyword => "Me", 
				SyntaxKind.ModKeyword => "Mod", 
				SyntaxKind.ModuleKeyword => "Module", 
				SyntaxKind.MustInheritKeyword => "MustInherit", 
				SyntaxKind.MustOverrideKeyword => "MustOverride", 
				SyntaxKind.MyBaseKeyword => "MyBase", 
				SyntaxKind.MyClassKeyword => "MyClass", 
				SyntaxKind.NameOfKeyword => "NameOf", 
				SyntaxKind.NamespaceKeyword => "Namespace", 
				SyntaxKind.NarrowingKeyword => "Narrowing", 
				SyntaxKind.NextKeyword => "Next", 
				SyntaxKind.NewKeyword => "New", 
				SyntaxKind.NotKeyword => "Not", 
				SyntaxKind.NothingKeyword => "Nothing", 
				SyntaxKind.NotInheritableKeyword => "NotInheritable", 
				SyntaxKind.NotOverridableKeyword => "NotOverridable", 
				SyntaxKind.ObjectKeyword => "Object", 
				SyntaxKind.OfKeyword => "Of", 
				SyntaxKind.OnKeyword => "On", 
				SyntaxKind.OperatorKeyword => "Operator", 
				SyntaxKind.OptionKeyword => "Option", 
				SyntaxKind.OptionalKeyword => "Optional", 
				SyntaxKind.OrKeyword => "Or", 
				SyntaxKind.OrElseKeyword => "OrElse", 
				SyntaxKind.OverloadsKeyword => "Overloads", 
				SyntaxKind.OverridableKeyword => "Overridable", 
				SyntaxKind.OverridesKeyword => "Overrides", 
				SyntaxKind.ParamArrayKeyword => "ParamArray", 
				SyntaxKind.PartialKeyword => "Partial", 
				SyntaxKind.PrivateKeyword => "Private", 
				SyntaxKind.PropertyKeyword => "Property", 
				SyntaxKind.ProtectedKeyword => "Protected", 
				SyntaxKind.PublicKeyword => "Public", 
				SyntaxKind.RaiseEventKeyword => "RaiseEvent", 
				SyntaxKind.ReadOnlyKeyword => "ReadOnly", 
				SyntaxKind.ReDimKeyword => "ReDim", 
				SyntaxKind.REMKeyword => "REM", 
				SyntaxKind.RemoveHandlerKeyword => "RemoveHandler", 
				SyntaxKind.ResumeKeyword => "Resume", 
				SyntaxKind.ReturnKeyword => "Return", 
				SyntaxKind.SByteKeyword => "SByte", 
				SyntaxKind.SelectKeyword => "Select", 
				SyntaxKind.SetKeyword => "Set", 
				SyntaxKind.ShadowsKeyword => "Shadows", 
				SyntaxKind.SharedKeyword => "Shared", 
				SyntaxKind.ShortKeyword => "Short", 
				SyntaxKind.SingleKeyword => "Single", 
				SyntaxKind.StaticKeyword => "Static", 
				SyntaxKind.StepKeyword => "Step", 
				SyntaxKind.StopKeyword => "Stop", 
				SyntaxKind.StringKeyword => "String", 
				SyntaxKind.StructureKeyword => "Structure", 
				SyntaxKind.SubKeyword => "Sub", 
				SyntaxKind.SyncLockKeyword => "SyncLock", 
				SyntaxKind.ThenKeyword => "Then", 
				SyntaxKind.ThrowKeyword => "Throw", 
				SyntaxKind.ToKeyword => "To", 
				SyntaxKind.TrueKeyword => "True", 
				SyntaxKind.TryKeyword => "Try", 
				SyntaxKind.TryCastKeyword => "TryCast", 
				SyntaxKind.TypeOfKeyword => "TypeOf", 
				SyntaxKind.UIntegerKeyword => "UInteger", 
				SyntaxKind.ULongKeyword => "ULong", 
				SyntaxKind.UShortKeyword => "UShort", 
				SyntaxKind.UsingKeyword => "Using", 
				SyntaxKind.WhenKeyword => "When", 
				SyntaxKind.WhileKeyword => "While", 
				SyntaxKind.WideningKeyword => "Widening", 
				SyntaxKind.WithKeyword => "With", 
				SyntaxKind.WithEventsKeyword => "WithEvents", 
				SyntaxKind.WriteOnlyKeyword => "WriteOnly", 
				SyntaxKind.XorKeyword => "Xor", 
				SyntaxKind.EndIfKeyword => "EndIf", 
				SyntaxKind.GosubKeyword => "Gosub", 
				SyntaxKind.VariantKeyword => "Variant", 
				SyntaxKind.WendKeyword => "Wend", 
				SyntaxKind.AggregateKeyword => "Aggregate", 
				SyntaxKind.AllKeyword => "All", 
				SyntaxKind.AnsiKeyword => "Ansi", 
				SyntaxKind.AscendingKeyword => "Ascending", 
				SyntaxKind.AssemblyKeyword => "Assembly", 
				SyntaxKind.AutoKeyword => "Auto", 
				SyntaxKind.BinaryKeyword => "Binary", 
				SyntaxKind.ByKeyword => "By", 
				SyntaxKind.CompareKeyword => "Compare", 
				SyntaxKind.CustomKeyword => "Custom", 
				SyntaxKind.DescendingKeyword => "Descending", 
				SyntaxKind.DisableKeyword => "Disable", 
				SyntaxKind.DistinctKeyword => "Distinct", 
				SyntaxKind.EnableKeyword => "Enable", 
				SyntaxKind.EqualsKeyword => "Equals", 
				SyntaxKind.ExplicitKeyword => "Explicit", 
				SyntaxKind.ExternalSourceKeyword => "ExternalSource", 
				SyntaxKind.ExternalChecksumKeyword => "ExternalChecksum", 
				SyntaxKind.FromKeyword => "From", 
				SyntaxKind.GroupKeyword => "Group", 
				SyntaxKind.InferKeyword => "Infer", 
				SyntaxKind.IntoKeyword => "Into", 
				SyntaxKind.IsFalseKeyword => "IsFalse", 
				SyntaxKind.IsTrueKeyword => "IsTrue", 
				SyntaxKind.JoinKeyword => "Join", 
				SyntaxKind.KeyKeyword => "Key", 
				SyntaxKind.MidKeyword => "Mid", 
				SyntaxKind.OffKeyword => "Off", 
				SyntaxKind.OrderKeyword => "Order", 
				SyntaxKind.OutKeyword => "Out", 
				SyntaxKind.PreserveKeyword => "Preserve", 
				SyntaxKind.RegionKeyword => "Region", 
				SyntaxKind.SkipKeyword => "Skip", 
				SyntaxKind.StrictKeyword => "Strict", 
				SyntaxKind.TakeKeyword => "Take", 
				SyntaxKind.TextKeyword => "Text", 
				SyntaxKind.UnicodeKeyword => "Unicode", 
				SyntaxKind.UntilKeyword => "Until", 
				SyntaxKind.WarningKeyword => "Warning", 
				SyntaxKind.WhereKeyword => "Where", 
				SyntaxKind.TypeKeyword => "Type", 
				SyntaxKind.XmlKeyword => "xml", 
				SyntaxKind.AsyncKeyword => "Async", 
				SyntaxKind.AwaitKeyword => "Await", 
				SyntaxKind.IteratorKeyword => "Iterator", 
				SyntaxKind.YieldKeyword => "Yield", 
				SyntaxKind.ExclamationToken => "!", 
				SyntaxKind.AtToken => "@", 
				SyntaxKind.CommaToken => ",", 
				SyntaxKind.HashToken => "#", 
				SyntaxKind.AmpersandToken => "&", 
				SyntaxKind.SingleQuoteToken => "'", 
				SyntaxKind.OpenParenToken => "(", 
				SyntaxKind.CloseParenToken => ")", 
				SyntaxKind.OpenBraceToken => "{", 
				SyntaxKind.CloseBraceToken => "}", 
				SyntaxKind.SemicolonToken => ";", 
				SyntaxKind.AsteriskToken => "*", 
				SyntaxKind.PlusToken => "+", 
				SyntaxKind.MinusToken => "-", 
				SyntaxKind.DotToken => ".", 
				SyntaxKind.SlashToken => "/", 
				SyntaxKind.ColonToken => ":", 
				SyntaxKind.LessThanToken => "<", 
				SyntaxKind.LessThanEqualsToken => "<=", 
				SyntaxKind.LessThanGreaterThanToken => "<>", 
				SyntaxKind.EqualsToken => "=", 
				SyntaxKind.GreaterThanToken => ">", 
				SyntaxKind.GreaterThanEqualsToken => ">=", 
				SyntaxKind.BackslashToken => "\\", 
				SyntaxKind.CaretToken => "^", 
				SyntaxKind.ColonEqualsToken => ":=", 
				SyntaxKind.AmpersandEqualsToken => "&=", 
				SyntaxKind.AsteriskEqualsToken => "*=", 
				SyntaxKind.PlusEqualsToken => "+=", 
				SyntaxKind.MinusEqualsToken => "-=", 
				SyntaxKind.SlashEqualsToken => "/=", 
				SyntaxKind.BackslashEqualsToken => "\\=", 
				SyntaxKind.CaretEqualsToken => "^=", 
				SyntaxKind.LessThanLessThanToken => "<<", 
				SyntaxKind.GreaterThanGreaterThanToken => ">>", 
				SyntaxKind.LessThanLessThanEqualsToken => "<<=", 
				SyntaxKind.GreaterThanGreaterThanEqualsToken => ">>=", 
				SyntaxKind.QuestionToken => "?", 
				SyntaxKind.DoubleQuoteToken => "\"", 
				SyntaxKind.DollarSignDoubleQuoteToken => "$\"", 
				SyntaxKind.StatementTerminatorToken => "\r\n", 
				SyntaxKind.SlashGreaterThanToken => "/>", 
				SyntaxKind.LessThanSlashToken => "</", 
				SyntaxKind.LessThanExclamationMinusMinusToken => "<!--", 
				SyntaxKind.MinusMinusGreaterThanToken => "-->", 
				SyntaxKind.LessThanQuestionToken => "<?", 
				SyntaxKind.QuestionGreaterThanToken => "?>", 
				SyntaxKind.LessThanPercentEqualsToken => "<%=", 
				SyntaxKind.PercentGreaterThanToken => "%>", 
				SyntaxKind.BeginCDataToken => "<![CDATA[", 
				SyntaxKind.EndCDataToken => "]]>", 
				SyntaxKind.ColonTrivia => ":", 
				SyntaxKind.LineContinuationTrivia => "_\r\n", 
				SyntaxKind.DocumentationCommentExteriorTrivia => "'''", 
				_ => string.Empty, 
			};
		}

		internal static char MakeFullWidth(char c)
		{
			return Convert.ToChar(Convert.ToUInt16(c) + 65248);
		}

		internal static bool IsHalfWidth(char c)
		{
			if (c >= '!')
			{
				return c <= '~';
			}
			return false;
		}

		internal static char MakeHalfWidth(char c)
		{
			return Convert.ToChar(Convert.ToUInt16(c) - 65248);
		}

		internal static bool IsFullWidth(char c)
		{
			return c > '\uff00' && c < '｟';
		}

		public static bool IsWhitespace(char c)
		{
			if (' ' != c && '\t' != c)
			{
				if (c > '\u0080')
				{
					return IsWhitespaceNotAscii(c);
				}
				return false;
			}
			return true;
		}

		public static bool IsXmlWhitespace(char c)
		{
			if (' ' != c && '\t' != c)
			{
				if (c > '\u0080')
				{
					return XmlCharType.IsWhiteSpace(c);
				}
				return false;
			}
			return true;
		}

		internal static bool IsWhitespaceNotAscii(char ch)
		{
			switch (ch)
			{
			case '\u00a0':
			case '\u2000':
			case '\u2001':
			case '\u2002':
			case '\u2003':
			case '\u2004':
			case '\u2005':
			case '\u2006':
			case '\u2007':
			case '\u2008':
			case '\u2009':
			case '\u200a':
			case '\u200b':
			case '\u3000':
				return true;
			default:
				return CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.SpaceSeparator;
			}
		}

		public static bool IsNewLine(char c)
		{
			if ('\r' != c && '\n' != c)
			{
				if (c >= '\u0085')
				{
					if ('\u0085' != c && '\u2028' != c)
					{
						return '\u2029' == c;
					}
					return true;
				}
				return false;
			}
			return true;
		}

		internal static bool IsSingleQuote(char c)
		{
			if (c != '\'')
			{
				if (c >= '‘')
				{
					return c == '＇' || c == '‘' || c == '’';
				}
				return false;
			}
			return true;
		}

		internal static bool IsDoubleQuote(char c)
		{
			if (c != '"')
			{
				if (c >= '“')
				{
					return c == '＂' || c == '“' || c == '”';
				}
				return false;
			}
			return true;
		}

		internal static bool IsLeftCurlyBracket(char c)
		{
			if (c != '{')
			{
				return c == '｛';
			}
			return true;
		}

		internal static bool IsRightCurlyBracket(char c)
		{
			if (c != '}')
			{
				return c == '｝';
			}
			return true;
		}

		public static bool IsColon(char c)
		{
			if (c != ':')
			{
				return c == '：';
			}
			return true;
		}

		public static bool IsUnderscore(char c)
		{
			return c == '_';
		}

		public static bool IsHash(char c)
		{
			if (c != '#')
			{
				return c == '＃';
			}
			return true;
		}

		public static bool IsIdentifierStartCharacter(char c)
		{
			UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			if (!IsPropAlpha(unicodeCategory) && !IsPropLetterDigit(unicodeCategory))
			{
				return IsPropConnectorPunctuation(unicodeCategory);
			}
			return true;
		}

		internal static byte IntegralLiteralCharacterValue(char Digit)
		{
			if (IsFullWidth(Digit))
			{
				Digit = MakeHalfWidth(Digit);
			}
			int num = Digit;
			if (IsDecimalDigit(Digit))
			{
				return (byte)(num - 48);
			}
			if (Digit >= 'A' && Digit <= 'F')
			{
				return (byte)(num + -55);
			}
			return (byte)(num + -87);
		}

		internal static bool BeginsBaseLiteral(char c)
		{
			if (!(c == 'H' || c == 'O' || c == 'B' || c == 'h' || c == 'o' || c == 'b'))
			{
				return (IsFullWidth(c) && (c == 'Ｈ' || c == 'ｈ')) || c == 'Ｏ' || c == 'ｏ' || c == 'Ｂ' || c == 'ｂ';
			}
			return true;
		}

		internal static bool IsNarrowIdentifierCharacter(ushort c)
		{
			return s_isIDChar[c];
		}

		public static bool IsIdentifierPartCharacter(char c)
		{
			if (c < '\u0080')
			{
				return IsNarrowIdentifierCharacter(Convert.ToUInt16(c));
			}
			return IsWideIdentifierCharacter(c);
		}

		public static bool IsValidIdentifier(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return false;
			}
			if (!IsIdentifierStartCharacter(name[0]))
			{
				return false;
			}
			int num = name.Length - 1;
			for (int i = 1; i <= num; i++)
			{
				if (!IsIdentifierPartCharacter(name[i]))
				{
					return false;
				}
			}
			return true;
		}

		public static string MakeHalfWidthIdentifier(string text)
		{
			if (text == null)
			{
				return text;
			}
			char[] array = null;
			int num = text.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				char c = text[i];
				if (IsFullWidth(c))
				{
					if (array == null)
					{
						array = new char[text.Length - 1 + 1];
						text.CopyTo(0, array, 0, i);
					}
					array[i] = MakeHalfWidth(c);
				}
				else if (array != null)
				{
					array[i] = c;
				}
			}
			return (array == null) ? text : new string(array);
		}

		internal static bool IsWideIdentifierCharacter(char c)
		{
			UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			if (!IsPropAlphaNumeric(unicodeCategory) && !IsPropLetterDigit(unicodeCategory) && !IsPropConnectorPunctuation(unicodeCategory) && !IsPropCombining(unicodeCategory))
			{
				return IsPropOtherFormat(unicodeCategory);
			}
			return true;
		}

		internal static bool BeginsExponent(char c)
		{
			return c == 'E' || c == 'e' || c == 'Ｅ' || c == 'ｅ';
		}

		internal static bool IsBinaryDigit(char c)
		{
			return (c >= '0' && c <= '1') || (c >= '０' && c <= '１');
		}

		internal static bool IsOctalDigit(char c)
		{
			return (c >= '0' && c <= '7') || (c >= '０' && c <= '７');
		}

		internal static bool IsDecimalDigit(char c)
		{
			return (c >= '0' && c <= '9') || (c >= '０' && c <= '９');
		}

		internal static bool IsHexDigit(char c)
		{
			if (!IsDecimalDigit(c) && !(c >= 'a' && c <= 'f') && !(c >= 'A' && c <= 'F') && !(c >= 'ａ' && c <= 'ｆ'))
			{
				return c >= 'Ａ' && c <= 'Ｆ';
			}
			return true;
		}

		internal static bool IsDateSeparatorCharacter(char c)
		{
			return c == '/' || c == '-' || c == '／' || c == '－';
		}

		internal static bool IsLetterC(char ch)
		{
			return ch == 'c' || ch == 'C' || ch == 'Ｃ' || ch == 'ｃ';
		}

		internal static bool MatchOneOrAnother(char ch, char one, char another)
		{
			return ch == one || ch == another;
		}

		internal static bool MatchOneOrAnotherOrFullwidth(char ch, char one, char another)
		{
			if (IsFullWidth(ch))
			{
				ch = MakeHalfWidth(ch);
			}
			return ch == one || ch == another;
		}

		internal static bool IsPropAlpha(UnicodeCategory CharacterProperties)
		{
			return CharacterProperties <= UnicodeCategory.OtherLetter;
		}

		internal static bool IsPropAlphaNumeric(UnicodeCategory CharacterProperties)
		{
			return CharacterProperties <= UnicodeCategory.DecimalDigitNumber;
		}

		internal static bool IsPropLetterDigit(UnicodeCategory CharacterProperties)
		{
			return CharacterProperties == UnicodeCategory.LetterNumber;
		}

		internal static bool IsPropConnectorPunctuation(UnicodeCategory CharacterProperties)
		{
			return CharacterProperties == UnicodeCategory.ConnectorPunctuation;
		}

		internal static bool IsPropCombining(UnicodeCategory CharacterProperties)
		{
			if (CharacterProperties >= UnicodeCategory.NonSpacingMark)
			{
				return CharacterProperties <= UnicodeCategory.EnclosingMark;
			}
			return false;
		}

		internal static bool IsConnectorPunctuation(char c)
		{
			return CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.ConnectorPunctuation;
		}

		internal static bool IsSpaceSeparator(char c)
		{
			return CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
		}

		internal static bool IsPropOtherFormat(UnicodeCategory CharacterProperties)
		{
			return CharacterProperties == UnicodeCategory.Format;
		}

		internal static bool IsSurrogate(char c)
		{
			return char.IsSurrogate(c);
		}

		internal static bool IsHighSurrogate(char c)
		{
			return char.IsHighSurrogate(c);
		}

		internal static bool IsLowSurrogate(char c)
		{
			return char.IsLowSurrogate(c);
		}

		internal static char ReturnFullWidthOrSelf(char c)
		{
			if (IsHalfWidth(c))
			{
				return MakeFullWidth(c);
			}
			return c;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use IsAccessorStatementAccessorKeyword instead.", true)]
		public static bool IsAccessorStatementKeyword(SyntaxKind kind)
		{
			return IsAccessorStatementAccessorKeyword(kind);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use IsDeclareStatementSubOrFunctionKeyword instead.", true)]
		public static bool IsDeclareStatementKeyword(SyntaxKind kind)
		{
			return IsDeclareStatementSubOrFunctionKeyword(kind);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use IsDelegateStatementSubOrFunctionKeyword instead.", true)]
		public static bool IsDelegateStatementKeyword(SyntaxKind kind)
		{
			return IsDelegateStatementSubOrFunctionKeyword(kind);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use IsLambdaHeaderSubOrFunctionKeyword instead.", true)]
		public static bool IsLambdaHeaderKeyword(SyntaxKind kind)
		{
			return IsLambdaHeaderSubOrFunctionKeyword(kind);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This member is obsolete. Use IsMethodStatementSubOrFunctionKeyword instead.", true)]
		public static bool IsMethodStatementKeyword(SyntaxKind kind)
		{
			return IsMethodStatementSubOrFunctionKeyword(kind);
		}

		public static bool IsTrivia(SyntaxKind @this)
		{
			return IsSyntaxTrivia(@this);
		}

		public static IEnumerable<SyntaxKind> GetKeywordKinds()
		{
			return GetReservedKeywordKinds().Concat(GetContextualKeywordKinds());
		}

		public static bool IsPredefinedType(SyntaxKind kind)
		{
			return IsPredefinedTypeKeyword(kind);
		}

		internal static bool IsPredefinedTypeOrVariant(SyntaxKind kind)
		{
			if (!IsPredefinedTypeKeyword(kind))
			{
				return kind == SyntaxKind.VariantKeyword;
			}
			return true;
		}

		public static bool IsInvoked(ExpressionSyntax node)
		{
			node = SyntaxFactory.GetStandaloneExpression(node);
			if (node.Parent is InvocationExpressionSyntax invocationExpressionSyntax)
			{
				return invocationExpressionSyntax.Expression == node;
			}
			return false;
		}

		public static bool IsAddressOfOperand(ExpressionSyntax node)
		{
			VisualBasicSyntaxNode parent = node.Parent;
			if (parent != null)
			{
				return parent.Kind() == SyntaxKind.AddressOfExpression;
			}
			return false;
		}

		public static bool IsInvocationOrAddressOfOperand(ExpressionSyntax node)
		{
			if (!IsInvoked(node))
			{
				return IsAddressOfOperand(node);
			}
			return true;
		}

		public static bool IsInTypeOnlyContext(ExpressionSyntax node)
		{
			if (!(node is TypeSyntax))
			{
				return false;
			}
			VisualBasicSyntaxNode parent = node.Parent;
			if (parent != null)
			{
				switch (parent.Kind())
				{
				case SyntaxKind.SimpleAsClause:
				case SyntaxKind.AsNewClause:
					return SyntaxExtensions.Type((AsClauseSyntax)parent) == node;
				case SyntaxKind.GetTypeExpression:
					return ((GetTypeExpressionSyntax)parent).Type == node;
				case SyntaxKind.TypeOfIsExpression:
				case SyntaxKind.TypeOfIsNotExpression:
					return ((TypeOfExpressionSyntax)parent).Type == node;
				case SyntaxKind.CTypeExpression:
				case SyntaxKind.DirectCastExpression:
				case SyntaxKind.TryCastExpression:
					return ((CastExpressionSyntax)parent).Type == node;
				case SyntaxKind.TypeArgumentList:
					return true;
				case SyntaxKind.InheritsStatement:
				case SyntaxKind.ImplementsStatement:
					return true;
				case SyntaxKind.TypeConstraint:
					return true;
				case SyntaxKind.CrefSignaturePart:
					return true;
				case SyntaxKind.Attribute:
					return ((AttributeSyntax)parent).Name == node;
				case SyntaxKind.ObjectCreationExpression:
					return ((ObjectCreationExpressionSyntax)parent).Type == node;
				case SyntaxKind.ArrayCreationExpression:
					return ((ArrayCreationExpressionSyntax)parent).Type == node;
				case SyntaxKind.ArrayType:
					return ((ArrayTypeSyntax)parent).ElementType == node;
				case SyntaxKind.NullableType:
					return ((NullableTypeSyntax)parent).ElementType == node;
				case SyntaxKind.QualifiedName:
				{
					QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)parent;
					if (qualifiedNameSyntax.Parent != null && qualifiedNameSyntax.Parent.Kind() == SyntaxKind.ImplementsClause)
					{
						return qualifiedNameSyntax.Left == node;
					}
					return qualifiedNameSyntax.Right == node;
				}
				case SyntaxKind.TypedTupleElement:
					return ((TypedTupleElementSyntax)parent).Type == node;
				}
			}
			return false;
		}

		internal static bool IsImplementedMember(SyntaxNode node)
		{
			SyntaxNode parent = node.Parent;
			if (parent != null)
			{
				return Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.ImplementsClause);
			}
			return false;
		}

		internal static bool IsHandlesEvent(SyntaxNode node)
		{
			SyntaxNode parent = node.Parent;
			if (parent != null && Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.HandlesClauseItem))
			{
				return node is IdentifierNameSyntax;
			}
			return false;
		}

		internal static bool IsHandlesContainer(SyntaxNode node)
		{
			return node is WithEventsEventContainerSyntax;
		}

		internal static bool IsHandlesProperty(SyntaxNode node)
		{
			SyntaxNode parent = node.Parent;
			if (parent != null && Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.WithEventsPropertyEventContainer))
			{
				return node is IdentifierNameSyntax;
			}
			return false;
		}

		public static bool IsInNamespaceOrTypeContext(SyntaxNode node)
		{
			if (node != null)
			{
				if (!(node is TypeSyntax))
				{
					return false;
				}
				SyntaxNode parent = node.Parent;
				if (parent != null)
				{
					switch (VisualBasicExtensions.Kind(parent))
					{
					case SyntaxKind.SimpleImportsClause:
						return ((SimpleImportsClauseSyntax)parent).Name == node;
					case SyntaxKind.NamespaceStatement:
						return ((NamespaceStatementSyntax)parent).Name == node;
					case SyntaxKind.QualifiedName:
					{
						QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)parent;
						if (qualifiedNameSyntax.Parent == null || qualifiedNameSyntax.Parent.Kind() != SyntaxKind.ImplementsClause)
						{
							return ((QualifiedNameSyntax)parent).Left == node;
						}
						break;
					}
					}
				}
				if (node is ExpressionSyntax node2)
				{
					return IsInTypeOnlyContext(node2);
				}
			}
			return false;
		}

		private static bool InOrBeforeSpanOrEffectiveTrailingOfNode(SyntaxNode node, int position)
		{
			if (position >= node.SpanStart)
			{
				return InSpanOrEffectiveTrailingOfNode(node, position);
			}
			return true;
		}

		internal static bool InSpanOrEffectiveTrailingOfNode(SyntaxNode node, int position)
		{
			TextSpan span = node.Span;
			if (span.Contains(position))
			{
				return true;
			}
			if (position >= span.End && position < node.FullSpan.End)
			{
				SyntaxTriviaList.Enumerator enumerator = node.GetTrailingTrivia().GetEnumerator();
				while (enumerator.MoveNext())
				{
					SyntaxTrivia current = enumerator.Current;
					if (VisualBasicExtensions.Kind(current) == SyntaxKind.EndOfLineTrivia || VisualBasicExtensions.Kind(current) == SyntaxKind.ColonTrivia)
					{
						break;
					}
					if (current.FullSpan.Contains(position))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal static bool InBlockInterior(SyntaxNode possibleBlock, int position)
		{
			SyntaxList<StatementSyntax> body = default(SyntaxList<StatementSyntax>);
			return InBlockInterior(possibleBlock, position, ref body);
		}

		internal static bool InLambdaInterior(SyntaxNode possibleLambda, int position)
		{
			bool flag;
			bool flag2;
			switch (VisualBasicExtensions.Kind(possibleLambda))
			{
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
			{
				SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = (SingleLineLambdaExpressionSyntax)possibleLambda;
				ParameterListSyntax parameterList2 = singleLineLambdaExpressionSyntax.SubOrFunctionHeader.ParameterList;
				flag = ((parameterList2 != null && !parameterList2.CloseParenToken.IsMissing) ? (position >= parameterList2.CloseParenToken.SpanStart) : (position >= singleLineLambdaExpressionSyntax.SubOrFunctionHeader.Span.End));
				flag2 = position <= singleLineLambdaExpressionSyntax.Body.Span.End;
				break;
			}
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
			{
				MultiLineLambdaExpressionSyntax multiLineLambdaExpressionSyntax = (MultiLineLambdaExpressionSyntax)possibleLambda;
				ParameterListSyntax parameterList = multiLineLambdaExpressionSyntax.SubOrFunctionHeader.ParameterList;
				flag = ((parameterList != null && !parameterList.CloseParenToken.IsMissing) ? (position >= parameterList.CloseParenToken.SpanStart) : (position >= multiLineLambdaExpressionSyntax.SubOrFunctionHeader.Span.End));
				flag2 = position < multiLineLambdaExpressionSyntax.EndSubOrFunctionStatement.SpanStart;
				break;
			}
			default:
				return false;
			}
			return flag && flag2;
		}

		internal static bool InBlockInterior(SyntaxNode possibleBlock, int position, ref SyntaxList<StatementSyntax> body)
		{
			StatementSyntax beginStatement = null;
			StatementSyntax endStatement = null;
			SyntaxToken beginTerminator = default(SyntaxToken);
			if (IsBlockStatement(possibleBlock, ref beginStatement, ref beginTerminator, ref body, ref endStatement))
			{
				bool flag = true;
				bool flag2 = true;
				flag = ((VisualBasicExtensions.Kind(beginTerminator) == SyntaxKind.None || beginTerminator.Width <= 0) ? (!InOrBeforeSpanOrEffectiveTrailingOfNode(beginStatement, position)) : (position >= beginTerminator.SpanStart));
				if (endStatement == null)
				{
					SyntaxKind syntaxKind = VisualBasicExtensions.Kind(possibleBlock);
					if (syntaxKind == SyntaxKind.SingleLineIfStatement || syntaxKind == SyntaxKind.SingleLineElseClause)
					{
						if (body.Count > 0)
						{
							flag2 = InOrBeforeSpanOrEffectiveTrailingOfNode(body[body.Count - 1], position);
						}
					}
					else
					{
						SyntaxToken nextToken = possibleBlock.GetLastToken(includeZeroWidth: true).GetNextToken();
						if (nextToken != default(SyntaxToken))
						{
							flag2 = position < nextToken.SpanStart;
						}
					}
				}
				else if (endStatement.Width > 0)
				{
					flag2 = InOrBeforeSpanOrEffectiveTrailingOfNode(endStatement, position);
				}
				return flag && flag2;
			}
			return false;
		}

		internal static bool IsBlockStatement(SyntaxNode possibleBlock, ref StatementSyntax beginStatement, ref SyntaxToken beginTerminator, ref SyntaxList<StatementSyntax> body, ref StatementSyntax endStatement)
		{
			beginTerminator = default(SyntaxToken);
			switch (VisualBasicExtensions.Kind(possibleBlock))
			{
			case SyntaxKind.NamespaceBlock:
			{
				NamespaceBlockSyntax namespaceBlockSyntax = (NamespaceBlockSyntax)possibleBlock;
				beginStatement = namespaceBlockSyntax.NamespaceStatement;
				body = namespaceBlockSyntax.Members;
				endStatement = namespaceBlockSyntax.EndNamespaceStatement;
				return true;
			}
			case SyntaxKind.ModuleBlock:
			case SyntaxKind.StructureBlock:
			case SyntaxKind.InterfaceBlock:
			case SyntaxKind.ClassBlock:
			{
				TypeBlockSyntax typeBlockSyntax = (TypeBlockSyntax)possibleBlock;
				beginStatement = typeBlockSyntax.BlockStatement;
				body = typeBlockSyntax.Members;
				endStatement = typeBlockSyntax.EndBlockStatement;
				return true;
			}
			case SyntaxKind.EnumBlock:
			{
				EnumBlockSyntax enumBlockSyntax = (EnumBlockSyntax)possibleBlock;
				beginStatement = enumBlockSyntax.EnumStatement;
				body = enumBlockSyntax.Members;
				endStatement = enumBlockSyntax.EndEnumStatement;
				return true;
			}
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.ConstructorBlock:
			case SyntaxKind.OperatorBlock:
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
			case SyntaxKind.AddHandlerAccessorBlock:
			case SyntaxKind.RemoveHandlerAccessorBlock:
			case SyntaxKind.RaiseEventAccessorBlock:
			{
				MethodBlockBaseSyntax methodBlockBaseSyntax = (MethodBlockBaseSyntax)possibleBlock;
				beginStatement = methodBlockBaseSyntax.BlockStatement;
				body = methodBlockBaseSyntax.Statements;
				endStatement = methodBlockBaseSyntax.EndBlockStatement;
				return true;
			}
			case SyntaxKind.PropertyBlock:
			{
				PropertyBlockSyntax propertyBlockSyntax = (PropertyBlockSyntax)possibleBlock;
				beginStatement = propertyBlockSyntax.PropertyStatement;
				body = default(SyntaxList<StatementSyntax>);
				endStatement = propertyBlockSyntax.EndPropertyStatement;
				return true;
			}
			case SyntaxKind.EventBlock:
			{
				EventBlockSyntax eventBlockSyntax = (EventBlockSyntax)possibleBlock;
				beginStatement = eventBlockSyntax.EventStatement;
				body = default(SyntaxList<StatementSyntax>);
				endStatement = eventBlockSyntax.EndEventStatement;
				return true;
			}
			case SyntaxKind.WhileBlock:
			{
				WhileBlockSyntax whileBlockSyntax = (WhileBlockSyntax)possibleBlock;
				beginStatement = whileBlockSyntax.WhileStatement;
				body = whileBlockSyntax.Statements;
				endStatement = whileBlockSyntax.EndWhileStatement;
				return true;
			}
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
			{
				ForOrForEachBlockSyntax forOrForEachBlockSyntax = (ForOrForEachBlockSyntax)possibleBlock;
				beginStatement = forOrForEachBlockSyntax.ForOrForEachStatement;
				body = forOrForEachBlockSyntax.Statements;
				endStatement = forOrForEachBlockSyntax.NextStatement;
				return true;
			}
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
			case SyntaxKind.DoUntilLoopBlock:
			case SyntaxKind.DoLoopWhileBlock:
			case SyntaxKind.DoLoopUntilBlock:
			{
				DoLoopBlockSyntax doLoopBlockSyntax = (DoLoopBlockSyntax)possibleBlock;
				beginStatement = doLoopBlockSyntax.DoStatement;
				body = doLoopBlockSyntax.Statements;
				endStatement = doLoopBlockSyntax.LoopStatement;
				return true;
			}
			case SyntaxKind.UsingBlock:
			{
				UsingBlockSyntax usingBlockSyntax = (UsingBlockSyntax)possibleBlock;
				beginStatement = usingBlockSyntax.UsingStatement;
				body = usingBlockSyntax.Statements;
				endStatement = usingBlockSyntax.EndUsingStatement;
				return true;
			}
			case SyntaxKind.SyncLockBlock:
			{
				SyncLockBlockSyntax syncLockBlockSyntax = (SyncLockBlockSyntax)possibleBlock;
				beginStatement = syncLockBlockSyntax.SyncLockStatement;
				body = syncLockBlockSyntax.Statements;
				endStatement = syncLockBlockSyntax.EndSyncLockStatement;
				return true;
			}
			case SyntaxKind.WithBlock:
			{
				WithBlockSyntax withBlockSyntax = (WithBlockSyntax)possibleBlock;
				beginStatement = withBlockSyntax.WithStatement;
				body = withBlockSyntax.Statements;
				endStatement = withBlockSyntax.EndWithStatement;
				return true;
			}
			case SyntaxKind.SelectBlock:
			{
				SelectBlockSyntax selectBlockSyntax = (SelectBlockSyntax)possibleBlock;
				beginStatement = selectBlockSyntax.SelectStatement;
				body = default(SyntaxList<StatementSyntax>);
				endStatement = selectBlockSyntax.EndSelectStatement;
				return true;
			}
			case SyntaxKind.CaseBlock:
			case SyntaxKind.CaseElseBlock:
			{
				CaseBlockSyntax caseBlockSyntax = (CaseBlockSyntax)possibleBlock;
				beginStatement = caseBlockSyntax.CaseStatement;
				body = caseBlockSyntax.Statements;
				endStatement = null;
				return true;
			}
			case SyntaxKind.SingleLineIfStatement:
			{
				SingleLineIfStatementSyntax singleLineIfStatementSyntax = (SingleLineIfStatementSyntax)possibleBlock;
				beginStatement = null;
				beginTerminator = singleLineIfStatementSyntax.ThenKeyword;
				body = singleLineIfStatementSyntax.Statements;
				endStatement = null;
				return true;
			}
			case SyntaxKind.SingleLineElseClause:
			{
				SingleLineElseClauseSyntax singleLineElseClauseSyntax = (SingleLineElseClauseSyntax)possibleBlock;
				beginStatement = null;
				beginTerminator = singleLineElseClauseSyntax.ElseKeyword;
				body = singleLineElseClauseSyntax.Statements;
				endStatement = null;
				return true;
			}
			case SyntaxKind.MultiLineIfBlock:
			{
				MultiLineIfBlockSyntax multiLineIfBlockSyntax = (MultiLineIfBlockSyntax)possibleBlock;
				beginStatement = multiLineIfBlockSyntax.IfStatement;
				body = multiLineIfBlockSyntax.Statements;
				endStatement = multiLineIfBlockSyntax.EndIfStatement;
				return true;
			}
			case SyntaxKind.ElseIfBlock:
			{
				ElseIfBlockSyntax elseIfBlockSyntax = (ElseIfBlockSyntax)possibleBlock;
				beginStatement = elseIfBlockSyntax.ElseIfStatement;
				body = elseIfBlockSyntax.Statements;
				endStatement = null;
				return true;
			}
			case SyntaxKind.ElseBlock:
			{
				ElseBlockSyntax elseBlockSyntax = (ElseBlockSyntax)possibleBlock;
				beginStatement = elseBlockSyntax.ElseStatement;
				body = elseBlockSyntax.Statements;
				endStatement = null;
				return true;
			}
			case SyntaxKind.TryBlock:
			{
				TryBlockSyntax tryBlockSyntax = (TryBlockSyntax)possibleBlock;
				beginStatement = tryBlockSyntax.TryStatement;
				body = tryBlockSyntax.Statements;
				endStatement = tryBlockSyntax.EndTryStatement;
				return true;
			}
			case SyntaxKind.CatchBlock:
			{
				CatchBlockSyntax catchBlockSyntax = (CatchBlockSyntax)possibleBlock;
				beginStatement = catchBlockSyntax.CatchStatement;
				body = catchBlockSyntax.Statements;
				endStatement = null;
				return true;
			}
			case SyntaxKind.FinallyBlock:
			{
				FinallyBlockSyntax finallyBlockSyntax = (FinallyBlockSyntax)possibleBlock;
				beginStatement = finallyBlockSyntax.FinallyStatement;
				body = finallyBlockSyntax.Statements;
				endStatement = null;
				return true;
			}
			default:
				return false;
			}
		}

		internal static SyntaxNode BeginOfBlockStatementIfAny(SyntaxNode node)
		{
			StatementSyntax beginStatement = null;
			SyntaxList<StatementSyntax> body = default(SyntaxList<StatementSyntax>);
			StatementSyntax endStatement = null;
			SyntaxToken beginTerminator = default(SyntaxToken);
			if (IsBlockStatement(node, ref beginStatement, ref beginTerminator, ref body, ref endStatement) && beginStatement != null)
			{
				return beginStatement;
			}
			return node;
		}

		public static string GetText(Accessibility accessibility)
		{
			return accessibility switch
			{
				Accessibility.Internal => GetText(SyntaxKind.FriendKeyword), 
				Accessibility.NotApplicable => string.Empty, 
				Accessibility.Private => GetText(SyntaxKind.PrivateKeyword), 
				Accessibility.Protected => GetText(SyntaxKind.ProtectedKeyword), 
				Accessibility.ProtectedAndInternal => GetText(SyntaxKind.PrivateKeyword) + " " + GetText(SyntaxKind.ProtectedKeyword), 
				Accessibility.ProtectedOrInternal => GetText(SyntaxKind.ProtectedKeyword) + " " + GetText(SyntaxKind.FriendKeyword), 
				Accessibility.Public => GetText(SyntaxKind.PublicKeyword), 
				_ => null, 
			};
		}

		public static bool IsAnyToken(SyntaxKind kind)
		{
			if (kind >= SyntaxKind.AddHandlerKeyword)
			{
				return kind <= SyntaxKind.CharacterLiteralToken;
			}
			return false;
		}

		public static SyntaxKind GetUnaryExpression(SyntaxKind token)
		{
			return token switch
			{
				SyntaxKind.PlusToken => SyntaxKind.UnaryPlusExpression, 
				SyntaxKind.MinusToken => SyntaxKind.UnaryMinusExpression, 
				SyntaxKind.NotKeyword => SyntaxKind.NotExpression, 
				SyntaxKind.AddressOfKeyword => SyntaxKind.AddressOfExpression, 
				_ => SyntaxKind.None, 
			};
		}

		public static bool IsPreprocessorPunctuation(SyntaxKind kind)
		{
			return kind == SyntaxKind.HashToken;
		}

		public static bool IsLanguagePunctuation(SyntaxKind kind)
		{
			if (IsPunctuation(kind))
			{
				return !IsPreprocessorPunctuation(kind);
			}
			return false;
		}

		public static bool IsName(SyntaxKind kind)
		{
			if (kind != SyntaxKind.IdentifierName && kind != SyntaxKind.GenericName && kind != SyntaxKind.QualifiedName)
			{
				return kind == SyntaxKind.GlobalName;
			}
			return true;
		}

		public static bool IsNamespaceMemberDeclaration(SyntaxKind kind)
		{
			if (kind != SyntaxKind.ClassStatement && kind != SyntaxKind.InterfaceStatement && kind != SyntaxKind.StructureStatement && kind != SyntaxKind.EnumStatement && kind != SyntaxKind.ModuleStatement && kind != SyntaxKind.NamespaceStatement && kind != SyntaxKind.DelegateFunctionStatement)
			{
				return kind == SyntaxKind.DelegateSubStatement;
			}
			return true;
		}

		public static bool IsPunctuationOrKeyword(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if ((syntaxKind >= SyntaxKind.AddHandlerKeyword && syntaxKind <= SyntaxKind.EndOfXmlToken) || syntaxKind == SyntaxKind.NameOfKeyword || syntaxKind == SyntaxKind.DollarSignDoubleQuoteToken || syntaxKind == SyntaxKind.EndOfInterpolatedStringToken)
			{
				return true;
			}
			return false;
		}

		public static VarianceKind VarianceKindFromToken(SyntaxToken token)
		{
			return VisualBasicExtensions.Kind(token) switch
			{
				SyntaxKind.OutKeyword => VarianceKind.Out, 
				SyntaxKind.InKeyword => VarianceKind.In, 
				_ => VarianceKind.None, 
			};
		}

		public static bool IsAttributeName(SyntaxNode node)
		{
			for (SyntaxNode syntaxNode = node; syntaxNode != null; syntaxNode = syntaxNode.Parent)
			{
				switch (VisualBasicExtensions.Kind(syntaxNode))
				{
				case SyntaxKind.IdentifierName:
				case SyntaxKind.QualifiedName:
					break;
				case SyntaxKind.Attribute:
				{
					AttributeSyntax attributeSyntax = (AttributeSyntax)syntaxNode;
					if (attributeSyntax.Name == node)
					{
						return true;
					}
					return attributeSyntax.Name is QualifiedNameSyntax qualifiedNameSyntax && qualifiedNameSyntax.Right == node;
				}
				default:
					return false;
				}
			}
			return false;
		}

		public static bool IsNamedArgumentName(SyntaxNode node)
		{
			if (VisualBasicExtensions.Kind(node) != SyntaxKind.IdentifierName)
			{
				return false;
			}
			if (!(node.Parent is NameColonEqualsSyntax nameColonEqualsSyntax))
			{
				return false;
			}
			VisualBasicSyntaxNode parent = nameColonEqualsSyntax.Parent.Parent;
			if (parent == null || !Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.ArgumentList))
			{
				return false;
			}
			VisualBasicSyntaxNode parent2 = parent.Parent;
			if (parent2 == null)
			{
				return false;
			}
			SyntaxKind syntaxKind = parent2.Kind();
			if (syntaxKind == SyntaxKind.RaiseEventStatement || syntaxKind - 296 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static string GetBlockName(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.CaseBlock:
				return "Case";
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
			case SyntaxKind.DoUntilLoopBlock:
			case SyntaxKind.DoLoopWhileBlock:
			case SyntaxKind.DoLoopUntilBlock:
				return "Do Loop";
			case SyntaxKind.WhileBlock:
				return "While";
			case SyntaxKind.WithBlock:
				return "With";
			case SyntaxKind.SyncLockBlock:
				return "SyncLock";
			case SyntaxKind.UsingBlock:
				return "Using";
			case SyntaxKind.ForBlock:
				return "For";
			case SyntaxKind.ForEachBlock:
				return "For Each";
			case SyntaxKind.SelectBlock:
				return "Select";
			case SyntaxKind.MultiLineIfBlock:
				return "If";
			case SyntaxKind.ElseIfBlock:
				return "Else If";
			case SyntaxKind.ElseBlock:
				return "Else";
			case SyntaxKind.TryBlock:
				return "Try";
			case SyntaxKind.CatchBlock:
				return "Catch";
			case SyntaxKind.FinallyBlock:
				return "Finally";
			default:
				throw new ArgumentOutOfRangeException("kind");
			}
		}

		public static bool AllowsTrailingImplicitLineContinuation(SyntaxToken token)
		{
			if (token.Parent == null)
			{
				throw new ArgumentException("'token' must be parented by a SyntaxNode.");
			}
			SyntaxKind syntaxKind = VisualBasicExtensions.Kind(token);
			SyntaxKind syntaxKind2 = VisualBasicExtensions.Kind(token.Parent);
			switch (syntaxKind)
			{
			case SyntaxKind.CommaToken:
				return true;
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
				return token.Parent is BinaryExpressionSyntax;
			case SyntaxKind.GreaterThanToken:
				if (syntaxKind2 == SyntaxKind.XmlNamespaceImportsClause)
				{
					return false;
				}
				break;
			}
			if (IsBinaryExpressionOperatorToken(syntaxKind) || IsAssignmentStatementOperatorToken(syntaxKind))
			{
				return true;
			}
			switch (syntaxKind)
			{
			case SyntaxKind.ColonEqualsToken:
				return true;
			case SyntaxKind.OpenParenToken:
			case SyntaxKind.LessThanPercentEqualsToken:
			case SyntaxKind.PercentGreaterThanToken:
				return true;
			case SyntaxKind.OpenBraceToken:
				if (syntaxKind2 == SyntaxKind.Interpolation)
				{
					return false;
				}
				return true;
			case SyntaxKind.DotToken:
				switch (syntaxKind2)
				{
				case SyntaxKind.NamedFieldInitializer:
					return false;
				case SyntaxKind.SimpleMemberAccessExpression:
					return ((MemberAccessExpressionSyntax)token.Parent).Expression != null || VisualBasicExtensions.Kind(token.Parent!.Parent) == SyntaxKind.NamedFieldInitializer;
				case SyntaxKind.XmlElementAccessExpression:
				case SyntaxKind.XmlDescendantAccessExpression:
				case SyntaxKind.XmlAttributeAccessExpression:
					if (((XmlMemberAccessExpressionSyntax)token.Parent).Base != null)
					{
						return VisualBasicExtensions.Kind(token.GetNextToken()) != SyntaxKind.DotToken;
					}
					return false;
				default:
					return true;
				}
			case SyntaxKind.WithKeyword:
				return syntaxKind2 == SyntaxKind.ObjectMemberInitializer;
			case SyntaxKind.AggregateKeyword:
			case SyntaxKind.ByKeyword:
			case SyntaxKind.EqualsKeyword:
			case SyntaxKind.FromKeyword:
			case SyntaxKind.IntoKeyword:
			case SyntaxKind.JoinKeyword:
			case SyntaxKind.WhereKeyword:
				return true;
			case SyntaxKind.GetXmlNamespaceKeyword:
			case SyntaxKind.OfKeyword:
				return true;
			case SyntaxKind.GroupKeyword:
				return syntaxKind2 != SyntaxKind.GroupJoinClause;
			case SyntaxKind.SkipKeyword:
				return syntaxKind2 != SyntaxKind.SkipWhileClause;
			case SyntaxKind.TakeKeyword:
				return syntaxKind2 != SyntaxKind.TakeWhileClause;
			case SyntaxKind.InKeyword:
				return syntaxKind2 == SyntaxKind.CollectionRangeVariable || syntaxKind2 == SyntaxKind.ForEachStatement;
			case SyntaxKind.LetKeyword:
			case SyntaxKind.OnKeyword:
			case SyntaxKind.SelectKeyword:
			case SyntaxKind.WhileKeyword:
				return token.Parent is QueryClauseSyntax;
			case SyntaxKind.XmlKeyword:
			case SyntaxKind.DoubleQuoteToken:
			case SyntaxKind.LessThanExclamationMinusMinusToken:
			case SyntaxKind.LessThanQuestionToken:
			case SyntaxKind.BeginCDataToken:
			case SyntaxKind.XmlNameToken:
			case SyntaxKind.XmlTextLiteralToken:
				return true;
			case SyntaxKind.SlashGreaterThanToken:
			case SyntaxKind.MinusMinusGreaterThanToken:
			case SyntaxKind.QuestionGreaterThanToken:
			case SyntaxKind.EndCDataToken:
			{
				for (XmlNodeSyntax xmlNodeSyntax = token.Parent!.Parent as XmlNodeSyntax; xmlNodeSyntax != null; xmlNodeSyntax = xmlNodeSyntax.Parent as XmlNodeSyntax)
				{
					if (xmlNodeSyntax.EndPosition > token.EndPosition)
					{
						return true;
					}
				}
				return false;
			}
			case SyntaxKind.ColonToken:
				return syntaxKind2 == SyntaxKind.XmlPrefix;
			default:
				return false;
			}
		}

		public static bool AllowsLeadingImplicitLineContinuation(SyntaxToken token)
		{
			if (token.Parent == null)
			{
				throw new ArgumentException("'token' must be parented by a SyntaxNode.");
			}
			SyntaxKind syntaxKind = VisualBasicExtensions.Kind(token);
			SyntaxKind syntaxKind2 = VisualBasicExtensions.Kind(token.Parent);
			switch (syntaxKind)
			{
			case SyntaxKind.LessThanToken:
			case SyntaxKind.GreaterThanToken:
				return syntaxKind2 == SyntaxKind.AttributeList;
			case SyntaxKind.CloseParenToken:
			case SyntaxKind.PercentGreaterThanToken:
				return true;
			case SyntaxKind.CloseBraceToken:
				if (syntaxKind2 == SyntaxKind.Interpolation)
				{
					return false;
				}
				return true;
			case SyntaxKind.AscendingKeyword:
			case SyntaxKind.DescendingKeyword:
			case SyntaxKind.DistinctKeyword:
			case SyntaxKind.GroupKeyword:
			case SyntaxKind.IntoKeyword:
			case SyntaxKind.OrderKeyword:
			case SyntaxKind.SkipKeyword:
			case SyntaxKind.TakeKeyword:
			case SyntaxKind.WhereKeyword:
				return true;
			case SyntaxKind.JoinKeyword:
				return syntaxKind2 != SyntaxKind.GroupJoinClause;
			case SyntaxKind.InKeyword:
			case SyntaxKind.LetKeyword:
			case SyntaxKind.OnKeyword:
			case SyntaxKind.SelectKeyword:
				return token.Parent is QueryClauseSyntax;
			case SyntaxKind.AggregateKeyword:
			case SyntaxKind.FromKeyword:
			{
				VisualBasicSyntaxNode visualBasicSyntaxNode = token.Parent as QueryClauseSyntax;
				if (visualBasicSyntaxNode == null)
				{
					return false;
				}
				for (visualBasicSyntaxNode = visualBasicSyntaxNode.Parent; visualBasicSyntaxNode != null; visualBasicSyntaxNode = visualBasicSyntaxNode.Parent)
				{
					if (visualBasicSyntaxNode is QueryExpressionSyntax)
					{
						if (visualBasicSyntaxNode.GetLocation().SourceSpan.Start < token.SpanStart)
						{
							return true;
						}
						return false;
					}
				}
				return false;
			}
			default:
				return false;
			}
		}

		public static SyntaxKind GetOperatorKind(string operatorMetadataName)
		{
			OverloadResolution.OperatorInfo operatorInfo = OverloadResolution.GetOperatorInfo(operatorMetadataName);
			if (operatorInfo.ParamCount != 0)
			{
				return OverloadResolution.GetOperatorTokenKind(operatorInfo);
			}
			return SyntaxKind.None;
		}

		public static bool IsAccessibilityModifier(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.FriendKeyword || syntaxKind == SyntaxKind.PrivateKeyword || syntaxKind - 533 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		internal static bool IsTerminator(SyntaxKind kind)
		{
			if (kind != SyntaxKind.StatementTerminatorToken && kind != SyntaxKind.ColonToken)
			{
				return kind == SyntaxKind.EndOfFileToken;
			}
			return true;
		}

		internal static bool IsWithinPreprocessorConditionalExpression(SyntaxNode node)
		{
			for (SyntaxNode parent = node.Parent; parent != null; parent = node.Parent)
			{
				switch (VisualBasicExtensions.Kind(parent))
				{
				case SyntaxKind.IfDirectiveTrivia:
				case SyntaxKind.ElseIfDirectiveTrivia:
					return ((IfDirectiveTriviaSyntax)parent).Condition == node;
				case SyntaxKind.ConstDirectiveTrivia:
					return ((ConstDirectiveTriviaSyntax)parent).Value == node;
				}
				node = parent;
			}
			return false;
		}

		public static bool IsReservedTupleElementName(string elementName)
		{
			return TupleTypeSymbol.IsElementNameReserved(elementName) != -1;
		}

		public static bool IsReservedKeyword(SyntaxKind kind)
		{
			if (kind - 413 > SyntaxKind.SingleLineIfStatement)
			{
				return kind == SyntaxKind.NameOfKeyword;
			}
			return true;
		}

		public static bool IsContextualKeyword(SyntaxKind kind)
		{
			if (kind != SyntaxKind.ReferenceKeyword)
			{
				if (SyntaxKind.AggregateKeyword <= kind)
				{
					return kind <= SyntaxKind.YieldKeyword;
				}
				return false;
			}
			return true;
		}

		public static bool IsInstanceExpression(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.MeKeyword || syntaxKind - 506 <= SyntaxKind.List)
			{
				return true;
			}
			return false;
		}

		public static SyntaxKind GetInstanceExpression(SyntaxKind kind)
		{
			return kind switch
			{
				SyntaxKind.MeKeyword => SyntaxKind.MeExpression, 
				SyntaxKind.MyClassKeyword => SyntaxKind.MyClassExpression, 
				SyntaxKind.MyBaseKeyword => SyntaxKind.MyBaseExpression, 
				_ => SyntaxKind.None, 
			};
		}

		public static bool IsPreprocessorKeyword(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.ConstKeyword:
			case SyntaxKind.ReferenceKeyword:
			case SyntaxKind.ElseKeyword:
			case SyntaxKind.ElseIfKeyword:
			case SyntaxKind.EndKeyword:
			case SyntaxKind.IfKeyword:
			case SyntaxKind.EndIfKeyword:
			case SyntaxKind.DisableKeyword:
			case SyntaxKind.EnableKeyword:
			case SyntaxKind.ExternalSourceKeyword:
			case SyntaxKind.ExternalChecksumKeyword:
			case SyntaxKind.RegionKeyword:
				return true;
			default:
				return false;
			}
		}

		public static IEnumerable<SyntaxKind> GetReservedKeywordKinds()
		{
			return s_reservedKeywords;
		}

		public static IEnumerable<SyntaxKind> GetContextualKeywordKinds()
		{
			return s_contextualKeywords;
		}

		public static IEnumerable<SyntaxKind> GetPunctuationKinds()
		{
			return s_punctuationKinds;
		}

		public static IEnumerable<SyntaxKind> GetPreprocessorKeywordKinds()
		{
			return s_preprocessorKeywords;
		}

		internal static bool IsSpecifier(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.ConstKeyword:
			case SyntaxKind.DefaultKeyword:
			case SyntaxKind.DimKeyword:
			case SyntaxKind.FriendKeyword:
			case SyntaxKind.MustInheritKeyword:
			case SyntaxKind.MustOverrideKeyword:
			case SyntaxKind.NarrowingKeyword:
			case SyntaxKind.NotInheritableKeyword:
			case SyntaxKind.NotOverridableKeyword:
			case SyntaxKind.OverloadsKeyword:
			case SyntaxKind.OverridableKeyword:
			case SyntaxKind.OverridesKeyword:
			case SyntaxKind.PartialKeyword:
			case SyntaxKind.PrivateKeyword:
			case SyntaxKind.ProtectedKeyword:
			case SyntaxKind.PublicKeyword:
			case SyntaxKind.ReadOnlyKeyword:
			case SyntaxKind.ShadowsKeyword:
			case SyntaxKind.SharedKeyword:
			case SyntaxKind.StaticKeyword:
			case SyntaxKind.WideningKeyword:
			case SyntaxKind.WithEventsKeyword:
			case SyntaxKind.WriteOnlyKeyword:
			case SyntaxKind.CustomKeyword:
			case SyntaxKind.AsyncKeyword:
			case SyntaxKind.IteratorKeyword:
				return true;
			default:
				return false;
			}
		}

		internal static bool CanStartSpecifierDeclaration(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.ClassKeyword:
			case SyntaxKind.DeclareKeyword:
			case SyntaxKind.DelegateKeyword:
			case SyntaxKind.EnumKeyword:
			case SyntaxKind.EventKeyword:
			case SyntaxKind.FunctionKeyword:
			case SyntaxKind.InterfaceKeyword:
			case SyntaxKind.ModuleKeyword:
			case SyntaxKind.OperatorKeyword:
			case SyntaxKind.PropertyKeyword:
			case SyntaxKind.StructureKeyword:
			case SyntaxKind.SubKeyword:
			case SyntaxKind.IdentifierToken:
				return true;
			default:
				return false;
			}
		}

		public static bool IsRelationalOperator(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 653 <= SyntaxKind.EndIfStatement)
			{
				return true;
			}
			return false;
		}

		public static bool IsOperator(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.AndKeyword:
			case SyntaxKind.AndAlsoKeyword:
			case SyntaxKind.CBoolKeyword:
			case SyntaxKind.CByteKeyword:
			case SyntaxKind.CCharKeyword:
			case SyntaxKind.CDateKeyword:
			case SyntaxKind.CDecKeyword:
			case SyntaxKind.CDblKeyword:
			case SyntaxKind.CIntKeyword:
			case SyntaxKind.CLngKeyword:
			case SyntaxKind.CObjKeyword:
			case SyntaxKind.CSByteKeyword:
			case SyntaxKind.CShortKeyword:
			case SyntaxKind.CSngKeyword:
			case SyntaxKind.CStrKeyword:
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.CUIntKeyword:
			case SyntaxKind.CULngKeyword:
			case SyntaxKind.CUShortKeyword:
			case SyntaxKind.DirectCastKeyword:
			case SyntaxKind.GetTypeKeyword:
			case SyntaxKind.IsKeyword:
			case SyntaxKind.IsNotKeyword:
			case SyntaxKind.LikeKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.NewKeyword:
			case SyntaxKind.NotKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.OrElseKeyword:
			case SyntaxKind.TryCastKeyword:
			case SyntaxKind.TypeOfKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.IsFalseKeyword:
			case SyntaxKind.IsTrueKeyword:
			case SyntaxKind.ExclamationToken:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.DotToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.AmpersandEqualsToken:
			case SyntaxKind.AsteriskEqualsToken:
			case SyntaxKind.PlusEqualsToken:
			case SyntaxKind.MinusEqualsToken:
			case SyntaxKind.SlashEqualsToken:
			case SyntaxKind.BackslashEqualsToken:
			case SyntaxKind.CaretEqualsToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
			case SyntaxKind.LessThanLessThanEqualsToken:
			case SyntaxKind.GreaterThanGreaterThanEqualsToken:
			case SyntaxKind.NameOfKeyword:
				return true;
			default:
				return false;
			}
		}

		public static bool IsPreprocessorDirective(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.ConstDirectiveTrivia:
			case SyntaxKind.IfDirectiveTrivia:
			case SyntaxKind.ElseIfDirectiveTrivia:
			case SyntaxKind.ElseDirectiveTrivia:
			case SyntaxKind.EndIfDirectiveTrivia:
			case SyntaxKind.RegionDirectiveTrivia:
			case SyntaxKind.EndRegionDirectiveTrivia:
			case SyntaxKind.ExternalSourceDirectiveTrivia:
			case SyntaxKind.EndExternalSourceDirectiveTrivia:
			case SyntaxKind.ExternalChecksumDirectiveTrivia:
			case SyntaxKind.EnableWarningDirectiveTrivia:
			case SyntaxKind.DisableWarningDirectiveTrivia:
			case SyntaxKind.ReferenceDirectiveTrivia:
			case SyntaxKind.BadDirectiveTrivia:
				return true;
			default:
				return false;
			}
		}

		internal static bool SupportsContinueStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind == SyntaxKind.WhileBlock || syntaxKind - 237 <= SyntaxKind.List || syntaxKind - 756 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		internal static bool SupportsExitStatement(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.PropertyBlock:
			case SyntaxKind.WhileBlock:
			case SyntaxKind.TryBlock:
			case SyntaxKind.SelectBlock:
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
			case SyntaxKind.SingleLineSubLambdaExpression:
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
			case SyntaxKind.DoUntilLoopBlock:
				return true;
			default:
				return false;
			}
		}

		internal static bool IsEndBlockLoopOrNextStatement(SyntaxKind kind)
		{
			SyntaxKind syntaxKind = kind;
			if (syntaxKind - 5 <= SyntaxKind.EndAddHandlerStatement || syntaxKind == SyntaxKind.NextStatement || syntaxKind - 773 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		internal static bool IsXmlSyntax(SyntaxKind kind)
		{
			switch (kind)
			{
			case SyntaxKind.XmlDocument:
			case SyntaxKind.XmlDeclaration:
			case SyntaxKind.XmlDeclarationOption:
			case SyntaxKind.XmlElement:
			case SyntaxKind.XmlText:
			case SyntaxKind.XmlElementStartTag:
			case SyntaxKind.XmlElementEndTag:
			case SyntaxKind.XmlEmptyElement:
			case SyntaxKind.XmlAttribute:
			case SyntaxKind.XmlString:
			case SyntaxKind.XmlName:
			case SyntaxKind.XmlBracketedName:
			case SyntaxKind.XmlPrefix:
			case SyntaxKind.XmlComment:
			case SyntaxKind.XmlCDataSection:
			case SyntaxKind.XmlEmbeddedExpression:
				return true;
			default:
				return false;
			}
		}

		public static SyntaxKind GetKeywordKind(string text)
		{
			text = MakeHalfWidthIdentifier(text);
			SyntaxKind syntaxKind = KeywordTable.TokenOfString(text);
			if (syntaxKind != SyntaxKind.IdentifierToken && !IsContextualKeyword(syntaxKind))
			{
				return syntaxKind;
			}
			return SyntaxKind.None;
		}

		public static SyntaxKind GetAccessorStatementKind(SyntaxKind keyword)
		{
			return keyword switch
			{
				SyntaxKind.GetKeyword => SyntaxKind.GetAccessorStatement, 
				SyntaxKind.SetKeyword => SyntaxKind.SetAccessorStatement, 
				SyntaxKind.AddHandlerKeyword => SyntaxKind.AddHandlerStatement, 
				SyntaxKind.RemoveHandlerKeyword => SyntaxKind.RemoveHandlerStatement, 
				SyntaxKind.RaiseEventKeyword => SyntaxKind.RaiseEventAccessorStatement, 
				_ => SyntaxKind.None, 
			};
		}

		public static SyntaxKind GetBaseTypeStatementKind(SyntaxKind keyword)
		{
			if (keyword != SyntaxKind.EnumKeyword)
			{
				return GetTypeStatementKind(keyword);
			}
			return SyntaxKind.EnumStatement;
		}

		public static SyntaxKind GetTypeStatementKind(SyntaxKind keyword)
		{
			return keyword switch
			{
				SyntaxKind.ClassKeyword => SyntaxKind.ClassStatement, 
				SyntaxKind.InterfaceKeyword => SyntaxKind.InterfaceStatement, 
				SyntaxKind.StructureKeyword => SyntaxKind.StructureStatement, 
				_ => SyntaxKind.None, 
			};
		}

		public static SyntaxKind GetBinaryExpression(SyntaxKind keyword)
		{
			return keyword switch
			{
				SyntaxKind.IsKeyword => SyntaxKind.IsExpression, 
				SyntaxKind.IsNotKeyword => SyntaxKind.IsNotExpression, 
				SyntaxKind.LikeKeyword => SyntaxKind.LikeExpression, 
				SyntaxKind.AndKeyword => SyntaxKind.AndExpression, 
				SyntaxKind.AndAlsoKeyword => SyntaxKind.AndAlsoExpression, 
				SyntaxKind.OrKeyword => SyntaxKind.OrExpression, 
				SyntaxKind.OrElseKeyword => SyntaxKind.OrElseExpression, 
				SyntaxKind.XorKeyword => SyntaxKind.ExclusiveOrExpression, 
				SyntaxKind.AmpersandToken => SyntaxKind.ConcatenateExpression, 
				SyntaxKind.AsteriskToken => SyntaxKind.MultiplyExpression, 
				SyntaxKind.PlusToken => SyntaxKind.AddExpression, 
				SyntaxKind.MinusToken => SyntaxKind.SubtractExpression, 
				SyntaxKind.SlashToken => SyntaxKind.DivideExpression, 
				SyntaxKind.BackslashToken => SyntaxKind.IntegerDivideExpression, 
				SyntaxKind.ModKeyword => SyntaxKind.ModuloExpression, 
				SyntaxKind.CaretToken => SyntaxKind.ExponentiateExpression, 
				SyntaxKind.LessThanToken => SyntaxKind.LessThanExpression, 
				SyntaxKind.LessThanEqualsToken => SyntaxKind.LessThanOrEqualExpression, 
				SyntaxKind.LessThanGreaterThanToken => SyntaxKind.NotEqualsExpression, 
				SyntaxKind.EqualsToken => SyntaxKind.EqualsExpression, 
				SyntaxKind.GreaterThanToken => SyntaxKind.GreaterThanExpression, 
				SyntaxKind.GreaterThanEqualsToken => SyntaxKind.GreaterThanOrEqualExpression, 
				SyntaxKind.LessThanLessThanToken => SyntaxKind.LeftShiftExpression, 
				SyntaxKind.GreaterThanGreaterThanToken => SyntaxKind.RightShiftExpression, 
				_ => SyntaxKind.None, 
			};
		}

		public static SyntaxKind GetContextualKeywordKind(string text)
		{
			text = MakeHalfWidthIdentifier(text);
			SyntaxKind value = SyntaxKind.None;
			if (!s_contextualKeywordToSyntaxKindMap.TryGetValue(text, out value))
			{
				return SyntaxKind.None;
			}
			return value;
		}

		public static SyntaxKind GetPreprocessorKeywordKind(string text)
		{
			text = MakeHalfWidthIdentifier(text);
			SyntaxKind value = SyntaxKind.None;
			if (!s_preprocessorKeywordToSyntaxKindMap.TryGetValue(text, out value))
			{
				return SyntaxKind.None;
			}
			return value;
		}

		public static SyntaxKind GetLiteralExpression(SyntaxKind token)
		{
			switch (token)
			{
			case SyntaxKind.IntegerLiteralToken:
			case SyntaxKind.FloatingLiteralToken:
			case SyntaxKind.DecimalLiteralToken:
				return SyntaxKind.NumericLiteralExpression;
			case SyntaxKind.CharacterLiteralToken:
				return SyntaxKind.CharacterLiteralExpression;
			case SyntaxKind.DateLiteralToken:
				return SyntaxKind.DateLiteralExpression;
			case SyntaxKind.StringLiteralToken:
				return SyntaxKind.StringLiteralExpression;
			case SyntaxKind.TrueKeyword:
				return SyntaxKind.TrueLiteralExpression;
			case SyntaxKind.FalseKeyword:
				return SyntaxKind.FalseLiteralExpression;
			case SyntaxKind.NothingKeyword:
				return SyntaxKind.NothingLiteralExpression;
			default:
				return SyntaxKind.None;
			}
		}
	}
}
