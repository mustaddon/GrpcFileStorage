namespace ConsoleClient
{
    internal class MyCustomMetadata
    {
        public string? Author { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;

    }
}
