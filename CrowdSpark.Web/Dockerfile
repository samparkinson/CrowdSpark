FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "CrowdSpark.Web.dll"]