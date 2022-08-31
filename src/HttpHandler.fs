namespace Giraffe

open System.Globalization
open Microsoft.AspNetCore.Http

open Giraffe.ViewEngine.HtmlElements

[<RequireQualifiedAccess>]
module HttpHandler =
    // Core

    let inline handleContext (contextMap: HttpContext -> HttpFuncResult) (source: HttpHandler): HttpHandler =
        source >=> handleContext contextMap

    let inline choose handlers (source: HttpHandler): HttpHandler = source >=> choose handlers

    let inline GET(source: HttpHandler): HttpHandler = source >=> GET
    let inline POST(source: HttpHandler): HttpHandler = source >=> POST
    let inline PUT(source: HttpHandler): HttpHandler = source >=> PUT
    let inline PATCH(source: HttpHandler): HttpHandler = source >=> PATCH
    let inline DELETE(source: HttpHandler): HttpHandler = source >=> DELETE
    let inline HEAD(source: HttpHandler): HttpHandler = source >=> HEAD
    let inline OPTIONS(source: HttpHandler): HttpHandler = source >=> OPTIONS
    let inline TRACE(source: HttpHandler): HttpHandler = source >=> TRACE
    let inline CONNECT(source: HttpHandler): HttpHandler = source >=> CONNECT

    let inline GET_HEAD(source: HttpHandler): HttpHandler = source >=> GET_HEAD
    let inline clearResponse (source: HttpHandler): HttpHandler = source >=> clearResponse
    let inline setContentType contentType (source: HttpHandler): HttpHandler = source >=> setContentType contentType
    let inline setStatusCode (statusCode: int) (source: HttpHandler): HttpHandler = source >=> setStatusCode statusCode
    let inline setHttpHeader (key: string) (value: obj) (source: HttpHandler): HttpHandler =
        source >=> setHttpHeader key value
    let inline mustAccept (mimeTypes: string list) (source: HttpHandler): HttpHandler = source >=> mustAccept mimeTypes
    let inline redirectTo (permanent: bool) (location: string) (source: HttpHandler): HttpHandler =
        source >=> redirectTo permanent location
    let inline bindJson<'T> (f: 'T -> HttpHandler) (source: HttpHandler): HttpHandler = source >=> bindJson<'T> f
    let inline bindXml<'T> (f: 'T -> HttpHandler) (source: HttpHandler): HttpHandler = source >=> bindXml<'T> f
    let inline bindForm<'T> (culture: CultureInfo option) (f: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> bindForm<'T> culture f
    let inline tryBindForm<'T> (parsingErrorHandler: string -> HttpHandler) (culture: CultureInfo option)
               (successHandler: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> tryBindForm<'T> parsingErrorHandler culture successHandler
    let inline bindQuery<'T> (culture: CultureInfo option) (f: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> bindQuery<'T> culture f
    let inline tryBindQuery<'T> (parsingErrorHandler: string -> HttpHandler) (culture: CultureInfo option)
               (successHandler: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> tryBindQuery<'T> parsingErrorHandler culture successHandler
    let inline bindModel<'T> (culture: CultureInfo option) (f: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> bindModel<'T> culture f
    let inline setBody (bytes: byte array) (source: HttpHandler): HttpHandler = source >=> setBody bytes
    let inline setBodyFromString (str: string) (source: HttpHandler): HttpHandler = source >=> setBodyFromString str

    /// <summary>
    /// Writes an UTF-8 encoded string to the body of the HTTP response and sets the HTTP Content-Length header
    /// accordingly, as well as the Content-Type header to text/plain.
    /// </summary>
    /// <param name="str">The string value to be send back to the client.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>
    /// A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.
    /// </returns>
    let inline text (str: string) (source: HttpHandler): HttpHandler = source >=> text str

    let inline json<'T> (dataObj: 'T) (source: HttpHandler): HttpHandler = source >=> json dataObj
    let inline jsonChunked<'T> (dataObj: 'T) (source: HttpHandler): HttpHandler = source >=> jsonChunked<'T> dataObj
    let inline xml (dataObj: obj) (source: HttpHandler): HttpHandler = source >=> xml dataObj
    let inline htmlFile (filePath: string) (source: HttpHandler): HttpHandler = source >=> htmlFile filePath
    let inline htmlString (html: string) (source: HttpHandler): HttpHandler = source >=> htmlString html
    let inline htmlView (htmlView: XmlNode) (source: HttpHandler): HttpHandler = source >=> Core.htmlView htmlView

    // Routing

    /// <summary>
    /// Filters an incoming HTTP request based on the port.
    /// </summary>
    /// <param name="fns">List of port to <see cref="HttpHandler"/> mappings</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routePorts (fns: (int * HttpHandler) list) (source: HttpHandler): HttpHandler =
        source >=> routePorts fns

    /// <summary>
    /// Filters an incoming HTTP request based on the request path (case sensitive).
    /// </summary>
    /// <param name="path">Request path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline route (path: string) (source: HttpHandler): HttpHandler = source >=> route path

    /// <summary>
    /// Filters an incoming HTTP request based on the request path (case insensitive).
    /// </summary>
    /// <param name="path">Request path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeCi (path: string) (source: HttpHandler): HttpHandler = source >=> routeCi path

    /// <summary>
    /// Filters an incoming HTTP request based on the request path using Regex (case sensitive).
    /// </summary>
    /// <param name="path">Regex path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routex (path: string) (source: HttpHandler): HttpHandler = source >=> routex path

    /// <summary>
    /// Filters an incoming HTTP request based on the request path using Regex (case sensitive).
    ///
    /// If the route matches the incoming HTTP request then the Regex groups will be passed into the supplied `routeHandler`.
    ///
    /// This is similar to routex but also allows to use matched strings as parameters for a controller.
    /// </summary>
    /// <param name="path">Regex path.</param>
    /// <param name="routeHandler">A function which accepts a string sequence of the matched groups and returns a `HttpHandler` function which will subsequently deal with the request.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routexp (path: string) (routeHandler: seq<string> -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> routexp path routeHandler

    // <summary>
    /// Filters an incoming HTTP request based on the request path using Regex (case insensitive).
    /// </summary>
    /// <param name="path">Regex path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeCix (path: string) (source: HttpHandler): HttpHandler = source >=> routeCix path

    // <summary>
    /// Filters an incoming HTTP request based on the request path (case sensitive).
    /// If the route matches the incoming HTTP request then the arguments from the <see cref="Microsoft.FSharp.Core.PrintfFormat"/> will be automatically resolved and passed into the supplied routeHandler.
    ///
    /// Supported format chars**
    ///
    /// %b: bool
    /// %c: char
    /// %s: string
    /// %i: int
    /// %d: int64
    /// %f: float/double
    /// %O: Guid
    /// </summary>
    /// <param name="path">A format string representing the expected request path.</param>
    /// <param name="routeHandler">A function which accepts a tuple 'T of the parsed arguments and returns a <see cref="HttpHandler"/> function which will subsequently deal with the request.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routef (path: PrintfFormat<_, _, _, _, 'T>) (routeHandler: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> routef path routeHandler

    /// <summary>
    /// Filters an incoming HTTP request based on the request path.
    /// If the route matches the incoming HTTP request then the arguments from the <see cref="Microsoft.FSharp.Core.PrintfFormat"/> will be automatically resolved and passed into the supplied routeHandler.
    ///
    /// Supported format chars**
    ///
    /// %b: bool
    /// %c: char
    /// %s: string
    /// %i: int
    /// %d: int64
    /// %f: float/double
    /// %O: Guid
    /// </summary>
    /// <param name="path">A format string representing the expected request path.</param>
    /// <param name="routeHandler">A function which accepts a tuple 'T of the parsed arguments and returns a <see cref="HttpHandler"/> function which will subsequently deal with the request.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeCif (path: PrintfFormat<_, _, _, _, 'T>) (routeHandler: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> routeCif path routeHandler

    /// <summary>
    /// Filters an incoming HTTP request based on the request path (case insensitive).
    /// If the route matches the incoming HTTP request then the parameters from the string will be used to create an instance of 'T and subsequently passed into the supplied routeHandler.
    /// </summary>
    /// <param name="route">A string representing the expected request path. Use {propertyName} for reserved parameter names which should map to the properties of type 'T. You can also use valid Regex within the route string.</param>
    /// <param name="routeHandler">A function which accepts a tuple 'T of the parsed parameters and returns a <see cref="HttpHandler"/> function which will subsequently deal with the request.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeBind<'T> (route: string) (routeHandler: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> routeBind<'T> route routeHandler

    /// <summary>
    /// Filters an incoming HTTP request based on the beginning of the request path (case sensitive).
    /// </summary>
    /// <param name="subPath">The expected beginning of a request path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    // <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeStartsWith (subPath: string) (source: HttpHandler): HttpHandler =
        source >=> routeStartsWith subPath

    /// <summary>
    /// Filters an incoming HTTP request based on the beginning of the request path (case insensitive).
    /// </summary>
    /// <param name="subPath">The expected beginning of a request path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeStartsWithCi (subPath: string) (source: HttpHandler): HttpHandler =
        source >=> routeStartsWithCi subPath

    // <summary>
    /// Filters an incoming HTTP request based on the beginning of the request path (case sensitive).
    /// If the route matches the incoming HTTP request then the arguments from the <see cref="Microsoft.FSharp.Core.PrintfFormat"/> will be automatically resolved and passed into the supplied routeHandler.
    ///
    /// Supported format chars**
    ///
    /// %b: bool
    /// %c: char
    /// %s: string
    /// %i: int
    /// %d: int64
    /// %f: float/double
    /// %O: Guid
    /// </summary>
    /// <param name="path">A format string representing the expected request path.</param>
    /// <param name="routeHandler">A function which accepts a tuple 'T of the parsed arguments and returns a <see cref="HttpHandler"/> function which will subsequently deal with the request.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeStartsWithf (path: PrintfFormat<_, _, _, _, 'T>) (routeHandler: 'T -> HttpHandler)
               (source: HttpHandler): HttpHandler = source >=> routeStartsWithf path routeHandler

    // <summary>
    /// Filters an incoming HTTP request based on the beginning of the request path (case insensitive).
    /// If the route matches the incoming HTTP request then the arguments from the <see cref="Microsoft.FSharp.Core.PrintfFormat"/> will be automatically resolved and passed into the supplied routeHandler.
    ///
    /// Supported format chars**
    ///
    /// %b: bool
    /// %c: char
    /// %s: string
    /// %i: int
    /// %d: int64
    /// %f: float/double
    /// %O: Guid
    /// </summary>
    /// <param name="path">A format string representing the expected request path.</param>
    /// <param name="routeHandler">A function which accepts a tuple 'T of the parsed arguments and returns a <see cref="HttpHandler"/> function which will subsequently deal with the request.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeStartsWithCif (path: PrintfFormat<_, _, _, _, 'T>) (routeHandler: 'T -> HttpHandler)
               (source: HttpHandler): HttpHandler = source >=> routeStartsWithCif path routeHandler

    // <summary>
    /// Filters an incoming HTTP request based on a part of the request path (case sensitive).
    /// Subsequent route handlers inside the given handler function should omit the already validated path.
    /// </summary>
    /// <param name="path">A part of an expected request path.</param>
    /// <param name="handler">A Giraffe <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let subRoute (path: string) (handler: HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> subRoute path handler

    /// <summary>
    /// Filters an incoming HTTP request based on a part of the request path (case insensitive).
    /// Subsequent route handlers inside the given handler function should omit the already validated path.
    /// </summary>
    /// <param name="path">A part of an expected request path.</param>
    /// <param name="handler">A Giraffe <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let subRouteCi (path: string) (handler: HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> subRouteCi path handler

    /// <summary>
    /// Filters an incoming HTTP request based on a part of the request path (case sensitive).
    /// If the sub route matches the incoming HTTP request then the arguments from the <see cref="Microsoft.FSharp.Core.PrintfFormat"/> will be automatically resolved and passed into the supplied routeHandler.
    ///
    /// Supported format chars
    ///
    /// %b: bool
    /// %c: char
    /// %s: string
    /// %i: int
    /// %d: int64
    /// %f: float/double
    /// %O: Guid
    ///
    /// Subsequent routing handlers inside the given handler function should omit the already validated path.
    /// </summary>
    /// <param name="path">A format string representing the expected request sub path.</param>
    /// <param name="routeHandler">A function which accepts a tuple 'T of the parsed arguments and returns a <see cref="HttpHandler"/> function which will subsequently deal with the request.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let subRoutef (path: PrintfFormat<_, _, _, _, 'T>) (routeHandler: 'T -> HttpHandler) (source: HttpHandler): HttpHandler =
        source >=> subRoutef path routeHandler
