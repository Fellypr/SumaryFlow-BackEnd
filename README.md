<div align="center">

# SumaryYoutube API

**O cérebro por trás da plataforma.**  
Backend robusto em C# responsável por autenticação, regras de negócio, integração com IA e orquestração dos serviços da aplicação.

[![C#](https://img.shields.io/badge/C%23-.NET_8-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-5C2D91?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-4169E1?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![JWT](https://img.shields.io/badge/JWT-Authentication-000000?style=flat-square&logo=jsonwebtokens)](https://jwt.io/)
[![Docker](https://img.shields.io/badge/Docker-Container-2496ED?style=flat-square&logo=docker)](https://www.docker.com/)

</div>

---

## Sobre o Backend

O **SumaryYoutube API** foi projetado para centralizar toda a inteligência operacional da plataforma.

Ele é responsável por gerenciar autenticação e autorização de usuários, orquestrar requisições entre frontend, IA e microsserviços, processar regras de negócio, persistir dados de usuários e histórico, e garantir segurança, escalabilidade e organização arquitetural.

> Mais do que uma API comum, ele atua como núcleo estratégico da aplicação.

---

## Responsabilidades Principais

| Camada | Função |
|---|---|
| 🔐 **Auth** | Login, registro e emissão de JWT |
| 🧠 **Business Rules** | Regras centrais da aplicação |
| 🔄 **Orquestração** | Comunicação com IA e microsserviço Python |
| 💾 **Persistence** | Operações com PostgreSQL |
| ⚠️ **Exceptions** | Tratamento padronizado de erros |
| 📡 **API REST** | Endpoints consumidos pelo frontend |

---

## Arquitetura

O projeto foi estruturado com foco em **baixo acoplamento**, manutenção e crescimento futuro.

```
┌────────────────────────────────────────────────────┐
│                    Presentation                    │
│              Controllers · Endpoints               │
└────────────────────┬───────────────────────────────┘
                     │
┌────────────────────▼───────────────────────────────┐
│                    Application                     │
│         Services · DTOs · Interfaces               │
│         Regras de negócio · Casos de uso           │
└────────────────────┬───────────────────────────────┘
                     │
┌────────────────────▼───────────────────────────────┐
│                     Domain                         │
│               Models · Entidades                   │
└────────────────────┬───────────────────────────────┘
                     │
┌────────────────────▼───────────────────────────────┐
│                 Infrastructure                     │
│     PostgreSQL · EF Core · JWT · External APIs     │
└────────────────────────────────────────────────────┘
```

### Estrutura do Projeto

```
📦 SumaryYoutube.API
 ┣ 📂 Controllers
 ┣ 📂 Services
 ┣ 📂 Interfaces
 ┣ 📂 DTOs
 ┣ 📂 Models
 ┣ 📂 Exceptions
 ┣ 📂 Data
 ┣ 📂 Configurations
 ┗ 📄 Program.cs
```

---

## Stack

**Core**
- C# · .NET 8 · ASP.NET Core Web API

**Banco de Dados**
- PostgreSQL · Entity Framework Core

**Segurança**
- JWT Token · BCrypt (hash de senha)

**Integrações**
- Google Gemini API · Microsserviço Python (FastAPI)

**Infraestrutura**
- Docker · Swagger / OpenAPI

---

## Fluxo Principal

```
Frontend envia vídeo
        ↓
API recebe requisição
        ↓
Valida usuário via JWT
        ↓
Chama microsserviço Python
        ↓
Recebe transcript
        ↓
Envia conteúdo para Gemini
        ↓
Processa resposta
        ↓
Salva histórico no banco
        ↓
Retorna resultado ao frontend
```

---

## Decisões Técnicas

**Por que ASP.NET Core?**  
Alta performance, tipagem forte, excelente DI nativo e arquitetura sólida para APIs modernas.

**Por que Entity Framework Core?**  
Produtividade no acesso a dados com migrations, LINQ e manutenção simplificada.

**Por que JWT?**  
Autenticação stateless ideal para aplicações SPA modernas.

**Por que arquitetura em camadas?**  
Facilita manutenção, testes e escalabilidade futura.

---

## Diferenciais Técnicos

Este backend vai além de CRUD simples:

- Arquitetura organizada em camadas
- Injeção de dependência nativa
- Integração com IA generativa
- Comunicação entre múltiplos serviços
- Segurança com JWT + BCrypt
- Tratamento global de exceções
- Persistência relacional robusta
- Containerização com Docker

---

## Impacto do Backend

| Sem Estrutura | Com SumaryYoutube API |
|---|---|
| Código acoplado | Camadas bem definidas |
| API simples CRUD | Backend com orquestração real |
| Segurança básica | JWT + Hashing |
| Sem integrações | IA + Python + Banco |
| Difícil escalar | Base pronta para crescimento |
