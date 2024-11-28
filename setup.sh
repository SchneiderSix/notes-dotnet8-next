#!/bin/bash

# Ensure script is executed with superuser privileges
if [[ $EUID -ne 0 ]]; then
   echo "This script must be run as root. Use sudo to run it."
   exit 1
fi

# Function to install Docker
install_docker() {
    echo "Updating package index..."
    apt-get update -y || { echo "Failed to update package index"; exit 1; }

    echo "Installing prerequisites for Docker..."
    apt-get install -y apt-transport-https ca-certificates curl software-properties-common || { echo "Failed to install prerequisites"; exit 1; }

    echo "Adding Docker's official GPG key..."
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg || { echo "Failed to add Docker GPG key"; exit 1; }

    echo "Setting up the stable Docker repository..."
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null || { echo "Failed to add Docker repository"; exit 1; }

    echo "Updating package index again..."
    apt-get update -y || { echo "Failed to update package index"; exit 1; }

    echo "Installing Docker..."
    apt-get install -y docker-ce docker-ce-cli containerd.io || { echo "Failed to install Docker"; exit 1; }

    echo "Docker installation complete."
}

# Function to install Docker Compose
install_docker_compose() {
    echo "Downloading Docker Compose..."
    curl -L "https://github.com/docker/compose/releases/download/v2.22.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose || { echo "Failed to download Docker Compose"; exit 1; }

    echo "Making Docker Compose executable..."
    chmod +x /usr/local/bin/docker-compose || { echo "Failed to set permissions for Docker Compose"; exit 1; }

    echo "Docker Compose installation complete."
}

# Function to initialize Docker
initialize_docker() {
    echo "Starting Docker service..."
    systemctl start docker || { echo "Failed to start Docker service"; exit 1; }

    echo "Enabling Docker service to start on boot..."
    systemctl enable docker || { echo "Failed to enable Docker service"; exit 1; }

    echo "Adding the current user to the Docker group..."
    usermod -aG docker $USER || { echo "Failed to add user to Docker group"; exit 1; }

    echo "Docker initialized successfully. Please log out and log back in for changes to take effect."
}

# Function to run docker-compose command
run_docker_compose() {
    echo "Running 'docker-compose up --build'..."
    docker-compose up --build || { echo "Failed to execute docker-compose command"; exit 1; }
}

# Main script execution
echo "Starting the script to install and initialize Docker..."

install_docker
install_docker_compose
initialize_docker

echo "Docker and Docker Compose are installed and initialized."

run_docker_compose