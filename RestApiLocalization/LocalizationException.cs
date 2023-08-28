using System.Runtime.Serialization;

namespace RestApiLocalization;

/// <summary>Payroll exception</summary>
public class LocalizationException : Exception
{
    /// <inheritdoc/>
    public LocalizationException()
    {
    }

    /// <inheritdoc/>
    public LocalizationException(string message) :
        base(message)
    {
    }

    /// <inheritdoc/>
    public LocalizationException(string message, Exception innerException) :
        base(message, innerException)
    {
    }

    /// <inheritdoc/>
    protected LocalizationException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
    }
}