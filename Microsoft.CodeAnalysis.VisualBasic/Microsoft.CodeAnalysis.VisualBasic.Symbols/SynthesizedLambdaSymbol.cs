using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedLambdaSymbol : LambdaSymbol
	{
		private readonly SynthesizedLambdaKind _kind;

		public override SynthesizedLambdaKind SynthesizedKind => _kind;

		public override bool IsAsync => false;

		public override bool IsIterator => false;

		public override bool IsImplicitlyDeclared => true;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		internal override bool GenerateDebugInfoImpl
		{
			get
			{
				if (_kind != SynthesizedLambdaKind.DelegateRelaxationStub)
				{
					return _kind != SynthesizedLambdaKind.LateBoundAddressOfLambda;
				}
				return false;
			}
		}

		public SynthesizedLambdaSymbol(SynthesizedLambdaKind kind, SyntaxNode syntaxNode, ImmutableArray<BoundLambdaParameterSymbol> parameters, TypeSymbol returnType, Binder binder)
			: base(syntaxNode, parameters, returnType, binder)
		{
			_kind = kind;
		}

		public override bool Equals(object obj)
		{
			return obj == this;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public void SetQueryLambdaReturnType(TypeSymbol returnType)
		{
			m_ReturnType = returnType;
		}
	}
}
