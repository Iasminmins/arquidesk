namespace ArquiDesk.Domain.Enums;

public static class UserRoles
{
    public const string Administrador = "Administrador";
    public const string Arquiteto = "Arquiteto";
    public const string Projetista = "Projetista";

    // Mantidos apenas para migrar usuários antigos e evitar quebrar referências antigas.
    // Novos usuários não serão cadastrados nesses papéis.
    public const string Instalador = "Instalador";
    public const string Cliente = "Cliente";

    public const string EquipeInterna = $"{Administrador},{Arquiteto},{Projetista}";

    public static readonly string[] All =
    [
        Administrador,
        Arquiteto,
        Projetista
    ];
}
