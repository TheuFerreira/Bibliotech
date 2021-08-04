using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bibliotech.Model.DAO
{
    public class DAOLector: Connection
    {
        public async Task<DataTable> FillDataGrid(string query, int branch)
        {
            await Connect();
            string strSql;

            if (branch != 0)
            {
                strSql = "SELECT l.id_lector, l.name, l.responsible, l.birth_date, l.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number) as endereco, b.name, b.id_branch, a.id_address " +
                         "FROM lector AS l " +
                         "INNER JOIN branch as b ON b.id_branch = l.id_branch " +
                         "INNER JOIN address as a ON a.id_address = l.id_address " +
                         "WHERE l.name like '%" + query + "%' and b.id_branch = " + branch +
                         " LIMIT 30";
            }
            else
            {
                strSql = "SELECT l.id_lector, l.name, l.responsible, l.birth_date, l.telephone, concat(a.city, ', ', a.neighborhood, ', ', a.street, ', ', a.number as endereco, b.name, b.id_branch, a.id_address " +
                         "FROM lector AS l " +
                         "INNER JOIN branch as b ON b.id_branch = l.id_branch " +
                         "INNER JOIN address as a ON a.id_address = l.id_address " +
                         "WHERE l.name like '%" + query + "%'" +
                         "LIMIT 30";
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
