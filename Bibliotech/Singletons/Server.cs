using Microsoft.Win32;

namespace Bibliotech.Singletons
{
    public class Server
    {
        public string ServerName { get; set; }
        public int? Port { get; set; }
        public string Database { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public Server()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Bibliotech\Bibliotech");
            if (key == null)
            {
                return;
            }

            string result = key.GetValue("Server").ToString();
            key.Close();

            string[] results = result.Split('%');
            ServerName = results[0];
            Port = int.Parse(results[1]);
            Database = results[2];
            UserName = results[3];
            Password = results[4];
        }

        public void Save()
        {
            string value = $"{ServerName}%{Port.Value}%{Database}%{UserName}%{Password}";

            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Bibliotech\Bibliotech");
            key.SetValue("Server", value);
            key.Close();
        }

        public override string ToString()
        {
            return $"Server={ServerName};Port={Port};Database={Database};Username={UserName};Password={Password};AllowZeroDateTime=true;Allow User Variables=true;";
        }
    }
}
