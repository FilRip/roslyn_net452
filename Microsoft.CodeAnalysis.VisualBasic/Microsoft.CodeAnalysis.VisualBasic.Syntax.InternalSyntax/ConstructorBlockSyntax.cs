using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ConstructorBlockSyntax : MethodBlockBaseSyntax
	{
		internal readonly SubNewStatementSyntax _subNewStatement;

		internal readonly EndBlockStatementSyntax _endSubStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal SubNewStatementSyntax SubNewStatement => _subNewStatement;

		internal EndBlockStatementSyntax EndSubStatement => _endSubStatement;

		public override MethodBaseSyntax Begin => SubNewStatement;

		public override EndBlockStatementSyntax End => EndSubStatement;

		internal ConstructorBlockSyntax(SyntaxKind kind, SubNewStatementSyntax subNewStatement, GreenNode statements, EndBlockStatementSyntax endSubStatement)
			: base(kind, statements)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(subNewStatement);
			_subNewStatement = subNewStatement;
			AdjustFlagsAndWidth(endSubStatement);
			_endSubStatement = endSubStatement;
		}

		internal ConstructorBlockSyntax(SyntaxKind kind, SubNewStatementSyntax subNewStatement, GreenNode statements, EndBlockStatementSyntax endSubStatement, ISyntaxFactoryContext context)
			: base(kind, statements)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(subNewStatement);
			_subNewStatement = subNewStatement;
			AdjustFlagsAndWidth(endSubStatement);
			_endSubStatement = endSubStatement;
		}

		internal ConstructorBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, SubNewStatementSyntax subNewStatement, GreenNode statements, EndBlockStatementSyntax endSubStatement)
			: base(kind, errors, annotations, statements)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(subNewStatement);
			_subNewStatement = subNewStatement;
			AdjustFlagsAndWidth(endSubStatement);
			_endSubStatement = endSubStatement;
		}

		internal ConstructorBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			SubNewStatementSyntax subNewStatementSyntax = (SubNewStatementSyntax)reader.ReadValue();
			if (subNewStatementSyntax != null)
			{
				AdjustFlagsAndWidth(subNewStatementSyntax);
				_subNewStatement = subNewStatementSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endSubStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_subNewStatement);
			writer.WriteValue(_endSubStatement);
		}

		static ConstructorBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new ConstructorBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ConstructorBlockSyntax), (ObjectReader r) => new ConstructorBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstructorBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _subNewStatement, 
				1 => _statements, 
				2 => _endSubStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ConstructorBlockSyntax(base.Kind, newErrors, GetAnnotations(), _subNewStatement, _statements, _endSubStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ConstructorBlockSyntax(base.Kind, GetDiagnostics(), annotations, _subNewStatement, _statements, _endSubStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitConstructorBlock(this);
		}
	}
}
