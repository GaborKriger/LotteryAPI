using LotteryAPI.Model;

namespace LotteryAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<HungarianFiveLotteryResults> HungarianFiveLotteryResults { get; set; }
        public DbSet<HungarianFiveLotteryAvarge> HungarianFiveLotteryAvarge { get; set; }
        public DbSet<HungarianFiveLotteryAvargeYearly> HungarianFiveLotteryAvargeYearly { get; set; }

    }
}
