# Use the latest Alpine base image
FROM alpine:latest

# Install libicu and bash
RUN apk add --no-cache libicu bash && \
    apk add --no-cache libstdc++ # Required for .NET runtime dependencies

# Set a working directory inside the container
WORKDIR /app

# Copy your single-file executable into the container
COPY bin/Release/net8.0/linux-x64/publish/YawShop /app/YawShop
COPY Frontend/dist /app/Frontend/dist

# Grant execution permissions to the single-file application
RUN chmod +x /app/YawShop

# Expose any necessary ports (if your app uses a web server)
EXPOSE 5000

# Run the single-file application as the default command
ENTRYPOINT ["/bin/bash"]