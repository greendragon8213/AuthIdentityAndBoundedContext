using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BLL.Exceptions;
using BLL.Interfaces;

namespace BLL.Implementation
{
    public abstract class ValidationService : IValidationService
    {
        public List<ValidationResult> GetValidationResults(IValidatable model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        public void ValidateModel(IValidatable model)
        {
            var validationResult = GetValidationResults(model);

            ThrowValidationFailedExceptionIfValidationResultsAny(validationResult);
        }

        public void ValidateModel<TValidatable>(List<TValidatable> model) where TValidatable : IValidatable
        {
            var validationResult = new List<ValidationResult>();
            model?.ForEach(p => validationResult.AddRange(GetValidationResults(p)));

            ThrowValidationFailedExceptionIfValidationResultsAny(validationResult);
        }

        #region Private methods

        private void ThrowValidationFailedExceptionIfValidationResultsAny(List<ValidationResult> validationResult)
        {
            if (validationResult.Any())
            {
                string message = validationResult.Aggregate("", (current, v) => current + v.ErrorMessage + " ");
                throw new ValidationFailedException($"{"Validation failed. "}{message}");
            }
        }

        #endregion
    }
}
