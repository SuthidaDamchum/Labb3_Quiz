using System.IO;
using System.Text.Json;
using Labb3_Quiz.Models;

namespace Labb3_Quiz.Helpers
{
    public class JsonService
    {
        public async void AddToFile(QuestionPack[] questionPack)
        {
            var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Suthidas_Labb3");
            if (!Directory.Exists(root) == false)
                Directory.CreateDirectory(root);

            var fileName = Path.Combine(root, "QuestionPacks.json");
            await using FileStream stream = File.Create(fileName);

            await JsonSerializer.SerializeAsync<QuestionPack[]>(stream, questionPack);
        }

        public async Task<QuestionPack[]?> ReadFile()
        {
            var root = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Suthidas_Labb3"
                    );
            var fileName = Path.Combine(root, "QuestionPacks.json");

            if (File.Exists(fileName) == false)
            {
                return Array.Empty<QuestionPack>();
            }

            try
            {
                using var stream = File.OpenRead(fileName);
                return await JsonSerializer.DeserializeAsync<QuestionPack[]?>(stream);
             }
            catch (Exception e)
            {

                Console.WriteLine($"Can not read  the file");
                return Array.Empty<QuestionPack>();
            }

        }
    }
}
