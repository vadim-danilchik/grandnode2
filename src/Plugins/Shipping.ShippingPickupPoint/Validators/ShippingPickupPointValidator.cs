using FluentValidation;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure.Validators;
using Shipping.ShippingPickupPoint.Models;
using System.Collections.Generic;

namespace Shipping.ShippingPickupPoint.Validators
{
    public class ShippingPickupPointValidator : BaseGrandValidator<ShippingPickupPointModel>
    {
        public ShippingPickupPointValidator(
            IEnumerable<IValidatorConsumer<ShippingPickupPointModel>> validators,
            ITranslationService translationService)
            : base(validators)
        {
            RuleFor(x => x.ShippingPickupPointName).NotEmpty().WithMessage(translationService.GetResource("Shipping.ShippingPickupPoint.RequiredShippingPickupPointName"));
            RuleFor(x => x.Description).NotEmpty().WithMessage(translationService.GetResource("Shipping.ShippingPickupPoint.RequiredDescription"));
            RuleFor(x => x.OpeningHours).NotEmpty().WithMessage(translationService.GetResource("Shipping.ShippingPickupPoint.RequiredOpeningHours"));
            RuleFor(x => x.CountryId).NotNull().WithMessage(translationService.GetResource("Admin.Address.Fields.Country.Required"));
            RuleFor(x => x.City).NotEmpty().WithMessage(translationService.GetResource("Admin.Address.Fields.City.Required"));
            RuleFor(x => x.Address1).NotEmpty().WithMessage(translationService.GetResource("Admin.Address.Fields.Address1.Required"));
            RuleFor(x => x.ZipPostalCode).NotEmpty().WithMessage(translationService.GetResource("Admin.Address.Fields.ZipPostalCode.Required"));
        }
    }

}