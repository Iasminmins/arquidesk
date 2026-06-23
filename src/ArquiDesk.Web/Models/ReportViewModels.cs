namespace ArquiDesk.Web.Models;

public class ReportsViewModel
{
    public int TotalTickets { get; set; }
    public int OpenTickets { get; set; }
    public int OverdueTickets { get; set; }
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int TotalLeads { get; set; }
    public IReadOnlyDictionary<string, int> TicketsByStatus { get; set; } = new Dictionary<string, int>();
    public IReadOnlyDictionary<string, int> TicketsByPriority { get; set; } = new Dictionary<string, int>();
    public IReadOnlyList<RecentTicketReportItem> RecentTickets { get; set; } = [];
}

public class RecentTicketReportItem
{
    public int Number { get; set; }
    public string Project { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime OpenedAt { get; set; }
    public DateTime SlaDueAt { get; set; }
}
