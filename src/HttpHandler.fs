namespace Giraffe

open System.Globalization
open Microsoft.AspNetCore.Http

open Giraffe.ViewEngine.HtmlElements

[<RequireQualifiedAccess>]
module HttpHandler =
    // Core

    /// <summary>
    /// The warbler function is a <see cref="HttpHandler"/> wrapper function which prevents a <see cref="HttpHandler"/> to be pre-evaluated at startup.
    /// </summary>
    /// <param name="f">A function which takes a HttpFunc * HttpContext tuple and returns a <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <example>
    /// <code>
    /// warbler(fun _ -> someHttpHandler)
    /// </code>
    /// </example>
    /// <returns>Returns a <see cref="HttpHandler"/> function.</returns>
    let inline warbler f (source: HttpHandler) : HttpHandler = source >=> warbler f

    /// <summary>
    /// The handleContext function is a convenience function which can be used to create a new <see cref="HttpHandler"/> function which only requires access to the <see cref="Microsoft.AspNetCore.Http.HttpContext"/> object.
    /// </summary>
    /// <param name="contextMap">A function which accepts a <see cref="Microsoft.AspNetCore.Http.HttpContext"/> object and returns a <see cref="HttpFuncResult"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline handleContext (contextMap: HttpContext -> HttpFuncResult) (source: HttpHandler) : HttpHandler =
        source >=> handleContext contextMap
    /// <summary>
    /// Iterates through a list of <see cref="HttpHandler"/> functions and returns the result of the first <see cref="HttpHandler"/> of which the outcome is Some HttpContext.
    /// Please mind that all <see cref="HttpHandler"/> functions will get pre-evaluated at runtime by applying the next (HttpFunc) parameter to each handler.
    /// </summary>
    /// <param name="handlers"></param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A <see cref="HttpFunc"/>.</returns>

    let inline choose handlers (source: HttpHandler) : HttpHandler = source >=> choose handlers

    let inline GET (source: HttpHandler) : HttpHandler = source >=> GET
    let inline POST (source: HttpHandler) : HttpHandler = source >=> POST
    let inline PUT (source: HttpHandler) : HttpHandler = source >=> PUT
    let inline PATCH (source: HttpHandler) : HttpHandler = source >=> PATCH
    let inline DELETE (source: HttpHandler) : HttpHandler = source >=> DELETE
    let inline HEAD (source: HttpHandler) : HttpHandler = source >=> HEAD
    let inline OPTIONS (source: HttpHandler) : HttpHandler = source >=> OPTIONS
    let inline TRACE (source: HttpHandler) : HttpHandler = source >=> TRACE
    let inline CONNECT (source: HttpHandler) : HttpHandler = source >=> CONNECT

    let inline GET_HEAD (source: HttpHandler) : HttpHandler = source >=> GET_HEAD

    /// <summary>
    /// Clears the current <see cref="Microsoft.AspNetCore.Http.HttpResponse"/> object.
    /// This can be useful if a <see cref="HttpHandler"/> function needs to overwrite the response of all previous <see cref="HttpHandler"/> functions with its own response (most commonly used by an <see cref="ErrorHandler"/> function).
    /// </summary>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline clearResponse (source: HttpHandler) : HttpHandler = source >=> clearResponse

    /// <summary>
    /// Sets the Content-Type HTTP header in the response.
    /// </summary>
    /// <param name="contentType">The mime type of the response (e.g.: application/json or text/html).</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline setContentType contentType (source: HttpHandler) : HttpHandler = source >=> setContentType contentType

    /// <summary>
    /// Sets the HTTP status code of the response.
    /// </summary>
    /// <param name="statusCode">The status code to be set in the response. For convenience you can use the static <see cref="Microsoft.AspNetCore.Http.StatusCodes"/> class for passing in named status codes instead of using pure int values.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline setStatusCode (statusCode: int) (source: HttpHandler) : HttpHandler = source >=> setStatusCode statusCode

    /// <summary>
    /// Adds or sets a HTTP header in the response.
    /// </summary>
    /// <param name="key">The HTTP header name. For convenience you can use the static <see cref="Microsoft.Net.Http.Headers.HeaderNames"/> class for passing in strongly typed header names instead of using pure string values.</param>
    /// <param name="value">The value to be set. Non string values will be converted to a string using the object's ToString() method.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline setHttpHeader (key: string) (value: obj) (source: HttpHandler) : HttpHandler =
        source >=> setHttpHeader key value

    /// <summary>
    /// Filters an incoming HTTP request based on the accepted mime types of the client (Accept HTTP header).
    /// If the client doesn't accept any of the provided mimeTypes then the handler will not continue executing the next <see cref="HttpHandler"/> function.
    /// </summary>
    /// <param name="mimeTypes">List of mime types of which the client has to accept at least one.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline mustAccept (mimeTypes: string list) (source: HttpHandler) : HttpHandler = source >=> mustAccept mimeTypes

    /// <summary>
    /// Redirects to a different location with a `302` or `301` (when permanent) HTTP status code.
    /// </summary>
    /// <param name="permanent">If true the redirect is permanent (301), otherwise temporary (302).</param>
    /// <param name="location">The URL to redirect the client to.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline redirectTo (permanent: bool) (location: string) (source: HttpHandler) : HttpHandler =
        source >=> redirectTo permanent location

    /// <summary>
    /// Parses a JSON payload into an instance of type 'T.
    /// </summary>
    /// <param name="f">A function which accepts an object of type 'T and returns a <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline bindJson<'T> (f: 'T -> HttpHandler) (source: HttpHandler) : HttpHandler = source >=> bindJson<'T> f

    /// <summary>
    /// Parses a XML payload into an instance of type 'T.
    /// </summary>
    /// <param name="f">A function which accepts an object of type 'T and returns a <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline bindXml<'T> (f: 'T -> HttpHandler) (source: HttpHandler) : HttpHandler = source >=> bindXml<'T> f

    /// <summary>
    /// Parses a HTTP form payload into an instance of type 'T.
    /// </summary>
    /// <param name="culture">An optional <see cref="System.Globalization.CultureInfo"/> element to be used when parsing culture specific data such as float, DateTime or decimal values.</param>
    /// <param name="f">A function which accepts an object of type 'T and returns a <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline bindForm<'T> (culture: CultureInfo option) (f: 'T -> HttpHandler) (source: HttpHandler) : HttpHandler =
        source >=> bindForm<'T> culture f

    /// <summary>
    /// Tries to parse a HTTP form payload into an instance of type 'T.
    /// </summary>
    /// <param name="parsingErrorHandler">A <see cref="System.String"/> -> <see cref="HttpHandler"/> function which will get invoked when the model parsing fails. The <see cref="System.String"/> parameter holds the parsing error message.</param>
    /// <param name="culture">An optional <see cref="System.Globalization.CultureInfo"/> element to be used when parsing culture specific data such as float, DateTime or decimal values.</param>
    /// <param name="successHandler">A function which accepts an object of type 'T and returns a <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline tryBindForm<'T>
        (parsingErrorHandler: string -> HttpHandler)
        (culture: CultureInfo option)
        (successHandler: 'T -> HttpHandler)
        (source: HttpHandler)
        : HttpHandler =
        source >=> tryBindForm<'T> parsingErrorHandler culture successHandler

    /// <summary>
    /// Parses a HTTP query string into an instance of type 'T.
    /// </summary>
    /// <param name="culture">An optional <see cref="System.Globalization.CultureInfo"/> element to be used when parsing culture specific data such as float, DateTime or decimal values.</param>
    /// <param name="f">A function which accepts an object of type 'T and returns a <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline bindQuery<'T> (culture: CultureInfo option) (f: 'T -> HttpHandler) (source: HttpHandler) : HttpHandler =
        source >=> bindQuery<'T> culture f

    /// <summary>
    /// Tries to parse a query string into an instance of type `'T`.
    /// </summary>
    /// <param name="parsingErrorHandler">A <see href="HttpHandler"/> function which will get invoked when the model parsing fails. The <see cref="System.String"/> input parameter holds the parsing error message.</param>
    /// <param name="culture">An optional <see cref="System.Globalization.CultureInfo"/> element to be used when parsing culture specific data such as float, DateTime or decimal values.</param>
    /// <param name="successHandler">A function which accepts an object of type 'T and returns a <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline tryBindQuery<'T>
        (parsingErrorHandler: string -> HttpHandler)
        (culture: CultureInfo option)
        (successHandler: 'T -> HttpHandler)
        (source: HttpHandler)
        : HttpHandler =
        source >=> tryBindQuery<'T> parsingErrorHandler culture successHandler

    /// <summary>
    /// Parses a HTTP payload into an instance of type 'T.
    /// The model can be sent via XML, JSON, form or query string.
    /// </summary>
    /// <param name="culture">An optional <see cref="System.Globalization.CultureInfo"/> element to be used when parsing culture specific data such as float, DateTime or decimal values.</param>
    /// <param name="f">A function which accepts an object of type 'T and returns a <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline bindModel<'T> (culture: CultureInfo option) (f: 'T -> HttpHandler) (source: HttpHandler) : HttpHandler =
        source >=> bindModel<'T> culture f

    /// <summary>
    /// Writes a byte array to the body of the HTTP response and sets the HTTP Content-Length header accordingly.
    /// </summary>
    /// <param name="bytes">The byte array to be send back to the client.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.</returns>
    let inline setBody (bytes: byte array) (source: HttpHandler) : HttpHandler = source >=> setBody bytes

    /// <summary>
    /// Writes an UTF-8 encoded string to the body of the HTTP response and sets the HTTP Content-Length header accordingly.
    /// </summary>
    /// <param name="str">The string value to be send back to the client.</param>
    /// <returns>A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.</returns>
    let inline setBodyFromString (str: string) (source: HttpHandler) : HttpHandler = source >=> setBodyFromString str

    /// <summary>
    /// Writes an UTF-8 encoded string to the body of the HTTP response and sets the HTTP Content-Length header
    /// accordingly, as well as the Content-Type header to text/plain.
    /// </summary>
    /// <param name="str">The string value to be send back to the client.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>
    /// A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.
    /// </returns>
    let inline text (str: string) (source: HttpHandler) : HttpHandler = source >=> text str

    /// <summary>
    /// Serializes an object to JSON and writes the output to the body of the HTTP response.
    /// It also sets the HTTP Content-Type header to application/json and sets the Content-Length header accordingly.
    /// The JSON serializer can be configured in the ASP.NET Core startup code by registering a custom class of type <see cref="Json.ISerializer"/>.
    /// </summary>
    /// <param name="dataObj">The object to be send back to the client.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <typeparam name="'T"></typeparam>
    /// <returns>A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.</returns>
    let inline json<'T> (dataObj: 'T) (source: HttpHandler) : HttpHandler = source >=> json dataObj

    /// <summary>
    /// Serializes an object to JSON and writes the output to the body of the HTTP response using chunked transfer encoding.
    /// It also sets the HTTP Content-Type header to application/json and sets the Transfer-Encoding header to chunked.
    /// The JSON serializer can be configured in the ASP.NET Core startup code by registering a custom class of type <see cref="Json.ISerializer"/>.
    /// </summary>
    /// <param name="dataObj">The object to be send back to the client.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.</returns>
    let inline jsonChunked<'T> (dataObj: 'T) (source: HttpHandler) : HttpHandler = source >=> jsonChunked<'T> dataObj

    /// <summary>
    /// Serializes an object to XML and writes the output to the body of the HTTP response.
    /// It also sets the HTTP Content-Type header to application/xml and sets the Content-Length header accordingly.
    /// The JSON serializer can be configured in the ASP.NET Core startup code by registering a custom class of type <see cref="Xml.ISerializer"/>.
    /// </summary>
    /// <param name="dataObj">The object to be send back to the client.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.</returns>
    let inline xml (dataObj: obj) (source: HttpHandler) : HttpHandler = source >=> xml dataObj

    /// <summary>
    /// Reads a HTML file from disk and writes its contents to the body of the HTTP response.
    /// It also sets the HTTP header Content-Type to text/html and sets the Content-Length header accordingly.
    /// </summary>
    /// <param name="filePath">A relative or absolute file path to the HTML file.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.</returns>
    let inline htmlFile (filePath: string) (source: HttpHandler) : HttpHandler = source >=> htmlFile filePath

    /// <summary>
    /// Writes a HTML string to the body of the HTTP response.
    /// It also sets the HTTP header Content-Type to text/html and sets the Content-Length header accordingly.
    /// </summary>
    /// <param name="html">The HTML string to be send back to the client.</param>
    /// <returns>A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.</returns>
    let inline htmlString (html: string) (source: HttpHandler) : HttpHandler = source >=> htmlString html

    /// <summary>
    /// <para>Compiles a `Giraffe.GiraffeViewEngine.XmlNode` object to a HTML view and writes the output to the body of the HTTP response.</para>
    /// <para>It also sets the HTTP header `Content-Type` to `text/html` and sets the `Content-Length` header accordingly.</para>
    /// </summary>
    /// <param name="htmlView">An `XmlNode` object to be send back to the client and which represents a valid HTML view.</param>
    /// <returns>A Giraffe `HttpHandler` function which can be composed into a bigger web application.</returns>
    let inline htmlView (htmlView: XmlNode) (source: HttpHandler) : HttpHandler = source >=> Core.htmlView htmlView

    // Routing

    /// <summary>
    /// Filters an incoming HTTP request based on the port.
    /// </summary>
    /// <param name="fns">List of port to <see cref="HttpHandler"/> mappings</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routePorts (fns: (int * HttpHandler) list) (source: HttpHandler) : HttpHandler =
        source >=> routePorts fns

    /// <summary>
    /// Filters an incoming HTTP request based on the request path (case sensitive).
    /// </summary>
    /// <param name="path">Request path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline route (path: string) (source: HttpHandler) : HttpHandler = source >=> route path

    /// <summary>
    /// Filters an incoming HTTP request based on the request path (case insensitive).
    /// </summary>
    /// <param name="path">Request path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeCi (path: string) (source: HttpHandler) : HttpHandler = source >=> routeCi path

    /// <summary>
    /// Filters an incoming HTTP request based on the request path using Regex (case sensitive).
    /// </summary>
    /// <param name="path">Regex path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routex (path: string) (source: HttpHandler) : HttpHandler = source >=> routex path

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
    let inline routexp (path: string) (routeHandler: seq<string> -> HttpHandler) (source: HttpHandler) : HttpHandler =
        source >=> routexp path routeHandler

    // <summary>
    /// Filters an incoming HTTP request based on the request path using Regex (case insensitive).
    /// </summary>
    /// <param name="path">Regex path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeCix (path: string) (source: HttpHandler) : HttpHandler = source >=> routeCix path

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
    let inline routef
        (path: PrintfFormat<_, _, _, _, 'T>)
        (routeHandler: 'T -> HttpHandler)
        (source: HttpHandler)
        : HttpHandler =
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
    let inline routeCif
        (path: PrintfFormat<_, _, _, _, 'T>)
        (routeHandler: 'T -> HttpHandler)
        (source: HttpHandler)
        : HttpHandler =
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
    let inline routeBind<'T> (route: string) (routeHandler: 'T -> HttpHandler) (source: HttpHandler) : HttpHandler =
        source >=> routeBind<'T> route routeHandler

    /// <summary>
    /// Filters an incoming HTTP request based on the beginning of the request path (case sensitive).
    /// </summary>
    /// <param name="subPath">The expected beginning of a request path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    // <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeStartsWith (subPath: string) (source: HttpHandler) : HttpHandler =
        source >=> routeStartsWith subPath

    /// <summary>
    /// Filters an incoming HTTP request based on the beginning of the request path (case insensitive).
    /// </summary>
    /// <param name="subPath">The expected beginning of a request path.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline routeStartsWithCi (subPath: string) (source: HttpHandler) : HttpHandler =
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
    let inline routeStartsWithf
        (path: PrintfFormat<_, _, _, _, 'T>)
        (routeHandler: 'T -> HttpHandler)
        (source: HttpHandler)
        : HttpHandler =
        source >=> routeStartsWithf path routeHandler

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
    let inline routeStartsWithCif
        (path: PrintfFormat<_, _, _, _, 'T>)
        (routeHandler: 'T -> HttpHandler)
        (source: HttpHandler)
        : HttpHandler =
        source >=> routeStartsWithCif path routeHandler

    // <summary>
    /// Filters an incoming HTTP request based on a part of the request path (case sensitive).
    /// Subsequent route handlers inside the given handler function should omit the already validated path.
    /// </summary>
    /// <param name="path">A part of an expected request path.</param>
    /// <param name="handler">A Giraffe <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline subRoute (path: string) (handler: HttpHandler) (source: HttpHandler) : HttpHandler =
        source >=> subRoute path handler

    /// <summary>
    /// Filters an incoming HTTP request based on a part of the request path (case insensitive).
    /// Subsequent route handlers inside the given handler function should omit the already validated path.
    /// </summary>
    /// <param name="path">A part of an expected request path.</param>
    /// <param name="handler">A Giraffe <see cref="HttpHandler"/> function.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler"/> function which can be composed into a bigger web application.</returns>
    let inline subRouteCi (path: string) (handler: HttpHandler) (source: HttpHandler) : HttpHandler =
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
    let inline subRoutef
        (path: PrintfFormat<_, _, _, _, 'T>)
        (routeHandler: 'T -> HttpHandler)
        (source: HttpHandler)
        : HttpHandler =
        source >=> subRoutef path routeHandler

    /// <summary>
    /// Sends a response back to the client based on the request's Accept header.
    ///
    /// The negotiation rules as well as a <see cref="HttpHandler" /> for unacceptable requests can be configured in the ASP.NET Core startup code by registering a custom class of type <see cref="INegotiationConfig"/>.
    /// </summary>
    /// <param name="responseObj">The object to send back to the client.</param>
    /// <param name="source">The previous HTTP handler to compose.</param>
    /// <returns>A Giraffe <see cref="HttpHandler" /> function which can be composed into a bigger web application.</returns>
    let inline negotiate (responseObj: obj) (source: HttpHandler) : HttpHandler = source >=> negotiate responseObj
