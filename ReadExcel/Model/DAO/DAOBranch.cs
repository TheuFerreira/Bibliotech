using Bibliotech.Model.DAO;
using MySqlConnector;
using ReadExcel.Model.Entities;
using System.Collections.Generic;
using System.Data;

namespace ReadExcel.Model.DAO
{
    public class DAOBranch : Connection
    {
        public List<Branch> GetAll()
        {
            try
            {
                Connect().Wait();

                string sql = "" +
                    "SELECT id_branch, name FROM branch WHERE status = 1";

                MySqlCommand command = new MySqlCommand(sql, SqlConnection);

                List<Branch> branches = new List<Branch>();
                MySqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    int idBranch = reader.GetFieldValue<int>(0);
                    string name = reader.GetFieldValue<string>(1);

                    Branch branch = new Branch
                    {
                        IdBranch = idBranch,
                        Name = name,
                    };

                    branches.Add(branch);
                }

                return branches;
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                Disconnect().Wait();
            }
        }
    }
}
