FROM microsoft/aspnet:1.0.0-rc1-update1

COPY . /app
WORKDIR /app/JD.TreadHud.Api
RUN ["dnu", "restore"]

EXPOSE 5000/tcp
ENTRYPOINT ["dnx", "web", "--server.urls", "http://0.0.0.0:5000"]
