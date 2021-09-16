using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOExamplary : Connection
    {
        public async Task<List<Exemplary>> GetExemplarysByBook(Book book, TypeSearch typeSearch, Branch currentBranch, string text, Status filterStatus)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT " +
                        "e.id_exemplary, e.id_index, e.status, " +
                        "b.id_branch, b.name " +
                    "FROM exemplary AS e " +
                    "INNER JOIN branch AS b ON b.id_branch = e.id_branch " +
                    "WHERE e.id_book = ? " +
                        "AND IF(? = 1, TRUE, b.id_branch = ?) " +
                        "AND IF(? = '', TRUE, e.id_exemplary = ?) " +
                        "AND IF(? = -1, e.status != ?, e.status = ?) " +
                    "ORDER BY e.id_index ASC; ";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = book.IdBook;
                command.Parameters.Add("?", DbType.Int32).Value = typeSearch;
                command.Parameters.Add("?", DbType.Int32).Value = currentBranch.IdBranch;
                command.Parameters.Add("?", DbType.String).Value = text;
                command.Parameters.Add("?", DbType.String).Value = text;
                command.Parameters.Add("?", DbType.Int32).Value = filterStatus;
                command.Parameters.Add("?", DbType.Int32).Value = Status.Inactive;
                command.Parameters.Add("?", DbType.Int32).Value = filterStatus;

                List<Exemplary> exemplaries = new List<Exemplary>();
                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idExemplary = await reader.GetFieldValueAsync<int>(0);
                    int idIndex = await reader.GetFieldValueAsync<int>(1);
                    Status status = (Status)await reader.GetFieldValueAsync<int>(2);

                    int idBranch = await reader.GetFieldValueAsync<int>(3);
                    string nameBranch = await reader.GetFieldValueAsync<string>(4);

                    Branch branch = new Branch
                    {
                        IdBranch = idBranch,
                        Name = nameBranch,
                    };

                    Exemplary exemplary = new Exemplary
                    {
                        IdExemplary = idExemplary,
                        Book = book,
                        Branch = branch,
                        IdIndex = idIndex,
                        Status = status,
                    };

                    exemplaries.Add(exemplary);
                }

                return exemplaries;
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

        public async Task<bool> SetGiveBack(Exemplary exemplary, DateTime date)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string sql = "" +
                    "UPDATE exemplary " +
                    "SET status = ? " +
                    "WHERE id_exemplary = ?;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection, transaction);
                command.Parameters.Add("?", DbType.Int32).Value = Status.Stock;
                command.Parameters.Add("?", DbType.Int32).Value = exemplary.IdExemplary;
                await command.ExecuteNonQueryAsync();

                sql = "" +
                    "UPDATE lending " +
                    "SET return_date = ? " +
                    "WHERE id_exemplary = ?;";
                command.Parameters.Clear();
                command.CommandText = sql;
                command.Parameters.Add("?", DbType.DateTime).Value = date;
                command.Parameters.Add("?", DbType.Int32).Value = exemplary.IdExemplary;

                await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

                return true;
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

        public async Task<bool> SetStatus(Exemplary exemplary, Status status)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string sql = "" +
                    "UPDATE exemplary " +
                    "SET status = ? " +
                    "WHERE id_exemplary = ?;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection, transaction);
                command.Parameters.Add("?", DbType.Int32).Value = status;
                command.Parameters.Add("?", DbType.Int32).Value = exemplary.IdExemplary;

                _ = await command.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

                return true;
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

        public async Task<bool> AddExemplaries(Branch branch, Book book, int numberExemplaries)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                string sql = "" +
                    "SELECT MAX(e.id_index) " +
                    "FROM book AS b " +
                    "INNER JOIN exemplary AS e ON b.id_book = e.id_book " +
                    "WHERE b.id_book = ? " +
                        "AND e.id_branch = ? " +
                        "AND e.status != 0;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection, transaction);
                command.Parameters.Add("?", DbType.Int32).Value = book.IdBook;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                object result = await command.ExecuteScalarAsync();
                int lastindex = 0;
                if (int.TryParse(result.ToString(), out int res))
                {
                    lastindex = res;
                }
                int nextIndex = lastindex + 1;

                for (int i = 0; i < numberExemplaries; i++)
                {
                    command.Parameters.Clear();

                    sql = "INSERT INTO exemplary(id_book, id_branch, id_index, status) VALUES (?, ?, ?, ?);";
                    command.CommandText = sql;
                    command.Parameters.Add("?", DbType.Int32).Value = book.IdBook;
                    command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;
                    command.Parameters.Add("?", DbType.Int32).Value = nextIndex;
                    command.Parameters.Add("?", DbType.Int32).Value = Status.Stock;

                    _ = await command.ExecuteNonQueryAsync();

                    nextIndex++;
                }

                await transaction.CommitAsync();

                return true;
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

        public async Task<List<Exemplary>> GetAllExemplariesByBranch(Branch branch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT b.id_book, b.title, b.subtitle, b.publishing_company, GROUP_CONCAT(a.name) as authors, e.id_index " +
                    "FROM exemplary AS e " +
                    "INNER JOIN book AS b ON b.id_book = e.id_book " +
                    "INNER JOIN book_has_author AS ba ON ba.id_book = b.id_book " +
                    "INNER JOIN author AS a ON a.id_author = ba.id_author " +
                    "WHERE e.id_branch = ? " +
                    "GROUP BY e.id_exemplary " +
                    "ORDER BY b.title, e.id_index; ";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                List<Exemplary> exemplaries = new List<Exemplary>();
                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idBook = await reader.GetFieldValueAsync<int>(0);
                    string title = await reader.GetFieldValueAsync<string>(1);
                    string subtitle = await reader.GetFieldValueAsync<string>(2);
                    string publishingCompany = await reader.GetFieldValueAsync<string>(3);
                    string concatenedAuthors = await reader.GetFieldValueAsync<string>(4);
                    int idIndex = await reader.GetFieldValueAsync<int>(5);

                    List<Author> authors = concatenedAuthors.Split(',').Select(x => new Author
                    {
                        Name = x,
                    }).ToList();

                    Book book = new Book
                    {
                        IdBook = idBook,
                        Title = title,
                        Subtitle = subtitle,
                        PublishingCompany = publishingCompany,
                        Authors = authors,
                    };

                    Exemplary exemplary = new Exemplary
                    {
                        Branch = branch,
                        Book = book,
                        IdIndex = idIndex,
                    };

                    exemplaries.Add(exemplary);
                }

                return exemplaries;
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
