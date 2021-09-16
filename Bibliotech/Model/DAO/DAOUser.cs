using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOUser : Connection
    {
        public async Task<User> IsValidUser(string user, string password)
        {
            try
            {
                await Connect();
                string select = " select u.id_user, u.id_type_user, u.name, b.id_branch, b.id_address, b.name " +
                    " from users as u " +
                    " inner join branch as b on b.id_branch = u.id_branch " +
                    " where binary user_name = @user and aes_decrypt(password, 'bibliotech2021') = @password " +
                    "and u.status = 1 and b.status = 1;";

                MySqlCommand cmd = new MySqlCommand(select, SqlConnection);
                _ = cmd.Parameters.AddWithValue("@user", user);
                _ = cmd.Parameters.AddWithValue("@password", password);

                User User = null;
                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idUser = reader.GetInt32(0);
                    TypeUser typeUser = (TypeUser)reader.GetInt32(1);
                    string nameUser = reader.GetString(2);

                    int idBranch = reader.GetInt32(3);
                    int idAddressBranch = reader.GetInt32(4);
                    string nameBranch = reader.GetString(5);

                    Address address = new Address
                    {
                        IdAddress = idAddressBranch
                    };

                    Branch branch = new Branch
                    {
                        IdBranch = idBranch,
                        Name = nameBranch,
                        Address = address,
                    };

                    User = new User(idUser, typeUser, nameUser)
                    {
                        IdUser = idUser,
                        TypeUser = typeUser,
                        Name = nameUser,
                        UserName = user,
                        Branch = branch,
                    };
                }

                return User;
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

        public async Task<List<User>> SearchByText(TypeSearch typeSearch, string text, Branch branch)
        {
            try
            {
                await Connect();

                string str = "" +
                    "SELECT " +
                        "u.id_user, u.name, u.id_type_user, u.telephone, " +
                        "a.id_address, a.city, a.neighborhood, a.street, a.number, a.complement, " +
                        "b.id_branch, b.name " +
                    "FROM users AS u " +
                    "INNER JOIN address AS a ON u.id_address = a.id_address " +
                    "INNER JOIN branch AS b ON u.id_branch = b.id_branch " +
                    "WHERE u.status = 1 " +
                        "AND u.name LIKE ? " +
                        "AND IF(? = 0, u.id_branch = ?, TRUE) " +
                    "ORDER BY u.name ASC; ";
                MySqlCommand command = new MySqlCommand(str, SqlConnection);
                command.Parameters.Add("?", DbType.String).Value = '%' + text + '%';
                command.Parameters.Add("?", DbType.Int32).Value = typeSearch;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                List<User> users = new List<User>();
                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idUser = await reader.GetFieldValueAsync<int>(0);
                    string name = await reader.GetFieldValueAsync<string>(1);
                    TypeUser typeUser = (TypeUser)await reader.GetFieldValueAsync<int>(2);
                    long? telephone = null;

                    if (await reader.IsDBNullAsync(3) == false)
                    {
                        telephone = await reader.GetFieldValueAsync<long>(3);
                    }

                    int idAddress = await reader.GetFieldValueAsync<int>(4);
                    string city = await reader.GetFieldValueAsync<string>(5);
                    string neighborhood = await reader.GetFieldValueAsync<string>(6);
                    string street = await reader.GetFieldValueAsync<string>(7);
                    string number = await reader.GetFieldValueAsync<string>(8);
                    string complement = await reader.GetFieldValueAsync<string>(9);

                    int idBranch = await reader.GetFieldValueAsync<int>(10);
                    string nameBranch = await reader.GetFieldValueAsync<string>(11);

                    Address address = new Address
                    {
                        IdAddress = idAddress,
                        City = city,
                        Neighborhood = neighborhood,
                        Street = street,
                        Number = number,
                    };

                    Branch userBranch = new Branch
                    {
                        IdBranch = idBranch,
                        Name = nameBranch,
                    };

                    User user = new User
                    {
                        IdUser = idUser,
                        Name = name,
                        TypeUser = typeUser,
                        Telephone = telephone,
                        Address = address,
                        Branch = userBranch,
                    };

                    users.Add(user);
                }

                return users;
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

        public async Task Delete(int idUser)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string str = "" +
                    "UPDATE users SET status = 0 WHERE id_user = ?";

                MySqlCommand command = new MySqlCommand(str, SqlConnection, transaction);
                command.Parameters.Add("?", DbType.Int32).Value = idUser;

                _ = await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
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

        private async Task Insert(User user)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand command = new MySqlCommand(SqlConnection, transaction);

                user.Address.IdAddress = await new DAOAddress().Insert(user.Address, command);

                string str = "" +
                    "INSERT INTO users(id_type_user, id_branch, name, user_name, password, birth_date, telephone, id_address, status) " +
                    "VALUES (?, ?, ?, ?, AES_ENCRYPT(?, 'bibliotech2021'), ?, ?, ?, 1);";

                command.Parameters.Clear();
                command.CommandText = str;
                command.Parameters.Add("?", DbType.Int32).Value = user.TypeUser;
                command.Parameters.Add("?", DbType.Int32).Value = user.Branch.IdBranch;
                command.Parameters.Add("?", DbType.String).Value = user.Name;
                command.Parameters.Add("?", DbType.String).Value = user.UserName;
                command.Parameters.Add("?", DbType.String).Value = user.Password;
                command.Parameters.Add("?", DbType.Date).Value = user.BirthDate;
                command.Parameters.Add("?", DbType.Int64).Value = user.Telephone;
                command.Parameters.Add("?", DbType.Int32).Value = user.Address.IdAddress;

                _ = await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
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

        private async Task Update(User user)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand command = new MySqlCommand(SqlConnection, transaction);

                await new DAOAddress().Update(user.Address, command);

                string sql = "" +
                    "UPDATE users " +
                    "SET id_type_user = ?, id_branch = ?, name = ?, user_name = ?, password = AES_ENCRYPT(?, 'bibliotech2021'), birth_date = ?, telephone = ? " +
                    "WHERE id_user = ?;";
                command.CommandText = sql;

                command.Parameters.Clear();
                command.Parameters.Add("?", DbType.Int32).Value = user.TypeUser;
                command.Parameters.Add("?", DbType.Int32).Value = user.Branch.IdBranch;
                command.Parameters.Add("?", DbType.String).Value = user.Name;
                command.Parameters.Add("?", DbType.String).Value = user.UserName;
                command.Parameters.Add("?", DbType.String).Value = user.Password;
                command.Parameters.Add("?", DbType.DateTime).Value = user.BirthDate;
                command.Parameters.Add("?", DbType.Int64).Value = user.Telephone;
                command.Parameters.Add("?", DbType.Int32).Value = user.IdUser;

                _ = await command.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
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

        public async Task Save(User user)
        {
            if (user.IdUser == -1)
            {
                await Insert(user);
            }
            else
            {
                await Update(user);
            }
        }

        public async Task<bool> UserNameExists(string userName)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT COUNT(id_user) " +
                    "FROM users " +
                    "WHERE BINARY user_name = ? " +
                        "AND status = 1;";
                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.String).Value = userName;

                object result = await command.ExecuteScalarAsync();
                return int.Parse(result.ToString()) > 0;
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

        public async Task<User> GetUserById(int idUser)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT " +
                        "u.id_type_user, u.name, u.user_name, CAST(AES_DECRYPT(u.password, 'bibliotech2021') AS CHAR), u.birth_date, u.telephone, " +
                        "b.id_branch, b.name, " +
                        "a.id_address, a.city, a.neighborhood, a.street, a.number, a.complement " +
                    "FROM users AS u " +
                    "INNER JOIN branch AS b ON u.id_branch = b.id_branch " +
                    "INNER JOIN address AS a ON u.id_address = a.id_address " +
                    "WHERE u.id_user = ?;";
                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = idUser;

                User user = new User();
                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    TypeUser typeUser = (TypeUser)await reader.GetFieldValueAsync<int>(0);
                    string name = await reader.GetFieldValueAsync<string>(1);
                    string userName = await reader.GetFieldValueAsync<string>(2);
                    string password = await reader.GetFieldValueAsync<string>(3);
                    object objectBirthDate = await reader.GetFieldValueAsync<object>(4);
                    object objectTelephone = await reader.GetFieldValueAsync<object>(5);

                    int idBranch = await reader.GetFieldValueAsync<int>(6);
                    string nameBranch = await reader.GetFieldValueAsync<string>(7);

                    int idAddress = await reader.GetFieldValueAsync<int>(8);
                    string city = await reader.GetFieldValueAsync<string>(9);
                    string neighborhood = await reader.GetFieldValueAsync<string>(10);
                    string street = await reader.GetFieldValueAsync<string>(11);
                    string number = await reader.GetFieldValueAsync<string>(12);
                    string complement = await reader.GetFieldValueAsync<string>(13);

                    DateTime? birthDate = null;
                    if (DateTime.TryParse(objectBirthDate.ToString(), out DateTime result))
                    {
                        birthDate = result;
                    }

                    long? telephone = null;
                    if (long.TryParse(objectTelephone.ToString(), out long resultLong))
                    {
                        telephone = resultLong;
                    }

                    Branch branch = new Branch()
                    {
                        IdBranch = idBranch,
                        Name = nameBranch,
                    };

                    Address address = new Address()
                    {
                        IdAddress = idAddress,
                        City = city,
                        Neighborhood = neighborhood,
                        Street = street,
                        Number = number,
                        Complement = complement
                    };

                    user = new User()
                    {
                        IdUser = idUser,
                        TypeUser = typeUser,
                        Branch = branch,
                        Name = name,
                        UserName = userName,
                        Password = password,
                        BirthDate = birthDate,
                        Telephone = telephone,
                        Address = address,
                        Status = Status.Active,
                    };
                }

                return user;
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
                    "SELECT COUNT(id_user) FROM users WHERE status = 1;";

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
    }
}