#!/bin/bash
set -e

echo "Настройка репликации на мастере..."

# Выдать пользователю права репликации
psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    ALTER ROLE "$POSTGRES_USER" WITH REPLICATION;
EOSQL
