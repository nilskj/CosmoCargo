FROM node:18-alpine AS base

FROM base AS deps
WORKDIR /app
COPY package.json package-lock.json* ./
RUN npm ci

# Skapa public-katalogen och nödvändiga filer
RUN mkdir -p public
RUN echo "User-agent: *\nAllow: /" > public/robots.txt
RUN echo '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100" width="100" height="100"><circle cx="50" cy="50" r="45" fill="#1E3A8A" /><path d="M30,30 L70,30 L70,70 L30,70 Z" fill="none" stroke="white" stroke-width="3" /><circle cx="50" cy="50" r="10" fill="#BE185D" /><path d="M20,20 L80,80" stroke="white" stroke-width="2" /><path d="M80,20 L20,80" stroke="white" stroke-width="2" /></svg>' > public/logo.svg
RUN echo '{"name":"CosmoCargo","short_name":"CosmoCargo","icons":[{"src":"/favicon.ico","sizes":"64x64 32x32 24x24 16x16","type":"image/x-icon"},{"src":"/logo.svg","type":"image/svg+xml","sizes":"192x192 512x512"}],"start_url":"/","display":"standalone","theme_color":"#1E3A8A","background_color":"#0F172A"}' > public/manifest.json
# Skapa en tom favicon.ico
RUN touch public/favicon.ico

FROM base AS builder
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY --from=deps /app/public ./public
COPY . .
RUN npm run build

FROM base AS runner
WORKDIR /app

ENV NODE_ENV production

COPY --from=builder /app/public ./public
COPY --from=builder /app/.next ./.next
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/package.json ./package.json

EXPOSE 3000

ENV PORT 3000

CMD ["npm", "start"] 