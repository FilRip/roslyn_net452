using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class ClassBlockSyntax : TypeBlockSyntax
	{
		internal readonly ClassStatementSyntax _classStatement;

		internal readonly EndBlockStatementSyntax _endClassStatement;

		internal static Func<ObjectReader, object> CreateInstance;

		internal ClassStatementSyntax ClassStatement => _classStatement;

		internal EndBlockStatementSyntax EndClassStatement => _endClassStatement;

		public override TypeStatementSyntax BlockStatement => ClassStatement;

		public override EndBlockStatementSyntax EndBlockStatement => EndClassStatement;

		internal ClassBlockSyntax(SyntaxKind kind, ClassStatementSyntax classStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endClassStatement)
			: base(kind, inherits, implements, members)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(classStatement);
			_classStatement = classStatement;
			AdjustFlagsAndWidth(endClassStatement);
			_endClassStatement = endClassStatement;
		}

		internal ClassBlockSyntax(SyntaxKind kind, ClassStatementSyntax classStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endClassStatement, ISyntaxFactoryContext context)
			: base(kind, inherits, implements, members)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(classStatement);
			_classStatement = classStatement;
			AdjustFlagsAndWidth(endClassStatement);
			_endClassStatement = endClassStatement;
		}

		internal ClassBlockSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, ClassStatementSyntax classStatement, GreenNode inherits, GreenNode implements, GreenNode members, EndBlockStatementSyntax endClassStatement)
			: base(kind, errors, annotations, inherits, implements, members)
		{
			base._slotCount = 5;
			AdjustFlagsAndWidth(classStatement);
			_classStatement = classStatement;
			AdjustFlagsAndWidth(endClassStatement);
			_endClassStatement = endClassStatement;
		}

		internal ClassBlockSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			ClassStatementSyntax classStatementSyntax = (ClassStatementSyntax)reader.ReadValue();
			if (classStatementSyntax != null)
			{
				AdjustFlagsAndWidth(classStatementSyntax);
				_classStatement = classStatementSyntax;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = (EndBlockStatementSyntax)reader.ReadValue();
			if (endBlockStatementSyntax != null)
			{
				AdjustFlagsAndWidth(endBlockStatementSyntax);
				_endClassStatement = endBlockStatementSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_classStatement);
			writer.WriteValue(_endClassStatement);
		}

		static ClassBlockSyntax()
		{
			CreateInstance = (ObjectReader o) => new ClassBlockSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(ClassBlockSyntax), (ObjectReader r) => new ClassBlockSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _classStatement, 
				1 => _inherits, 
				2 => _implements, 
				3 => _members, 
				4 => _endClassStatement, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new ClassBlockSyntax(base.Kind, newErrors, GetAnnotations(), _classStatement, _inherits, _implements, _members, _endClassStatement);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new ClassBlockSyntax(base.Kind, GetDiagnostics(), annotations, _classStatement, _inherits, _implements, _members, _endClassStatement);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitClassBlock(this);
		}
	}
}
