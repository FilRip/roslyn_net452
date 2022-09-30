using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ForBlockSyntax : ForOrForEachBlockSyntax
	{
		internal readonly ForStatementSyntax _forStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ForStatementSyntax ForStatement => _forStatement;

		internal ForBlockSyntax(SyntaxKind kind, ForStatementSyntax forStatement, GreenNode statements, NextStatementSyntax nextStatement)
			: base(kind, statements, nextStatement)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(forStatement);
			_forStatement = forStatement;
		}

		internal ForBlockSyntax(SyntaxKind kind, ForStatementSyntax forStatement, GreenNode statements, NextStatementSyntax nextStatement, ISyntaxFactoryContext context)
			: base(kind, statements, nextStatement)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(forStatement);
			_forStatement = forStatement;
		}

		internal ForBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ForStatementSyntax forStatement, GreenNode statements, NextStatementSyntax nextStatement)
			: base(kind, errors, annotations, statements, nextStatement)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(forStatement);
			_forStatement = forStatement;
		}

		internal ForBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ForStatementSyntax forStatementSyntax = (ForStatementSyntax)reader.ReadValue();
			if (forStatementSyntax != null)
			{
				AdjustFlagsAndWidth(forStatementSyntax);
				_forStatement = forStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_forStatement);
		}

		static ForBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new ForBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ForBlockSyntax), (ObjectReader r) => new ForBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _forStatement, 
				1 => _statements, 
				2 => _nextStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ForBlockSyntax(base.Kind, newErrors, GetAnnotations(), _forStatement, _statements, _nextStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ForBlockSyntax(base.Kind, GetDiagnostics(), annotations, _forStatement, _statements, _nextStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitForBlock(this);
		}
	}
}
