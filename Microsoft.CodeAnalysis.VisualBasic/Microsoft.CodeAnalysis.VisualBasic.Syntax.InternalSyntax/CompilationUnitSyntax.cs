using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CompilationUnitSyntax : VisualBasicSyntaxNode
	{
		internal readonly GreenNode _options;

		internal readonly GreenNode _imports;

		internal readonly GreenNode _attributes;

		internal readonly GreenNode _members;

		internal readonly PunctuationSyntax _endOfFileToken;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<OptionStatementSyntax> Options => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<OptionStatementSyntax>(_options);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImportsStatementSyntax> Imports => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImportsStatementSyntax>(_imports);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributesStatementSyntax> Attributes => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributesStatementSyntax>(_attributes);

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> Members => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(_members);

		internal PunctuationSyntax EndOfFileToken => _endOfFileToken;

		internal CompilationUnitSyntax(SyntaxKind kind, GreenNode options, GreenNode imports, GreenNode attributes, GreenNode members, PunctuationSyntax endOfFileToken)
			: base(kind)
		{
			base._slotCount = 5;
			if (options != null)
			{
				AdjustFlagsAndWidth(options);
				_options = options;
			}
			if (imports != null)
			{
				AdjustFlagsAndWidth(imports);
				_imports = imports;
			}
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endOfFileToken);
			_endOfFileToken = endOfFileToken;
		}

		internal CompilationUnitSyntax(SyntaxKind kind, GreenNode options, GreenNode imports, GreenNode attributes, GreenNode members, PunctuationSyntax endOfFileToken, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 5;
			SetFactoryContext(context);
			if (options != null)
			{
				AdjustFlagsAndWidth(options);
				_options = options;
			}
			if (imports != null)
			{
				AdjustFlagsAndWidth(imports);
				_imports = imports;
			}
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endOfFileToken);
			_endOfFileToken = endOfFileToken;
		}

		internal CompilationUnitSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode options, GreenNode imports, GreenNode attributes, GreenNode members, PunctuationSyntax endOfFileToken)
			: base(kind, errors, annotations)
		{
			base._slotCount = 5;
			if (options != null)
			{
				AdjustFlagsAndWidth(options);
				_options = options;
			}
			if (imports != null)
			{
				AdjustFlagsAndWidth(imports);
				_imports = imports;
			}
			if (attributes != null)
			{
				AdjustFlagsAndWidth(attributes);
				_attributes = attributes;
			}
			if (members != null)
			{
				AdjustFlagsAndWidth(members);
				_members = members;
			}
			AdjustFlagsAndWidth(endOfFileToken);
			_endOfFileToken = endOfFileToken;
		}

		internal CompilationUnitSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 5;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_options = greenNode;
			}
			GreenNode greenNode2 = (GreenNode)reader.ReadValue();
			if (greenNode2 != null)
			{
				AdjustFlagsAndWidth(greenNode2);
				_imports = greenNode2;
			}
			GreenNode greenNode3 = (GreenNode)reader.ReadValue();
			if (greenNode3 != null)
			{
				AdjustFlagsAndWidth(greenNode3);
				_attributes = greenNode3;
			}
			GreenNode greenNode4 = (GreenNode)reader.ReadValue();
			if (greenNode4 != null)
			{
				AdjustFlagsAndWidth(greenNode4);
				_members = greenNode4;
			}
			PunctuationSyntax punctuationSyntax = (PunctuationSyntax)reader.ReadValue();
			if (punctuationSyntax != null)
			{
				AdjustFlagsAndWidth(punctuationSyntax);
				_endOfFileToken = punctuationSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_options);
			writer.WriteValue(_imports);
			writer.WriteValue(_attributes);
			writer.WriteValue(_members);
			writer.WriteValue(_endOfFileToken);
		}

		static CompilationUnitSyntax()
		{
			CreateInstance = (ObjectReader o) => new CompilationUnitSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CompilationUnitSyntax), (ObjectReader r) => new CompilationUnitSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _options, 
				1 => _imports, 
				2 => _attributes, 
				3 => _members, 
				4 => _endOfFileToken, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CompilationUnitSyntax(base.Kind, newErrors, GetAnnotations(), _options, _imports, _attributes, _members, _endOfFileToken);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CompilationUnitSyntax(base.Kind, GetDiagnostics(), annotations, _options, _imports, _attributes, _members, _endOfFileToken);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCompilationUnit(this);
		}
	}
}
