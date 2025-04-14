#!/bin/bash

# Installera dotnet-ef om det inte redan finns
if ! command -v dotnet-ef &> /dev/null
then
    echo "Installerar dotnet-ef verktyg..."
    dotnet tool install --global dotnet-ef
fi

# Skapa en ny migration
if [ "$1" = "add" ] && [ -n "$2" ]
then
    echo "Skapar ny migration: $2"
    dotnet ef migrations add $2
    exit 0
fi

# Ta bort senaste migrationen
if [ "$1" = "remove" ]
then
    echo "Tar bort senaste migrationen"
    dotnet ef migrations remove
    exit 0
fi

# Uppdatera databasen
if [ "$1" = "update" ] || [ -z "$1" ]
then
    echo "Uppdaterar databasen"
    dotnet ef database update
    exit 0
fi

# Visa hjälp
echo "Användning:"
echo "  ./db-migrate.sh add <namn>  - Skapa en ny migration"
echo "  ./db-migrate.sh remove      - Ta bort senaste migrationen"
echo "  ./db-migrate.sh update      - Uppdatera databasen (standard)"
echo "  ./db-migrate.sh             - Uppdatera databasen (standard)" 