#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal interface IValueSet
    {
        bool IsEmpty { get; }

        ConstantValue? Sample { get; }

        IValueSet Intersect(IValueSet other);

        IValueSet Union(IValueSet other);

        IValueSet Complement();

        bool Any(BinaryOperatorKind relation, ConstantValue value);

        bool All(BinaryOperatorKind relation, ConstantValue value);
    }
    internal interface IValueSet<T> : IValueSet
    {
        IValueSet<T> Intersect(IValueSet<T> other);

        IValueSet<T> Union(IValueSet<T> other);

        new IValueSet<T> Complement();

        bool Any(BinaryOperatorKind relation, T value);

        bool All(BinaryOperatorKind relation, T value);
    }
}
