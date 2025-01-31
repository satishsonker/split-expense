using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SplitExpense.Validator;

namespace SplitExpense.Middleware
{
    public static class RegisterValidators
    {
        public static IServiceCollection RegisterValidator(this IServiceCollection services)
        {
            return services.AddValidatorsFromAssemblyContaining<GroupRequestValidator>()
                .AddValidatorsFromAssemblyContaining<AddFriendInGroupValidator>();
        }
    }
}
