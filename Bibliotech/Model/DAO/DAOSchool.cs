using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Windows;


namespace Bibliotech.Model.DAO
{
    public class DAOSchool: Connection
    {
        public async Task InsertSchool(String name, String city, String dist, String phone, String street, String number)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            String strSql = "insert into address(city, neighborhood, street, number) " +
                                         "values(@city, @dist, @street, @number); "+
                            "select @@identity;";

            String strSql1 = " insert into branch(name, id_address, telephone, status) "+
                                          "values(@name, @id, @phone, 1);";


            try
            {
                
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);
                MySqlCommand cmd1 = new MySqlCommand(strSql1, SqlConnection, transaction);
             

                cmd1.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@dist", dist);
                cmd1.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@street", street);
                cmd.Parameters.AddWithValue("@number", number);
               

                
                object result = await cmd.ExecuteScalarAsync();
                int id = Convert.ToInt32(result.ToString());

                cmd1.Parameters.AddWithValue("@id", id);
          
                await cmd1.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

            }
            catch (MySqlException)
            {
                await transaction.RollbackAsync();
                throw;
                
            }
            finally
            {
                await Disconnect();
            }
        }
    }
}
















/*"START TRANSACTION; " +
           " set " + aux + " = -1; " +

           "insert into address(city, neighborhood, street, number) " +
                        "values(@city, @dist, @street, @number); " +

           "SELECT id_address INTO " + @aux +
           " from address " +
           "where city = @city and neighborhood = @dist and street = @street and number = @number; " +

           "insert into branch(name, id_address, telephone, status) " +
                       "values(@name, " + @aux + " , @phone, 1); " +
           "COMMIT;";*/