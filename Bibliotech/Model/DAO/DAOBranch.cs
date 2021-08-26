using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOBranch : Connection
    {
        public async Task<bool> Save(Branch branch)
        {
            return branch.IsNew() ? await Insert(branch) : await Update(branch);
        }

        private async Task<bool> Insert(Branch branch)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string strSql = "insert into address(city, neighborhood, street, number) " +
                                       "values(@city, @dist, @street, @number); " +
                                       "select @@identity;";

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);

                _ = cmd.Parameters.AddWithValue("@city", branch.Address.City);
                _ = cmd.Parameters.AddWithValue("@dist", branch.Address.Neighborhood);
                _ = cmd.Parameters.AddWithValue("@street", branch.Address.Street);
                _ = cmd.Parameters.AddWithValue("@number", branch.Address.Number);

                object result = await cmd.ExecuteScalarAsync();
                branch.Address.IdAddress = Convert.ToInt32(result.ToString());

                strSql = " insert into branch(name, id_address, telephone, status) " +
                                      "values(@name, @id, @phone, 1);";

                cmd.Parameters.Clear();

                cmd.CommandText = strSql;
                _ = cmd.Parameters.AddWithValue("@name", branch.Name);
                _ = cmd.Parameters.AddWithValue("@id", branch.Address.IdAddress);
                _ = cmd.Parameters.AddWithValue("@phone", branch.Telephone);

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

        private async Task<bool> Update(Branch branch)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string strSql = "update address " +
                             "set city = @city, neighborhood = @dist, street = @street, number = @number " +
                             "where id_address = @id_address; ";

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);

                _ = cmd.Parameters.AddWithValue("@city", branch.Address.City);
                _ = cmd.Parameters.AddWithValue("@dist", branch.Address.Neighborhood);
                _ = cmd.Parameters.AddWithValue("@street", branch.Address.Street);
                _ = cmd.Parameters.AddWithValue("@number", branch.Address.Number);
                _ = cmd.Parameters.AddWithValue("@id_address", branch.Address.IdAddress);

                _ = await cmd.ExecuteNonQueryAsync();

                strSql = "update branch " +
                             "set name = '" + branch.Name + "', telephone = @phone " +
                             " where id_branch = @id;";

                cmd.Parameters.Clear();

                cmd.CommandText = strSql;
                _ = cmd.Parameters.AddWithValue("@id", branch.IdBranch);
                _ = cmd.Parameters.AddWithValue("@phone", branch.Telephone);

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

        public async Task OnOff(Status status, Branch branch)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string sql = "update branch set status = " + (int)status + " where id_branch = " + branch.IdBranch + ";";

                MySqlCommand cmd = new MySqlCommand(sql, SqlConnection, transaction);
                _ = await cmd.ExecuteNonQueryAsync();
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

        public async Task<int> UsersCount(Branch branch)
        {
            try
            {
                await Connect();
                string sql = "" +
                    "select count(id_user) " +
                    "from users AS u " +
                    "inner join branch as b on b.id_branch = u.id_branch " +
                    "where u.status = 1 " +
                        "AND b.id_branch = ?;";

                MySqlCommand cmd = new MySqlCommand(sql, SqlConnection);
                cmd.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                object result = await cmd.ExecuteScalarAsync();

                return Convert.ToInt32(result.ToString());
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

        public async Task<List<Branch>> FillDataGrid(string query)
        {
            try
            {
                await Connect();

                string sql = "select b.id_branch, b.name, b.telephone, b.id_address, a.city, a.neighborhood, a.street, a.number, b.status " +
                                "from branch as b " +
                                "inner join address as a on b.id_address = a.id_address " +
                                "where b.name like \"%" + query + "%\";";

                MySqlCommand cmd = new MySqlCommand(sql, SqlConnection);

                List<Branch> branches = new List<Branch>();
                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idBranch = await reader.GetFieldValueAsync<int>(0);
                    string name = await reader.GetFieldValueAsync<string>(1);
                    long? telephone = null;
                    if (await reader.IsDBNullAsync(2) == false)
                    {
                        telephone = await reader.GetFieldValueAsync<long>(2);
                    }

                    int idAddress = await reader.GetFieldValueAsync<int>(3);
                    string city = await reader.GetFieldValueAsync<string>(4);
                    string neighborhood = await reader.GetFieldValueAsync<string>(5);
                    string street = await reader.GetFieldValueAsync<string>(6);
                    string number = await reader.GetFieldValueAsync<string>(7);
                    Status statusBranch = (Status)await reader.GetFieldValueAsync<int>(8);

                    Address address = new Address
                    {
                        IdAddress = idAddress,
                        City = city,
                        Neighborhood = neighborhood,
                        Street = street,
                        Number = number,
                    };

                    Branch branch = new Branch
                    {
                        IdBranch = idBranch,
                        Name = name,
                        Telephone = telephone,
                        Address = address,
                        Status = statusBranch,
                    };

                    branches.Add(branch);
                }

                return branches;
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

        public async Task<Branch> GetById(int idBranch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT " +
                        "b.name, b.telephone, " +
                        "a.id_address, a.city, a.neighborhood, a.street, a.number " +
                    "FROM branch AS b " +
                    "INNER JOIN address AS a ON b.id_address = a.id_address " +
                    "WHERE id_branch = ?;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = idBranch;

                Branch school = new Branch();
                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    string name = await reader.GetFieldValueAsync<string>(0);
                    long? telephone = null;
                    if (await reader.IsDBNullAsync(1) == false)
                    {
                        telephone = await reader.GetFieldValueAsync<long>(1);
                    }

                    int idAddress = await reader.GetFieldValueAsync<int>(2);
                    string city = await reader.GetFieldValueAsync<string>(3);
                    string neighborhood = await reader.GetFieldValueAsync<string>(4);
                    string street = await reader.GetFieldValueAsync<string>(5);
                    string number = await reader.GetFieldValueAsync<string>(6);

                    Address address = new Address
                    {
                        IdAddress = idAddress,
                        City = city,
                        Neighborhood = neighborhood,
                        Street = street,
                        Number = number,
                    };

                    school = new Branch
                    {
                        IdBranch = idBranch,
                        Name = name,
                        Telephone = telephone,
                        Address = address,
                    };
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

