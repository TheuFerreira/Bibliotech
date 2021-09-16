using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using Bibliotech.View.Reports.CustomEnums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOLector : Connection
    {
        private const string BASE_REPORT_SQL_SELECT_LECTOR = "" +
            "SELECT l.id_lector, l.name, IF(pick.pickup IS NULL, 0, pick.pickup), IF(returned.returned IS NULL, 0, returned.returned) AS returned " +
            "FROM lector AS l " +
            "INNER JOIN branch AS b ON b.id_branch = l.id_branch " +
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

        private const string BASE_REPORT_SQL_WHERE_CONDITION = "" +
            "WHERE IF (? = 0, b.id_branch = ?, TRUE) ";

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

        public async Task<Lector> GetById(int idLector)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT " +
                    "l.name, l.responsible, l.birth_date, l.telephone, " +
                    "a.id_address, a.city, a.neighborhood, a.street, a.number, a.complement, " +
                    "b.id_branch " +
                    "FROM lector AS l " +
                    "INNER JOIN address AS a ON a.id_address = l.id_address " +
                    "INNER JOIN branch AS b ON b.id_branch = l.id_branch " +
                    "WHERE l.id_lector = ?;";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = idLector;

                MySqlDataReader reader = await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                await reader.ReadAsync();

                string name = await reader.GetFieldValueAsync<string>(0);

                string responsible = "";
                if (await reader.IsDBNullAsync(1) == false)
                {
                    responsible = await reader.GetFieldValueAsync<string>(1);
                }

                DateTime? birthDate = null;
                if (await reader.IsDBNullAsync(2) == false)
                {
                    birthDate = await reader.GetFieldValueAsync<DateTime>(2);
                }

                long? telephone = null;
                if (await reader.IsDBNullAsync(3) == false)
                {
                    telephone = await reader.GetFieldValueAsync<long>(3);
                }

                int idAddress = await reader.GetFieldValueAsync<int>(4);
                string city = await reader.GetFieldValueAsync<string>(5);
                string neighborhood = await reader.GetFieldValueAsync<string>(6);
                string street = await reader.GetFieldValueAsync<string>(7);
                string number = await reader.GetFieldValueAsync<string>(8);

                string complement = "";
                if (await reader.IsDBNullAsync(9) == false)
                {
                    complement = await reader.GetFieldValueAsync<string>(9);
                }

                int idBranch = await reader.GetFieldValueAsync<int>(10);

                Address address = new Address
                {
                    IdAddress = idAddress,
                    City = city,
                    Neighborhood = neighborhood,
                    Street = street,
                    Number = number,
                    Complement = complement,
                };

                Lector lector = new Lector
                {
                    IdLector = idLector,
                    IdBranch = idBranch,
                    Address = address,
                    Name = name,
                    Responsible = responsible,
                    BirthDate = birthDate,
                    Phone = telephone,
                };

                return lector;
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

            strSql = "SELECT IF(len.status IS NULL, 0, 1) AS icon, l.id_lector, l.name, l.responsible, " +
                     "l.birth_date, l.telephone, " +
                     "concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number, if(a.complement is null, '', concat(', ', a.complement))) as endereco, " +
                     "b.name s_name, b.id_branch, a.id_address  " +
                     "FROM lector AS l " +
                     "INNER JOIN branch as b ON b.id_branch = l.id_branch " +
                     "INNER JOIN address as a ON a.id_address = l.id_address " +
                     "LEFT JOIN ( " +
                        "SELECT len.*, exe.status " +
                        "FROM lending AS len " +
                        "INNER JOIN exemplary as exe on exe.id_exemplary = len.id_exemplary " +
                        "WHERE len.return_date IS NULL" +
                        ") AS len ON len.id_lector = l.id_lector " +
                     "WHERE l.name like '%" + query + "%' and IF ( " + (int)typeSearch + " = 1, TRUE, b.id_branch = " + branch + ") and l.status = " + ((int)Status.Active) +
                     " GROUP BY l.id_lector;";

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

            string strSql = "select IF (len.return_date IS NOT NULL, " +
                    "3, " +
                    "IF(NOW() > len.expected_date, " +
                        "5, " +
                        "IF(exe.status = 4, " +
                            "4, " +
                            "2) " +
                        ") " +
                    ") AS status, bk.title, exe.id_index, len.loan_date, if(len.return_date is null, 'N/A', len.return_date) as return_date1 " +
                     "from lending as len " +
                     "inner join lector as lec on lec.id_lector = len.id_lector " +
                     "INNER JOIN exemplary as exe on exe.id_exemplary = len.id_exemplary " +
                     "inner join book as bk on bk.id_book = exe.id_book " +
                     "where lec.id_lector = " + idlector + " " +
                        "AND (bk.title like '%" + query + "%' OR CONCAT(LPAD(exe.id_branch, 2, '0'), LPAD(bk.id_book, 6, '0'), LPAD(exe.id_index, 5, '0')) = '" + query + "') " +
                    "ORDER BY len.loan_date DESC;";

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

        public async Task<DataView> ReportSearchByDay(DateTime day, Filter filter, Branch branch)
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
                        "GROUP BY lc.id_lector) AS returned ON returned.id_lector = l.id_lector " +
                    BASE_REPORT_SQL_WHERE_CONDITION +
                    "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Date).Value = day.Date;
                command.Parameters.Add("?", DbType.Date).Value = day.Date;
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

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

        public async Task<DataView> ReportSearchByMonth(int year, int month, Filter filter, Branch branch)
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
                        "GROUP BY lc.id_lector) AS returned ON returned.id_lector = l.id_lector " +
                    BASE_REPORT_SQL_WHERE_CONDITION +
                    "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = month;
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = month;
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

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

        public async Task<DataView> ReportSearchByYear(int year, Filter filter, Branch branch)
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
                        "GROUP BY lc.id_lector) AS returned ON returned.id_lector = l.id_lector " +
                    BASE_REPORT_SQL_WHERE_CONDITION +
                    "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = year;
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

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

        public async Task<DataView> ReportSearchByCustomTime(DateTime start, DateTime end, Filter filter, Branch branch)
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
                        "GROUP BY lc.id_lector) AS returned ON returned.id_lector = l.id_lector " +
                    BASE_REPORT_SQL_WHERE_CONDITION +
                    "";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.DateTime).Value = start;
                command.Parameters.Add("?", DbType.DateTime).Value = end;
                command.Parameters.Add("?", DbType.DateTime).Value = start;
                command.Parameters.Add("?", DbType.DateTime).Value = end;
                command.Parameters.Add("?", DbType.Int32).Value = filter;
                command.Parameters.Add("?", DbType.Int32).Value = branch.IdBranch;

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

        public async Task<List<Lector>> GetLectors(int idBranch, string text)
        {
            List<Lector> lectors = new List<Lector>();
            try
            {
                await Connect();
                string selectLector = "" +
                    "select l.id_lector, l.name " +
                    "from lector as l " +
                    "inner join lending as le on le.id_lector = l.id_lector " +
                    "inner join exemplary as e on e.id_exemplary = le.id_exemplary " +
                    "where l.id_branch = ?  and e.status = 2 and name like '%" + text + "%' " +
                    "group by l.id_lector ; ";

                MySqlCommand cmd = new MySqlCommand(selectLector, SqlConnection);

                cmd.Parameters.Clear();
                cmd.Parameters.Add("?", DbType.Int32).Value = idBranch;

                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                while (await reader.ReadAsync())
                {
                    int idLector = await reader.GetFieldValueAsync<int>(0);
                    string nameLector = await reader.GetFieldValueAsync<string>(1);

                    Lector lector = new Lector()
                    {
                        IdLector = idLector,
                        Name = nameLector,
                    };

                    lectors.Add(lector);
                }

                return lectors;
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

        public async Task<List<Exemplary>> GetBooks(int idLector)
        {
            try
            {
                await Connect();
                string selectBooks = "" +
                    "select le.id_lending, e.id_exemplary, e.id_index, b.title, b.subtitle, " +
                    "group_concat(distinct a.name separator', ') as name, b.publishing_company, " +
                    "le.expected_date as date, le.return_date " +
                    "from lending as le " +
                    "inner join exemplary as e on e.id_exemplary = le.id_exemplary " +
                    "inner join book_has_author as ba on ba.id_book = e.id_book " +
                    "inner join book as b on b.id_book = ba.id_book " +
                    "inner join author as a on a.id_author = ba.id_author " +
                    "inner join lector as lec on lec.id_lector = le.id_lector " +
                    "where lec.id_lector = ? and e.status = 2 " +
                    "group by e.id_exemplary; ";

                MySqlCommand cmd = new MySqlCommand(selectBooks, SqlConnection);

                cmd.Parameters.Clear();
                cmd.Parameters.Add("?", DbType.Int32).Value = idLector;

                List<Exemplary> exemplaries = new List<Exemplary>();
                List<Author> authors = new List<Author>();

                MySqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                while (await reader.ReadAsync())
                {
                    int idLending = await reader.GetFieldValueAsync<int>(0);
                    int idExemplary = await reader.GetFieldValueAsync<int>(1);
                    int idIndex = await reader.GetFieldValueAsync<int>(2);
                    string title = await reader.GetFieldValueAsync<string>(3);
                    string subtitle = await reader.GetFieldValueAsync<string>(4);
                    string name = await reader.GetFieldValueAsync<string>(5);
                    string publishingCompany = await reader.GetFieldValueAsync<string>(6);
                    DateTime expectedDate = await reader.GetFieldValueAsync<DateTime>(7);
                    DateTime? returnDate = null;

                    if (await reader.IsDBNullAsync(8) == false)
                    {
                        returnDate = await reader.GetFieldValueAsync<DateTime>(8);
                    }

                    Lending lending = new Lending()
                    {
                        IdLending = idLending,
                        ExpectedDate = expectedDate,
                        ReturnDate = returnDate,
                    };

                    Author author = new Author()
                    {
                        Name = name,
                    };

                    authors.Add(author);

                    Book book = new Book()
                    {
                        Title = title,
                        Subtitle = subtitle,
                        Authors = authors,
                        PublishingCompany = publishingCompany,
                    };

                    Exemplary exemplary = new Exemplary()
                    {
                        Lending = lending,
                        IdExemplary = idExemplary,
                        IdIndex = idIndex,
                        Book = book,
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

        public async Task GetStatusDevolution(int status, int idExemplary, int idLending, DateTime devolution)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string update = "update exemplary set status = ? where id_exemplary = ?; ";

                cmd.CommandText = update;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("?", DbType.Int32).Value = status;
                cmd.Parameters.Add("?", DbType.Int32).Value = idExemplary;

                await cmd.ExecuteNonQueryAsync();

                update = "update lending set return_date = ? where id_lending = ? ;  ";

                cmd.CommandText = update;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("?", DbType.Date).Value = devolution;
                cmd.Parameters.Add("?", DbType.Int32).Value = idLending;

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

        public async Task GetExtendDevolution(DateTime date, int idLending)
        {
            await Connect();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string update = "update lending set expected_date = ? where id_lending = ? ; ";

                cmd.CommandText = update;
                cmd.Parameters.Clear();
                cmd.Parameters.Add("?", DbType.Date).Value = date;
                cmd.Parameters.Add("?", DbType.Int32).Value = idLending;

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

        public async Task<bool> HasBookPending(Lector lector)
        {
            try
            {
                await Connect();

                string sql = "" +
                    "SELECT IF(len.status IS NULL, 0, 1) AS icon " +
                    "FROM lector AS l " +
                    "LEFT JOIN( " +
                        "SELECT len.*, exe.status " +
                        "FROM lending AS len " +
                        "INNER JOIN exemplary as exe on exe.id_exemplary = len.id_exemplary " +
                        "WHERE len.return_date IS NULL " +
                        ") AS len ON len.id_lector = l.id_lector " +
                    "WHERE l.id_lector = ? " +
                    "GROUP BY l.id_lector; ";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);
                command.Parameters.Add("?", DbType.Int32).Value = lector.IdLector;

                object result = await command.ExecuteScalarAsync();
                int value = int.Parse(result.ToString());

                return value > 0;
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
