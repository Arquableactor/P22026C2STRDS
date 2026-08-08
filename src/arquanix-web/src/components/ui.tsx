import { useEffect, type CSSProperties, type ReactNode } from 'react'
import {
  PRIORITY_LABEL,
  STATUS_LABEL,
  type ClaimPriority,
  type ClaimStatus,
} from '../api/types'

export const STATUS_COLOR: Record<ClaimStatus, string> = {
  Open: 'var(--open)',
  InProgress: 'var(--progress)',
  Resolved: 'var(--resolved)',
  Closed: 'var(--closed)',
}

export const PRIORITY_COLOR: Record<ClaimPriority, string> = {
  Low: 'var(--closed)',
  Medium: 'var(--open)',
  High: 'var(--progress)',
  Critical: 'var(--critical)',
}

export function StatusTag({ status }: { status: ClaimStatus }) {
  return (
    <span className="tag">
      <i style={{ background: STATUS_COLOR[status] }} aria-hidden="true" />
      {STATUS_LABEL[status]}
    </span>
  )
}

export function PriorityTag({ priority }: { priority: ClaimPriority }) {
  return (
    <span className="tag">
      <i style={{ background: PRIORITY_COLOR[priority] }} aria-hidden="true" />
      {PRIORITY_LABEL[priority]}
    </span>
  )
}

/**
 * Medidor de antigüedad: cinco segmentos que se encienden a medida que el
 * reclamo envejece contra un objetivo de atención de 10 días. Es la lectura
 * que un supervisor necesita de un vistazo antes que cualquier otra.
 */
export function AgingGauge({
  dias,
  cerrado,
  objetivo = 10,
}: {
  dias: number
  cerrado: boolean
  objetivo?: number
}) {
  const segmentos = 5
  const encendidos = Math.min(segmentos, Math.ceil((dias / objetivo) * segmentos))

  let color = 'var(--resolved)'
  if (cerrado) color = 'var(--closed)'
  else if (dias >= objetivo) color = 'var(--critical)'
  else if (dias >= objetivo * 0.6) color = 'var(--progress)'
  else if (dias >= objetivo * 0.3) color = 'var(--open)'

  const etiqueta = cerrado
    ? `Resuelto en ${dias} día(s)`
    : `Abierto hace ${dias} día(s), objetivo ${objetivo}`

  return (
    <div className={cerrado ? 'aging aging--done' : 'aging'} title={etiqueta}>
      <span className="aging-segments" aria-hidden="true">
        {Array.from({ length: segmentos }, (_, i) => (
          <i
            key={i}
            className={i < encendidos ? 'on' : ''}
            style={{ '--seg': color } as CSSProperties}
          />
        ))}
      </span>
      <span className="aging-days">{dias}d</span>
    </div>
  )
}

export function Modal({
  title,
  onClose,
  children,
}: {
  title: string
  onClose: () => void
  children: ReactNode
}) {
  useEffect(() => {
    const alCerrar = (e: KeyboardEvent) => {
      if (e.key === 'Escape') onClose()
    }
    window.addEventListener('keydown', alCerrar)
    return () => window.removeEventListener('keydown', alCerrar)
  }, [onClose])

  return (
    <div
      className="overlay"
      role="dialog"
      aria-modal="true"
      aria-label={title}
      onMouseDown={(e) => {
        if (e.target === e.currentTarget) onClose()
      }}
    >
      <div className="modal">
        <div className="modal-head">
          <h2>{title}</h2>
          <button type="button" className="btn btn--ghost" onClick={onClose} aria-label="Cerrar">
            ✕
          </button>
        </div>
        {children}
      </div>
    </div>
  )
}

export function Notice({ kind, children }: { kind: 'error' | 'ok'; children: ReactNode }) {
  return (
    <div className={`notice notice--${kind}`} role={kind === 'error' ? 'alert' : 'status'}>
      {children}
    </div>
  )
}

export function Empty({ title, hint }: { title: string; hint: string }) {
  return (
    <div className="empty">
      <h3>{title}</h3>
      <p>{hint}</p>
    </div>
  )
}

export function Skeletons({ count = 4 }: { count?: number }) {
  return (
    <div aria-busy="true" aria-label="Cargando">
      {Array.from({ length: count }, (_, i) => (
        <div className="skeleton" key={i} />
      ))}
    </div>
  )
}

export function formatDate(iso: string | null | undefined): string {
  if (!iso) return '—'
  const fecha = new Date(iso)
  if (Number.isNaN(fecha.getTime())) return '—'
  return fecha.toLocaleDateString('es-DO', {
    day: '2-digit',
    month: 'short',
    year: 'numeric',
  })
}
