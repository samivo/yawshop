FROM debian:bullseye-slim

RUN apt-get update && apt-get install -y libstdc++6 libicu67 curl && apt-get clean

WORKDIR /app

COPY bin/Release/net8.0/linux-x64/publish/YawShop /app/YawShop
COPY Frontend/dist /app/Frontend/dist
RUN chmod +x /app/YawShop

EXPOSE 5000

ENTRYPOINT ["/app/YawShop"]
