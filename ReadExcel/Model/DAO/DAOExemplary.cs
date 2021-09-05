using Bibliotech.Model.DAO;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
using ReadExcel.Model.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ReadExcel.Model.DAO
{
    public class DAOExemplary : Connection
    {
        public async Task AddListOfExemplaries(Branch branch, List<Book> books)
        {
            await Connect();

            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand command = new MySqlCommand(SqlConnection, transaction);

                foreach (Book book in books)
                {
                    await AddExemplaries(command, branch, book, book.Exemplaries.Count);
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

        private async Task<bool> AddExemplaries(MySqlCommand command, Branch branch, Book book, int numberExemplaries)
        {
            try
            {
                string sql = "" +
                    "SELECT MAX(e.id_index) " +
                    "FROM book AS b " +
                    "INNER JOIN exemplary AS e ON b.id_book = e.id_book " +
                    "WHERE b.id_book = ? " +
                        "AND e.id_branch = ? " +
                        "AND e.status != 0;";

                command.Parameters.Clear();
                command.CommandText = sql;
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

                return true;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
        }
    }
}
