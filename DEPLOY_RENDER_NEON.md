# Deploy do ArquiDesk no Render + Neon

Este projeto ja esta preparado para rodar no Render usando PostgreSQL no Neon.

## 1. Criar banco no Neon

1. Acesse `https://console.neon.tech`.
2. Crie um novo projeto.
3. Abra `Connection details`.
4. Copie a connection string no formato `.NET` se estiver disponivel.

Use uma string parecida com esta no Render:

```text
Host=ep-seu-host.neon.tech;Database=neondb;Username=neondb_owner;Password=SUA_SENHA;SSL Mode=Require;Trust Server Certificate=true
```

Se voce copiar uma URL `postgresql://...`, o ArquiDesk tambem converte automaticamente.

## 2. Subir codigo para GitHub

O Render publica a partir de um repositorio Git.

```powershell
git init
git add .
git commit -m "Prepare ArquiDesk for Render and Neon"
git branch -M main
git remote add origin URL_DO_SEU_REPOSITORIO
git push -u origin main
```

## 3. Criar Web Service no Render

1. Acesse `https://dashboard.render.com`.
2. Clique em `New`.
3. Escolha `Web Service`.
4. Conecte o repositorio do GitHub.
5. Escolha `Docker`.
6. Plano: `Free`.
7. O Render vai usar o `Dockerfile` deste projeto.

## 4. Variaveis de ambiente no Render

Cadastre estas variaveis:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=Host=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true
```

## 5. Primeiro acesso

Na primeira inicializacao, o ArquiDesk aplica migrations automaticamente e cria o usuario admin:

```text
Email: admin@arquidesk.com
Senha: Admin@123456
```

Tambem existe cadastro normal pela tela de login para criar novos usuarios com e-mail e senha.

## Observacao sobre anexos

Hoje os anexos sao salvos no disco local da aplicacao. No Render Free, isso serve para demonstracao, mas arquivos podem sumir em redeploy. Para producao real, use Supabase Storage, S3, Cloudinary ou outro storage externo.
