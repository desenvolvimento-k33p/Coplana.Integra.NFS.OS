using System.Reflection;

namespace Integracao.Magis5.Services.SQLs
{
    public static class SQLSupport
    {
        public static string GetConsultas(string nomeConsulta)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var targetName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith($".{nomeConsulta}.sql"));

            using (var stream = assembly.GetManifestResourceStream(targetName))
            using (var reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();

                return result;
            }

        }
    }
}
