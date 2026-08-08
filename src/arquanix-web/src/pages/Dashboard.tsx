import { Link } from 'react-router-dom'
import { api } from '../api/client'
import { useAsync } from '../hooks/useAsync'
import { STATUS_LABEL, type ClaimStatus } from '../api/types'
import {
  AgingGauge,
  Notice,
  PriorityTag,
  Skeletons,
  STATUS_COLOR,
  StatusTag,
} from '../components/ui'

export default function Dashboard() {
  const stats = useAsync(() => api.stats(), [])
  const abiertos = useAsync(() => api.listClaims({ status: 'Open' }), [])

  if (stats.error) {
    return (
      <>
        <Encabezado />
        <Notice kind="error">{stats.error}</Notice>
        <button type="button" className="btn" onClick={stats.recargar}>
          Reintentar
        </button>
      </>
    )
  }

  if (stats.loading || !stats.data) {
    return (
      <>
        <Encabezado />
        <Skeletons count={3} />
      </>
    )
  }

  const s = stats.data
  const distribucion: { estado: ClaimStatus; valor: number }[] = [
    { estado: 'Open', valor: s.open },
    { estado: 'InProgress', valor: s.inProgress },
    { estado: 'Resolved', valor: s.resolved },
    { estado: 'Closed', valor: s.closed },
  ]
  const mayor = Math.max(1, ...distribucion.map((d) => d.valor))
  const pendientes = s.open + s.inProgress

  return (
    <>
      <Encabezado />

      <dl className="readouts">
        <Readout etiqueta="Reclamos pendientes" valor={pendientes} nota="sin resolver" />
        <Readout etiqueta="Críticos vigentes" valor={s.critical} alerta={s.critical > 0} />
        <Readout etiqueta="Días promedio de cierre" valor={s.promedioDiasAtencion} nota="días" />
        <Readout
          etiqueta="Clientes activos"
          valor={s.clientesActivos}
          nota={`de ${s.totalClientes}`}
        />
      </dl>

      <div className="split">
        <section className="panel">
          <h2>Distribución por estado</h2>
          <div className="bars">
            {distribucion.map((d) => (
              <div className="bar-row" key={d.estado}>
                <span>{STATUS_LABEL[d.estado]}</span>
                <div className="bar-track">
                  <div
                    className="bar-fill"
                    style={{
                      width: `${(d.valor / mayor) * 100}%`,
                      background: STATUS_COLOR[d.estado],
                    }}
                  />
                </div>
                <b>{d.valor}</b>
              </div>
            ))}
          </div>
        </section>

        <section className="panel">
          <h2>Carga del período</h2>
          <p style={{ color: 'var(--text-dim)', marginTop: 0, fontSize: 14 }}>
            De {s.total} reclamos registrados, {pendientes} siguen en la cola de atención. El
            equipo cierra un caso en {s.promedioDiasAtencion} días en promedio.
          </p>
          <Link to="/reclamos" className="btn btn--primary" style={{ display: 'inline-block' }}>
            Ir a la cola de reclamos
          </Link>
        </section>
      </div>

      <section>
        <p className="eyebrow">Cola abierta</p>
        <h2 style={{ fontSize: 19, marginBottom: 14 }}>Reclamos sin asignar seguimiento</h2>

        {abiertos.loading && <Skeletons count={3} />}
        {abiertos.error && <Notice kind="error">{abiertos.error}</Notice>}

        {abiertos.data && abiertos.data.length === 0 && (
          <div className="empty">
            <h3>Cola vacía</h3>
            <p>No hay reclamos abiertos en este momento.</p>
          </div>
        )}

        {abiertos.data && abiertos.data.length > 0 && (
          <div className="board">
            <div className="board-head">
              <span>ID</span>
              <span>Reclamo</span>
              <span>Prioridad</span>
              <span>Estado</span>
              <span>Antigüedad</span>
              <span />
            </div>
            {abiertos.data.slice(0, 6).map((c) => (
              <div className="board-row" key={c.id}>
                <span className="cell-id">#{String(c.id).padStart(3, '0')}</span>
                <div className="cell-title">
                  <strong>{c.title}</strong>
                  <span>{c.clientName ?? `Cliente ${c.clientId}`}</span>
                </div>
                <PriorityTag priority={c.priority} />
                <StatusTag status={c.status} />
                <AgingGauge dias={c.diasDeAtencion} cerrado={false} />
                <span />
              </div>
            ))}
          </div>
        )}
      </section>
    </>
  )
}

function Encabezado() {
  return (
    <div className="page-head">
      <div>
        <p className="eyebrow">Turno actual</p>
        <h1>Tablero de operación</h1>
        <p>
          Estado de la cola de soporte en tiempo real. Los datos provienen del servicio ArquanixApi.
        </p>
      </div>
    </div>
  )
}

function Readout({
  etiqueta,
  valor,
  nota,
  alerta = false,
}: {
  etiqueta: string
  valor: number
  nota?: string
  alerta?: boolean
}) {
  return (
    <div className={alerta ? 'readout readout--alert' : 'readout'}>
      <dt>{etiqueta}</dt>
      <dd>
        {valor}
        {nota && <small>{nota}</small>}
      </dd>
    </div>
  )
}
