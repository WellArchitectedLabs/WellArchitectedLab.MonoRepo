-- ============================================================================
-- Flyway Migration: V2 - Create Weather Insight Schema
-- ============================================================================
-- Description: Creates tables for weather insights and audit trail
-- Author: DevOps Team
-- Date: 2026-02-13
-- ============================================================================

-- --------------------------------------------------------------------------
-- Table: public.wf_insights
-- --------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.wf_insights
(
    id integer NOT NULL
        GENERATED ALWAYS AS IDENTITY
        ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),

    timestamp_utc timestamp without time zone NOT NULL,

    temperature numeric NOT NULL,

    wind_speed numeric NOT NULL,

    precipitation numeric NOT NULL,

    city_id integer NOT NULL,

    CONSTRAINT "PK_wf_insights" PRIMARY KEY (id)
);

ALTER TABLE IF EXISTS public.wf_insights
    OWNER TO postgres;

COMMENT ON TABLE public.wf_insights
    IS 'Stores weather forecasting engine calculation results';

COMMENT ON COLUMN public.wf_insights.id
    IS 'Auto-incremented weather insight identifier';

COMMENT ON COLUMN public.wf_insights.timestamp_utc
    IS 'Timestamp associated to the calculated value, stored in UTC';

COMMENT ON COLUMN public.wf_insights.temperature
    IS 'Celsius based temperature';

COMMENT ON COLUMN public.wf_insights.wind_speed
    IS 'Wind speed parameter used by weather forecasting engine';

COMMENT ON COLUMN public.wf_insights.precipitation
    IS 'Precipitation parameter used by weather forecasting engine';

COMMENT ON COLUMN public.wf_insights.city_id
    IS 'External city identifier (no foreign key required)';

-- Grant permissions to application user
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.wf_insights TO ${DB_APP_USER};


-- --------------------------------------------------------------------------
-- Table: public.wf_insight_audits
-- --------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS public.wf_insight_audits
(
    request_time_utc timestamp without time zone NOT NULL,

    response_time_utc timestamp without time zone NOT NULL,

    weather_engine_input text NOT NULL,

    weather_engine_output text NOT NULL,

    weather_insight_id integer NULL,

    CONSTRAINT "FK_weather_insight_id_wf_insights"
        FOREIGN KEY (weather_insight_id)
        REFERENCES public.wf_insights (id)
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
);

ALTER TABLE IF EXISTS public.wf_insight_audits
    OWNER TO postgres;

COMMENT ON TABLE public.wf_insight_audits
    IS 'Stores input and output payloads for weather engine executions';

COMMENT ON COLUMN public.wf_insight_audits.request_time_utc
    IS 'Time the calculation was requested from prediction engine (UTC)';

COMMENT ON COLUMN public.wf_insight_audits.response_time_utc
    IS 'Time the calculation was received from prediction engine (UTC)';

COMMENT ON COLUMN public.wf_insight_audits.weather_engine_input
    IS 'Serialized input payload for weather forecasting engine';

COMMENT ON COLUMN public.wf_insight_audits.weather_engine_output
    IS 'Serialized output payload from weather forecasting engine';

COMMENT ON COLUMN public.wf_insight_audits.weather_insight_id
    IS 'Associated weather insight identifier; null if no insight was persisted';

-- Grant permissions to application user
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE public.wf_insight_audits TO ${DB_APP_USER};

-- ============================================================================
-- End of V1 Migration
-- ============================================================================
