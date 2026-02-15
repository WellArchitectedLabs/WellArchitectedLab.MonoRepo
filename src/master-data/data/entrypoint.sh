#!/bin/bash
set -e

echo "=== Processing SQL templates with envsubst ==="

# Process all SQL templates and output to flyway/sql directory
for template in /flyway/sql-templates/*.sql; do
    if [ -f "$template" ]; then
        filename=$(basename "$template")
        echo "Processing: $filename"
        envsubst < "$template" > "/flyway/sql/$filename"
    fi
done

echo "=== Template processing complete ==="
echo "=== Starting Flyway with command: $@ ==="

# Execute flyway with provided arguments
exec flyway "$@"