using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace Bibliotech.Model.DAO
{
    public abstract class Connection
    { 
        protected MySqlConnection SqlConnection { get; private set; }
        private const string Connection_string = "Server=localhost;Port=3306;Database=bibliotech;Username=bibliotech;Password=@bibliotech123;AllowZeroDateTime=true;";

        protected async Task Connect()
        {
            try
            {
                SqlConnection = new MySqlConnection(Connection_string);
                await SqlConnection.OpenAsync();
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
        }

        protected async Task Disconnect()
        {
            try
            {
                await SqlConnection.CloseAsync();
            }
            catch(MySqlException ex)
            {
                throw ex;
            }

        }

    }
}
