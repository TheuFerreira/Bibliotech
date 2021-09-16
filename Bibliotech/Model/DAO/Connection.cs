using Bibliotech.Singletons;
using MySqlConnector;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public abstract class Connection
    {
        protected MySqlConnection SqlConnection { get; private set; }
        //private const string CONNECTION_STRING = "Server=localhost;Port=3306;Database=bibliotech;Username=bibliotech;Password=@bibliotech123;AllowZeroDateTime=true;Allow User Variables=true;";
        private const string CONNECTION_STRING = "Server=ns858.hostgator.com.br;Port=3306;Database=asifmg99_bibliotech;Username=asifmg99_bib;Password=@bibliotech2021;AllowZeroDateTime=true;Allow User Variables=true;";

        protected async Task Connect()
        {
            try
            {
                SqlConnection = new MySqlConnection(CONNECTION_STRING);
                await SqlConnection.OpenAsync();
            }
            catch (MySqlException ex)
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
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
    }
}
