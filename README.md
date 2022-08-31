# Giraffe.Pipelines

[![Build and Test](https://github.com/dbrattli/Giraffe.Pipelines/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/dbrattli/Giraffe.Pipelines/actions/workflows/build-and-test.yml)
[![Nuget](https://img.shields.io/nuget/vpre/Giraffe.Pipelines)](https://www.nuget.org/packages/Giraffe.Pipelines/)


Functional pipelining for the
[Giraffe](https://github.com/giraffe-fsharp/Giraffe) ASP.NET Core micro
web framework. This library enables you to write Giraffe HTTP handler
pipelines using normal F# pipes (`|>`) instead of Kleisli composition
(`>=>`). Kleisli composition can be problematic for several reasons:

- Everyone in F# understands the `|>` operator, but most do not
  understand `>=>`.
- Having to use functional or Kleisli composition as the main
  abstraction for a library is generally a bad idea.
- Having serveral libraries using different `>=>` are incompatible with
  each other. You will get very confusing error messages.
- The IDE support and type hints are not as good as with the `|>`
  operator.

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
into "operators" from the `HttpHandler` from the `Giraffe.Pipelines`
library, e.g `route "/ping" |> HttpHandler.text "pong"`. These operators
have the type `HttpHandler -> HttpHandler` and can be used to transform
the pipeline.

```fsharp
let someHttpHandler: HttpHandler =
    setStatusCode 200
    |> HttpHandler.text "Hello World"
```

You may even compose two `HttpHandler` pipeline aware operators together 
using normal functional composition `>>` operator to create your own 
pipeline aware operators.

```fsharp
let someHttpOperator: HttpHandler -> HttpHandler =
    HttpHandler.setStatusCode 200
    >> HttpHandler.text "Hello World"
```

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
        route "/ping" |> HttpHandler.text "pong"
        route "/"     |> HttpHandler.htmlFile "/pages/index.html" ]

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
