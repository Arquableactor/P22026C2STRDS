import type {
  Claim,
  ClaimInput,
  ClaimStats,
  ClaimStatus,
  Client,
  ClientInput,
} from './types'

const BASE_URL = (import.meta.env.VITE_API_URL ?? 'http://localhost:5233').replace(/\/$/, '')

/** Error de transporte o de negocio devuelto por la API. */
export class ApiError extends Error {
  readonly status: number
  /** Errores por campo tal como los devuelve ValidationProblemDetails. */
  readonly fieldErrors: Record<string, string[]>

  constructor(message: string, status: number, fieldErrors: Record<string, string[]> = {}) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.fieldErrors = fieldErrors
  }
}

interface ProblemDetails {
  title?: string
  detail?: string
  message?: string
  errors?: Record<string, string[]>
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response

  try {
    response = await fetch(`${BASE_URL}${path}`, {
      ...init,
      headers: {
        'Content-Type': 'application/json',
        ...(init?.headers ?? {}),
      },
    })
  } catch {
    throw new ApiError(
      `No se pudo contactar el servicio en ${BASE_URL}. Verifique que ArquanixApi esté ejecutándose.`,
      0,
    )
  }

  if (!response.ok) {
    let problem: ProblemDetails = {}
    try {
      problem = (await response.json()) as ProblemDetails
    } catch {
      /* la respuesta no traía cuerpo JSON */
    }

    const mensaje =
      problem.message ??
      problem.detail ??
      problem.title ??
      `La solicitud falló con código ${response.status}.`

    throw new ApiError(mensaje, response.status, problem.errors ?? {})
  }

  if (response.status === 204) {
    return undefined as T
  }

  return (await response.json()) as T
}

function query(params: Record<string, string | number | boolean | undefined>): string {
  const search = new URLSearchParams()
  for (const [clave, valor] of Object.entries(params)) {
    if (valor !== undefined && valor !== '') {
      search.append(clave, String(valor))
    }
  }
  const texto = search.toString()
  return texto ? `?${texto}` : ''
}

export const api = {
  baseUrl: BASE_URL,

  // ----- Clientes -----
  listClients(filtros: { activos?: boolean; busqueda?: string } = {}) {
    return request<Client[]>(`/api/clients${query(filtros)}`)
  },
  getClient(id: number) {
    return request<Client>(`/api/clients/${id}`)
  },
  createClient(body: ClientInput) {
    return request<Client>('/api/clients', { method: 'POST', body: JSON.stringify(body) })
  },
  updateClient(id: number, body: ClientInput) {
    return request<void>(`/api/clients/${id}`, { method: 'PUT', body: JSON.stringify(body) })
  },
  deleteClient(id: number) {
    return request<void>(`/api/clients/${id}`, { method: 'DELETE' })
  },

  // ----- Reclamos -----
  listClaims(filtros: { clientId?: number; status?: ClaimStatus } = {}) {
    return request<Claim[]>(`/api/claims${query(filtros)}`)
  },
  getClaim(id: number) {
    return request<Claim>(`/api/claims/${id}`)
  },
  createClaim(body: ClaimInput) {
    return request<Claim>('/api/claims', { method: 'POST', body: JSON.stringify(body) })
  },
  updateClaim(id: number, body: ClaimInput) {
    return request<void>(`/api/claims/${id}`, { method: 'PUT', body: JSON.stringify(body) })
  },
  deleteClaim(id: number) {
    return request<void>(`/api/claims/${id}`, { method: 'DELETE' })
  },
  stats() {
    return request<ClaimStats>('/api/claims/stats')
  },
}
