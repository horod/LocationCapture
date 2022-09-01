using Microsoft.EntityFrameworkCore;

namespace LocationCapture.DAL.Sqlite2
{
    public class SqliteLocationDbContext : LocationDbContext
    {
        public SqliteLocationDbContext() : base() { }

        public SqliteLocationDbContext(string dbIdentifier) : base(dbIdentifier) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(DbIdentifier ?? "Data Source=locationCapture.db");
        }
    }
}
