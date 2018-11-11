using AspNet.Core.Common.Exceptions;

namespace AspNet.Core.Common
{
    public static class ExceptionsBuilder
    {
        public static void ThrowNotFoundException(string entityName)
        {
            string errorMessage = string.Format(Messages.ErrorMessages.EntityNotFound, entityName);
            throw new NotFoundException(errorMessage);
        }

        public static void ThrowNotFoundException()
        {
            string errorMessage = Messages.ErrorMessages.NotFound;
            throw new NotFoundException(errorMessage);
        }
    }
}