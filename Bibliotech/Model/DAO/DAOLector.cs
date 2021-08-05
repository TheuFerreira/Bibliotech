using Bibliotech.Model.Entities;
using Bibliotech.Model.Entities.Enums;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Bibliotech.Model.DAO
{
    public class DAOLector: Connection
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

            if (typeSearch == TypeSearch.Current)
            {
                strSql = "SELECT l.id_lector, l.name, l.responsible, l.birth_date, l.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number, if(a.complement is null, '', concat(', ', a.complement))) as endereco, b.name s_name, b.id_branch, a.id_address  " +
                         "FROM lector AS l " +
                         "INNER JOIN branch as b ON b.id_branch = l.id_branch " +
                         "INNER JOIN address as a ON a.id_address = l.id_address " +
                         "WHERE l.name like '%" + query + "%' and b.id_branch = " + branch + " and l.status = " + ((int)Status.Active) +
                         " LIMIT 30";
            }
            else
            {
                strSql = "SELECT l.id_lector, l.name, l.responsible, l.birth_date, l.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number, if(a.complement is null, '', concat(', ', a.complement))) as endereco, b.name s_name, b.id_branch, a.id_address  " +
                         "FROM lector AS l " +
                         "INNER JOIN branch as b ON b.id_branch = l.id_branch " +
                         "INNER JOIN address as a ON a.id_address = l.id_address " +
                         "WHERE l.name like '%" + query + "%'" + " and l.status = "+ ((int)Status.Active) +
                         " LIMIT 30";
            }

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
                return null;
                throw;
            }
            finally
            {
                await Disconnect();
            }

            
        }
    }
}
