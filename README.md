# ArquiDesk

Sistema de chamados e controle operacional para arquitetura, marcenarias e empresas de moveis planejados.

## Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core Code First
- PostgreSQL
- ASP.NET Identity
- Bootstrap 5
- AutoMapper
- Repository Pattern
- Clean Architecture
- FluentValidation
- Swagger para APIs auxiliares
- Chart.js no dashboard

## Como executar localmente

Instale o .NET 8 SDK e tenha um PostgreSQL disponivel. Depois execute:

```powershell
dotnet restore
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/ArquiDesk.Infrastructure --startup-project src/ArquiDesk.Web
dotnet run --project src/ArquiDesk.Web
```

A connection string local fica em:

```text
src/ArquiDesk.Web/appsettings.json
```

## Deploy

Para publicar no Render com banco PostgreSQL no Neon, siga o guia:

[DEPLOY_RENDER_NEON.md](DEPLOY_RENDER_NEON.md)

## Login e cadastro

O acesso e o primeiro cadastro usam o fluxo normal de e-mail e senha pelo ASP.NET Identity.

## Migrations

Para criar uma nova migration:

```powershell
dotnet tool run dotnet-ef migrations add NomeDaMigration --project src/ArquiDesk.Infrastructure --startup-project src/ArquiDesk.Web
```

Para aplicar no banco configurado:

```powershell
dotnet tool run dotnet-ef database update --project src/ArquiDesk.Infrastructure --startup-project src/ArquiDesk.Web
```

## Usuario inicial

Usuario inicial criado pelo seed:

- Email: `admin@arquidesk.com`
- Senha: `Admin@123456`

## Estrutura

- `ArquiDesk.Domain`: entidades, enums e contratos de auditoria.
- `ArquiDesk.Application`: DTOs, interfaces, validacoes, mapeamentos e casos de uso.
- `ArquiDesk.Infrastructure`: EF Core, Identity, repositorios, seed e servicos externos.
- `ArquiDesk.Web`: MVC, controllers, views, assets e APIs auxiliares.

## Principais recursos

- Controle de chamados por projeto.
- Controle de leads com atendimento, interesse, envio de projeto e follow-up.
- Controle de negociacoes com custo, valor ofertado, valor a vista e proximo contato.
- Agenda de montagens com pedido recebido e data de instalacao.
- Assistencias tecnicas com visita, pedido efetuado e conclusao.
- SLA automatico por tipo de solicitacao.
- Prioridades e status corporativos.
- Comentarios publicos e internos.
- Upload de PDF, JPG, PNG, DWG e DOCX.
- Dashboard com cards e graficos.
- Soft delete e auditoria.
- Controle de acesso por roles.
- API auxiliar documentada via Swagger.
