namespace InvestTrackerWebApi.HttpApi.OpenApi;

using System.Reflection;
using AutoFilterer.Types;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

public class InnerFilterPropertiesOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {

        if (context.OperationDescription.Operation is not null)
        {
            var ignoredProps = GetIgnoredProperties();
            for (int i = 0; i < context.OperationDescription.Operation.Parameters.Count; i++)
            {
                var parameter = context.OperationDescription.Operation.Parameters[i];
                if (IsIgnored(parameter.Name, ignoredProps))
                {
                    context.OperationDescription.Operation.Parameters.Remove(parameter);
                    i--;
                }
            }
        }

        return true;
    }

    private static PropertyInfo[] GetIgnoredProperties() => typeof(OrderableFilterBase).GetProperties();

    private static bool IsIgnored(string propertyName, PropertyInfo[] properties) =>
        properties.Any(a => propertyName.IndexOf("." + a.Name, StringComparison.InvariantCultureIgnoreCase) > 0);
}
