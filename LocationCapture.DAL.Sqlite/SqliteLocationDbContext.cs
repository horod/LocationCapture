using Microsoft.EntityFrameworkCore;

namespace LocationCapture.DAL.Sqlite
{
    public class SqliteLocationDbContext : LocationDbContext
    {
        public SqliteLocationDbContext(string dbIdentifier) : base(dbIdentifier) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(DbIdentifier);
        }
    }
}
