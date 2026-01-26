This is where the weatherly app source code lives.
The solution is developed using Aspnet core...
Setupping database migrator: flyway
flyway baseline generation command: flyway -url='jdbc:postgresql://localhost:5432/master_data?sslmode=disable' -user=postgres -password=postgres baseline
flyway migrate command: flyway -url='jdbc:postgresql://localhost:5432/master_data?sslmode=disable' -user=postgres -password=postgres migrate 