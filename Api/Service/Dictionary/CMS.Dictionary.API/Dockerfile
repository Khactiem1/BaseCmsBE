#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# docker build -f Api/Service/Dictionary/CMS.Dictionary.API/Dockerfile -t cms_be_dictionary D:/Freelancer/CMS/apicms

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api/Service/Dictionary/CMS.Dictionary.API/CMS.Dictionary.API.csproj", "Api/Service/Dictionary/CMS.Dictionary.API/"]
COPY ["PlatformCore/CMS.Core.Web/CMS.Core.Web.csproj", "PlatformCore/CMS.Core.Web/"]
COPY ["Api/BL/Cms.BL/Cms.BL.csproj", "Api/BL/Cms.BL/"]
COPY ["Api/DL/Cms.DL/Cms.DL.csproj", "Api/DL/Cms.DL/"]
COPY ["PlatformCore/CMS.Core.Database/CMS.Core.Database.csproj", "PlatformCore/CMS.Core.Database/"]
COPY ["Api/Library/Cms.Core.Common/Cms.Core.Common.csproj", "Api/Library/Cms.Core.Common/"]
COPY ["Api/Model/Cms.Model/Cms.Model.csproj", "Api/Model/Cms.Model/"]

RUN dotnet restore "./Api/Service/Dictionary/CMS.Dictionary.API/CMS.Dictionary.API.csproj"
COPY . .

# Ghi đè appsettings.json từ Config vào đúng thư mục
COPY ["Config/appsettings.json", "/src/Api/Service/Dictionary/CMS.Dictionary.API/appsettings.json"]

WORKDIR "/src/Api/Service/Dictionary/CMS.Dictionary.API"
RUN dotnet build "./CMS.Dictionary.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CMS.Dictionary.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN ls -lta
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CMS.Dictionary.API.dll"]
