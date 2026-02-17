-- ============================================================================
-- Flyway Migration: V3 - Add unique constraint on wf_insights
-- ============================================================================
-- Description: Hardens wf_insights table by enforcing uniqueness on the
--              (city_id, timestamp_utc) pair, preventing duplicate insight
--              entries for the same city at the same point in time.
-- Author: DevOps Team
-- Date: 2026-02-13
-- ============================================================================

ALTER TABLE public.wf_insights
    ADD CONSTRAINT "UQ_wf_insights_city_id_timestamp_utc"
        UNIQUE (city_id, timestamp_utc);

-- ============================================================================
-- End of V3 Migration
-- ============================================================================