using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
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
                string select = " select u.id_user, u.id_type_user, u.name, b.id_branch, b.name, b.id_address, b.telephone " +
                    " from users as u " +
                    " inner join branch as b on b.id_branch = u.id_branch " +
                    " where user_name = @user and aes_decrypt(password, 'bibliotech2021') = @password;";

                MySqlCommand cmd = new MySqlCommand(select, SqlConnection);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@password", password);
                MySqlDataReader reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);

                if (!await reader.ReadAsync())
                {
                    return null;
                }
                else
                {
                    int idUser = reader.GetInt32(0);
                    TypeUser typeUser = (TypeUser)reader.GetInt32("u.id_type_user");
                    string nameUser = reader.GetString(2);
                    int statusUser = reader.GetInt32(3);
                    int idBranch = reader.GetInt32(4);
                    string nameBranch = reader.GetString(5);
                    int idAddressBranch = reader.GetInt32(6);
                    long telephone = reader.GetInt64(7);

                    User User = new User(idUser, typeUser, nameUser);
                    School school = new School(idBranch, nameBranch, idAddressBranch, telephone);

                    return User;

                }

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

        public async Task<DataView> SearchByText(string text)
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
                        "AND u.name LIKE ?; ";
                MySqlCommand command = new MySqlCommand(str, SqlConnection);
                command.Parameters.Add("?", DbType.String).Value = '%' + text + '%';

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

        public async Task<bool> Delete(int idUser)
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

                return true;
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

                user.Address.Id_address = await new DAOAddress().Insert(user.Address, command);

                string str = "" +
                    "INSERT INTO users(id_type_user, id_branch, name, user_name, password, birth_date, telephone, id_address, status) " +
                    "VALUES (?, ?, ?, ?, AES_ENCRYPT(?, 'bibliotech2021'), ?, ?, ?, 1);";

                command.Parameters.Clear();
                command.CommandText = str;
                command.Parameters.Add("?", DbType.Int32).Value = user.TypeUser;
                command.Parameters.Add("?", DbType.Int32).Value = user.Branch.Id_branch;
                command.Parameters.Add("?", DbType.String).Value = user.Name;
                command.Parameters.Add("?", DbType.String).Value = user.UserName;
                command.Parameters.Add("?", DbType.String).Value = user.Password;
                command.Parameters.Add("?", DbType.Date).Value = user.BirthDate;
                command.Parameters.Add("?", DbType.Int64).Value = user.Telephone;
                command.Parameters.Add("?", DbType.Int32).Value = user.Address.Id_address;

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

        public async Task Save(User user)
        {
            if (user.IdUser == -1)
                await Insert(user);
        }
    }
}