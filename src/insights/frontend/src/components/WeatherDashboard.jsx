import { useState, useEffect, useCallback } from "react";
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
} from "recharts";

// Cities are fetched dynamically from GET /city

// ─── Helpers ────────────────────────────────────────────────────────────────
function toLocalDatetimeValue(date) {
  const pad = (n) => String(n).padStart(2, "0");
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:00`;
}

const MONTHS = ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"];

// Full label stored on each data point (used by tooltip)
function formatFull(ts) {
  const d = new Date(ts);
  return `${MONTHS[d.getMonth()]} ${String(d.getDate()).padStart(2,"0")} ${String(d.getHours()).padStart(2,"0")}:00`;
}

// Tick label: show "Dec 03" only at midnight (hour 0), otherwise just "14:00"
// This creates natural day boundaries on the X-axis without crowding.
function formatTick(ts, multiDay) {
  const d = new Date(ts);
  const h = d.getHours();
  if (!multiDay) return `${String(h).padStart(2,"0")}:00`;
  if (h === 0) return `${MONTHS[d.getMonth()]} ${String(d.getDate()).padStart(2,"0")}`;
  return `${String(h).padStart(2,"0")}:00`;
}

// ─── Custom Tooltip ──────────────────────────────────────────────────────────
const CustomTooltip = ({ active, payload }) => {
  if (!active || !payload?.length) return null;
  const friendlyLabel = payload[0]?.payload?.label ?? "";
  return (
    <div style={{
      background: "rgba(10, 18, 35, 0.95)",
      border: "1px solid rgba(99,179,237,0.25)",
      borderRadius: 12,
      padding: "12px 18px",
      backdropFilter: "blur(12px)",
      boxShadow: "0 8px 32px rgba(0,0,0,0.4)",
    }}>
      <p style={{ color: "#63b3ed", fontFamily: "'DM Mono', monospace", fontSize: 12, marginBottom: 8, letterSpacing: 1 }}>
        {friendlyLabel}
      </p>
      {payload.map((entry) => (
        <p key={entry.name} style={{
          color: entry.color,
          fontFamily: "'DM Mono', monospace",
          fontSize: 13,
          margin: "3px 0",
        }}>
          <span style={{ opacity: 0.7 }}>{entry.name}: </span>
          <strong>{typeof entry.value === "number" ? entry.value.toFixed(2) : entry.value}</strong>
          <span style={{ opacity: 0.5, marginLeft: 4 }}>
            {entry.name === "Temperature" ? "°C" : entry.name === "Wind Speed" ? " m/s" : " mm"}
          </span>
        </p>
      ))}
    </div>
  );
};

// ─── Stat Card ───────────────────────────────────────────────────────────────
const StatCard = ({ label, value, unit, accent }) => (
  <div style={{
    background: "rgba(255,255,255,0.03)",
    border: `1px solid ${accent}30`,
    borderRadius: 16,
    padding: "20px 24px",
    flex: 1,
    minWidth: 140,
    position: "relative",
    overflow: "hidden",
  }}>
    <div style={{
      position: "absolute", top: 0, left: 0, right: 0, height: 2,
      background: `linear-gradient(90deg, transparent, ${accent}, transparent)`,
    }} />
    <p style={{ color: "#718096", fontFamily: "'DM Mono', monospace", fontSize: 11, letterSpacing: 2, textTransform: "uppercase", marginBottom: 8 }}>
      {label}
    </p>
    <p style={{ color: "#f7fafc", fontFamily: "'Syne', sans-serif", fontSize: 28, fontWeight: 800, margin: 0, lineHeight: 1 }}>
      {value !== null ? Number(value).toFixed(1) : "—"}
      <span style={{ fontSize: 14, color: accent, marginLeft: 4, fontWeight: 400 }}>{unit}</span>
    </p>
  </div>
);

// ─── Main Component ──────────────────────────────────────────────────────────
export default function WeatherDashboard({ backendUrl }) {
  const now = new Date();
  const dayStart = new Date(now);
  dayStart.setHours(0, 0, 0, 0);
  const dayEnd = new Date(now);
  dayEnd.setHours(23, 0, 0, 0);

  const [cities, setCities] = useState([]);
  const [citiesLoading, setCitiesLoading] = useState(true);
  const [cityId, setCityId] = useState(null);
  const [fromDt, setFromDt] = useState(toLocalDatetimeValue(dayStart));
  const [toDt, setToDt] = useState(toLocalDatetimeValue(dayEnd));
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [submitted, setSubmitted] = useState(false);

  // Fetch cities from backend on mount
  useEffect(() => {
    (async () => {
      try {
        const res = await fetch(`${backendUrl}/city`);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const json = await res.json();
        setCities(json);
        if (json.length > 0) setCityId(json[0].id);
      } catch {
        setError("Could not load cities from backend.");
      } finally {
        setCitiesLoading(false);
      }
    })();
  }, []);

  const cityName = cities.find((c) => c.id === Number(cityId))?.name ?? "";

  const fetchData = useCallback(async () => {
    setLoading(true);
    setError(null);
    setData([]);
    try {
      const from = new Date(fromDt).toISOString();
      const to = new Date(toDt).toISOString();
      const res = await fetch(`${backendUrl}/insight/${cityId}/${from}/${to}`);
      if (res.status === 204) {
        // No content — backend found nothing for this range
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
  }, [cityId, fromDt, toDt]);

  // Auto-load weather data once cities are loaded and a city is selected
  useEffect(() => {
    if (cityId !== null) fetchData();
  }, [cityId]);

  // Detect whether the range spans more than one calendar day
  const multiDay = data.length > 0 && (() => {
    const first = new Date(data[0].timestampUtc);
    const last  = new Date(data[data.length - 1].timestampUtc);
    return first.toDateString() !== last.toDateString();
  })();

  const chartData = data.map((d) => ({
    time: d.timestampUtc,           // raw ISO — used as dataKey for XAxis
    label: formatFull(d.timestampUtc), // shown in tooltip
    Temperature: parseFloat(d.temperature.toFixed(2)),
    "Wind Speed": parseFloat(d.windSpeed.toFixed(2)),
    Precipitation: parseFloat(d.precipitation.toFixed(2)),
  }));

  const temps = data.map((d) => d.temperature);
  const winds = data.map((d) => d.windSpeed);
  const maxTemp = temps.length ? Math.max(...temps) : null;
  const minTemp = temps.length ? Math.min(...temps) : null;
  const avgWind = winds.length ? winds.reduce((a, b) => a + b, 0) / winds.length : null;
  const totalPrecip = data.length ? data.reduce((a, b) => a + b.precipitation, 0) : null;

  return (
    <>
      {/* ── Google Fonts ── */}
      <style>{`
        @import url('https://fonts.googleapis.com/css2?family=Syne:wght@400;600;700;800&family=DM+Mono:wght@300;400;500&display=swap');

        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        body {
          background: #060d1a;
          min-height: 100vh;
        }

        .wd-select, .wd-input {
          background: rgba(255,255,255,0.04);
          border: 1px solid rgba(99,179,237,0.2);
          border-radius: 10px;
          color: #e2e8f0;
          font-family: 'DM Mono', monospace;
          font-size: 13px;
          padding: 10px 14px;
          outline: none;
          transition: border-color 0.2s, box-shadow 0.2s;
          appearance: none;
          -webkit-appearance: none;
          cursor: pointer;
          width: 100%;
        }
        .wd-select:hover, .wd-input:hover {
          border-color: rgba(99,179,237,0.5);
        }
        .wd-select:focus, .wd-input:focus {
          border-color: #63b3ed;
          box-shadow: 0 0 0 3px rgba(99,179,237,0.12);
        }
        .wd-select option {
          background: #0d1b2e;
          color: #e2e8f0;
        }
        input[type="datetime-local"]::-webkit-calendar-picker-indicator {
          filter: invert(0.7) sepia(1) hue-rotate(185deg);
          cursor: pointer;
        }
        .wd-btn {
          background: linear-gradient(135deg, #2b6cb0, #3182ce);
          border: none;
          border-radius: 10px;
          color: #fff;
          cursor: pointer;
          font-family: 'Syne', sans-serif;
          font-size: 14px;
          font-weight: 700;
          letter-spacing: 1px;
          padding: 11px 28px;
          text-transform: uppercase;
          transition: transform 0.15s, box-shadow 0.15s, opacity 0.15s;
          white-space: nowrap;
          box-shadow: 0 4px 20px rgba(49,130,206,0.3);
        }
        .wd-btn:hover:not(:disabled) {
          transform: translateY(-1px);
          box-shadow: 0 6px 28px rgba(49,130,206,0.45);
        }
        .wd-btn:active:not(:disabled) {
          transform: translateY(0);
        }
        .wd-btn:disabled {
          opacity: 0.5;
          cursor: not-allowed;
        }
        .wd-label {
          display: block;
          font-family: 'DM Mono', monospace;
          font-size: 10px;
          letter-spacing: 2px;
          text-transform: uppercase;
          color: #718096;
          margin-bottom: 7px;
        }
        .select-wrapper {
          position: relative;
        }
        .select-wrapper::after {
          content: '▾';
          position: absolute;
          right: 14px;
          top: 50%;
          transform: translateY(-50%);
          color: #63b3ed;
          pointer-events: none;
          font-size: 12px;
        }

        /* Fade-in animation */
        @keyframes fadeUp {
          from { opacity: 0; transform: translateY(16px); }
          to   { opacity: 1; transform: translateY(0); }
        }
        .fade-up { animation: fadeUp 0.5s ease forwards; }

        /* Pulsing dot for loading */
        @keyframes pulse {
          0%, 100% { opacity: 1; } 50% { opacity: 0.3; }
        }
        .loading-dot { animation: pulse 1.2s ease infinite; }
      `}</style>

      <div style={{
        minHeight: "100vh",
        background: "radial-gradient(ellipse at 20% 0%, #0d2140 0%, #060d1a 60%)",
        padding: "40px 24px 60px",
        fontFamily: "'Syne', sans-serif",
        position: "relative",
      }}>
        {/* Background decorative rings */}
        <div style={{
          position: "fixed", top: -200, right: -200, width: 600, height: 600,
          borderRadius: "50%",
          border: "1px solid rgba(99,179,237,0.05)",
          pointerEvents: "none",
        }} />
        <div style={{
          position: "fixed", top: -300, right: -300, width: 800, height: 800,
          borderRadius: "50%",
          border: "1px solid rgba(99,179,237,0.03)",
          pointerEvents: "none",
        }} />

        {/* ── HEADER ── */}
        <div style={{ maxWidth: 1100, margin: "0 auto 48px" }} className="fade-up">
          <div style={{ display: "flex", alignItems: "center", gap: 14, marginBottom: 6 }}>
            <div style={{
              width: 8, height: 8, borderRadius: "50%",
              background: "#63b3ed",
              boxShadow: "0 0 12px #63b3ed",
            }} />
            <span style={{
              fontFamily: "'DM Mono', monospace", fontSize: 11,
              letterSpacing: 3, color: "#4a9fd4", textTransform: "uppercase",
            }}>
              Weather Insights Platform
            </span>
          </div>
          <h1 style={{
            fontSize: "clamp(28px, 5vw, 52px)",
            fontWeight: 800,
            color: "#f7fafc",
            lineHeight: 1.1,
            letterSpacing: -1,
          }}>
            Climate{" "}
            <span style={{
              background: "linear-gradient(135deg, #63b3ed, #4299e1)",
              WebkitBackgroundClip: "text",
              WebkitTextFillColor: "transparent",
            }}>
              Insights
            </span>
          </h1>
          <p style={{ color: "#4a5568", fontFamily: "'DM Mono', monospace", fontSize: 13, marginTop: 10 }}>
            Hourly temperature, wind & precipitation analysis
          </p>
        </div>

        {/* ── CONTROL PANEL ── */}
        <div style={{ maxWidth: 1100, margin: "0 auto" }}>
          <div
            className="fade-up"
            style={{
              background: "rgba(255,255,255,0.025)",
              border: "1px solid rgba(99,179,237,0.12)",
              borderRadius: 20,
              padding: "28px 32px",
              backdropFilter: "blur(20px)",
              animationDelay: "0.1s",
            }}
          >
            <div style={{
              display: "flex",
              flexWrap: "wrap",
              gap: 20,
              alignItems: "flex-end",
            }}>
              {/* City */}
              <div style={{ flex: "2 1 200px", minWidth: 180 }}>
                <label className="wd-label">City</label>
                <div className="select-wrapper">
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

              {/* From */}
              <div style={{ flex: "1.5 1 180px", minWidth: 160 }}>
                <label className="wd-label">From</label>
                <input
                  type="datetime-local"
                  className="wd-input"
                  value={fromDt}
                  onChange={(e) => setFromDt(e.target.value)}
                />
              </div>

              {/* To */}
              <div style={{ flex: "1.5 1 180px", minWidth: 160 }}>
                <label className="wd-label">To</label>
                <input
                  type="datetime-local"
                  className="wd-input"
                  value={toDt}
                  onChange={(e) => setToDt(e.target.value)}
                />
              </div>

              {/* Submit */}
              <div style={{ flex: "0 0 auto" }}>
                <button
                  className="wd-btn"
                  onClick={fetchData}
                  disabled={loading || citiesLoading || cityId === null}
                >
                  {loading ? (
                    <span className="loading-dot">Fetching…</span>
                  ) : (
                    "Analyse →"
                  )}
                </button>
              </div>
            </div>
          </div>

          {/* Error notice */}
          {error && (
            <p style={{
              fontFamily: "'DM Mono', monospace", fontSize: 12,
              color: "#f6ad55", marginTop: 12, paddingLeft: 4,
            }}>
              ⚠ {error}
            </p>
          )}
        </div>

        {/* ── DIVIDER ── */}
        {submitted && (
          <div style={{
            maxWidth: 1100, margin: "40px auto",
            display: "flex", alignItems: "center", gap: 16,
          }}>
            <div style={{ flex: 1, height: 1, background: "linear-gradient(90deg, transparent, rgba(99,179,237,0.3))" }} />
            <span style={{
              fontFamily: "'DM Mono', monospace", fontSize: 10,
              letterSpacing: 3, color: "#4a9fd4", textTransform: "uppercase",
              whiteSpace: "nowrap",
            }}>
              {cityName} · {data.length > 0 ? `${data.length} data points` : "no data"}
            </span>
            <div style={{ flex: 1, height: 1, background: "linear-gradient(90deg, rgba(99,179,237,0.3), transparent)" }} />
          </div>
        )}

        {/* ── STATS + CHART ── */}
        {submitted && (
          <div style={{ maxWidth: 1100, margin: "0 auto" }} className="fade-up">

            {/* Stat cards — only when there's data */}
            {data.length > 0 && (
              <div style={{ display: "flex", gap: 16, flexWrap: "wrap", marginBottom: 28 }}>
                <StatCard label="Max Temp" value={maxTemp} unit="°C" accent="#f6ad55" />
                <StatCard label="Min Temp" value={minTemp} unit="°C" accent="#63b3ed" />
                <StatCard label="Avg Wind" value={avgWind} unit="m/s" accent="#68d391" />
                <StatCard label="Total Precip." value={totalPrecip} unit="mm" accent="#b794f4" />
              </div>
            )}

            {/* Temperature Chart */}
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

            {/* Wind Chart */}
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

            {/* Precipitation Chart */}
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

        {/* Empty / loading state */}
        {!submitted && !loading && (
          <div style={{ textAlign: "center", marginTop: 80, color: "#2d3748" }}>
            <p style={{ fontSize: 48 }}>🌐</p>
            <p style={{ fontFamily: "'DM Mono', monospace", fontSize: 13, marginTop: 12 }}>
              Select a city and time range, then click Analyse
            </p>
          </div>
        )}
      </div>
    </>
  );
}

// ─── Reusable Chart Block ────────────────────────────────────────────────────
function ChartBlock({ title, accent, accentEnd, dataKey, unit, chartData, yDomain, empty, multiDay, style }) {
  const gradId = `grad-${dataKey.replace(/\s/g, "")}`;
  return (
    <div
      style={{
        background: "rgba(255,255,255,0.025)",
        border: "1px solid rgba(255,255,255,0.06)",
        borderRadius: 20,
        padding: "28px 24px 20px",
        backdropFilter: "blur(10px)",
        position: "relative",
        overflow: "hidden",
        ...style,
      }}
    >
      {/* top accent stripe */}
      <div style={{
        position: "absolute", top: 0, left: 0, right: 0, height: 2,
        background: `linear-gradient(90deg, ${accent}, ${accentEnd}, transparent)`,
      }} />

      <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 20 }}>
        <div style={{
          width: 10, height: 10, borderRadius: "50%",
          background: accent,
          boxShadow: `0 0 10px ${accent}`,
        }} />
        <h2 style={{
          fontFamily: "'Syne', sans-serif",
          fontSize: 15, fontWeight: 700,
          color: "#cbd5e0", letterSpacing: 0.5,
        }}>
          {title}
          <span style={{ color: "#4a5568", fontWeight: 400, marginLeft: 8, fontSize: 12 }}>({unit})</span>
        </h2>
      </div>

      {empty ? (
        <div style={{
          height: 200,
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          flexDirection: "column",
          gap: 10,
          border: "1px dashed rgba(255,255,255,0.08)",
          borderRadius: 12,
        }}>
          <span style={{ fontSize: 28, opacity: 0.3 }}>📭</span>
          <p style={{
            fontFamily: "'DM Mono', monospace",
            fontSize: 12,
            color: "#4a5568",
            letterSpacing: 1,
          }}>
            No data found to display
          </p>
        </div>
      ) : (() => {
        // Give each data point ~12px of horizontal space so the axis never collapses.
        // For small datasets (≤48 pts / 2 days) fill the full container width instead.
        const pointWidth = 12;
        const minWidth = chartData.length * pointWidth;
        // Show a tick every 6 hours = every 6 points
        const tickInterval = 5;
        return (
          <div style={{ overflowX: "auto", overflowY: "hidden", cursor: "grab" }}>
            <div style={{ width: Math.max(minWidth, 600), height: 220 }}>
              <AreaChart
                width={Math.max(minWidth, 600)}
                height={220}
                data={chartData}
                margin={{ top: 4, right: 20, bottom: 24, left: 0 }}
              >
                <defs>
                  <linearGradient id={gradId} x1="0" y1="0" x2="0" y2="1">
                    <stop offset="0%" stopColor={accent} stopOpacity={0.35} />
                    <stop offset="100%" stopColor={accent} stopOpacity={0} />
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="rgba(255,255,255,0.04)" vertical={false} />
                <XAxis
                  dataKey="time"
                  tick={{ fill: "#4a5568", fontSize: 10, fontFamily: "DM Mono, monospace" }}
                  axisLine={false}
                  tickLine={{ stroke: "rgba(255,255,255,0.06)" }}
                  interval={tickInterval}
                  tickFormatter={(ts) => formatTick(ts, multiDay)}
                  angle={multiDay ? -35 : 0}
                  textAnchor={multiDay ? "end" : "middle"}
                  height={multiDay ? 40 : 20}
                />
                <YAxis
                  domain={yDomain}
                  tick={{ fill: "#4a5568", fontSize: 11, fontFamily: "DM Mono, monospace" }}
                  axisLine={false}
                  tickLine={false}
                  width={42}
                  tickFormatter={(v) => v.toFixed(1)}
                />
                <Tooltip content={<CustomTooltip />} />
                <Area
                  type="monotone"
                  dataKey={dataKey}
                  stroke={accent}
                  strokeWidth={1.5}
                  fill={`url(#${gradId})`}
                  dot={false}
                  activeDot={{ r: 4, fill: accent, stroke: "#060d1a", strokeWidth: 2 }}
                  isAnimationActive={chartData.length < 200}
                />
              </AreaChart>
            </div>
          </div>
        );
      })()}
    </div>
  );
}