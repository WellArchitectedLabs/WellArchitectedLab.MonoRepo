-- ===========================================================================================
-- Flyway Migration: V3 - Migrate to timezone-aware timestamps
-- ===========================================================================================
-- Description: Uses recommended time zone based timestamps instead of no time zoned data types
-- Author: Master Data API development team
-- Date: 2026-02-22
-- ===========================================================================================

ALTER TABLE public.wf_actuals
ALTER COLUMN timestamp_utc
TYPE timestamptz
USING timestamp_utc AT TIME ZONE 'UTC';

-- ===========================================================================================
-- End of V3 Migration
-- ===========================================================================================