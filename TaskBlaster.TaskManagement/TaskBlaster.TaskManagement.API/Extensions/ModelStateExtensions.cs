using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TaskBlaster.TaskManagement.API.Extensions;

public static class ModelStateExtensions
{
    public static string RetrieveErrorString(this ModelStateDictionary modelState) =>
        modelState.Values.Aggregate("Model was not properly formatted \n",
            (current1, value) => value.Errors.Aggregate(current1,
                (current, error) =>
                    current + $"Attempted value: {value.AttemptedValue}, Error: {error.ErrorMessage}\n"));
}