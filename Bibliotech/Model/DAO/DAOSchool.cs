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

     
        public async Task Update(int id, String name, String city, String dist, String phone, String street, String number, int id_address)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            String strSql1 = "update address "+
                             "set city = @city, neighborhood = @dist, street = @street, number = @number " +
                             "where id_address = @id_address; ";

            String strSql2 = "update branch " +
                             "set name = '" + name + "', telephone = '" + phone + "' " +
                             " where id_branch = @id;";


            try
            {
              
                MySqlCommand cmd1 = new MySqlCommand(strSql1, SqlConnection, transaction);
                MySqlCommand cmd2= new MySqlCommand(strSql2, SqlConnection, transaction);

                cmd1.Parameters.AddWithValue("@city", city);
                cmd1.Parameters.AddWithValue("@dist", dist);
                cmd1.Parameters.AddWithValue("@street", street);
                cmd1.Parameters.AddWithValue("@number", number);
                cmd1.Parameters.AddWithValue("@id_address", id_address);

                cmd2.Parameters.AddWithValue("@id", id);


                await cmd1.ExecuteNonQueryAsync();
                await cmd2.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                dialogService.ShowSuccess("Usuário salvo com sucesso");

            }
            catch (Exception)
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

            String strSql = "select b.id_branch, b.name, b.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number) as endereco, b.status, b.id_address, a.city, a.neighborhood, a.street, a.number, a.complement " +
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

            String strSql = "select b.id_branch, b.name, b.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number) as endereco, b.status, b.id_address, a.city, a.neighborhood, a.street, a.number, a.complement " +
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
                dialogService.ShowSuccess("Ativado/Desativado com sucesso");
            }
            catch (Exception)
            {
                dialogService.ShowError("Algo deu errado\nTente novamente");
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

