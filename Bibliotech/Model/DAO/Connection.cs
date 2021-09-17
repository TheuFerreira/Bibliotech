using Bibliotech.Services;
using Bibliotech.Singletons;
using Bibliotech.View;
using Bibliotech.View.Users;
using MySqlConnector;
using System.Data;
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
                //SqlConnection = new MySqlConnection(Session.Instance.Server.ToString());
                SqlConnection = new MySqlConnection(CONNECTION_STRING);
                await SqlConnection.OpenAsync();
            }
            catch (MySqlException ex)
            {
                new DialogService().ShowError("Houve um problema ao se conectar com o servidor! O programa será fechado.");
               
                App.Current.Shutdown();
                throw ex;
            }
        }

        protected async Task Disconnect()
        {
            try
            {
                if (SqlConnection.State != ConnectionState.Closed)
                    await SqlConnection.CloseAsync();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
    }
}
