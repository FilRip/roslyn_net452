using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class NextStatementSyntax : StatementSyntax
	{
		internal readonly KeywordSyntax _nextKeyword;

		internal readonly GreenNode _controlVariables;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax NextKeyword => _nextKeyword;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> ControlVariables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExpressionSyntax>(_controlVariables));

		internal NextStatementSyntax(SyntaxKind kind, KeywordSyntax nextKeyword, GreenNode controlVariables)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(nextKeyword);
			_nextKeyword = nextKeyword;
			if (controlVariables != null)
			{
				AdjustFlagsAndWidth(controlVariables);
				_controlVariables = controlVariables;
			}
		}

		internal NextStatementSyntax(SyntaxKind kind, KeywordSyntax nextKeyword, GreenNode controlVariables, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(nextKeyword);
			_nextKeyword = nextKeyword;
			if (controlVariables != null)
			{
				AdjustFlagsAndWidth(controlVariables);
				_controlVariables = controlVariables;
			}
		}

		internal NextStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax nextKeyword, GreenNode controlVariables)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(nextKeyword);
			_nextKeyword = nextKeyword;
			if (controlVariables != null)
			{
				AdjustFlagsAndWidth(controlVariables);
				_controlVariables = controlVariables;
			}
		}

		internal NextStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_nextKeyword = keywordSyntax;
			}
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_controlVariables = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_nextKeyword);
			writer.WriteValue(_controlVariables);
		}

		static NextStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new NextStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(NextStatementSyntax), (ObjectReader r) => new NextStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.NextStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _nextKeyword, 
				1 => _controlVariables, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new NextStatementSyntax(base.Kind, newErrors, GetAnnotations(), _nextKeyword, _controlVariables);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new NextStatementSyntax(base.Kind, GetDiagnostics(), annotations, _nextKeyword, _controlVariables);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitNextStatement(this);
		}
	}
}
