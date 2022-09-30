using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class RaiseEventStatementSyntax : ExecutableStatementSyntax
	{
		internal readonly KeywordSyntax _raiseEventKeyword;

		internal readonly IdentifierNameSyntax _name;

		internal readonly ArgumentListSyntax _argumentList;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax RaiseEventKeyword => _raiseEventKeyword;

		internal IdentifierNameSyntax Name => _name;

		internal ArgumentListSyntax ArgumentList => _argumentList;

		internal RaiseEventStatementSyntax(SyntaxKind kind, KeywordSyntax raiseEventKeyword, IdentifierNameSyntax name, ArgumentListSyntax argumentList)
			: base(kind)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(raiseEventKeyword);
			_raiseEventKeyword = raiseEventKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal RaiseEventStatementSyntax(SyntaxKind kind, KeywordSyntax raiseEventKeyword, IdentifierNameSyntax name, ArgumentListSyntax argumentList, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 3;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(raiseEventKeyword);
			_raiseEventKeyword = raiseEventKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal RaiseEventStatementSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax raiseEventKeyword, IdentifierNameSyntax name, ArgumentListSyntax argumentList)
			: base(kind, errors, annotations)
		{
			base._slotCount = 3;
			AdjustFlagsAndWidth(raiseEventKeyword);
			_raiseEventKeyword = raiseEventKeyword;
			AdjustFlagsAndWidth(name);
			_name = name;
			if (argumentList != null)
			{
				AdjustFlagsAndWidth(argumentList);
				_argumentList = argumentList;
			}
		}

		internal RaiseEventStatementSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 3;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_raiseEventKeyword = keywordSyntax;
			}
			IdentifierNameSyntax identifierNameSyntax = (IdentifierNameSyntax)reader.ReadValue();
			if (identifierNameSyntax != null)
			{
				AdjustFlagsAndWidth(identifierNameSyntax);
				_name = identifierNameSyntax;
			}
			ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)reader.ReadValue();
			if (argumentListSyntax != null)
			{
				AdjustFlagsAndWidth(argumentListSyntax);
				_argumentList = argumentListSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_raiseEventKeyword);
			writer.WriteValue(_name);
			writer.WriteValue(_argumentList);
		}

		static RaiseEventStatementSyntax()
		{
			CreateInstance = (ObjectReader o) => new RaiseEventStatementSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(RaiseEventStatementSyntax), (ObjectReader r) => new RaiseEventStatementSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.RaiseEventStatementSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _raiseEventKeyword, 
				1 => _name, 
				2 => _argumentList, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new RaiseEventStatementSyntax(base.Kind, newErrors, GetAnnotations(), _raiseEventKeyword, _name, _argumentList);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new RaiseEventStatementSyntax(base.Kind, GetDiagnostics(), annotations, _raiseEventKeyword, _name, _argumentList);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitRaiseEventStatement(this);
		}
	}
}
