namespace InvestTrackerWebApi.HttpApi.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

public class ModelStateFeature
{
    public ModelStateDictionary ModelState { get; set; }

    public ModelStateFeature(ModelStateDictionary state) => this.ModelState = state;
}
