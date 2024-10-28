namespace PawsAndHearts.SharedKernel;

public static class Constants
{
    public static readonly int MAX_NAME_LENGTH = 50;

    public static readonly int MAX_EXPERIENCE_VALUE = 90;

    public static readonly int MAX_PHONE_LENGTH = 14;

    public static readonly int MAX_TEXT_LENGTH = 100;

    public static readonly int MAX_DESCRIPTION_LENGTH = 500;

    public static readonly int MAX_HEATH_INFO_LENGTH = 250;
    
    public static readonly int MAX_ADDRESS_LENGTH = 200;

    public static readonly string[] PERMITTED_FILE_EXTENSIONS = [".jpg", ".png"];
    
    public static readonly int MAX_FILE_SIZE = 10 * 1024 * 1024;
    
    public static readonly string[] PERMITTED_HELP_STATUSES_FROM_VOLUNTEER = ["LookingForHome", "FoundHome"];

    public static readonly int FREQUENCY_OF_DELETION = 24;
    
    public static class FilePaths
    {
        public static readonly string ACCOUNTS = "etc/accounts.json";
    }
}