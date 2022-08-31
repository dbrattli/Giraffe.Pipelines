# Giraffe.Pipelines

[![Build and Test](https://github.com/dbrattli/Giraffe.Pipelines/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/dbrattli/Giraffe.Pipelines/actions/workflows/build-and-test.yml)
[![Nuget](https://img.shields.io/nuget/vpre/Giraffe.Pipelines)](https://www.nuget.org/packages/Giraffe.Pipelines/)


Functional pipelining for the
[Giraffe](https://github.com/giraffe-fsharp/Giraffe) ASP.NET Core micro
web framework. This library enables you to write Giraffe HTTP handler
pipelines using normal F# pipes (`|>`) instead of Kleisli composition
(`>=>`). Kleisli composition can be problematic for several reasons:

- Kleisli composition is a complex abstraction that can be hard to
  understand.
- Having to use functional composition as the main abstraction for a
  library is generally a bad idea.
- Serveral libraries using Kleisli composition are incompatible with
  each other. Thus you will get very confusing error messages.
- The IDE support is not as good as with pipes.

Giraffe.Pipelines fixes all this by letting you use normal F# pipes
(`|>`) and normal functional composition (`>=>`) in your Giraffe
pipelines.

Now you may enjoy your favorite F# library using a simpler and familiar
syntax.

## Installation

```console
> dotnet add package Giraffe.Pipelines
```

## Usage

Pipelines are started using the normal Giraffe pipeline syntax e.g.
`route "/ping"`. The pipeline is then transformed by piping the handler
into operators from the `Giraffe.Pipelines` library e.g `route "/ping"
|> HttpHandler.text "pong"`.

Here is the minimal self-contained example:

```fsharp
open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

let webApp =
    choose [
        route "/ping" |> HttpHandle.text "pong"
        route "/"     |> HttpHandle.htmlFile "/pages/index.html" ]

type Startup() =
    member __.ConfigureServices (services : IServiceCollection) =
        // Register default Giraffe dependencies
        services.AddGiraffe() |> ignore

    member __.Configure (app : IApplicationBuilder)
                        (env : IHostEnvironment)
                        (loggerFactory : ILoggerFactory) =
        // Add Giraffe to the ASP.NET Core pipeline
        app.UseGiraffe webApp

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseStartup<Startup>()
                    |> ignore)
        .Build()
        .Run()
    0
```
