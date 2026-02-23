/**
 * weatherHelpers.js
 * Pure utility functions and constants for the Weather Dashboard.
 * No React imports — safe to use anywhere.
 */

// ─── Constants ───────────────────────────────────────────────────────────────

export const MONTHS = [
  "Jan", "Feb", "Mar", "Apr", "May", "Jun",
  "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
];

/**
 * X-axis tick stride.
 * Show one label every N data points so the axis doesn't crowd.
 * 5 = every 6th point (index 0, 6, 12…) which equals every 6 hours
 * for hourly data.
 */
export const X_TICK_INTERVAL = 5;

// ─── Date helpers ─────────────────────────────────────────────────────────────

/**
 * Convert a Date to the value string expected by <input type="datetime-local">.
 * Example output: "2025-12-03T14:00"
 *
 * @param   {Date}   date
 * @returns {string}
 */
export function toLocalDatetimeValue(date) {
  const pad = (n) => String(n).padStart(2, "0");
  return (
    `${date.getFullYear()}-` +
    `${pad(date.getMonth() + 1)}-` +
    `${pad(date.getDate())}T` +
    `${pad(date.getHours())}:00`
  );
}

/**
 * Full human-readable label stored on each chart data point.
 * Used exclusively by the custom tooltip.
 * Example: "Dec 03 14:00"
 *
 * @param   {string} isoTimestamp
 * @returns {string}
 */
export function formatFullLabel(isoTimestamp) {
  const d = new Date(isoTimestamp);
  return (
    `${MONTHS[d.getMonth()]} ` +
    `${String(d.getDate()).padStart(2, "0")} ` +
    `${String(d.getHours()).padStart(2, "0")}:00`
  );
}

/**
 * Compact X-axis tick label.
 *
 * Single-day range  → "14:00"
 * Multi-day range   → "Dec 03" at midnight boundaries, "14:00" for all other hours.
 *
 * @param   {string}  isoTimestamp
 * @param   {boolean} isMultiDay
 * @returns {string}
 */
export function formatAxisTick(isoTimestamp, isMultiDay) {
  const d = new Date(isoTimestamp);
  const hour = d.getHours();

  if (!isMultiDay) {
    return `${String(hour).padStart(2, "0")}:00`;
  }

  if (hour === 0) {
    return `${MONTHS[d.getMonth()]} ${String(d.getDate()).padStart(2, "0")}`;
  }

  return `${String(hour).padStart(2, "0")}:00`;
}

// ─── Data helpers ─────────────────────────────────────────────────────────────

/**
 * Returns true when the data array spans more than one calendar day.
 *
 * @param   {Array<{timestampUtc: string}>} dataPoints
 * @returns {boolean}
 */
export function isMultiDayRange(dataPoints) {
  if (!dataPoints || dataPoints.length < 2) return false;
  const first = new Date(dataPoints[0].timestampUtc);
  const last  = new Date(dataPoints[dataPoints.length - 1].timestampUtc);
  return first.toDateString() !== last.toDateString();
}

/**
 * Transform raw API response items into the shape Recharts expects.
 *
 * @param   {Array<{timestampUtc: string, temperature: number, windSpeed: number, precipitation: number}>} dataPoints
 * @returns {Array<{time: string, label: string, Temperature: number, "Wind Speed": number, Precipitation: number}>}
 */
export function toChartData(dataPoints) {
  return dataPoints.map((d) => ({
    time:          d.timestampUtc,
    label:         formatFullLabel(d.timestampUtc),
    Temperature:   parseFloat(d.temperature.toFixed(2)),
    "Wind Speed":  parseFloat(d.windSpeed.toFixed(2)),
    Precipitation: parseFloat(d.precipitation.toFixed(2)),
  }));
}

/**
 * Derive summary statistics from raw API data points.
 *
 * @param   {Array<{temperature: number, windSpeed: number, precipitation: number}>} dataPoints
 * @returns {{ maxTemp: number|null, minTemp: number|null, avgWind: number|null, totalPrecip: number|null }}
 */
export function computeStats(dataPoints) {
  if (!dataPoints.length) {
    return { maxTemp: null, minTemp: null, avgWind: null, totalPrecip: null };
  }

  const temps = dataPoints.map((d) => d.temperature);
  const winds = dataPoints.map((d) => d.windSpeed);

  return {
    maxTemp:    Math.max(...temps),
    minTemp:    Math.min(...temps),
    avgWind:    winds.reduce((a, b) => a + b, 0) / winds.length,
    totalPrecip: dataPoints.reduce((sum, d) => sum + d.precipitation, 0),
  };
}