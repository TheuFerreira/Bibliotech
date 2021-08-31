using Bibliotech.Model.DAO;
using MySqlConnector;
using ReadExcel.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ReadExcel.Model.DAO
{
    public class DAOBook : Connection
    {
        public async Task<List<Book>> GetAll()
        {
            await Connect();

            try
            {
                string selectBook = "" +
                    "select b.id_book, b.title, b.subtitle, b.publishing_company, " +
                    "group_concat(bookauthor.id_author separator ',') AS IdAuthors, " +
                    "group_concat(name separator ',') as NameAuthors, b.gender, " +
                    "b.edition, b.pages, b.year_publication, b.language, b.volume, b.collection " +
                    "from book_has_author as bookauthor " +
                    "inner join author as a on a.id_author = bookauthor.id_author " +
                    "inner join book as b on b.id_book = bookauthor.id_book " +
                    "where b.status = 1 and a.status = 1 " +
                    "group by bookauthor.id_book; ";

                MySqlCommand cmd = new MySqlCommand(selectBook, SqlConnection);
                List<Book> books = new List<Book>();

                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idBook = await reader.GetFieldValueAsync<int>(0);
                    string title = await reader.GetFieldValueAsync<string>(1);
                    string subtitle = await reader.GetFieldValueAsync<string>(2);
                    string publishingCompany = await reader.GetFieldValueAsync<string>(3);
                    string idAuthors = await reader.GetFieldValueAsync<string>(4);
                    string nameAuthors = await reader.GetFieldValueAsync<string>(5);
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

                    int[] ids = idAuthors.Split(',').Select(x => int.Parse(x)).ToArray();
                    string[] names = nameAuthors.Split(',');

                    List<Author> authors = new List<Author>();
                    for (int i = 0; i < ids.Length; i++)
                    {
                        Author author = new Author(ids[i], names[i]);
                        authors.Add(author);
                    }

                    Book book = new Book()
                    {
                        IdBook = idBook,
                        Title = title,
                        Subtitle = subtitle,
                        Authors = authors,
                        PublishingCompany = publishingCompany,
                        Gender = gender,
                        Edition = edition,
                        Pages = pages,
                        YearPublication = yearPublication,
                        Language = language,
                        Volume = volume,
                        Collection = collection,
                    };

                    books.Add(book);
                }

                return books;
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

        public async Task InsertAll(List<Book> books)
        {
            await Connect();

            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand command = new MySqlCommand(SqlConnection, transaction);

                foreach (Book book in books)
                {
                    if (book.IdBook != -1)
                        continue;

                    await Insert(command, book);
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

        private async Task Insert(MySqlCommand cmd, Book book)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string insertBook = "insert into book(title, subtitle, publishing_company, gender, " +
                    "edition, pages, year_publication, language, volume, collection, status ) values  (?, ?, ?, ?, ?, ?, ?, ?, ?, ?,  1); select last_insert_id();";

                cmd.CommandText = insertBook;
                cmd.Parameters.Clear();

                cmd.Parameters.Add("?", DbType.String).Value = book.Title;
                cmd.Parameters.Add("?", DbType.String).Value = book.Subtitle;
                cmd.Parameters.Add("?", DbType.String).Value = book.PublishingCompany;
                cmd.Parameters.Add("?", DbType.String).Value = book.Gender;
                cmd.Parameters.Add("?", DbType.String).Value = book.Edition;
                cmd.Parameters.Add("?", DbType.Int32).Value = book.Pages == 0 ? null : (object)book.Pages;
                cmd.Parameters.Add("?", DbType.Int32).Value = book.YearPublication == 0 ? null : (object)book.YearPublication;
                cmd.Parameters.Add("?", DbType.String).Value = book.Language;
                cmd.Parameters.Add("?", DbType.String).Value = book.Volume;
                cmd.Parameters.Add("?", DbType.String).Value = book.Collection;

                var result = await cmd.ExecuteScalarAsync();
                book.IdBook = Convert.ToInt32(result);

                await new DAOAuthor().AddAllInBook(cmd, book);

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
