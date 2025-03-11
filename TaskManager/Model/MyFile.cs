namespace TaskManager.Model
{
    public class MyFile
    {
        public int FileID { get; }
        public int TaskID { get; }
        public string? Name { get; set; }
        public byte[]? Data { get; set; }
    }
}
