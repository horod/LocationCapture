using Microsoft.EntityFrameworkCore;

namespace LocationCapture.DAL.SqlServer
{
    public class SqlServerLocationDbContext : LocationDbContext
    {
        private readonly string _connectionString;

        public SqlServerLocationDbContext(string connectionString) : base(connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
