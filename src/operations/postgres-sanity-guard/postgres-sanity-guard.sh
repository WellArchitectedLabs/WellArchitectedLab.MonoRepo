#!/bin/bash
set -e

# ==============================================================================
# PostgreSQL Sanity Guard
# ==============================================================================
# Description: Validates PostgreSQL database readiness and user connectivity
# Usage: Set required environment variables and run
# ==============================================================================

# Required environment variables
: "${DB_HOST:?ERROR: DB_HOST is required}"
: "${DB_NAME:?ERROR: DB_NAME is required}"
: "${DB_APP_USER:?ERROR: DB_APP_USER is required}"
: "${DB_APP_PASSWORD:?ERROR: DB_APP_PASSWORD is required}"
: "${POSTGRES_SUPERUSER_PASSWORD:?ERROR: POSTGRES_SUPERUSER_PASSWORD is required}"

# Optional environment variables with defaults
DB_PORT="${DB_PORT:-5432}"
MAX_RETRIES="${MAX_RETRIES:-30}"
RETRY_INTERVAL="${RETRY_INTERVAL:-2}"

echo "=========================================="
echo "PostgreSQL Sanity Guard"
echo "=========================================="
echo "Database Host: $DB_HOST"
echo "Database Port: $DB_PORT"
echo "Database Name: $DB_NAME"
echo "App User: $DB_APP_USER"
echo "Max Retries: $MAX_RETRIES"
echo "Retry Interval: ${RETRY_INTERVAL}s"
echo "=========================================="

# ==============================================================================
# Step 1: Check if PostgreSQL is accepting connections
# ==============================================================================
echo ""
echo "Step 1: Checking if PostgreSQL is ready..."
retry_count=0

until pg_isready -h "$DB_HOST" -p "$DB_PORT" -U postgres; do
  retry_count=$((retry_count + 1))
  if [ $retry_count -ge $MAX_RETRIES ]; then
    echo "✗ ERROR: PostgreSQL not ready after $MAX_RETRIES attempts"
    exit 1
  fi
  echo "PostgreSQL not ready yet (attempt $retry_count/$MAX_RETRIES)..."
  sleep "$RETRY_INTERVAL"
done

echo "✓ PostgreSQL is accepting connections"

# ==============================================================================
# Step 2: Verify database exists
# ==============================================================================
echo ""
echo "Step 2: Verifying database '$DB_NAME' exists..."
export PGPASSWORD=$POSTGRES_SUPERUSER_PASSWORD

if psql -h "$DB_HOST" -p "$DB_PORT" -U postgres -d "$DB_NAME" -c "SELECT 1" > /dev/null 2>&1; then
  echo "✓ Database '$DB_NAME' exists"
else
  echo "✗ ERROR: Database '$DB_NAME' does NOT exist!"
  exit 1
fi

# ==============================================================================
# Step 3: Check if application user exists
# ==============================================================================
echo ""
echo "Step 3: Checking if user '$DB_APP_USER' exists..."

USER_EXISTS=$(psql -h "$DB_HOST" -p "$DB_PORT" -U postgres -d "$DB_NAME" \
                   -tAc "SELECT 1 FROM pg_roles WHERE rolname='$DB_APP_USER'" 2>/dev/null || echo "")

if [ "$USER_EXISTS" = "1" ]; then
  echo "✓ User '$DB_APP_USER' exists"
else
  echo "✗ ERROR: User '$DB_APP_USER' does NOT exist!"
  echo "Please ensure your database migrations create this user."
  exit 1
fi

# ==============================================================================
# Step 4: Test connection as application user
# ==============================================================================
echo ""
echo "Step 4: Testing connection as '$DB_APP_USER'..."
export PGPASSWORD=$DB_APP_PASSWORD

retry_count=0
until psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_APP_USER" -d "$DB_NAME" \
           -c "SELECT 1" > /dev/null 2>&1; do
  retry_count=$((retry_count + 1))
  if [ $retry_count -ge $MAX_RETRIES ]; then
    echo "✗ ERROR: Cannot connect as '$DB_APP_USER' after $MAX_RETRIES attempts"
    exit 1
  fi
  echo "Cannot connect as '$DB_APP_USER' (attempt $retry_count/$MAX_RETRIES)..."
  sleep "$RETRY_INTERVAL"
done

echo "✓ Successfully connected as '$DB_APP_USER'"

# ==============================================================================
# Step 5: Verify user has necessary permissions
# ==============================================================================
echo ""
echo "Step 5: Verifying user permissions..."

# Check if user can list tables
if psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_APP_USER" -d "$DB_NAME" \
        -c "\dt" > /dev/null 2>&1; then
  echo "✓ User can list tables"
else
  echo "⚠ WARNING: User cannot list tables (may be OK if no tables exist yet)"
fi

# Check if user has CONNECT privilege
HAS_CONNECT=$(psql -h "$DB_HOST" -p "$DB_PORT" -U postgres -d "$DB_NAME" \
                   -tAc "SELECT has_database_privilege('$DB_APP_USER', '$DB_NAME', 'CONNECT')" 2>/dev/null || echo "f")

if [ "$HAS_CONNECT" = "t" ]; then
  echo "✓ User has CONNECT privilege"
else
  echo "⚠ WARNING: User does NOT have CONNECT privilege"
fi

# ==============================================================================
# Success!
# ==============================================================================
echo ""
echo "=========================================="
echo "✓ All sanity checks passed!"
echo "=========================================="
echo "PostgreSQL is ready for application use."
echo ""

exit 0