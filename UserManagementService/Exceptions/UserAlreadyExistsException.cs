namespace UserManagementService.Exceptions
{
    public class UserAlreadyExistsException(string message) : AppException(message, StatusCodes.Status409Conflict)
    {
    }
}
