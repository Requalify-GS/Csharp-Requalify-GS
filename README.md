# 🎓 Requalify – Plataforma de Requalificação Profissional (Reskilling AI)

Este projeto foi desenvolvido como parte do Global Solution FIAP, aplicando conceitos avançados de Desenvolvimento Web com ASP.NET Core, Integração com Banco Oracle, Observabilidade (Logging + Tracing) e Machine Learning (ML.NET).

A Requalify é uma plataforma voltada para auxiliar usuários a descobrirem novas áreas profissionais com base em suas habilidades, experiências e formação — permitindo que empresas ou candidatos encontrem caminhos de requalificação de forma inteligente.

Este projeto foi desenvolvido como parte do Challenge da FIAP em parceria com a empresa **Mottu**, para a disciplina de **Desenvolvimento Web com ASP.NET Core**.  
O objetivo é construir uma **API RESTful** para gerenciamento de usuários, pendings e bikes. Esses usuários utilizarão o sistema para realizar o aluguel de motos.

---

## 📚 Tecnologias Utilizadas

### 🔧 Backend

- ASP.NET Core 8 Web API
- C#
- Entity Framework Core + Oracle Provider
- Swagger / OpenAPI v3
- API Versioning
- HATEOAS (Hypermedia as the Engine of Application State)

### 📊 Infra & Observabilidade

- OpenTelemetry (Tracing)
- Console Exporter
- Logging estruturado
- Health Checks (API + Oracle)

### 🤖 Machine Learning

- ML.NET (modelo de recomendação de área profissional)
- Classificação baseada em cargo, skill principal, nível e formação

### 🧪 Testes

- xUnit
- Moq
- FluentAssertions
- EF Core InMemory / SQLite
- WebApplicationFactory

---

## 👥 Integrantes
- João Vitor da Silva Nascimento - RM554694 - TURMA 2TDSPZ
- Rafael Souza Bezerra - RM555357 - TURMA 2TDSPZ
- Guilherme Alves Pedroso - RM557888 - TURMA 2TDSPZ

---

## 🧠 Domínio: Requalificação Profissional Inteligente

A Requalify foi desenvolvida como uma solução moderna capaz de:

- ✔ Gerenciar perfis profissionais
- ✔ Registrar habilidades, formação e cursos
- ✔ Realizar previsões de “área ideal” usando Machine Learning
- ✔ Auxiliar empresas e estudantes em planejamento de carreira

🔑 Entidades centrais

- User – dados profissionais e pessoais
- Skill – habilidades com nível e categoria
- Education – histórico educacional
- Course – cursos realizados pelo usuário
- ML Model – recomendação de área profissional

## 🏛️ Arquitetura

A solução segue boas práticas de mercado:

✔ Arquitetura em camadas

- Controllers
- Services
- DTOs
- Mappers
- Models
- HATEOAS
- Paginação

✔ Clean API

- Versionamento: v1, v2, v3, v4
- Separação clara por responsabilidade
- Documentação automática por versão

✔ Banco de dados Oracle (Tablespace FIAP)

- Persistência estruturada em tabelas normalizadas
- ORM: Entity Framework Core

✔ Observabilidade integrada

- Logging estruturado em todas as Services
- Tracing com ActivitySource (OpenTelemetry)
- Tracing de:
- Requests de API
- Acesso ao banco
- Execução de Services
- Predição de Machine Learning

✔ ML.NET

- Modelo treinado manualmente
- Inferência em tempo real via endpoint
- Não depende de arquivos CSV externos

---

## 🎯 Funcionalidades da API
### 👤 User (v1)
- Criar usuário
- Listar usuários (com paginação + HATEOAS)
- Buscar por ID
- Buscar por e-mail
- Atualizar
- Deletar

### ⭐ Skills (v2)
- CRUD completo
- Listar skills do usuário
- Paginação + HATEOAS

### 🎓 Education (v3)
- CRUD
- Buscar por usuário
- Paginação + HATEOAS

### 📘 Courses (v4)
- CRUD
- Buscar por usuário
- Paginação + HATEOAS

### 🤖 ML – Machine Learning
- 🔥 /api/ml/predict-interest
- Modelo ML.NET que prevê “área de carreira mais adequada” baseado em:
- cargo atual
- skill principal
- nível do skill
- formação acadêmica
---

## 🔗 Rotas da API

### 🧠 ML.NET Prediction

| Método | Endpoint                           | Descrição                        |
|--------|------------------------------------|----------------------------------|
| POST   | `/api/ml/predict-interest`         | Recomendação de área profissional|


### 👤 Users — v1 (CRUD)

| Método | Endpoint                           | Descrição                        |
|--------|------------------------------------|----------------------------------|
| GET    | `/api/v1/users`                    | Lista todos os usuários          |
| GET    | `/api/v1/users/{id}`               | Retorna um usuário específico    |
| POST   | `/api/v1/users`                    | Cadastra um novo usuário         |
| PUT    | `/api/v1/users/{id}`               | Atualiza os dados de um usuário  |
| DELETE | `/api/v1/users/{id}`               | Remove um usuário                |


### ⭐ Skills — v2 (CRUD)

| Método | Endpoint                           | Descrição                        |
|--------|------------------------------------|----------------------------------|
| GET    | `/api/v2/skills`                   | Lista todos as skills          |
| GET    | `/api/v2/skills/user/{id}`         | Retorna uma skill específico    |
| POST   | `/api/v2/skills`                   | Cadastra uma skill         |
| PUT    | `/api/v2/skills/{id}`              | Atualiza a skill |
| DELETE | `/api/v2/skills/{id}`              | Remove uma skill               |


### 🎓 Education — v3 (CRUD)

| Método | Endpoint                           | Descrição                        |
|--------|------------------------------------|----------------------------------|
| GET    | `/api/v3/education`                | Lista todos as formações          |
| GET    | `/api/v3/education/user/{id}`      | Retorna uma formação específico    |
| POST   | `/api/v3/education`                | Cadastra uma nova formação         |
| PUT    | `/api/v3/education/{id}`           | Atualiza uma formação   |
| DELETE | `/api/v3/education/{id}`           | Remove uma formação                |

### 📘 Courses — v4 (CRUD)

| Método | Endpoint                           | Descrição                        |
|--------|------------------------------------|----------------------------------|
| GET    | `/api/v3/courses`                | Lista todos os cursos          |
| GET    | `/api/v3/courses/user/{id}`      | Retorna um curso específico    |
| POST   | `/api/v3/courses`                | Cadastra um nova curso         |
| PUT    | `/api/v3/courses/{id}`           | Atualiza um curso   |
| DELETE | `/api/v3/courses/{id}`           | Remove um curso                |

---

## 📥 Exemplo de Requisição

### 🔸 POST `/api/v1/users`

### 🔸 Exemplo de Requisição (POST /api/v1/users)

```json {
{
  "nome": "string",
  "email": "string",
  "senha": "string",
  "telefone": "string",
  "dataNascimento": "2025-11-20T19:44:38.666Z",
  "cargoAtual": "string",
  "areaInteresse": "string"
}
````

🔸 Exemplo de Resposta (201 Created)
```json {
{
  "id": 22,
  "nome": "Maria Luísa Andrade",
  "email": "marialuisa.andrade@example.com",
  "telefone": "+55 21 99988-1234",
  "dataNascimento": "2003-04-22T00:00:00Z",
  "cargoAtual": "Estagiária de Desenvolvimento",
  "areaInteresse": "Front-end e UI/UX",
  "skills": null,
  "courses": null,
  "educations": null,
  "links": [
    {
      "rel": "self",
      "href": "/api/v1/users/22",
      "method": "GET"
    },
    {
      "rel": "update",
      "href": "/api/v1/users/22",
      "method": "PUT"
    },
    {
      "rel": "delete",
      "href": "/api/v1/users/22",
      "method": "DELETE"
    }
  ]
}
````

### 🔸 POST `/api/v2/skills`

### 🔸 Exemplo de Requisição (POST /api/v2/skills)

```json {
{
  "name": "string",
  "level": "string",
  "category": "string",
  "proficiencyPercentage": 0,
  "description": "string",
  "userId": 0
}
````

🔸 Exemplo de Resposta (201 Created)
```json {
{
  "id": 2,
  "name": "React",
  "level": "Intermediário",
  "category": "Front-end",
  "proficiencyPercentage": 70,
  "description": "Conhecimento em criação de interfaces modernas, hooks, context API e integração com APIs.",
  "links": [
    {
      "rel": "self",
      "href": "/api/v2/skills/2",
      "method": "GET"
    },
    {
      "rel": "update",
      "href": "/api/v2/skills/2",
      "method": "PUT"
    },
    {
      "rel": "delete",
      "href": "/api/v2/skills/2",
      "method": "DELETE"
    }
  ]
}
````

### 🔸 POST `/api/v3/education`

### 🔸 Exemplo de Requisição (POST /api/v3/education)

```json {
{
  "degree": "Formação em Business Intelligence com Power BI",
  "instituion": "Udemy",
  "completionDate": "2025-02-10T00:00:00.000Z",
  "certificate": "udemy_powerbi_2025.pdf",
  "userId": 1
}
````

🔸 Exemplo de Resposta (201 Created)
```json {
{
  "id": 3,
  "degree": "Formação em Business Intelligence com Power BI",
  "instituion": "Udemy",
  "completionDate": "2025-02-10T00:00:00Z",
  "certificate": "udemy_powerbi_2025.pdf",
  "links": [
    {
      "rel": "self",
      "href": "/api/v3/education/3",
      "method": "GET"
    },
    {
      "rel": "update",
      "href": "/api/v3/education/3",
      "method": "PUT"
    },
    {
      "rel": "delete",
      "href": "/api/v3/education/3",
      "method": "DELETE"
    }
  ]
}
````

### 🔸 POST `/api/v4/courses`

### 🔸 Exemplo de Requisição (POST /api/v4/courses)

```json {
{
  "title": "Dashboard de Indicadores Operacionais",
  "description": "Criação de um dashboard completo com KPIs de desempenho, métricas financeiras e relatórios interativos utilizando Power BI.",
  "category": "Data Analytics",
  "difficulty": "Intermediário",
  "url": "https://example.com/powerbi-dashboard",
  "userId": 1
}
````

🔸 Exemplo de Resposta (201 Created)
```json {
{
  "id": 1,
  "title": "Dashboard de Indicadores Operacionais",
  "description": "Criação de um dashboard completo com KPIs de desempenho, métricas financeiras e relatórios interativos utilizando Power BI.",
  "category": "Data Analytics",
  "difficulty": "Intermediário",
  "url": "https://example.com/powerbi-dashboard",
  "links": [
    {
      "rel": "self",
      "href": "/api/v4/courses/1",
      "method": "GET"
    },
    {
      "rel": "update",
      "href": "/api/v4/courses/1",
      "method": "PUT"
    },
    {
      "rel": "delete",
      "href": "/api/v4/courses/1",
      "method": "DELETE"
    }
  ]
}
````

## 📦 Códigos de Resposta HTTP
- 200	OK (requisição bem-sucedida)
- 201	Created (recurso criado)
- 204	No Content (sem conteúdo)
- 400	Bad Request (erro na requisição)
- 404	Not Found (recurso não encontrado)

## 🚀 Instalação e Execução
Clone o repositório:
git clone https://github.com/seu-usuario/seu-projeto.git

## 🧪 Testes Unitários — Passo a Passo

Esta seção orienta como configurar, escrever e executar testes unitários do projeto Challenge Mottu com xUnit, Moq e FluentAssertions. Inclui exemplos de teste para Services, Controllers e camada de dados usando EF Core com SQLite in-memory.

## Preparar o projeto de testes

No diretório da solução (onde está o .sln):

### 1. Criar o projeto de testes (xUnit)
dotnet new xunit -n Requalify.Tests

### 2. Adicionar o projeto de testes à solution
dotnet sln add Requalify.Tests/Requalify.Tests.csproj

### 3. Referenciar o projeto principal (ajuste o caminho/nome se necessário)
dotnet add Requalify.Tests reference Requalify/Requalify.csproj

### 4. Adicionar pacotes úteis
- dotnet add Requalify.Tests package Moq
- dotnet add Requalify.Tests package FluentAssertions
- dotnet add Requalify.Tests package Microsoft.AspNetCore.Mvc.Testing
- dotnet add Requalify.Tests package Microsoft.EntityFrameworkCore.Sqlite
- dotnet add Requalify.Tests package Microsoft.EntityFrameworkCore.InMemory
- dotnet add Requalify.Tests package coverlet.collector

## Convenções e estrutura
## Estrutura do Projeto

      /Requalify.Tests
      └─ Services/ 
      │  └─ UsuarioServiceTests.cs
      │  └─ BikeServiceTests.cs
      │  └─ PendingServiceTests.cs

---


## Padrão AAA (Arrange–Act–Assert) em todos os testes.

## Como rodar os testes
1) Via CLI (recomendado)

No diretório da solução (.sln):

## Restaurar dependências
dotnet restore

## Compilar em modo Release (opcional, mas comum em CI)
dotnet build -c Release

## Executar todos os testes da solução
dotnet test -c Release

## Rodar apenas um projeto de testes
dotnet test ./Requalify.Tests/Requalify.Tests.csproj -c Release

## COMO RODAR O PROJETO
Abra o projeto no Visual Studio.

Configure a string de conexão com o banco de dados no arquivo appsettings.json.

Execute a aplicação (pressionando F5) ou via terminal:

dotnet run

Acesse a documentação Swagger para testar os endpoints:

http://localhost:{porta}/swagger


## 🎉 FIAP • Global Solution – Requalify AI
