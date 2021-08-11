using Bibliotech.Model.Entities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOBook : Connection
    {
        private readonly Author author;
        public async Task InsertBook(Book book)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string insertBook = "insert into book(title, subtitle, publishing_company, gender, " +
                    "edition, pages, year_publication, language, volume, collection, status ) values  (?, ?, ?, ?, ?, ?, ?, ?, ?, ?,  1); ";

                cmd.Parameters.Clear();
                cmd.CommandText = insertBook;
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
        public async Task UpdateBook(Book book)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string updateBook = "update book set title = ?, subtitle = ?, publishing_company = ?, gender = ?, " +
                    "edition = ?, pages = ?, year_publication = ?, language = ?, volume = ?, collection = ?, status = ?" +
                    "where id_book = ? ;";
                // falta update author
               
                cmd.CommandText = updateBook;
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
        public async Task BookHasAuthor()
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string insertBookHasAuthor = "insert into bookHasAuthor (id_book, id_author) " +
                    " select b.id_book, a.id_author from book as b, author as a " +
                    "order by id_book desc, id_author desc limit 1;"; 

                cmd.Parameters.Clear();
                cmd.CommandText = insertBookHasAuthor;

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
        public async Task<DataTable> SearchBooks()
        {
            try
            {
                await Connect();
                string selectBook = "select b.id_book, b.title, b.subtitle, a.name as author, b.publishing_company as publishingCompany " +
                    "from bookhasauthor as bookauthor " +
                    "inner join author as a on a.id_author = bookauthor.id_author " +
                    "inner join book as b on b.id_book = bookauthor.id_book " +
                    "where b.status = 1 ";

                MySqlCommand cmd = new MySqlCommand(selectBook, SqlConnection);

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                adapter.Fill(dt);

                return dt;

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

        public async Task<List<Book>> GetBook()
        {
            await Connect();
            try
            {
                Book book = new Book();

                string selectBook = "select b.id_book, a.id_author, b.title as title, b.subtitle as subtitle, a.name as author, b.publishing_company as publishingCompany, b.gender, " +
                    "b.edition, b.pages, b.year_publication, b.language, b.volume, b.collection " +
                    "from bookhasauthor as bookauthor " +
                    "inner join author as a on a.id_author = bookauthor.id_author " +
                    "inner join book as b on b.id_book = bookauthor.id_book " +
                    "where b.status = 1 ";

                MySqlCommand cmd = new MySqlCommand(selectBook, SqlConnection);
                List<Book> books = new List<Book>();
               
                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while(await reader.ReadAsync())
                {
                    book.IdBook  = await reader.GetFieldValueAsync<int>(0);
                   // book.Author.IdAuthor = await reader.GetFieldValueAsync<int>(1);
                    book.Title = await reader.GetFieldValueAsync<string>(2);
                    book.Subtitle = await reader.GetFieldValueAsync<string>(3);
                   // book.Author.Name = await reader.GetFieldValueAsync<string>(4);
                    book.PublishingCompany = await reader.GetFieldValueAsync<string>(5);
                    book.Gender = await reader.GetFieldValueAsync<string>(6);
                    book.Edition = await reader.GetFieldValueAsync<string>(7);
                    book.Pages = await reader.GetFieldValueAsync<int>(8);
                    book.YearPublication = await reader.GetFieldValueAsync<int>(9);
                    book.Language = await reader.GetFieldValueAsync<string>(10);
                    book.Volume = await reader.GetFieldValueAsync<string>(11);
                    book.Collection = await reader.GetFieldValueAsync<string>(12);

                    books.Add(book);
                }

                return books;
            }
            catch(MySqlException ex)
            {
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
