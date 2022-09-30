using System.Reflection;

using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedEventAccessorSymbol : SourceEventAccessorSymbol
    {
        private readonly object _methodChecksLockObject = new object();

        public override bool IsImplicitlyDeclared => true;

        internal override bool GenerateDebugInfo => false;

        protected override SourceMemberMethodSymbol BoundAttributesSource
        {
            get
            {
                if (MethodKind != MethodKind.EventAdd)
                {
                    return null;
                }
                return (SourceMemberMethodSymbol)base.AssociatedEvent.RemoveMethod;
            }
        }

        protected override IAttributeTargetSymbol AttributeOwner => base.AssociatedEvent;

        protected override object MethodChecksLockObject => _methodChecksLockObject;

        internal override MethodImplAttributes ImplementationAttributes
        {
            get
            {
                MethodImplAttributes methodImplAttributes = base.ImplementationAttributes;
                if (!IsAbstract && !base.AssociatedEvent.IsWindowsRuntimeEvent && !ContainingType.IsStructType() && (object)DeclaringCompilation.GetWellKnownTypeMember(WellKnownMember.System_Threading_Interlocked__CompareExchange_T) == null)
                {
                    methodImplAttributes |= MethodImplAttributes.Synchronized;
                }
                return methodImplAttributes;
            }
        }

        internal SynthesizedEventAccessorSymbol(SourceEventSymbol @event, bool isAdder, EventSymbol explicitlyImplementedEventOpt = null, string aliasQualifierOpt = null)
            : base(@event, null, @event.Locations, explicitlyImplementedEventOpt, aliasQualifierOpt, isAdder, isIterator: false, isNullableAnalysisEnabled: false)
        {
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(base.AssociatedEvent.AttributeDeclarationSyntaxList);
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
        }
    }
}
