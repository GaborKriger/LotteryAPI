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
            StatisticsCalculate();
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
        private void StatisticsCalculate()
        {
            var results = _context.HungarianFiveLotteryResults.ToList();

            Task AvargeNumberStatisticsTask = Task.Factory.StartNew(() => AvargeNumberStatistics(results));
            Task AvargeNumberStatisticsYearlyTask = Task.Factory.StartNew(() => AvargeNumberStatisticsYearly(results));

            Task.WaitAll(AvargeNumberStatisticsTask, AvargeNumberStatisticsYearlyTask);
            _context.SaveChanges();
        }

        private void AvargeNumberStatistics(List<HungarianFiveLotteryResults> results)
        {
            int sum = 0;
            int ctn = 0;
            foreach (var result in results)
            {
                sum = sum + 
                    result.FirstNumber + result.SecondNumber + result.ThirdNumber + result.FourthNumber + result.FifthNumber;
                ctn += 5;
            }

            if (_context.HungarianFiveLotteryAvarge.Any())
            {
                var avg = _context.HungarianFiveLotteryAvarge.FirstOrDefault();
                avg.Avarge = sum / ctn;
            }
            else
            {
                var avg = new HungarianFiveLotteryAvarge
                {
                    Avarge = sum / ctn
                };
                _context.HungarianFiveLotteryAvarge.Add(avg);
            }
        }

        private void AvargeNumberStatisticsYearly(List<HungarianFiveLotteryResults> results)
        {
            HashSet<int> years = new();
            foreach (var result in results)
            {
                years.Add(result.Year);
            }

            Dictionary<int, int> avargeYearly = new();
            foreach (var year in years)
            {
                int sum = 0;
                int ctn = 0;
                foreach (var result in results)
                {
                    if (result.Year == year)
                    {
                        sum = sum +
                            result.FirstNumber + result.SecondNumber + result.ThirdNumber + result.FourthNumber + result.FifthNumber;
                        ctn += 5;
                    }
                }
                avargeYearly[year] = sum / ctn;
            }

            foreach (var key in avargeYearly.Keys)
            {
                var year = new HungarianFiveLotteryAvargeYearly();
                if (_context.HungarianFiveLotteryAvargeYearly.Any())
                {
                    year = _context.HungarianFiveLotteryAvargeYearly.
                        Where(x => x.Year == key).FirstOrDefault();  
                }

                if (year == null)
                {
                    _context.HungarianFiveLotteryAvargeYearly.Add(new HungarianFiveLotteryAvargeYearly
                    {
                        Year = key,
                        Avarge = avargeYearly[key]
                    });
                }
                else
                {
                    year.Avarge = avargeYearly[key];
                }
            }
        }
    }
}
