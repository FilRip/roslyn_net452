using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	internal class KeywordTable
	{
		public struct KeywordDescription
		{
			public OperatorPrecedence kdOperPrec;

			public bool kdNew7To8kwd;

			public bool kdIsQueryClause;

			public bool kdCanFollowExpr;

			public KeywordDescription(bool New7To8, OperatorPrecedence Precedence, bool isQueryClause, bool canFollowExpr)
			{
				this = default(KeywordDescription);
				kdNew7To8kwd = New7To8;
				kdOperPrec = Precedence;
				kdIsQueryClause = isQueryClause;
				kdCanFollowExpr = canFollowExpr;
			}
		}

		private static readonly Dictionary<string, SyntaxKind> s_keywords;

		private static readonly Dictionary<ushort, KeywordDescription> s_keywordProperties;

		static KeywordTable()
		{
			s_keywords = new Dictionary<string, SyntaxKind>(CaseInsensitiveComparison.Comparer);
			s_keywordProperties = new Dictionary<ushort, KeywordDescription>();
			ushort[] array = new ushort[462]
			{
				413, 0, 414, 0, 415, 0, 416, 1027, 417, 1027,
				418, 0, 421, 0, 422, 0, 423, 0, 424, 0,
				425, 0, 426, 0, 427, 0, 428, 0, 429, 0,
				432, 0, 433, 0, 434, 0, 435, 0, 436, 0,
				437, 0, 438, 0, 439, 0, 440, 0, 441, 0,
				443, 256, 444, 256, 445, 0, 446, 0, 447, 0,
				448, 0, 449, 256, 450, 256, 453, 256, 454, 0,
				455, 0, 456, 0, 457, 0, 458, 0, 459, 0,
				460, 0, 461, 0, 462, 0, 463, 0, 464, 1024,
				465, 0, 466, 0, 467, 0, 468, 0, 469, 0,
				470, 0, 471, 0, 474, 0, 475, 0, 476, 0,
				477, 0, 478, 0, 479, 0, 480, 0, 481, 0,
				482, 256, 483, 0, 484, 0, 485, 0, 486, 1024,
				487, 0, 488, 1024, 489, 0, 490, 0, 491, 0,
				492, 1029, 495, 1285, 496, 1536, 497, 0, 498, 1029,
				499, 0, 500, 0, 501, 0, 502, 1033, 503, 0,
				504, 0, 505, 0, 506, 0, 507, 0, 778, 0,
				508, 0, 509, 256, 510, 0, 511, 0, 512, 4,
				513, 0, 516, 0, 517, 0, 518, 0, 519, 256,
				520, 1024, 521, 256, 522, 0, 523, 0, 524, 1026,
				525, 1026, 526, 0, 527, 0, 528, 0, 529, 0,
				530, 256, 531, 0, 532, 0, 533, 0, 534, 0,
				537, 0, 538, 0, 442, 0, 539, 0, 540, 1024,
				541, 0, 542, 0, 543, 0, 544, 256, 545, 1536,
				546, 0, 547, 0, 548, 0, 549, 0, 550, 0,
				551, 0, 552, 1024, 553, 0, 554, 0, 555, 0,
				558, 0, 559, 0, 560, 1024, 561, 0, 562, 1024,
				563, 0, 564, 0, 565, 256, 566, 0, 567, 256,
				568, 256, 569, 256, 570, 256, 571, 0, 572, 0,
				573, 256, 574, 0, 575, 0, 578, 0, 579, 1025,
				584, 1536, 585, 0, 586, 0, 587, 1024, 588, 0,
				589, 0, 590, 0, 591, 1024, 592, 0, 593, 0,
				594, 1024, 595, 0, 596, 1536, 599, 0, 600, 1024,
				601, 0, 602, 0, 603, 0, 604, 1536, 605, 1536,
				606, 0, 607, 1024, 608, 0, 609, 0, 610, 1536,
				611, 0, 612, 0, 613, 0, 614, 1536, 615, 0,
				616, 0, 617, 0, 620, 1536, 621, 0, 623, 0,
				622, 1536, 624, 0, 625, 0, 626, 0, 627, 1536,
				630, 0, 631, 14, 632, 0, 633, 0, 580, 0,
				581, 0, 628, 0, 582, 0, 583, 0, 636, 1024,
				638, 1031, 641, 0, 642, 1024, 643, 1024, 644, 0,
				645, 1024, 647, 1035, 648, 1032, 649, 1032, 651, 1035,
				653, 1029, 654, 1029, 655, 1029, 656, 1029, 657, 1029,
				658, 5, 659, 1034, 662, 1037, 663, 0, 664, 7,
				665, 11, 666, 8, 667, 8, 668, 11, 669, 10,
				670, 13, 671, 6, 672, 6, 673, 6, 674, 6,
				689, 1024
			};
			int num = array.Length - 1;
			for (int i = 0; i <= num; i += 2)
			{
				ushort num2 = array[i + 1];
				AddKeyword((SyntaxKind)array[i], (num2 & 0x100) != 0, (OperatorPrecedence)(num2 & 0xFFu), (num2 & 0x200) != 0, (num2 & 0x400) != 0);
			}
		}

		internal static SyntaxKind TokenOfString(string tokenName)
		{
			tokenName = EnsureHalfWidth(tokenName);
			if (!s_keywords.TryGetValue(tokenName, out var value))
			{
				return SyntaxKind.IdentifierToken;
			}
			return value;
		}

		private static string EnsureHalfWidth(string s)
		{
			char[] array = null;
			int num = s.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				char c = s[i];
				if (SyntaxFacts.IsFullWidth(c))
				{
					c = SyntaxFacts.MakeHalfWidth(c);
					if (array == null)
					{
						array = new char[s.Length - 1 + 1];
						int num2 = i - 1;
						for (int j = 0; j <= num2; j++)
						{
							array[j] = s[j];
						}
					}
					array[i] = c;
				}
				else if (array != null)
				{
					array[i] = c;
				}
			}
			if (array != null)
			{
				return new string(array);
			}
			return s;
		}

		internal static bool CanFollowExpression(SyntaxKind kind)
		{
			KeywordDescription value = default(KeywordDescription);
			if (s_keywordProperties.TryGetValue((ushort)kind, out value))
			{
				return value.kdCanFollowExpr;
			}
			return false;
		}

		internal static bool IsQueryClause(SyntaxKind kind)
		{
			KeywordDescription value = default(KeywordDescription);
			if (s_keywordProperties.TryGetValue((ushort)kind, out value))
			{
				return value.kdIsQueryClause;
			}
			return false;
		}

		internal static OperatorPrecedence TokenOpPrec(SyntaxKind kind)
		{
			KeywordDescription value = default(KeywordDescription);
			if (s_keywordProperties.TryGetValue((ushort)kind, out value))
			{
				return value.kdOperPrec;
			}
			return OperatorPrecedence.PrecedenceNone;
		}

		private static void AddKeyword(SyntaxKind Token, bool New7To8, OperatorPrecedence Precedence, bool isQueryClause, bool canFollowExpr)
		{
			KeywordDescription value = new KeywordDescription(New7To8, Precedence, isQueryClause, canFollowExpr);
			s_keywordProperties.Add((ushort)Token, value);
			string text = SyntaxFacts.GetText(Token);
			s_keywords.Add(text, Token);
		}
	}
}
