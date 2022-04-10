using LotteryAPI.Model;
using System.Net;

namespace LotteryAPI.Calculating
{
    public class CalculateHungarianFivelotteryStatistics
    {
        private readonly WebClient _client;
        private readonly string _filePath;
        private readonly DataContext _context;

        public CalculateHungarianFivelotteryStatistics(DataContext context)
        {
            _client = new WebClient();
            _filePath = "HungarianFiveLotteryResults.csv";
            _context = context;
        }
        public void Calculate()
        {
            DownloadFile();
            GetResultsList();
        }


        private void DownloadFile()
        {
            try
            {
                _client.DownloadFile(
                    "https://bet.szerencsejatek.hu/cmsfiles/otos.csv",
                    _filePath
                    );
            }
            catch (WebException ex)
            {
                throw new WebException(_filePath, ex);
            }
        }

        private void GetResultsList()
        {
            List<HungarianFiveLotteryResults> hflrs = new();
            StreamReader _reader = new(_filePath);

            while (!_reader.EndOfStream)
            {
                var values = _reader.ReadLine().Split(";");
                HungarianFiveLotteryResults hflr = new();

                hflr.Year = int.Parse(values[0]);
                hflr.Week = int.Parse(values[1]);

                hflr.FirstNumber = int.Parse(values[11]);
                hflr.SecondNumber = int.Parse(values[12]);
                hflr.ThirdNumber = int.Parse(values[13]);
                hflr.FourthNumber = int.Parse(values[14]);
                hflr.FifthNumber = int.Parse(values[15]);

                hflrs.Add(hflr);
            }
            _reader.Close();

            var lastID = new HungarianFiveLotteryResults();
            if (_context.HungarianFiveLotteryResults.Any())
            {
                lastID = _context.HungarianFiveLotteryResults.
                    OrderByDescending(x => x.ID).First();
            }

            for (int i = (hflrs.Count -1); i >= 0; i--)
            {
                if ((lastID.Year <= hflrs[i].Year && lastID.Week < hflrs[i].Week) 
                    || lastID.Year < hflrs[i].Year)
                {
                    _context.Add(hflrs[i]);
                    _context.SaveChanges();
                }
            }
        }
    }
}
