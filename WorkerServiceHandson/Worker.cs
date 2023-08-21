using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Xml.Linq;

namespace WorkerServiceHandson
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                string fileLocation = _configuration.GetValue<string>("FilePath");
                string fileName = _configuration.GetValue<string>("FileName");
                CreateFile(fileLocation, fileName);
                await Task.Delay(2000);
                DeleteFile(fileLocation);


                await Task.Delay(1000, stoppingToken);
            }
        }
        private void CreateFile(string fileLocation, string fileName)
        {
            var file = fileLocation + "\\" + fileName;
            if (!File.Exists(file))
            {
                for (int i = 1; i <= 50; i++)
                {
                    var fName = fileLocation + "\\" + "file_" + i + ".txt";
                    File.Create(fName).Dispose();
                    _logger.LogInformation("File created :{fName}", fName);
                }
                var log = "File created in " + System.Environment.MachineName + " at " + file;
              
            }
        }

        private void DeleteFile(string fileLocation)
        {
            try
            {
                string[] files = Directory.GetFiles(fileLocation);
                foreach (var file in files)
                {
                    File.Delete(file);
                    _logger.LogInformation("Deleted the files {file} at {time}", file, DateTime.Now);
                }

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
            }
        }
        public override  Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service started.");
            return base.StartAsync(stoppingToken);
        }
        public override  Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service stopped.");
            return base.StopAsync(stoppingToken);
        }
    }
}