using System.IO;
using Newtonsoft.Json;

namespace EssentialsPlus
{
    public class Config
    {
        [JsonProperty("Comandos deshabilitados en PvP")]
        public string[] DisabledCommandsInPvp = new string[]
        {
            "eback"
        };

        [JsonProperty("Historial de posiciones de retroceso")]
        public int BackPositionHistory = 10;

        [JsonProperty("Host de MySql")]
        public string MySqlHost = "";

        [JsonProperty("Nombre de la base de datos MySql")]
        public string MySqlDbName = "";

        [JsonProperty("Nombre de usuario de MySql")]
        public string MySqlUsername = "";

        [JsonProperty("Contraseña de MySql")]
        public string MySqlPassword = "";

        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string path)
        {
            return File.Exists(path) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(path)) : new Config();
        }
    }
}
