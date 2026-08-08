import { NavLink, Outlet } from 'react-router-dom'

const SECCIONES = [
  { to: '/', label: 'Tablero', end: true },
  { to: '/reclamos', label: 'Reclamos', end: false },
  { to: '/clientes', label: 'Clientes', end: false },
]

export default function Layout() {
  return (
    <div className="shell">
      <aside className="rail">
        <div className="wordmark">
          <strong>
            Arqua<span>nix</span>
          </strong>
          <small>Control de reclamos</small>
        </div>

        <nav className="nav" aria-label="Secciones">
          {SECCIONES.map((s) => (
            <NavLink key={s.to} to={s.to} end={s.end} className="nav-link">
              {s.label}
            </NavLink>
          ))}
        </nav>
      </aside>

      <main className="main">
        <Outlet />
      </main>
    </div>
  )
}
