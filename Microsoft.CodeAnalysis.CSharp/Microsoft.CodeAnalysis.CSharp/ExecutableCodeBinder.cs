using System;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class ExecutableCodeBinder : Binder
    {
        private readonly Symbol _memberSymbol;

        private readonly SyntaxNode _root;

        private readonly Action<Binder, SyntaxNode> _binderUpdatedHandler;

        private SmallDictionary<SyntaxNode, Binder> _lazyBinderMap;

        internal override Symbol ContainingMemberOrLambda => _memberSymbol ?? base.Next!.ContainingMemberOrLambda;

        protected override bool InExecutableBinder => true;

        internal Symbol MemberSymbol => _memberSymbol;

        private SmallDictionary<SyntaxNode, Binder> BinderMap
        {
            get
            {
                if (_lazyBinderMap == null)
                {
                    ComputeBinderMap();
                }
                return _lazyBinderMap;
            }
        }

        internal ExecutableCodeBinder(SyntaxNode root, Symbol memberSymbol, Binder next, Action<Binder, SyntaxNode> binderUpdatedHandler = null)
            : this(root, memberSymbol, next, next.Flags)
        {
            _binderUpdatedHandler = binderUpdatedHandler;
        }

        internal ExecutableCodeBinder(SyntaxNode root, Symbol memberSymbol, Binder next, BinderFlags additionalFlags)
            : base(next, (next.Flags | additionalFlags) & ~BinderFlags.AllClearedAtExecutableCodeBoundary)
        {
            _memberSymbol = memberSymbol;
            _root = root;
        }

        internal override Binder GetBinder(SyntaxNode node)
        {
            if (!BinderMap.TryGetValue(node, out var value))
            {
                return base.Next!.GetBinder(node);
            }
            return value;
        }

        private void ComputeBinderMap()
        {
            SmallDictionary<SyntaxNode, Binder> smallDictionary;
            if (!(_memberSymbol is SynthesizedSimpleProgramEntryPointSymbol synthesizedSimpleProgramEntryPointSymbol) || _root != synthesizedSimpleProgramEntryPointSymbol.SyntaxNode)
            {
                smallDictionary = (((object)_memberSymbol == null || _root == null) ? SmallDictionary<SyntaxNode, Binder>.Empty : LocalBinderFactory.BuildMap(_memberSymbol, _root, this, _binderUpdatedHandler));
            }
            else
            {
                SimpleProgramBinder simpleProgramBinder = new SimpleProgramBinder(this, synthesizedSimpleProgramEntryPointSymbol);
                smallDictionary = LocalBinderFactory.BuildMap(_memberSymbol, _root, simpleProgramBinder, _binderUpdatedHandler);
                smallDictionary.Add(_root, simpleProgramBinder);
            }
            Interlocked.CompareExchange(ref _lazyBinderMap, smallDictionary, null);
        }

        public static void ValidateIteratorMethod(CSharpCompilation compilation, MethodSymbol iterator, BindingDiagnosticBag diagnostics)
        {
            if (!iterator.IsIterator)
            {
                return;
            }

            foreach (var parameter in iterator.Parameters)
            {
                if (parameter.RefKind != RefKind.None)
                {
                    diagnostics.Add(ErrorCode.ERR_BadIteratorArgType, parameter.Locations[0]);
                }
                else if (parameter.Type.IsUnsafe())
                {
                    diagnostics.Add(ErrorCode.ERR_UnsafeIteratorArgType, parameter.Locations[0]);
                }
            }

            Location errorLocation = (iterator as SynthesizedSimpleProgramEntryPointSymbol)?.ReturnTypeSyntax.GetLocation() ?? iterator.Locations[0];
            if (iterator.IsVararg)
            {
                // error CS1636: __arglist is not allowed in the parameter list of iterators
                diagnostics.Add(ErrorCode.ERR_VarargsIterator, errorLocation);
            }

            if (((iterator as SourceMemberMethodSymbol)?.IsUnsafe == true || (iterator as LocalFunctionSymbol)?.IsUnsafe == true)
                && compilation.Options.AllowUnsafe) // Don't cascade
            {
                diagnostics.Add(ErrorCode.ERR_IllegalInnerUnsafe, errorLocation);
            }

            var returnType = iterator.ReturnType;
            RefKind refKind = iterator.RefKind;
            TypeWithAnnotations elementType = InMethodBinder.GetIteratorElementTypeFromReturnType(compilation, refKind, returnType, errorLocation, diagnostics);

            if (elementType.IsDefault)
            {
                if (refKind != RefKind.None)
                {
                    Error(diagnostics, ErrorCode.ERR_BadIteratorReturnRef, errorLocation, iterator);
                }
                else if (!returnType.IsErrorType())
                {
                    Error(diagnostics, ErrorCode.ERR_BadIteratorReturn, errorLocation, iterator, returnType);
                }
            }

            bool asyncInterface = InMethodBinder.IsAsyncStreamInterface(compilation, refKind, returnType);
            if (asyncInterface && !iterator.IsAsync)
            {
                diagnostics.Add(ErrorCode.ERR_IteratorMustBeAsync, errorLocation, iterator, returnType);
            }
        }
    }
}
