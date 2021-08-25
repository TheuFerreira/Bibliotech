using Bibliotech.Model.Entities;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOAuthor : Connection
    {
        public async Task Insert(Author author)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string insertAuthor = "insert into author(name, status) values (?, 1); ";

                MySqlCommand cmd = new MySqlCommand(insertAuthor, SqlConnection, transaction);
                _ = cmd.Parameters.Add("?", DbType.String).Value = author.Name;

                _ = await cmd.ExecuteNonQueryAsync();
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

        public async Task<List<Author>> GetAll(string text)
        {
            try
            {
                await Connect();
                string selectAuthor = "" +
                    "select a.id_author, a.name " +
                    "from author AS a " +
                    "where a.name like '%" + text + "%' " +
                        "and a.status = 1 " +
                    "order by a.name; ";

                MySqlCommand cmd = new MySqlCommand(selectAuthor, SqlConnection);

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

        public async Task Update(Author author)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string updateBook = "update author set name = ? where id_author = ? ;";

                MySqlCommand cmd = new MySqlCommand(updateBook, SqlConnection, transaction);
                cmd.Parameters.Add("?", DbType.String).Value = author.Name;
                cmd.Parameters.Add("?", DbType.Int32).Value = author.IdAuthor;

                _ = await cmd.ExecuteNonQueryAsync();
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

        public async Task RemoveAllByBook(Book book, MySqlCommand cmd)
        {
            try
            {
                cmd.Parameters.Clear();

                string deleteAllAuthors = "DELETE FROM book_has_author WHERE id_book = ?;";

                cmd.CommandText = deleteAllAuthors;
                cmd.Parameters.Add("?", DbType.Int32).Value = book.IdBook;

                _ = await cmd.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }

        public async Task AddAllInBook(Book book, MySqlCommand cmd)
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
