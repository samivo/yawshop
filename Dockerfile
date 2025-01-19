FROM almalinux:9-minimal

RUN microdnf update -y && \
    microdnf install libicu -y && \
    microdnf clean all

WORKDIR /app

COPY bin/Release/net8.0/linux-x64/publish/YawShop /app/YawShop
COPY Frontend/dist /app/Frontend/dist

RUN chmod +x /app/YawShop

EXPOSE 5000

ENTRYPOINT ["/app/YawShop"]
