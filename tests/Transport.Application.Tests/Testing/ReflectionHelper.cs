using System.Reflection;

namespace Transport.Application.Tests.Testing;

internal static class ReflectionHelper
{
    public static void SetProperty<TTarget, TValue>(TTarget target, string propertyName, TValue value)
    {
        var property = typeof(TTarget).GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        if (property is null)
            throw new InvalidOperationException($"Property {propertyName} was not found on {typeof(TTarget).Name}.");

        property.SetValue(target, value);
    }
}
