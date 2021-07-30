using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOUser : Connection
    {
#pragma warning disable IDE0017 // Simplificar a inicialização de objeto

        public User User = new User();
        public async Task<User> IsValidUser(string user, string password)
        {
            try
            {
                await Connect();
                string select = " select u.id_user, u.id_type_user, u.name, b.id_branch, b.name, b.id_address, b.telephone " +
                    " from users as u " +
                    " inner join branch as b on b.id_branch = u.id_branch " +
                    " where user_name = @user and aes_decrypt(password, 'bibliotech2021') = @password;";

                MySqlCommand cmd = new MySqlCommand(select, SqlConnection);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@password", password);
                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                User User = null;

                while (await reader.ReadAsync())
                {
                    int idUser = reader.GetInt32(0);
                    TypeUser typeUser = (TypeUser)reader.GetInt32(1);
                    string nameUser = reader.GetString(2);
                    int idBranch = reader.GetInt32(3);
                    string nameBranch = reader.GetString(4);
                    int idAddressBranch = reader.GetInt32(5);
                    long telephone = reader.GetInt64(6);

                    User = new User(idUser, typeUser, nameUser);
                    Address address = new Address();
                    address.IdAddress = idAddressBranch;
                    Branch school = new Branch(idBranch, nameBranch, address, telephone);
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
        public async Task<DataView> SearchByText(TypeSearch typeSearch, string text, Branch branch)
        {
            try
            {
                await Connect();

                string str = "" +
                    "SELECT u.id_user, u.name, u.telephone, CONCAT(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number) AS address, u.birth_date, tu.description " +
                    "FROM users AS u " +
                    "INNER JOIN type_users AS tu ON u.id_type_user = tu.id_type_user " +
                    "INNER JOIN address AS a ON u.id_address = a.id_address " +
                    "WHERE u.status = 1 " +
                        "AND u.name LIKE ? " +
                        "AND IF(? = 0, u.id_branch = ?, TRUE) " +
                    "ORDER BY u.name ASC; ";
                MySqlCommand command = new MySqlCommand(str, SqlConnection);
                command.Parameters.Add("?", DbType.String).Value = '%' + text + '%';
                command.Parameters.Add("?", DbType.Int32).Value = typeSearch;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                DataTable dt = new DataTable();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                _ = dataAdapter.Fill(dt);

                return dt.DefaultView;
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
                    "WHERE user_name = ? " +
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
    }
}