namespace N2ImageAgent.AzureBlob.Models
{
    public class ImageInfo
    {
        public string Id { set; get; }
        public int Width { set; get; }
        public int Height { set; get; }
        public string Extension { set; get; }
        public string Tag { get; set; }
    }
}
