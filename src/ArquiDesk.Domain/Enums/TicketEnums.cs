using System.ComponentModel.DataAnnotations;

namespace ArquiDesk.Domain.Enums;

public enum TicketType
{
    [Display(Name = "Alteração de Projeto")]
    AlteracaoDeProjeto = 1,
    [Display(Name = "Erro de Medida")]
    ErroDeMedida = 2,
    [Display(Name = "Dúvida Técnica")]
    DuvidaTecnica = 3,
    [Display(Name = "Problema na Produção")]
    ProblemaNaProducao = 4,
    [Display(Name = "Problema na Instalação")]
    ProblemaNaInstalacao = 5,
    [Display(Name = "Assistência Técnica")]
    AssistenciaTecnica = 6,
    [Display(Name = "Garantia")]
    Garantia = 7,
    [Display(Name = "Orçamento")]
    Orcamento = 8
}

public enum TicketPriority
{
    [Display(Name = "Baixa")]
    Baixa = 1,
    [Display(Name = "Média")]
    Media = 2,
    [Display(Name = "Alta")]
    Alta = 3,
    [Display(Name = "Urgente")]
    Urgente = 4
}

public enum TicketStatus
{
    [Display(Name = "Aberto")]
    Aberto = 1,
    [Display(Name = "Em Análise")]
    EmAnalise = 2,
    [Display(Name = "Aguardando Cliente")]
    AguardandoCliente = 3,
    [Display(Name = "Aguardando Projetista")]
    AguardandoProjetista = 4,
    [Display(Name = "Em Produção")]
    EmProducao = 5,
    [Display(Name = "Em Instalação")]
    EmInstalacao = 6,
    [Display(Name = "Resolvido")]
    Resolvido = 7,
    [Display(Name = "Cancelado")]
    Cancelado = 8
}
