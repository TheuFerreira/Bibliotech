using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bibliotech.Model.Entities;
using MySqlConnector;
using System.Data;

namespace Bibliotech.Model.DAO
{
    public class DAOBook : Connection
    {
        private readonly Author author;
        public async void InsertBook(Book book, Author author)
        {
            await Connect ();
            MySqlTransaction transaction = await SqlConnection.BeginTransactionAsync();

            try
            {
                MySqlCommand cmd = new MySqlCommand(SqlConnection, transaction);

                string insertAuthor = "insert into author(name, status) values (?, 1); " +
                    "select @@identity; ";
                cmd.Parameters.Clear();
                cmd.CommandText = insertAuthor;
                cmd.Parameters.Add("?", DbType.String).Value = author.Name;

                object id = await cmd.ExecuteScalarAsync();
                author.IdAuthor = Convert.ToInt32(id.ToString());

                string insertBook = "insert into book(id_author, title, subtitle, publishing_company, gender, " +
                    "edition, pages, year_publication, language, volume, collection, status ) values  (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 1) ;";
                
                cmd.Parameters.Clear();
                cmd.CommandText = insertBook;
                cmd.Parameters.Add("?", DbType.Int32).Value = author.IdAuthor;
                cmd.Parameters.Add("?", DbType.String).Value = book.Title;
                cmd.Parameters.Add("?", DbType.String).Value = book.Subtitle;
                cmd.Parameters.Add("?", DbType.String).Value = book.PublishingCompany;
                cmd.Parameters.Add("?", DbType.String).Value = book.Gender;
                cmd.Parameters.Add("?", DbType.String).Value = book.Edition;
                cmd.Parameters.Add("?", DbType.Int32).Value = book.Pages;
                cmd.Parameters.Add("?", DbType.Int32).Value = book.YearPublication;
                cmd.Parameters.Add("?", DbType.String).Value = book.Language;
                cmd.Parameters.Add("?", DbType.String).Value = book.Volume;
                cmd.Parameters.Add("?", DbType.String).Value = book.Collection;

                await cmd.ExecuteNonQueryAsync();
                await transaction.CommitAsync();

            }
            catch (MySqlException ex)
            {
                await transaction .RollbackAsync();
                throw ex;
            }
            finally
            {
                await Disconnect();
            }
        }
    }
}
