using ArquiDesk.Application.DTOs;
using ArquiDesk.Domain.Enums;

namespace ArquiDesk.Web.Models;

public class TicketFilterViewModel
{
    public string? Search { get; set; }
    public TicketStatus? Status { get; set; }
    public TicketPriority? Priority { get; set; }
    public TicketType? Type { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int Total { get; set; }
    public IReadOnlyList<TicketDto> Items { get; set; } = [];
}
