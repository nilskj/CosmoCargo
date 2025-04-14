# CosmoCargo Backend

## Databashantering

### Förutsättningar
- .NET SDK 9.0
- Docker och Docker Compose
- PostgreSQL-klient (valfritt)

### Starta databasen
```bash
docker-compose up db -d
```

### Köra migrationer
```bash
# Från backend-katalogen
chmod +x db-migrate.sh
./db-migrate.sh
```

### Skapa en ny migration
```bash
./db-migrate.sh add NamnPåMigration
```

### Ta bort senaste migrationen
```bash
./db-migrate.sh remove
```

### Ansluta till databasen
PostgreSQL-databasen är tillgänglig på:
- Host: localhost
- Port: 5432
- Databas: cosmocargo
- Användare: postgres
- Lösenord: postgres

### Använda pgAdmin
pgAdmin är tillgängligt på http://localhost:5050
- E-post: admin@cosmocargo.com
- Lösenord: admin

För att ansluta till databasen i pgAdmin:
1. Högerklicka på "Servers" och välj "Create" > "Server..."
2. Ange ett namn (t.ex. "CosmoCargo")
3. Gå till fliken "Connection" och ange:
   - Host: db (använd detta namn i Docker-nätverket)
   - Port: 5432
   - Databas: cosmocargo
   - Användare: postgres
   - Lösenord: postgres
4. Klicka på "Save"

## API-dokumentation

API-dokumentation är tillgänglig via Swagger på http://localhost:5000/swagger när applikationen körs.

## Testdata

Systemet seedar automatiskt databasen med testdata vid första start:

### Användare
- Admin: admin@cosmocargo.com / Admin123!
- Pilot: pilot@cosmocargo.com / Pilot123!
- Kund: kund@example.com / Kund123! 