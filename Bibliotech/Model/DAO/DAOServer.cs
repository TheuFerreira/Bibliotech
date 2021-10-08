using iText.Svg.Renderers.Path.Impl;
using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOServer : Connection
    {
        public async Task<bool> TestConnection(string server, string port, string database, string username, string password)
        {
            string str = $"Server={server};Port={port};Database={database};Username={username};Password={password};AllowZeroDateTime=true;";
            MySqlConnection connection = new MySqlConnection(str);

            try
            {
                await connection.OpenAsync();

                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                await Connect(false);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                await Disconnect();
            }
        }
    }
}
