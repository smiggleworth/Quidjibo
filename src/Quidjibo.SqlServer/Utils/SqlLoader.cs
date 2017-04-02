using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Quidjibo.SqlServer.Utils
{
    public class SqlLoader
    {
        public static async Task<string> GetScript(string scriptName)
        {
            var assembly = typeof(SqlLoader).GetTypeInfo().Assembly;
            var fullName = $"Quidjibo.SqlServer.Scripts.{scriptName}.sql";
            using (var stream = assembly.GetManifestResourceStream(fullName) ?? new MemoryStream())
            using (var reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}