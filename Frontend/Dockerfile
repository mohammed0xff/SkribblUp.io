# Use an official Node runtime as a parent image
FROM node:14-alpine

# Expose port 5000 (default port for Vite)
EXPOSE 5000

# Set the working directory to /app
WORKDIR /app

# Copy the package.json and package-lock.json file to the working directory
COPY package.json package-lock.json ./
COPY vite.config.js ./

# Install dependencies
RUN npm i

# Install xdg-utils package
# This package provides the xdg-open command and other utilities
# for working with file types and URLs. ( required for vite ).
RUN apk add --no-cache xdg-utils

# Copy the rest of the application code to the working directory
COPY . .

# Build the application
RUN npm run build

# Start the application
CMD ["npm", "run", "preview"]