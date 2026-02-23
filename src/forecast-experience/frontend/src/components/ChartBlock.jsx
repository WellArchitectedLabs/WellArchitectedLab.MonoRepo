/**
 * ChartBlock.jsx
 * A scrollable area-chart card for one weather metric.
 *
 * ── How chart width works ────────────────────────────────────────────────────
 *
 * There are two distinct rendering modes:
 *
 *  A) FEW POINTS (close date range, e.g. one day = ~24 pts)
 *     → needsScroll = false
 *     → We render a <ResponsiveContainer width="100%"> so the chart fills
 *       the full card width automatically. No clipping, no truncation.
 *
 *  B) MANY POINTS (wide date range, e.g. two weeks = ~336 pts)
 *     → needsScroll = true
 *     → We render a fixed-pixel <AreaChart width={canvasWidth}> inside an
 *       overflow-x: auto scroll container. The canvas is wide enough to give
 *       each point its breathing room.
 *
 * The threshold is SCROLL_THRESHOLD_PX: if (pointCount × PX_PER_POINT) would
 * exceed that, we switch to scroll mode. 800px is a safe default that covers
 * screens from 768px upwards.
 *
 * This avoids the ResizeObserver timing race (measuring after paint gave the
 * wrong width on first render) and avoids the ResponsiveContainer collapse bug
 * (ResponsiveContainer needs a parent with a known, non-zero width — the scroll
 * div with overflow:auto satisfies that only in mode B when width is explicit).
 */

import {
  AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
} from "recharts";
import { CustomTooltip } from "./CustomTooltip";
import { formatAxisTick, X_TICK_INTERVAL } from "../utils/weatherHelpers";

// ─── Constants ────────────────────────────────────────────────────────────────

/** Pixels per data point in scroll mode. */
const PX_PER_POINT = 14;

/**
 * If the data-driven pixel width exceeds this, switch to scroll mode.
 * Below this threshold, ResponsiveContainer fills the card naturally.
 */
const SCROLL_THRESHOLD_PX = 800;

// ─── Component ────────────────────────────────────────────────────────────────

/**
 * @param {string}   title      Metric name shown in the card header, e.g. "Temperature"
 * @param {string}   accent     Primary colour — stroke, glow dot, gradient start
 * @param {string}   accentEnd  Gradient end colour for the top stripe
 * @param {string}   dataKey    Recharts series key, must match a key in chartData items
 * @param {string}   unit       Unit shown in the header, e.g. "°C"
 * @param {Array}    chartData  Output of toChartData() from weatherHelpers
 * @param {Array}    yDomain    Recharts YAxis domain, e.g. ["auto","auto"] or [0,"auto"]
 * @param {boolean}  empty      True when the backend returned no data (204 or [])
 * @param {boolean}  multiDay   True when the data spans more than one calendar day
 * @param {object}   [style]    Optional extra inline styles for the outer card div
 */
export function ChartBlock({
  title, accent, accentEnd, dataKey, unit,
  chartData, yDomain, empty, multiDay, style,
}) {
  // Unique gradient id per chart — prevents gradients bleeding across SVGs
  const gradientId = `wd-grad-${dataKey.replace(/\s+/g, "")}`;

  const dataPixelWidth = chartData.length * PX_PER_POINT;
  const needsScroll    = dataPixelWidth > SCROLL_THRESHOLD_PX;
  const canvasWidth    = needsScroll ? dataPixelWidth : undefined; // undefined → ResponsiveContainer handles it
  const xAxisHeight    = multiDay ? 40 : 20;

  // Shared Recharts chart props (identical regardless of scroll mode)
  const chartProps = {
    data:   chartData,
    height: 220,
    margin: { top: 4, right: 20, bottom: 24, left: 0 },
  };

  // Shared child elements (identical regardless of scroll mode)
  const chartChildren = (
    <>
      <defs>
        <linearGradient id={gradientId} x1="0" y1="0" x2="0" y2="1">
          <stop offset="0%"   stopColor={accent} stopOpacity={0.35} />
          <stop offset="100%" stopColor={accent} stopOpacity={0}    />
        </linearGradient>
      </defs>

      <CartesianGrid
        strokeDasharray="3 3"
        stroke="var(--chart-grid)"
        vertical={false}
      />

      <XAxis
        dataKey="time"
        tick={{ fill: "var(--chart-tick)", fontSize: 10, fontFamily: "DM Mono, monospace" }}
        axisLine={false}
        tickLine={{ stroke: "var(--chart-tickline)" }}
        interval={X_TICK_INTERVAL}
        tickFormatter={(ts) => formatAxisTick(ts, multiDay)}
        angle={multiDay ? -35 : 0}
        textAnchor={multiDay ? "end" : "middle"}
        height={xAxisHeight}
      />

      <YAxis
        domain={yDomain}
        tick={{ fill: "var(--chart-tick)", fontSize: 11, fontFamily: "DM Mono, monospace" }}
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
        fill={`url(#${gradientId})`}
        dot={false}
        activeDot={{ r: 4, fill: accent, stroke: "#060d1a", strokeWidth: 2 }}
        isAnimationActive={chartData.length < 200}
      />
    </>
  );

  return (
    <div className="wd-chart-card" style={style}>

      {/* Top accent stripe */}
      <div
        className="wd-chart-card__bar"
        style={{ background: `linear-gradient(90deg, ${accent}, ${accentEnd}, transparent)` }}
      />

      {/* Card header */}
      <div className="wd-chart-card__header">
        <div
          className="wd-chart-card__dot"
          style={{ background: accent, boxShadow: `0 0 10px ${accent}` }}
        />
        <h2 className="wd-chart-card__title">
          {title}
          <span className="wd-chart-card__unit-label">({unit})</span>
        </h2>
      </div>

      {/* Empty state */}
      {empty ? (
        <div className="wd-chart-card__empty">
          <span className="wd-chart-card__empty-icon">📭</span>
          <p className="wd-chart-card__empty-text">No data found to display</p>
        </div>

      ) : needsScroll ? (
        /* ── Mode B: wide dataset — fixed width canvas inside a scroll container ── */
        <div className="wd-chart-card__scroll">
          <AreaChart width={canvasWidth} {...chartProps}>
            {chartChildren}
          </AreaChart>
        </div>

      ) : (
        /* ── Mode A: close dates — ResponsiveContainer fills the card width ── */
        <ResponsiveContainer width="100%" height={220}>
          <AreaChart {...chartProps}>
            {chartChildren}
          </AreaChart>
        </ResponsiveContainer>
      )}

    </div>
  );
}