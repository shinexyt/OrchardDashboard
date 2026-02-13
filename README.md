# Orchard Core Framework Samples

Sample web applications demonstrating how to build a Modular ASP.NET Core application using the Orchard Core Framework.

You can watch a video providing a step by step demonstration of building a modular, multi-tenant Orchard Core Framework application here https://www.youtube.com/watch?v=yrQaKv2mxFU&list=PLReL099Y5nRd04p81Q7p5TtyjCrj9tz1t. It was presented at .NET Conf 2019 using Orchard Core RC1. The names of the C# projects in the demo do not match these sample projects, but the demonstration is very similar to these samples.

## Contents of the Solution

### DashboardApplication

An ASP.NET Core MVC application with Orchard Admin UI that references the modules projects.

The application has minimal features and dependencies. There are users, roles and features management in admin UI. The sample contains a customizable minimal setup recipe.

### Module1

A sample module containing ASP.NET Core MVC controllers, views, and pages.

### Module2

A sample module that registers custom middleware.

## Running

### From Visual Studio 2022

Open the `OrchardCore.Samples.sln` solution file and run either application and visit its homepage (any project with a name ending in "Application"). 

Open the website in your browser, and use the URLs or links it provides to explore.

### From the Command Line

Open either Web application folder, `ModularApplication` or `MultiTenantApplication`, then run these commands:

- `dotnet restore`
- `dotnet build`
- `dotnet run`


## Creating new Modules

Modules can be .NET Standard 2.0 class libraries or .NET Core 3.0 class libraries that reference the **OrchardCore.Module.Targets** Nuget Package.

If you need it, development Orchard Core Nuget packages are available in a MyGet feed at this url: `https://nuget.cloudsmith.io/orchardcore/preview/v3/index.json`

Optionally, modules can be packaged as Nuget packages and made available on Nuget or MyGet, including static files and views.
The Orchard Core CMS builds upon the Orchard Core Framework.
More examples of modules can be found for the Orchard Core CMS in this repository: https://github.com/OrchardCMS/OrchardCore

## Creating New Modular or Multi-Tenant Applications

A modular application that hosts module only needs to reference one of these targets packages:

- **OrchardCore.Application.Targets**: Allows the application to reference and import modules, and optionally use multi-tenancy.
- **OrchardCore.Application.Mvc.Targets**: Same as **OrchardCore.Application.Targets** but also references the **OrchardCore.Mvc** module
- **OrchardCore.Application.Nancy.Targets**: Same as **OrchardCore.Application.Targets** but also references the **OrchardCore.Nancy** module
