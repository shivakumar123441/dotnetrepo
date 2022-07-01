namespace InvestTrackerWebApi.HttpApi.OpenApi;

using System.Reflection;
using AutoFilterer.Abstractions;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

public class OrderableEnumOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {

        if (context.OperationDescription.Operation is not null)
        {
            foreach (var parameter in context.OperationDescription.Operation.Parameters)
            {
                var parameterDescription = ((AspNetCoreOperationProcessorContext)context).ApiDescription.ParameterDescriptions.FirstOrDefault(x => x.Name.Equals(parameter.Name, StringComparison.InvariantCultureIgnoreCase));
                if (IsOrderableAndSortParameter(parameterDescription))
                {
                    var possibleSortings = parameterDescription?.ModelMetadata.ContainerType?.GetCustomAttribute<PossibleSortingsAttribute>();
                    if (possibleSortings != null)
                    {
                        foreach (var propertyName in possibleSortings.PropertyNames)
                        {
                            parameter.Schema.Enumeration.Add(new string(propertyName));
                        }

                        break;
                    }

                    foreach (var prop in parameterDescription?.ModelMetadata?.ContainerType?.GetProperties()!)
                    {
                        if (!Attribute.IsDefined(prop, typeof(IgnoreFilterAttribute)))
                        {
                            AddPropertyAsEnum(parameter, prop);
                        }
                    }
                }
            }
        }

        return true;
    }

    private static bool IsOrderableAndSortParameter(ApiParameterDescription? param) => param?.ModelMetadata?.ContainerType != null
            && typeof(IOrderable).IsAssignableFrom(param.ModelMetadata.ContainerType)
            && param.Name == nameof(IOrderable.Sort);

    private static void AddPropertyAsEnum(OpenApiParameter item, PropertyInfo prop, string aggregatedName = "")
    {
        var compareTo = prop.GetCustomAttribute<CompareToAttribute>();
        if (compareTo != null)
        {
            foreach (var propertyName in compareTo.PropertyNames)
            {
                if (IsValidPropertyToOrder(propertyName))
                {
                    item.Schema.Enumeration.Add(new string((aggregatedName + "." + propertyName).Trim('.')));
                }
            }
        }
        else if (typeof(IFilter).IsAssignableFrom(prop.PropertyType))
        {
            foreach (var innerProp in prop.PropertyType.GetProperties())
            {
                if (IsValidPropertyToOrder(innerProp.Name))
                {
                    AddPropertyAsEnum(item, innerProp, (aggregatedName + "." + prop.Name).Trim('.'));
                }
            }
        }
        else if (IsValidPropertyToOrder(prop.Name))
        {
            item.Schema.Enumeration.Add(new string((aggregatedName + "." + prop.Name).Trim('.')));
        }
    }

    private static bool IsValidPropertyToOrder(string propertyName)
        => !typeof(OrderableFilterBase).GetProperties().Any(a => a.Name == propertyName);
}
