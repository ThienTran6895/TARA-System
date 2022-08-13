using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL.Core
{
    public abstract class BaseRepository
    {
        protected IDbConnection _db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    }
}
