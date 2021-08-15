using Bibliotech.Model.Entities;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOLending : Connection
    {
        public async Task<List<Lending>> SearchLendings()
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT e.id_exemplary, b.title, b.subtitle, lc.id_lector, lc.name " +
                    "FROM lending AS l " +
                    "INNER JOIN lending_has_exemplary AS le ON l.id_lending = le.id_lending " +
                    "INNER JOIN exemplary AS e ON le.id_exemplary = e.id_exemplary " +
                    "INNER JOIN book AS b ON e.id_book = b.id_book " +
                    "INNER JOIN lector AS lc ON l.id_lector = lc.id_lector;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);

                List<Lending> lendings = new List<Lending>();
                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idExemplary = await reader.GetFieldValueAsync<int>(0);
                    string titleBook = await reader.GetFieldValueAsync<string>(1);
                    string subtitleBook = await reader.GetFieldValueAsync<string>(2);
                    int idLector = await reader.GetFieldValueAsync<int>(3);
                    string nameLector = await reader.GetFieldValueAsync<string>(4);

                    Book book = new Book
                    {
                        Title = titleBook,
                        Subtitle = subtitleBook,
                    };

                    Exemplary exemplary = new Exemplary
                    {
                        IdExemplary = idExemplary,
                        Book = book,
                    };

                    Lector lector = new Lector
                    {
                        IdLector = idLector,
                        Name = nameLector,
                    };

                    Lending lending = new Lending
                    {
                        Lector = lector,
                        Exemplaries = new List<Exemplary>() { exemplary },
                    };
                    lendings.Add(lending);
                }

                return lendings;
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
