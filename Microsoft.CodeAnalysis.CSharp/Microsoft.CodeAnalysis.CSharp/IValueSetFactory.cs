using System;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal interface IValueSetFactory
    {
        IValueSet AllValues { get; }

        IValueSet NoValues { get; }

        IValueSet Related(BinaryOperatorKind relation, ConstantValue value);

        bool Related(BinaryOperatorKind relation, ConstantValue left, ConstantValue right);

        IValueSet Random(int expectedSize, Random random);

        ConstantValue RandomValue(Random random);
    }
    internal interface IValueSetFactory<T> : IValueSetFactory
    {
        IValueSet<T> Related(BinaryOperatorKind relation, T value);
    }
}
