using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOExamplary : Connection
    {
        public async Task<List<Exemplary>> GetExemplarysByBook(Book book)
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
                    "ORDER BY e.id_index ASC; ";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = book.IdBook;

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
    }
}
