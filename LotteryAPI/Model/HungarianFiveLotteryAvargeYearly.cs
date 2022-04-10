using System.ComponentModel.DataAnnotations.Schema;

namespace LotteryAPI.Model
{
    [Table("AvargeYearly", Schema = "HungarianFiveLottery")]
    public class HungarianFiveLotteryAvargeYearly
    {
        public int ID { get; set; }
        public int Year { get; set; }
        public int Avarge { get; set; }
    }
}
