/**
 * useCities.js
 * Fetches the city list from GET /api/v1/city and auto-selects the first entry.
 */

import { useState, useEffect } from "react";

/**
 * @param   {string} backendUrl  Base URL, e.g. "http://localhost:83"
 * @returns {{
 *   cities:        Array<{id: number, name: string}>,
 *   citiesLoading: boolean,
 *   citiesError:   string|null,
 *   cityId:        number|null,
 *   setCityId:     (id: number) => void,
 * }}
 */
export function useCities(backendUrl) {
  const [cities, setCities]               = useState([]);
  const [citiesLoading, setCitiesLoading] = useState(true);
  const [citiesError, setCitiesError]     = useState(null);
  const [cityId, setCityId]               = useState(null);

  useEffect(() => {
    let cancelled = false;

    (async () => {
      try {
        const res = await fetch(`${backendUrl}/api/v1/city`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const json = await res.json();
        if (!cancelled) {
          setCities(json);
          if (json.length > 0) setCityId(json[0].id);
        }
      } catch {
        if (!cancelled) setCitiesError("Could not load cities from backend.");
      } finally {
        if (!cancelled) setCitiesLoading(false);
      }
    })();

    return () => { cancelled = true; };
  }, [backendUrl]);

  return { cities, citiesLoading, citiesError, cityId, setCityId };
}