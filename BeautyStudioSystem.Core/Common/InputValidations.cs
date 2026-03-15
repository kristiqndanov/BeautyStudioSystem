namespace BeautyStudioSystem.Core.Common
{
    public static class InputValidations
    {
        //Client Constraints
        public const int FirstNameMinLength = 3;
        public const int LastNameMinLength = 3;
        public const int FirstNameMaxLength = 50;
        public const int LastNameMaxLength = 90;
        public const int FullNameMinLength = 3;
        public const int FullNameMaxLength = 510;
        public const int EmailMaxLength = 255;
        public const int PhoneLength = 10;

        //Service Constraints
        public const int ServiceNameMaxLength = 300;


        //ClientService Messages
        public const string ClientByUserIdNotFoundMessage = "No client found with the provided user ID.";
        public const string EmptySearchTermMessage = "Search term cannot be empty.";

        //ReservationService Messages
        public const string InvalidDateMessage = "Invalid date";
        public const string InvalidStartTimeMessage = "Invalid start time.";
        public const string ReservationInPastMessage = "Reservation date and time cannot be in the past.";
        public const string ServiceDoesNotExistMessage = "Selected service does not exist.";
        public const string ReservationDuplicateMessage = "Another reservation is already booked for this employee at the same time.";
        public const string ClientNotFoundMessage = "Your account doesn't exist on the database.";
        public const string ReservationNotFoundMessage = "No reservation found.";
        public const string EmployeeNotFoundMessage = "No employee found.";

        //ServiceService Messages
        public const string PriceCannotBeNegativeMessage = "Price cannot be negative number.";


        //ClientViewModel Messages
        public const string FullNameErrorMessage = "Full name must have at least 1 letter for first name and at least 1 letter for last name";
        public const string FullNameContainsTwoWordsMessage = "Full name must contain first name and last name separated by a space.";
        public const string InvalidEmailMessage = "Invalid Email Address";
        public const string PhoneNumberErrorMessage = "Phone number must be exactly 10 digits.";
    }
}
