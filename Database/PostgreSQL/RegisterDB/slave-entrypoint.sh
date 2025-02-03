#!/bin/bash
set -e

if [ -z "$(ls -A $PGDATA)" ]; then
  echo "Initializing replication from master ($MASTER_HOST)..."

  until pg_isready -h "$MASTER_HOST" -p 5432; do
    echo "Waiting for master to become ready..."
    sleep 2
  done

  echo "$MASTER_HOST:5432:*:$REPLICATION_USER:$REPLICATION_PASSWORD" > ~/.pgpass
  chmod 600 ~/.pgpass

  pg_basebackup -h "$MASTER_HOST" -p 5432 -D "$PGDATA" -U "$REPLICATION_USER" -v -P --wal-method=stream

  touch "$PGDATA/standby.signal"

  echo "primary_conninfo = 'host=$MASTER_HOST port=5432 user=$REPLICATION_USER password=$REPLICATION_PASSWORD application_name=${APPLICATION_NAME:-postgres_authorize_slave}'" \
    >> "$PGDATA/postgresql.auto.conf"

  chown -R postgres:postgres "$PGDATA"
  chmod 0700 "$PGDATA"
fi

exec gosu postgres "$@"
