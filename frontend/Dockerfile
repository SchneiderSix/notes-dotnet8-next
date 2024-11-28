# Use an official Node.js runtime as a parent image
FROM node:18 AS builder

# Set the working directory in the container
WORKDIR /app

# Copy package.json and package-lock.json (or yarn.lock) to the working directory
COPY package.json package-lock.json ./

# Install dependencies
RUN npm install

# Copy the rest of your application code
COPY . .

# Build the Next.js app
RUN npm run build

# Use a lightweight web server for serving the app
FROM node:18 AS runner

# Set the working directory in the runner container
WORKDIR /app

# Copy the build output and necessary files from the builder stage
COPY --from=builder /app ./

# Install only the production dependencies
RUN npm ci --only=production

# Expose the port the app runs on
EXPOSE 3000

# Start the Next.js app
CMD ["npm", "start"]