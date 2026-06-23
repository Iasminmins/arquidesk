using ArquiDesk.Domain.Enums;
using ArquiDesk.Web.Models;
using ArquiDesk.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArquiDesk.Web.Controllers;

[Authorize(Roles = $"{UserRoles.Administrador},{UserRoles.Arquiteto},{UserRoles.Projetista}")]
public class ImportsController(SpreadsheetImportService importService) : Controller
{
    public IActionResult Index()
    {
        return View(new ImportSpreadsheetViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ImportSpreadsheetViewModel model, CancellationToken cancellationToken)
    {
        if (model.File is null || model.File.Length == 0)
        {
            ModelState.AddModelError(nameof(model.File), "Selecione uma planilha .xlsx para importar.");
            return View(model);
        }

        var extension = Path.GetExtension(model.File.FileName);
        if (!string.Equals(extension, ".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(model.File), "Envie uma planilha no formato .xlsx.");
            return View(model);
        }

        try
        {
            await using var stream = model.File.OpenReadStream();
            model.Result = await importService.ImportAsync(stream, cancellationToken);
        }
        catch (InvalidDataException)
        {
            ModelState.AddModelError(nameof(model.File), "Não foi possível ler a planilha. Confira se o arquivo é um .xlsx válido.");
        }

        return View(model);
    }
}
