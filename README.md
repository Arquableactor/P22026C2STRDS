ArquanixApi

API REST para la gestión de **clientes** y **reclamos** (claims) de soporte. Permite registrar clientes, crear reclamos asociados a un cliente y dar seguimiento a su estado y prioridad. Los datos se guardan de forma persistente en una base de datos **SQLite** mediante **Entity Framework Core**.

Objetivo del sistema

Ofrecer un CRUD completo sobre dos entidades:

- **Client**: clientes del sistema (nombre, correo, teléfono, estado activo).
- **Claim**: reclamos asociados a un cliente, con estado (`Open`, `InProgress`, `Resolved`, `Closed`) y prioridad (`Low`, `Medium`, `High`, `Critical`).

Un reclamo siempre debe estar asociado a un cliente existente; no se permite crear reclamos con un `ClientId` inexistente.

Tecnologías utilizadas

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.Sqlite`)
- Base de datos SQLite
- Swagger / Swashbuckle (documentación y pruebas de la API)

Requisitos previos

- [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Herramienta de EF Core (solo para gestionar migraciones manualmente):
