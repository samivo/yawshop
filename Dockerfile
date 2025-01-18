# Use the latest Ubuntu base image
FROM ubuntu:latest

RUN apt-get update && \
    apt-get install -y --no-install-recommends libicu-dev && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

# Set a working directory inside the container
WORKDIR /app

# Copy your single-file executable into the container
# Replace 'YourAppFileName' with the actual filename of your single-file application
COPY bin/Release/net8.0/linux-x64/publish/YawShop /app/YawShop
COPY Frontend/dist /app/Frontend/dist

# Grant execution permissions to the single-file application
RUN chmod +x /app/YawShop

# Expose any necessary ports (if your app uses a web server)
EXPOSE 5000

# Run the single-file application as the default command
ENTRYPOINT ["/app/YawShop"]