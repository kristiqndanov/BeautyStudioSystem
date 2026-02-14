namespace BeautyStudioSystem.Core.Common
{
    public static class InputValidations
    {
        /* Client Constraints */
        public const int FirstNameMinLength = 3;
        public const int LastNameMinLength = 3;
        public const int FirstNameMaxLength = 50;
        public const int LastNameMaxLength = 90;
        public const int FullNameMinLength = 3;
        public const int FullNameMaxLength = 510;
        public const int EmailMaxLength = 255;
        public const int PhoneLength = 10;

        /*Service Constraints */
        public const int ServiceNameMaxLength = 300;
    }
}
