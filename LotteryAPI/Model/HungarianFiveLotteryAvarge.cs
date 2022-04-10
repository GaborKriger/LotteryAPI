using System.ComponentModel.DataAnnotations.Schema;

namespace LotteryAPI.Model
{
    [Table("Avarge", Schema = "HungarianFiveLottery")]
    public class HungarianFiveLotteryAvarge
    {
        public int ID { get; set; }
        public int Avarge { get; set; }
    }
}
