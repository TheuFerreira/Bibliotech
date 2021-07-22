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
        DialogService dialogService = new DialogService();
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

                dialogService.ShowSuccess("Usuário salvo com sucesso");

            }
            catch (MySqlException)
            {
                await transaction.RollbackAsync();
                dialogService.ShowError("Algo de errado aconteceu.\nTente novamente.");
            }
            finally
            {
                await Disconnect();
            }
        }

        public async Task<int> UserCount()
        {
            await Connect();
            int number;
            String strSql = "select count(id_user) from users;";

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

        public async Task FillDataGrid(DataGrid dataGrid)
        {
            await Connect();

            String strSql = "select b.id_branch, b.name, b.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number) as endereco, b.status "+
                            "from branch as b "+
                            "inner join address as a on b.id_address = a.id_address; ";

            
            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);
                await cmd.ExecuteNonQueryAsync();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable("branch");
                adapter.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
                adapter.Update(dt);
                dataGrid.Columns[0].Visibility = Visibility.Hidden;

            }
            catch (Exception)
            {
                dialogService.ShowError("Algo de errado aconteceu.\nTente novamente.");
                
            }
            finally
            {
                await Disconnect();
            }
        }



        public async Task FillDataGrid(DataGrid dataGrid, String query)
        {
            await Connect();

            String strSql = "select b.id_branch, b.name, b.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number) as endereco, b.status " +
                            "from branch as b " +
                            "inner join address as a on b.id_address = a.id_address " +
                            "where b.name like \"%" +query +"%\";";

            
            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);
                await cmd.ExecuteNonQueryAsync();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable("branch");
                adapter.Fill(dt);
                dataGrid.ItemsSource = dt.DefaultView;
                adapter.Update(dt);
                dataGrid.Columns[0].Visibility = Visibility.Hidden;

            }
            catch (Exception)
            {
                dialogService.ShowError("Algo de errado aconteceu.\nTente novamente.");

            }
            finally
            {
                await Disconnect();
            }
        }

        public async Task OnOffSchool(int status, int id)
        {
            await Connect();
            String strSql = "update branch set status = " + status + "where id_branch = " + id + ";";

            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
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