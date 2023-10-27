using System.Net;
using System.Text.Json.Serialization;

namespace Chit.Utilities;

public class ChitStandardResponse<T> : Link
{
    public bool Status { get; set; }
    public string? Message { get; set; }
    public T Data { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public object? Errors { get; set; }
    [JsonIgnore]
    public Link Self { get; set; }


    public ChitStandardResponse()
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationService.ViewModel.ChitStandardResponse"/> class.
    /// </summary>
    /// <param name="status">If set to <c>true</c> status.</param>
    private ChitStandardResponse(bool status)
    {
        Status = status;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationService.ViewModel.ChitStandardResponse"/> class.
    /// </summary>
    /// <param name="status">If set to <c>true</c> status.</param>
    /// <param name="message">Message.</param>
    private ChitStandardResponse(bool status, string message)
    {
        Status = status;
        Message = message;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ApplicationService.ViewModel.ChitStandardResponse"/> class.
    /// </summary>
    /// <param name="status">If set to <c>true</c> status.</param>
    /// <param name="message">Message.</param>
    /// <param name="data">Data.</param>
    private ChitStandardResponse(bool status, string message, T data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    /// <summary>
    /// Initialize ChitStandardResponse(bool status, string message, object data, string statusCode)
    /// </summary>
    /// <param name="status"></param>
    /// <param name="message"></param>
    /// <param name="data"></param>
    /// <param name="statusCode"></param>
    private ChitStandardResponse(bool status, string message, T data, HttpStatusCode statusCode)
    {
        Status = status;
        Message = message;
        Data = data;
        StatusCode = statusCode;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static ChitStandardResponse<T> Create()
    {
        return new ChitStandardResponse<T>();
    }

    /// <summary>
    /// Create the specified status.
    /// </summary>
    /// <returns>The create.</returns>
    /// <param name="status">If set to <c>true</c> status.</param>
    public static ChitStandardResponse<T> Create(bool status)
    {
        return new ChitStandardResponse<T>(status);
    }

    /// <summary>
    /// Create the specified status and message.
    /// </summary>
    /// <returns>The create.</returns>
    /// <param name="status">If set to <c>true</c> status.</param>
    /// <param name="message">Message.</param>
    public static ChitStandardResponse<T> Create(bool status, string message)
    {
        return new ChitStandardResponse<T>(status, message);
    }

    /// <summary>
    /// Create the specified status, message and data.
    /// </summary>
    /// <returns>The create.</returns>
    /// <param name="status">If set to <c>true</c> status.</param>
    /// <param name="message">Message.</param>
    /// <param name="data">Data.</param>
    public static ChitStandardResponse<T> Create(bool status, string message, T data)
    {
        return new ChitStandardResponse<T>(status, message, data);
    }

    /// <summary>
    /// Adds the status code.
    /// </summary>
    /// <returns>The status code.</returns>
    /// <param name="statusCode">Status code.</param>

    public ChitStandardResponse<T> AddStatusCode(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
        return this;
    }

    /// <summary>
    /// Adds the status message.
    /// </summary>
    /// <returns>The status message.</returns>
    /// <param name="message">Message.</param>
    public ChitStandardResponse<T> AddStatusMessage(string message)
    {
        this.Message = message;
        return this;
    }

    /// <summary>
    /// Error that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Error(string message)
    {
        return new ChitStandardResponse<T>(false, message, default(T), HttpStatusCode.InternalServerError);
    }

    /// <summary>
    /// Unauthorized that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Unauthorized()
    {
        return new ChitStandardResponse<T>(false, "", default(T), HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Error that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Error(string message, HttpStatusCode statusCode)
    {
        return new ChitStandardResponse<T>(false, message, default(T), statusCode);
    }

    /// <summary>
    ///  Ok that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Ok(string message)
    {
        return new ChitStandardResponse<T>(true, message, default(T), HttpStatusCode.OK);
    }

    /// <summary>
    ///  Ok that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Ok(T data)
    {
        return new ChitStandardResponse<T>(true, StandardResponseMessages.SUCCESSFUL, data, HttpStatusCode.OK);
    }

    public static ChitStandardResponse<T> NotFound(string message)
    {
        return new ChitStandardResponse<T>(false, message, default(T), HttpStatusCode.NotFound);
    }
    /// <summary>
    ///  Ok that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Ok()
    {
        return new ChitStandardResponse<T>(true, StandardResponseMessages.SUCCESSFUL, default(T), HttpStatusCode.OK);
    }

    /// <summary>
    ///  Failed that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Failed()
    {
        return new ChitStandardResponse<T>(false, StandardResponseMessages.UNSUCCESSFUL);
    }

    /// <summary>
    ///  Failed that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Failed(string message)
    {
        return new ChitStandardResponse<T>(false, message, default(T), HttpStatusCode.InternalServerError);
    }

    /// <summary>
    ///  Failed that return this
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static ChitStandardResponse<T> Failed(string message, HttpStatusCode statusCode)
    {
        return new ChitStandardResponse<T>(false, message, default(T), statusCode);
    }


    /// <summary>
    /// Build this instance.
    /// </summary>
    /// <returns>The build.</returns>
    public ChitStandardResponse<T> Build()
    {
        return this;
    }

    public ChitStandardResponse<T> AddData(T data)
    {
        this.Data = data;
        return this;
    }

}
