// Espejo en TypeScript de los DTO que expone ArquanixApi.
// Los enum viajan como texto gracias a JsonStringEnumConverter en el backend.

export type ClaimStatus = 'Open' | 'InProgress' | 'Resolved' | 'Closed'
export type ClaimPriority = 'Low' | 'Medium' | 'High' | 'Critical'

export const CLAIM_STATUSES: ClaimStatus[] = ['Open', 'InProgress', 'Resolved', 'Closed']
export const CLAIM_PRIORITIES: ClaimPriority[] = ['Low', 'Medium', 'High', 'Critical']

export const STATUS_LABEL: Record<ClaimStatus, string> = {
  Open: 'Abierto',
  InProgress: 'En proceso',
  Resolved: 'Resuelto',
  Closed: 'Cerrado',
}

export const PRIORITY_LABEL: Record<ClaimPriority, string> = {
  Low: 'Baja',
  Medium: 'Media',
  High: 'Alta',
  Critical: 'Crítica',
}

export interface Client {
  id: number
  name: string
  email: string
  phone?: string | null
  isActive: boolean
  createdAt: string
  rol: string
  reclamosVigentes: number
}

export interface Claim {
  id: number
  clientId: number
  clientName?: string | null
  title: string
  description: string
  status: ClaimStatus
  priority: ClaimPriority
  createdAt: string
  closedAt?: string | null
  diasDeAtencion: number
  resumen: string
}

export interface ClaimStats {
  total: number
  open: number
  inProgress: number
  resolved: number
  closed: number
  critical: number
  promedioDiasAtencion: number
  totalClientes: number
  clientesActivos: number
}

export interface ClientInput {
  name: string
  email: string
  phone?: string | null
  isActive: boolean
}

export interface ClaimInput {
  clientId: number
  title: string
  description: string
  status: ClaimStatus
  priority: ClaimPriority
}
