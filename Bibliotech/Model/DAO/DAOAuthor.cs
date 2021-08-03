using Bibliotech.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Data;

namespace Bibliotech.Model.DAO
{
    public class DAOAuthor : Connection
    {
        public async void InsertAuthor(Author author)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();
            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);
                string insertAuthor = "insert into author(name, status) values (?, 1);";
                cmd.Parameters.Clear();
                cmd.CommandText = insertAuthor;
                cmd.Parameters.Add("?", DbType.String).Value = author.Name;

                await cmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

            }
            catch(MySqlException ex)
            {
                await transaction.RollbackAsync();
                throw ex;
            }
            finally
            {
                await Disconnect ();
            }

        }
    }
}
