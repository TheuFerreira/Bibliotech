using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Windows;
using Bibliotech.Services;
using System.Data;
using System.Windows.Controls;

namespace Bibliotech.Model.DAO
{
    public class DAOSchool: Connection
    {

        public async Task<bool> Insert(String name, String city, String dist, long? phone, String street, String number)
        {

            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                String strSql = "insert into address(city, neighborhood, street, number) " +
                                       "values(@city, @dist, @street, @number); " +
                                       "select @@identity;";

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);

                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@dist", dist);
                cmd.Parameters.AddWithValue("@street", street);
                cmd.Parameters.AddWithValue("@number", number);

                object result = await cmd.ExecuteScalarAsync();
                int id = Convert.ToInt32(result.ToString());

                strSql = " insert into branch(name, id_address, telephone, status) " +
                                      "values(@name, @id, @phone, 1);";

                cmd.CommandText = strSql;
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@phone", phone);

                await cmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return true;
                

            }
            catch (MySqlException)
            {
                await transaction.RollbackAsync();
                return false;
                throw;
            }
            finally
            {
                await Disconnect();
            }
        }

        public async Task<bool> Update(int id, String name, String city, String dist, long? phone, String street, String number, int id_address)
        {
           
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                String strSql = "update address " +
                             "set city = @city, neighborhood = @dist, street = @street, number = @number " +
                             "where id_address = @id_address; ";

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);
               

                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@dist", dist);
                cmd.Parameters.AddWithValue("@street", street);
                cmd.Parameters.AddWithValue("@number", number);
                cmd.Parameters.AddWithValue("@id_address", id_address);

                await cmd.ExecuteNonQueryAsync();

                strSql = "update branch " +
                             "set name = '" + name + "', telephone = @phone " +
                             " where id_branch = @id;";

                cmd.CommandText = strSql;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@phone", phone);
                
                await cmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return true;

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
                throw;
            }
            finally
            {
                await Disconnect();
            }
        }

        public async Task OnOff(int status, int id)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            String strSql = "update branch set status = " + status + " where id_branch = " + id + ";";

            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);
                await cmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();
                
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await Disconnect();
            }

        }

        public async Task<int> Count()
        {
            await Connect();
            int number;
            String strSql = "select count(id_user) from users where status = 1;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);
                object result = await cmd.ExecuteScalarAsync();
                number = Convert.ToInt32(result.ToString());              
            }
            catch (MySqlException)
            {
                number = -1;
            }
            finally
            {
                await Disconnect();
            }

            return number;
        }

        public async Task<DataTable> FillDataGrid(String query)
        {
            await Connect();

            String strSql = "select b.id_branch, b.name, b.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number) as endereco, s.description, b.id_address, a.city, a.neighborhood, a.street, a.number " +
                            "from branch as b " +
                            "inner join address as a on b.id_address = a.id_address " +
                            "inner join status as s on s.id_status = b.status " +
                            "where b.name like \"%" +query +"%\";";

            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);
                await cmd.ExecuteNonQueryAsync();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable("branch");
                
                adapter.Fill(dt);
                return dt;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
            finally
            {
                await Disconnect();
            }
        }


       
    }
}

