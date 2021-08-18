using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOLector : Connection
    {
        private const string BASE_REPORT_SQL_SELECT_LECTOR = "" +
            "SELECT l.id_lector, l.name, IF(pick.pickup IS NULL, 0, pick.pickup), IF(returned.returned IS NULL, 0, returned.returned) AS returned " +
            "FROM lector AS l " +
            "";

        private const string BASE_REPORT_SQL_LEFT_JOIN_PICKUP = "" +
            "LEFT JOIN( " +
                "SELECT lc.id_lector, COUNT(le.id_exemplary) AS pickup " +
                "FROM lending AS le " +
                "INNER JOIN lector AS lc ON lc.id_lector = le.id_lector " +
            "";

        private const string BASE_REPORT_SQL_LEFT_JOIN_RETURNED = "" +
            "LEFT JOIN( " +
                "SELECT lc.id_lector, COUNT(le.id_exemplary) AS returned " +
                "FROM lending AS le " +
                "INNER JOIN lector AS lc ON lc.id_lector = le.id_lector " +
            "";

        private async Task<DataView> ReportReader(MySqlCommand command)
        {
            DataTable dt = new DataTable();
            _ = dt.Columns.Add("IdLector", typeof(int));
            _ = dt.Columns.Add("Name", typeof(string));
            _ = dt.Columns.Add("Pickup", typeof(int));
            _ = dt.Columns.Add("Returned", typeof(int));

            MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await reader.ReadAsync())
            {
                int idLector = await reader.GetFieldValueAsync<int>(0);
                string name = await reader.GetFieldValueAsync<string>(1);
                int pickup = await reader.GetFieldValueAsync<int>(2);
                int returned = await reader.GetFieldValueAsync<int>(3);

                object[] values = new object[4]
                {
                        idLector,
                        name,
                        pickup,
                        returned
                };

                _ = dt.Rows.Add(values);
            }

            return dt.DefaultView;
        }

        public async Task<bool> Insert(int idBranch, Lector lector, Address address)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            //mudar formato de data
            /* DateTime.TryParseExact(lector.BirthDate.ToString(), "yyyy/MM/dd hh:mm:ss tt", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out DateTime date2);*/

            try
            {
                string strSql = "insert into address (city, neighborhood, street, number, complement) " +
                            "values(@address.City, @address.Neighborhood, @address.Street, @address.Number, @address.Complement); " +
                            "select @@identity;";

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);

                _ = cmd.Parameters.AddWithValue("@address.City", address.City);
                _ = cmd.Parameters.AddWithValue("@address.Neighborhood", address.Neighborhood);
                _ = cmd.Parameters.AddWithValue("@address.Street", address.Street);
                _ = cmd.Parameters.AddWithValue("@address.Number", address.Number);
                _ = cmd.Parameters.AddWithValue("@address.Complement", address.Complement);

                object temp = await cmd.ExecuteScalarAsync();
                int idAddress = Convert.ToInt32(temp.ToString());

                strSql = "insert into lector (id_branch, id_address, user_registration, name, responsible, birth_date, telephone) " +
                         "values(@idBranch, @idAddress, 000, @lector.Name, @lector.Responsible, @lector.BirthDate, @lector.Phone);";

                cmd.CommandText = strSql;

                _ = cmd.Parameters.AddWithValue("@idBranch", idBranch);
                _ = cmd.Parameters.AddWithValue("@idAddress", idAddress);
                _ = cmd.Parameters.AddWithValue("@lector.Name", lector.Name);
                _ = cmd.Parameters.AddWithValue("@lector.Responsible", lector.Responsible);
                _ = cmd.Parameters.AddWithValue("@lector.BirthDate", lector.BirthDate);
                _ = cmd.Parameters.AddWithValue("@lector.Phone", lector.Phone);

                _ = await cmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await Disconnect();
            }

        }

        public async Task<bool> Update(Lector lector, Address address)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {

                string strSql = "update lector " +
                                "set name = @name, responsible = @resp, birth_date = @birth, telephone = @phone " +
                                "where id_lector = @idLector;";

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);

                cmd.Parameters.AddWithValue("@name", lector.Name);
                cmd.Parameters.AddWithValue("@resp", lector.Responsible);
                cmd.Parameters.AddWithValue("@birth", lector.BirthDate);
                cmd.Parameters.AddWithValue("@phone", lector.Phone);
                cmd.Parameters.AddWithValue("@idLector", lector.IdLector);

                await cmd.ExecuteNonQueryAsync();

                strSql = "update address " +
                         "set city = @city, neighborhood = @dist, street = @street, number = @number, complement = @complement " +
                         "where id_address = @idAddress;";

                cmd.CommandText = strSql;

                cmd.Parameters.AddWithValue("@city", address.City);
                cmd.Parameters.AddWithValue("@dist", address.Neighborhood);
                cmd.Parameters.AddWithValue("@street", address.Street);
                cmd.Parameters.AddWithValue("@number", address.Number);
                cmd.Parameters.AddWithValue("@complement", address.Complement);
                cmd.Parameters.AddWithValue("@idAddress", address.IdAddress);

                await cmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        public async Task<bool> Delete(int idLector)
        {
            await Connect();

            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            string strSql = "update lector set status = " + ((int)Status.Inactive) + " where id_lector = @idLector;";

            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection, transaction);

                cmd.Parameters.AddWithValue("@idLector", idLector);

                await cmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await Disconnect();
            }
        }

        public async Task<DataTable> FillDataGrid(string query, int branch, TypeSearch typeSearch)
        {
            await Connect();
            string strSql;

            strSql = "SELECT IF(len.status IS NULL, 0, 1) AS icon, l.id_lector, l.name, l.responsible, l.birth_date, l.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number, if(a.complement is null, '', concat(', ', a.complement))) as endereco, b.name s_name, b.id_branch, a.id_address  " +
                     "FROM lector AS l " +
                     "INNER JOIN branch as b ON b.id_branch = l.id_branch " +
                     "INNER JOIN address as a ON a.id_address = l.id_address " +
                     "LEFT JOIN ( " +
                        "SELECT len.*, exe.status " +
                        "FROM lending AS len " +
                        "INNER JOIN lending_has_exemplary as lhe on lhe.id_lending = len.id_lending " +
                        "INNER JOIN exemplary as exe on exe.id_exemplary = lhe.id_exemplary " +
                        "WHERE len.return_date IS NULL" +
                        ") AS len ON len.id_lector = l.id_lector " +
                     "WHERE l.name like '%" + query + "%' and IF ( " + (int)typeSearch + " = 1, TRUE, b.id_branch = " + branch + ") and l.status = " + ((int)Status.Active) +
                     " GROUP BY l.id_lector " +
                     "LIMIT 30";

            try
            {

                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);
                _ = await cmd.ExecuteNonQueryAsync();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable("lector");

                adapter.Fill(dt);
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

        public async Task<DataTable> FillDataGrid(string query, int idlector)
        {
            await Connect();
            string strSql;

            strSql = "select IF (len.return_date IS NOT NULL, " +
                    "3, " +
                    "IF(NOW() > len.expected_date, " +
                        "5, " +
                        "IF(exe.status = 4, " +
                            "4, " +
                            "2) " +
                        ") " +
                    ") AS status, bk.title, exe.id_exemplary, len.loan_date, if(len.return_date is null, 'N/A', len.return_date) as return_date1 " +
                     "from lending as len " +
                     "inner join lector as lec on lec.id_lector = len.id_lector " +
                     "INNER JOIN lending_has_exemplary as lhe on lhe.id_lending = len.id_lending " +
                     "INNER JOIN exemplary as exe on exe.id_exemplary = lhe.id_exemplary " +
                     "inner join book as bk on bk.id_book = exe.id_book " +
                     "where lec.id_lector = " + idlector + " and bk.title like '%" + query + "%';";



            try
            {
                MySqlCommand cmd = new MySqlCommand(strSql, SqlConnection);
                _ = await cmd.ExecuteNonQueryAsync();

                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable("lector");

                _ = adapter.Fill(dt);
                return dt;
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

        public async Task<DataView> ReportSearchByDay(DateTime day)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_REPORT_SQL_SELECT_LECTOR +
                    BASE_REPORT_SQL_LEFT_JOIN_PICKUP +
                        "WHERE le.return_date IS NULL AND DATE(le.loan_date) = ? " +
                        "GROUP BY lc.id_lector) AS pick ON pick.id_lector = l.id_lector " +
                    BASE_REPORT_SQL_LEFT_JOIN_RETURNED +
                        "WHERE le.return_date IS NOT NULL AND DATE(le.loan_date) = ? " +
                        "GROUP BY lc.id_lector) AS returned ON returned.id_lector = l.id_lector; " +
                    "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Date).Value = day.Date;
                command.Parameters.Add("?", DbType.Date).Value = day.Date;

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

        public async Task<DataView> ReportSearchByMonth(int year, int month)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_REPORT_SQL_SELECT_LECTOR +
                    BASE_REPORT_SQL_LEFT_JOIN_PICKUP +
                        "WHERE le.return_date IS NULL AND YEAR(le.loan_date) = ? AND MONTH(le.loan_date) = ? " +
                        "GROUP BY lc.id_lector) AS pick ON pick.id_lector = l.id_lector " +
                    BASE_REPORT_SQL_LEFT_JOIN_RETURNED +
                        "WHERE le.return_date IS NOT NULL AND YEAR(le.loan_date) = ? AND MONTH(le.loan_date) = ? " +
                        "GROUP BY lc.id_lector) AS returned ON returned.id_lector = l.id_lector; " +
                    "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = month;
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = month;

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

        public async Task<DataView> ReportSearchByYear(int year)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_REPORT_SQL_SELECT_LECTOR +
                    BASE_REPORT_SQL_LEFT_JOIN_PICKUP +
                        "WHERE le.return_date IS NULL AND YEAR(le.loan_date) = ? " +
                        "GROUP BY lc.id_lector) AS pick ON pick.id_lector = l.id_lector " +
                    BASE_REPORT_SQL_LEFT_JOIN_RETURNED +
                        "WHERE le.return_date IS NOT NULL AND YEAR(le.loan_date) = ? " +
                        "GROUP BY lc.id_lector) AS returned ON returned.id_lector = l.id_lector; " +
                    "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = year;

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

        public async Task<DataView> ReportSearchByCustomTime(DateTime start, DateTime end)
        {
            try
            {
                await Connect();

                string sql = "" +
                    BASE_REPORT_SQL_SELECT_LECTOR +
                    BASE_REPORT_SQL_LEFT_JOIN_PICKUP +
                        "WHERE le.return_date IS NULL AND DATE(le.loan_date) >= ? AND DATE(le.loan_date) <= ? " +
                        "GROUP BY lc.id_lector) AS pick ON pick.id_lector = l.id_lector " +
                    BASE_REPORT_SQL_LEFT_JOIN_RETURNED +
                        "WHERE le.return_date IS NOT NULL AND DATE(le.loan_date) >= ? AND DATE(le.loan_date) <= ? " +
                        "GROUP BY lc.id_lector) AS returned ON returned.id_lector = l.id_lector; " +
                    "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.DateTime).Value = start;
                command.Parameters.Add("?", DbType.DateTime).Value = end;
                command.Parameters.Add("?", DbType.DateTime).Value = start;
                command.Parameters.Add("?", DbType.DateTime).Value = end;

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
    }
}
