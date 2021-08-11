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
                return false;
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

            MySqlTransaction transaction = SqlConnection.BeginTransaction();

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
                return false;
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
                        "INNER JOIN exemplary AS exe ON len.id_exemplary = exe.id_exemplary " +
                        "WHERE len.return_date IS NULL " +
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
                     "inner join exemplary as exe on exe.id_exemplary = len.id_exemplary " +
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
    }
}
