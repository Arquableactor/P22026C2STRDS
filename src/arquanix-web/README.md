# arquanix-web

Cliente web del **Sistema de Gestión de Reclamos Arquanix**. Consume el servicio
REST `ArquanixApi` sobre HTTP. Es el segundo nodo de la arquitectura distribuida:
un proceso independiente, que no comparte memoria ni sesión con el servidor.

**React 18 · TypeScript (modo estricto) · Vite · react-router-dom**

---

## Puesta en marcha

Requiere Node.js 18 o superior.

```bash
cd arquanix-web
cp .env.example .env      # ajusta VITE_API_URL si cambiaste el puerto
npm install
npm run dev               # http://localhost:5173
```

El servicio `ArquanixApi` debe estar corriendo antes, en `http://localhost:5233`.

Para la versión de producción:

```bash
npm run build             # compila TypeScript y empaqueta en dist/
npm run preview           # sirve dist/ en http://localhost:4173
```

`npm run build` corre `tsc --noEmit` primero: si hay un error de tipos, el build falla.
Eso es intencional — es lo que hace que un cambio en el contrato de la API se detecte
al compilar y no en producción.

---

## Estructura

```
src/
├── api/
│   ├── client.ts      Única puerta hacia la API. Ninguna página escribe fetch.
│   └── types.ts       Espejo en TypeScript de los DTO del servidor.
├── components/
│   ├── ui.tsx         Etiquetas de estado y prioridad, medidor de antigüedad,
│   │                  modal, avisos, esqueletos de carga.
│   └── Layout.tsx     Rail de navegación y contenedor.
├── hooks/
│   └── useAsync.ts    Encapsula carga, error y recarga de cualquier consulta.
├── pages/
│   ├── Dashboard.tsx   Indicadores del turno + cola de abiertos.
│   ├── ClaimsPage.tsx  Cola de reclamos con filtros y CRUD.
│   └── ClientsPage.tsx Maestro de clientes con búsqueda y CRUD.
└── styles.css
```

La separación de capas del servidor se repite aquí: las páginas no saben cómo se
hace una petición HTTP, y el módulo de acceso no sabe cómo se pinta un reclamo.

---

## Pantallas

| Ruta | Pantalla | Qué hace |
|---|---|---|
| `/` | Tablero | Cuatro indicadores del turno, distribución por estado, cola de abiertos. |
| `/reclamos` | Cola de reclamos | Filtro por estado, alta y edición en modal, eliminación. |
| `/clientes` | Clientes | Búsqueda por nombre o correo, filtro de activos, alta, edición y baja. |

---

## Decisiones de diseño

**El color nunca decora.** Cada tono corresponde a un estado o una prioridad, y esa
correspondencia es constante en toda la aplicación: azul abierto, ámbar en proceso,
verde resuelto, gris cerrado, rojo crítico.

**Los números van en monoespaciada con cifras tabulares.** Identificadores, días de
atención y conteos se leen alineados de un vistazo, como en un tablero de operación.

**Medidor de antigüedad.** Cada reclamo muestra cinco segmentos que se encienden
contra un objetivo de 10 días. El caso vencido salta a la vista antes de leer una
palabra. Es el elemento distintivo de la interfaz.

---

## Si algo falla

| Síntoma | Causa y solución |
|---|---|
| «No se pudo contactar el servicio» | `ArquanixApi` no está corriendo, o `VITE_API_URL` apunta a otro puerto. |
| Error de CORS en la consola | El origen `http://localhost:5173` no está en `Cors:Origenes` de `appsettings.json`. Agrégalo y reinicia el servicio. |
| El navegador rechaza el certificado | Arranca la API con `--launch-profile http`, no https. |
| Los listados salen vacíos | La carga de demostración solo corre con la base vacía. Borra `arquanix.db` y reinicia la API. |

---

## Nota sobre las variables de entorno

`VITE_API_URL` se lee **en tiempo de compilación**, no en ejecución. Si la cambias,
hay que reiniciar `npm run dev` o volver a compilar.
