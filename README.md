# Plataforma EAD - CRUD com ASP.NET Core MVC

Aplicação ASP.NET Core MVC para gerenciar alunos, cursos e matrículas de uma plataforma EAD. O foco é modelar um relacionamento N:N com carga usando Entity Framework Core e PostgreSQL, com validações de domínio, buscas e feedback de UX.

## Tecnologias
- .NET 9 + ASP.NET Core MVC
- Entity Framework Core 9
- PostgreSQL (Npgsql)
- Bootstrap 5 (layout padrão do template)

## Configuração do ambiente
1. Ajuste a string de conexão em `CrudNN.Web/appsettings.json` (`DefaultConnection`).
2. Opcionalmente use variáveis de ambiente (`ASPNETCORE_ConnectionStrings__DefaultConnection`).
3. Crie/aplique o banco de dados:
   ```bash
   dotnet ef database update --project CrudNN.Web --startup-project CrudNN.Web
   ```

## Execução
```bash
dotnet run --project CrudNN.Web
```
A aplicação ficará disponível em `https://localhost:5001` (ou `http://localhost:5000`).

## Decisões importantes
- **Data/hora das matrículas:** armazenada em UTC no banco (`timestamp with time zone`). As datas são convertidas para horário local apenas na camada de apresentação.
- **Valores decimais:** configuramos `pt-BR` como cultura padrão para garantir ponto flutuante com vírgula nos formulários. No banco, preços usam precisão `decimal(18,2)`.
- **Integridade das matrículas:** a chave composta (`AlunoId`, `CursoId`) impede duplicidade. Exclusões em cascata são bloqueadas (`DeleteBehavior.Restrict`) para preservar histórico.
- **Regras de negócio:**
  - Status `Concluido` exige progresso `100`.
  - Preço = 0; Progresso entre 0..100; Nota final opcional (0..10).
  - Pelo menos um curso deve ser selecionado ao salvar matrículas de um aluno.

## Funcionalidades
- CRUD completo de alunos e cursos, com busca por texto e alertas de sucesso/erro.
- Painel inicial com métricas gerais e últimas matrículas.
- Tela de gerenciamento por aluno para matricular/remover cursos em lote e atualizar progresso, status e notas.
- Listagem de matrículas com filtros.

## Desenvolvimento
- Gerar nova migration: `dotnet ef migrations add NomeDaMigration --project CrudNN.Web --startup-project CrudNN.Web`
- Executar build: `dotnet build`

