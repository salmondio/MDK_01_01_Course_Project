using Newtonsoft.Json.Bson;

namespace Couse_project_RestAPI.Helpers
{
    public class LogHelper
    {
        public static async Task Log(string message)
        {
            await File.AppendAllTextAsync(Directory.GetCurrentDirectory() + @"\Logs.log", DateTime.Now.ToString() + ": " + message + '\n');
        }
    }
}
