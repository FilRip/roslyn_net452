using System.Collections.Generic;
using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class CSDiagnosticInfo : DiagnosticInfoWithSymbols
    {
        public static readonly DiagnosticInfo EmptyErrorInfo = new CSDiagnosticInfo(0);

        public static readonly DiagnosticInfo VoidDiagnosticInfo = new CSDiagnosticInfo(ErrorCode.Void);

        private readonly IReadOnlyList<Location> _additionalLocations;

        public override IReadOnlyList<Location> AdditionalLocations => _additionalLocations;

        internal new ErrorCode Code => (ErrorCode)base.Code;

        public CSDiagnosticInfo(ErrorCode code)
            : this(code, new object[0], ImmutableArray<Symbol>.Empty, ImmutableArray<Location>.Empty)
        {
        }

        public CSDiagnosticInfo(ErrorCode code, params object[] args)
            : this(code, args, ImmutableArray<Symbol>.Empty, ImmutableArray<Location>.Empty)
        {
        }

        public CSDiagnosticInfo(ErrorCode code, ImmutableArray<Symbol> symbols, object[] args)
            : this(code, args, symbols, ImmutableArray<Location>.Empty)
        {
        }

        public CSDiagnosticInfo(ErrorCode code, object[] args, ImmutableArray<Symbol> symbols, ImmutableArray<Location> additionalLocations)
            : base(code, args, symbols)
        {
            IReadOnlyList<Location> additionalLocations2;
            if (!additionalLocations.IsDefaultOrEmpty)
            {
                IReadOnlyList<Location> readOnlyList = additionalLocations;
                additionalLocations2 = readOnlyList;
            }
            else
            {
                additionalLocations2 = SpecializedCollections.EmptyReadOnlyList<Location>();
            }
            _additionalLocations = additionalLocations2;
        }

        internal static bool IsEmpty(DiagnosticInfo info)
        {
            return info == EmptyErrorInfo;
        }
    }
}
