using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MemberShip.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddModelErrorList(this ModelStateDictionary modelState, List<string> errors)
        {
            foreach (var error in errors)
            {
                modelState.AddModelError(string.Empty, error);
            }
        }
    }
}
