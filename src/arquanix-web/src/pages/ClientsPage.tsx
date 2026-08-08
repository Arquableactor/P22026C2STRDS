import { useState } from 'react'
import { ApiError, api } from '../api/client'
import { useAsync } from '../hooks/useAsync'
import type { Client, ClientInput } from '../api/types'
import { Empty, Modal, Notice, Skeletons, formatDate } from '../components/ui'

const VACIO: ClientInput = { name: '', email: '', phone: '', isActive: true }

export default function ClientsPage() {
  const [busqueda, setBusqueda] = useState('')
  const [soloActivos, setSoloActivos] = useState(false)
  const [editando, setEditando] = useState<Client | null>(null)
  const [creando, setCreando] = useState(false)
  const [aviso, setAviso] = useState<{ kind: 'ok' | 'error'; texto: string } | null>(null)

  const clientes = useAsync(
    () => api.listClients(busqueda ? { busqueda } : soloActivos ? { activos: true } : {}),
    [busqueda, soloActivos],
  )

  const cerrarFormulario = () => {
    setCreando(false)
    setEditando(null)
  }

  const notificar = (kind: 'ok' | 'error', texto: string) => {
    setAviso({ kind, texto })
    window.setTimeout(() => setAviso(null), 4500)
  }

  const trasGuardar = (texto: string) => {
    cerrarFormulario()
    notificar('ok', texto)
    clientes.recargar()
  }

  const eliminar = async (c: Client) => {
    if (!window.confirm(`¿Eliminar a ${c.name}? Se perderá su historial de reclamos cerrados.`)) {
      return
    }
    try {
      await api.deleteClient(c.id)
      notificar('ok', `${c.name} fue eliminado.`)
      clientes.recargar()
    } catch (err) {
      notificar(
        'error',
        err instanceof ApiError ? err.message : 'No se pudo eliminar el cliente.',
      )
    }
  }

  return (
    <>
      <div className="page-head">
        <div>
          <p className="eyebrow">Maestro</p>
          <h1>Clientes</h1>
          <p>Registro de las personas y empresas que pueden levantar reclamos.</p>
        </div>
        <button type="button" className="btn btn--primary" onClick={() => setCreando(true)}>
          Agregar cliente
        </button>
      </div>

      {aviso && <Notice kind={aviso.kind}>{aviso.texto}</Notice>}

      <div className="toolbar">
        <div className="field field-search" style={{ marginBottom: 0 }}>
          <input
            type="search"
            value={busqueda}
            placeholder="Buscar por nombre o correo"
            aria-label="Buscar cliente"
            onChange={(e) => setBusqueda(e.target.value)}
          />
        </div>
        <button
          type="button"
          className="chip-toggle"
          aria-pressed={soloActivos}
          onClick={() => setSoloActivos((v) => !v)}
          disabled={busqueda.length > 0}
        >
          Solo activos
        </button>
      </div>

      {clientes.loading && <Skeletons count={4} />}
      {clientes.error && <Notice kind="error">{clientes.error}</Notice>}

      {clientes.data && clientes.data.length === 0 && (
        <Empty
          title="Sin coincidencias"
          hint="Ajuste la búsqueda o registre un cliente nuevo para empezar."
        />
      )}

      {clientes.data && clientes.data.length > 0 && (
        <div className="board">
          <div className="board-head">
            <span>ID</span>
            <span>Cliente</span>
            <span>Teléfono</span>
            <span>Estado</span>
            <span>Alta</span>
            <span />
          </div>

          {clientes.data.map((c) => (
            <div className="board-row" key={c.id}>
              <span className="cell-id">#{String(c.id).padStart(3, '0')}</span>
              <div className="cell-title">
                <strong>{c.name}</strong>
                <span>{c.email}</span>
              </div>
              <span className="cell-id">{c.phone || '—'}</span>
              <span className="tag">
                <i
                  style={{ background: c.isActive ? 'var(--resolved)' : 'var(--closed)' }}
                  aria-hidden="true"
                />
                {c.isActive ? 'Activo' : 'Inactivo'}
              </span>
              <span className="cell-id">{formatDate(c.createdAt)}</span>
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
          title={editando ? `Editar ${editando.name}` : 'Agregar cliente'}
          onClose={cerrarFormulario}
        >
          <ClientForm
            inicial={
              editando
                ? {
                    name: editando.name,
                    email: editando.email,
                    phone: editando.phone ?? '',
                    isActive: editando.isActive,
                  }
                : VACIO
            }
            onCancel={cerrarFormulario}
            onSubmit={async (valores) => {
              if (editando) {
                await api.updateClient(editando.id, valores)
                trasGuardar(`${valores.name} fue actualizado.`)
              } else {
                const creado = await api.createClient(valores)
                trasGuardar(`${creado.name} fue registrado.`)
              }
            }}
          />
        </Modal>
      )}
    </>
  )
}

function ClientForm({
  inicial,
  onSubmit,
  onCancel,
}: {
  inicial: ClientInput
  onSubmit: (valores: ClientInput) => Promise<void>
  onCancel: () => void
}) {
  const [valores, setValores] = useState<ClientInput>(inicial)
  const [errores, setErrores] = useState<Record<string, string[]>>({})
  const [general, setGeneral] = useState<string | null>(null)
  const [enviando, setEnviando] = useState(false)

  const cambiar = <K extends keyof ClientInput>(campo: K, valor: ClientInput[K]) =>
    setValores((prev) => ({ ...prev, [campo]: valor }))

  const errorDe = (campo: string) =>
    errores[campo]?.[0] ?? errores[campo.charAt(0).toUpperCase() + campo.slice(1)]?.[0]

  const enviar = async () => {
    setEnviando(true)
    setGeneral(null)
    setErrores({})
    try {
      await onSubmit({ ...valores, phone: valores.phone?.trim() || null })
    } catch (err) {
      if (err instanceof ApiError) {
        setErrores(err.fieldErrors)
        setGeneral(err.message)
      } else {
        setGeneral('No se pudo guardar el cliente.')
      }
      setEnviando(false)
    }
  }

  return (
    <div>
      {general && <Notice kind="error">{general}</Notice>}

      <div className="field">
        <label htmlFor="name">Nombre</label>
        <input
          id="name"
          value={valores.name}
          maxLength={100}
          placeholder="Ej. Ferretería El Progreso"
          onChange={(e) => cambiar('name', e.target.value)}
        />
        {errorDe('name') && <span className="field-error">{errorDe('name')}</span>}
      </div>

      <div className="field-row">
        <div className="field">
          <label htmlFor="email">Correo</label>
          <input
            id="email"
            type="email"
            value={valores.email}
            placeholder="contacto@correo.do"
            onChange={(e) => cambiar('email', e.target.value)}
          />
          {errorDe('email') && <span className="field-error">{errorDe('email')}</span>}
        </div>

        <div className="field">
          <label htmlFor="phone">Teléfono</label>
          <input
            id="phone"
            value={valores.phone ?? ''}
            placeholder="809-555-0100"
            onChange={(e) => cambiar('phone', e.target.value)}
          />
          {errorDe('phone') && <span className="field-error">{errorDe('phone')}</span>}
        </div>
      </div>

      <label className="switch">
        <input
          type="checkbox"
          checked={valores.isActive}
          onChange={(e) => cambiar('isActive', e.target.checked)}
        />
        Cliente activo: puede levantar reclamos nuevos
      </label>

      <div className="modal-foot">
        <button type="button" className="btn" onClick={onCancel} disabled={enviando}>
          Cancelar
        </button>
        <button type="button" className="btn btn--primary" onClick={enviar} disabled={enviando}>
          {enviando ? 'Guardando…' : 'Guardar cliente'}
        </button>
      </div>
    </div>
  )
}
