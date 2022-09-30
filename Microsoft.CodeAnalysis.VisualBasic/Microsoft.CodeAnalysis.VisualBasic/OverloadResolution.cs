using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.CodeGen;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class OverloadResolution
	{
		internal struct OperatorInfo
		{
			private readonly int _Id;

			public int ParamCount => _Id & 3;

			public bool IsBinary => ParamCount == 2;

			public bool IsUnary => ParamCount == 1;

			public UnaryOperatorKind UnaryOperatorKind
			{
				get
				{
					if (!IsUnary)
					{
						return UnaryOperatorKind.Error;
					}
					return (UnaryOperatorKind)(_Id >> 2);
				}
			}

			public BinaryOperatorKind BinaryOperatorKind
			{
				get
				{
					if (!IsBinary)
					{
						return BinaryOperatorKind.Error;
					}
					return (BinaryOperatorKind)(_Id >> 2);
				}
			}

			public OperatorInfo(UnaryOperatorKind op)
			{
				this = default(OperatorInfo);
				_Id = 1 | ((int)op << 2);
			}

			public OperatorInfo(BinaryOperatorKind op)
			{
				this = default(OperatorInfo);
				_Id = 2 | ((int)op << 2);
			}
		}

		private class BinaryOperatorTables
		{
			public enum TableKind
			{
				Addition,
				SubtractionMultiplicationModulo,
				Division,
				Power,
				IntegerDivision,
				Shift,
				Logical,
				Bitwise,
				Relational,
				ConcatenationLike
			}

			public static readonly sbyte[,,] Table;

			static BinaryOperatorTables()
			{
				Table = new sbyte[10, 16, 16]
				{
					{
						{
							1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 1
						},
						{
							1, 20, 19, 20, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 20
						},
						{
							1, 19, 11, 0, 9, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 20, 0, 20, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 19, 9, 0, 9, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 11, 0, 11, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 13, 0, 13, 13, 13, 15, 13, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 11, 0, 11, 11, 13, 15, 10, 12,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 13, 0, 13, 13, 13, 15, 12, 12,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 15, 0, 15, 15, 15, 15, 14, 14,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 17, 0, 17, 17, 17, 17, 16, 16,
							16, 16, 18, 19, 17, 0
						},
						{
							1, 19, 18, 0, 18, 18, 18, 18, 18, 18,
							18, 18, 18, 19, 18, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 17, 0, 17, 17, 17, 17, 17, 17,
							17, 17, 18, 19, 17, 0
						},
						{
							1, 20, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 20
						}
					},
					{
						{
							1, 1, 1, 0, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 11, 0, 9, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 19, 9, 0, 9, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 11, 0, 11, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 13, 0, 13, 13, 13, 15, 13, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 11, 0, 11, 11, 13, 15, 10, 12,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 13, 0, 13, 13, 13, 15, 12, 12,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 15, 0, 15, 15, 15, 15, 14, 14,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 17, 0, 17, 17, 17, 17, 16, 16,
							16, 16, 18, 19, 17, 0
						},
						{
							1, 19, 18, 0, 18, 18, 18, 18, 18, 18,
							18, 18, 18, 19, 18, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 17, 0, 17, 17, 17, 17, 17, 17,
							17, 17, 18, 19, 17, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						}
					},
					{
						{
							1, 1, 1, 0, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 18, 19, 17, 0
						},
						{
							1, 19, 18, 0, 18, 18, 18, 18, 18, 18,
							18, 18, 18, 19, 18, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 17, 0, 17, 17, 17, 17, 17, 17,
							17, 17, 18, 19, 17, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						}
					},
					{
						{
							1, 1, 1, 0, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						}
					},
					{
						{
							1, 1, 1, 0, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 11, 0, 9, 11, 13, 15, 11, 13,
							15, 15, 15, 15, 15, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 15, 9, 0, 9, 11, 13, 15, 11, 13,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 11, 0, 11, 11, 13, 15, 11, 13,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 13, 0, 13, 13, 13, 15, 13, 13,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 11, 0, 11, 11, 13, 15, 10, 12,
							14, 16, 15, 15, 15, 0
						},
						{
							1, 15, 13, 0, 13, 13, 13, 15, 12, 12,
							14, 16, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 14, 14,
							14, 16, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 16, 16,
							16, 16, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						}
					},
					{
						{
							1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 1
						},
						{
							1, 15, 15, 15, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 15
						},
						{
							1, 11, 11, 11, 11, 11, 11, 11, 11, 11,
							11, 11, 11, 11, 11, 11
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 9, 9, 9, 9, 9, 9, 9, 9, 9,
							9, 9, 9, 9, 9, 9
						},
						{
							1, 11, 11, 11, 11, 11, 11, 11, 11, 11,
							11, 11, 11, 11, 11, 11
						},
						{
							1, 13, 13, 13, 13, 13, 13, 13, 13, 13,
							13, 13, 13, 13, 13, 13
						},
						{
							1, 15, 15, 15, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 15
						},
						{
							1, 10, 10, 10, 10, 10, 10, 10, 10, 10,
							10, 10, 10, 10, 10, 10
						},
						{
							1, 12, 12, 12, 12, 12, 12, 12, 12, 12,
							12, 12, 12, 12, 12, 12
						},
						{
							1, 14, 14, 14, 14, 14, 14, 14, 14, 14,
							14, 14, 14, 14, 14, 14
						},
						{
							1, 16, 16, 16, 16, 16, 16, 16, 16, 16,
							16, 16, 16, 16, 16, 16
						},
						{
							1, 15, 15, 15, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 15
						},
						{
							1, 15, 15, 15, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 15
						},
						{
							1, 15, 15, 15, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 15
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						}
					},
					{
						{
							1, 1, 1, 0, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							1, 7, 7, 0, 7, 7, 7, 7, 7, 7,
							7, 7, 7, 7, 7, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						}
					},
					{
						{
							1, 1, 1, 0, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 0
						},
						{
							1, 15, 7, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 7, 7, 0, 9, 11, 13, 15, 11, 13,
							15, 15, 15, 15, 15, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 15, 9, 0, 9, 11, 13, 15, 11, 13,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 11, 0, 11, 11, 13, 15, 11, 13,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 13, 0, 13, 13, 13, 15, 13, 13,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 11, 0, 11, 11, 13, 15, 10, 12,
							14, 16, 15, 15, 15, 0
						},
						{
							1, 15, 13, 0, 13, 13, 13, 15, 12, 12,
							14, 16, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 14, 14,
							14, 16, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 16, 16,
							16, 16, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							1, 15, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 15, 15, 15, 15, 0
						},
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						}
					},
					{
						{
							1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 1
						},
						{
							1, 20, 7, 20, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 33
						},
						{
							1, 7, 7, 0, 9, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 20, 0, 8, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0
						},
						{
							1, 19, 9, 0, 9, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 11, 0, 11, 11, 13, 15, 11, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 13, 0, 13, 13, 13, 15, 13, 13,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 15, 0, 15, 15, 15, 15, 15, 15,
							15, 17, 18, 19, 17, 0
						},
						{
							1, 19, 11, 0, 11, 11, 13, 15, 10, 12,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 13, 0, 13, 13, 13, 15, 12, 12,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 15, 0, 15, 15, 15, 15, 14, 14,
							14, 16, 18, 19, 17, 0
						},
						{
							1, 19, 17, 0, 17, 17, 17, 17, 16, 16,
							16, 16, 18, 19, 17, 0
						},
						{
							1, 19, 18, 0, 18, 18, 18, 18, 18, 18,
							18, 18, 18, 19, 18, 0
						},
						{
							1, 19, 19, 0, 19, 19, 19, 19, 19, 19,
							19, 19, 19, 19, 19, 0
						},
						{
							1, 19, 17, 0, 17, 17, 17, 17, 17, 17,
							17, 17, 18, 19, 17, 0
						},
						{
							1, 33, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 33
						}
					},
					{
						{
							1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
							1, 1, 1, 1, 1, 1
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						},
						{
							1, 20, 20, 20, 20, 20, 20, 20, 20, 20,
							20, 20, 20, 20, 20, 20
						}
					}
				};
			}
		}

		private sealed class LiftedParameterSymbol : ParameterSymbol
		{
			private readonly ParameterSymbol _parameterToLift;

			private readonly TypeSymbol _type;

			public override string Name => _parameterToLift.Name;

			public override Symbol ContainingSymbol => _parameterToLift.ContainingSymbol;

			public override ImmutableArray<CustomModifier> CustomModifiers => _parameterToLift.CustomModifiers;

			public override ImmutableArray<CustomModifier> RefCustomModifiers => _parameterToLift.RefCustomModifiers;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

			internal override ConstantValue ExplicitDefaultConstantValue => _parameterToLift.get_ExplicitDefaultConstantValue(inProgress);

			public override bool HasExplicitDefaultValue => _parameterToLift.HasExplicitDefaultValue;

			public override bool IsByRef => _parameterToLift.IsByRef;

			internal override bool IsExplicitByRef => _parameterToLift.IsExplicitByRef;

			public override bool IsOptional => _parameterToLift.IsOptional;

			internal override bool IsMetadataOut => _parameterToLift.IsMetadataOut;

			internal override bool IsMetadataIn => _parameterToLift.IsMetadataIn;

			internal override MarshalPseudoCustomAttributeData MarshallingInformation => _parameterToLift.MarshallingInformation;

			internal override bool HasOptionCompare => _parameterToLift.HasOptionCompare;

			internal override bool IsIDispatchConstant => _parameterToLift.IsIDispatchConstant;

			internal override bool IsIUnknownConstant => _parameterToLift.IsIUnknownConstant;

			internal override bool IsCallerLineNumber => _parameterToLift.IsCallerLineNumber;

			internal override bool IsCallerMemberName => _parameterToLift.IsCallerMemberName;

			internal override bool IsCallerFilePath => _parameterToLift.IsCallerFilePath;

			public override bool IsParamArray => _parameterToLift.IsParamArray;

			public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

			public override int Ordinal => _parameterToLift.Ordinal;

			public override TypeSymbol Type => _type;

			public LiftedParameterSymbol(ParameterSymbol parameter, TypeSymbol type)
			{
				_parameterToLift = parameter;
				_type = type;
			}

			internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
			{
				return _parameterToLift.GetUseSiteInfo();
			}
		}

		public abstract class Candidate
		{
			public abstract Symbol UnderlyingSymbol { get; }

			public virtual bool IsExtensionMethod => false;

			public virtual bool IsOperator => false;

			public virtual bool IsLifted => false;

			public virtual int PrecedenceLevel => 0;

			public virtual BitVector FixedTypeParameters => BitVector.Null;

			public abstract bool IsGeneric { get; }

			public abstract int ParameterCount { get; }

			public abstract TypeSymbol ReturnType { get; }

			public abstract int Arity { get; }

			public abstract ImmutableArray<TypeParameterSymbol> TypeParameters { get; }

			public abstract TypeSymbol ReceiverType { get; }

			public abstract TypeSymbol ReceiverTypeDefinition { get; }

			internal abstract Candidate Construct(ImmutableArray<TypeSymbol> typeArguments);

			public abstract ParameterSymbol Parameters(int index);

			internal void GetAllParameterCounts(ref int requiredCount, ref int maxCount, ref bool hasParamArray)
			{
				maxCount = ParameterCount;
				hasParamArray = false;
				requiredCount = -1;
				int num = maxCount - 1;
				int num2 = num;
				for (int i = 0; i <= num2; i++)
				{
					ParameterSymbol parameterSymbol = Parameters(i);
					if (i == num && parameterSymbol.IsParamArray)
					{
						hasParamArray = true;
					}
					else if (!parameterSymbol.IsOptional)
					{
						requiredCount = i;
					}
				}
				requiredCount++;
			}

			internal bool TryGetNamedParamIndex(string name, ref int index)
			{
				int num = ParameterCount - 1;
				for (int i = 0; i <= num; i++)
				{
					ParameterSymbol parameterSymbol = Parameters(i);
					if (CaseInsensitiveComparison.Equals(name, parameterSymbol.Name))
					{
						index = i;
						return true;
					}
				}
				index = -1;
				return false;
			}

			internal abstract bool IsOverriddenBy(Symbol otherSymbol);
		}

		public class MethodCandidate : Candidate
		{
			protected readonly MethodSymbol m_Method;

			public override bool IsGeneric => m_Method.IsGenericMethod;

			public override int ParameterCount => m_Method.ParameterCount;

			public override TypeSymbol ReturnType => m_Method.ReturnType;

			public override TypeSymbol ReceiverType => m_Method.ContainingType;

			public override TypeSymbol ReceiverTypeDefinition => m_Method.ContainingType;

			public override int Arity => m_Method.Arity;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => m_Method.TypeParameters;

			public override Symbol UnderlyingSymbol => m_Method;

			public MethodCandidate(MethodSymbol method)
			{
				m_Method = method;
			}

			internal override Candidate Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				return new MethodCandidate(m_Method.Construct(typeArguments));
			}

			public override ParameterSymbol Parameters(int index)
			{
				return m_Method.Parameters[index];
			}

			internal override bool IsOverriddenBy(Symbol otherSymbol)
			{
				MethodSymbol originalDefinition = m_Method.OriginalDefinition;
				if (originalDefinition.IsOverridable || originalDefinition.IsOverrides || originalDefinition.IsMustOverride)
				{
					MethodSymbol overriddenMethod = ((MethodSymbol)otherSymbol).OverriddenMethod;
					while ((object)overriddenMethod != null)
					{
						if (overriddenMethod.OriginalDefinition.Equals(originalDefinition))
						{
							return true;
						}
						overriddenMethod = overriddenMethod.OverriddenMethod;
					}
				}
				return false;
			}
		}

		public sealed class ExtensionMethodCandidate : MethodCandidate
		{
			private BitVector _fixedTypeParameters;

			public override bool IsExtensionMethod => true;

			public override int PrecedenceLevel => m_Method.Proximity;

			public override BitVector FixedTypeParameters => _fixedTypeParameters;

			public override TypeSymbol ReceiverType => m_Method.ReceiverType;

			public override TypeSymbol ReceiverTypeDefinition => m_Method.ReducedFrom.Parameters[0].Type;

			public ExtensionMethodCandidate(MethodSymbol method)
				: this(method, GetFixedTypeParameters(method))
			{
			}

			private static BitVector GetFixedTypeParameters(MethodSymbol method)
			{
				if (method.FixedTypeParameters.Length > 0)
				{
					BitVector result = BitVector.Create(method.ReducedFrom.Arity);
					ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>>.Enumerator enumerator = method.FixedTypeParameters.GetEnumerator();
					while (enumerator.MoveNext())
					{
						result[enumerator.Current.Key.Ordinal] = true;
					}
					return result;
				}
				return default(BitVector);
			}

			private ExtensionMethodCandidate(MethodSymbol method, BitVector fixedTypeParameters)
				: base(method)
			{
				_fixedTypeParameters = fixedTypeParameters;
			}

			internal override Candidate Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				return new ExtensionMethodCandidate(m_Method.Construct(typeArguments), _fixedTypeParameters);
			}

			internal override bool IsOverriddenBy(Symbol otherSymbol)
			{
				return false;
			}
		}

		public class OperatorCandidate : MethodCandidate
		{
			public sealed override bool IsOperator => true;

			public OperatorCandidate(MethodSymbol method)
				: base(method)
			{
			}
		}

		public class LiftedOperatorCandidate : OperatorCandidate
		{
			private readonly ImmutableArray<ParameterSymbol> _parameters;

			private readonly TypeSymbol _returnType;

			public override int ParameterCount => _parameters.Length;

			public override TypeSymbol ReturnType => _returnType;

			public override bool IsLifted => true;

			public LiftedOperatorCandidate(MethodSymbol method, ImmutableArray<ParameterSymbol> parameters, TypeSymbol returnType)
				: base(method)
			{
				_parameters = parameters;
				_returnType = returnType;
			}

			public override ParameterSymbol Parameters(int index)
			{
				return _parameters[index];
			}
		}

		public sealed class PropertyCandidate : Candidate
		{
			private readonly PropertySymbol _property;

			public override bool IsGeneric => false;

			public override int ParameterCount => _property.Parameters.Length;

			public override TypeSymbol ReturnType => _property.Type;

			public override TypeSymbol ReceiverType => _property.ContainingType;

			public override TypeSymbol ReceiverTypeDefinition => _property.ContainingType;

			public override int Arity => 0;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

			public override Symbol UnderlyingSymbol => _property;

			public PropertyCandidate(PropertySymbol property)
			{
				_property = property;
			}

			internal override Candidate Construct(ImmutableArray<TypeSymbol> typeArguments)
			{
				throw ExceptionUtilities.Unreachable;
			}

			public override ParameterSymbol Parameters(int index)
			{
				return _property.Parameters[index];
			}

			internal override bool IsOverriddenBy(Symbol otherSymbol)
			{
				PropertySymbol originalDefinition = _property.OriginalDefinition;
				if (originalDefinition.IsOverridable || originalDefinition.IsOverrides || originalDefinition.IsMustOverride)
				{
					PropertySymbol overriddenProperty = ((PropertySymbol)otherSymbol).OverriddenProperty;
					while ((object)overriddenProperty != null)
					{
						if (overriddenProperty.OriginalDefinition.Equals(originalDefinition))
						{
							return true;
						}
						overriddenProperty = overriddenProperty.OverriddenProperty;
					}
				}
				return false;
			}
		}

		public enum CandidateAnalysisResultState : byte
		{
			Applicable,
			HasUnsupportedMetadata,
			HasUseSiteError,
			Ambiguous,
			BadGenericArity,
			ArgumentCountMismatch,
			TypeInferenceFailed,
			ArgumentMismatch,
			GenericConstraintsViolated,
			RequiresNarrowing,
			RequiresNarrowingNotFromObject,
			ExtensionMethodVsInstanceMethod,
			Shadowed,
			LessApplicable,
			ExtensionMethodVsLateBinding,
			Count
		}

		[Flags]
		private enum SmallFieldMask
		{
			State = 0xFF,
			IsExpandedParamArrayForm = 0x100,
			InferenceLevelShift = 9,
			InferenceLevelMask = 0x600,
			ArgumentMatchingDone = 0x800,
			RequiresNarrowingConversion = 0x1000,
			RequiresNarrowingNotFromObject = 0x2000,
			RequiresNarrowingNotFromNumericConstant = 0x4000,
			DelegateRelaxationLevelMask = 0x38000,
			SomeInferenceFailed = 0x40000,
			AllFailedInferenceIsDueToObject = 0x80000,
			InferenceErrorReasonsShift = 0x14,
			InferenceErrorReasonsMask = 0x300000,
			IgnoreExtensionMethods = 0x400000,
			IllegalInAttribute = 0x800000
		}

		public struct OptionalArgument
		{
			public readonly BoundExpression DefaultValue;

			public readonly KeyValuePair<ConversionKind, MethodSymbol> Conversion;

			public readonly ImmutableArray<AssemblySymbol> Dependencies;

			public OptionalArgument(BoundExpression value, KeyValuePair<ConversionKind, MethodSymbol> conversion, ImmutableArray<AssemblySymbol> dependencies)
			{
				this = default(OptionalArgument);
				DefaultValue = value;
				Conversion = conversion;
				Dependencies = dependencies.NullToEmpty();
			}
		}

		public struct CandidateAnalysisResult
		{
			private int _smallFields;

			public Candidate Candidate;

			public int ExpandedParamArrayArgumentsUsed;

			public int EquallyApplicableCandidatesBucket;

			public ImmutableArray<int> ArgsToParamsOpt;

			public ImmutableArray<KeyValuePair<ConversionKind, MethodSymbol>> ConversionsOpt;

			public ImmutableArray<KeyValuePair<ConversionKind, MethodSymbol>> ConversionsBackOpt;

			public ImmutableArray<OptionalArgument> OptionalArguments;

			public BitVector NotInferredTypeArguments;

			public BindingDiagnosticBag TypeArgumentInferenceDiagnosticsOpt;

			public bool IsExpandedParamArrayForm => (_smallFields & 0x100) != 0;

			public TypeArgumentInference.InferenceLevel InferenceLevel => (TypeArgumentInference.InferenceLevel)((_smallFields & 0x600) >> 9);

			public bool ArgumentMatchingDone => (_smallFields & 0x800) != 0;

			public bool RequiresNarrowingConversion => (_smallFields & 0x1000) != 0;

			public bool RequiresNarrowingNotFromObject => (_smallFields & 0x2000) != 0;

			public bool RequiresNarrowingNotFromNumericConstant => (_smallFields & 0x4000) != 0;

			public ConversionKind MaxDelegateRelaxationLevel => (ConversionKind)(_smallFields & 0x38000);

			public bool SomeInferenceFailed => (_smallFields & 0x40000) != 0;

			public bool IsIllegalInAttribute => (_smallFields & 0x800000) != 0;

			public bool AllFailedInferenceIsDueToObject => (_smallFields & 0x80000) != 0;

			public InferenceErrorReasons InferenceErrorReasons => (InferenceErrorReasons)((_smallFields & 0x300000) >> 20);

			public CandidateAnalysisResultState State
			{
				get
				{
					return (CandidateAnalysisResultState)((uint)_smallFields & 0xFFu);
				}
				set
				{
					int num = _smallFields & -256;
					num = (_smallFields = num | (int)value);
				}
			}

			public bool IgnoreExtensionMethods
			{
				get
				{
					return (_smallFields & 0x400000) != 0;
				}
				set
				{
					if (value)
					{
						_smallFields |= 4194304;
					}
					else
					{
						_smallFields &= -4194305;
					}
				}
			}

			public bool UsedOptionalParameterDefaultValue => !OptionalArguments.IsDefault;

			public void SetIsExpandedParamArrayForm()
			{
				_smallFields |= 256;
			}

			public void SetInferenceLevel(TypeArgumentInference.InferenceLevel level)
			{
				int num = (int)((uint)level << 9);
				_smallFields = (_smallFields & -1537) | (num & 0x600);
			}

			public void SetArgumentMatchingDone()
			{
				_smallFields |= 2048;
			}

			public void SetRequiresNarrowingConversion()
			{
				_smallFields |= 4096;
			}

			public void SetRequiresNarrowingNotFromObject()
			{
				_smallFields |= 8192;
			}

			public void SetRequiresNarrowingNotFromNumericConstant()
			{
				IgnoreExtensionMethods = false;
				_smallFields |= 16384;
			}

			public void RegisterDelegateRelaxationLevel(ConversionKind conversionKind)
			{
				int num = (int)(conversionKind & ConversionKind.DelegateRelaxationLevelMask);
				if (num > (_smallFields & 0x38000))
				{
					if (num == 131072)
					{
						IgnoreExtensionMethods = false;
					}
					_smallFields = (_smallFields & -229377) | num;
				}
			}

			public void SetSomeInferenceFailed()
			{
				_smallFields |= 262144;
			}

			public void SetIllegalInAttribute()
			{
				_smallFields |= 8388608;
			}

			public void SetAllFailedInferenceIsDueToObject()
			{
				_smallFields |= 524288;
			}

			public void SetInferenceErrorReasons(InferenceErrorReasons reasons)
			{
				int num = (int)((uint)reasons << 20);
				_smallFields = (_smallFields & -3145729) | (num & 0x300000);
			}

			public CandidateAnalysisResult(Candidate candidate, CandidateAnalysisResultState state)
			{
				this = default(CandidateAnalysisResult);
				Candidate = candidate;
				State = state;
			}

			public CandidateAnalysisResult(Candidate candidate)
			{
				this = default(CandidateAnalysisResult);
				Candidate = candidate;
				State = CandidateAnalysisResultState.Applicable;
			}
		}

		internal struct OverloadResolutionResult
		{
			private readonly CandidateAnalysisResult? _bestResult;

			private readonly ImmutableArray<CandidateAnalysisResult> _allResults;

			private readonly bool _resolutionIsLateBound;

			private readonly bool _remainingCandidatesRequireNarrowingConversion;

			public readonly ImmutableArray<BoundExpression> AsyncLambdaSubToFunctionMismatch;

			public ImmutableArray<CandidateAnalysisResult> Candidates => _allResults;

			public CandidateAnalysisResult? BestResult => _bestResult;

			public bool ResolutionIsLateBound => _resolutionIsLateBound;

			public bool RemainingCandidatesRequireNarrowingConversion => _remainingCandidatesRequireNarrowingConversion;

			public OverloadResolutionResult(ImmutableArray<CandidateAnalysisResult> allResults, bool resolutionIsLateBound, bool remainingCandidatesRequireNarrowingConversion, HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch)
			{
				this = default(OverloadResolutionResult);
				_allResults = allResults;
				_resolutionIsLateBound = resolutionIsLateBound;
				_remainingCandidatesRequireNarrowingConversion = remainingCandidatesRequireNarrowingConversion;
				AsyncLambdaSubToFunctionMismatch = asyncLambdaSubToFunctionMismatch?.ToArray().AsImmutableOrNull() ?? ImmutableArray<BoundExpression>.Empty;
				if (!resolutionIsLateBound)
				{
					_bestResult = GetBestResult(allResults);
				}
			}

			private static CandidateAnalysisResult? GetBestResult(ImmutableArray<CandidateAnalysisResult> allResults)
			{
				CandidateAnalysisResult? result = null;
				for (int i = 0; i < allResults.Length; i++)
				{
					CandidateAnalysisResult value = allResults[i];
					if (value.State == CandidateAnalysisResultState.Applicable)
					{
						if (result.HasValue)
						{
							return null;
						}
						result = value;
					}
				}
				return result;
			}
		}

		private class InferenceLevelComparer : IComparer<int>
		{
			private readonly ArrayBuilder<CandidateAnalysisResult> _candidates;

			public InferenceLevelComparer(ArrayBuilder<CandidateAnalysisResult> candidates)
			{
				_candidates = candidates;
			}

			public int Compare(int indexX, int indexY)
			{
				return ((int)_candidates[indexX].InferenceLevel).CompareTo((int)_candidates[indexY].InferenceLevel);
			}

			int IComparer<int>.Compare(int indexX, int indexY)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Compare
				return this.Compare(indexX, indexY);
			}
		}

		private enum ApplicabilityComparisonResult
		{
			Undefined,
			EquallyApplicable,
			LeftIsMoreApplicable,
			RightIsMoreApplicable
		}

		private struct QuickApplicabilityInfo
		{
			public readonly Candidate Candidate;

			public readonly CandidateAnalysisResultState State;

			public readonly bool AppliesToNormalForm;

			public readonly bool AppliesToParamArrayForm;

			public QuickApplicabilityInfo(Candidate candidate, CandidateAnalysisResultState state, bool appliesToNormalForm = true, bool appliesToParamArrayForm = true)
			{
				this = default(QuickApplicabilityInfo);
				Candidate = candidate;
				State = state;
				AppliesToNormalForm = appliesToNormalForm;
				AppliesToParamArrayForm = appliesToParamArrayForm;
			}
		}

		[Flags]
		private enum TypeParameterKind
		{
			None = 0,
			Method = 1,
			Type = 2,
			Both = 3
		}

		private static readonly Dictionary<string, OperatorInfo> s_operatorNames;

		private const int s_stateSize = 8;

		internal static OperatorInfo GetOperatorInfo(string name)
		{
			OperatorInfo value = default(OperatorInfo);
			if (name.Length > 3 && CaseInsensitiveComparison.Equals("op_", name.Substring(0, 3)) && s_operatorNames.TryGetValue(name, out value))
			{
				return value;
			}
			return default(OperatorInfo);
		}

		static OverloadResolution()
		{
			s_operatorNames = new Dictionary<string, OperatorInfo>(CaseInsensitiveComparison.Comparer)
			{
				{
					"op_OnesComplement",
					new OperatorInfo(UnaryOperatorKind.Not)
				},
				{
					"op_True",
					new OperatorInfo(UnaryOperatorKind.IsTrue)
				},
				{
					"op_False",
					new OperatorInfo(UnaryOperatorKind.IsFalse)
				},
				{
					"op_UnaryPlus",
					new OperatorInfo(UnaryOperatorKind.Plus)
				},
				{
					"op_Addition",
					new OperatorInfo(BinaryOperatorKind.Add)
				},
				{
					"op_UnaryNegation",
					new OperatorInfo(UnaryOperatorKind.Minus)
				},
				{
					"op_Subtraction",
					new OperatorInfo(BinaryOperatorKind.Subtract)
				},
				{
					"op_Multiply",
					new OperatorInfo(BinaryOperatorKind.Multiply)
				},
				{
					"op_Division",
					new OperatorInfo(BinaryOperatorKind.Divide)
				},
				{
					"op_IntegerDivision",
					new OperatorInfo(BinaryOperatorKind.IntegerDivide)
				},
				{
					"op_Modulus",
					new OperatorInfo(BinaryOperatorKind.Modulo)
				},
				{
					"op_Exponent",
					new OperatorInfo(BinaryOperatorKind.Power)
				},
				{
					"op_Equality",
					new OperatorInfo(BinaryOperatorKind.Equals)
				},
				{
					"op_Inequality",
					new OperatorInfo(BinaryOperatorKind.NotEquals)
				},
				{
					"op_LessThan",
					new OperatorInfo(BinaryOperatorKind.LessThan)
				},
				{
					"op_GreaterThan",
					new OperatorInfo(BinaryOperatorKind.GreaterThan)
				},
				{
					"op_LessThanOrEqual",
					new OperatorInfo(BinaryOperatorKind.LessThanOrEqual)
				},
				{
					"op_GreaterThanOrEqual",
					new OperatorInfo(BinaryOperatorKind.GreaterThanOrEqual)
				},
				{
					"op_Like",
					new OperatorInfo(BinaryOperatorKind.Like)
				},
				{
					"op_Concatenate",
					new OperatorInfo(BinaryOperatorKind.Concatenate)
				},
				{
					"op_BitwiseAnd",
					new OperatorInfo(BinaryOperatorKind.And)
				},
				{
					"op_BitwiseOr",
					new OperatorInfo(BinaryOperatorKind.Or)
				},
				{
					"op_ExclusiveOr",
					new OperatorInfo(BinaryOperatorKind.Xor)
				},
				{
					"op_LeftShift",
					new OperatorInfo(BinaryOperatorKind.LeftShift)
				},
				{
					"op_RightShift",
					new OperatorInfo(BinaryOperatorKind.RightShift)
				},
				{
					"op_Implicit",
					new OperatorInfo(UnaryOperatorKind.Implicit)
				},
				{
					"op_Explicit",
					new OperatorInfo(UnaryOperatorKind.Explicit)
				},
				{
					"op_LogicalNot",
					new OperatorInfo(UnaryOperatorKind.Not)
				},
				{
					"op_LogicalAnd",
					new OperatorInfo(BinaryOperatorKind.And)
				},
				{
					"op_LogicalOr",
					new OperatorInfo(BinaryOperatorKind.Or)
				},
				{
					"op_UnsignedLeftShift",
					new OperatorInfo(BinaryOperatorKind.LeftShift)
				},
				{
					"op_UnsignedRightShift",
					new OperatorInfo(BinaryOperatorKind.RightShift)
				}
			};
		}

		internal static SyntaxKind GetOperatorTokenKind(string name)
		{
			return GetOperatorTokenKind(GetOperatorInfo(name));
		}

		internal static SyntaxKind GetOperatorTokenKind(OperatorInfo opInfo)
		{
			if (opInfo.IsUnary)
			{
				return GetOperatorTokenKind(opInfo.UnaryOperatorKind);
			}
			return GetOperatorTokenKind(opInfo.BinaryOperatorKind);
		}

		internal static SyntaxKind GetOperatorTokenKind(UnaryOperatorKind op)
		{
			switch (op)
			{
			case UnaryOperatorKind.IsFalse:
				return SyntaxKind.IsFalseKeyword;
			case UnaryOperatorKind.IsTrue:
				return SyntaxKind.IsTrueKeyword;
			case UnaryOperatorKind.Minus:
				return SyntaxKind.MinusToken;
			case UnaryOperatorKind.Not:
				return SyntaxKind.NotKeyword;
			case UnaryOperatorKind.Plus:
				return SyntaxKind.PlusToken;
			case UnaryOperatorKind.Implicit:
			case UnaryOperatorKind.Explicit:
				return SyntaxKind.CTypeKeyword;
			default:
				throw ExceptionUtilities.UnexpectedValue(op);
			}
		}

		internal static SyntaxKind GetOperatorTokenKind(BinaryOperatorKind op)
		{
			return op switch
			{
				BinaryOperatorKind.Add => SyntaxKind.PlusToken, 
				BinaryOperatorKind.Subtract => SyntaxKind.MinusToken, 
				BinaryOperatorKind.Multiply => SyntaxKind.AsteriskToken, 
				BinaryOperatorKind.Divide => SyntaxKind.SlashToken, 
				BinaryOperatorKind.IntegerDivide => SyntaxKind.BackslashToken, 
				BinaryOperatorKind.Modulo => SyntaxKind.ModKeyword, 
				BinaryOperatorKind.Power => SyntaxKind.CaretToken, 
				BinaryOperatorKind.Equals => SyntaxKind.EqualsToken, 
				BinaryOperatorKind.NotEquals => SyntaxKind.LessThanGreaterThanToken, 
				BinaryOperatorKind.LessThan => SyntaxKind.LessThanToken, 
				BinaryOperatorKind.GreaterThan => SyntaxKind.GreaterThanToken, 
				BinaryOperatorKind.LessThanOrEqual => SyntaxKind.LessThanEqualsToken, 
				BinaryOperatorKind.GreaterThanOrEqual => SyntaxKind.GreaterThanEqualsToken, 
				BinaryOperatorKind.Like => SyntaxKind.LikeKeyword, 
				BinaryOperatorKind.Concatenate => SyntaxKind.AmpersandToken, 
				BinaryOperatorKind.And => SyntaxKind.AndKeyword, 
				BinaryOperatorKind.Or => SyntaxKind.OrKeyword, 
				BinaryOperatorKind.Xor => SyntaxKind.XorKeyword, 
				BinaryOperatorKind.LeftShift => SyntaxKind.LessThanLessThanToken, 
				BinaryOperatorKind.RightShift => SyntaxKind.GreaterThanGreaterThanToken, 
				BinaryOperatorKind.AndAlso => SyntaxKind.AndAlsoKeyword, 
				BinaryOperatorKind.OrElse => SyntaxKind.OrElseKeyword, 
				BinaryOperatorKind.Is => SyntaxKind.IsKeyword, 
				BinaryOperatorKind.IsNot => SyntaxKind.IsNotKeyword, 
				_ => throw ExceptionUtilities.UnexpectedValue(op), 
			};
		}

		internal static string TryGetOperatorName(BinaryOperatorKind op)
		{
			return (op & BinaryOperatorKind.OpMask) switch
			{
				BinaryOperatorKind.Add => "op_Addition", 
				BinaryOperatorKind.Concatenate => "op_Concatenate", 
				BinaryOperatorKind.Like => "op_Like", 
				BinaryOperatorKind.Equals => "op_Equality", 
				BinaryOperatorKind.NotEquals => "op_Inequality", 
				BinaryOperatorKind.LessThanOrEqual => "op_LessThanOrEqual", 
				BinaryOperatorKind.GreaterThanOrEqual => "op_GreaterThanOrEqual", 
				BinaryOperatorKind.LessThan => "op_LessThan", 
				BinaryOperatorKind.GreaterThan => "op_GreaterThan", 
				BinaryOperatorKind.Subtract => "op_Subtraction", 
				BinaryOperatorKind.Multiply => "op_Multiply", 
				BinaryOperatorKind.Power => "op_Exponent", 
				BinaryOperatorKind.Divide => "op_Division", 
				BinaryOperatorKind.Modulo => "op_Modulus", 
				BinaryOperatorKind.IntegerDivide => "op_IntegerDivision", 
				BinaryOperatorKind.LeftShift => "op_LeftShift", 
				BinaryOperatorKind.RightShift => "op_RightShift", 
				BinaryOperatorKind.Xor => "op_ExclusiveOr", 
				BinaryOperatorKind.Or => "op_BitwiseOr", 
				BinaryOperatorKind.And => "op_BitwiseAnd", 
				_ => null, 
			};
		}

		internal static string TryGetOperatorName(UnaryOperatorKind op)
		{
			return (op & UnaryOperatorKind.OpMask) switch
			{
				UnaryOperatorKind.Plus => "op_UnaryPlus", 
				UnaryOperatorKind.Minus => "op_UnaryNegation", 
				UnaryOperatorKind.Not => "op_OnesComplement", 
				UnaryOperatorKind.Implicit => "op_Implicit", 
				UnaryOperatorKind.Explicit => "op_Explicit", 
				UnaryOperatorKind.IsTrue => "op_True", 
				UnaryOperatorKind.IsFalse => "op_False", 
				_ => null, 
			};
		}

		internal static bool ValidateOverloadedOperator(MethodSymbol method, OperatorInfo opInfo)
		{
			return ValidateOverloadedOperator(method, opInfo, null, null);
		}

		internal static bool ValidateOverloadedOperator(MethodSymbol method, OperatorInfo opInfo, BindingDiagnosticBag diagnosticsOpt, AssemblySymbol assemblyBeingBuiltOpt)
		{
			if (method.ParameterCount != opInfo.ParamCount)
			{
				return false;
			}
			bool flag = true;
			NamedTypeSymbol containingType = method.ContainingType;
			bool flag2 = false;
			bool flag3 = false;
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = ((diagnosticsOpt != null) ? new CompoundUseSiteInfo<AssemblySymbol>(diagnosticsOpt, assemblyBeingBuiltOpt) : CompoundUseSiteInfo<AssemblySymbol>.Discarded);
			ERRID id;
			if (opInfo.IsUnary)
			{
				UnaryOperatorKind unaryOperatorKind = opInfo.UnaryOperatorKind;
				if (unaryOperatorKind == UnaryOperatorKind.Implicit || unaryOperatorKind == UnaryOperatorKind.Explicit)
				{
					flag3 = true;
					id = ERRID.ERR_ConvParamMustBeContainingType1;
					if (OverloadedOperatorTargetsContainingType(containingType, method.ReturnType))
					{
						flag2 = true;
					}
				}
				else
				{
					id = ERRID.ERR_UnaryParamMustBeContainingType1;
					if (!TypeSymbolExtensions.IsBooleanType(method.ReturnType))
					{
						switch (opInfo.UnaryOperatorKind)
						{
						case UnaryOperatorKind.IsTrue:
							if (diagnosticsOpt != null)
							{
								diagnosticsOpt.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OperatorRequiresBoolReturnType1, SyntaxFacts.GetText(SyntaxKind.IsTrueKeyword)), method.Locations[0]);
								flag = false;
								break;
							}
							return false;
						case UnaryOperatorKind.IsFalse:
							if (diagnosticsOpt != null)
							{
								diagnosticsOpt.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OperatorRequiresBoolReturnType1, SyntaxFacts.GetText(SyntaxKind.IsFalseKeyword)), method.Locations[0]);
								flag = false;
								break;
							}
							return false;
						}
					}
				}
			}
			else
			{
				id = ERRID.ERR_BinaryParamMustBeContainingType1;
				BinaryOperatorKind binaryOperatorKind = opInfo.BinaryOperatorKind;
				if ((uint)(binaryOperatorKind - 16) <= 1u && TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(method.Parameters[1].Type).SpecialType != SpecialType.System_Int32)
				{
					if (diagnosticsOpt == null)
					{
						return false;
					}
					diagnosticsOpt.Add(ErrorFactory.ErrorInfo(ERRID.ERR_OperatorRequiresIntegerParameter1, SyntaxFacts.GetText((opInfo.BinaryOperatorKind == BinaryOperatorKind.LeftShift) ? SyntaxKind.LessThanLessThanToken : SyntaxKind.GreaterThanGreaterThanToken)), method.Locations[0]);
					flag = false;
				}
			}
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = method.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				if (OverloadedOperatorTargetsContainingType(containingType, current.Type))
				{
					flag2 = true;
				}
				if (current.IsByRef)
				{
					if (diagnosticsOpt == null)
					{
						return false;
					}
					flag = false;
				}
			}
			if (!flag2)
			{
				if (diagnosticsOpt == null)
				{
					return false;
				}
				diagnosticsOpt.Add(ErrorFactory.ErrorInfo(id, method.ContainingSymbol), method.Locations[0]);
				flag = false;
			}
			else if (flag3)
			{
				TypeSymbol type = method.Parameters[0].Type;
				TypeSymbol returnType = method.ReturnType;
				if (TypeSymbolExtensions.IsObjectType(type))
				{
					if (diagnosticsOpt == null)
					{
						return false;
					}
					diagnosticsOpt.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ConversionFromObject), method.Locations[0]);
					flag = false;
				}
				else if (TypeSymbolExtensions.IsObjectType(returnType))
				{
					if (diagnosticsOpt == null)
					{
						return false;
					}
					diagnosticsOpt.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ConversionToObject), method.Locations[0]);
					flag = false;
				}
				else if (TypeSymbolExtensions.IsInterfaceType(type))
				{
					if (diagnosticsOpt == null)
					{
						return false;
					}
					diagnosticsOpt.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ConversionFromInterfaceType), method.Locations[0]);
					flag = false;
				}
				else if (TypeSymbolExtensions.IsInterfaceType(returnType))
				{
					if (diagnosticsOpt == null)
					{
						return false;
					}
					diagnosticsOpt.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ConversionToInterfaceType), method.Locations[0]);
					flag = false;
				}
				else if ((containingType.SpecialType == SpecialType.System_Nullable_T) ? ((object)type == returnType) : ((object)TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type) == TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(returnType)))
				{
					if (diagnosticsOpt == null)
					{
						return false;
					}
					diagnosticsOpt.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ConversionToSameType), method.Locations[0]);
					flag = false;
				}
				else if ((type.Kind == SymbolKind.NamedType || type.Kind == SymbolKind.TypeParameter) && (returnType.Kind == SymbolKind.NamedType || returnType.Kind == SymbolKind.TypeParameter))
				{
					if (Conversions.HasWideningDirectCastConversionButNotEnumTypeConversion(returnType, type, ref useSiteInfo))
					{
						if (diagnosticsOpt == null)
						{
							return false;
						}
						diagnosticsOpt.Add(ErrorFactory.ErrorInfo(((object)returnType == method.ContainingSymbol) ? ERRID.ERR_ConversionFromBaseType : ERRID.ERR_ConversionToDerivedType), method.Locations[0]);
						flag = false;
					}
					else if (Conversions.HasWideningDirectCastConversionButNotEnumTypeConversion(type, returnType, ref useSiteInfo))
					{
						if (diagnosticsOpt == null)
						{
							return false;
						}
						diagnosticsOpt.Add(ErrorFactory.ErrorInfo(((object)returnType == method.ContainingSymbol) ? ERRID.ERR_ConversionFromDerivedType : ERRID.ERR_ConversionToBaseType), method.Locations[0]);
						flag = false;
					}
				}
			}
			if (flag)
			{
				diagnosticsOpt?.Add(method.Locations[0], useSiteInfo);
			}
			return flag;
		}

		private static bool OverloadedOperatorTargetsContainingType(NamedTypeSymbol containingType, TypeSymbol typeFromSignature)
		{
			if (containingType.SpecialType == SpecialType.System_Nullable_T)
			{
				return (object)typeFromSignature == containingType;
			}
			return TypeSymbol.Equals(TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(typeFromSignature)), TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(containingType), TypeCompareKind.ConsiderEverything);
		}

		public static UnaryOperatorKind MapUnaryOperatorKind(SyntaxKind opCode)
		{
			return opCode switch
			{
				SyntaxKind.UnaryPlusExpression => UnaryOperatorKind.Plus, 
				SyntaxKind.UnaryMinusExpression => UnaryOperatorKind.Minus, 
				SyntaxKind.NotExpression => UnaryOperatorKind.Not, 
				_ => throw ExceptionUtilities.UnexpectedValue(opCode), 
			};
		}

		public static UnaryOperatorKind ResolveUnaryOperator(UnaryOperatorKind opCode, BoundExpression operand, Binder binder, out SpecialType intrinsicOperatorType, out OverloadResolutionResult userDefinedOperator, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			opCode &= UnaryOperatorKind.Not;
			intrinsicOperatorType = SpecialType.None;
			userDefinedOperator = default(OverloadResolutionResult);
			TypeSymbol type = operand.Type;
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type);
			bool flag = (object)type != nullableUnderlyingTypeOrSelf;
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf);
			bool flag2 = (object)enumUnderlyingTypeOrSelf != nullableUnderlyingTypeOrSelf;
			if ((flag2 || flag) && (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsObjectType(enumUnderlyingTypeOrSelf)))
			{
				return UnaryOperatorKind.Error;
			}
			TypeSymbol typeSymbol = enumUnderlyingTypeOrSelf;
			if (typeSymbol.SpecialType != SpecialType.System_Object && !TypeSymbolExtensions.IsIntrinsicType(typeSymbol))
			{
				if (TypeSymbolExtensions.CanContainUserDefinedOperators(type, ref useSiteInfo))
				{
					userDefinedOperator = ResolveUserDefinedUnaryOperator(operand, opCode, binder, ref useSiteInfo);
					if (!userDefinedOperator.BestResult.HasValue && userDefinedOperator.Candidates.Length == 0)
					{
						userDefinedOperator = ResolveUserDefinedUnaryOperator(operand, opCode, binder, ref useSiteInfo, includeEliminatedCandidates: true);
						if (userDefinedOperator.Candidates.Length == 0)
						{
							return UnaryOperatorKind.Error;
						}
					}
					return UnaryOperatorKind.UserDefined;
				}
				return UnaryOperatorKind.Error;
			}
			UnaryOperatorKind unaryOperatorKind = UnaryOperatorKind.Error;
			if (flag2 && opCode == UnaryOperatorKind.Not && TypeSymbolExtensions.IsIntegralType(typeSymbol))
			{
				unaryOperatorKind = UnaryOperatorKind.Not;
			}
			else
			{
				intrinsicOperatorType = ResolveNotLiftedIntrinsicUnaryOperator(opCode, typeSymbol.SpecialType);
				if (intrinsicOperatorType != 0)
				{
					unaryOperatorKind = opCode;
				}
			}
			if (unaryOperatorKind != UnaryOperatorKind.Error && flag)
			{
				unaryOperatorKind |= UnaryOperatorKind.Lifted;
			}
			return unaryOperatorKind;
		}

		internal static SpecialType ResolveNotLiftedIntrinsicUnaryOperator(UnaryOperatorKind opCode, SpecialType operandSpecialType)
		{
			switch (opCode)
			{
			case UnaryOperatorKind.Not:
				switch (operandSpecialType)
				{
				case SpecialType.System_Object:
				case SpecialType.System_Boolean:
				case SpecialType.System_SByte:
				case SpecialType.System_Byte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
				case SpecialType.System_UInt32:
				case SpecialType.System_Int64:
				case SpecialType.System_UInt64:
					return operandSpecialType;
				case SpecialType.System_Decimal:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
				case SpecialType.System_String:
					return SpecialType.System_Int64;
				default:
					return SpecialType.None;
				}
			case UnaryOperatorKind.Plus:
				switch (operandSpecialType)
				{
				case SpecialType.System_Boolean:
					return SpecialType.System_Int16;
				case SpecialType.System_Object:
				case SpecialType.System_SByte:
				case SpecialType.System_Byte:
				case SpecialType.System_Int16:
				case SpecialType.System_UInt16:
				case SpecialType.System_Int32:
				case SpecialType.System_UInt32:
				case SpecialType.System_Int64:
				case SpecialType.System_UInt64:
				case SpecialType.System_Decimal:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
					return operandSpecialType;
				case SpecialType.System_String:
					return SpecialType.System_Double;
				default:
					return SpecialType.None;
				}
			case UnaryOperatorKind.Minus:
				switch (operandSpecialType)
				{
				case SpecialType.System_Boolean:
				case SpecialType.System_Byte:
					return SpecialType.System_Int16;
				case SpecialType.System_Object:
				case SpecialType.System_SByte:
				case SpecialType.System_Int16:
				case SpecialType.System_Int32:
				case SpecialType.System_Int64:
				case SpecialType.System_Decimal:
				case SpecialType.System_Single:
				case SpecialType.System_Double:
					return operandSpecialType;
				case SpecialType.System_UInt16:
					return SpecialType.System_Int32;
				case SpecialType.System_UInt32:
					return SpecialType.System_Int64;
				case SpecialType.System_UInt64:
					return SpecialType.System_Decimal;
				case SpecialType.System_String:
					return SpecialType.System_Double;
				default:
					return SpecialType.None;
				}
			default:
				throw ExceptionUtilities.UnexpectedValue(opCode);
			}
		}

		public static ConstantValue TryFoldConstantUnaryOperator(UnaryOperatorKind op, BoundExpression operand, TypeSymbol resultType, ref bool integerOverflow)
		{
			integerOverflow = false;
			ConstantValue value = operand.ConstantValueOpt;
			if ((object)value == null || value.IsBad || TypeSymbolExtensions.IsErrorType(resultType))
			{
				return null;
			}
			TypeSymbol type = operand.Type;
			ConstantValue result = null;
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(resultType);
			if (TypeSymbolExtensions.AllowsCompileTimeOperations(type) && TypeSymbolExtensions.AllowsCompileTimeOperations(enumUnderlyingTypeOrSelf))
			{
				if (TypeSymbolExtensions.IsIntegralType(enumUnderlyingTypeOrSelf))
				{
					long num = CompileTimeCalculations.GetConstantValueAsInt64(ref value);
					switch (op & UnaryOperatorKind.Not)
					{
					case UnaryOperatorKind.Minus:
						if (num == long.MinValue)
						{
							integerOverflow = true;
						}
						num = -num;
						break;
					case UnaryOperatorKind.Not:
						num = ~num;
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(op);
					case UnaryOperatorKind.Plus:
						break;
					}
					bool flag = integerOverflow;
					ConstantValueTypeDiscriminator constantValueTypeDiscriminator = TypeSymbolExtensions.GetConstantValueTypeDiscriminator(enumUnderlyingTypeOrSelf);
					result = CompileTimeCalculations.GetConstantValue(constantValueTypeDiscriminator, CompileTimeCalculations.NarrowIntegralResult(num, constantValueTypeDiscriminator, constantValueTypeDiscriminator, ref integerOverflow));
					integerOverflow = (op & UnaryOperatorKind.Not) == UnaryOperatorKind.Minus && (integerOverflow || flag);
				}
				else if (TypeSymbolExtensions.IsFloatingType(enumUnderlyingTypeOrSelf))
				{
					double num2 = (TypeSymbolExtensions.IsSingleType(enumUnderlyingTypeOrSelf) ? ((double)value.SingleValue) : value.DoubleValue);
					switch (op & UnaryOperatorKind.Not)
					{
					case UnaryOperatorKind.Minus:
						num2 = 0.0 - num2;
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(op);
					case UnaryOperatorKind.Plus:
						break;
					}
					bool overflow = false;
					num2 = CompileTimeCalculations.NarrowFloatingResult(num2, TypeSymbolExtensions.GetConstantValueTypeDiscriminator(enumUnderlyingTypeOrSelf), ref overflow);
					result = ((!TypeSymbolExtensions.IsSingleType(enumUnderlyingTypeOrSelf)) ? ConstantValue.Create(num2) : ConstantValue.Create((float)num2));
				}
				else if (TypeSymbolExtensions.IsDecimalType(enumUnderlyingTypeOrSelf))
				{
					decimal num3 = value.DecimalValue;
					switch (op & UnaryOperatorKind.Not)
					{
					case UnaryOperatorKind.Minus:
						num3 = decimal.Negate(num3);
						break;
					default:
						throw ExceptionUtilities.UnexpectedValue(op);
					case UnaryOperatorKind.Plus:
						break;
					}
					result = ConstantValue.Create(num3);
				}
				else if (TypeSymbolExtensions.IsBooleanType(enumUnderlyingTypeOrSelf))
				{
					result = ConstantValue.Create(!value.BooleanValue);
				}
			}
			return result;
		}

		public static BinaryOperatorKind MapBinaryOperatorKind(SyntaxKind opCode)
		{
			return opCode switch
			{
				SyntaxKind.AddExpression => BinaryOperatorKind.Add, 
				SyntaxKind.ConcatenateExpression => BinaryOperatorKind.Concatenate, 
				SyntaxKind.LikeExpression => BinaryOperatorKind.Like, 
				SyntaxKind.EqualsExpression => BinaryOperatorKind.Equals, 
				SyntaxKind.NotEqualsExpression => BinaryOperatorKind.NotEquals, 
				SyntaxKind.LessThanOrEqualExpression => BinaryOperatorKind.LessThanOrEqual, 
				SyntaxKind.GreaterThanOrEqualExpression => BinaryOperatorKind.GreaterThanOrEqual, 
				SyntaxKind.LessThanExpression => BinaryOperatorKind.LessThan, 
				SyntaxKind.GreaterThanExpression => BinaryOperatorKind.GreaterThan, 
				SyntaxKind.SubtractExpression => BinaryOperatorKind.Subtract, 
				SyntaxKind.MultiplyExpression => BinaryOperatorKind.Multiply, 
				SyntaxKind.ExponentiateExpression => BinaryOperatorKind.Power, 
				SyntaxKind.DivideExpression => BinaryOperatorKind.Divide, 
				SyntaxKind.ModuloExpression => BinaryOperatorKind.Modulo, 
				SyntaxKind.IntegerDivideExpression => BinaryOperatorKind.IntegerDivide, 
				SyntaxKind.LeftShiftExpression => BinaryOperatorKind.LeftShift, 
				SyntaxKind.RightShiftExpression => BinaryOperatorKind.RightShift, 
				SyntaxKind.ExclusiveOrExpression => BinaryOperatorKind.Xor, 
				SyntaxKind.OrExpression => BinaryOperatorKind.Or, 
				SyntaxKind.OrElseExpression => BinaryOperatorKind.OrElse, 
				SyntaxKind.AndExpression => BinaryOperatorKind.And, 
				SyntaxKind.AndAlsoExpression => BinaryOperatorKind.AndAlso, 
				_ => throw ExceptionUtilities.UnexpectedValue(opCode), 
			};
		}

		public static BinaryOperatorKind ResolveBinaryOperator(BinaryOperatorKind opCode, BoundExpression left, BoundExpression right, Binder binder, bool considerUserDefinedOrLateBound, out SpecialType intrinsicOperatorType, out OverloadResolutionResult userDefinedOperator, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			opCode &= BinaryOperatorKind.OpMask;
			intrinsicOperatorType = SpecialType.None;
			userDefinedOperator = default(OverloadResolutionResult);
			TypeSymbol type = left.Type;
			TypeSymbol type2 = right.Type;
			TypeSymbol nullableUnderlyingTypeOrSelf = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type);
			bool flag = (object)type != nullableUnderlyingTypeOrSelf;
			TypeSymbol nullableUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type2);
			bool flag2 = (object)type2 != nullableUnderlyingTypeOrSelf2;
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf);
			bool flag3 = (object)enumUnderlyingTypeOrSelf != nullableUnderlyingTypeOrSelf;
			TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(nullableUnderlyingTypeOrSelf2);
			bool flag4 = (object)enumUnderlyingTypeOrSelf2 != nullableUnderlyingTypeOrSelf2;
			if (((flag3 || flag) && (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsObjectType(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf))) || ((flag4 || flag2) && (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf2) || TypeSymbolExtensions.IsObjectType(enumUnderlyingTypeOrSelf2) || TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf2))))
			{
				return BinaryOperatorKind.Error;
			}
			if (UseUserDefinedBinaryOperators(opCode, type, type2))
			{
				if (considerUserDefinedOrLateBound)
				{
					if (TypeSymbolExtensions.CanContainUserDefinedOperators(type, ref useSiteInfo) || TypeSymbolExtensions.CanContainUserDefinedOperators(type2, ref useSiteInfo) || (opCode == BinaryOperatorKind.Subtract && TypeSymbolExtensions.IsDateTimeType(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type)) && TypeSymbolExtensions.IsDateTimeType(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type2))))
					{
						userDefinedOperator = ResolveUserDefinedBinaryOperator(left, right, opCode, binder, ref useSiteInfo);
						if (userDefinedOperator.ResolutionIsLateBound)
						{
							intrinsicOperatorType = SpecialType.System_Object;
							return opCode;
						}
						if (!userDefinedOperator.BestResult.HasValue && userDefinedOperator.Candidates.Length == 0)
						{
							BinaryOperatorKind opKind = opCode;
							CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
							userDefinedOperator = ResolveUserDefinedBinaryOperator(left, right, opKind, binder, ref useSiteInfo2, includeEliminatedCandidates: true);
							if (userDefinedOperator.Candidates.Length == 0)
							{
								return BinaryOperatorKind.Error;
							}
						}
						return BinaryOperatorKind.UserDefined;
					}
					bool flag5 = false;
					if (TypeSymbolExtensions.IsObjectType(type))
					{
						if (TypeSymbolExtensions.IsTypeParameter(type2) && (object)ConstraintsHelper.GetNonInterfaceConstraint((TypeParameterSymbol)type2, ref useSiteInfo) == null)
						{
							flag5 = true;
						}
					}
					else if (TypeSymbolExtensions.IsObjectType(type2) && TypeSymbolExtensions.IsTypeParameter(type) && (object)ConstraintsHelper.GetNonInterfaceConstraint((TypeParameterSymbol)type, ref useSiteInfo) == null)
					{
						flag5 = true;
					}
					if (flag5)
					{
						intrinsicOperatorType = SpecialType.System_Object;
						return opCode;
					}
				}
				return BinaryOperatorKind.Error;
			}
			BinaryOperatorKind binaryOperatorKind = BinaryOperatorKind.Error;
			if (flag3 && flag4 && (opCode == BinaryOperatorKind.Xor || opCode == BinaryOperatorKind.And || opCode == BinaryOperatorKind.Or) && TypeSymbolExtensions.IsSameTypeIgnoringAll(nullableUnderlyingTypeOrSelf, nullableUnderlyingTypeOrSelf2))
			{
				binaryOperatorKind = opCode;
				if (flag || flag2)
				{
					binaryOperatorKind |= BinaryOperatorKind.Lifted;
				}
			}
			else
			{
				SpecialType specialType = enumUnderlyingTypeOrSelf.SpecialType;
				SpecialType specialType2 = enumUnderlyingTypeOrSelf2.SpecialType;
				if (specialType == SpecialType.None && TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf))
				{
					specialType = SpecialType.System_String;
				}
				if (specialType2 == SpecialType.None && TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf2))
				{
					specialType2 = SpecialType.System_String;
				}
				intrinsicOperatorType = ResolveNotLiftedIntrinsicBinaryOperator(opCode, specialType, specialType2);
				if (intrinsicOperatorType != 0)
				{
					binaryOperatorKind = opCode;
				}
				if (binaryOperatorKind != BinaryOperatorKind.Error && (flag || flag2) && intrinsicOperatorType != 0 && intrinsicOperatorType != SpecialType.System_String && intrinsicOperatorType != SpecialType.System_Object && opCode != BinaryOperatorKind.Concatenate && opCode != BinaryOperatorKind.Like)
				{
					binaryOperatorKind |= BinaryOperatorKind.Lifted;
				}
			}
			return binaryOperatorKind;
		}

		public static bool UseUserDefinedBinaryOperators(BinaryOperatorKind opCode, TypeSymbol leftType, TypeSymbol rightType)
		{
			TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(leftType));
			TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(rightType));
			if ((enumUnderlyingTypeOrSelf.SpecialType != SpecialType.System_Object && !TypeSymbolExtensions.IsIntrinsicType(enumUnderlyingTypeOrSelf) && !TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf)) || (enumUnderlyingTypeOrSelf2.SpecialType != SpecialType.System_Object && !TypeSymbolExtensions.IsIntrinsicType(enumUnderlyingTypeOrSelf2) && !TypeSymbolExtensions.IsCharSZArray(enumUnderlyingTypeOrSelf2)) || (TypeSymbolExtensions.IsDateTimeType(enumUnderlyingTypeOrSelf) && TypeSymbolExtensions.IsDateTimeType(enumUnderlyingTypeOrSelf2) && opCode == BinaryOperatorKind.Subtract))
			{
				return true;
			}
			return false;
		}

		public static ConstantValue TryFoldConstantBinaryOperator(BinaryOperatorKind operatorKind, BoundExpression left, BoundExpression right, TypeSymbol resultType, ref bool integerOverflow, ref bool divideByZero, ref bool lengthOutOfLimit)
		{
			integerOverflow = false;
			divideByZero = false;
			lengthOutOfLimit = false;
			ConstantValue constantValueOpt = left.ConstantValueOpt;
			ConstantValue constantValueOpt2 = right.ConstantValueOpt;
			if ((object)constantValueOpt == null || constantValueOpt.IsBad || (object)constantValueOpt2 == null || constantValueOpt2.IsBad || TypeSymbolExtensions.IsErrorType(resultType))
			{
				return null;
			}
			TypeSymbol type = left.Type;
			TypeSymbol type2 = right.Type;
			BinaryOperatorKind binaryOperatorKind = operatorKind & BinaryOperatorKind.OpMask;
			ConstantValue constantValue = null;
			if (binaryOperatorKind != BinaryOperatorKind.Like && (operatorKind & BinaryOperatorKind.CompareText) == 0 && TypeSymbolExtensions.AllowsCompileTimeOperations(type) && TypeSymbolExtensions.AllowsCompileTimeOperations(type2) && TypeSymbolExtensions.AllowsCompileTimeOperations(resultType))
			{
				TypeSymbol enumUnderlyingTypeOrSelf = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(type);
				TypeSymbol enumUnderlyingTypeOrSelf2 = TypeSymbolExtensions.GetEnumUnderlyingTypeOrSelf(resultType);
				if (TypeSymbolExtensions.IsIntegralType(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsCharType(enumUnderlyingTypeOrSelf) || TypeSymbolExtensions.IsDateTimeType(enumUnderlyingTypeOrSelf))
				{
					constantValue = FoldIntegralCharOrDateTimeBinaryOperator(binaryOperatorKind, constantValueOpt, constantValueOpt2, enumUnderlyingTypeOrSelf, enumUnderlyingTypeOrSelf2, ref integerOverflow, ref divideByZero);
				}
				else if (TypeSymbolExtensions.IsFloatingType(enumUnderlyingTypeOrSelf))
				{
					constantValue = FoldFloatingBinaryOperator(binaryOperatorKind, constantValueOpt, constantValueOpt2, enumUnderlyingTypeOrSelf, enumUnderlyingTypeOrSelf2);
				}
				else if (TypeSymbolExtensions.IsDecimalType(enumUnderlyingTypeOrSelf))
				{
					constantValue = FoldDecimalBinaryOperator(binaryOperatorKind, constantValueOpt, constantValueOpt2, enumUnderlyingTypeOrSelf2, ref divideByZero);
				}
				else if (TypeSymbolExtensions.IsStringType(enumUnderlyingTypeOrSelf))
				{
					constantValue = FoldStringBinaryOperator(binaryOperatorKind, constantValueOpt, constantValueOpt2);
					if (constantValue.IsBad)
					{
						lengthOutOfLimit = true;
					}
				}
				else if (TypeSymbolExtensions.IsBooleanType(enumUnderlyingTypeOrSelf))
				{
					constantValue = FoldBooleanBinaryOperator(binaryOperatorKind, constantValueOpt, constantValueOpt2);
				}
			}
			return constantValue;
		}

		private static ConstantValue FoldIntegralCharOrDateTimeBinaryOperator(BinaryOperatorKind op, ConstantValue left, ConstantValue right, TypeSymbol operandType, TypeSymbol resultType, ref bool integerOverflow, ref bool divideByZero)
		{
			long constantValueAsInt = CompileTimeCalculations.GetConstantValueAsInt64(ref left);
			long constantValueAsInt2 = CompileTimeCalculations.GetConstantValueAsInt64(ref right);
			if (TypeSymbolExtensions.IsBooleanType(resultType))
			{
				return ConstantValue.Create(op switch
				{
					BinaryOperatorKind.Equals => TypeSymbolExtensions.IsUnsignedIntegralType(operandType) ? (CompileTimeCalculations.UncheckedCULng(constantValueAsInt) == CompileTimeCalculations.UncheckedCULng(constantValueAsInt2)) : (constantValueAsInt == constantValueAsInt2), 
					BinaryOperatorKind.NotEquals => TypeSymbolExtensions.IsUnsignedIntegralType(operandType) ? (CompileTimeCalculations.UncheckedCULng(constantValueAsInt) != CompileTimeCalculations.UncheckedCULng(constantValueAsInt2)) : (constantValueAsInt != constantValueAsInt2), 
					BinaryOperatorKind.LessThanOrEqual => TypeSymbolExtensions.IsUnsignedIntegralType(operandType) ? (CompileTimeCalculations.UncheckedCULng(constantValueAsInt) <= CompileTimeCalculations.UncheckedCULng(constantValueAsInt2)) : (constantValueAsInt <= constantValueAsInt2), 
					BinaryOperatorKind.GreaterThanOrEqual => TypeSymbolExtensions.IsUnsignedIntegralType(operandType) ? (CompileTimeCalculations.UncheckedCULng(constantValueAsInt) >= CompileTimeCalculations.UncheckedCULng(constantValueAsInt2)) : (constantValueAsInt >= constantValueAsInt2), 
					BinaryOperatorKind.LessThan => TypeSymbolExtensions.IsUnsignedIntegralType(operandType) ? (CompileTimeCalculations.UncheckedCULng(constantValueAsInt) < CompileTimeCalculations.UncheckedCULng(constantValueAsInt2)) : (constantValueAsInt < constantValueAsInt2), 
					BinaryOperatorKind.GreaterThan => TypeSymbolExtensions.IsUnsignedIntegralType(operandType) ? (CompileTimeCalculations.UncheckedCULng(constantValueAsInt) > CompileTimeCalculations.UncheckedCULng(constantValueAsInt2)) : (constantValueAsInt > constantValueAsInt2), 
					_ => throw ExceptionUtilities.UnexpectedValue(op), 
				});
			}
			ConstantValueTypeDiscriminator constantValueTypeDiscriminator = TypeSymbolExtensions.GetConstantValueTypeDiscriminator(operandType);
			ConstantValueTypeDiscriminator constantValueTypeDiscriminator2 = TypeSymbolExtensions.GetConstantValueTypeDiscriminator(resultType);
			long num = default(long);
			switch (op)
			{
			case BinaryOperatorKind.Add:
				num = CompileTimeCalculations.NarrowIntegralResult(constantValueAsInt + constantValueAsInt2, constantValueTypeDiscriminator, constantValueTypeDiscriminator2, ref integerOverflow);
				if (!TypeSymbolExtensions.IsUnsignedIntegralType(resultType))
				{
					if ((constantValueAsInt2 > 0 && num < constantValueAsInt) || (constantValueAsInt2 < 0 && num > constantValueAsInt))
					{
						integerOverflow = true;
					}
				}
				else if (CompileTimeCalculations.UncheckedCULng(num) < CompileTimeCalculations.UncheckedCULng(constantValueAsInt))
				{
					integerOverflow = true;
				}
				break;
			case BinaryOperatorKind.Subtract:
				num = CompileTimeCalculations.NarrowIntegralResult(constantValueAsInt - constantValueAsInt2, constantValueTypeDiscriminator, constantValueTypeDiscriminator2, ref integerOverflow);
				if (!TypeSymbolExtensions.IsUnsignedIntegralType(resultType))
				{
					if ((constantValueAsInt2 > 0 && num > constantValueAsInt) || (constantValueAsInt2 < 0 && num < constantValueAsInt))
					{
						integerOverflow = true;
					}
				}
				else if (CompileTimeCalculations.UncheckedCULng(num) > CompileTimeCalculations.UncheckedCULng(constantValueAsInt))
				{
					integerOverflow = true;
				}
				break;
			case BinaryOperatorKind.Multiply:
				num = CompileTimeCalculations.Multiply(constantValueAsInt, constantValueAsInt2, constantValueTypeDiscriminator, constantValueTypeDiscriminator2, ref integerOverflow);
				break;
			case BinaryOperatorKind.IntegerDivide:
				if (constantValueAsInt2 == 0L)
				{
					divideByZero = true;
					break;
				}
				num = CompileTimeCalculations.NarrowIntegralResult(TypeSymbolExtensions.IsUnsignedIntegralType(resultType) ? CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(constantValueAsInt) / CompileTimeCalculations.UncheckedCULng(constantValueAsInt2)) : CompileTimeCalculations.UncheckedIntegralDiv(constantValueAsInt, constantValueAsInt2), constantValueTypeDiscriminator, constantValueTypeDiscriminator2, ref integerOverflow);
				if (!TypeSymbolExtensions.IsUnsignedIntegralType(resultType) && constantValueAsInt == long.MinValue && constantValueAsInt2 == -1)
				{
					integerOverflow = true;
				}
				break;
			case BinaryOperatorKind.Modulo:
				if (constantValueAsInt2 == 0L)
				{
					divideByZero = true;
				}
				else
				{
					num = ((!TypeSymbolExtensions.IsUnsignedIntegralType(resultType)) ? ((constantValueAsInt2 == -1) ? 0 : (constantValueAsInt % constantValueAsInt2)) : CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(constantValueAsInt) % CompileTimeCalculations.UncheckedCULng(constantValueAsInt2)));
				}
				break;
			case BinaryOperatorKind.Xor:
				num = constantValueAsInt ^ constantValueAsInt2;
				break;
			case BinaryOperatorKind.Or:
				num = constantValueAsInt | constantValueAsInt2;
				break;
			case BinaryOperatorKind.And:
				num = constantValueAsInt & constantValueAsInt2;
				break;
			case BinaryOperatorKind.LeftShift:
			{
				num = constantValueAsInt << ((int)constantValueAsInt2 & CodeGenerator.GetShiftSizeMask(operandType));
				bool overflow = false;
				num = CompileTimeCalculations.NarrowIntegralResult(num, constantValueTypeDiscriminator, constantValueTypeDiscriminator2, ref overflow);
				break;
			}
			case BinaryOperatorKind.RightShift:
				num = ((!TypeSymbolExtensions.IsUnsignedIntegralType(resultType)) ? (constantValueAsInt >> ((int)constantValueAsInt2 & CodeGenerator.GetShiftSizeMask(operandType))) : CompileTimeCalculations.UncheckedCLng(CompileTimeCalculations.UncheckedCULng(constantValueAsInt) >> ((int)constantValueAsInt2 & CodeGenerator.GetShiftSizeMask(operandType))));
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(op);
			}
			if (divideByZero)
			{
				return ConstantValue.Bad;
			}
			return CompileTimeCalculations.GetConstantValue(constantValueTypeDiscriminator2, num);
		}

		private static ConstantValue FoldFloatingBinaryOperator(BinaryOperatorKind op, ConstantValue left, ConstantValue right, TypeSymbol operandType, TypeSymbol resultType)
		{
			double num = (TypeSymbolExtensions.IsSingleType(operandType) ? ((double)left.SingleValue) : left.DoubleValue);
			double num2 = (TypeSymbolExtensions.IsSingleType(operandType) ? ((double)right.SingleValue) : right.DoubleValue);
			if (TypeSymbolExtensions.IsBooleanType(resultType))
			{
				return ConstantValue.Create(op switch
				{
					BinaryOperatorKind.Equals => num == num2, 
					BinaryOperatorKind.NotEquals => num != num2, 
					BinaryOperatorKind.LessThanOrEqual => num <= num2, 
					BinaryOperatorKind.GreaterThanOrEqual => num >= num2, 
					BinaryOperatorKind.LessThan => num < num2, 
					BinaryOperatorKind.GreaterThan => num > num2, 
					_ => throw ExceptionUtilities.UnexpectedValue(op), 
				});
			}
			double num3 = 0.0;
			switch (op)
			{
			case BinaryOperatorKind.Add:
				num3 = num + num2;
				break;
			case BinaryOperatorKind.Subtract:
				num3 = num - num2;
				break;
			case BinaryOperatorKind.Multiply:
				num3 = num * num2;
				break;
			case BinaryOperatorKind.Power:
				if (double.IsInfinity(num2))
				{
					if (num.Equals(1.0))
					{
						num3 = num;
						break;
					}
					if (num.Equals(-1.0))
					{
						num3 = double.NaN;
						break;
					}
				}
				else if (double.IsNaN(num2))
				{
					num3 = double.NaN;
					break;
				}
				num3 = Math.Pow(num, num2);
				break;
			case BinaryOperatorKind.Divide:
				num3 = num / num2;
				break;
			case BinaryOperatorKind.Modulo:
				num3 = num % num2;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(op);
			}
			bool overflow = false;
			num3 = CompileTimeCalculations.NarrowFloatingResult(num3, TypeSymbolExtensions.GetConstantValueTypeDiscriminator(resultType), ref overflow);
			if (TypeSymbolExtensions.IsSingleType(resultType))
			{
				return ConstantValue.Create((float)num3);
			}
			return ConstantValue.Create(num3);
		}

		private static ConstantValue FoldDecimalBinaryOperator(BinaryOperatorKind op, ConstantValue left, ConstantValue right, TypeSymbol resultType, ref bool divideByZero)
		{
			decimal decimalValue = left.DecimalValue;
			decimal decimalValue2 = right.DecimalValue;
			if (TypeSymbolExtensions.IsBooleanType(resultType))
			{
				bool flag = false;
				int num = decimalValue.CompareTo(decimalValue2);
				return ConstantValue.Create(op switch
				{
					BinaryOperatorKind.Equals => num == 0, 
					BinaryOperatorKind.NotEquals => num != 0, 
					BinaryOperatorKind.LessThanOrEqual => num <= 0, 
					BinaryOperatorKind.GreaterThanOrEqual => num >= 0, 
					BinaryOperatorKind.LessThan => num < 0, 
					BinaryOperatorKind.GreaterThan => num > 0, 
					_ => throw ExceptionUtilities.UnexpectedValue(op), 
				});
			}
			bool flag2 = false;
			decimal value = default(decimal);
			try
			{
				value = op switch
				{
					BinaryOperatorKind.Add => decimal.Add(decimalValue, decimalValue2), 
					BinaryOperatorKind.Subtract => decimal.Subtract(decimalValue, decimalValue2), 
					BinaryOperatorKind.Multiply => decimal.Multiply(decimalValue, decimalValue2), 
					BinaryOperatorKind.Divide => decimal.Divide(decimalValue, decimalValue2), 
					BinaryOperatorKind.Modulo => decimal.Remainder(decimalValue, decimalValue2), 
					_ => throw ExceptionUtilities.UnexpectedValue(op), 
				};
			}
			catch (OverflowException ex)
			{
				ProjectData.SetProjectError(ex);
				OverflowException ex2 = ex;
				flag2 = true;
				ProjectData.ClearProjectError();
			}
			catch (DivideByZeroException ex3)
			{
				ProjectData.SetProjectError(ex3);
				DivideByZeroException ex4 = ex3;
				divideByZero = true;
				ProjectData.ClearProjectError();
			}
			if (flag2 || divideByZero)
			{
				return ConstantValue.Bad;
			}
			return ConstantValue.Create(value);
		}

		private static ConstantValue FoldStringBinaryOperator(BinaryOperatorKind op, ConstantValue left, ConstantValue right)
		{
			ConstantValue result;
			switch (op)
			{
			case BinaryOperatorKind.Concatenate:
			{
				Rope rope = (left.IsNothing ? Rope.Empty : left.RopeValue);
				Rope rope2 = (right.IsNothing ? Rope.Empty : right.RopeValue);
				if ((long)rope.Length + (long)rope2.Length > int.MaxValue)
				{
					return ConstantValue.Bad;
				}
				try
				{
					result = ConstantValue.CreateFromRope(Rope.Concat(rope, rope2));
				}
				catch (OutOfMemoryException ex)
				{
					ProjectData.SetProjectError(ex);
					OutOfMemoryException ex2 = ex;
					ConstantValue bad = ConstantValue.Bad;
					ProjectData.ClearProjectError();
					return bad;
				}
				break;
			}
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.LessThanOrEqual:
			case BinaryOperatorKind.GreaterThanOrEqual:
			case BinaryOperatorKind.LessThan:
			case BinaryOperatorKind.GreaterThan:
			{
				string strA = (left.IsNothing ? string.Empty : left.StringValue);
				string strB = (right.IsNothing ? string.Empty : right.StringValue);
				bool value = false;
				int num = string.Compare(strA, strB, StringComparison.Ordinal);
				switch (op)
				{
				case BinaryOperatorKind.Equals:
					value = num == 0;
					break;
				case BinaryOperatorKind.NotEquals:
					value = num != 0;
					break;
				case BinaryOperatorKind.GreaterThan:
					value = num > 0;
					break;
				case BinaryOperatorKind.GreaterThanOrEqual:
					value = num >= 0;
					break;
				case BinaryOperatorKind.LessThan:
					value = num < 0;
					break;
				case BinaryOperatorKind.LessThanOrEqual:
					value = num <= 0;
					break;
				}
				result = ConstantValue.Create(value);
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(op);
			}
			return result;
		}

		private static ConstantValue FoldBooleanBinaryOperator(BinaryOperatorKind op, ConstantValue left, ConstantValue right)
		{
			bool booleanValue = left.BooleanValue;
			bool booleanValue2 = right.BooleanValue;
			bool flag = false;
			switch (op)
			{
			case BinaryOperatorKind.Equals:
				flag = booleanValue == booleanValue2;
				break;
			case BinaryOperatorKind.NotEquals:
				flag = booleanValue != booleanValue2;
				break;
			case BinaryOperatorKind.GreaterThan:
				flag = !booleanValue && booleanValue2;
				break;
			case BinaryOperatorKind.GreaterThanOrEqual:
				flag = !booleanValue || booleanValue2;
				break;
			case BinaryOperatorKind.LessThan:
				flag = booleanValue && !booleanValue2;
				break;
			case BinaryOperatorKind.LessThanOrEqual:
				flag = booleanValue || !booleanValue2;
				break;
			case BinaryOperatorKind.Xor:
				flag = booleanValue ^ booleanValue2;
				break;
			case BinaryOperatorKind.Or:
			case BinaryOperatorKind.OrElse:
				flag = booleanValue || booleanValue2;
				break;
			case BinaryOperatorKind.And:
			case BinaryOperatorKind.AndAlso:
				flag = booleanValue && booleanValue2;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(op);
			}
			return ConstantValue.Create(flag);
		}

		internal static SpecialType ResolveNotLiftedIntrinsicBinaryOperator(BinaryOperatorKind opCode, SpecialType left, SpecialType right)
		{
			int? num = Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.TypeToIndex(left);
			int? num2 = Microsoft.CodeAnalysis.VisualBasic.Symbols.SpecialTypeExtensions.TypeToIndex(right);
			if (!num.HasValue || !num2.HasValue)
			{
				return SpecialType.None;
			}
			BinaryOperatorTables.TableKind tableKind;
			switch (opCode)
			{
			case BinaryOperatorKind.Add:
				tableKind = BinaryOperatorTables.TableKind.Addition;
				break;
			case BinaryOperatorKind.Subtract:
			case BinaryOperatorKind.Multiply:
			case BinaryOperatorKind.Modulo:
				tableKind = BinaryOperatorTables.TableKind.SubtractionMultiplicationModulo;
				break;
			case BinaryOperatorKind.Divide:
				tableKind = BinaryOperatorTables.TableKind.Division;
				break;
			case BinaryOperatorKind.IntegerDivide:
				tableKind = BinaryOperatorTables.TableKind.IntegerDivision;
				break;
			case BinaryOperatorKind.Power:
				tableKind = BinaryOperatorTables.TableKind.Power;
				break;
			case BinaryOperatorKind.LeftShift:
			case BinaryOperatorKind.RightShift:
				tableKind = BinaryOperatorTables.TableKind.Shift;
				break;
			case BinaryOperatorKind.OrElse:
			case BinaryOperatorKind.AndAlso:
				tableKind = BinaryOperatorTables.TableKind.Logical;
				break;
			case BinaryOperatorKind.Concatenate:
			case BinaryOperatorKind.Like:
				tableKind = BinaryOperatorTables.TableKind.ConcatenationLike;
				break;
			case BinaryOperatorKind.Equals:
			case BinaryOperatorKind.NotEquals:
			case BinaryOperatorKind.LessThanOrEqual:
			case BinaryOperatorKind.GreaterThanOrEqual:
			case BinaryOperatorKind.LessThan:
			case BinaryOperatorKind.GreaterThan:
				tableKind = BinaryOperatorTables.TableKind.Relational;
				break;
			case BinaryOperatorKind.Xor:
			case BinaryOperatorKind.Or:
			case BinaryOperatorKind.And:
				tableKind = BinaryOperatorTables.TableKind.Bitwise;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(opCode);
			}
			return (SpecialType)BinaryOperatorTables.Table[(int)tableKind, num.Value, num2.Value];
		}

		public static KeyValuePair<ConversionKind, MethodSymbol> ResolveUserDefinedConversion(TypeSymbol source, TypeSymbol destination, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			KeyValuePair<ConversionKind, MethodSymbol> result = default(KeyValuePair<ConversionKind, MethodSymbol>);
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			CollectUserDefinedConversionOperators(source, destination, instance, ref useSiteInfo);
			if (instance.Count == 0)
			{
				instance.Free();
				return result;
			}
			ArrayBuilder<KeyValuePair<ConversionKind, ConversionKind>> instance2 = ArrayBuilder<KeyValuePair<ConversionKind, ConversionKind>>.GetInstance();
			instance2.ZeroInit(instance.Count);
			BitVector applicable = BitVector.Create(instance.Count);
			MethodSymbol bestMatch = null;
			if (DetermineMostSpecificWideningConversion(source, destination, instance, instance2, ref applicable, out bestMatch, suppressViabilityChecks: false, ref useSiteInfo))
			{
				if ((object)bestMatch != null)
				{
					result = new KeyValuePair<ConversionKind, MethodSymbol>(ConversionKind.Widening | ConversionKind.UserDefined, bestMatch);
				}
			}
			else if (instance.Count != 0)
			{
				if (DetermineMostSpecificNarrowingConversion(source, destination, instance, instance2, ref applicable, out bestMatch, suppressViabilityChecks: false, ref useSiteInfo))
				{
					if ((object)bestMatch != null)
					{
						result = new KeyValuePair<ConversionKind, MethodSymbol>(ConversionKind.Narrowing | ConversionKind.UserDefined, bestMatch);
					}
				}
				else if (instance.Count != 0 && TypeSymbolExtensions.IsNullableType(source) && TypeSymbolExtensions.IsNullableType(destination))
				{
					applicable.Clear();
					instance2.ZeroInit(instance.Count);
					TypeSymbol nullableUnderlyingType = TypeSymbolExtensions.GetNullableUnderlyingType(source);
					TypeSymbol nullableUnderlyingType2 = TypeSymbolExtensions.GetNullableUnderlyingType(destination);
					if (!TypeSymbolExtensions.IsErrorType(nullableUnderlyingType) && !TypeSymbolExtensions.IsErrorType(nullableUnderlyingType2))
					{
						if (DetermineMostSpecificWideningConversion(nullableUnderlyingType, nullableUnderlyingType2, instance, instance2, ref applicable, out bestMatch, suppressViabilityChecks: true, ref useSiteInfo))
						{
							if ((object)bestMatch != null)
							{
								result = new KeyValuePair<ConversionKind, MethodSymbol>(ConversionKind.WideningNullable | ConversionKind.UserDefined, bestMatch);
							}
						}
						else if (DetermineMostSpecificNarrowingConversion(nullableUnderlyingType, nullableUnderlyingType2, instance, instance2, ref applicable, out bestMatch, suppressViabilityChecks: true, ref useSiteInfo) && (object)bestMatch != null)
						{
							result = new KeyValuePair<ConversionKind, MethodSymbol>(ConversionKind.NarrowingNullable | ConversionKind.UserDefined, bestMatch);
						}
					}
				}
			}
			instance2.Free();
			instance.Free();
			return result;
		}

		private static bool DetermineMostSpecificWideningConversion(TypeSymbol source, TypeSymbol destination, ArrayBuilder<MethodSymbol> opSet, ArrayBuilder<KeyValuePair<ConversionKind, ConversionKind>> conversionKinds, [In] ref BitVector applicable, out MethodSymbol bestMatch, bool suppressViabilityChecks, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bestMatch = null;
			int bestDegreeOfGenericity = -1;
			bool bestMatchIsAmbiguous = false;
			TypeSymbol typeSymbol = null;
			TypeSymbol typeSymbol2 = null;
			int num = 0;
			int num2 = 0;
			int num3 = opSet.Count - 1;
			for (int i = 0; i <= num3; i++)
			{
				MethodSymbol methodSymbol = opSet[i];
				int num4 = num2;
				num2++;
				if (num4 < i)
				{
					opSet[num4] = methodSymbol;
				}
				if (!IsWidening(methodSymbol))
				{
					continue;
				}
				if (ClassifyConversionOperatorInOutConversions(source, destination, methodSymbol, out var conversionIn, out var conversionOut, suppressViabilityChecks, ref useSiteInfo))
				{
					conversionKinds[num4] = new KeyValuePair<ConversionKind, ConversionKind>(conversionIn, conversionOut);
					if ((object)bestMatch == null)
					{
						if (!Conversions.IsWideningConversion(conversionIn) || !Conversions.IsWideningConversion(conversionOut))
						{
							continue;
						}
						if (Conversions.IsIdentityConversion(conversionIn) && Conversions.IsIdentityConversion(conversionOut))
						{
							bestMatch = methodSymbol;
							applicable.Clear();
							num = 0;
							continue;
						}
						if (Conversions.IsIdentityConversion(conversionIn))
						{
							typeSymbol = source;
						}
						if (Conversions.IsIdentityConversion(conversionOut))
						{
							typeSymbol2 = destination;
						}
						applicable[num4] = true;
						num++;
					}
					else if (Conversions.IsIdentityConversion(conversionIn) && Conversions.IsIdentityConversion(conversionOut))
					{
						bestMatch = LeastGenericConversionOperator(bestMatch, methodSymbol, ref bestDegreeOfGenericity, ref bestMatchIsAmbiguous);
						if (bestMatchIsAmbiguous && bestDegreeOfGenericity == 0)
						{
							break;
						}
					}
				}
				else
				{
					num2 = num4;
				}
			}
			opSet.Clip(num2);
			conversionKinds.Clip(num2);
			if ((object)bestMatch != null)
			{
				if (bestMatchIsAmbiguous)
				{
					bestMatch = null;
				}
				return true;
			}
			if (num > 0)
			{
				if (num > 1)
				{
					ArrayBuilder<TypeSymbol> arrayBuilder = null;
					if ((object)typeSymbol == null)
					{
						arrayBuilder = ArrayBuilder<TypeSymbol>.GetInstance();
						int num5 = opSet.Count - 1;
						for (int j = 0; j <= num5; j++)
						{
							if (applicable[j])
							{
								arrayBuilder.Add(opSet[j].Parameters[0].Type);
							}
						}
						typeSymbol = MostEncompassed(arrayBuilder, ref useSiteInfo);
					}
					if ((object)typeSymbol2 == null && (object)typeSymbol != null)
					{
						if (arrayBuilder == null)
						{
							arrayBuilder = ArrayBuilder<TypeSymbol>.GetInstance();
						}
						else
						{
							arrayBuilder.Clear();
						}
						int num6 = opSet.Count - 1;
						for (int k = 0; k <= num6; k++)
						{
							if (applicable[k])
							{
								arrayBuilder.Add(opSet[k].ReturnType);
							}
						}
						typeSymbol2 = MostEncompassing(arrayBuilder, ref useSiteInfo);
					}
					arrayBuilder?.Free();
					if ((object)typeSymbol != null && (object)typeSymbol2 != null)
					{
						bestMatch = ChooseMostSpecificConversionOperator(opSet, applicable, typeSymbol, typeSymbol2, out bestMatchIsAmbiguous);
					}
					if ((object)bestMatch != null && bestMatchIsAmbiguous)
					{
						bestMatch = null;
					}
				}
				else
				{
					int num7 = opSet.Count - 1;
					for (int l = 0; l <= num7; l++)
					{
						if (applicable[l])
						{
							bestMatch = opSet[l];
							break;
						}
					}
				}
				return true;
			}
			return false;
		}

		private static MethodSymbol ChooseMostSpecificConversionOperator(ArrayBuilder<MethodSymbol> opSet, BitVector applicable, TypeSymbol mostSpecificSourceType, TypeSymbol mostSpecificTargetType, out bool bestMatchIsAmbiguous)
		{
			int bestDegreeOfGenericity = -1;
			MethodSymbol methodSymbol = null;
			bestMatchIsAmbiguous = false;
			int num = opSet.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (!applicable[i])
				{
					continue;
				}
				MethodSymbol methodSymbol2 = opSet[i];
				if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(mostSpecificSourceType, methodSymbol2.Parameters[0].Type) || !TypeSymbolExtensions.IsSameTypeIgnoringAll(mostSpecificTargetType, methodSymbol2.ReturnType))
				{
					continue;
				}
				if ((object)methodSymbol == null)
				{
					methodSymbol = methodSymbol2;
					continue;
				}
				methodSymbol = LeastGenericConversionOperator(methodSymbol, methodSymbol2, ref bestDegreeOfGenericity, ref bestMatchIsAmbiguous);
				if (bestMatchIsAmbiguous && bestDegreeOfGenericity == 0)
				{
					break;
				}
			}
			return methodSymbol;
		}

		private static bool ClassifyConversionOperatorInOutConversions(TypeSymbol source, TypeSymbol destination, MethodSymbol method, out ConversionKind conversionIn, out ConversionKind conversionOut, bool suppressViabilityChecks, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol type = method.Parameters[0].Type;
			TypeSymbol returnType = method.ReturnType;
			if (!suppressViabilityChecks && !IsConversionOperatorViableBasedOnTypesInvolved(method, type, returnType, ref useSiteInfo))
			{
				conversionIn = ConversionKind.DelegateRelaxationLevelNone;
				conversionOut = ConversionKind.DelegateRelaxationLevelNone;
				return false;
			}
			if (source is ArrayLiteralTypeSymbol arrayLiteralTypeSymbol)
			{
				BoundArrayLiteral arrayLiteral = arrayLiteralTypeSymbol.ArrayLiteral;
				conversionIn = Conversions.ClassifyArrayLiteralConversion(arrayLiteral, type, arrayLiteral.Binder, ref useSiteInfo);
				if (Conversions.IsWideningConversion(conversionIn) && TypeSymbolExtensions.IsSameTypeIgnoringAll(arrayLiteralTypeSymbol, type))
				{
					conversionIn = ConversionKind.Identity;
				}
			}
			else
			{
				conversionIn = Conversions.ClassifyPredefinedConversion(source, type, ref useSiteInfo);
			}
			conversionOut = Conversions.ClassifyPredefinedConversion(returnType, destination, ref useSiteInfo);
			return true;
		}

		private static bool IsConversionOperatorViableBasedOnTypesInvolved(MethodSymbol method, TypeSymbol inputType, TypeSymbol outputType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (TypeSymbolExtensions.IsErrorType(inputType) || TypeSymbolExtensions.IsErrorType(outputType))
			{
				return false;
			}
			if (!method.ContainingType.IsDefinition)
			{
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = (useSiteInfo.AccumulatesDependencies ? new CompoundUseSiteInfo<AssemblySymbol>(useSiteInfo.AssemblyBeingBuilt) : CompoundUseSiteInfo<AssemblySymbol>.DiscardedDependencies);
				if (Conversions.ConversionExists(Conversions.ClassifyPredefinedConversion(inputType, outputType, ref useSiteInfo2)) || !useSiteInfo2.Diagnostics.IsNullOrEmpty())
				{
					useSiteInfo.MergeAndClear(ref useSiteInfo2);
					return false;
				}
				useSiteInfo.MergeAndClear(ref useSiteInfo2);
			}
			return true;
		}

		private static bool DetermineMostSpecificNarrowingConversion(TypeSymbol source, TypeSymbol destination, ArrayBuilder<MethodSymbol> opSet, ArrayBuilder<KeyValuePair<ConversionKind, ConversionKind>> conversionKinds, [In] ref BitVector applicable, out MethodSymbol bestMatch, bool suppressViabilityChecks, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bestMatch = null;
			int bestDegreeOfGenericity = -1;
			bool bestMatchIsAmbiguous = false;
			TypeSymbol typeSymbol = null;
			TypeSymbol typeSymbol2 = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = opSet.Count - 1;
			for (int i = 0; i <= num5; i++)
			{
				MethodSymbol methodSymbol = opSet[i];
				int num6 = num4;
				num4++;
				if (num6 < i)
				{
					opSet[num6] = methodSymbol;
					conversionKinds[num6] = conversionKinds[i];
				}
				ConversionKind conversionIn;
				ConversionKind conversionOut;
				if (IsWidening(methodSymbol))
				{
					KeyValuePair<ConversionKind, ConversionKind> keyValuePair = conversionKinds[num6];
					conversionIn = keyValuePair.Key;
					conversionOut = keyValuePair.Value;
				}
				else
				{
					if (!ClassifyConversionOperatorInOutConversions(source, destination, methodSymbol, out conversionIn, out conversionOut, suppressViabilityChecks, ref useSiteInfo))
					{
						num4 = num6;
						continue;
					}
					conversionKinds[num6] = new KeyValuePair<ConversionKind, ConversionKind>(conversionIn, conversionOut);
				}
				if ((object)bestMatch == null)
				{
					if (!Conversions.ConversionExists(conversionIn) || !Conversions.ConversionExists(conversionOut))
					{
						continue;
					}
					if (Conversions.IsIdentityConversion(conversionIn) && Conversions.IsIdentityConversion(conversionOut))
					{
						bestMatch = methodSymbol;
						applicable.Clear();
						num = 0;
						continue;
					}
					if (Conversions.IsWideningConversion(conversionIn))
					{
						if (Conversions.IsIdentityConversion(conversionIn))
						{
							typeSymbol = source;
						}
						else
						{
							num2++;
						}
						if (Conversions.IsWideningConversion(conversionOut))
						{
							if (Conversions.IsIdentityConversion(conversionOut))
							{
								typeSymbol2 = destination;
							}
							else
							{
								num3++;
							}
						}
					}
					else if (Conversions.IsIdentityConversion(conversionOut))
					{
						typeSymbol2 = destination;
					}
					else if (!Conversions.IsNarrowingConversion(conversionOut) || (TypeSymbolExtensions.IsNullableType(source) && TypeSymbolExtensions.IsNullableType(destination) && TypeSymbolExtensions.IsIntrinsicType(methodSymbol.ReturnType)))
					{
						continue;
					}
					applicable[num6] = true;
					num++;
				}
				else if (Conversions.IsIdentityConversion(conversionIn) && Conversions.IsIdentityConversion(conversionOut))
				{
					bestMatch = LeastGenericConversionOperator(bestMatch, methodSymbol, ref bestDegreeOfGenericity, ref bestMatchIsAmbiguous);
					if (bestMatchIsAmbiguous && bestDegreeOfGenericity == 0)
					{
						break;
					}
				}
			}
			opSet.Clip(num4);
			conversionKinds.Clip(num4);
			if ((object)bestMatch != null)
			{
				if (bestMatchIsAmbiguous)
				{
					bestMatch = null;
				}
				return true;
			}
			if (num > 0)
			{
				if (num > 1)
				{
					ArrayBuilder<TypeSymbol> arrayBuilder = null;
					if ((object)typeSymbol == null)
					{
						arrayBuilder = ArrayBuilder<TypeSymbol>.GetInstance();
						int num7 = opSet.Count - 1;
						for (int j = 0; j <= num7; j++)
						{
							if (applicable[j] && (num2 == 0 || Conversions.IsWideningConversion(conversionKinds[j].Key)))
							{
								arrayBuilder.Add(opSet[j].Parameters[0].Type);
							}
						}
						typeSymbol = ((num2 == 0) ? MostEncompassing(arrayBuilder, ref useSiteInfo) : MostEncompassed(arrayBuilder, ref useSiteInfo));
					}
					if ((object)typeSymbol2 == null && (object)typeSymbol != null)
					{
						if (arrayBuilder == null)
						{
							arrayBuilder = ArrayBuilder<TypeSymbol>.GetInstance();
						}
						else
						{
							arrayBuilder.Clear();
						}
						int num8 = opSet.Count - 1;
						for (int k = 0; k <= num8; k++)
						{
							if (applicable[k] && (num3 == 0 || Conversions.IsWideningConversion(conversionKinds[k].Value)))
							{
								arrayBuilder.Add(opSet[k].ReturnType);
							}
						}
						typeSymbol2 = ((num3 == 0) ? MostEncompassed(arrayBuilder, ref useSiteInfo) : MostEncompassing(arrayBuilder, ref useSiteInfo));
					}
					arrayBuilder?.Free();
					if ((object)typeSymbol != null && (object)typeSymbol2 != null)
					{
						bestMatch = ChooseMostSpecificConversionOperator(opSet, applicable, typeSymbol, typeSymbol2, out bestMatchIsAmbiguous);
					}
					if ((object)bestMatch != null && bestMatchIsAmbiguous)
					{
						bestMatch = null;
						return true;
					}
				}
				else
				{
					int num9 = opSet.Count - 1;
					for (int l = 0; l <= num9; l++)
					{
						if (applicable[l])
						{
							bestMatch = opSet[l];
							break;
						}
					}
				}
			}
			return (object)bestMatch != null;
		}

		private static TypeSymbol MostEncompassed(ArrayBuilder<TypeSymbol> typeSet, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeSymbol = null;
			int num = typeSet.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSymbol typeSymbol2 = typeSet[i];
				if ((object)typeSymbol != null && TypeSymbolExtensions.IsSameTypeIgnoringAll(typeSymbol, typeSymbol2))
				{
					continue;
				}
				int num2 = typeSet.Count - 1;
				int num3 = 0;
				while (num3 <= num2)
				{
					if (i == num3 || Conversions.IsWideningConversion(Conversions.ClassifyPredefinedConversion(typeSymbol2, typeSet[num3], ref useSiteInfo)))
					{
						num3++;
						continue;
					}
					goto IL_0064;
				}
				if ((object)typeSymbol == null)
				{
					typeSymbol = typeSymbol2;
					continue;
				}
				typeSymbol = null;
				break;
				IL_0064:;
			}
			return typeSymbol;
		}

		private static TypeSymbol MostEncompassing(ArrayBuilder<TypeSymbol> typeSet, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeSymbol = null;
			int num = typeSet.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSymbol typeSymbol2 = typeSet[i];
				if ((object)typeSymbol != null && TypeSymbolExtensions.IsSameTypeIgnoringAll(typeSymbol, typeSymbol2))
				{
					continue;
				}
				int num2 = typeSet.Count - 1;
				int num3 = 0;
				while (num3 <= num2)
				{
					if (i == num3 || Conversions.IsWideningConversion(Conversions.ClassifyPredefinedConversion(typeSet[num3], typeSymbol2, ref useSiteInfo)))
					{
						num3++;
						continue;
					}
					goto IL_0064;
				}
				if ((object)typeSymbol == null)
				{
					typeSymbol = typeSymbol2;
					continue;
				}
				typeSymbol = null;
				break;
				IL_0064:;
			}
			return typeSymbol;
		}

		private static MethodSymbol LeastGenericConversionOperator(MethodSymbol method1, MethodSymbol method2, [In][Out] ref int bestDegreeOfGenericity, [In][Out] ref bool isAmbiguous)
		{
			if (bestDegreeOfGenericity == -1)
			{
				bestDegreeOfGenericity = DetermineConversionOperatorDegreeOfGenericity(method1);
			}
			int num = DetermineConversionOperatorDegreeOfGenericity(method2);
			if (bestDegreeOfGenericity < num)
			{
				return method1;
			}
			if (num < bestDegreeOfGenericity)
			{
				isAmbiguous = false;
				bestDegreeOfGenericity = num;
				return method2;
			}
			isAmbiguous = true;
			return method1;
		}

		private static int DetermineConversionOperatorDegreeOfGenericity(MethodSymbol method)
		{
			if (!method.ContainingType.IsGenericType)
			{
				return 0;
			}
			int num = 0;
			MethodSymbol originalDefinition = method.OriginalDefinition;
			if (DetectReferencesToGenericParameters(originalDefinition.Parameters[0].Type, TypeParameterKind.Type, BitVector.Null) != 0)
			{
				num++;
			}
			if (DetectReferencesToGenericParameters(originalDefinition.ReturnType, TypeParameterKind.Type, BitVector.Null) != 0)
			{
				num++;
			}
			return num;
		}

		internal static bool IsWidening(MethodSymbol method)
		{
			char c = method.Name[3];
			if (c == 'I' || c == 'i')
			{
				return true;
			}
			return false;
		}

		private static void CollectUserDefinedConversionOperators(TypeSymbol source, TypeSymbol destination, ArrayBuilder<MethodSymbol> opSet, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			CollectUserDefinedOperators(source, destination, MethodKind.Conversion, "op_Implicit", new OperatorInfo(UnaryOperatorKind.Implicit), "op_Explicit", new OperatorInfo(UnaryOperatorKind.Explicit), opSet, ref useSiteInfo);
		}

		internal static void CollectUserDefinedOperators(TypeSymbol type1, TypeSymbol type2, MethodKind opKind, string name1, OperatorInfo name1Info, string name2Opt, OperatorInfo name2InfoOpt, ArrayBuilder<MethodSymbol> opSet, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			type1 = GetTypeToLookForOperatorsIn(type1, ref useSiteInfo);
			if ((object)type2 != null)
			{
				type2 = GetTypeToLookForOperatorsIn(type2, ref useSiteInfo);
			}
			NamedTypeSymbol namedTypeSymbol = null;
			if ((object)type1 != null && type1.Kind == SymbolKind.NamedType && !TypeSymbolExtensions.IsInterfaceType(type1))
			{
				NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)type1;
				do
				{
					if ((object)type2 != null && (object)namedTypeSymbol == null && TypeSymbolExtensions.IsOrDerivedFrom(type2, namedTypeSymbol2, ref useSiteInfo))
					{
						namedTypeSymbol = namedTypeSymbol2;
					}
					if (CollectUserDefinedOperators(namedTypeSymbol2, name1, opKind, name1Info, opSet) | (name2Opt != null && CollectUserDefinedOperators(namedTypeSymbol2, name2Opt, opKind, name2InfoOpt, opSet)))
					{
						break;
					}
					namedTypeSymbol2 = namedTypeSymbol2.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
				}
				while ((object)namedTypeSymbol2 != null);
			}
			if ((object)type2 == null || type2.Kind != SymbolKind.NamedType || TypeSymbolExtensions.IsInterfaceType(type2))
			{
				return;
			}
			NamedTypeSymbol namedTypeSymbol3 = (NamedTypeSymbol)type2;
			while (((object)namedTypeSymbol == null || !TypeSymbolExtensions.IsSameTypeIgnoringAll(namedTypeSymbol, namedTypeSymbol3)) && !(CollectUserDefinedOperators(namedTypeSymbol3, name1, opKind, name1Info, opSet) | (name2Opt != null && CollectUserDefinedOperators(namedTypeSymbol3, name2Opt, opKind, name2InfoOpt, opSet))))
			{
				namedTypeSymbol3 = namedTypeSymbol3.BaseTypeWithDefinitionUseSiteDiagnostics(ref useSiteInfo);
				if ((object)namedTypeSymbol3 == null)
				{
					break;
				}
			}
		}

		private static bool CollectUserDefinedOperators(TypeSymbol type, string opName, MethodKind opKind, OperatorInfo opInfo, ArrayBuilder<MethodSymbol> opSet)
		{
			bool result = false;
			ImmutableArray<Symbol>.Enumerator enumerator = type.GetMembers(opName).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind != SymbolKind.Method)
				{
					continue;
				}
				MethodSymbol methodSymbol = (MethodSymbol)current;
				if (methodSymbol.MethodKind == opKind)
				{
					if (SymbolExtensions.IsShadows(methodSymbol))
					{
						result = true;
					}
					if (!methodSymbol.IsMethodKindBasedOnSyntax || ValidateOverloadedOperator(methodSymbol.OriginalDefinition, opInfo))
					{
						opSet.Add(methodSymbol);
					}
				}
			}
			return result;
		}

		private static TypeSymbol GetTypeToLookForOperatorsIn(TypeSymbol type, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			type = TypeSymbolExtensions.GetNullableUnderlyingTypeOrSelf(type);
			if (type.Kind == SymbolKind.TypeParameter)
			{
				type = ConstraintsHelper.GetNonInterfaceConstraint((TypeParameterSymbol)type, ref useSiteInfo);
			}
			return type;
		}

		public static OverloadResolutionResult ResolveIsTrueOperator(BoundExpression argument, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			CollectUserDefinedOperators(argument.Type, null, MethodKind.UserDefinedOperator, "op_True", new OperatorInfo(UnaryOperatorKind.IsTrue), null, default(OperatorInfo), instance, ref useSiteInfo);
			OverloadResolutionResult result = OperatorInvocationOverloadResolution(instance, argument, null, binder, lateBindingIsAllowed: false, includeEliminatedCandidates: false, ref useSiteInfo);
			instance.Free();
			return result;
		}

		public static OverloadResolutionResult ResolveIsFalseOperator(BoundExpression argument, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			CollectUserDefinedOperators(argument.Type, null, MethodKind.UserDefinedOperator, "op_False", new OperatorInfo(UnaryOperatorKind.IsFalse), null, default(OperatorInfo), instance, ref useSiteInfo);
			OverloadResolutionResult result = OperatorInvocationOverloadResolution(instance, argument, null, binder, lateBindingIsAllowed: false, includeEliminatedCandidates: false, ref useSiteInfo);
			instance.Free();
			return result;
		}

		public static OverloadResolutionResult ResolveUserDefinedUnaryOperator(BoundExpression argument, UnaryOperatorKind opKind, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool includeEliminatedCandidates = false)
		{
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			switch (opKind)
			{
			case UnaryOperatorKind.Not:
			{
				OperatorInfo operatorInfo = new OperatorInfo(UnaryOperatorKind.Not);
				CollectUserDefinedOperators(argument.Type, null, MethodKind.UserDefinedOperator, "op_OnesComplement", operatorInfo, "op_LogicalNot", operatorInfo, instance, ref useSiteInfo);
				break;
			}
			case UnaryOperatorKind.Minus:
				CollectUserDefinedOperators(argument.Type, null, MethodKind.UserDefinedOperator, "op_UnaryNegation", new OperatorInfo(UnaryOperatorKind.Minus), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case UnaryOperatorKind.Plus:
				CollectUserDefinedOperators(argument.Type, null, MethodKind.UserDefinedOperator, "op_UnaryPlus", new OperatorInfo(UnaryOperatorKind.Minus), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(opKind);
			}
			OverloadResolutionResult result = OperatorInvocationOverloadResolution(instance, argument, null, binder, lateBindingIsAllowed: false, includeEliminatedCandidates, ref useSiteInfo);
			instance.Free();
			return result;
		}

		public static OverloadResolutionResult ResolveUserDefinedBinaryOperator(BoundExpression left, BoundExpression right, BinaryOperatorKind opKind, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool includeEliminatedCandidates = false)
		{
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			switch (opKind)
			{
			case BinaryOperatorKind.Add:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Addition", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.Subtract:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Subtraction", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.Multiply:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Multiply", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.Divide:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Division", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.IntegerDivide:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_IntegerDivision", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.Modulo:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Modulus", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.Power:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Exponent", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.Equals:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Equality", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.NotEquals:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Inequality", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.LessThan:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_LessThan", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.GreaterThan:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_GreaterThan", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.LessThanOrEqual:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_LessThanOrEqual", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.GreaterThanOrEqual:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_GreaterThanOrEqual", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.Like:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Like", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.Concatenate:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_Concatenate", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.And:
			case BinaryOperatorKind.AndAlso:
			{
				OperatorInfo operatorInfo4 = new OperatorInfo(opKind);
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_BitwiseAnd", operatorInfo4, "op_LogicalAnd", operatorInfo4, instance, ref useSiteInfo);
				break;
			}
			case BinaryOperatorKind.Or:
			case BinaryOperatorKind.OrElse:
			{
				OperatorInfo operatorInfo3 = new OperatorInfo(opKind);
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_BitwiseOr", operatorInfo3, "op_LogicalOr", operatorInfo3, instance, ref useSiteInfo);
				break;
			}
			case BinaryOperatorKind.Xor:
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_ExclusiveOr", new OperatorInfo(opKind), null, default(OperatorInfo), instance, ref useSiteInfo);
				break;
			case BinaryOperatorKind.LeftShift:
			{
				OperatorInfo operatorInfo2 = new OperatorInfo(opKind);
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_LeftShift", operatorInfo2, "op_UnsignedLeftShift", operatorInfo2, instance, ref useSiteInfo);
				break;
			}
			case BinaryOperatorKind.RightShift:
			{
				OperatorInfo operatorInfo = new OperatorInfo(opKind);
				CollectUserDefinedOperators(left.Type, right.Type, MethodKind.UserDefinedOperator, "op_RightShift", operatorInfo, "op_UnsignedRightShift", operatorInfo, instance, ref useSiteInfo);
				break;
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(opKind);
			}
			OverloadResolutionResult result = OperatorInvocationOverloadResolution(instance, left, right, binder, lateBindingIsAllowed: true, includeEliminatedCandidates, ref useSiteInfo);
			instance.Free();
			return result;
		}

		private static OverloadResolutionResult OperatorInvocationOverloadResolution(ArrayBuilder<MethodSymbol> opSet, BoundExpression argument1, BoundExpression argument2, Binder binder, bool lateBindingIsAllowed, bool includeEliminatedCandidates, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (opSet.Count != 0)
			{
				NamedTypeSymbol specialType = opSet[0].ContainingAssembly.GetSpecialType(SpecialType.System_Nullable_T);
				bool flag = specialType.GetUseSiteInfo().DiagnosticInfo == null;
				ArrayBuilder<CandidateAnalysisResult> instance = ArrayBuilder<CandidateAnalysisResult>.GetInstance();
				ArrayBuilder<MethodSymbol>.Enumerator enumerator = opSet.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MethodSymbol current = enumerator.Current;
					if (current.HasUnsupportedMetadata)
					{
						if (includeEliminatedCandidates)
						{
							instance.Add(new CandidateAnalysisResult(new OperatorCandidate(current), CandidateAnalysisResultState.HasUnsupportedMetadata));
						}
						continue;
					}
					UseSiteInfo<AssemblySymbol> useSiteInfo2 = current.GetUseSiteInfo();
					useSiteInfo.Add(useSiteInfo2);
					if (useSiteInfo2.DiagnosticInfo != null)
					{
						if (includeEliminatedCandidates)
						{
							instance.Add(new CandidateAnalysisResult(new OperatorCandidate(current), CandidateAnalysisResultState.HasUseSiteError));
						}
						continue;
					}
					CombineCandidates(instance, new CandidateAnalysisResult(new OperatorCandidate(current)), current.ParameterCount, default(ImmutableArray<string>), ref useSiteInfo);
					if (!flag)
					{
						continue;
					}
					ParameterSymbol parameterSymbol = current.Parameters[0];
					TypeSymbol type = parameterSymbol.Type;
					bool flag2 = TypeSymbolExtensions.IsNullableType(type);
					bool flag3 = !flag2 && type.IsValueType && !TypeSymbolExtensions.IsRestrictedType(type);
					ParameterSymbol parameterSymbol2 = null;
					TypeSymbol typeSymbol = null;
					bool flag4 = false;
					bool flag5 = false;
					if (argument2 != null && !flag2)
					{
						parameterSymbol2 = current.Parameters[1];
						typeSymbol = parameterSymbol2.Type;
						flag4 = TypeSymbolExtensions.IsNullableType(typeSymbol);
						flag5 = !flag4 && typeSymbol.IsValueType && !TypeSymbolExtensions.IsRestrictedType(typeSymbol);
					}
					if ((flag3 || flag5) && !flag2 && !flag4)
					{
						if (flag3)
						{
							parameterSymbol = LiftParameterSymbol(parameterSymbol, specialType);
						}
						if (flag5)
						{
							parameterSymbol2 = LiftParameterSymbol(parameterSymbol2, specialType);
						}
						TypeSymbol typeSymbol2 = current.ReturnType;
						if (CanLiftType(typeSymbol2))
						{
							typeSymbol2 = specialType.Construct(typeSymbol2);
						}
						CombineCandidates(instance, new CandidateAnalysisResult(new LiftedOperatorCandidate(current, (argument2 == null) ? ImmutableArray.Create(parameterSymbol) : ImmutableArray.Create(parameterSymbol, parameterSymbol2), typeSymbol2)), current.ParameterCount, default(ImmutableArray<string>), ref useSiteInfo);
					}
				}
				BoundMethodGroup methodOrPropertyGroup = new BoundMethodGroup(argument1.Syntax, null, ImmutableArray<MethodSymbol>.Empty, LookupResultKind.Good, null, QualificationKind.Unqualified);
				ImmutableArray<BoundExpression> arguments = ((argument2 == null) ? ImmutableArray.Create(argument1) : ImmutableArray.Create(argument1, argument2));
				HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch = null;
				OverloadResolutionResult result = ResolveOverloading(methodOrPropertyGroup, instance, arguments, default(ImmutableArray<string>), null, lateBindingIsAllowed, binder, ref asyncLambdaSubToFunctionMismatch, null, forceExpandedForm: false, ref useSiteInfo);
				instance.Free();
				return result;
			}
			OverloadResolutionResult result2 = ((!lateBindingIsAllowed) ? new OverloadResolutionResult(ImmutableArray<CandidateAnalysisResult>.Empty, resolutionIsLateBound: false, remainingCandidatesRequireNarrowingConversion: false, null) : new OverloadResolutionResult(ImmutableArray<CandidateAnalysisResult>.Empty, TypeSymbolExtensions.IsObjectType(argument1.Type) || TypeSymbolExtensions.IsObjectType(argument2.Type), remainingCandidatesRequireNarrowingConversion: false, null));
			return result2;
		}

		internal static bool CanLiftType(TypeSymbol type)
		{
			if (!TypeSymbolExtensions.IsNullableType(type) && type.IsValueType)
			{
				return !TypeSymbolExtensions.IsRestrictedType(type);
			}
			return false;
		}

		internal static bool IsValidInLiftedSignature(TypeSymbol type)
		{
			return TypeSymbolExtensions.IsNullableType(type);
		}

		private static ParameterSymbol LiftParameterSymbol(ParameterSymbol param, NamedTypeSymbol nullableOfT)
		{
			if (param.IsDefinition)
			{
				return new LiftedParameterSymbol(param, nullableOfT.Construct(param.Type));
			}
			ParameterSymbol originalDefinition = param.OriginalDefinition;
			return SubstitutedParameterSymbol.CreateMethodParameter((SubstitutedMethodSymbol)param.ContainingSymbol, new LiftedParameterSymbol(originalDefinition, nullableOfT.Construct(originalDefinition.Type)));
		}

		private OverloadResolution()
		{
			throw ExceptionUtilities.Unreachable;
		}

		public static OverloadResolutionResult MethodOrPropertyInvocationOverloadResolution(BoundMethodOrPropertyGroup group, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, Binder binder, SyntaxNode callerInfoOpt, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool includeEliminatedCandidates = false, bool forceExpandedForm = false)
		{
			if (group.Kind == BoundKind.MethodGroup)
			{
				return MethodInvocationOverloadResolution((BoundMethodGroup)group, arguments, argumentNames, binder, callerInfoOpt, ref useSiteInfo, includeEliminatedCandidates, null, null, lateBindingIsAllowed: true, isQueryOperatorInvocation: false, forceExpandedForm);
			}
			return PropertyInvocationOverloadResolution((BoundPropertyGroup)group, arguments, argumentNames, binder, callerInfoOpt, ref useSiteInfo, includeEliminatedCandidates);
		}

		public static OverloadResolutionResult QueryOperatorInvocationOverloadResolution(BoundMethodGroup methodGroup, ImmutableArray<BoundExpression> arguments, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool includeEliminatedCandidates = false)
		{
			return MethodInvocationOverloadResolution(methodGroup, arguments, default(ImmutableArray<string>), binder, null, ref useSiteInfo, includeEliminatedCandidates, null, null, lateBindingIsAllowed: false, isQueryOperatorInvocation: true);
		}

		public static OverloadResolutionResult MethodInvocationOverloadResolution(BoundMethodGroup methodGroup, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, Binder binder, SyntaxNode callerInfoOpt, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool includeEliminatedCandidates = false, TypeSymbol delegateReturnType = null, BoundNode delegateReturnTypeReferenceBoundNode = null, bool lateBindingIsAllowed = true, bool isQueryOperatorInvocation = false, bool forceExpandedForm = false)
		{
			ImmutableArray<TypeSymbol> typeArguments = ((methodGroup.TypeArgumentsOpt != null) ? methodGroup.TypeArgumentsOpt.Arguments : ImmutableArray<TypeSymbol>.Empty);
			if (typeArguments.IsDefault)
			{
				typeArguments = ImmutableArray<TypeSymbol>.Empty;
			}
			if (arguments.IsDefault)
			{
				arguments = ImmutableArray<BoundExpression>.Empty;
			}
			ArrayBuilder<CandidateAnalysisResult> instance = ArrayBuilder<CandidateAnalysisResult>.GetInstance();
			ArrayBuilder<Candidate> instance2 = ArrayBuilder<Candidate>.GetInstance();
			ArrayBuilder<Candidate> instance3 = ArrayBuilder<Candidate>.GetInstance();
			ImmutableArray<MethodSymbol> methods = methodGroup.Methods;
			if (!methods.IsDefault)
			{
				ImmutableArray<MethodSymbol>.Enumerator enumerator = methods.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MethodSymbol current = enumerator.Current;
					if ((object)current.ReducedFrom == null)
					{
						instance2.Add(new MethodCandidate(current));
					}
					else
					{
						instance3.Add(new ExtensionMethodCandidate(current));
					}
				}
			}
			HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch = null;
			int applicableNarrowingCandidates = 0;
			int num = 0;
			if (instance2.Count > 0)
			{
				CollectOverloadedCandidates(binder, instance, instance2, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, isQueryOperatorInvocation, forceExpandedForm, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
				num = EliminateNotApplicableToArguments(methodGroup, instance, arguments, argumentNames, binder, out applicableNarrowingCandidates, ref asyncLambdaSubToFunctionMismatch, callerInfoOpt, forceExpandedForm, ref useSiteInfo);
			}
			instance2.Free();
			instance2 = null;
			bool flag = false;
			if (ShouldConsiderExtensionMethods(instance))
			{
				if (methodGroup.ResultKind == LookupResultKind.Good)
				{
					ImmutableArray<MethodSymbol>.Enumerator enumerator2 = methodGroup.AdditionalExtensionMethods(ref useSiteInfo).GetEnumerator();
					while (enumerator2.MoveNext())
					{
						MethodSymbol current2 = enumerator2.Current;
						instance3.Add(new ExtensionMethodCandidate(current2));
					}
				}
				if (instance3.Count > 0)
				{
					flag = true;
					CollectOverloadedCandidates(binder, instance, instance3, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, isQueryOperatorInvocation, forceExpandedForm, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
				}
			}
			instance3.Free();
			OverloadResolutionResult result = ((num != 0 || flag) ? ResolveOverloading(methodGroup, instance, arguments, argumentNames, delegateReturnType, lateBindingIsAllowed, binder, ref asyncLambdaSubToFunctionMismatch, callerInfoOpt, forceExpandedForm, ref useSiteInfo) : ReportOverloadResolutionFailedOrLateBound(instance, num, lateBindingIsAllowed && binder.OptionStrict != OptionStrict.On, asyncLambdaSubToFunctionMismatch));
			instance.Free();
			return result;
		}

		private static OverloadResolutionResult ReportOverloadResolutionFailedOrLateBound(ArrayBuilder<CandidateAnalysisResult> candidates, int applicableNarrowingCandidateCount, bool lateBindingIsAllowed, HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch)
		{
			bool resolutionIsLateBound = false;
			if (lateBindingIsAllowed)
			{
				ArrayBuilder<CandidateAnalysisResult>.Enumerator enumerator = candidates.GetEnumerator();
				while (enumerator.MoveNext())
				{
					CandidateAnalysisResult current = enumerator.Current;
					if (current.State == CandidateAnalysisResultState.TypeInferenceFailed && current.AllFailedInferenceIsDueToObject && !current.Candidate.IsExtensionMethod)
					{
						resolutionIsLateBound = true;
						break;
					}
				}
			}
			return new OverloadResolutionResult(candidates.ToImmutable(), resolutionIsLateBound, applicableNarrowingCandidateCount > 0, asyncLambdaSubToFunctionMismatch);
		}

		public static OverloadResolutionResult PropertyInvocationOverloadResolution(BoundPropertyGroup propertyGroup, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, Binder binder, SyntaxNode callerInfoOpt, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool includeEliminatedCandidates = false)
		{
			ImmutableArray<PropertySymbol> properties = propertyGroup.Properties;
			if (arguments.IsDefault)
			{
				arguments = ImmutableArray<BoundExpression>.Empty;
			}
			ArrayBuilder<CandidateAnalysisResult> instance = ArrayBuilder<CandidateAnalysisResult>.GetInstance();
			ArrayBuilder<Candidate> instance2 = ArrayBuilder<Candidate>.GetInstance(properties.Length - 1);
			int num = properties.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				instance2.Add(new PropertyCandidate(properties[i]));
			}
			HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch = null;
			CollectOverloadedCandidates(binder, instance, instance2, ImmutableArray<TypeSymbol>.Empty, arguments, argumentNames, null, null, includeEliminatedCandidates, isQueryOperatorInvocation: false, forceExpandedForm: false, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
			instance2.Free();
			OverloadResolutionResult result = ResolveOverloading(propertyGroup, instance, arguments, argumentNames, null, lateBindingIsAllowed: true, binder, ref asyncLambdaSubToFunctionMismatch, callerInfoOpt, forceExpandedForm: false, ref useSiteInfo);
			instance.Free();
			return result;
		}

		private static bool ShouldConsiderExtensionMethods(ArrayBuilder<CandidateAnalysisResult> candidates)
		{
			int num = candidates.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (candidates[i].IgnoreExtensionMethods)
				{
					return false;
				}
			}
			return true;
		}

		private static OverloadResolutionResult ResolveOverloading(BoundMethodOrPropertyGroup methodOrPropertyGroup, ArrayBuilder<CandidateAnalysisResult> candidates, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, TypeSymbol delegateReturnType, bool lateBindingIsAllowed, Binder binder, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, SyntaxNode callerInfoOpt, bool forceExpandedForm, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool resolutionIsLateBound = false;
			bool remainingCandidatesRequireNarrowingConversion = false;
			int applicableNarrowingCandidates = 0;
			int applicableCandidates = EliminateNotApplicableToArguments(methodOrPropertyGroup, candidates, arguments, argumentNames, binder, out applicableNarrowingCandidates, ref asyncLambdaSubToFunctionMismatch, callerInfoOpt, forceExpandedForm, ref useSiteInfo);
			if (applicableCandidates < 2)
			{
				remainingCandidatesRequireNarrowingConversion = applicableNarrowingCandidates > 0;
			}
			else
			{
				applicableCandidates = ShadowBasedOnDelegateRelaxation(candidates, ref applicableNarrowingCandidates);
				if (applicableCandidates < 2)
				{
					remainingCandidatesRequireNarrowingConversion = applicableNarrowingCandidates > 0;
				}
				else
				{
					ShadowBasedOnInferenceLevel(candidates, arguments, !argumentNames.IsDefault, delegateReturnType, binder, ref applicableCandidates, ref applicableNarrowingCandidates, ref useSiteInfo);
					if (applicableCandidates < 2)
					{
						remainingCandidatesRequireNarrowingConversion = applicableNarrowingCandidates > 0;
					}
					else if (applicableCandidates == applicableNarrowingCandidates)
					{
						remainingCandidatesRequireNarrowingConversion = true;
						applicableCandidates = AnalyzeNarrowingCandidates(candidates, arguments, delegateReturnType, lateBindingIsAllowed && binder.OptionStrict != OptionStrict.On, binder, ref resolutionIsLateBound, ref useSiteInfo);
					}
					else
					{
						if (applicableNarrowingCandidates > 0)
						{
							applicableCandidates = EliminateNarrowingCandidates(candidates);
							if (applicableCandidates < 2)
							{
								goto IL_00b5;
							}
						}
						applicableCandidates = EliminateLessApplicableToTheArguments(candidates, arguments, delegateReturnType, appliedTieBreakingRules: false, binder, ref useSiteInfo);
					}
				}
			}
			goto IL_00b5;
			IL_00b5:
			if (!resolutionIsLateBound && applicableCandidates == 0)
			{
				return ReportOverloadResolutionFailedOrLateBound(candidates, applicableCandidates, lateBindingIsAllowed && binder.OptionStrict != OptionStrict.On, asyncLambdaSubToFunctionMismatch);
			}
			return new OverloadResolutionResult(candidates.ToImmutable(), resolutionIsLateBound, remainingCandidatesRequireNarrowingConversion, asyncLambdaSubToFunctionMismatch);
		}

		private static int EliminateNarrowingCandidates(ArrayBuilder<CandidateAnalysisResult> candidates)
		{
			int num = 0;
			int num2 = candidates.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				CandidateAnalysisResult value = candidates[i];
				if (value.State == CandidateAnalysisResultState.Applicable)
				{
					if (value.RequiresNarrowingConversion)
					{
						value.State = CandidateAnalysisResultState.RequiresNarrowing;
						candidates[i] = value;
					}
					else
					{
						num++;
					}
				}
			}
			return num;
		}

		private static int EliminateLessApplicableToTheArguments(ArrayBuilder<CandidateAnalysisResult> candidates, ImmutableArray<BoundExpression> arguments, TypeSymbol delegateReturnType, bool appliedTieBreakingRules, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool mostApplicableMustNarrowOnlyFromNumericConstants = false)
		{
			ArrayBuilder<int> instance = ArrayBuilder<int>.GetInstance();
			int result;
			if (!FastFindMostApplicableCandidates(candidates, arguments, instance, binder, ref useSiteInfo) || (mostApplicableMustNarrowOnlyFromNumericConstants && candidates[instance[0]].RequiresNarrowingNotFromNumericConstant && instance.Count != CountApplicableCandidates(candidates)))
			{
				result = (appliedTieBreakingRules ? CountApplicableCandidates(candidates) : ApplyTieBreakingRulesToEquallyApplicableCandidates(candidates, arguments, delegateReturnType, binder, ref useSiteInfo));
			}
			else
			{
				int num = 0;
				int num2 = instance[num];
				int num3 = candidates.Count - 1;
				for (int i = 0; i <= num3; i++)
				{
					if (i == num2)
					{
						num++;
						num2 = ((num >= instance.Count) ? candidates.Count : instance[num]);
						continue;
					}
					CandidateAnalysisResult value = candidates[i];
					if (value.State == CandidateAnalysisResultState.Applicable)
					{
						value.State = CandidateAnalysisResultState.LessApplicable;
						candidates[i] = value;
					}
				}
				result = (appliedTieBreakingRules ? instance.Count : ApplyTieBreakingRules(candidates, instance, arguments, delegateReturnType, binder, ref useSiteInfo));
			}
			instance.Free();
			return result;
		}

		private static int CountApplicableCandidates(ArrayBuilder<CandidateAnalysisResult> candidates)
		{
			int num = 0;
			int num2 = candidates.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				if (candidates[i].State == CandidateAnalysisResultState.Applicable)
				{
					num++;
				}
			}
			return num;
		}

		private static int ApplyTieBreakingRulesToEquallyApplicableCandidates(ArrayBuilder<CandidateAnalysisResult> candidates, ImmutableArray<BoundExpression> arguments, TypeSymbol delegateReturnType, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ArrayBuilder<ArrayBuilder<int>> arrayBuilder = GroupEquallyApplicableCandidates(candidates, arguments, binder);
			int num = 0;
			int num2 = arrayBuilder.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				num += ApplyTieBreakingRules(candidates, arrayBuilder[i], arguments, delegateReturnType, binder, ref useSiteInfo);
			}
			int num3 = arrayBuilder.Count - 1;
			for (int j = 0; j <= num3; j++)
			{
				arrayBuilder[j].Free();
			}
			arrayBuilder.Free();
			return num;
		}

		private static bool FastFindMostApplicableCandidates(ArrayBuilder<CandidateAnalysisResult> candidates, ImmutableArray<BoundExpression> arguments, ArrayBuilder<int> indexesOfMostApplicableCandidates, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			int num = -1;
			CandidateAnalysisResult left = default(CandidateAnalysisResult);
			indexesOfMostApplicableCandidates.Clear();
			int num2 = candidates.Count - 1;
			for (int i = 0; i <= num2; i++)
			{
				CandidateAnalysisResult right = candidates[i];
				if (right.State != 0)
				{
					continue;
				}
				if (num == -1)
				{
					num = i;
					left = right;
					indexesOfMostApplicableCandidates.Add(i);
					continue;
				}
				switch (CompareApplicabilityToTheArguments(ref left, ref right, arguments, binder, ref useSiteInfo))
				{
				case ApplicabilityComparisonResult.RightIsMoreApplicable:
					num = i;
					left = right;
					indexesOfMostApplicableCandidates.Clear();
					indexesOfMostApplicableCandidates.Add(i);
					break;
				case ApplicabilityComparisonResult.Undefined:
					num = -1;
					indexesOfMostApplicableCandidates.Clear();
					break;
				case ApplicabilityComparisonResult.EquallyApplicable:
					indexesOfMostApplicableCandidates.Add(i);
					break;
				}
			}
			int num3 = num - 1;
			for (int j = 0; j <= num3; j++)
			{
				CandidateAnalysisResult right2 = candidates[j];
				if (right2.State == CandidateAnalysisResultState.Applicable)
				{
					ApplicabilityComparisonResult applicabilityComparisonResult = CompareApplicabilityToTheArguments(ref left, ref right2, arguments, binder, ref useSiteInfo);
					if (applicabilityComparisonResult == ApplicabilityComparisonResult.RightIsMoreApplicable || applicabilityComparisonResult == ApplicabilityComparisonResult.Undefined || applicabilityComparisonResult == ApplicabilityComparisonResult.EquallyApplicable)
					{
						num = -1;
						break;
					}
				}
			}
			return num > -1;
		}

		private static int ApplyTieBreakingRules(ArrayBuilder<CandidateAnalysisResult> candidates, ArrayBuilder<int> bucket, ImmutableArray<BoundExpression> arguments, TypeSymbol delegateReturnType, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			int num = bucket.Count;
			int num2 = bucket.Count - 1;
			bool leftWins = default(bool);
			bool rightWins = default(bool);
			for (int i = 0; i <= num2; i++)
			{
				CandidateAnalysisResult candidateAnalysisResult = candidates[bucket[i]];
				if (candidateAnalysisResult.State != 0)
				{
					continue;
				}
				int num3 = i + 1;
				int num4 = bucket.Count - 1;
				for (int j = num3; j <= num4; j++)
				{
					CandidateAnalysisResult candidateAnalysisResult2 = candidates[bucket[j]];
					if (candidateAnalysisResult2.State == CandidateAnalysisResultState.Applicable && ShadowBasedOnTieBreakingRules(candidateAnalysisResult, candidateAnalysisResult2, arguments, delegateReturnType, ref leftWins, ref rightWins, binder, ref useSiteInfo))
					{
						if (!leftWins)
						{
							candidateAnalysisResult.State = CandidateAnalysisResultState.Shadowed;
							candidates[bucket[i]] = candidateAnalysisResult;
							num--;
							break;
						}
						candidateAnalysisResult2.State = CandidateAnalysisResultState.Shadowed;
						candidates[bucket[j]] = candidateAnalysisResult2;
						num--;
					}
				}
			}
			return num;
		}

		private static bool ShadowBasedOnTieBreakingRules(CandidateAnalysisResult left, CandidateAnalysisResult right, ImmutableArray<BoundExpression> arguments, TypeSymbol delegateReturnType, ref bool leftWins, ref bool rightWins, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			leftWins = false;
			rightWins = false;
			if (ShadowBasedOnParamArrayUsage(left, right, ref leftWins, ref rightWins))
			{
				return true;
			}
			if (ShadowBasedOnReceiverType(left, right, ref leftWins, ref rightWins, ref useSiteInfo))
			{
				return true;
			}
			if (ShadowBasedOnExtensionMethodTargetTypeGenericity(left, right, ref leftWins, ref rightWins))
			{
				return true;
			}
			if (ShadowBasedOnGenericity(left, right, ref leftWins, ref rightWins, arguments, binder))
			{
				return true;
			}
			if (ShadowBasedOnExtensionVsInstanceAndPrecedence(left, right, ref leftWins, ref rightWins))
			{
				return true;
			}
			if (ShadowBasedOnOptionalParametersDefaultsUsed(left, right, ref leftWins, ref rightWins))
			{
				return true;
			}
			if (ShadowBasedOnSubOrFunction(left, right, delegateReturnType, ref leftWins, ref rightWins))
			{
				return true;
			}
			if (ShadowBasedOnDepthOfGenericity(left, right, ref leftWins, ref rightWins, arguments, binder))
			{
				return true;
			}
			return false;
		}

		private static bool ShadowBasedOnSubOrFunction(CandidateAnalysisResult left, CandidateAnalysisResult right, TypeSymbol delegateReturnType, ref bool leftWins, ref bool rightWins)
		{
			if ((object)delegateReturnType == null)
			{
				return false;
			}
			bool flag = TypeSymbolExtensions.IsVoidType(left.Candidate.ReturnType);
			bool flag2 = TypeSymbolExtensions.IsVoidType(right.Candidate.ReturnType);
			if (flag == flag2)
			{
				return false;
			}
			if (TypeSymbolExtensions.IsVoidType(delegateReturnType) == flag)
			{
				leftWins = true;
				return true;
			}
			rightWins = true;
			return true;
		}

		private static int ShadowBasedOnDelegateRelaxation(ArrayBuilder<CandidateAnalysisResult> candidates, ref int applicableNarrowingCandidates)
		{
			ConversionKind conversionKind = ConversionKind.DelegateRelaxationLevelInvalid;
			int num = candidates.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				CandidateAnalysisResult candidateAnalysisResult = candidates[i];
				if (candidateAnalysisResult.State == CandidateAnalysisResultState.Applicable)
				{
					ConversionKind maxDelegateRelaxationLevel = candidateAnalysisResult.MaxDelegateRelaxationLevel;
					if (maxDelegateRelaxationLevel < conversionKind)
					{
						conversionKind = maxDelegateRelaxationLevel;
					}
				}
			}
			int num2 = 0;
			applicableNarrowingCandidates = 0;
			int num3 = candidates.Count - 1;
			for (int j = 0; j <= num3; j++)
			{
				CandidateAnalysisResult value = candidates[j];
				if (value.State != 0)
				{
					continue;
				}
				if (value.MaxDelegateRelaxationLevel > conversionKind)
				{
					value.State = CandidateAnalysisResultState.Shadowed;
					candidates[j] = value;
					continue;
				}
				num2++;
				if (value.RequiresNarrowingConversion)
				{
					applicableNarrowingCandidates++;
				}
			}
			return num2;
		}

		private static bool ShadowBasedOnOptionalParametersDefaultsUsed(CandidateAnalysisResult left, CandidateAnalysisResult right, ref bool leftWins, ref bool rightWins)
		{
			bool usedOptionalParameterDefaultValue = left.UsedOptionalParameterDefaultValue;
			if (usedOptionalParameterDefaultValue == right.UsedOptionalParameterDefaultValue)
			{
				return false;
			}
			if (!usedOptionalParameterDefaultValue)
			{
				leftWins = true;
			}
			else
			{
				rightWins = true;
			}
			return true;
		}

		private static void ShadowBasedOnInferenceLevel(ArrayBuilder<CandidateAnalysisResult> candidates, ImmutableArray<BoundExpression> arguments, bool haveNamedArguments, TypeSymbol delegateReturnType, Binder binder, ref int applicableCandidates, ref int applicableNarrowingCandidates, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool flag = false;
			TypeArgumentInference.InferenceLevel inferenceLevel = (TypeArgumentInference.InferenceLevel)255;
			int num = candidates.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				CandidateAnalysisResult candidateAnalysisResult = candidates[i];
				if (candidateAnalysisResult.State == CandidateAnalysisResultState.Applicable)
				{
					TypeArgumentInference.InferenceLevel inferenceLevel2 = candidateAnalysisResult.InferenceLevel;
					if (inferenceLevel == (TypeArgumentInference.InferenceLevel)255)
					{
						inferenceLevel = inferenceLevel2;
					}
					else if (inferenceLevel2 != inferenceLevel)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				return;
			}
			if (haveNamedArguments)
			{
				ArrayBuilder<int> instance = ArrayBuilder<int>.GetInstance(applicableCandidates);
				int num2 = candidates.Count - 1;
				for (int j = 0; j <= num2; j++)
				{
					if (candidates[j].State == CandidateAnalysisResultState.Applicable)
					{
						instance.Add(j);
					}
				}
				instance.Sort(new InferenceLevelComparer(candidates));
				int num3 = instance.Count - 2;
				for (int k = 0; k <= num3; k++)
				{
					CandidateAnalysisResult candidate = candidates[instance[k]];
					if (candidate.State != 0)
					{
						continue;
					}
					int num4 = k + 1;
					int num5 = instance.Count - 1;
					for (int l = num4; l <= num5; l++)
					{
						CandidateAnalysisResult candidate2 = candidates[instance[l]];
						if (candidate2.State != 0)
						{
							continue;
						}
						bool flag2 = true;
						int num6 = arguments.Length - 1;
						for (int m = 0; m <= num6; m++)
						{
							TypeSymbol parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref candidate, candidate.ArgsToParamsOpt[m]);
							TypeSymbol parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref candidate2, candidate2.ArgsToParamsOpt[m]);
							if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(parameterTypeFromVirtualSignature, parameterTypeFromVirtualSignature2))
							{
								flag2 = false;
								break;
							}
						}
						if (!flag2)
						{
							continue;
						}
						bool flag3 = true;
						if (candidate.Candidate.ParameterCount != candidate2.Candidate.ParameterCount)
						{
							flag3 = false;
						}
						else
						{
							int num7 = candidate.Candidate.ParameterCount - 1;
							for (int n = 0; n <= num7; n++)
							{
								TypeSymbol type = candidate.Candidate.Parameters(n).Type;
								TypeSymbol type2 = candidate2.Candidate.Parameters(n).Type;
								if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(type, type2))
								{
									flag3 = false;
									break;
								}
							}
						}
						bool leftWins = false;
						bool rightWins = false;
						if ((!flag3 && ShadowBasedOnParamArrayUsage(candidate, candidate2, ref leftWins, ref rightWins)) || ShadowBasedOnReceiverType(candidate, candidate2, ref leftWins, ref rightWins, ref useSiteInfo) || ShadowBasedOnExtensionMethodTargetTypeGenericity(candidate, candidate2, ref leftWins, ref rightWins))
						{
							if (leftWins)
							{
								candidate2.State = CandidateAnalysisResultState.Shadowed;
								candidates[instance[l]] = candidate2;
							}
							else if (rightWins)
							{
								candidate.State = CandidateAnalysisResultState.Shadowed;
								candidates[instance[k]] = candidate;
								break;
							}
						}
					}
					if (candidate.State == CandidateAnalysisResultState.Applicable)
					{
						break;
					}
				}
			}
			TypeArgumentInference.InferenceLevel inferenceLevel3 = TypeArgumentInference.InferenceLevel.Invalid;
			int num8 = candidates.Count - 1;
			for (int num9 = 0; num9 <= num8; num9++)
			{
				CandidateAnalysisResult candidateAnalysisResult2 = candidates[num9];
				if (candidateAnalysisResult2.State == CandidateAnalysisResultState.Applicable)
				{
					TypeArgumentInference.InferenceLevel inferenceLevel4 = candidateAnalysisResult2.InferenceLevel;
					if (inferenceLevel4 < inferenceLevel3)
					{
						inferenceLevel3 = inferenceLevel4;
					}
				}
			}
			applicableCandidates = 0;
			applicableNarrowingCandidates = 0;
			int num10 = candidates.Count - 1;
			for (int num11 = 0; num11 <= num10; num11++)
			{
				CandidateAnalysisResult value = candidates[num11];
				if (value.State != 0)
				{
					continue;
				}
				if (value.InferenceLevel > inferenceLevel3)
				{
					value.State = CandidateAnalysisResultState.Shadowed;
					candidates[num11] = value;
					continue;
				}
				applicableCandidates++;
				if (value.RequiresNarrowingConversion)
				{
					applicableNarrowingCandidates++;
				}
			}
		}

		private static ApplicabilityComparisonResult CompareApplicabilityToTheArguments(ref CandidateAnalysisResult left, ref CandidateAnalysisResult right, ImmutableArray<BoundExpression> arguments, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			int paramIndex = 0;
			int paramIndex2 = 0;
			int num = arguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSymbol parameterTypeFromVirtualSignature;
				if (left.ArgsToParamsOpt.IsDefault)
				{
					parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref left, paramIndex);
					AdvanceParameterInVirtualSignature(ref left, ref paramIndex);
				}
				else
				{
					parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref left, left.ArgsToParamsOpt[i]);
				}
				TypeSymbol parameterTypeFromVirtualSignature2;
				if (right.ArgsToParamsOpt.IsDefault)
				{
					parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref right, paramIndex2);
					AdvanceParameterInVirtualSignature(ref right, ref paramIndex2);
				}
				else
				{
					parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref right, right.ArgsToParamsOpt[i]);
				}
				if (arguments[i].Kind == BoundKind.OmittedArgument)
				{
					continue;
				}
				switch (CompareParameterTypeApplicability(parameterTypeFromVirtualSignature, parameterTypeFromVirtualSignature2, arguments[i], binder, ref useSiteInfo))
				{
				case ApplicabilityComparisonResult.LeftIsMoreApplicable:
					flag2 = true;
					if (flag3)
					{
						return ApplicabilityComparisonResult.Undefined;
					}
					flag = false;
					break;
				case ApplicabilityComparisonResult.RightIsMoreApplicable:
					flag3 = true;
					if (flag2)
					{
						return ApplicabilityComparisonResult.Undefined;
					}
					flag = false;
					break;
				case ApplicabilityComparisonResult.Undefined:
					flag = false;
					break;
				}
			}
			if (flag2)
			{
				return ApplicabilityComparisonResult.LeftIsMoreApplicable;
			}
			if (flag3)
			{
				return ApplicabilityComparisonResult.RightIsMoreApplicable;
			}
			return flag ? ApplicabilityComparisonResult.EquallyApplicable : ApplicabilityComparisonResult.Undefined;
		}

		private static ApplicabilityComparisonResult CompareParameterTypeApplicability(TypeSymbol left, TypeSymbol right, BoundExpression argument, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			KeyValuePair<ConversionKind, MethodSymbol> keyValuePair = Conversions.ClassifyConversion(left, right, ref useSiteInfo);
			if (Conversions.IsIdentityConversion(keyValuePair.Key))
			{
				return ApplicabilityComparisonResult.EquallyApplicable;
			}
			if (Conversions.IsWideningConversion(keyValuePair.Key))
			{
				if (!Conversions.IsWideningConversion(Conversions.ClassifyConversion(right, left, ref useSiteInfo).Key))
				{
					if (argument != null && BoundExpressionExtensions.IsIntegerZeroLiteral(argument) && left.TypeKind == TypeKind.Enum && right.TypeKind != TypeKind.Enum)
					{
						return ApplicabilityComparisonResult.RightIsMoreApplicable;
					}
					return ApplicabilityComparisonResult.LeftIsMoreApplicable;
				}
			}
			else
			{
				if (Conversions.IsWideningConversion(Conversions.ClassifyConversion(right, left, ref useSiteInfo).Key))
				{
					if (argument != null && BoundExpressionExtensions.IsIntegerZeroLiteral(argument) && right.TypeKind == TypeKind.Enum && left.TypeKind != TypeKind.Enum)
					{
						return ApplicabilityComparisonResult.LeftIsMoreApplicable;
					}
					return ApplicabilityComparisonResult.RightIsMoreApplicable;
				}
				if (TypeSymbolExtensions.IsNumericType(left) && TypeSymbolExtensions.IsNumericType(right))
				{
					SpecialType specialType = left.SpecialType;
					SpecialType specialType2 = right.SpecialType;
					if (specialType == SpecialType.System_Byte && specialType2 == SpecialType.System_SByte)
					{
						return ApplicabilityComparisonResult.LeftIsMoreApplicable;
					}
					if (specialType == SpecialType.System_SByte && specialType2 == SpecialType.System_Byte)
					{
						return ApplicabilityComparisonResult.RightIsMoreApplicable;
					}
					if (specialType < specialType2)
					{
						return ApplicabilityComparisonResult.LeftIsMoreApplicable;
					}
					return ApplicabilityComparisonResult.RightIsMoreApplicable;
				}
				if (argument != null)
				{
					bool wasExpression = default(bool);
					NamedTypeSymbol namedTypeSymbol = TypeSymbolExtensions.DelegateOrExpressionDelegate(left, binder, ref wasExpression);
					bool wasExpression2 = default(bool);
					NamedTypeSymbol namedTypeSymbol2 = TypeSymbolExtensions.DelegateOrExpressionDelegate(right, binder, ref wasExpression2);
					if ((object)namedTypeSymbol != null && (object)namedTypeSymbol2 != null && (wasExpression == wasExpression2 || BoundNodeExtensions.IsAnyLambda(argument)))
					{
						MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
						MethodSymbol delegateInvokeMethod2 = namedTypeSymbol2.DelegateInvokeMethod;
						if ((object)delegateInvokeMethod != null && !delegateInvokeMethod.IsSub && (object)delegateInvokeMethod2 != null && !delegateInvokeMethod2.IsSub)
						{
							BoundExpression argument2 = null;
							if (argument.Kind == BoundKind.QueryLambda)
							{
								argument2 = ((BoundQueryLambda)argument).Expression;
							}
							return CompareParameterTypeApplicability(delegateInvokeMethod.ReturnType, delegateInvokeMethod2.ReturnType, argument2, binder, ref useSiteInfo);
						}
					}
				}
			}
			if (argument != null)
			{
				TypeSymbol typeSymbol = ((argument.Kind != BoundKind.ArrayLiteral) ? argument.Type : ((BoundArrayLiteral)argument).InferredType);
				if ((object)typeSymbol != null)
				{
					if (TypeSymbolExtensions.IsSameTypeIgnoringAll(left, typeSymbol))
					{
						return ApplicabilityComparisonResult.LeftIsMoreApplicable;
					}
					if (TypeSymbolExtensions.IsSameTypeIgnoringAll(right, typeSymbol))
					{
						return ApplicabilityComparisonResult.RightIsMoreApplicable;
					}
				}
			}
			return ApplicabilityComparisonResult.Undefined;
		}

		private static ArrayBuilder<ArrayBuilder<int>> GroupEquallyApplicableCandidates(ArrayBuilder<CandidateAnalysisResult> candidates, ImmutableArray<BoundExpression> arguments, Binder binder)
		{
			ArrayBuilder<ArrayBuilder<int>> instance = ArrayBuilder<ArrayBuilder<int>>.GetInstance();
			int num = candidates.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				CandidateAnalysisResult left = candidates[i];
				if (left.State != 0 || left.EquallyApplicableCandidatesBucket > 0)
				{
					continue;
				}
				left.EquallyApplicableCandidatesBucket = instance.Count + 1;
				candidates[i] = left;
				ArrayBuilder<int> instance2 = ArrayBuilder<int>.GetInstance();
				instance2.Add(i);
				instance.Add(instance2);
				int num2 = i + 1;
				int num3 = candidates.Count - 1;
				for (int j = num2; j <= num3; j++)
				{
					CandidateAnalysisResult right = candidates[j];
					if (right.State == CandidateAnalysisResultState.Applicable && right.EquallyApplicableCandidatesBucket <= 0 && right.Candidate != left.Candidate && CandidatesAreEquallyApplicableToArguments(ref left, ref right, arguments, binder))
					{
						right.EquallyApplicableCandidatesBucket = left.EquallyApplicableCandidatesBucket;
						candidates[j] = right;
						instance2.Add(j);
					}
				}
			}
			return instance;
		}

		private static bool CandidatesAreEquallyApplicableToArguments(ref CandidateAnalysisResult left, ref CandidateAnalysisResult right, ImmutableArray<BoundExpression> arguments, Binder binder)
		{
			int paramIndex = 0;
			int paramIndex2 = 0;
			int num = arguments.Length - 1;
			int i;
			for (i = 0; i <= num; i++)
			{
				TypeSymbol parameterTypeFromVirtualSignature;
				if (left.ArgsToParamsOpt.IsDefault)
				{
					parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref left, paramIndex);
					AdvanceParameterInVirtualSignature(ref left, ref paramIndex);
				}
				else
				{
					parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref left, left.ArgsToParamsOpt[i]);
				}
				TypeSymbol parameterTypeFromVirtualSignature2;
				if (right.ArgsToParamsOpt.IsDefault)
				{
					parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref right, paramIndex2);
					AdvanceParameterInVirtualSignature(ref right, ref paramIndex2);
				}
				else
				{
					parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref right, right.ArgsToParamsOpt[i]);
				}
				if (arguments[i].Kind != BoundKind.OmittedArgument && !ParametersAreEquallyApplicableToArgument(parameterTypeFromVirtualSignature, parameterTypeFromVirtualSignature2, arguments[i], binder))
				{
					break;
				}
			}
			return i >= arguments.Length;
		}

		private static bool ParametersAreEquallyApplicableToArgument(TypeSymbol leftParamType, TypeSymbol rightParamType, BoundExpression argument, Binder binder)
		{
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(leftParamType, rightParamType))
			{
				if (argument != null)
				{
					bool wasExpression = default(bool);
					NamedTypeSymbol namedTypeSymbol = TypeSymbolExtensions.DelegateOrExpressionDelegate(leftParamType, binder, ref wasExpression);
					bool wasExpression2 = default(bool);
					NamedTypeSymbol namedTypeSymbol2 = TypeSymbolExtensions.DelegateOrExpressionDelegate(rightParamType, binder, ref wasExpression2);
					if ((object)namedTypeSymbol != null && (object)namedTypeSymbol2 != null && (wasExpression == wasExpression2 || BoundNodeExtensions.IsAnyLambda(argument)))
					{
						MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
						MethodSymbol delegateInvokeMethod2 = namedTypeSymbol2.DelegateInvokeMethod;
						if ((object)delegateInvokeMethod != null && !delegateInvokeMethod.IsSub && (object)delegateInvokeMethod2 != null && !delegateInvokeMethod2.IsSub)
						{
							BoundExpression argument2 = null;
							if (argument.Kind == BoundKind.QueryLambda)
							{
								argument2 = ((BoundQueryLambda)argument).Expression;
							}
							return ParametersAreEquallyApplicableToArgument(delegateInvokeMethod.ReturnType, delegateInvokeMethod2.ReturnType, argument2, binder);
						}
					}
				}
				return false;
			}
			return true;
		}

		private static int AnalyzeNarrowingCandidates(ArrayBuilder<CandidateAnalysisResult> candidates, ImmutableArray<BoundExpression> arguments, TypeSymbol delegateReturnType, bool lateBindingIsAllowed, Binder binder, ref bool resolutionIsLateBound, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			int num = 0;
			bool flag = false;
			if (candidates[0].Candidate.IsOperator)
			{
				int num2 = candidates.Count - 2;
				int i;
				for (i = 0; i <= num2; i++)
				{
					CandidateAnalysisResult candidateAnalysisResult = candidates[i];
					if (candidateAnalysisResult.State == CandidateAnalysisResultState.Applicable && !candidateAnalysisResult.Candidate.IsLifted && candidateAnalysisResult.RequiresNarrowingNotFromNumericConstant)
					{
						CandidateAnalysisResult candidateAnalysisResult2 = candidates[i + 1];
						if (candidateAnalysisResult2.State == CandidateAnalysisResultState.Applicable && candidateAnalysisResult2.Candidate.IsLifted && (object)candidateAnalysisResult.Candidate.UnderlyingSymbol == candidateAnalysisResult2.Candidate.UnderlyingSymbol)
						{
							break;
						}
					}
				}
				if (i < candidates.Count - 1)
				{
					if (!flag)
					{
						num = ApplyTieBreakingRulesToEquallyApplicableCandidates(candidates, arguments, delegateReturnType, binder, ref useSiteInfo);
						flag = true;
					}
					int num3 = i;
					int num4 = candidates.Count - 2;
					for (i = num3; i <= num4; i++)
					{
						CandidateAnalysisResult value = candidates[i];
						if (value.State != 0 || value.Candidate.IsLifted || !value.RequiresNarrowingNotFromNumericConstant)
						{
							continue;
						}
						CandidateAnalysisResult value2 = candidates[i + 1];
						if (value2.State != 0 || !value2.Candidate.IsLifted || (object)value.Candidate.UnderlyingSymbol != value2.Candidate.UnderlyingSymbol)
						{
							continue;
						}
						int num5 = arguments.Length - 1;
						int num6 = 0;
						while (true)
						{
							if (num6 <= num5)
							{
								KeyValuePair<ConversionKind, MethodSymbol> keyValuePair = value.ConversionsOpt[num6];
								if (Conversions.IsNarrowingConversion(keyValuePair.Key))
								{
									bool flag2 = false;
									if ((keyValuePair.Key & ConversionKind.UserDefined) == 0)
									{
										if (IsUnwrappingNullable(keyValuePair.Key, arguments[num6].Type, value.Candidate.Parameters(num6).Type))
										{
											flag2 = true;
										}
									}
									else if ((keyValuePair.Key & ConversionKind.Nullable) == 0)
									{
										if (IsUnwrappingNullable(arguments[num6].Type, keyValuePair.Value.Parameters[0].Type, ref useSiteInfo))
										{
											flag2 = true;
										}
										else if (IsUnwrappingNullable(keyValuePair.Value.ReturnType, value.Candidate.Parameters(num6).Type, ref useSiteInfo))
										{
											flag2 = true;
										}
									}
									if (flag2)
									{
										value.State = CandidateAnalysisResultState.Shadowed;
										candidates[i] = value;
										i++;
										break;
									}
								}
								num6++;
								continue;
							}
							value2.State = CandidateAnalysisResultState.Shadowed;
							candidates[i + 1] = value2;
							i++;
							break;
						}
					}
				}
			}
			if (lateBindingIsAllowed)
			{
				bool flag3 = HaveNarrowingOnlyFromObjectCandidates(candidates);
				if (flag3 && !flag)
				{
					num = ApplyTieBreakingRulesToEquallyApplicableCandidates(candidates, arguments, delegateReturnType, binder, ref useSiteInfo);
					flag = true;
					if (num < 2)
					{
						return num;
					}
					flag3 = HaveNarrowingOnlyFromObjectCandidates(candidates);
				}
				if (flag3)
				{
					num = 0;
					int num7 = candidates.Count - 1;
					for (int j = 0; j <= num7; j++)
					{
						CandidateAnalysisResult value3 = candidates[j];
						if (value3.State == CandidateAnalysisResultState.Applicable)
						{
							if (value3.RequiresNarrowingNotFromObject || value3.Candidate.IsExtensionMethod)
							{
								value3.State = CandidateAnalysisResultState.ExtensionMethodVsLateBinding;
								candidates[j] = value3;
							}
							else
							{
								num++;
							}
						}
					}
					if (num > 1)
					{
						resolutionIsLateBound = true;
					}
					return num;
				}
			}
			num = EliminateLessApplicableToTheArguments(candidates, arguments, delegateReturnType, flag, binder, ref useSiteInfo, mostApplicableMustNarrowOnlyFromNumericConstants: true);
			if (num == 2)
			{
				int num8 = candidates.Count - 1;
				for (int k = 0; k <= num8; k++)
				{
					CandidateAnalysisResult candidateAnalysisResult3 = candidates[k];
					if (candidateAnalysisResult3.State != 0)
					{
						continue;
					}
					int num9 = k + 1;
					int num10 = candidates.Count - 1;
					int num11 = num9;
					CandidateAnalysisResult candidateAnalysisResult4;
					while (num11 <= num10)
					{
						candidateAnalysisResult4 = candidates[num11];
						if (candidateAnalysisResult4.State != 0)
						{
							num11++;
							continue;
						}
						goto IL_0385;
					}
					continue;
					IL_0385:
					if (!candidateAnalysisResult3.Candidate.UnderlyingSymbol.Equals(candidateAnalysisResult4.Candidate.UnderlyingSymbol))
					{
						break;
					}
					bool leftWins = false;
					bool rightWins = false;
					if (ShadowBasedOnParamArrayUsage(candidateAnalysisResult3, candidateAnalysisResult4, ref leftWins, ref rightWins))
					{
						if (leftWins)
						{
							candidateAnalysisResult4.State = CandidateAnalysisResultState.Shadowed;
							candidates[num11] = candidateAnalysisResult4;
							num = 1;
						}
						else if (rightWins)
						{
							candidateAnalysisResult3.State = CandidateAnalysisResultState.Shadowed;
							candidates[k] = candidateAnalysisResult3;
							num = 1;
						}
					}
					break;
				}
			}
			return num;
		}

		private static bool IsUnwrappingNullable(ConversionKind conv, TypeSymbol sourceType, TypeSymbol targetType)
		{
			if ((conv & ConversionKind.Nullable) != 0 && (object)sourceType != null && TypeSymbolExtensions.IsNullableType(sourceType))
			{
				return !TypeSymbolExtensions.IsNullableType(targetType);
			}
			return false;
		}

		private static bool IsUnwrappingNullable(TypeSymbol sourceType, TypeSymbol targetType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if ((object)sourceType != null)
			{
				return IsUnwrappingNullable(Conversions.ClassifyPredefinedConversion(sourceType, targetType, ref useSiteInfo), sourceType, targetType);
			}
			return false;
		}

		private static bool HaveNarrowingOnlyFromObjectCandidates(ArrayBuilder<CandidateAnalysisResult> candidates)
		{
			bool result = false;
			int num = candidates.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				CandidateAnalysisResult candidateAnalysisResult = candidates[i];
				if (candidateAnalysisResult.State == CandidateAnalysisResultState.Applicable && !candidateAnalysisResult.RequiresNarrowingNotFromObject && !candidateAnalysisResult.Candidate.IsExtensionMethod)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private static int EliminateNotApplicableToArguments(BoundMethodOrPropertyGroup methodOrPropertyGroup, ArrayBuilder<CandidateAnalysisResult> candidates, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, Binder binder, out int applicableNarrowingCandidates, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, SyntaxNode callerInfoOpt, bool forceExpandedForm, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			int num = 0;
			int num2 = 0;
			applicableNarrowingCandidates = 0;
			int num3 = candidates.Count - 1;
			for (int i = 0; i <= num3; i++)
			{
				CandidateAnalysisResult candidate = candidates[i];
				if (candidate.State != 0)
				{
					continue;
				}
				if (!candidate.ArgumentMatchingDone)
				{
					MatchArguments(methodOrPropertyGroup, ref candidate, arguments, argumentNames, binder, ref asyncLambdaSubToFunctionMismatch, callerInfoOpt, forceExpandedForm, ref useSiteInfo);
					candidate.SetArgumentMatchingDone();
					candidates[i] = candidate;
				}
				if (candidate.State == CandidateAnalysisResultState.Applicable)
				{
					num++;
					if (candidate.RequiresNarrowingConversion)
					{
						applicableNarrowingCandidates++;
					}
					if (candidate.IsIllegalInAttribute)
					{
						num2++;
					}
				}
			}
			if (num2 > 0 && num > num2)
			{
				int num4 = candidates.Count - 1;
				for (int j = 0; j <= num4; j++)
				{
					CandidateAnalysisResult value = candidates[j];
					if (value.State == CandidateAnalysisResultState.Applicable && value.IsIllegalInAttribute)
					{
						num--;
						if (value.RequiresNarrowingConversion)
						{
							applicableNarrowingCandidates--;
						}
						value.State = CandidateAnalysisResultState.ArgumentMismatch;
						candidates[j] = value;
					}
				}
			}
			return num;
		}

		private static void BuildParameterToArgumentMap(ref CandidateAnalysisResult candidate, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, ref ArrayBuilder<int> parameterToArgumentMap, ref ArrayBuilder<int> paramArrayItems)
		{
			parameterToArgumentMap = ArrayBuilder<int>.GetInstance(candidate.Candidate.ParameterCount, -1);
			ArrayBuilder<int> arrayBuilder = null;
			if (!argumentNames.IsDefault)
			{
				arrayBuilder = ArrayBuilder<int>.GetInstance(arguments.Length, -1);
			}
			paramArrayItems = null;
			if (candidate.IsExpandedParamArrayForm)
			{
				paramArrayItems = ArrayBuilder<int>.GetInstance();
			}
			int num = 0;
			int index = 0;
			int num2 = arguments.Length - 1;
			int num3 = 0;
			while (true)
			{
				if (num3 <= num2)
				{
					if (!argumentNames.IsDefault && argumentNames[num3] != null)
					{
						if (!candidate.Candidate.TryGetNamedParamIndex(argumentNames[num3], ref index))
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							break;
						}
						if (index != num3)
						{
							goto IL_0165;
						}
						if (index == candidate.Candidate.ParameterCount - 1 && candidate.Candidate.Parameters(index).IsParamArray)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							break;
						}
					}
					num++;
					if (arrayBuilder != null)
					{
						arrayBuilder[num3] = index;
					}
					if (arguments[num3].Kind == BoundKind.OmittedArgument)
					{
						if (index == candidate.Candidate.ParameterCount - 1 && candidate.Candidate.Parameters(index).IsParamArray)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							break;
						}
						parameterToArgumentMap[index] = num3;
						index++;
					}
					else if (candidate.IsExpandedParamArrayForm && index == candidate.Candidate.ParameterCount - 1)
					{
						paramArrayItems.Add(num3);
					}
					else
					{
						parameterToArgumentMap[index] = num3;
						index++;
					}
					num3++;
					continue;
				}
				goto IL_0165;
				IL_0165:
				int num4 = num;
				int num5 = arguments.Length - 1;
				int num6 = num4;
				while (true)
				{
					if (num6 <= num5)
					{
						if (argumentNames[num6] == null)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							break;
						}
						if (!candidate.Candidate.TryGetNamedParamIndex(argumentNames[num6], ref index))
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							break;
						}
						if (arrayBuilder != null)
						{
							arrayBuilder[num6] = index;
						}
						if (index == candidate.Candidate.ParameterCount - 1 && candidate.Candidate.Parameters(index).IsParamArray)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							break;
						}
						if (parameterToArgumentMap[index] != -1)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							break;
						}
						if (index < num)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							break;
						}
						parameterToArgumentMap[index] = num6;
						num6++;
						continue;
					}
					if (arrayBuilder != null)
					{
						candidate.ArgsToParamsOpt = arrayBuilder.ToImmutableAndFree();
						arrayBuilder = null;
					}
					break;
				}
				break;
			}
			if (arrayBuilder != null)
			{
				arrayBuilder.Free();
				arrayBuilder = null;
			}
		}

		private static void MatchArguments(BoundMethodOrPropertyGroup methodOrPropertyGroup, ref CandidateAnalysisResult candidate, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, Binder binder, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, SyntaxNode callerInfoOpt, bool forceExpandedForm, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ArrayBuilder<int> parameterToArgumentMap = null;
			ArrayBuilder<int> paramArrayItems = null;
			KeyValuePair<ConversionKind, MethodSymbol>[] array = null;
			KeyValuePair<ConversionKind, MethodSymbol>[] array2 = null;
			OptionalArgument[] array3 = null;
			BindingDiagnosticBag bindingDiagnosticBag = null;
			BuildParameterToArgumentMap(ref candidate, arguments, argumentNames, ref parameterToArgumentMap, ref paramArrayItems);
			if (candidate.State == CandidateAnalysisResultState.Applicable)
			{
				if (!candidate.Candidate.IsExtensionMethod)
				{
					candidate.IgnoreExtensionMethods = true;
				}
				Symbol underlyingSymbol = candidate.Candidate.UnderlyingSymbol;
				if (underlyingSymbol.Kind == SymbolKind.Method)
				{
					MethodSymbol methodSymbol = (MethodSymbol)underlyingSymbol;
					if (methodSymbol.IsGenericMethod)
					{
						ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
						ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
						bool flag = ConstraintsHelper.CheckConstraints(methodSymbol, instance, ref useSiteDiagnosticsBuilder, useSiteInfo);
						instance.Free();
						if (useSiteDiagnosticsBuilder != null && useSiteDiagnosticsBuilder.Count > 0)
						{
							ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = useSiteDiagnosticsBuilder.GetEnumerator();
							while (enumerator.MoveNext())
							{
								useSiteInfo.Add(enumerator.Current.UseSiteInfo);
							}
						}
						if (!flag)
						{
							candidate.State = CandidateAnalysisResultState.GenericConstraintsViolated;
							goto IL_05ba;
						}
					}
				}
				bool flag2 = candidate.Candidate.UnderlyingSymbol.Kind == SymbolKind.Property;
				int num = candidate.Candidate.ParameterCount - 1;
				for (int i = 0; i <= num; i++)
				{
					if (candidate.State != 0 && !candidate.IgnoreExtensionMethods)
					{
						break;
					}
					ParameterSymbol parameterSymbol = candidate.Candidate.Parameters(i);
					bool isByRef = parameterSymbol.IsByRef;
					TypeSymbol type = parameterSymbol.Type;
					int num2;
					if (parameterSymbol.IsParamArray && i == candidate.Candidate.ParameterCount - 1)
					{
						if (type.Kind != SymbolKind.ArrayType)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							candidate.IgnoreExtensionMethods = false;
							break;
						}
						if (!candidate.IsExpandedParamArrayForm)
						{
							num2 = parameterToArgumentMap[i];
							BoundExpression boundExpression = ((num2 == -1) ? null : arguments[num2]);
							KeyValuePair<ConversionKind, MethodSymbol> outConvKind = default(KeyValuePair<ConversionKind, MethodSymbol>);
							if (boundExpression == null || boundExpression.HasErrors || !CanPassToParamArray(boundExpression, type, out outConvKind, binder, ref useSiteInfo))
							{
								candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
								candidate.IgnoreExtensionMethods = false;
								break;
							}
							if (Conversions.IsNarrowingConversion(outConvKind.Key) && binder.OptionStrict == OptionStrict.On)
							{
								candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							}
							else
							{
								if (Conversions.IsIdentityConversion(outConvKind.Key))
								{
									continue;
								}
								if (array == null)
								{
									array = new KeyValuePair<ConversionKind, MethodSymbol>[arguments.Length - 1 + 1];
									int num3 = array.Length - 1;
									for (int j = 0; j <= num3; j++)
									{
										array[j] = Conversions.Identity;
									}
								}
								array[num2] = outConvKind;
							}
							continue;
						}
						if (paramArrayItems.Count == 1 && BoundExpressionExtensions.IsNothingLiteral(arguments[paramArrayItems[0]]) && !forceExpandedForm)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							candidate.IgnoreExtensionMethods = false;
							break;
						}
						ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
						if (!arrayTypeSymbol.IsSZArray)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							candidate.IgnoreExtensionMethods = false;
							break;
						}
						type = arrayTypeSymbol.ElementType;
						if (type.Kind == SymbolKind.ErrorType)
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							continue;
						}
						int num4 = paramArrayItems.Count - 1;
						int num5 = 0;
						while (num5 <= num4)
						{
							KeyValuePair<ConversionKind, MethodSymbol> outConversionKind = default(KeyValuePair<ConversionKind, MethodSymbol>);
							if (!arguments[paramArrayItems[num5]].HasErrors)
							{
								if (MatchArgumentToByValParameter(methodOrPropertyGroup, ref candidate, arguments[paramArrayItems[num5]], type, binder, out outConversionKind, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo) && !Conversions.IsIdentityConversion(outConversionKind.Key))
								{
									if (array == null)
									{
										array = new KeyValuePair<ConversionKind, MethodSymbol>[arguments.Length - 1 + 1];
										int num6 = array.Length - 1;
										for (int k = 0; k <= num6; k++)
										{
											array[k] = Conversions.Identity;
										}
									}
									array[paramArrayItems[num5]] = outConversionKind;
								}
								num5++;
								continue;
							}
							goto IL_02f9;
						}
						continue;
					}
					num2 = parameterToArgumentMap[i];
					BoundExpression boundExpression2 = ((num2 == -1) ? null : arguments[num2]);
					BoundExpression boundExpression3 = null;
					if (boundExpression2 == null || boundExpression2.Kind == BoundKind.OmittedArgument)
					{
						if (bindingDiagnosticBag == null)
						{
							bindingDiagnosticBag = BindingDiagnosticBag.GetInstance();
						}
						else
						{
							bindingDiagnosticBag.Clear();
						}
						boundExpression3 = binder.GetArgumentForParameterDefaultValue(parameterSymbol, (boundExpression2 ?? methodOrPropertyGroup).Syntax, bindingDiagnosticBag, callerInfoOpt);
						if (boundExpression3 == null || bindingDiagnosticBag.HasAnyErrors())
						{
							candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
							candidate.IgnoreExtensionMethods = false;
							break;
						}
						boundExpression3.SetWasCompilerGenerated();
						boundExpression2 = boundExpression3;
					}
					if (type.Kind == SymbolKind.ErrorType)
					{
						candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
						continue;
					}
					if (boundExpression2.HasErrors)
					{
						candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
						candidate.IgnoreExtensionMethods = false;
						break;
					}
					KeyValuePair<ConversionKind, MethodSymbol> outConversionKind2 = default(KeyValuePair<ConversionKind, MethodSymbol>);
					KeyValuePair<ConversionKind, MethodSymbol> outConversionBackKind = default(KeyValuePair<ConversionKind, MethodSymbol>);
					if (isByRef && !flag2 && boundExpression3 == null && (parameterSymbol.IsExplicitByRef || ((object)boundExpression2.Type != null && TypeSymbolExtensions.IsStringType(boundExpression2.Type))))
					{
						MatchArgumentToByRefParameter(methodOrPropertyGroup, ref candidate, boundExpression2, type, binder, out outConversionKind2, out outConversionBackKind, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
					}
					else
					{
						outConversionBackKind = Conversions.Identity;
						MatchArgumentToByValParameter(methodOrPropertyGroup, ref candidate, boundExpression2, type, binder, out outConversionKind2, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo, boundExpression3 != null);
					}
					if (!Conversions.IsIdentityConversion(outConversionKind2.Key))
					{
						if (array == null)
						{
							array = new KeyValuePair<ConversionKind, MethodSymbol>[arguments.Length - 1 + 1];
							int num7 = array.Length - 1;
							for (int l = 0; l <= num7; l++)
							{
								array[l] = Conversions.Identity;
							}
						}
						if (boundExpression3 == null)
						{
							array[num2] = outConversionKind2;
						}
					}
					if (boundExpression3 != null)
					{
						if (array3 == null)
						{
							array3 = new OptionalArgument[candidate.Candidate.ParameterCount - 1 + 1];
						}
						array3[i] = new OptionalArgument(boundExpression3, outConversionKind2, bindingDiagnosticBag.DependenciesBag.ToImmutableArray());
					}
					if (Conversions.IsIdentityConversion(outConversionBackKind.Key))
					{
						continue;
					}
					if (array2 == null)
					{
						array2 = new KeyValuePair<ConversionKind, MethodSymbol>[arguments.Length - 1 + 1];
						int num8 = array2.Length - 1;
						for (int m = 0; m <= num8; m++)
						{
							array2[m] = Conversions.Identity;
						}
					}
					array2[num2] = outConversionBackKind;
					continue;
					IL_02f9:
					candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
					candidate.IgnoreExtensionMethods = false;
					break;
				}
			}
			goto IL_05ba;
			IL_05ba:
			bindingDiagnosticBag?.Free();
			paramArrayItems?.Free();
			if (array != null)
			{
				candidate.ConversionsOpt = array.AsImmutableOrNull();
			}
			if (array2 != null)
			{
				candidate.ConversionsBackOpt = array2.AsImmutableOrNull();
			}
			if (array3 != null)
			{
				candidate.OptionalArguments = array3.AsImmutableOrNull();
			}
			parameterToArgumentMap?.Free();
		}

		private static void MatchArgumentToByRefParameter(BoundMethodOrPropertyGroup methodOrPropertyGroup, ref CandidateAnalysisResult candidate, BoundExpression argument, TypeSymbol targetType, Binder binder, out KeyValuePair<ConversionKind, MethodSymbol> outConversionKind, out KeyValuePair<ConversionKind, MethodSymbol> outConversionBackKind, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (BoundExpressionExtensions.IsSupportingAssignment(argument))
			{
				if (argument.IsLValue && TypeSymbolExtensions.IsSameTypeIgnoringAll(targetType, argument.Type))
				{
					outConversionKind = Conversions.Identity;
					outConversionBackKind = Conversions.Identity;
					return;
				}
				outConversionBackKind = Conversions.Identity;
				if (!MatchArgumentToByValParameter(methodOrPropertyGroup, ref candidate, argument, targetType, binder, out outConversionKind, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo))
				{
					return;
				}
				TypeSymbol typeOfAssignmentTarget = BoundExpressionExtensions.GetTypeOfAssignmentTarget(argument);
				KeyValuePair<ConversionKind, MethodSymbol> keyValuePair = (outConversionBackKind = Conversions.ClassifyConversion(targetType, typeOfAssignmentTarget, ref useSiteInfo));
				if (Conversions.NoConversion(keyValuePair.Key))
				{
					candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
					candidate.IgnoreExtensionMethods = false;
					return;
				}
				if (Conversions.IsNarrowingConversion(keyValuePair.Key))
				{
					candidate.SetRequiresNarrowingConversion();
					candidate.SetRequiresNarrowingNotFromNumericConstant();
					if (binder.OptionStrict == OptionStrict.On)
					{
						candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
						return;
					}
					if (targetType.SpecialType != SpecialType.System_Object)
					{
						candidate.SetRequiresNarrowingNotFromObject();
					}
				}
				candidate.RegisterDelegateRelaxationLevel(keyValuePair.Key);
			}
			else
			{
				if (binder.Report_ERRID_ReadOnlyInClosure(argument))
				{
					candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
				}
				outConversionBackKind = Conversions.Identity;
				MatchArgumentToByValParameter(methodOrPropertyGroup, ref candidate, argument, targetType, binder, out outConversionKind, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
			}
		}

		private static bool MatchArgumentToByValParameter(BoundMethodOrPropertyGroup methodOrPropertyGroup, ref CandidateAnalysisResult candidate, BoundExpression argument, TypeSymbol targetType, Binder binder, out KeyValuePair<ConversionKind, MethodSymbol> outConversionKind, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, bool isDefaultValueArgument = false)
		{
			outConversionKind = default(KeyValuePair<ConversionKind, MethodSymbol>);
			if (TypeSymbolExtensions.IsErrorType(targetType))
			{
				candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
				return false;
			}
			KeyValuePair<ConversionKind, MethodSymbol> conversion = (outConversionKind = Conversions.ClassifyConversion(argument, targetType, binder, ref useSiteInfo));
			if (Conversions.NoConversion(conversion.Key))
			{
				candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
				candidate.IgnoreExtensionMethods = false;
				if ((conversion.Key & (ConversionKind.DelegateRelaxationLevelMask | ConversionKind.Lambda)) == (ConversionKind.DelegateRelaxationLevelInvalid | ConversionKind.Lambda))
				{
					BoundExpression boundExpression = argument;
					while (boundExpression.Kind == BoundKind.Parenthesized && (object)boundExpression.Type == null)
					{
						boundExpression = ((BoundParenthesized)boundExpression).Expression;
					}
					UnboundLambda unboundLambda = ((boundExpression.Kind == BoundKind.UnboundLambda) ? ((UnboundLambda)boundExpression) : null);
					if (unboundLambda != null && !unboundLambda.IsFunctionLambda && (unboundLambda.Flags & SourceMemberFlags.Async) != 0 && TypeSymbolExtensions.IsDelegateType(targetType))
					{
						MethodSymbol delegateInvokeMethod = ((NamedTypeSymbol)targetType).DelegateInvokeMethod;
						if ((object)delegateInvokeMethod != null)
						{
							BoundLambda boundLambda = unboundLambda.GetBoundLambda(new UnboundLambda.TargetSignature(delegateInvokeMethod));
							if (boundLambda != null && (boundLambda.MethodConversionKind & MethodConversionKind.AllErrorReasons) == MethodConversionKind.Error_SubToFunction && !boundLambda.Diagnostics.Diagnostics.HasAnyErrors())
							{
								if (asyncLambdaSubToFunctionMismatch == null)
								{
									asyncLambdaSubToFunctionMismatch = new HashSet<BoundExpression>(ReferenceEqualityComparer.Instance);
								}
								asyncLambdaSubToFunctionMismatch.Add(unboundLambda);
							}
						}
					}
				}
				return false;
			}
			if (Conversions.IsNarrowingConversion(conversion.Key))
			{
				if (!isDefaultValueArgument)
				{
					candidate.SetRequiresNarrowingConversion();
				}
				if ((conversion.Key & ConversionKind.InvolvesNarrowingFromNumericConstant) == 0)
				{
					if (!isDefaultValueArgument)
					{
						candidate.SetRequiresNarrowingNotFromNumericConstant();
					}
					if (binder.OptionStrict == OptionStrict.On)
					{
						candidate.State = CandidateAnalysisResultState.ArgumentMismatch;
						return false;
					}
				}
				TypeSymbol type = argument.Type;
				if (((object)type == null || type.SpecialType != SpecialType.System_Object) && !isDefaultValueArgument)
				{
					candidate.SetRequiresNarrowingNotFromObject();
				}
			}
			else if ((conversion.Key & ConversionKind.InvolvesNarrowingFromNumericConstant) != 0 && !isDefaultValueArgument)
			{
				candidate.SetRequiresNarrowingConversion();
				candidate.SetRequiresNarrowingNotFromObject();
			}
			if (!isDefaultValueArgument)
			{
				candidate.RegisterDelegateRelaxationLevel(conversion.Key);
			}
			if (binder.BindingLocation == BindingLocation.Attribute && !candidate.IsIllegalInAttribute && !methodOrPropertyGroup.WasCompilerGenerated && methodOrPropertyGroup.Kind == BoundKind.MethodGroup && IsWithinAppliedAttributeName(methodOrPropertyGroup.Syntax) && ((MethodSymbol)candidate.Candidate.UnderlyingSymbol).MethodKind == MethodKind.Constructor && TypeSymbolExtensions.IsBaseTypeOf(binder.Compilation.GetWellKnownType(WellKnownType.System_Attribute), candidate.Candidate.UnderlyingSymbol.ContainingType, ref useSiteInfo))
			{
				BoundExpression boundExpression2 = binder.PassArgumentByVal(argument, conversion, targetType, BindingDiagnosticBag.Discarded);
				if (!boundExpression2.IsConstant)
				{
					Binder.AttributeExpressionVisitor attributeExpressionVisitor = new Binder.AttributeExpressionVisitor(binder, boundExpression2.HasErrors);
					attributeExpressionVisitor.VisitExpression(boundExpression2, BindingDiagnosticBag.Discarded);
					if (attributeExpressionVisitor.HasErrors)
					{
						candidate.SetIllegalInAttribute();
					}
				}
			}
			return true;
		}

		private static bool IsWithinAppliedAttributeName(SyntaxNode syntax)
		{
			for (SyntaxNode parent = syntax.Parent; parent != null; parent = parent.Parent)
			{
				if (VisualBasicExtensions.Kind(parent) == SyntaxKind.Attribute)
				{
					return ((AttributeSyntax)parent).Name.Span.Contains(syntax.Position);
				}
				if (parent is ExpressionSyntax || parent is StatementSyntax)
				{
					break;
				}
			}
			return false;
		}

		public static bool CanPassToParamArray(BoundExpression expression, TypeSymbol targetType, out KeyValuePair<ConversionKind, MethodSymbol> outConvKind, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			outConvKind = Conversions.ClassifyConversion(expression, targetType, binder, ref useSiteInfo);
			if (Conversions.IsWideningConversion(outConvKind.Key))
			{
				return true;
			}
			if (BoundExpressionExtensions.IsNothingLiteral(expression))
			{
				return true;
			}
			return false;
		}

		private static void CollectOverloadedCandidates(Binder binder, ArrayBuilder<CandidateAnalysisResult> results, ArrayBuilder<Candidate> group, ImmutableArray<TypeSymbol> typeArguments, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, TypeSymbol delegateReturnType, BoundNode delegateReturnTypeReferenceBoundNode, bool includeEliminatedCandidates, bool isQueryOperatorInvocation, bool forceExpandedForm, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ArrayBuilder<QuickApplicabilityInfo> instance = ArrayBuilder<QuickApplicabilityInfo>.GetInstance();
			ModuleSymbol sourceModule = binder.SourceModule;
			int num = group.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (group[i] == null)
				{
					continue;
				}
				QuickApplicabilityInfo quickApplicabilityInfo = DoQuickApplicabilityCheck(group[i], typeArguments, arguments, isQueryOperatorInvocation, forceExpandedForm, ref useSiteInfo);
				if (quickApplicabilityInfo.Candidate == null)
				{
					continue;
				}
				if ((object)quickApplicabilityInfo.Candidate.UnderlyingSymbol.ContainingModule == sourceModule || quickApplicabilityInfo.Candidate.IsExtensionMethod)
				{
					CollectOverloadedCandidate(results, quickApplicabilityInfo, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, binder, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
					continue;
				}
				Symbol containingSymbol = quickApplicabilityInfo.Candidate.UnderlyingSymbol.ContainingSymbol;
				instance.Clear();
				instance.Add(quickApplicabilityInfo);
				int num2 = ((quickApplicabilityInfo.State == CandidateAnalysisResultState.Applicable) ? 1 : 0);
				int num3 = i + 1;
				int num4 = group.Count - 1;
				for (int j = num3; j <= num4; j++)
				{
					if (group[j] == null || group[j].IsExtensionMethod || !(containingSymbol == group[j].UnderlyingSymbol.ContainingSymbol))
					{
						continue;
					}
					quickApplicabilityInfo = DoQuickApplicabilityCheck(group[j], typeArguments, arguments, isQueryOperatorInvocation, forceExpandedForm, ref useSiteInfo);
					group[j] = null;
					if (quickApplicabilityInfo.Candidate != null)
					{
						if (quickApplicabilityInfo.State != 0)
						{
							instance.Add(quickApplicabilityInfo);
						}
						else if (num2 == instance.Count)
						{
							instance.Add(quickApplicabilityInfo);
							num2++;
						}
						else
						{
							instance.Add(instance[num2]);
							instance[num2] = quickApplicabilityInfo;
							num2++;
						}
					}
				}
				int num5 = ((num2 > 0 || !includeEliminatedCandidates) ? num2 : instance.Count) - 1;
				for (int k = 0; k <= num5; k++)
				{
					quickApplicabilityInfo = instance[k];
					if (quickApplicabilityInfo.Candidate == null || quickApplicabilityInfo.State == CandidateAnalysisResultState.Ambiguous)
					{
						continue;
					}
					Symbol symbol = quickApplicabilityInfo.Candidate.UnderlyingSymbol.OriginalDefinition;
					if (SymbolExtensions.IsReducedExtensionMethod(symbol))
					{
						symbol = ((MethodSymbol)symbol).ReducedFrom;
					}
					int num6 = k + 1;
					int num7 = instance.Count - 1;
					for (int l = num6; l <= num7; l++)
					{
						QuickApplicabilityInfo quickApplicabilityInfo2 = instance[l];
						if (quickApplicabilityInfo2.Candidate == null || quickApplicabilityInfo2.State == CandidateAnalysisResultState.Ambiguous)
						{
							continue;
						}
						Symbol symbol2 = quickApplicabilityInfo2.Candidate.UnderlyingSymbol.OriginalDefinition;
						if (SymbolExtensions.IsReducedExtensionMethod(symbol2))
						{
							symbol2 = ((MethodSymbol)symbol2).ReducedFrom;
						}
						if (OverrideHidingHelper.DetailedSignatureCompare(symbol, symbol2, (SymbolComparisonResults)115525, (SymbolComparisonResults)115525) == (SymbolComparisonResults)0)
						{
							int num8 = LookupResult.CompareAccessibilityOfSymbolsConflictingInSameContainer(symbol, symbol2);
							if (num8 > 0)
							{
								instance[l] = default(QuickApplicabilityInfo);
							}
							else if (num8 < 0)
							{
								instance[k] = quickApplicabilityInfo2;
								instance[l] = default(QuickApplicabilityInfo);
								symbol = symbol2;
								quickApplicabilityInfo = quickApplicabilityInfo2;
							}
							else
							{
								quickApplicabilityInfo = (instance[k] = new QuickApplicabilityInfo(quickApplicabilityInfo.Candidate, CandidateAnalysisResultState.Ambiguous));
								instance[l] = new QuickApplicabilityInfo(quickApplicabilityInfo2.Candidate, CandidateAnalysisResultState.Ambiguous);
							}
						}
					}
					if (quickApplicabilityInfo.State != CandidateAnalysisResultState.Ambiguous)
					{
						CollectOverloadedCandidate(results, quickApplicabilityInfo, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, binder, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
					}
					else
					{
						if (!includeEliminatedCandidates)
						{
							continue;
						}
						CollectOverloadedCandidate(results, quickApplicabilityInfo, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, binder, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
						int num9 = k + 1;
						int num10 = instance.Count - 1;
						for (int m = num9; m <= num10; m++)
						{
							QuickApplicabilityInfo candidate = instance[m];
							if (candidate.Candidate != null && candidate.State == CandidateAnalysisResultState.Ambiguous)
							{
								instance[m] = default(QuickApplicabilityInfo);
								CollectOverloadedCandidate(results, candidate, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, includeEliminatedCandidates, binder, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
							}
						}
					}
				}
			}
			instance.Free();
		}

		private static QuickApplicabilityInfo DoQuickApplicabilityCheck(Candidate candidate, ImmutableArray<TypeSymbol> typeArguments, ImmutableArray<BoundExpression> arguments, bool isQueryOperatorInvocation, bool forceExpandedForm, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			QuickApplicabilityInfo result;
			int maxCount = default(int);
			bool hasParamArray = default(bool);
			if (isQueryOperatorInvocation && ((MethodSymbol)candidate.UnderlyingSymbol).IsSub)
			{
				result = default(QuickApplicabilityInfo);
			}
			else if (candidate.UnderlyingSymbol.HasUnsupportedMetadata)
			{
				result = new QuickApplicabilityInfo(candidate, CandidateAnalysisResultState.HasUnsupportedMetadata);
			}
			else if (typeArguments.Length > 0 && candidate.Arity != typeArguments.Length)
			{
				result = new QuickApplicabilityInfo(candidate, CandidateAnalysisResultState.BadGenericArity);
			}
			else
			{
				int requiredCount = default(int);
				candidate.GetAllParameterCounts(ref requiredCount, ref maxCount, ref hasParamArray);
				if (isQueryOperatorInvocation)
				{
					if (arguments.Length == maxCount)
					{
						goto IL_00b8;
					}
					result = new QuickApplicabilityInfo(candidate, CandidateAnalysisResultState.ArgumentCountMismatch, appliesToNormalForm: true, appliesToParamArrayForm: false);
				}
				else
				{
					if (arguments.Length >= requiredCount && (hasParamArray || arguments.Length <= maxCount))
					{
						goto IL_00b8;
					}
					result = new QuickApplicabilityInfo(candidate, CandidateAnalysisResultState.ArgumentCountMismatch, !hasParamArray, hasParamArray);
				}
			}
			goto IL_0114;
			IL_00b8:
			UseSiteInfo<AssemblySymbol> useSiteInfo2 = candidate.UnderlyingSymbol.GetUseSiteInfo();
			useSiteInfo.Add(useSiteInfo2);
			if (useSiteInfo2.DiagnosticInfo != null)
			{
				result = new QuickApplicabilityInfo(candidate, CandidateAnalysisResultState.HasUseSiteError);
			}
			else
			{
				bool appliesToNormalForm = false;
				bool appliesToParamArrayForm = false;
				if (!hasParamArray || (arguments.Length == maxCount && !forceExpandedForm))
				{
					appliesToNormalForm = true;
				}
				if (hasParamArray && !isQueryOperatorInvocation)
				{
					appliesToParamArrayForm = true;
				}
				result = new QuickApplicabilityInfo(candidate, CandidateAnalysisResultState.Applicable, appliesToNormalForm, appliesToParamArrayForm);
			}
			goto IL_0114;
			IL_0114:
			return result;
		}

		private static void CollectOverloadedCandidate(ArrayBuilder<CandidateAnalysisResult> results, QuickApplicabilityInfo candidate, ImmutableArray<TypeSymbol> typeArguments, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, TypeSymbol delegateReturnType, BoundNode delegateReturnTypeReferenceBoundNode, bool includeEliminatedCandidates, Binder binder, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			switch (candidate.State)
			{
			case CandidateAnalysisResultState.HasUnsupportedMetadata:
				if (includeEliminatedCandidates)
				{
					results.Add(new CandidateAnalysisResult(candidate.Candidate, CandidateAnalysisResultState.HasUnsupportedMetadata));
				}
				break;
			case CandidateAnalysisResultState.HasUseSiteError:
				if (includeEliminatedCandidates)
				{
					results.Add(new CandidateAnalysisResult(candidate.Candidate, CandidateAnalysisResultState.HasUseSiteError));
				}
				break;
			case CandidateAnalysisResultState.BadGenericArity:
				if (includeEliminatedCandidates)
				{
					results.Add(new CandidateAnalysisResult(candidate.Candidate, CandidateAnalysisResultState.BadGenericArity));
				}
				break;
			case CandidateAnalysisResultState.ArgumentCountMismatch:
				if (includeEliminatedCandidates)
				{
					CandidateAnalysisResult item = new CandidateAnalysisResult(ConstructIfNeedTo(candidate.Candidate, typeArguments), CandidateAnalysisResultState.ArgumentCountMismatch);
					if (candidate.AppliesToParamArrayForm)
					{
						item.SetIsExpandedParamArrayForm();
					}
					results.Add(item);
				}
				break;
			case CandidateAnalysisResultState.Applicable:
			{
				CandidateAnalysisResult newCandidate = ((typeArguments.Length <= 0) ? new CandidateAnalysisResult(candidate.Candidate) : new CandidateAnalysisResult(candidate.Candidate.Construct(typeArguments)));
				if (candidate.AppliesToNormalForm)
				{
					InferTypeArgumentsIfNeedToAndCombineWithExistingCandidates(results, newCandidate, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, binder, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
				}
				if (candidate.AppliesToParamArrayForm)
				{
					newCandidate.SetIsExpandedParamArrayForm();
					newCandidate.ExpandedParamArrayArgumentsUsed = Math.Max(arguments.Length - candidate.Candidate.ParameterCount + 1, 0);
					InferTypeArgumentsIfNeedToAndCombineWithExistingCandidates(results, newCandidate, typeArguments, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, binder, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo);
				}
				break;
			}
			case CandidateAnalysisResultState.Ambiguous:
				if (includeEliminatedCandidates)
				{
					results.Add(new CandidateAnalysisResult(candidate.Candidate, CandidateAnalysisResultState.Ambiguous));
				}
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(candidate.State);
			}
		}

		private static void InferTypeArgumentsIfNeedToAndCombineWithExistingCandidates(ArrayBuilder<CandidateAnalysisResult> results, CandidateAnalysisResult newCandidate, ImmutableArray<TypeSymbol> typeArguments, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, TypeSymbol delegateReturnType, BoundNode delegateReturnTypeReferenceBoundNode, Binder binder, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (typeArguments.Length == 0 && newCandidate.Candidate.Arity > 0 && !InferTypeArguments(ref newCandidate, arguments, argumentNames, delegateReturnType, delegateReturnTypeReferenceBoundNode, ref asyncLambdaSubToFunctionMismatch, binder, ref useSiteInfo))
			{
				results.Add(newCandidate);
			}
			else
			{
				CombineCandidates(results, newCandidate, arguments.Length, argumentNames, ref useSiteInfo);
			}
		}

		private static void CombineCandidates(ArrayBuilder<CandidateAnalysisResult> results, CandidateAnalysisResult newCandidate, int argumentCount, ImmutableArray<string> argumentNames, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			bool isOperator = newCandidate.Candidate.IsOperator;
			int num = 0;
			while (true)
			{
				CandidateAnalysisResult candidate;
				bool leftWins;
				bool rightWins;
				bool flag;
				if (num < results.Count)
				{
					candidate = results[num];
					if (candidate.State != CandidateAnalysisResultState.ArgumentCountMismatch && candidate.State != CandidateAnalysisResultState.BadGenericArity && candidate.State != CandidateAnalysisResultState.Ambiguous && candidate.Candidate != newCandidate.Candidate)
					{
						leftWins = false;
						rightWins = false;
						if (!isOperator && ShadowBasedOnOverriding(candidate, newCandidate, ref leftWins, ref rightWins))
						{
							goto IL_0201;
						}
						if (candidate.State != CandidateAnalysisResultState.TypeInferenceFailed && !candidate.SomeInferenceFailed && candidate.State != CandidateAnalysisResultState.HasUseSiteError && candidate.State != CandidateAnalysisResultState.HasUnsupportedMetadata)
						{
							if (argumentNames.IsDefault)
							{
								int paramIndex = 0;
								int paramIndex2 = 0;
								int num2 = argumentCount - 1;
								int num3 = 0;
								while (num3 <= num2)
								{
									TypeSymbol parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref candidate, paramIndex);
									TypeSymbol parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref newCandidate, paramIndex2);
									if (TypeSymbolExtensions.IsSameTypeIgnoringAll(parameterTypeFromVirtualSignature, parameterTypeFromVirtualSignature2))
									{
										AdvanceParameterInVirtualSignature(ref candidate, ref paramIndex);
										AdvanceParameterInVirtualSignature(ref newCandidate, ref paramIndex2);
										num3++;
										continue;
									}
									goto IL_0211;
								}
							}
							flag = true;
							if (candidate.Candidate.ParameterCount != newCandidate.Candidate.ParameterCount)
							{
								flag = false;
							}
							else
							{
								if (isOperator)
								{
									if (candidate.Candidate.IsLifted)
									{
										if (newCandidate.Candidate.IsLifted)
										{
											goto IL_01a5;
										}
										rightWins = true;
									}
									else
									{
										if (!newCandidate.Candidate.IsLifted)
										{
											goto IL_01a5;
										}
										leftWins = true;
									}
									goto IL_0201;
								}
								int num4 = candidate.Candidate.ParameterCount - 1;
								for (int i = 0; i <= num4; i++)
								{
									TypeSymbol type = candidate.Candidate.Parameters(i).Type;
									TypeSymbol type2 = newCandidate.Candidate.Parameters(i).Type;
									if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(type, type2))
									{
										flag = false;
										break;
									}
								}
							}
							goto IL_01a5;
						}
					}
					goto IL_0211;
				}
				results.Add(newCandidate);
				break;
				IL_0201:
				if (rightWins)
				{
					results.RemoveAt(num);
					continue;
				}
				if (leftWins)
				{
					break;
				}
				goto IL_0211;
				IL_01a5:
				if (argumentNames.IsDefault || flag)
				{
					if ((flag || !ShadowBasedOnParamArrayUsage(candidate, newCandidate, ref leftWins, ref rightWins)) && (argumentNames.IsDefault || (!candidate.Candidate.IsExtensionMethod && !newCandidate.Candidate.IsExtensionMethod)) && !ShadowBasedOnReceiverType(candidate, newCandidate, ref leftWins, ref rightWins, ref useSiteInfo))
					{
						ShadowBasedOnExtensionMethodTargetTypeGenericity(candidate, newCandidate, ref leftWins, ref rightWins);
					}
					goto IL_0201;
				}
				goto IL_0211;
				IL_0211:
				num++;
			}
		}

		private static bool ShadowBasedOnOverriding(CandidateAnalysisResult existingCandidate, CandidateAnalysisResult newCandidate, ref bool existingWins, ref bool newWins)
		{
			Symbol underlyingSymbol = existingCandidate.Candidate.UnderlyingSymbol;
			Symbol underlyingSymbol2 = newCandidate.Candidate.UnderlyingSymbol;
			NamedTypeSymbol containingType = underlyingSymbol.ContainingType;
			NamedTypeSymbol containingType2 = underlyingSymbol2.ContainingType;
			bool flag = existingCandidate.State == CandidateAnalysisResultState.Applicable;
			if (flag && !TypeSymbolExtensions.IsRestrictedType(containingType) && !TypeSymbolExtensions.IsRestrictedType(containingType2))
			{
				return false;
			}
			if ((object)containingType.OriginalDefinition != containingType2.OriginalDefinition)
			{
				if (newCandidate.Candidate.IsOverriddenBy(underlyingSymbol))
				{
					existingWins = true;
					return true;
				}
				if (flag && existingCandidate.Candidate.IsOverriddenBy(underlyingSymbol2))
				{
					newWins = true;
					return true;
				}
			}
			return false;
		}

		private static bool ShadowBasedOnExtensionVsInstanceAndPrecedence(CandidateAnalysisResult left, CandidateAnalysisResult right, ref bool leftWins, ref bool rightWins)
		{
			if (left.Candidate.IsExtensionMethod)
			{
				if (!right.Candidate.IsExtensionMethod)
				{
					rightWins = true;
					return true;
				}
				if (left.Candidate.PrecedenceLevel < right.Candidate.PrecedenceLevel)
				{
					leftWins = true;
					return true;
				}
				if (left.Candidate.PrecedenceLevel > right.Candidate.PrecedenceLevel)
				{
					rightWins = true;
					return true;
				}
			}
			else if (right.Candidate.IsExtensionMethod)
			{
				leftWins = true;
				return true;
			}
			return false;
		}

		private static bool ShadowBasedOnGenericity(CandidateAnalysisResult left, CandidateAnalysisResult right, ref bool leftWins, ref bool rightWins, ImmutableArray<BoundExpression> arguments, Binder binder)
		{
			TypeParameterKind typeParameterKind = TypeParameterKind.Both;
			if (!left.Candidate.IsGeneric && !right.Candidate.IsGeneric)
			{
				typeParameterKind &= ~TypeParameterKind.Method;
			}
			if (!NamedTypeSymbolExtensions.IsOrInGenericType(left.Candidate.UnderlyingSymbol.ContainingType) && (!left.Candidate.IsExtensionMethod || left.Candidate.FixedTypeParameters.IsNull) && !NamedTypeSymbolExtensions.IsOrInGenericType(right.Candidate.UnderlyingSymbol.ContainingType) && (!right.Candidate.IsExtensionMethod || right.Candidate.FixedTypeParameters.IsNull))
			{
				typeParameterKind &= ~TypeParameterKind.Type;
			}
			if (typeParameterKind == TypeParameterKind.None)
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			int paramIndex = 0;
			int paramIndex2 = 0;
			int num = arguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSymbol typeForGenericityCheck = null;
				TypeSymbol parameterTypeFromVirtualSignature;
				if (left.ArgsToParamsOpt.IsDefault)
				{
					parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref left, paramIndex, ref typeForGenericityCheck);
					AdvanceParameterInVirtualSignature(ref left, ref paramIndex);
				}
				else
				{
					parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref left, left.ArgsToParamsOpt[i], ref typeForGenericityCheck);
				}
				TypeSymbol typeForGenericityCheck2 = null;
				TypeSymbol parameterTypeFromVirtualSignature2;
				if (right.ArgsToParamsOpt.IsDefault)
				{
					parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref right, paramIndex2, ref typeForGenericityCheck2);
					AdvanceParameterInVirtualSignature(ref right, ref paramIndex2);
				}
				else
				{
					parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref right, right.ArgsToParamsOpt[i], ref typeForGenericityCheck2);
				}
				if (arguments[i].Kind == BoundKind.OmittedArgument)
				{
					continue;
				}
				if (SignatureMismatchForThePurposeOfShadowingBasedOnGenericity(parameterTypeFromVirtualSignature, parameterTypeFromVirtualSignature2, arguments[i], binder))
				{
					return false;
				}
				TypeParameterKind typeParameterKind2 = DetectReferencesToGenericParameters(typeForGenericityCheck, typeParameterKind, left.Candidate.FixedTypeParameters);
				TypeParameterKind typeParameterKind3 = DetectReferencesToGenericParameters(typeForGenericityCheck2, typeParameterKind, right.Candidate.FixedTypeParameters);
				if ((typeParameterKind & TypeParameterKind.Method) != 0)
				{
					if ((typeParameterKind2 & TypeParameterKind.Method) == 0)
					{
						if ((typeParameterKind3 & TypeParameterKind.Method) != 0)
						{
							flag = true;
						}
					}
					else if ((typeParameterKind3 & TypeParameterKind.Method) == 0)
					{
						flag3 = true;
					}
					if (flag && flag3)
					{
						typeParameterKind &= ~TypeParameterKind.Method;
					}
				}
				if ((typeParameterKind & TypeParameterKind.Type) != 0)
				{
					if ((typeParameterKind2 & TypeParameterKind.Type) == 0)
					{
						if ((typeParameterKind3 & TypeParameterKind.Type) != 0)
						{
							flag2 = true;
						}
					}
					else if ((typeParameterKind3 & TypeParameterKind.Type) == 0)
					{
						flag4 = true;
					}
					if (flag2 && flag4)
					{
						typeParameterKind &= ~TypeParameterKind.Type;
					}
				}
				if (typeParameterKind == TypeParameterKind.None)
				{
					return false;
				}
			}
			if (flag)
			{
				if (!flag3)
				{
					leftWins = true;
					return true;
				}
			}
			else if (flag3)
			{
				rightWins = true;
				return true;
			}
			if (flag2)
			{
				if (!flag4)
				{
					leftWins = true;
					return true;
				}
			}
			else if (flag4)
			{
				rightWins = true;
				return true;
			}
			return false;
		}

		private static bool SignatureMismatchForThePurposeOfShadowingBasedOnGenericity(TypeSymbol leftParamType, TypeSymbol rightParamType, BoundExpression argument, Binder binder)
		{
			if (TypeSymbolExtensions.IsSameTypeIgnoringAll(leftParamType, rightParamType))
			{
				return false;
			}
			bool wasExpression = default(bool);
			NamedTypeSymbol namedTypeSymbol = TypeSymbolExtensions.DelegateOrExpressionDelegate(leftParamType, binder, ref wasExpression);
			bool wasExpression2 = default(bool);
			NamedTypeSymbol namedTypeSymbol2 = TypeSymbolExtensions.DelegateOrExpressionDelegate(rightParamType, binder, ref wasExpression2);
			if ((object)namedTypeSymbol != null && (object)namedTypeSymbol2 != null && (wasExpression == wasExpression2 || BoundNodeExtensions.IsAnyLambda(argument)))
			{
				MethodSymbol delegateInvokeMethod = namedTypeSymbol.DelegateInvokeMethod;
				MethodSymbol delegateInvokeMethod2 = namedTypeSymbol2.DelegateInvokeMethod;
				if ((object)delegateInvokeMethod != null && (object)delegateInvokeMethod2 != null && MethodSignatureComparer.ParametersAndReturnTypeSignatureComparer.Equals(delegateInvokeMethod, delegateInvokeMethod2))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ShadowBasedOnDepthOfGenericity(CandidateAnalysisResult left, CandidateAnalysisResult right, ref bool leftWins, ref bool rightWins, ImmutableArray<BoundExpression> arguments, Binder binder)
		{
			int paramIndex = 0;
			int paramIndex2 = 0;
			int num = arguments.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeSymbol typeForGenericityCheck = null;
				TypeSymbol parameterTypeFromVirtualSignature;
				if (left.ArgsToParamsOpt.IsDefault)
				{
					parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref left, paramIndex, ref typeForGenericityCheck);
					AdvanceParameterInVirtualSignature(ref left, ref paramIndex);
				}
				else
				{
					parameterTypeFromVirtualSignature = GetParameterTypeFromVirtualSignature(ref left, left.ArgsToParamsOpt[i], ref typeForGenericityCheck);
				}
				TypeSymbol typeForGenericityCheck2 = null;
				TypeSymbol parameterTypeFromVirtualSignature2;
				if (right.ArgsToParamsOpt.IsDefault)
				{
					parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref right, paramIndex2, ref typeForGenericityCheck2);
					AdvanceParameterInVirtualSignature(ref right, ref paramIndex2);
				}
				else
				{
					parameterTypeFromVirtualSignature2 = GetParameterTypeFromVirtualSignature(ref right, right.ArgsToParamsOpt[i], ref typeForGenericityCheck2);
				}
				if (arguments[i].Kind == BoundKind.OmittedArgument)
				{
					continue;
				}
				if (SignatureMismatchForThePurposeOfShadowingBasedOnGenericity(parameterTypeFromVirtualSignature, parameterTypeFromVirtualSignature2, arguments[i], binder))
				{
					return false;
				}
				bool leftWins2 = false;
				bool rightWins2 = false;
				if (!CompareParameterTypeGenericDepth(typeForGenericityCheck, typeForGenericityCheck2, ref leftWins2, ref rightWins2))
				{
					continue;
				}
				if (leftWins2)
				{
					if (rightWins)
					{
						rightWins = false;
						return false;
					}
					leftWins = true;
				}
				else
				{
					if (leftWins)
					{
						leftWins = false;
						return false;
					}
					rightWins = true;
				}
			}
			return leftWins || rightWins;
		}

		private static bool CompareParameterTypeGenericDepth(TypeSymbol leftType, TypeSymbol rightType, ref bool leftWins, ref bool rightWins)
		{
			if ((object)leftType == rightType)
			{
				return false;
			}
			if (TypeSymbolExtensions.IsTypeParameter(leftType))
			{
				if (TypeSymbolExtensions.IsTypeParameter(rightType))
				{
					return false;
				}
				rightWins = true;
				return true;
			}
			if (TypeSymbolExtensions.IsTypeParameter(rightType))
			{
				leftWins = true;
				return true;
			}
			if (TypeSymbolExtensions.IsArrayType(leftType) && TypeSymbolExtensions.IsArrayType(rightType))
			{
				ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)leftType;
				ArrayTypeSymbol arrayTypeSymbol2 = (ArrayTypeSymbol)rightType;
				if (arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol2))
				{
					return CompareParameterTypeGenericDepth(arrayTypeSymbol.ElementType, arrayTypeSymbol2.ElementType, ref leftWins, ref rightWins);
				}
			}
			if (leftType.Kind == SymbolKind.NamedType && rightType.Kind == SymbolKind.NamedType)
			{
				NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(leftType);
				NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)TypeSymbolExtensions.GetTupleUnderlyingTypeOrSelf(rightType);
				if (namedTypeSymbol.Arity == namedTypeSymbol2.Arity)
				{
					ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics;
					ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics2 = namedTypeSymbol2.TypeArgumentsNoUseSiteDiagnostics;
					int num = typeArgumentsNoUseSiteDiagnostics.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						bool leftWins2 = false;
						bool rightWins2 = false;
						if (!CompareParameterTypeGenericDepth(typeArgumentsNoUseSiteDiagnostics[i], typeArgumentsNoUseSiteDiagnostics2[i], ref leftWins2, ref rightWins2))
						{
							continue;
						}
						if (leftWins2)
						{
							if (rightWins)
							{
								rightWins = false;
								return false;
							}
							leftWins = true;
						}
						else
						{
							if (leftWins)
							{
								leftWins = false;
								return false;
							}
							rightWins = true;
						}
					}
					return leftWins || rightWins;
				}
			}
			return false;
		}

		private static bool ShadowBasedOnExtensionMethodTargetTypeGenericity(CandidateAnalysisResult left, CandidateAnalysisResult right, ref bool leftWins, ref bool rightWins)
		{
			if (!left.Candidate.IsExtensionMethod || !right.Candidate.IsExtensionMethod)
			{
				return false;
			}
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(left.Candidate.ReceiverType, right.Candidate.ReceiverType))
			{
				return false;
			}
			TypeParameterKind num = DetectReferencesToGenericParameters(left.Candidate.ReceiverTypeDefinition, TypeParameterKind.Method, BitVector.Null);
			TypeParameterKind typeParameterKind = DetectReferencesToGenericParameters(right.Candidate.ReceiverTypeDefinition, TypeParameterKind.Method, BitVector.Null);
			if ((num & TypeParameterKind.Method) != 0)
			{
				if ((typeParameterKind & TypeParameterKind.Method) == 0)
				{
					rightWins = true;
					return true;
				}
			}
			else if ((typeParameterKind & TypeParameterKind.Method) != 0)
			{
				leftWins = true;
				return true;
			}
			return false;
		}

		private static TypeParameterKind DetectReferencesToGenericParameters(NamedTypeSymbol symbol, TypeParameterKind track, BitVector methodTypeParametersToTreatAsTypeTypeParameters)
		{
			TypeParameterKind typeParameterKind = TypeParameterKind.None;
			do
			{
				if ((object)symbol == symbol.OriginalDefinition)
				{
					if ((track & TypeParameterKind.Type) == 0)
					{
						return typeParameterKind;
					}
					if (symbol.Arity > 0)
					{
						return typeParameterKind | TypeParameterKind.Type;
					}
				}
				else
				{
					ImmutableArray<TypeSymbol>.Enumerator enumerator = symbol.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
					while (enumerator.MoveNext())
					{
						TypeSymbol current = enumerator.Current;
						typeParameterKind |= DetectReferencesToGenericParameters(current, track, methodTypeParametersToTreatAsTypeTypeParameters);
						if ((typeParameterKind & track) == track)
						{
							return typeParameterKind;
						}
					}
				}
				symbol = symbol.ContainingType;
			}
			while ((object)symbol != null);
			return typeParameterKind;
		}

		private static TypeParameterKind DetectReferencesToGenericParameters(TypeParameterSymbol symbol, TypeParameterKind track, BitVector methodTypeParametersToTreatAsTypeTypeParameters)
		{
			if (symbol.ContainingSymbol.Kind == SymbolKind.NamedType)
			{
				if ((track & TypeParameterKind.Type) != 0)
				{
					return TypeParameterKind.Type;
				}
			}
			else if (methodTypeParametersToTreatAsTypeTypeParameters.IsNull || !methodTypeParametersToTreatAsTypeTypeParameters[symbol.Ordinal])
			{
				if ((track & TypeParameterKind.Method) != 0)
				{
					return TypeParameterKind.Method;
				}
			}
			else if ((track & TypeParameterKind.Type) != 0)
			{
				return TypeParameterKind.Type;
			}
			return TypeParameterKind.None;
		}

		private static TypeParameterKind DetectReferencesToGenericParameters(TypeSymbol @this, TypeParameterKind track, BitVector methodTypeParametersToTreatAsTypeTypeParameters)
		{
			switch (@this.Kind)
			{
			case SymbolKind.TypeParameter:
				return DetectReferencesToGenericParameters((TypeParameterSymbol)@this, track, methodTypeParametersToTreatAsTypeTypeParameters);
			case SymbolKind.ArrayType:
				return DetectReferencesToGenericParameters(((ArrayTypeSymbol)@this).ElementType, track, methodTypeParametersToTreatAsTypeTypeParameters);
			case SymbolKind.ErrorType:
			case SymbolKind.NamedType:
				return DetectReferencesToGenericParameters((NamedTypeSymbol)@this, track, methodTypeParametersToTreatAsTypeTypeParameters);
			default:
				throw ExceptionUtilities.UnexpectedValue(@this.Kind);
			}
		}

		private static bool ShadowBasedOnReceiverType(CandidateAnalysisResult left, CandidateAnalysisResult right, ref bool leftWins, ref bool rightWins, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol receiverType = left.Candidate.ReceiverType;
			TypeSymbol receiverType2 = right.Candidate.ReceiverType;
			if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(receiverType, receiverType2))
			{
				if (DoesReceiverMatchInstance(receiverType, receiverType2, ref useSiteInfo))
				{
					leftWins = true;
					return true;
				}
				if (DoesReceiverMatchInstance(receiverType2, receiverType, ref useSiteInfo))
				{
					rightWins = true;
					return true;
				}
			}
			return false;
		}

		public static bool DoesReceiverMatchInstance(TypeSymbol instanceType, TypeSymbol receiverType, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return Conversions.HasWideningDirectCastConversionButNotEnumTypeConversion(instanceType, receiverType, ref useSiteInfo);
		}

		private static bool ShadowBasedOnParamArrayUsage(CandidateAnalysisResult left, CandidateAnalysisResult right, ref bool leftWins, ref bool rightWins)
		{
			if (left.IsExpandedParamArrayForm)
			{
				if (!right.IsExpandedParamArrayForm)
				{
					rightWins = true;
					return true;
				}
				if (left.ExpandedParamArrayArgumentsUsed > right.ExpandedParamArrayArgumentsUsed)
				{
					rightWins = true;
					return true;
				}
				if (left.ExpandedParamArrayArgumentsUsed < right.ExpandedParamArrayArgumentsUsed)
				{
					leftWins = true;
					return true;
				}
			}
			else if (right.IsExpandedParamArrayForm)
			{
				leftWins = true;
				return true;
			}
			return false;
		}

		internal static TypeSymbol GetParameterTypeFromVirtualSignature(ref CandidateAnalysisResult candidate, int paramIndex)
		{
			TypeSymbol typeSymbol = candidate.Candidate.Parameters(paramIndex).Type;
			if (candidate.IsExpandedParamArrayForm && paramIndex == candidate.Candidate.ParameterCount - 1 && typeSymbol.Kind == SymbolKind.ArrayType)
			{
				typeSymbol = ((ArrayTypeSymbol)typeSymbol).ElementType;
			}
			return typeSymbol;
		}

		private static TypeSymbol GetParameterTypeFromVirtualSignature(ref CandidateAnalysisResult candidate, int paramIndex, ref TypeSymbol typeForGenericityCheck)
		{
			ParameterSymbol parameterSymbol = candidate.Candidate.Parameters(paramIndex);
			ParameterSymbol parameterSymbol2 = parameterSymbol.OriginalDefinition;
			if (parameterSymbol2.ContainingSymbol.Kind == SymbolKind.Method)
			{
				MethodSymbol methodSymbol = (MethodSymbol)parameterSymbol2.ContainingSymbol;
				if (methodSymbol.IsReducedExtensionMethod)
				{
					parameterSymbol2 = methodSymbol.ReducedFrom.Parameters[paramIndex + 1];
				}
			}
			TypeSymbol typeSymbol = parameterSymbol.Type;
			typeForGenericityCheck = parameterSymbol2.Type;
			if (candidate.IsExpandedParamArrayForm && paramIndex == candidate.Candidate.ParameterCount - 1 && typeSymbol.Kind == SymbolKind.ArrayType)
			{
				typeSymbol = ((ArrayTypeSymbol)typeSymbol).ElementType;
				typeForGenericityCheck = ((ArrayTypeSymbol)typeForGenericityCheck).ElementType;
			}
			return typeSymbol;
		}

		internal static void AdvanceParameterInVirtualSignature(ref CandidateAnalysisResult candidate, ref int paramIndex)
		{
			if (!candidate.IsExpandedParamArrayForm || paramIndex != candidate.Candidate.ParameterCount - 1)
			{
				paramIndex++;
			}
		}

		private static bool InferTypeArguments(ref CandidateAnalysisResult candidate, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNames, TypeSymbol delegateReturnType, BoundNode delegateReturnTypeReferenceBoundNode, [In][Out] ref HashSet<BoundExpression> asyncLambdaSubToFunctionMismatch, Binder binder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ArrayBuilder<int> parameterToArgumentMap = null;
			ArrayBuilder<int> paramArrayItems = null;
			BuildParameterToArgumentMap(ref candidate, arguments, argumentNames, ref parameterToArgumentMap, ref paramArrayItems);
			if (candidate.State == CandidateAnalysisResultState.Applicable)
			{
				ImmutableArray<TypeSymbol> typeArguments = default(ImmutableArray<TypeSymbol>);
				TypeArgumentInference.InferenceLevel inferenceLevel = TypeArgumentInference.InferenceLevel.None;
				bool allFailedInferenceIsDueToObject = false;
				bool someInferenceFailed = false;
				InferenceErrorReasons inferenceErrorReasons = InferenceErrorReasons.Other;
				BitVector inferredTypeByAssumption = default(BitVector);
				ImmutableArray<SyntaxNodeOrToken> typeArgumentsLocation = default(ImmutableArray<SyntaxNodeOrToken>);
				if (TypeArgumentInference.Infer((MethodSymbol)candidate.Candidate.UnderlyingSymbol, arguments, parameterToArgumentMap, paramArrayItems, delegateReturnType, delegateReturnTypeReferenceBoundNode, ref typeArguments, ref inferenceLevel, ref allFailedInferenceIsDueToObject, ref someInferenceFailed, ref inferenceErrorReasons, out inferredTypeByAssumption, out typeArgumentsLocation, ref asyncLambdaSubToFunctionMismatch, ref useSiteInfo, ref candidate.TypeArgumentInferenceDiagnosticsOpt))
				{
					candidate.SetInferenceLevel(inferenceLevel);
					candidate.Candidate = candidate.Candidate.Construct(typeArguments);
					if (binder.OptionStrict == OptionStrict.On && !inferredTypeByAssumption.IsNull)
					{
						int num = typeArguments.Length - 1;
						for (int i = 0; i <= num; i++)
						{
							if (inferredTypeByAssumption[i])
							{
								BindingDiagnosticBag bindingDiagnosticBag = candidate.TypeArgumentInferenceDiagnosticsOpt;
								if (bindingDiagnosticBag == null)
								{
									bindingDiagnosticBag = (candidate.TypeArgumentInferenceDiagnosticsOpt = BindingDiagnosticBag.Create(withDiagnostics: true, useSiteInfo.AccumulatesDependencies));
								}
								Binder.ReportDiagnostic(bindingDiagnosticBag, typeArgumentsLocation[i], ERRID.WRN_TypeInferenceAssumed3, candidate.Candidate.TypeParameters[i], ((MethodSymbol)candidate.Candidate.UnderlyingSymbol).OriginalDefinition, typeArguments[i]);
							}
						}
					}
				}
				else
				{
					candidate.State = CandidateAnalysisResultState.TypeInferenceFailed;
					if (someInferenceFailed)
					{
						candidate.SetSomeInferenceFailed();
					}
					if (allFailedInferenceIsDueToObject)
					{
						candidate.SetAllFailedInferenceIsDueToObject();
						if (!candidate.Candidate.IsExtensionMethod)
						{
							candidate.IgnoreExtensionMethods = true;
						}
					}
					candidate.SetInferenceErrorReasons(inferenceErrorReasons);
					candidate.NotInferredTypeArguments = BitVector.Create(typeArguments.Length);
					int num2 = typeArguments.Length - 1;
					for (int j = 0; j <= num2; j++)
					{
						if ((object)typeArguments[j] == null)
						{
							candidate.NotInferredTypeArguments[j] = true;
						}
					}
				}
			}
			else
			{
				candidate.SetSomeInferenceFailed();
			}
			paramArrayItems?.Free();
			parameterToArgumentMap?.Free();
			return candidate.State == CandidateAnalysisResultState.Applicable;
		}

		private static Candidate ConstructIfNeedTo(Candidate candidate, ImmutableArray<TypeSymbol> typeArguments)
		{
			if (typeArguments.Length > 0)
			{
				return candidate.Construct(typeArguments);
			}
			return candidate;
		}
	}
}
