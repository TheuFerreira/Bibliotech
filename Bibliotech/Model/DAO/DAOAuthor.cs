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
        public async Task InsertAuthor(Author author)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string insertAuthor = "insert into author(name, status) values (?, 1); ";

                cmd.CommandText = insertAuthor;
                cmd.Parameters.Add("?", DbType.String).Value = author.Name;

                await cmd.ExecuteNonQueryAsync();
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
        public async Task<List<Author>> GetAuthor(string text)
        {
            try
            {
                await Connect();
                string selectAuthor = "select id_author, name " +
                    "from author where name like '%" + text + "%' and author.status = 1; ";

                MySqlCommand cmd = new MySqlCommand(selectAuthor, SqlConnection);
                cmd.CommandText = selectAuthor;

                cmd.Parameters.Clear();
                cmd.Parameters.Add("?", DbType.String).Value = text;

                List<Author> authors = new List<Author>();
                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idAuthor = await reader.GetFieldValueAsync<int>(0);
                    string nameAuthor = await reader.GetFieldValueAsync<string>(1);

                    Author author = new Author()
                    {
                        IdAuthor = idAuthor,
                        Name = nameAuthor,
                    };

                    authors.Add(author);
                }

                return authors;
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
        public async Task UpdateAuthor(Author author)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string updateBook = "update author set name = ? where id_author = ? ;";

                cmd.CommandText = updateBook;
                cmd.Parameters.Add("?", DbType.String).Value = author.Name;
                cmd.Parameters.Add("?", DbType.Int32).Value = author.IdAuthor;

                await cmd.ExecuteNonQueryAsync();
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
    }
}
