#!/bin/bash

# CosmoCargo deployment script
# This script deploys the CosmoCargo application to the specified environment

# Usage: ./deploy.sh [environment]
# Example: ./deploy.sh dev

set -e

# Default to dev environment if not specified
ENVIRONMENT=${1:-dev}
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )"
PROJECT_ROOT="$SCRIPT_DIR/../.."

# Validate environment
if [[ ! "$ENVIRONMENT" =~ ^(dev|staging|prod)$ ]]; then
  echo "Invalid environment: $ENVIRONMENT"
  echo "Valid environments: dev, staging, prod"
  exit 1
fi

echo "Deploying CosmoCargo to $ENVIRONMENT environment..."

# Load environment-specific variables
if [ -f "$SCRIPT_DIR/env/$ENVIRONMENT.env" ]; then
  source "$SCRIPT_DIR/env/$ENVIRONMENT.env"
else
  echo "Environment file not found: $SCRIPT_DIR/env/$ENVIRONMENT.env"
  exit 1
fi

# Copy docker-compose file to server
echo "Copying docker-compose file to server..."
scp "$PROJECT_ROOT/docker-compose.prod.yml" "$DEPLOY_USER@$DEPLOY_HOST:~/cosmocargo/"

# Deploy to server
echo "Deploying to server..."
ssh "$DEPLOY_USER@$DEPLOY_HOST" "cd ~/cosmocargo && \
  echo 'POSTGRES_PASSWORD=$DB_PASSWORD' > .env && \
  echo 'JWT_KEY=$JWT_KEY' >> .env && \
  docker-compose -f docker-compose.prod.yml pull && \
  docker-compose -f docker-compose.prod.yml up -d --force-recreate"

echo "Verifying deployment..."
ssh "$DEPLOY_USER@$DEPLOY_HOST" "cd ~/cosmocargo && \
  docker-compose -f docker-compose.prod.yml ps && \
  echo 'Deployment completed successfully!'"

echo "Deployment to $ENVIRONMENT completed successfully!" 