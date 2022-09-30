using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundMethodGroup : BoundMethodOrPropertyGroup
    {
        public MemberAccessExpressionSyntax? MemberAccessExpressionSyntax => Syntax as MemberAccessExpressionSyntax;

        public SyntaxNode NameSyntax
        {
            get
            {
                MemberAccessExpressionSyntax memberAccessExpressionSyntax = MemberAccessExpressionSyntax;
                if (memberAccessExpressionSyntax != null)
                {
                    return memberAccessExpressionSyntax.Name;
                }
                return Syntax;
            }
        }

        public BoundExpression? InstanceOpt
        {
            get
            {
                if (base.ReceiverOpt == null || base.ReceiverOpt!.Kind == BoundKind.TypeExpression)
                {
                    return null;
                }
                return base.ReceiverOpt;
            }
        }

        public bool SearchExtensionMethods => ((uint?)Flags & 1u) != 0;

        public override object Display => MessageID.IDS_MethodGroup.Localize();

        public ImmutableArray<TypeWithAnnotations> TypeArgumentsOpt { get; }

        public string Name { get; }

        public ImmutableArray<MethodSymbol> Methods { get; }

        public Symbol? LookupSymbolOpt { get; }

        public DiagnosticInfo? LookupError { get; }

        public BoundMethodGroupFlags? Flags { get; }

        public BoundMethodGroup(SyntaxNode syntax, ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, BoundExpression receiverOpt, string name, ImmutableArray<MethodSymbol> methods, LookupResult lookupResult, BoundMethodGroupFlags flags, bool hasErrors = false)
            : this(syntax, typeArgumentsOpt, name, methods, lookupResult.SingleSymbolOrDefault, lookupResult.Error, flags, receiverOpt, lookupResult.Kind, hasErrors)
        {
        }

        public BoundMethodGroup(SyntaxNode syntax, ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, string name, ImmutableArray<MethodSymbol> methods, Symbol? lookupSymbolOpt, DiagnosticInfo? lookupError, BoundMethodGroupFlags? flags, BoundExpression? receiverOpt, LookupResultKind resultKind, bool hasErrors = false)
            : base(BoundKind.MethodGroup, syntax, receiverOpt, resultKind, hasErrors || receiverOpt.HasErrors())
        {
            TypeArgumentsOpt = typeArgumentsOpt;
            Name = name;
            Methods = methods;
            LookupSymbolOpt = lookupSymbolOpt;
            LookupError = lookupError;
            Flags = flags;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitMethodGroup(this);
        }

        public BoundMethodGroup Update(ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, string name, ImmutableArray<MethodSymbol> methods, Symbol? lookupSymbolOpt, DiagnosticInfo? lookupError, BoundMethodGroupFlags? flags, BoundExpression? receiverOpt, LookupResultKind resultKind)
        {
            if (typeArgumentsOpt != TypeArgumentsOpt || name != Name || methods != Methods || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(lookupSymbolOpt, LookupSymbolOpt) || lookupError != LookupError || flags != Flags || receiverOpt != base.ReceiverOpt || resultKind != ResultKind)
            {
                BoundMethodGroup boundMethodGroup = new BoundMethodGroup(Syntax, typeArgumentsOpt, name, methods, lookupSymbolOpt, lookupError, flags, receiverOpt, resultKind, base.HasErrors);
                boundMethodGroup.CopyAttributes(this);
                return boundMethodGroup;
            }
            return this;
        }
    }
}
