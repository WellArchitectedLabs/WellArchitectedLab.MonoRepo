Why a separate database ?
Difference between database and pvc usage.
Commands to investigate : get all postgres users: kubectl exec -it grafana-database-postgresql-0 -n monitoring -- psql -U postgres -c "\l"
Same command for databases for example