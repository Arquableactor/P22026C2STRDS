import { useCallback, useEffect, useState } from 'react'
import { ApiError } from '../api/client'

interface Estado<T> {
  data: T | null
  loading: boolean
  error: string | null
}

/**
 * Ejecuta una consulta contra la API y expone su estado.
 * `deps` fuerza la recarga cuando cambian los filtros.
 */
export function useAsync<T>(fn: () => Promise<T>, deps: unknown[] = []) {
  const [estado, setEstado] = useState<Estado<T>>({ data: null, loading: true, error: null })

  // La función se re-crea con cada cambio de filtro; deps controla ese ciclo.
  // eslint-disable-next-line react-hooks/exhaustive-deps
  const consulta = useCallback(fn, deps)

  const recargar = useCallback(() => {
    let vigente = true
    setEstado((prev) => ({ ...prev, loading: true, error: null }))

    consulta()
      .then((data) => {
        if (vigente) setEstado({ data, loading: false, error: null })
      })
      .catch((err: unknown) => {
        if (!vigente) return
        const mensaje =
          err instanceof ApiError ? err.message : 'Ocurrió un error inesperado al consultar la API.'
        setEstado({ data: null, loading: false, error: mensaje })
      })

    return () => {
      vigente = false
    }
  }, [consulta])

  useEffect(() => recargar(), [recargar])

  return { ...estado, recargar }
}
