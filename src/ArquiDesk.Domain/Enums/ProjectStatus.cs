using System.ComponentModel.DataAnnotations;

namespace ArquiDesk.Domain.Enums;

public enum ProjectStatus
{
    [Display(Name = "Em Orçamento")]
    EmOrcamento = 1,
    [Display(Name = "Em Projeto")]
    EmProjeto = 2,
    [Display(Name = "Em Produção")]
    EmProducao = 3,
    [Display(Name = "Em Instalação")]
    EmInstalacao = 4,
    [Display(Name = "Entregue")]
    Entregue = 5,
    [Display(Name = "Finalizado")]
    Finalizado = 6
}
