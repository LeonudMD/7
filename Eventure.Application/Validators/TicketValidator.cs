using FluentValidation;
using Eventure.Application.Contracts;

namespace Eventure.Application.Validators
{
    public class TicketRequestValidator : AbstractValidator<TicketRequest>
    {
        public TicketRequestValidator()
        {
            RuleFor(ticket => ticket.name)
                .NotEmpty().WithMessage("Имя обязательно для заполнения.")
                .Length(2, 100).WithMessage("Имя должно содержать от 2 до 100 символов.")
                .Matches(@"^[a-zA-Zа-яА-ЯёЁ0-9\s\-]+$").WithMessage("Имя может содержать только буквы, цифры, пробелы и дефисы.");

            RuleFor(ticket => ticket.location)
                .NotEmpty().WithMessage("Место обязательно для заполнения.")
                .Length(2, 200).WithMessage("Место должно содержать от 2 до 200 символов.")
                .Matches(@"^[a-zA-Zа-яА-ЯёЁ0-9\s\-\,\.]+$").WithMessage("Место может содержать только буквы, цифры, пробелы, дефисы, запятые и точки.");

            RuleFor(ticket => ticket.date)
                .NotEmpty().WithMessage("Дата обязательна для заполнения.")
                .GreaterThan(DateTime.Now).WithMessage("Дата должна быть в будущем.")
                .LessThan(DateTime.Now.AddYears(1)).WithMessage("Дата не может быть более чем через 1 год от сегодняшнего дня.");

            RuleFor(ticket => ticket.freeSeats)
                .NotEmpty().WithMessage("Количество свободных мест обязательно для заполнения.")
                .GreaterThan(0).WithMessage("Количество свободных мест должно быть больше 0.")
                .LessThan(100000).WithMessage("Количество свободных мест не может превышать 99999.");

            RuleFor(ticket => ticket.price)?
                .NotEmpty().WithMessage("Цена обязательна для заполнения.")
                .GreaterThan(0).WithMessage("Цена должна быть больше 0.")
                .Must(price => price < 100000).WithMessage("Цена не может превышать 100000.");
        }
        public async Task ValidateOrThrowAsync(TicketRequest request)
        {
            var validationResult = await ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}
