namespace MyApp.API.Exceptions
{
    public class BadRequestException(string message) : Exception(message);
}
