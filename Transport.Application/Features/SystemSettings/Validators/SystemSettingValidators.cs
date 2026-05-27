using FluentValidation;
using Transport.Application.Features.SystemSettings.Commands.CreateSystemSetting;
using Transport.Application.Features.SystemSettings.Commands.UpdateSystemSetting;

namespace Transport.Application.Features.SystemSettings.Validators
{
    public class CreateSystemSettingValidator : AbstractValidator<CreateSystemSettingCommand>
    {
        public CreateSystemSettingValidator()
        {
            RuleFor(x => x.Key).NotEmpty().MaximumLength(150);
            RuleFor(x => x.Value).NotNull().MaximumLength(1000);
            RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DataType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }

    public class UpdateSystemSettingValidator : AbstractValidator<UpdateSystemSettingCommand>
    {
        public UpdateSystemSettingValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Value).NotNull().MaximumLength(1000);
            RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
            RuleFor(x => x.DataType).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}
