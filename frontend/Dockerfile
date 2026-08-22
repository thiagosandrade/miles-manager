# ============================================================
# Build Stage
# ============================================================

FROM node:22-alpine AS build

WORKDIR /app

# Copy dependencies first (better Docker cache)
COPY package*.json ./

# Install dependencies
RUN npm ci

# Copy application
COPY . .

# Build Angular
RUN npm run build -- --configuration production

# ============================================================
# Runtime Stage
# ============================================================

FROM nginx:1.27-alpine

# Custom Nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Angular application
COPY --from=build /app/dist/milesmanager-frontend/browser/ /usr/share/nginx/html

EXPOSE 80