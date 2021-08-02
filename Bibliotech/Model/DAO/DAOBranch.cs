using Bibliotech.Model.Entities;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOBranch : Connection
    {
        public async Task<bool> Insert(Branch school)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string strSql = "insert into address(city, neighborhood, street, number) " +
                                       "values(@city, @dist, @street, @number); " +
                                       "select @@identity;";

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);

                _ = cmd.Parameters.AddWithValue("@city", school.Address.City);
                _ = cmd.Parameters.AddWithValue("@dist", school.Address.Neighborhood);
                _ = cmd.Parameters.AddWithValue("@street", school.Address.Street);
                _ = cmd.Parameters.AddWithValue("@number", school.Address.Number);

                object result = await cmd.ExecuteScalarAsync();
                school.Address.IdAddress = Convert.ToInt32(result.ToString());

                strSql = " insert into branch(name, id_address, telephone, status) " +
                                      "values(@name, @id, @phone, 1);";

                cmd.Parameters.Clear();

                cmd.CommandText = strSql;
                _ = cmd.Parameters.AddWithValue("@name", school.Name);
                _ = cmd.Parameters.AddWithValue("@id", school.Address.IdAddress);
                _ = cmd.Parameters.AddWithValue("@phone", school.Telephone);

                _ = await cmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (MySqlException ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
            finally
            {
                await Disconnect();
            }
        }

        public async Task<bool> Update(int id, Branch school)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string strSql = "update address " +
                             "set city = @city, neighborhood = @dist, street = @street, number = @number " +
                             "where id_address = @id_address; ";

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);

                _ = cmd.Parameters.AddWithValue("@city", school.Address.City);
                _ = cmd.Parameters.AddWithValue("@dist", school.Address.Neighborhood);
                _ = cmd.Parameters.AddWithValue("@street", school.Address.Street);
                _ = cmd.Parameters.AddWithValue("@number", school.Address.Number);
                _ = cmd.Parameters.AddWithValue("@id_address", school.Address.IdAddress);

                _ = await cmd.ExecuteNonQueryAsync();

                strSql = "update branch " +
                             "set name = '" + school.Name + "', telephone = @phone " +
                             " where id_branch = @id;";

                cmd.Parameters.Clear();

                cmd.CommandText = strSql;
                _ = cmd.Parameters.AddWithValue("@id", id);
                _ = cmd.Parameters.AddWithValue("@phone", school.Telephone);

                _ = await cmd.ExecuteNonQueryAsync();

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

        public async Task<int> UsersCount()
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

        public async Task<int> Total()
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT COUNT(id_branch) FROM branch WHERE status = 1;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                object result = await command.ExecuteScalarAsync();

                return Convert.ToInt32(result);
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                await Disconnect();
            }
        }

        public async Task<DataTable> FillDataGrid(String query)
        {
            await Connect();

            String strSql = "select b.id_branch, b.name, b.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number) as endereco, s.description, b.id_address, a.city, a.neighborhood, a.street, a.number " +
                            "from branch as b " +
                            "inner join address as a on b.id_address = a.id_address " +
                            "inner join status as s on s.id_status = b.status " +
                            "where b.name like \"%" + query + "%\";";

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

        public async Task<Branch> GetById(int idBranch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT name " +
                    "FROM branch " +
                    "WHERE id_branch = ?;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = idBranch;

                Branch school = new Branch();
                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    string name = await reader.GetFieldValueAsync<string>(0);

                    school = new Branch(idBranch, name);
                }

                return school;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                await Disconnect();
            }
        }
    }
}

