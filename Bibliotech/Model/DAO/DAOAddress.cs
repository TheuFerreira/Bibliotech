using Bibliotech.Model.Entities;
using MySqlConnector;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOAddress
    {
        public async Task<int> Insert(Address address, MySqlCommand command)
        {
            try
            {
                string str = "" +
                    "INSERT INTO address(city, neighborhood, street, number, complement, status) " +
                    "VALUES (?, ?, ?, ?, ?, 1);" +
                    "SELECT @@IDENTITY;";

                command.Parameters.Clear();
                command.CommandText = str;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.City;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.Neighborhood;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.Street;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.Number;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.Complement;

                object result = await command.ExecuteScalarAsync();
                return int.Parse(result.ToString());
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        public async Task Update(Address address, MySqlCommand command)
        {
            try
            {
                string str = "" +
                    "UPDATE address " +
                    "SET city = ?, neighborhood = ?, street = ?, number = ?, complement = ? " +
                    "WHERE id_address = ?;";

                command.Parameters.Clear();
                command.CommandText = str;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.City;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.Neighborhood;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.Street;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.Number;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.Complement;
                command.Parameters.Add("?", System.Data.DbType.String).Value = address.IdAddress;

                await command.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
    }
}
