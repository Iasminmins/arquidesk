using Microsoft.AspNetCore.Http;

namespace ArquiDesk.Web.Models;

public class ImportSpreadsheetViewModel
{
    public IFormFile? File { get; set; }
    public SpreadsheetImportResult? Result { get; set; }
}

public class SpreadsheetImportResult
{
    public int LeadsImported { get; set; }
    public int NegotiationsImported { get; set; }
    public int InstallationsImported { get; set; }
    public int AssistanceImported { get; set; }
    public int RowsIgnored { get; set; }
    public List<string> Messages { get; set; } = [];

    public int TotalImported => LeadsImported + NegotiationsImported + InstallationsImported + AssistanceImported;
}
