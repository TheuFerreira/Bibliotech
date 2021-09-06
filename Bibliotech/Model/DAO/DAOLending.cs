using Bibliotech.Model.Entities;
using Bibliotech.Singletons;
using Bibliotech.View.Reports.CustomEnums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOLending : Connection
    {
        private const string BASE_SQL_LENDING = "" +
            "SELECT e.id_index, b.title, b.subtitle, lc.id_lector, lc.name, l.id_lending, b.id_book " +
            "FROM lending AS l " +
            "INNER JOIN exemplary AS e ON l.id_exemplary = e.id_exemplary " +
            "INNER JOIN book AS b ON e.id_book = b.id_book " +
            "INNER JOIN lector AS lc ON l.id_lector = lc.id_lector " +
            "INNER JOIN users AS u ON l.id_user = u.id_user " +
            "INNER JOIN branch AS bc ON bc.id_branch = u.id_branch " +
            "";

        private const string BASE_SQL_BY_BRANCH_CONDITION = "" +
            "AND IF(? = 0, bc.id_branch = ?, TRUE) ";

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

        public async Task<List<int>> GetYears()
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT DISTINCT YEAR(loan_date) " +
                    "FROM lending;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);

                List<int> years = new List<int>();

                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int year = await reader.GetFieldValueAsync<int>(0);

                    years.Add(year);
                }

                int currentYear = DateTime.Now.Year;
                if (years.Contains(currentYear) == false)
                {
                    years.Add(currentYear);
                }

                return years.OrderBy(x => x).ToList();
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

        private DataView ReportReader(MySqlCommand command)
        {
            DataTable dt = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            _ = adapter.Fill(dt);

            return dt.DefaultView;
        }

        public async Task<DataView> SearchLendingsByDay(DateTime day, TypeLending typeLending, Filter filter, Branch branch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_SQL_LENDING +
                    "Where DATE(l.loan_date) = ? " +
                        BASE_SQL_BY_BRANCH_CONDITION +
                        BASE_SQL_TYPE_LENDING_CONDITION +
                    "; ";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Date).Value = day.Date;
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                AddTypeLending(command, typeLending);

                return ReportReader(command);
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

        public async Task<DataView> SearchLendingsByMonth(int year, int month, TypeLending typeLending, Filter filter, Branch branch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_SQL_LENDING +
                    "WHERE YEAR(l.loan_date) = ? AND MONTH(l.loan_date) = ? " +
                        BASE_SQL_BY_BRANCH_CONDITION +
                        BASE_SQL_TYPE_LENDING_CONDITION +
                    ";";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = month;
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                AddTypeLending(command, typeLending);

                return ReportReader(command);
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

        public async Task<DataView> SearchLendingsByYear(int year, TypeLending typeLending, Filter filter, Branch branch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_SQL_LENDING +
                    "Where YEAR(l.loan_date) = ? " +
                        BASE_SQL_BY_BRANCH_CONDITION +
                        BASE_SQL_TYPE_LENDING_CONDITION +
                    ";";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                AddTypeLending(command, typeLending);

                return ReportReader(command);
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

        public async Task<DataView> SearchLendingsByCustomTime(DateTime start, DateTime end, TypeLending typeLending, Filter filter, Branch branch)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_SQL_LENDING +
                    "Where DATE(l.loan_date) >= ? AND DATE(l.loan_date) <= ? " +
                        BASE_SQL_BY_BRANCH_CONDITION +
                        BASE_SQL_TYPE_LENDING_CONDITION +
                    ";";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Date).Value = start;
                command.Parameters.Add("?", DbType.Date).Value = end;
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

                AddTypeLending(command, typeLending);

                return ReportReader(command);
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

        public async Task<bool> Insert(List<Exemplary> exemplary, Lector lector, DateTime begin, DateTime end)
        {
            await Connect();

            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            int idUser = Session.Instance.User.IdUser;

            string strSql = "insert into lending (id_exemplary, id_lector, id_user, loan_date, expected_date) " +
                                "values(@id_exemplary, @id_lector, @id_user, @loan_date, @expected_date);";

            try
            {
                for (int i = 0; i < exemplary.Count; i++)
                {

                    MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);
                    cmd.Parameters.AddWithValue("@id_exemplary", exemplary[i].IdExemplary);
                    cmd.Parameters.AddWithValue("@id_lector", lector.IdLector);
                    cmd.Parameters.AddWithValue("@id_user", idUser);
                    cmd.Parameters.AddWithValue("@loan_date", begin);
                    cmd.Parameters.AddWithValue("@expected_date", end);

                    object o = await cmd.ExecuteNonQueryAsync();
                    //MessageBox.Show(exemplary.Count().ToString() + ", " + exemplary[i].IdExemplary.ToString() + " ,retorno" + o.ToString());

                    strSql = "update exemplary set status = 2 where exemplary.id_exemplary = " + exemplary[i].IdExemplary;
                    cmd.CommandText = strSql;
                    _ = await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await Disconnect();
            }

        }
    }
}
