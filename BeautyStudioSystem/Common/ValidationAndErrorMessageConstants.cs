namespace BeautyStudioSystem.Common
{
    public static class ValidationAndErrorMessageConstants
    {
        // ClientsController constants
        public const string NoClientReservationsMessage = "Client doesn't have any reservations.";
        public const string ClientDeletedMessage = "Client deleted successfully.";
        public const string ClientUpdatedMessage = "Client updated successfully.";
        public const string UserNotFoundMessage = "User not found.";
        public const string PromoteToEmployeeMessage = "{0} is now an employee.";
        public const string NoCurrentReservationsMessage = "You do not have any reservations yet.";

        //EmployeeController constants
        public const string NoReservationsMessage = "No reservations founds.";
        public const string EmployeeHasNoReservationsMessage = "Employee doesn't have any reservations.";
        public const string EmployeeUpdatedMessage = "Employee updated successfully.";
        public const string RevertToClientMessage = "{0} has been reverted to Client.";
        public const string EmployeeDeletedMessage = "Employee deleted successfully.";

        //ReservationsController constants
        public const string ReservationDeletedMessage = "Reservation deleted successfully.";
        public const string ReservationCreatedMessage = "Reservation created successfully.";
    }
}
