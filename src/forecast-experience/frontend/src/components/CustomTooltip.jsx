/**
 * CustomTooltip.jsx
 * Recharts custom tooltip — shown on chart hover.
 * Reads the pre-formatted `label` field so it always displays a
 * human-readable timestamp regardless of how the X-axis is ticked.
 */

/**
 * @param {boolean} active   - Injected by Recharts: true when hovering a point
 * @param {Array}   payload  - Injected by Recharts: array of series values at cursor
 */
export function CustomTooltip({ active, payload }) {
  if (!active || !payload?.length) return null;

  const friendlyLabel = payload[0]?.payload?.label ?? "";

  return (
    <div className="wd-tooltip">
      <p className="wd-tooltip__time">{friendlyLabel}</p>

      {payload.map((entry) => (
        <p
          key={entry.name}
          className="wd-tooltip__row"
          style={{ color: entry.color }}
        >
          <span className="wd-tooltip__name">{entry.name}: </span>
          <strong>
            {typeof entry.value === "number"
              ? entry.value.toFixed(2)
              : entry.value}
          </strong>
          <span className="wd-tooltip__unit">
            {entry.name === "Temperature"
              ? "°C"
              : entry.name === "Wind Speed"
              ? " m/s"
              : " mm"}
          </span>
        </p>
      ))}
    </div>
  );
}