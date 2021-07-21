using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;


namespace Bibliotech.Model.DAO
{
    public class DAOSchool: Connection
    {
        public async Task InsertSchool(String name, String city, String dist, String phone, String street, String number)
        {
            String aux = "@aux";
            String strSql =
           "START TRANSACTION; " +
            " set " +aux+" = -1; " +

            "insert into address(city, neighborhood, street, number) " +
                         "values(@city, @dist, @street, @number); " +

            "SELECT id_address INTO "+ @aux + 
            " from address " +
            "where city = @city and neighborhood = @dist and street = @street and number = @number; " +

            "insert into branch(name, id_address, telephone, status) " +
                        "values(@name, " + @aux + " , @phone, 1); " +
            "COMMIT;";
            
            try
            {
                await Connect();
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@dist", dist);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@street", street);
                cmd.Parameters.AddWithValue("@number", number);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (MySqlException)
            {

                throw;
            }
            finally
            {
                await Disconnect();
            }
        }
    }
}
