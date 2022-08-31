module Giraffe.Tests.PipelineTests

open System
open System.IO
open System.Collections.Generic
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Xunit
open NSubstitute
open Giraffe

// ---------------------------------
// Test Types
// ---------------------------------

type Dummy =
    {
        Foo : string
        Bar : string
        Age : int
    }

[<Theory>]
[<MemberData("DefaultData", MemberType = typedefof<JsonSerializersData>)>]
let ``GET "/json" returns json object`` (settings) =
    let ctx = Substitute.For<HttpContext>()
    mockJson ctx settings
    let app =
        GET |> HttpHandler.choose [
            route "/"     |> HttpHandler.text "Hello World"
            route "/foo"  |> HttpHandler.text "bar"
            route "/json" |> HttpHandler.json { Foo = "john"; Bar = "doe"; Age = 30 }
            setStatusCode 404 |> HttpHandler.text "Not found" ]

    ctx.Request.Method.ReturnsForAnyArgs "GET" |> ignore
    ctx.Request.Path.ReturnsForAnyArgs (PathString("/json")) |> ignore
    ctx.Response.Body <- new MemoryStream()
    let expected = "{\"foo\":\"john\",\"bar\":\"doe\",\"age\":30}"

    task {
        let! result = app next ctx

        match result with
        | None     -> assertFailf "Result was expected to be %s" expected
        | Some ctx -> Assert.Equal(expected, getBody ctx)
    }

