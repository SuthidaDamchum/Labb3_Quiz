using System.IO;
using System.Text.Json;
using Labb3_Quiz.Models;

namespace Labb3_Quiz.Helpers
{
    public class JsonService
    {
        private IEnumerable<QuestionPack> newPacks;

        public void AddToFile(QuestionPack[] questionPack)
        {
            var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Suthidas_Labb3");
            if (!Directory.Exists(root) == false)
                Directory.CreateDirectory(root);

            var fileName = Path.Combine(root, "QuestionPacks.json");

            var combinedJson = JsonSerializer.Serialize(questionPack);
            File.WriteAllText(fileName, combinedJson);
        }

        public QuestionPack[]? ReadFile()
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
                var jsonstring = File.ReadAllText(fileName);
                return JsonSerializer.Deserialize<QuestionPack[]>(jsonstring) ?? Array.Empty<QuestionPack>();
            }
            catch (Exception e)
            {

                Console.WriteLine($"Can not read  the file");
                return Array.Empty<QuestionPack>();
            }

        }
    }
}
