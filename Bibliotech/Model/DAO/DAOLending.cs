using Bibliotech.Model.Entities;
using Bibliotech.View.Reports.CustomEnums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOLending : Connection
    {
        private const string BASE_SQL_LENDING = "" +
            "SELECT e.id_index, b.title, b.subtitle, lc.id_lector, lc.name, l.id_lending " +
            "FROM lending AS l " +
            "INNER JOIN lending_has_exemplary AS le ON l.id_lending = le.id_lending " +
            "INNER JOIN exemplary AS e ON le.id_exemplary = e.id_exemplary " +
            "INNER JOIN book AS b ON e.id_book = b.id_book " +
            "INNER JOIN lector AS lc ON l.id_lector = lc.id_lector " +
            "";

        private const string BASE_SQL_TYPE_LENDING_CONDITION = "" +
            " AND IF(? = TRUE, " +
                "TRUE, " +
                "IF(? = 1 AND l.return_date IS NULL, " +
                    "TRUE, " +
                    "IF(? = 2 AND l.return_date IS NOT NULL, " +
                        "TRUE, " +
                        "FALSE" +
                    ")" +
                ")" +
            ") " +
            "";

        private void AddTypeLending(MySqlCommand command, TypeLending typeLending)
        {
            command.Parameters.Add("?", DbType.Boolean).Value = typeLending == TypeLending.Both;
            command.Parameters.Add("?", DbType.Int32).Value = typeLending;
            command.Parameters.Add("?", DbType.Int32).Value = typeLending;
        }

        private async Task<List<Lending>> ReportReader(MySqlCommand command)
        {
            List<Lending> lendings = new List<Lending>();
            MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await reader.ReadAsync())
            {
                int idIndex = await reader.GetFieldValueAsync<int>(0);
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
                    IdIndex = idIndex,
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

        public async Task<List<Lending>> SearchLendingsByDay(DateTime day, TypeLending typeLending)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_SQL_LENDING +
                    "Where DATE(l.loan_date) = ? " +
                        BASE_SQL_TYPE_LENDING_CONDITION +
                    "; ";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Date).Value = day.Date;

                AddTypeLending(command, typeLending);

                return await ReportReader(command);
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

        public async Task<List<Lending>> SearchLendingsByMonth(int month, TypeLending typeLending)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_SQL_LENDING +
                    "Where MONTH(l.loan_date) = ? " +
                        BASE_SQL_TYPE_LENDING_CONDITION +
                    ";";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = month;

                AddTypeLending(command, typeLending);

                return await ReportReader(command);
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

        public async Task<List<Lending>> SearchLendingsByYear(int year, TypeLending typeLending)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_SQL_LENDING +
                    "Where YEAR(l.loan_date) = ? " +
                        BASE_SQL_TYPE_LENDING_CONDITION +
                    ";";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = year;

                AddTypeLending(command, typeLending);

                return await ReportReader(command);
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

        public async Task<List<Lending>> SearchLendingsByCustomTime(DateTime start, DateTime end, TypeLending typeLending)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_SQL_LENDING +
                    "Where DATE(l.loan_date) >= ? AND DATE(l.loan_date) <= ? " +
                        BASE_SQL_TYPE_LENDING_CONDITION +
                    ";";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Date).Value = start;
                command.Parameters.Add("?", DbType.Date).Value = end;

                AddTypeLending(command, typeLending);

                return await ReportReader(command);
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

        public async Task<DataTable> FillDataGrid()
        {
            await Connect();

            try
            {
                string strSql = "";

            }
            catch (System.Exception)
            {

                throw;
            }





            return null;
        }
    }
}
