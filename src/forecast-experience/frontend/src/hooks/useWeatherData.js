/**
 * useWeatherData.js
 * Fetches hourly forecast data from GET /api/v1/forecast/{cityId}/{from}/{to}.
 * Handles 204 No Content gracefully (empty result, no crash).
 */

import { useState, useCallback, useEffect } from "react";

/**
 * @param   {string}      backendUrl  Base URL, e.g. "http://localhost:83"
 * @param   {number|null} cityId      Currently selected city ID
 * @param   {string}      fromDt      datetime-local string, e.g. "2025-12-03T00:00"
 * @param   {string}      toDt        datetime-local string, e.g. "2025-12-03T23:00"
 * @returns {{
 *   data:      Array,
 *   loading:   boolean,
 *   error:     string|null,
 *   submitted: boolean,
 *   fetchData: () => Promise<void>,
 * }}
 */
export function useWeatherData(backendUrl, cityId, fromDt, toDt) {
  const [data, setData]           = useState([]);
  const [loading, setLoading]     = useState(false);
  const [error, setError]         = useState(null);
  const [submitted, setSubmitted] = useState(false);

  const fetchData = useCallback(async () => {
    if (cityId === null) return;

    setLoading(true);
    setError(null);
    setData([]);

    try {
      const from = new Date(fromDt).toISOString();
      const to   = new Date(toDt).toISOString();
      const url  = `${backendUrl}/api/v1/forecast/${cityId}/${from}/${to}`;

      const res = await fetch(url);

      if (res.status === 204) {
        // Backend found nothing for this range — valid empty result
        setData([]);
        setSubmitted(true);
        return;
      }

      if (!res.ok) throw new Error(`HTTP ${res.status}`);

      const json = await res.json();
      setData(json);
      setSubmitted(true);
    } catch (e) {
      setError(`Request failed — ${e.message}`);
      setSubmitted(true);
    } finally {
      setLoading(false);
    }
  }, [backendUrl, cityId, fromDt, toDt]);

  // Auto-fetch once a city becomes available after the cities load
  useEffect(() => {
    if (cityId !== null) fetchData();
    // fetchData is intentionally omitted: we only want this to fire
    // when cityId first becomes non-null, matching the original behaviour.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [cityId]);

  return { data, loading, error, submitted, fetchData };
}