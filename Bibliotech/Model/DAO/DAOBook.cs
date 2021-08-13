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
                if(book.Pages == 0)
                {
                    cmd.Parameters.Add("?", MySqlDbType.Null);
                }
                else
                {
                    cmd.Parameters.Add("?", DbType.Int32).Value = book.Pages;
                }

                if (book.YearPublication == 0)
                {
                    cmd.Parameters.Add("?", MySqlDbType.Null);
                }
                else
                {
                    cmd.Parameters.Add("?", DbType.Int32).Value = book.YearPublication;
                }

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
                    "edition = ?, pages = ?, year_publication = ?, language = ?, volume = ?, collection = ? " +
                    "where id_book = ? ;";
                
               
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
                cmd.Parameters.Add("?", DbType.Int32).Value = book.IdBook;

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

                string insertBookHasAuthor = "insert into book_has_author (id_book, id_author) " +
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
        

        public async Task<List<Book>> GetBook()
        {
            await Connect();
            try
            {
                Book book = new Book();

                string selectBook = "select b.id_book, a.id_author, b.title, b.subtitle, a.name, b.publishing_company, b.gender, " +
                    "b.edition, b.pages, b.year_publication, b.language, b.volume, b.collection " +
                    "from book_has_author as bookauthor " +
                    "inner join author as a on a.id_author = bookauthor.id_author " +
                    "inner join book as b on b.id_book = bookauthor.id_book " +
                    "where b.status = 1 and a.status = 1";

                MySqlCommand cmd = new MySqlCommand(selectBook, SqlConnection);
                List<Book> books = new List<Book>();
               
                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while(await reader.ReadAsync())
                {
                    int idBook  = await reader.GetFieldValueAsync<int>(0);
                    int idAuthor = await reader.GetFieldValueAsync<int>(1);
                    string title = await reader.GetFieldValueAsync<string>(2);
                    string subtitle = await reader.GetFieldValueAsync<string>(3);
                    string nameAuthor = await reader.GetFieldValueAsync<string>(4);
                    string publishingCompany = await reader.GetFieldValueAsync<string>(5);
                    string gender = await reader.GetFieldValueAsync<string>(6);
                    string edition = await reader.GetFieldValueAsync<string>(7);
                    int? pages = null;
                    if (await reader.IsDBNullAsync(8) == false)
                    {
                        pages = await reader.GetFieldValueAsync<int>(8);
                    }
                    int? yearPublication = null;
                    if (await reader.IsDBNullAsync(9) == false)
                    {
                        yearPublication = await reader.GetFieldValueAsync<int>(9);
                    }
                    string language = await reader.GetFieldValueAsync<string>(10);
                    string volume = await reader.GetFieldValueAsync<string>(11);
                    string collection = await reader.GetFieldValueAsync<string>(12);

                    Author author = new Author()
                    {
                        IdAuthor = idAuthor,
                        Name = nameAuthor,
                    };

                    book = new Book()
                    {
                        IdBook = idBook,
                        Title = title,
                        Subtitle = subtitle,
                        PublishingCompany = publishingCompany,
                        Gender = gender,
                        Edition = edition,
                        Pages = pages,
                        YearPublication = yearPublication,
                        Language = language,
                        Volume = volume,
                        Collection = collection,
                        Author = author,
                    };

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
                        "a.id_author, a.name " +
                    "FROM book AS b " +
                    "INNER JOIN book_has_author AS ba ON ba.id_book = b.id_book " +
                    "INNER JOIN author AS a ON a.id_author = ba.id_author " +
                    "WHERE b.id_book = ?;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = idBook;

                Book book = new Book();
                List<Author> authors = new List<Author>();

                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    string title = await reader.GetFieldValueAsync<string>(0);

                    int idAuthor = await reader.GetFieldValueAsync<int>(1);
                    string nameAuthor = await reader.GetFieldValueAsync<string>(2);

                    Author author = new Author
                    {
                        IdAuthor = idAuthor,
                        Name = nameAuthor,
                    };

                    authors.Add(author);

                    book = new Book
                    {
                        IdBook = idBook,
                        Title = title,
                        Authors = authors,
                    };
                }

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
