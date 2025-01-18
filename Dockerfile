# Use a specific Alpine version to ensure compatibility
FROM alpine:3.17

# Update repositories and install necessary packages
RUN apk update && \
    apk add --no-cache icu bash libstdc++

# Set a working directory
WORKDIR /app

# Copy the application
COPY bin/Release/net8.0/linux-x64/publish/YawShop /app/YawShop
COPY Frontend/dist /app/Frontend/dist

# Grant execution permissions
RUN chmod +x /app/YawShop

# Expose any necessary ports
EXPOSE 5000

# Set the default entrypoint to bash
ENTRYPOINT ["/bin/bash"]
