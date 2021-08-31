using Bibliotech.Model.DAO;
using MySqlConnector;
using ReadExcel.Model.Entities;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ReadExcel.Model.DAO
{
    public class DAOAuthor : Connection
    {
        public async Task InsertAll(List<Author> authors)
        {
            await Connect();

            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string sql = "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection, transaction);

                foreach (Author author in authors)
                {
                    if (author.IdAuthor != -1)
                        continue;

                    await Insert(command, author);
                }

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

        private async Task Insert(MySqlCommand command, Author author)
        {
            try
            {
                command.Parameters.Clear();

                string sql = "" +
                    "INSERT INTO author(name, status) VALUES (?, 1);" +
                    "SELECT last_insert_id();";
                command.CommandText = sql;
                command.Parameters.Add("?", MySqlDbType.String).Value = author.Name;

                object result = await command.ExecuteScalarAsync();
                author.IdAuthor = int.Parse(result.ToString());
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        public async Task<List<Author>> GetAll()
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT id_author, name " +
                    "FROM author ";
                MySqlCommand command = new MySqlCommand(sql, SqlConnection);

                List<Author> authors = new List<Author>();
                MySqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    int idAuthor = await reader.GetFieldValueAsync<int>(0);
                    string name = await reader.GetFieldValueAsync<string>(1);

                    Author author = new Author(idAuthor, name);
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

        public async Task AddAllInBook(MySqlCommand cmd, Book book)
        {
            try
            {
                for (int i = 0; i < book.Authors.Count; i++)
                {
                    cmd.Parameters.Clear();

                    string insertBookHasAuthor = "insert book_has_author (id_book, id_author) values (?, ?) ; ";
                    cmd.CommandText = insertBookHasAuthor;

                    Author author = book.Authors[i];
                    cmd.Parameters.Add("?", DbType.Int32).Value = book.IdBook;
                    cmd.Parameters.Add("?", DbType.Int32).Value = author.IdAuthor;

                    _ = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
    }
}
