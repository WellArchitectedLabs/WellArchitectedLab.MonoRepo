/**
 * WeatherDashboard.jsx
 * Root component for the Weather Insights dashboard.
 *
 * Responsibilities:
 *  - Own the date-range picker state (fromDt, toDt)
 *  - Delegate city fetching to useCities()
 *  - Delegate data fetching to useWeatherData()
 *  - Derive chart data and summary stats from raw API response
 *  - Compose the UI from StatCard, ChartBlock sub-components
 */

import { useState } from "react";
import "./WeatherDashboard.css";

import { useCities }     from "../hooks/useCities";
import { useWeatherData } from "../hooks/useWeatherData";
import { StatCard }      from "./StatCard";
import { ChartBlock }    from "./ChartBlock";
import {
  toLocalDatetimeValue,
  toChartData,
  computeStats,
  isMultiDayRange,
} from "../utils/weatherHelpers";

// ─── Component ────────────────────────────────────────────────────────────────

/**
 * @param {string} backendUrl  Base URL of the backend API, e.g. "http://localhost:83"
 */
export default function WeatherDashboard({ backendUrl }) {
  // ── Initialise date range to today (midnight → 23:00) ───────────────────
  const now = new Date();

  const dayStart = new Date(now);
  dayStart.setHours(0, 0, 0, 0);

  const dayEnd = new Date(now);
  dayEnd.setHours(23, 0, 0, 0);

  const [fromDt, setFromDt] = useState(toLocalDatetimeValue(dayStart));
  const [toDt,   setToDt  ] = useState(toLocalDatetimeValue(dayEnd));

  // ── Theme ────────────────────────────────────────────────────────────────
  const [isLight, setIsLight] = useState(false);
  const toggleTheme = () => setIsLight((prev) => !prev);

  // ── Remote data ──────────────────────────────────────────────────────────
  const { cities, citiesLoading, cityId, setCityId } = useCities(backendUrl);

  const { data, loading, error, submitted, fetchData } =
    useWeatherData(backendUrl, cityId, fromDt, toDt);

  // ── Derived values ───────────────────────────────────────────────────────
  const cityName  = cities.find((c) => c.id === Number(cityId))?.name ?? "";
  const chartData = toChartData(data);
  const multiDay  = isMultiDayRange(data);
  const stats     = computeStats(data);

  const isSubmitDisabled = loading || citiesLoading || cityId === null;

  // ── Render ───────────────────────────────────────────────────────────────
  return (
    <div className={`wd-layout${isLight ? " light" : ""}`}>

      {/* Decorative background rings (purely cosmetic) */}
      <div className="wd-bg-ring wd-bg-ring--sm" />
      <div className="wd-bg-ring wd-bg-ring--lg" />

      {/* ── Header ── */}
      <header className="wd-header fade-up">
        <div className="wd-header__eyebrow">
          <div className="wd-header__dot" />
          <span className="wd-header__platform">Weather Forecast Platform</span>

          {/* Theme toggle */}
          <button
            className="wd-theme-toggle"
            onClick={toggleTheme}
            aria-label={isLight ? "Switch to dark theme" : "Switch to light theme"}
          >
            <span className="wd-theme-toggle__icon">{isLight ? "☀️" : "🌙"}</span>
            <div className="wd-theme-toggle__track">
              <div className="wd-theme-toggle__knob" />
            </div>
            <span className="wd-theme-toggle__label">{isLight ? "Light" : "Dark"}</span>
          </button>
        </div>
        <h1 className="wd-header__title">
          Climate{" "}
          <span className="wd-header__title-accent">Insights</span>
        </h1>
        <p className="wd-header__subtitle">
          Hourly temperature, wind &amp; precipitation analysis
        </p>
      </header>

      {/* ── Control panel ── */}
      <section className="wd-controls-wrapper">
        <div className="wd-controls-panel fade-up">
          <div className="wd-controls-row">

            {/* City selector */}
            <div className="wd-field wd-field--wide">
              <label className="wd-label">City</label>
              <div className="wd-select-wrapper">
                <select
                  className="wd-select"
                  value={cityId ?? ""}
                  onChange={(e) => setCityId(Number(e.target.value))}
                  disabled={citiesLoading}
                >
                  {citiesLoading ? (
                    <option>Loading cities…</option>
                  ) : (
                    cities.map((c) => (
                      <option key={c.id} value={c.id}>{c.name}</option>
                    ))
                  )}
                </select>
              </div>
            </div>

            {/* From date */}
            <div className="wd-field">
              <label className="wd-label">From</label>
              <input
                type="datetime-local"
                className="wd-input"
                value={fromDt}
                onChange={(e) => setFromDt(e.target.value)}
              />
            </div>

            {/* To date */}
            <div className="wd-field">
              <label className="wd-label">To</label>
              <input
                type="datetime-local"
                className="wd-input"
                value={toDt}
                onChange={(e) => setToDt(e.target.value)}
              />
            </div>

            {/* Submit */}
            <div className="wd-field wd-field--btn">
              <button
                className="wd-btn"
                onClick={fetchData}
                disabled={isSubmitDisabled}
              >
                {loading
                  ? <span className="loading-dot">Fetching…</span>
                  : "Analyse →"
                }
              </button>
            </div>

          </div>
        </div>

        {/* Error notice (city load errors OR fetch errors) */}
        {error && (
          <p className="wd-error-notice">⚠ {error}</p>
        )}
      </section>

      {/* ── Divider ── */}
      {submitted && (
        <div className="wd-divider">
          <div className="wd-divider__line wd-divider__line--left" />
          <span className="wd-divider__label">
            {cityName} · {data.length > 0 ? `${data.length} data points` : "no data"}
          </span>
          <div className="wd-divider__line wd-divider__line--right" />
        </div>
      )}

      {/* ── Stats + charts ── */}
      {submitted && (
        <div className="wd-results fade-up">

          {/* Stat cards — only rendered when there is data */}
          {data.length > 0 && (
            <div className="wd-stat-grid">
              <StatCard label="Max Temp"     value={stats.maxTemp}     unit="°C"  accent="#f6ad55" />
              <StatCard label="Min Temp"     value={stats.minTemp}     unit="°C"  accent="#63b3ed" />
              <StatCard label="Avg Wind"     value={stats.avgWind}     unit="m/s" accent="#68d391" />
              <StatCard label="Total Precip." value={stats.totalPrecip} unit="mm" accent="#b794f4" />
            </div>
          )}

          <ChartBlock
            title="Temperature"
            accent="#f6ad55"
            accentEnd="#ed8936"
            dataKey="Temperature"
            unit="°C"
            chartData={chartData}
            yDomain={["auto", "auto"]}
            empty={data.length === 0}
            multiDay={multiDay}
          />

          <ChartBlock
            title="Wind Speed"
            accent="#68d391"
            accentEnd="#38a169"
            dataKey="Wind Speed"
            unit="m/s"
            chartData={chartData}
            yDomain={[0, "auto"]}
            empty={data.length === 0}
            multiDay={multiDay}
            style={{ marginTop: 20 }}
          />

          <ChartBlock
            title="Precipitation"
            accent="#b794f4"
            accentEnd="#805ad5"
            dataKey="Precipitation"
            unit="mm"
            chartData={chartData}
            yDomain={[0, "auto"]}
            empty={data.length === 0}
            multiDay={multiDay}
            style={{ marginTop: 20 }}
          />

        </div>
      )}

      {/* ── Initial empty state (shown before first Analyse click) ── */}
      {!submitted && !loading && (
        <div className="wd-empty-state">
          <span className="wd-empty-state__icon">🌐</span>
          <p className="wd-empty-state__text">
            Select a city and time range, then click Analyse
          </p>
        </div>
      )}

    </div>
  );
}