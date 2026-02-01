-- ============================================================================
-- Flyway Migration: V1 - Create Application User and Grant Permissions
-- ============================================================================
-- Description: Creates the application user with appropriate permissions
-- Author: DevOps Team
-- Date: 2026-02-01
-- ============================================================================

-- Note: This script runs as the postgres superuser (configured in Flyway)
-- Environment variables will be substituted by envsubst before execution

DO $$
BEGIN
  -- Create application user if it doesn't exist
  IF NOT EXISTS (
    SELECT FROM pg_catalog.pg_roles 
    WHERE rolname = '${DB_APP_USER}'
  ) THEN
    EXECUTE format(
      'CREATE ROLE %I LOGIN PASSWORD %L',
      '${DB_APP_USER}',
      '${DB_APP_PASSWORD}'
    );
    RAISE NOTICE 'User ${DB_APP_USER} created successfully';
  ELSE
    -- Update password if user already exists
    EXECUTE format(
      'ALTER ROLE %I WITH PASSWORD %L',
      '${DB_APP_USER}',
      '${DB_APP_PASSWORD}'
    );
    RAISE NOTICE 'User ${DB_APP_USER} already exists - password updated';
  END IF;
END
$$;

-- Grant database connection privileges
GRANT CONNECT ON DATABASE ${POSTGRES_DATABASE} TO ${DB_APP_USER};

-- Grant schema usage and creation privileges
GRANT USAGE, CREATE ON SCHEMA public TO ${DB_APP_USER};

-- Grant permissions on all existing tables
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO ${DB_APP_USER};

-- Grant permissions on all existing sequences (for auto-increment columns)
GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO ${DB_APP_USER};

-- Set default privileges for future objects created by postgres user
ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public
  GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO ${DB_APP_USER};

ALTER DEFAULT PRIVILEGES FOR ROLE postgres IN SCHEMA public
  GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO ${DB_APP_USER};

-- Verify user creation
DO $$
DECLARE
  user_info RECORD;
BEGIN
  SELECT rolname, rolcanlogin INTO user_info
  FROM pg_roles 
  WHERE rolname = '${DB_APP_USER}';
  
  RAISE NOTICE 'User verification: % (Can login: %)', user_info.rolname, user_info.rolcanlogin;
END
$$;

-- ============================================================================
-- End of V1 Migration
-- ============================================================================