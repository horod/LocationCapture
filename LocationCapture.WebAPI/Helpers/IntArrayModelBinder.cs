using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationCapture.WebAPI.Helpers
{
    public class IntArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Specify a default argument name if none is set by ModelBinderAttribute
            var modelName = bindingContext.BinderModelName;
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = "intArray";
            }

            // Try to fetch the value of the argument by name
            var valueProviderResult =
                bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName,
                valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            var valuesAsStr = value.Split(',');
            var intValues = new List<int>();
            foreach(var idStr in valuesAsStr)
            {
                int id = 0;
                if (!int.TryParse(idStr, out id))
                {
                    // Non-integer arguments result in model state errors
                    bindingContext.ModelState.TryAddModelError(
                                            bindingContext.ModelName,
                                            "Array element must be an integer.");
                    return Task.CompletedTask;
                }
                intValues.Add(id);
            }

            bindingContext.Result = ModelBindingResult.Success(intValues);
            return Task.CompletedTask;
        }
    }
}
