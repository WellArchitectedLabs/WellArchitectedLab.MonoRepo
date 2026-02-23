/**
 * StatCard.jsx
 * Displays one summary statistic (max temp, avg wind, etc.).
 *
 * @param {string}      label   Card label, e.g. "Max Temp"
 * @param {number|null} value   Numeric value; renders "—" when null
 * @param {string}      unit    Unit suffix, e.g. "°C"
 * @param {string}      accent  Hex colour used for the top bar and the unit text
 */
export function StatCard({ label, value, unit, accent }) {
  return (
    <div
      className="wd-stat-card"
      style={{ borderColor: `${accent}30` }}
    >
      {/* Top colour stripe */}
      <div
        className="wd-stat-card__bar"
        style={{
          background: `linear-gradient(90deg, transparent, ${accent}, transparent)`,
        }}
      />

      <p className="wd-stat-card__label">{label}</p>

      <p className="wd-stat-card__value">
        {value !== null ? Number(value).toFixed(1) : "—"}
        <span
          className="wd-stat-card__unit"
          style={{ color: accent }}
        >
          {unit}
        </span>
      </p>
    </div>
  );
}