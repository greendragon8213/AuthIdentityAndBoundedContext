using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Interfaces
{
    public interface IValidationService
    {
        //List<ValidationResult> GetValidationResults(IValidatable model);
        void ValidateModel(IValidatable model);
        void ValidateModel<TValidatable>(List<TValidatable> model) where TValidatable : IValidatable;
    }
}