using MediatR;
using SUPBank.Domain.Contstants;
using SUPBank.Domain.Responses;
using System.ComponentModel.DataAnnotations;

namespace SUPBank.Application.Features.Menu.Commands.Requests
{
    public class UpdateMenuCommandRequest : IRequest<IResponse>
    {
        [Range(1, long.MaxValue)]
        public required long Id { get; set; }

        [Range(0, long.MaxValue)]
        public required long ParentId { get; set; }

        [MinLength(LengthLimits.MenuNameMinLength)]
        [MaxLength(LengthLimits.MenuNameMaxLength)]
        public required string Name_TR { get; set; }

        [MinLength(LengthLimits.MenuNameMinLength)]
        [MaxLength(LengthLimits.MenuNameMaxLength)]
        public required string Name_EN { get; set; }

        [Range(LengthLimits.MenuScreenCodeMinRange, int.MaxValue)]
        public required int ScreenCode { get; set; }

        [Range(1, byte.MaxValue)]
        public required byte Type { get; set; }

        [Range(1, int.MaxValue)]
        public required int Priority { get; set; }

        [MinLength(LengthLimits.MenuKeywordMinLength)]
        [MaxLength(LengthLimits.MenuKeywordMaxLength)]
        public required string Keyword { get; set; }

        [MinLength(LengthLimits.MenuIconMinLength)]
        [MaxLength(LengthLimits.MenuIconMaxLength)]
        public string? Icon { get; set; }

        public required bool IsGroup { get; set; }

        public required bool IsNew { get; set; }

        public DateTime? NewStartDate { get; set; }

        public DateTime? NewEndDate { get; set; }

        public required bool IsActive { get; set; }
    }
}
