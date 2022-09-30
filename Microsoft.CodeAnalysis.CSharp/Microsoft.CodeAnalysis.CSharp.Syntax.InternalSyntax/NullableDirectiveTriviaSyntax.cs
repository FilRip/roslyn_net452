using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class NullableDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        internal readonly SyntaxToken hashToken;

        internal readonly SyntaxToken nullableKeyword;

        internal readonly SyntaxToken settingToken;

        internal readonly SyntaxToken? targetToken;

        internal readonly SyntaxToken endOfDirectiveToken;

        internal readonly bool isActive;

        public override SyntaxToken HashToken => hashToken;

        public SyntaxToken NullableKeyword => nullableKeyword;

        public SyntaxToken SettingToken => settingToken;

        public SyntaxToken? TargetToken => targetToken;

        public override SyntaxToken EndOfDirectiveToken => endOfDirectiveToken;

        public override bool IsActive => isActive;

        public NullableDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken nullableKeyword, SyntaxToken settingToken, SyntaxToken? targetToken, SyntaxToken endOfDirectiveToken, bool isActive, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(nullableKeyword);
            this.nullableKeyword = nullableKeyword;
            AdjustFlagsAndWidth(settingToken);
            this.settingToken = settingToken;
            if (targetToken != null)
            {
                AdjustFlagsAndWidth(targetToken);
                this.targetToken = targetToken;
            }
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public NullableDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken nullableKeyword, SyntaxToken settingToken, SyntaxToken? targetToken, SyntaxToken endOfDirectiveToken, bool isActive, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(nullableKeyword);
            this.nullableKeyword = nullableKeyword;
            AdjustFlagsAndWidth(settingToken);
            this.settingToken = settingToken;
            if (targetToken != null)
            {
                AdjustFlagsAndWidth(targetToken);
                this.targetToken = targetToken;
            }
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public NullableDirectiveTriviaSyntax(SyntaxKind kind, SyntaxToken hashToken, SyntaxToken nullableKeyword, SyntaxToken settingToken, SyntaxToken? targetToken, SyntaxToken endOfDirectiveToken, bool isActive)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(hashToken);
            this.hashToken = hashToken;
            AdjustFlagsAndWidth(nullableKeyword);
            this.nullableKeyword = nullableKeyword;
            AdjustFlagsAndWidth(settingToken);
            this.settingToken = settingToken;
            if (targetToken != null)
            {
                AdjustFlagsAndWidth(targetToken);
                this.targetToken = targetToken;
            }
            AdjustFlagsAndWidth(endOfDirectiveToken);
            this.endOfDirectiveToken = endOfDirectiveToken;
            this.isActive = isActive;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => hashToken,
                1 => nullableKeyword,
                2 => settingToken,
                3 => targetToken,
                4 => endOfDirectiveToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.NullableDirectiveTriviaSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNullableDirectiveTrivia(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitNullableDirectiveTrivia(this);
        }

        public NullableDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken nullableKeyword, SyntaxToken settingToken, SyntaxToken targetToken, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || nullableKeyword != NullableKeyword || settingToken != SettingToken || targetToken != TargetToken || endOfDirectiveToken != EndOfDirectiveToken)
            {
                NullableDirectiveTriviaSyntax nullableDirectiveTriviaSyntax = SyntaxFactory.NullableDirectiveTrivia(hashToken, nullableKeyword, settingToken, targetToken, endOfDirectiveToken, isActive);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    nullableDirectiveTriviaSyntax = nullableDirectiveTriviaSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    nullableDirectiveTriviaSyntax = nullableDirectiveTriviaSyntax.WithAnnotationsGreen(annotations);
                }
                return nullableDirectiveTriviaSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new NullableDirectiveTriviaSyntax(base.Kind, hashToken, nullableKeyword, settingToken, targetToken, endOfDirectiveToken, isActive, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new NullableDirectiveTriviaSyntax(base.Kind, hashToken, nullableKeyword, settingToken, targetToken, endOfDirectiveToken, isActive, GetDiagnostics(), annotations);
        }

        public NullableDirectiveTriviaSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            hashToken = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            nullableKeyword = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            settingToken = node3;
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                targetToken = syntaxToken;
            }
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            endOfDirectiveToken = node4;
            isActive = reader.ReadBoolean();
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(hashToken);
            writer.WriteValue(nullableKeyword);
            writer.WriteValue(settingToken);
            writer.WriteValue(targetToken);
            writer.WriteValue(endOfDirectiveToken);
            writer.WriteBoolean(isActive);
        }

        static NullableDirectiveTriviaSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(NullableDirectiveTriviaSyntax), (ObjectReader r) => new NullableDirectiveTriviaSyntax(r));
        }
    }
}
