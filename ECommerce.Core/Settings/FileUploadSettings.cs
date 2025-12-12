namespace ECommerce.Core.Settings
{
    public class FileUploadSettings
    {
        public int MaxFileSizeInMB { get; set; }
        public string[] AllowedExtensions { get; set; } = [];
    }
}
