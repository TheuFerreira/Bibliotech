using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySqlConnector;
using Bibliotech.Model.Entities;

namespace Bibliotech.Model.DAO
{
    public class DAOUser : Connection
    { 
        public async Task<User> IsValidUser(string user, string password)
        {
           
            try
            {
                await Connect();
                string select = " select u.id_user, u.id_type_user, " +
                    " u.name, u.user_name, u.password, b.name from users as u " +
                    " inner join branch as b on b.id_branch = u.id_branch " +
                    " where user_name = @user and aes_decrypt(password, 'bibliotech2021') = @password;";

                MySqlCommand cmd = new MySqlCommand(select, SqlConnection);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@password", password);
                MySqlDataReader reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
                
                if(!await reader.ReadAsync())
                {
                    return null;
                }
                else
                {
                    int idUser = reader.GetInt32(0);
                    int idTypeUser = reader.GetInt32(1);
                    string name = reader.GetString(2);
                    string nameBranch = reader.GetString(3);
                   
                    User User = new User(idUser, idTypeUser, name, nameBranch);

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
    }
}
