This is where the weatherly app source code lives.
The solution is developed using Aspnet core...
Setupping database migrator: flyway
flyway baseline generation command: flyway -url='jdbc:postgresql://localhost:5432/master_data?sslmode=disable' -user=postgres -password=postgres baseline
flyway migrate command: flyway -url='jdbc:postgresql://localhost:5432/master_data?sslmode=disable' -user=postgres -password=postgres migrate

adding details on make file usage and how to differentiate docker compose level orchestration (startup dependencies) and cross service level orchestration which is done by make.
Also talk about more modern tools.

Install task as a replacement to make.
Talk about Makefile vs Taskfile (more modern with yaml defnition)
installation: brew install go-task
Main commands: task --list, task target, features like runOnce and clean syntax with deps. Compare it also with modern CI yaml like github workflows or azure devops.
usage of no build flag: task import-cities NO_BUILD=true

Investigation tools:

docker ps --format "table {{.Names}}\t{{.Image}}\t{{.Status}}\t{{.Networks}}" (display every container with its associated network)