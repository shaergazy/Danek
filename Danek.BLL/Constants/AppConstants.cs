namespace Danek.BLL.Constants;

public static class AppConstants
{
    #region ServiceUri.Self
    public static string BaseUri { get; set; }

    public static string BaseFrontUri { get; set; }

    #endregion

    #region ContentTypes
    public static string ExcelContentType => "application/vnd.ms-excel";

    public static string AppJsonContentType => "application/json";

    public static string AppSomeJsonContentType => "application/*+json";
    #endregion
    public static string RelativeFilesPath { get; set; }
    public static string FullFilesPath { get; set; }
    public static string BaseDir { get; set; }
    public static string FlagDir = "Flags";
    public static string PosterDir = "Posters";
}
