using Bibliotech.Model.Entities;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOBook : Connection
    {
        private readonly Author author;
        public async void InsertBook(Book book, Author author)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string insertAuthor = "insert into author(name, status) values (?, 1); " +
                    "select @@identity; ";
                cmd.Parameters.Clear();
                cmd.CommandText = insertAuthor;
                cmd.Parameters.Add("?", DbType.String).Value = author.Name;

                object id = await cmd.ExecuteScalarAsync();
                author.IdAuthor = Convert.ToInt32(id.ToString());

                string insertBook = "insert into book(id_author, title, subtitle, publishing_company, gender, " +
                    "edition, pages, year_publication, language, volume, collection, status ) values  (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 1) ;";

                cmd.Parameters.Clear();
                cmd.CommandText = insertBook;
                cmd.Parameters.Add("?", DbType.Int32).Value = author.IdAuthor;
                cmd.Parameters.Add("?", DbType.String).Value = book.Title;
                cmd.Parameters.Add("?", DbType.String).Value = book.Subtitle;
                cmd.Parameters.Add("?", DbType.String).Value = book.PublishingCompany;
                cmd.Parameters.Add("?", DbType.String).Value = book.Gender;
                cmd.Parameters.Add("?", DbType.String).Value = book.Edition;
                cmd.Parameters.Add("?", DbType.Int32).Value = book.Pages;
                cmd.Parameters.Add("?", DbType.Int32).Value = book.YearPublication;
                cmd.Parameters.Add("?", DbType.String).Value = book.Language;
                cmd.Parameters.Add("?", DbType.String).Value = book.Volume;
                cmd.Parameters.Add("?", DbType.String).Value = book.Collection;

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

        public async Task<Book> GetById(int idBook)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT " +
                        "b.title, " +
                        "at.id_author, at.name " +
                    "FROM book AS b " +
                    "INNER JOIN author AS at ON at.id_author = b.id_author " +
                    "WHERE id_book = ?;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = idBook;

                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                _ = await reader.ReadAsync();

                string title = await reader.GetFieldValueAsync<string>(0);

                int idAuthor = await reader.GetFieldValueAsync<int>(1);
                string nameAuthor = await reader.GetFieldValueAsync<string>(2);

                Author author = new Author
                {
                    IdAuthor = idAuthor,
                    Name = nameAuthor,
                };

                Book book = new Book
                {
                    IdBook = idBook,
                    Title = title,
                    Author = author,
                };

                return book;
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
