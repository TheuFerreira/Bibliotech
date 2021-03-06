using Bibliotech.Model.Entities;
using Bibliotech.View.Reports.CustomEnums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOBook : Connection
    {
        public async Task Insert(Book book)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string insertBook = "insert into book(title, subtitle, publishing_company, gender, " +
                    "edition, pages, year_publication, language, volume, collection, status ) values  (?, ?, ?, ?, ?, ?, ?, ?, ?, ?,  1); select last_insert_id();";

                MySqlCommand cmd = new MySqlCommand(insertBook, SqlConnection, transaction);
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

                await new DAOAuthor().AddAllInBook(book, cmd);

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

        public async Task Update(Book book)
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

                DAOAuthor daoAuthor = new DAOAuthor();

                await daoAuthor.RemoveAllByBook(book, cmd);

                await daoAuthor.AddAllInBook(book, cmd);

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

        public async Task<List<Book>> GetAll(string text)
        {
            await Connect();

            try
            {
                Book book = new Book();

                string selectBook = "" +
                    "select b.id_book, b.title, b.subtitle, b.publishing_company, " +
                    "group_concat(bookauthor.id_author separator ',') AS IdAuthors, " +
                    "group_concat(name separator ',') as NameAuthors, b.gender, " +
                    "b.edition, b.pages, b.year_publication, b.language, b.volume, b.collection " +
                    "from book_has_author as bookauthor " +
                    "inner join author as a on a.id_author = bookauthor.id_author " +
                    "inner join book as b on b.id_book = bookauthor.id_book " +
                    "where b.title like '%" + text + "%' and b.status = 1 and a.status = 1 " +
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
                        Author author = new Author
                        {
                            IdAuthor = ids[i],
                            Name = names[i],
                        };

                        authors.Add(author);
                    }

                    book = new Book()
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

        public async Task<DataTable> FillSearchDataGrid(string query, int idBranch)
        {
            await Connect();

            string strSql = "" +
                "select exe.id_index, bk.title, bk.subtitle, group_concat(distinct name separator ', ') as autores, bk.publishing_company, bk.id_book, exe.id_exemplary " +
                "from book as bk " +
                "inner join book_has_author as bha on bha.id_book = bk.id_book " +
                "inner join author as aut on aut.id_author = bha.id_author " +
                "inner join exemplary as exe on exe.id_book = bk.id_book " +
                "where (bk.title like '%" + query + "%' OR CONCAT(LPAD(exe.id_branch, 2, '0'), LPAD(bha.id_book, 6, '0'), LPAD(exe.id_index, 5, '0')) = '" + query + "') and exe.status = 3 and exe.id_branch = " + idBranch +
                " group by bk.id_book, exe.id_index order by bk.title limit 200;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);

                _ = await cmd.ExecuteNonQueryAsync();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable("book");

                _ = adapter.Fill(dt);
                return dt;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await Disconnect();
            }
        }

        public async Task<DataView> ReportSearchByTitle(Filter filter, Branch branch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT b.title, COUNT(l.id_lending) " +
                    "FROM book AS b " +
                    "LEFT JOIN exemplary AS e ON e.id_book = b.id_book " +
                    "LEFT JOIN lending AS l ON l.id_exemplary = e.id_exemplary " +
                    "WHERE IF(? = 0, e.id_branch = ?, TRUE) " +
                    "GROUP BY b.id_book;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                DataTable dt = new DataTable();
                _ = dt.Columns.Add("title", typeof(string));
                _ = dt.Columns.Add("total", typeof(int));

                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    string title = await reader.GetFieldValueAsync<string>(0);
                    int total = await reader.GetFieldValueAsync<int>(1);

                    object[] values = new object[]
                    {
                        title,
                        total,
                    };

                    _ = dt.Rows.Add(values);
                }

                return dt.DefaultView;
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

        public async Task<DataView> ReportSearchByPublishingCompany(Filter filter, Branch branch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT b.publishing_company, COUNT(l.id_lending) " +
                    "FROM book AS b " +
                    "LEFT JOIN exemplary AS e ON e.id_book = b.id_book " +
                    "LEFT JOIN lending AS l ON l.id_exemplary = e.id_exemplary " +
                    "WHERE IF(? = 0, e.id_branch = ?, TRUE) " +
                    "GROUP BY b.publishing_company;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                DataTable dt = new DataTable();
                _ = dt.Columns.Add("title", typeof(string));
                _ = dt.Columns.Add("total", typeof(int));

                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    string title = await reader.GetFieldValueAsync<string>(0);
                    int total = await reader.GetFieldValueAsync<int>(1);

                    object[] values = new object[]
                    {
                        title,
                        total,
                    };

                    _ = dt.Rows.Add(values);
                }

                return dt.DefaultView;
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

        public async Task<DataView> ReportSearchByAuthors(Filter filter, Branch branch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT a.name, COUNT(l.id_lending) " +
                    "FROM lending AS l " +
                    "LEFT JOIN exemplary AS e ON e.id_exemplary = l.id_exemplary " +
                    "LEFT JOIN book_has_author AS ba ON ba.id_book = e.id_book " +
                    "RIGHT JOIN author AS a ON a.id_author = ba.id_author " +
                    "WHERE IF(? = 0, e.id_branch = ?, TRUE) " +
                    "GROUP BY a.id_author;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                DataTable dt = new DataTable();
                _ = dt.Columns.Add("title", typeof(string));
                _ = dt.Columns.Add("total", typeof(int));

                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    string title = await reader.GetFieldValueAsync<string>(0);
                    int total = await reader.GetFieldValueAsync<int>(1);

                    object[] values = new object[]
                    {
                        title,
                        total,
                    };

                    _ = dt.Rows.Add(values);
                }

                return dt.DefaultView;
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
