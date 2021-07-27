using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySqlConnector;
using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;

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
                
                if(!await reader.ReadAsync())
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
    }
}
