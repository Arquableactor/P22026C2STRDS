import { useState } from 'react'
import { ApiError, api } from '../api/client'
import { useAsync } from '../hooks/useAsync'
import {
  CLAIM_PRIORITIES,
  CLAIM_STATUSES,
  PRIORITY_LABEL,
  STATUS_LABEL,
  type Claim,
  type ClaimInput,
  type ClaimStatus,
} from '../api/types'
import {
  AgingGauge,
  Empty,
  Modal,
  Notice,
  PriorityTag,
  Skeletons,
  StatusTag,
  formatDate,
} from '../components/ui'

const VACIO: ClaimInput = {
  clientId: 0,
  title: '',
  description: '',
  status: 'Open',
  priority: 'Medium',
}

export default function ClaimsPage() {
  const [filtro, setFiltro] = useState<ClaimStatus | 'Todos'>('Todos')
  const [editando, setEditando] = useState<Claim | null>(null)
  const [creando, setCreando] = useState(false)
  const [aviso, setAviso] = useState<string | null>(null)

  const reclamos = useAsync(
    () => api.listClaims(filtro === 'Todos' ? {} : { status: filtro }),
    [filtro],
  )
  const clientes = useAsync(() => api.listClients(), [])

  const cerrarFormulario = () => {
    setCreando(false)
    setEditando(null)
  }

  const trasGuardar = (mensaje: string) => {
    cerrarFormulario()
    setAviso(mensaje)
    reclamos.recargar()
    window.setTimeout(() => setAviso(null), 4000)
  }

  const eliminar = async (c: Claim) => {
    if (!window.confirm(`¿Eliminar el reclamo #${c.id} "${c.title}"? Esta acción no se revierte.`)) {
      return
    }
    try {
      await api.deleteClaim(c.id)
      trasGuardar(`Reclamo #${c.id} eliminado.`)
    } catch (err) {
      setAviso(err instanceof ApiError ? err.message : 'No se pudo eliminar el reclamo.')
    }
  }

  return (
    <>
      <div className="page-head">
        <div>
          <p className="eyebrow">Operación</p>
          <h1>Cola de reclamos</h1>
          <p>Registro, seguimiento y cierre de los casos levantados por los clientes.</p>
        </div>
        <button
          type="button"
          className="btn btn--primary"
          onClick={() => setCreando(true)}
          disabled={!clientes.data || clientes.data.length === 0}
        >
          Registrar reclamo
        </button>
      </div>

      {aviso && <Notice kind="ok">{aviso}</Notice>}

      <div className="toolbar">
        <div className="chipset">
          {(['Todos', ...CLAIM_STATUSES] as const).map((op) => (
            <button
              key={op}
              type="button"
              className="chip-toggle"
              aria-pressed={filtro === op}
              onClick={() => setFiltro(op)}
            >
              {op === 'Todos' ? 'Todos' : STATUS_LABEL[op]}
            </button>
          ))}
        </div>
      </div>

      {reclamos.loading && <Skeletons count={5} />}
      {reclamos.error && <Notice kind="error">{reclamos.error}</Notice>}

      {reclamos.data && reclamos.data.length === 0 && (
        <Empty
          title="Sin reclamos en este filtro"
          hint="Cambie el filtro de estado o registre un reclamo nuevo."
        />
      )}

      {reclamos.data && reclamos.data.length > 0 && (
        <div className="board">
          <div className="board-head">
            <span>ID</span>
            <span>Reclamo / Cliente</span>
            <span>Prioridad</span>
            <span>Estado</span>
            <span>Antigüedad</span>
            <span />
          </div>

          {reclamos.data.map((c) => (
            <div className="board-row" key={c.id}>
              <span className="cell-id">#{String(c.id).padStart(3, '0')}</span>
              <div className="cell-title">
                <strong>{c.title}</strong>
                <span>
                  {c.clientName ?? `Cliente ${c.clientId}`} · abierto el {formatDate(c.createdAt)}
                </span>
              </div>
              <PriorityTag priority={c.priority} />
              <StatusTag status={c.status} />
              <AgingGauge dias={c.diasDeAtencion} cerrado={Boolean(c.closedAt)} />
              <div className="row-actions">
                <button type="button" className="btn btn--ghost" onClick={() => setEditando(c)}>
                  Editar
                </button>
                <button
                  type="button"
                  className="btn btn--ghost btn--danger"
                  onClick={() => eliminar(c)}
                >
                  Borrar
                </button>
              </div>
            </div>
          ))}
        </div>
      )}

      {(creando || editando) && (
        <Modal
          title={editando ? `Reclamo #${editando.id}` : 'Registrar reclamo'}
          onClose={cerrarFormulario}
        >
          <ClaimForm
            inicial={
              editando
                ? {
                    clientId: editando.clientId,
                    title: editando.title,
                    description: editando.description,
                    status: editando.status,
                    priority: editando.priority,
                  }
                : { ...VACIO, clientId: clientes.data?.[0]?.id ?? 0 }
            }
            clientes={clientes.data ?? []}
            onCancel={cerrarFormulario}
            onSubmit={async (valores) => {
              if (editando) {
                await api.updateClaim(editando.id, valores)
                trasGuardar(`Reclamo #${editando.id} actualizado.`)
              } else {
                const creado = await api.createClaim(valores)
                trasGuardar(`Reclamo #${creado.id} registrado.`)
              }
            }}
          />
        </Modal>
      )}
    </>
  )
}

function ClaimForm({
  inicial,
  clientes,
  onSubmit,
  onCancel,
}: {
  inicial: ClaimInput
  clientes: { id: number; name: string }[]
  onSubmit: (valores: ClaimInput) => Promise<void>
  onCancel: () => void
}) {
  const [valores, setValores] = useState<ClaimInput>(inicial)
  const [errores, setErrores] = useState<Record<string, string[]>>({})
  const [general, setGeneral] = useState<string | null>(null)
  const [enviando, setEnviando] = useState(false)

  const cambiar = <K extends keyof ClaimInput>(campo: K, valor: ClaimInput[K]) =>
    setValores((prev) => ({ ...prev, [campo]: valor }))

  const enviar = async () => {
    setEnviando(true)
    setGeneral(null)
    setErrores({})
    try {
      await onSubmit(valores)
    } catch (err) {
      if (err instanceof ApiError) {
        setErrores(err.fieldErrors)
        setGeneral(err.message)
      } else {
        setGeneral('No se pudo guardar el reclamo.')
      }
      setEnviando(false)
    }
  }

  const errorDe = (campo: string) =>
    errores[campo]?.[0] ?? errores[campo.charAt(0).toUpperCase() + campo.slice(1)]?.[0]

  return (
    <div>
      {general && <Notice kind="error">{general}</Notice>}

      <div className="field">
        <label htmlFor="clientId">Cliente</label>
        <select
          id="clientId"
          value={valores.clientId}
          onChange={(e) => cambiar('clientId', Number(e.target.value))}
        >
          {clientes.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>
        {errorDe('clientId') && <span className="field-error">{errorDe('clientId')}</span>}
      </div>

      <div className="field">
        <label htmlFor="title">Asunto</label>
        <input
          id="title"
          value={valores.title}
          maxLength={120}
          placeholder="Ej. Sin servicio de internet desde ayer"
          onChange={(e) => cambiar('title', e.target.value)}
        />
        {errorDe('title') && <span className="field-error">{errorDe('title')}</span>}
      </div>

      <div className="field">
        <label htmlFor="description">Detalle</label>
        <textarea
          id="description"
          value={valores.description}
          maxLength={1000}
          placeholder="Describa lo reportado por el cliente y lo verificado hasta ahora."
          onChange={(e) => cambiar('description', e.target.value)}
        />
        {errorDe('description') && <span className="field-error">{errorDe('description')}</span>}
      </div>

      <div className="field-row">
        <div className="field">
          <label htmlFor="priority">Prioridad</label>
          <select
            id="priority"
            value={valores.priority}
            onChange={(e) => cambiar('priority', e.target.value as ClaimInput['priority'])}
          >
            {CLAIM_PRIORITIES.map((p) => (
              <option key={p} value={p}>
                {PRIORITY_LABEL[p]}
              </option>
            ))}
          </select>
        </div>

        <div className="field">
          <label htmlFor="status">Estado</label>
          <select
            id="status"
            value={valores.status}
            onChange={(e) => cambiar('status', e.target.value as ClaimStatus)}
          >
            {CLAIM_STATUSES.map((s) => (
              <option key={s} value={s}>
                {STATUS_LABEL[s]}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div className="modal-foot">
        <button type="button" className="btn" onClick={onCancel} disabled={enviando}>
          Cancelar
        </button>
        <button type="button" className="btn btn--primary" onClick={enviar} disabled={enviando}>
          {enviando ? 'Guardando…' : 'Guardar reclamo'}
        </button>
      </div>
    </div>
  )
}
