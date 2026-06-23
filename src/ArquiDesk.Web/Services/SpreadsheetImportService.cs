using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml.Linq;
using ArquiDesk.Application.Interfaces;
using ArquiDesk.Domain.Entities;
using ArquiDesk.Domain.Enums;
using ArquiDesk.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ArquiDesk.Web.Services;

public class SpreadsheetImportService(IUnitOfWork unitOfWork)
{
    public async Task<SpreadsheetImportResult> ImportAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var workbook = XlsxWorkbook.Load(stream);
        var result = new SpreadsheetImportResult();

        result.LeadsImported = await ImportLeadsAsync(workbook.GetSheet("LEADS"), result, cancellationToken);
        result.NegotiationsImported = await ImportNegotiationsAsync(workbook.GetSheet("NEGOCIAÇÃO EM ABERTO") ?? workbook.GetSheet("NEGOCIACAO EM ABERTO"), result, cancellationToken);
        result.InstallationsImported = await ImportInstallationsAsync(workbook.GetSheet("MONTAGEM"), result, cancellationToken);
        result.AssistanceImported = await ImportAssistanceAsync(workbook.GetSheet("ASSISTENCIA") ?? workbook.GetSheet("ASSISTÊNCIA"), result, cancellationToken);

        await unitOfWork.SaveChangesAsync();

        if (result.TotalImported == 0)
        {
            result.Messages.Add("Nenhum registro novo foi encontrado para importar.");
        }

        return result;
    }

    private async Task<int> ImportLeadsAsync(IReadOnlyList<IReadOnlyList<string>>? rows, SpreadsheetImportResult result, CancellationToken cancellationToken)
    {
        if (rows is null)
        {
            result.Messages.Add("Aba LEADS não encontrada.");
            return 0;
        }

        var imported = 0;
        foreach (var row in rows.Skip(1))
        {
            var name = Cell(row, 0);
            if (string.IsNullOrWhiteSpace(name))
            {
                result.RowsIgnored++;
                continue;
            }

            var contact = Cell(row, 1);
            if (await LeadExistsAsync(name, contact, cancellationToken))
            {
                result.RowsIgnored++;
                continue;
            }

            var answered = ParseBool(Cell(row, 2));
            var interested = ParseBool(Cell(row, 3));
            var projectSent = ParseBool(Cell(row, 4));
            var observations = Cell(row, 5);

            await unitOfWork.Leads.AddAsync(new Lead
            {
                Name = name,
                Contact = contact,
                Answered = answered,
                Interested = interested,
                ProjectSent = projectSent,
                Observations = observations,
                Status = ResolveLeadStatus(answered, interested, projectSent, observations)
            });
            imported++;
        }

        return imported;
    }

    private async Task<int> ImportNegotiationsAsync(IReadOnlyList<IReadOnlyList<string>>? rows, SpreadsheetImportResult result, CancellationToken cancellationToken)
    {
        if (rows is null)
        {
            result.Messages.Add("Aba NEGOCIAÇÃO EM ABERTO não encontrada.");
            return 0;
        }

        var imported = 0;
        foreach (var row in rows.Skip(1))
        {
            var clientName = Cell(row, 0);
            if (string.IsNullOrWhiteSpace(clientName))
            {
                result.RowsIgnored++;
                continue;
            }

            var contact = Cell(row, 1);
            if (await NegotiationExistsAsync(clientName, contact, cancellationToken))
            {
                result.RowsIgnored++;
                continue;
            }

            await unitOfWork.Negotiations.AddAsync(new Negotiation
            {
                ClientName = clientName,
                Contact = contact,
                Cost = ParseDecimal(Cell(row, 2)),
                LastOfferedValue = ParseDecimal(Cell(row, 3)),
                CashValue = ParseDecimal(Cell(row, 4)),
                UpdatedValue = ParseDecimal(Cell(row, 5)),
                Status = NegotiationStatus.Aberta,
                Notes = Cell(row, 10)
            });
            imported++;
        }

        return imported;
    }

    private async Task<int> ImportInstallationsAsync(IReadOnlyList<IReadOnlyList<string>>? rows, SpreadsheetImportResult result, CancellationToken cancellationToken)
    {
        if (rows is null)
        {
            result.Messages.Add("Aba MONTAGEM não encontrada.");
            return 0;
        }

        var imported = 0;
        foreach (var row in rows.Skip(1))
        {
            var clientName = Cell(row, 0);
            if (string.IsNullOrWhiteSpace(clientName))
            {
                result.RowsIgnored++;
                continue;
            }

            var contact = Cell(row, 1);
            if (await InstallationExistsAsync(clientName, contact, cancellationToken))
            {
                result.RowsIgnored++;
                continue;
            }

            var orderArrived = ParseBool(Cell(row, 3));
            var installationDate = ParseDate(Cell(row, 4));

            await unitOfWork.Installations.AddAsync(new Installation
            {
                ClientName = clientName,
                Contact = contact,
                FactoryBillingDate = ParseDate(Cell(row, 2)),
                OrderArrived = orderArrived,
                InstallationDate = installationDate,
                Status = ResolveInstallationStatus(orderArrived, installationDate)
            });
            imported++;
        }

        return imported;
    }

    private async Task<int> ImportAssistanceAsync(IReadOnlyList<IReadOnlyList<string>>? rows, SpreadsheetImportResult result, CancellationToken cancellationToken)
    {
        if (rows is null)
        {
            result.Messages.Add("Aba ASSISTENCIA não encontrada.");
            return 0;
        }

        var imported = 0;
        foreach (var row in rows.Skip(1))
        {
            var clientName = Cell(row, 0);
            if (string.IsNullOrWhiteSpace(clientName))
            {
                result.RowsIgnored++;
                continue;
            }

            var contact = Cell(row, 1);
            if (await AssistanceExistsAsync(clientName, contact, cancellationToken))
            {
                result.RowsIgnored++;
                continue;
            }

            var visitCompleted = ParseBool(Cell(row, 2));
            var completed = ParseBool(Cell(row, 3));
            var orderPlaced = ParseBool(Cell(row, 4));

            await unitOfWork.AssistanceRequests.AddAsync(new AssistanceRequest
            {
                ClientName = clientName,
                Contact = contact,
                VisitCompleted = visitCompleted,
                Completed = completed,
                OrderPlaced = orderPlaced,
                AssistanceDate = ParseDate(Cell(row, 5)),
                Status = ResolveAssistanceStatus(visitCompleted, completed, orderPlaced)
            });
            imported++;
        }

        return imported;
    }

    private Task<bool> LeadExistsAsync(string name, string? contact, CancellationToken cancellationToken) =>
        unitOfWork.Leads.Query().AnyAsync(x => x.Name == name && x.Contact == contact, cancellationToken);

    private Task<bool> NegotiationExistsAsync(string clientName, string? contact, CancellationToken cancellationToken) =>
        unitOfWork.Negotiations.Query().AnyAsync(x => x.ClientName == clientName && x.Contact == contact, cancellationToken);

    private Task<bool> InstallationExistsAsync(string clientName, string? contact, CancellationToken cancellationToken) =>
        unitOfWork.Installations.Query().AnyAsync(x => x.ClientName == clientName && x.Contact == contact, cancellationToken);

    private Task<bool> AssistanceExistsAsync(string clientName, string? contact, CancellationToken cancellationToken) =>
        unitOfWork.AssistanceRequests.Query().AnyAsync(x => x.ClientName == clientName && x.Contact == contact, cancellationToken);

    private static LeadStatus ResolveLeadStatus(bool? answered, bool? interested, bool? projectSent, string? observations)
    {
        var text = Normalize(observations);
        if (text.Contains("desistencia"))
        {
            return LeadStatus.Desistencia;
        }

        if (text.Contains("sem interesse"))
        {
            return LeadStatus.SemInteresse;
        }

        if (projectSent == true)
        {
            return LeadStatus.ProjetoEnviado;
        }

        if (interested == true)
        {
            return LeadStatus.Interessado;
        }

        if (answered == true)
        {
            return LeadStatus.Atendido;
        }

        if (answered == false)
        {
            return LeadStatus.NaoAtende;
        }

        return LeadStatus.Novo;
    }

    private static InstallationStatus ResolveInstallationStatus(bool? orderArrived, DateTime? installationDate)
    {
        if (installationDate.HasValue)
        {
            return InstallationStatus.MontagemAgendada;
        }

        return orderArrived == true ? InstallationStatus.PedidoChegou : InstallationStatus.AguardandoFaturamento;
    }

    private static AssistanceStatus ResolveAssistanceStatus(bool? visitCompleted, bool? completed, bool? orderPlaced)
    {
        if (completed == true)
        {
            return AssistanceStatus.Concluida;
        }

        if (orderPlaced == true)
        {
            return AssistanceStatus.PedidoEfetuado;
        }

        if (visitCompleted == true)
        {
            return AssistanceStatus.VisitaFeita;
        }

        return AssistanceStatus.Aberta;
    }

    private static string Cell(IReadOnlyList<string> row, int index) =>
        index < row.Count ? row[index].Trim() : string.Empty;

    private static bool? ParseBool(string value)
    {
        var normalized = Normalize(value);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return null;
        }

        if (normalized is "sim" or "s" or "x" or "ok" or "feito" or "concluido" or "1" or "true")
        {
            return true;
        }

        if (normalized is "nao" or "n" or "0" or "false" or "pendente")
        {
            return false;
        }

        return null;
    }

    private static decimal? ParseDecimal(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var clean = value.Replace("R$", "", StringComparison.OrdinalIgnoreCase).Trim();
        if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.GetCultureInfo("pt-BR"), out var ptValue))
        {
            return ptValue;
        }

        if (decimal.TryParse(clean, NumberStyles.Any, CultureInfo.InvariantCulture, out var invariantValue))
        {
            return invariantValue;
        }

        return null;
    }

    private static DateTime? ParseDate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var serial))
        {
            return DateTime.FromOADate(serial);
        }

        var cultures = new[] { CultureInfo.GetCultureInfo("pt-BR"), CultureInfo.InvariantCulture };
        foreach (var culture in cultures)
        {
            if (DateTime.TryParse(value, culture, DateTimeStyles.AssumeLocal, out var date))
            {
                return date.Date;
            }
        }

        return null;
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        return new string(normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());
    }

    private sealed class XlsxWorkbook
    {
        private readonly Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> _sheets;

        private XlsxWorkbook(Dictionary<string, IReadOnlyList<IReadOnlyList<string>>> sheets)
        {
            _sheets = sheets;
        }

        public IReadOnlyList<IReadOnlyList<string>>? GetSheet(string name)
        {
            var normalized = Normalize(name);
            return _sheets.TryGetValue(normalized, out var rows) ? rows : null;
        }

        public static XlsxWorkbook Load(Stream stream)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: true);
            var sharedStrings = ReadSharedStrings(archive);
            var sheetTargets = ReadSheetTargets(archive);
            var sheets = new Dictionary<string, IReadOnlyList<IReadOnlyList<string>>>();

            foreach (var sheet in sheetTargets)
            {
                var entry = archive.GetEntry(ResolveWorkbookTarget(sheet.Target));
                if (entry is null)
                {
                    continue;
                }

                sheets[Normalize(sheet.Name)] = ReadWorksheet(entry, sharedStrings);
            }

            return new XlsxWorkbook(sheets);
        }

        private static List<string> ReadSharedStrings(ZipArchive archive)
        {
            var entry = archive.GetEntry("xl/sharedStrings.xml");
            if (entry is null)
            {
                return [];
            }

            using var entryStream = entry.Open();
            var document = XDocument.Load(entryStream);
            var ns = document.Root?.Name.Namespace ?? XNamespace.None;

            return document.Descendants(ns + "si")
                .Select(si => string.Concat(si.Descendants(ns + "t").Select(t => t.Value)))
                .ToList();
        }

        private static List<SheetTarget> ReadSheetTargets(ZipArchive archive)
        {
            var workbookEntry = archive.GetEntry("xl/workbook.xml") ?? throw new InvalidDataException("Arquivo workbook.xml não encontrado.");
            var relsEntry = archive.GetEntry("xl/_rels/workbook.xml.rels") ?? throw new InvalidDataException("Relacionamentos do workbook não encontrados.");

            using var workbookStream = workbookEntry.Open();
            using var relsStream = relsEntry.Open();

            var workbook = XDocument.Load(workbookStream);
            var rels = XDocument.Load(relsStream);
            var workbookNs = workbook.Root?.Name.Namespace ?? XNamespace.None;
            var relNs = XNamespace.Get("http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            var packageRelNs = rels.Root?.Name.Namespace ?? XNamespace.None;

            var relations = rels.Descendants(packageRelNs + "Relationship")
                .ToDictionary(x => (string?)x.Attribute("Id") ?? string.Empty, x => ((string?)x.Attribute("Target") ?? string.Empty).TrimStart('/'));

            return workbook.Descendants(workbookNs + "sheet")
                .Select(sheet =>
                {
                    var id = (string?)sheet.Attribute(relNs + "id") ?? string.Empty;
                    return new SheetTarget((string?)sheet.Attribute("name") ?? string.Empty, relations.GetValueOrDefault(id, string.Empty));
                })
                .Where(sheet => !string.IsNullOrWhiteSpace(sheet.Name) && !string.IsNullOrWhiteSpace(sheet.Target))
                .ToList();
        }

        private static string ResolveWorkbookTarget(string target)
        {
            var cleanTarget = target.TrimStart('/');
            return cleanTarget.StartsWith("xl/", StringComparison.OrdinalIgnoreCase)
                ? cleanTarget
                : $"xl/{cleanTarget}";
        }

        private static IReadOnlyList<IReadOnlyList<string>> ReadWorksheet(ZipArchiveEntry entry, IReadOnlyList<string> sharedStrings)
        {
            using var entryStream = entry.Open();
            var document = XDocument.Load(entryStream);
            var ns = document.Root?.Name.Namespace ?? XNamespace.None;
            var rows = new List<IReadOnlyList<string>>();

            foreach (var row in document.Descendants(ns + "row"))
            {
                var values = new SortedDictionary<int, string>();
                foreach (var cell in row.Elements(ns + "c"))
                {
                    var reference = (string?)cell.Attribute("r") ?? string.Empty;
                    var columnIndex = GetColumnIndex(reference);
                    values[columnIndex] = ReadCell(cell, ns, sharedStrings);
                }

                if (values.Count == 0)
                {
                    rows.Add([]);
                    continue;
                }

                var max = values.Keys.Max();
                var line = Enumerable.Range(0, max + 1)
                    .Select(index => values.GetValueOrDefault(index, string.Empty))
                    .ToList();

                rows.Add(line);
            }

            return rows;
        }

        private static string ReadCell(XElement cell, XNamespace ns, IReadOnlyList<string> sharedStrings)
        {
            var type = (string?)cell.Attribute("t");
            if (type == "inlineStr")
            {
                return string.Concat(cell.Descendants(ns + "t").Select(t => t.Value));
            }

            var rawValue = cell.Element(ns + "v")?.Value ?? string.Empty;
            if (type == "s" && int.TryParse(rawValue, out var sharedStringIndex) && sharedStringIndex >= 0 && sharedStringIndex < sharedStrings.Count)
            {
                return sharedStrings[sharedStringIndex];
            }

            return rawValue;
        }

        private static int GetColumnIndex(string reference)
        {
            var letters = new string(reference.TakeWhile(char.IsLetter).ToArray());
            var index = 0;
            foreach (var letter in letters)
            {
                index = index * 26 + (char.ToUpperInvariant(letter) - 'A' + 1);
            }

            return Math.Max(index - 1, 0);
        }

        private sealed record SheetTarget(string Name, string Target);
    }
}
