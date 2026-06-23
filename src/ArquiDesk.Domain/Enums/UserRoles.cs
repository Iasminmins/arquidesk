namespace ArquiDesk.Domain.Enums;

public static class UserRoles
{
    public const string Administrador = "Administrador";
    public const string Arquiteto = "Arquiteto";
    public const string Projetista = "Projetista";
    public const string Instalador = "Instalador";
    public const string Cliente = "Cliente";

    public static readonly string[] All =
    [
        Administrador,
        Arquiteto,
        Projetista,
        Instalador,
        Cliente
    ];
}
