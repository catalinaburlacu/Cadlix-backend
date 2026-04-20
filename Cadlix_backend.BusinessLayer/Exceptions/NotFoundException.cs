namespace Cadlix_backend.BusinessLayer.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}
