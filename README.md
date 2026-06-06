# ArquanixApi

API REST para la gestión de **clientes** y **reclamos** (claims) de soporte. Permite registrar clientes, crear reclamos asociados a un cliente y dar seguimiento a su estado y prioridad. Los datos se guardan de forma persistente en una base de datos **SQLite** mediante **Entity Framework Core**.

## Objetivo del sistema

Ofrecer un CRUD completo sobre dos entidades:

- **Client**: clientes del sistema (nombre, correo, teléfono, estado activo).
- **Claim**: reclamos asociados a un cliente, con estado (`Open`, `InProgress`, `Resolved`, `Closed`) y prioridad (`Low`, `Medium`, `High`, `Critical`).

Un reclamo siempre debe estar asociado a un cliente existente; no se permite crear reclamos con un `ClientId` inexistente.

## Tecnologías utilizadas

- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.Sqlite`)
- Base de datos SQLite
- Swagger / Swashbuckle (documentación y pruebas de la API)

## Requisitos previos

- [SDK de .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Herramienta de EF Core (solo para gestionar migraciones manualmente):

```bash
dotnet tool install --global dotnet-ef
```

## Cómo ejecutar el proyecto

Desde la raíz del repositorio:

```bash
cd src/ArquanixApi
dotnet run
```

Al iniciar, la aplicación aplica automáticamente las migraciones pendientes (`Database.Migrate()`), por lo que la base de datos `arquanix.db` se crea sola si no existe.

La API queda disponible en:

- HTTP: `http://localhost:5233`
- HTTPS: `https://localhost:7229`

## Swagger

En entorno de desarrollo, Swagger UI permite explorar y probar todos los endpoints desde el navegador:

```
http://localhost:5233/swagger
```

El documento OpenAPI está en `http://localhost:5233/swagger/v1/swagger.json`.

## Base de datos y migraciones

El proyecto usa **SQLite** con **Entity Framework Core**. La cadena de conexión se define en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=arquanix.db"
}
```

La migración inicial (`InitialCreate`) crea las tablas `Clients` y `Claims`. Comandos básicos (ejecutar dentro de `src/ArquanixApi`):

```bash
# Crear una nueva migración
dotnet ef migrations add NombreMigracion

# Aplicar las migraciones y generar/actualizar la base de datos
dotnet ef database update

# Listar las migraciones existentes
dotnet ef migrations list
```

> Nota: no es obligatorio ejecutar `dotnet ef database update` manualmente, ya que la aplicación aplica las migraciones al arrancar.

## Endpoints principales

### Clientes (`/api/clients`)

| Método | Ruta | Descripción | Respuestas |
| --- | --- | --- | --- |
| GET | `/api/clients` | Lista todos los clientes | 200 |
| GET | `/api/clients/{id}` | Obtiene un cliente por Id | 200, 404 |
| POST | `/api/clients` | Crea un cliente | 201, 400 |
| PUT | `/api/clients/{id}` | Actualiza un cliente | 204, 400, 404 |
| DELETE | `/api/clients/{id}` | Elimina un cliente | 204, 404 |

### Reclamos (`/api/claims`)

| Método | Ruta | Descripción | Respuestas |
| --- | --- | --- | --- |
| GET | `/api/claims` | Lista todos los reclamos | 200 |
| GET | `/api/claims/{id}` | Obtiene un reclamo por Id | 200, 404 |
| POST | `/api/claims` | Crea un reclamo asociado a un cliente | 201, 400 |
| PUT | `/api/claims/{id}` | Actualiza un reclamo | 204, 400, 404 |
| DELETE | `/api/claims/{id}` | Elimina un reclamo | 204, 404 |

### Ejemplos de cuerpo (DTOs)

Crear cliente (`POST /api/clients`):

```json
{
  "name": "Pedro Soto",
  "email": "pedro@test.com",
  "phone": "809-777-8888",
  "isActive": true
}
```

Crear reclamo (`POST /api/claims`):

```json
{
  "clientId": 1,
  "title": "Sin señal",
  "description": "No tengo señal de internet desde ayer",
  "status": "Open",
  "priority": "High"
}
```

## Estructura del proyecto

```
src/ArquanixApi/
├── Controllers/      Endpoints de la API (ClientsController, ClaimsController)
├── Data/             ArquanixDbContext (configuración de EF Core)
├── Dtos/             DTOs de entrada y salida (Create/Update/Read)
├── Migrations/       Migraciones de EF Core
├── Models/           Entidades de dominio (Client, Claim) y enums
├── Services/         Stores de acceso a datos (EfClientStore, EfClaimStore)
├── Program.cs        Configuración y arranque de la aplicación
└── appsettings.json  Configuración (incluye la cadena de conexión)
```
