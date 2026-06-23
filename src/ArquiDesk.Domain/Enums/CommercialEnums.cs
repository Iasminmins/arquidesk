using System.ComponentModel.DataAnnotations;

namespace ArquiDesk.Domain.Enums;

public enum LeadStatus
{
    [Display(Name = "Novo")]
    Novo = 1,
    [Display(Name = "Tentar Contato")]
    TentarContato = 2,
    [Display(Name = "Atendido")]
    Atendido = 3,
    [Display(Name = "Interessado")]
    Interessado = 4,
    [Display(Name = "Projeto Enviado")]
    ProjetoEnviado = 5,
    [Display(Name = "Negociação")]
    Negociacao = 6,
    [Display(Name = "Cliente Futuro")]
    ClienteFuturo = 7,
    [Display(Name = "Sem Interesse")]
    SemInteresse = 8,
    [Display(Name = "Não Atende")]
    NaoAtende = 9,
    [Display(Name = "Vendido")]
    Vendido = 10,
    [Display(Name = "Desistência")]
    Desistencia = 11
}

public enum NegotiationStatus
{
    [Display(Name = "Aberta")]
    Aberta = 1,
    [Display(Name = "Apresentação Marcada")]
    ApresentacaoMarcada = 2,
    [Display(Name = "Orçamento Enviado")]
    OrcamentoEnviado = 3,
    [Display(Name = "Aguardando Retorno")]
    AguardandoRetorno = 4,
    [Display(Name = "Ganha")]
    Ganha = 5,
    [Display(Name = "Perdida")]
    Perdida = 6
}

public enum InstallationStatus
{
    [Display(Name = "Aguardando Faturamento")]
    AguardandoFaturamento = 1,
    [Display(Name = "Pedido Chegou")]
    PedidoChegou = 2,
    [Display(Name = "Montagem Agendada")]
    MontagemAgendada = 3,
    [Display(Name = "Em Montagem")]
    EmMontagem = 4,
    [Display(Name = "Concluída")]
    Concluida = 5,
    [Display(Name = "Reagendar")]
    Reagendar = 6
}

public enum AssistanceStatus
{
    [Display(Name = "Aberta")]
    Aberta = 1,
    [Display(Name = "Visita Agendada")]
    VisitaAgendada = 2,
    [Display(Name = "Visita Feita")]
    VisitaFeita = 3,
    [Display(Name = "Pedido Efetuado")]
    PedidoEfetuado = 4,
    [Display(Name = "Concluída")]
    Concluida = 5,
    [Display(Name = "Cancelada")]
    Cancelada = 6
}
