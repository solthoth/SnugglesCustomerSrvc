using Newtonsoft.Json;

namespace UserAuthService.Common
{
    public class Settings
    {
        public static Settings Create(string filePath = "/config/config.json")
        {
            var json = System.IO.File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Settings>(json);
        }

        private Settings()
        {
        }

        public Data Data { get; set; }
    }

    public class Data
    {
        private Data()
        {
        }

        public Stormpath Stormpath { get; set; }
    }

    public class Stormpath
    {
        private Stormpath()
        {
        }

        public Application[] Applications { get; set; }
        public Directory[] Directories { get; set; }
        public User User { get; set; }
    }

    public class User
    {
        private User()
        {
        }

        public string Name { get; set; }
        public string ApiKeyID { get; set; }
        public string ApiKeySecret { get; set; }
    }

    public class Application
    {
        private Application()
        {
        }

        public string name { get; set; }
        public string href { get; set; }
    }

    public class Directory
    {
        private Directory()
        {
        }

        public string name { get; set; }
        public string href { get; set; }
    }
}