#!/bin/bash

# CosmoCargo database backup script
# This script creates a backup of the PostgreSQL database

# Usage: ./backup-db.sh [environment]
# Example: ./backup-db.sh dev

set -e

# Default to dev environment if not specified
ENVIRONMENT=${1:-dev}
TIMESTAMP=$(date +%Y%m%d%H%M%S)
BACKUP_DIR="/tmp/cosmocargo-backups"
BACKUP_FILE="$BACKUP_DIR/cosmocargo-$ENVIRONMENT-$TIMESTAMP.sql"

# Validate environment
if [[ ! "$ENVIRONMENT" =~ ^(dev|staging|prod)$ ]]; then
  echo "Invalid environment: $ENVIRONMENT"
  echo "Valid environments: dev, staging, prod"
  exit 1
fi

# Create backup directory if it doesn't exist
mkdir -p "$BACKUP_DIR"

echo "Creating backup of CosmoCargo database in $ENVIRONMENT environment..."

# Get database credentials from AWS Secrets Manager
if [[ "$ENVIRONMENT" != "dev" ]]; then
  DB_HOST=$(aws secretsmanager get-secret-value --secret-id cosmocargo-$ENVIRONMENT-db --query SecretString --output text | jq -r .host)
  DB_NAME=$(aws secretsmanager get-secret-value --secret-id cosmocargo-$ENVIRONMENT-db --query SecretString --output text | jq -r .dbname)
  DB_USER=$(aws secretsmanager get-secret-value --secret-id cosmocargo-$ENVIRONMENT-db --query SecretString --output text | jq -r .username)
  DB_PASSWORD=$(aws secretsmanager get-secret-value --secret-id cosmocargo-$ENVIRONMENT-db --query SecretString --output text | jq -r .password)
else
  # Use local development credentials
  DB_HOST="localhost"
  DB_NAME="cosmocargo"
  DB_USER="postgres"
  DB_PASSWORD="postgres"
fi

# Create database backup
PGPASSWORD="$DB_PASSWORD" pg_dump -h "$DB_HOST" -U "$DB_USER" -d "$DB_NAME" -f "$BACKUP_FILE"

# Compress backup
gzip "$BACKUP_FILE"

echo "Backup created: $BACKUP_FILE.gz"

# Upload backup to S3 if not in dev environment
if [[ "$ENVIRONMENT" != "dev" ]]; then
  S3_BUCKET="cosmocargo-$ENVIRONMENT-backups"
  S3_KEY="database/cosmocargo-$ENVIRONMENT-$TIMESTAMP.sql.gz"
  
  echo "Uploading backup to S3: s3://$S3_BUCKET/$S3_KEY"
  aws s3 cp "$BACKUP_FILE.gz" "s3://$S3_BUCKET/$S3_KEY"
  
  # Clean up local backup
  rm "$BACKUP_FILE.gz"
  
  echo "Backup uploaded to S3 and local file removed"
fi

echo "Backup process completed successfully!" 