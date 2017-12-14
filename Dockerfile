FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
EXPOSE 80
ENTRYPOINT ["dotnet", "CrowdSpark.Web.dll"]
