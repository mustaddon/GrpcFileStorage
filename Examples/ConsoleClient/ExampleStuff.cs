using System.Text;

namespace ConsoleClient
{
    class Example
    {
        public static Stream GenerateTextFile(string text, int count)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(string.Join("\n", Enumerable.Range(0, count).Select(i => $"{i + 1}: {text}"))));
        }
    }

    class MyCustomMetadata
    {
        public string? Author { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
    }
}
