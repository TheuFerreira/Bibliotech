﻿using Bibliotech.Model.Entities;
using Bibliotech.View.Reports.CustomEnums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOBook : Connection
    {
        public async Task InsertBook(Book book, List<int> idAuthors)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string insertBook = "insert into book(title, subtitle, publishing_company, gender, " +
                    "edition, pages, year_publication, language, volume, collection, status ) values  (?, ?, ?, ?, ?, ?, ?, ?, ?, ?,  1); select last_insert_id();";

                cmd.Parameters.Clear();
                cmd.CommandText = insertBook;
                cmd.Parameters.Add("?", DbType.String).Value = book.Title;
                cmd.Parameters.Add("?", DbType.String).Value = book.Subtitle;
                cmd.Parameters.Add("?", DbType.String).Value = book.PublishingCompany;
                cmd.Parameters.Add("?", DbType.String).Value = book.Gender;
                cmd.Parameters.Add("?", DbType.String).Value = book.Edition;
                if (book.Pages == 0)
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

                var result = await cmd.ExecuteScalarAsync();
                int idBook = Convert.ToInt32(result);

                for (int i = 0; i < idAuthors.Count; i++)
                {
                    cmd.Parameters.Clear();
                    string insertBookHasAuthor = "insert book_has_author (id_book, id_author) values (?, ?) ; ";
                    cmd.CommandText = insertBookHasAuthor;
                    int idAuthor = idAuthors[i];
                    cmd.Parameters.Add("?", DbType.Int32).Value = idBook;
                    cmd.Parameters.Add("?", DbType.Int32).Value = idAuthor;
                    await cmd.ExecuteNonQueryAsync();
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

        public async Task<List<Book>> GetBook(string text)
        {
            await Connect();
            
            try
            {
                Book book = new Book();

                string selectBook = "select b.id_book, b.title, b.subtitle, b.publishing_company, group_concat(distinct name separator ', ') as Name, b.gender, " +
                    "b.edition, b.pages, b.year_publication, b.language, b.volume, b.collection from book_has_author as bookauthor " +
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
                    string name = await reader.GetFieldValueAsync<string>(4);
                    string gender = await reader.GetFieldValueAsync<string>(5);
                    string edition = await reader.GetFieldValueAsync<string>(6);
                    int? pages = null;
                    if (await reader.IsDBNullAsync(7) == false)
                    {
                        pages = await reader.GetFieldValueAsync<int>(7);
                    }
                    int? yearPublication = null;
                    if (await reader.IsDBNullAsync(8) == false)
                    {
                        yearPublication = await reader.GetFieldValueAsync<int>(8);
                    }
                    string language = await reader.GetFieldValueAsync<string>(9);
                    string volume = await reader.GetFieldValueAsync<string>(10);
                    string collection = await reader.GetFieldValueAsync<string>(11);

                    Author author = new Author()
                    {
                        Name = name,
                    };

                    book = new Book()
                    {
                        IdBook = idBook,
                        Title = title,
                        Subtitle = subtitle,
                        PublishingCompany = publishingCompany,
                        Author = author,
                        Gender = gender,
                        Edition = edition,
                        Pages = pages,
                        YearPublication = yearPublication,
                        Language = language,
                        Volume = volume,
                        Collection = collection,
                    };

                    book.Authors = new List<Author>();

                    book.Authors.Add(author);
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

            string strSql = "select exe.id_index, bk.title, bk.subtitle, group_concat(distinct name separator ', ') as autores, bk.publishing_company, bk.id_book, exe.id_exemplary " +
                            "from book as bk " +
                            "inner join book_has_author as bha on bha.id_book = bk.id_book " +
                            "inner join author as aut on aut.id_author = bha.id_author " +
                            "inner join exemplary as exe on exe.id_book = bk.id_book " +
                            "where bk.title like '%" + query + "%' and exe.status = 3 and exe.id_branch = " + idBranch +
                            " group by bk.id_book, exe.id_index;";

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
                    "INNER JOIN exemplary AS e ON e.id_book = b.id_book " +
                    "INNER JOIN lending AS l ON l.id_exemplary = e.id_exemplary " +
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
                    "INNER JOIN exemplary AS e ON e.id_book = b.id_book " +
                    "INNER JOIN lending AS l ON l.id_exemplary = e.id_exemplary " +
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
                    "INNER JOIN exemplary AS e ON e.id_exemplary = l.id_exemplary " +
                    "INNER JOIN book_has_author AS ba ON ba.id_book = e.id_book " +
                    "INNER JOIN author AS a ON a.id_author = ba.id_author " +
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
