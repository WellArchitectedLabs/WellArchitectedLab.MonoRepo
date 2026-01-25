#!/bin/sh
set -eu

# Entrypoint wrapper to make CLI ergonomics safe for job runners.
# If the container is invoked with an option as the first argument (eg. --input)
# or with no arguments, this wrapper will prepend the DEFAULT_CMD before calling
# `python src/main.py` so callers can simply mount files and pass flags.

DEFAULT_CMD=${DEFAULT_CMD:-}

if [ $# -eq 0 ]; then
  if [ -n "$DEFAULT_CMD" ]; then
    exec python src/main.py $DEFAULT_CMD
  else
    exec python src/main.py
  fi
fi

case "$1" in
  -*) # first arg is an option -> prepend default command if provided
    if [ -n "$DEFAULT_CMD" ]; then
      exec python src/main.py $DEFAULT_CMD "$@"
    else
      exec python src/main.py "$@"
    fi
    ;;
  *) # assume the user provided a subcommand (eg. wf_import or cities_import)
    exec python src/main.py "$@"
    ;;
esac
