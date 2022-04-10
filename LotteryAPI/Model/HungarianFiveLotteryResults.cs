using System.ComponentModel.DataAnnotations.Schema;

namespace LotteryAPI.Model
{
    [Table("Results", Schema="HungarianFiveLottery")]
    public class HungarianFiveLotteryResults
    {
        public int ID { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
        public int FirstNumber { get; set; }
        public int SecondNumber { get; set; }
        public int ThirdNumber { get; set; }
        public int FourthNumber { get; set; }
        public int FifthNumber { get; set; }
    }
}
