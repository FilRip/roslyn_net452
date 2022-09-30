using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ForEachBlockSyntax : ForOrForEachBlockSyntax
	{
		internal readonly ForEachStatementSyntax _forEachStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ForEachStatementSyntax ForEachStatement => _forEachStatement;

		internal ForEachBlockSyntax(SyntaxKind kind, ForEachStatementSyntax forEachStatement, GreenNode statements, NextStatementSyntax nextStatement)
			: base(kind, statements, nextStatement)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(forEachStatement);
			_forEachStatement = forEachStatement;
		}

		internal ForEachBlockSyntax(SyntaxKind kind, ForEachStatementSyntax forEachStatement, GreenNode statements, NextStatementSyntax nextStatement, ISyntaxFactoryContext context)
			: base(kind, statements, nextStatement)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(forEachStatement);
			_forEachStatement = forEachStatement;
		}

		internal ForEachBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ForEachStatementSyntax forEachStatement, GreenNode statements, NextStatementSyntax nextStatement)
			: base(kind, errors, annotations, statements, nextStatement)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(forEachStatement);
			_forEachStatement = forEachStatement;
		}

		internal ForEachBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			ForEachStatementSyntax forEachStatementSyntax = (ForEachStatementSyntax)reader.ReadValue();
			if (forEachStatementSyntax != null)
			{
				AdjustFlagsAndWidth(forEachStatementSyntax);
				_forEachStatement = forEachStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_forEachStatement);
		}

		static ForEachBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new ForEachBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ForEachBlockSyntax), (ObjectReader r) => new ForEachBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _forEachStatement, 
				1 => _statements, 
				2 => _nextStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ForEachBlockSyntax(base.Kind, newErrors, GetAnnotations(), _forEachStatement, _statements, _nextStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ForEachBlockSyntax(base.Kind, GetDiagnostics(), annotations, _forEachStatement, _statements, _nextStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitForEachBlock(this);
		}
	}
}
