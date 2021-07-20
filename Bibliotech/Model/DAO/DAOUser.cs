using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using MySqlConnector;

namespace Bibliotech.Model.DAO
{
    public class DAOUser : Connection
    { 
        public async Task<int> IsValidUser(string user, string password)
        {
           // bool result = false;
            try
            {
                await Connect();
                string select = " select u.user_name, u.password, u.id_branch, u.id_user from users as u " +
                    " inner join branch as b on b.id_branch = u.id_branch " +
                    " where and u.user = @user and u.password = @password; ";

                MySqlCommand cmd = new MySqlCommand(select, SqlConnection);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@password", password);
                //MySqlDataReader reader = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);
                //result = reader.HasRows;
                return 1;


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
