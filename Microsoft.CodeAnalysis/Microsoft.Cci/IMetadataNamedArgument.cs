namespace Microsoft.Cci
{
    public interface IMetadataNamedArgument : IMetadataExpression
    {
        string ArgumentName { get; }

        IMetadataExpression ArgumentValue { get; }

        bool IsField { get; }
    }
}
