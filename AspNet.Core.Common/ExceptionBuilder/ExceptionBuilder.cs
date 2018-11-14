using AspNetCore.UnitOfWork.Common.Exceptions;

namespace AspNetCore.UnitOfWork.Common
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