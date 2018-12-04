using FluentScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EmailingSystem.Cron
{
    class Program
    {
        private static CronSettings _cronSettings;
        private static IConfigurationRoot _configuration;
        static void Main(string[] args)
        {
            if (!StartUpService())
            {
                return;
            }

            JobManager.AddJob(() => PostApi(_cronSettings.InitiateSetStatusProcessURL).GetAwaiter(), (s) => s
            .ToRunEvery(_cronSettings.InitiateSendEmailProcessRepeatNoOfDays)
            .Days()
            .At(_cronSettings.InitiateSendEmailProcessRepeatHour, _cronSettings.InitiateSendEmailProcessRepeatMin));

            JobManager.AddJob(() => PostApi(_cronSettings.InitiateSendEmailProcessURL).GetAwaiter(), (s) => s
            .ToRunEvery(_cronSettings.InitiateSetStatusProcessRepeatNoOfDays)
            .Days()
            .At(_cronSettings.InitiateSetStatusProcessRepeatHour, _cronSettings.InitiateSetStatusProcessRepeatMin));
            

            Console.ReadLine();
        }

        public static async Task PostApi(string uri)
        {
            using (var client = new HttpClient())
            {
                await client.PostAsync(uri, new StringContent("", Encoding.UTF8, "application/json"));
            }

        }

        private static bool StartUpService()
        {
            try
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                var builder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                    .AddEnvironmentVariables();

                _configuration = builder.Build();

                _cronSettings = new CronSettings();
                _configuration.GetSection("CronSettings").Bind(_cronSettings);


                var serviceProvider = new ServiceCollection()
                .Configure<CronSettings>(_configuration.GetSection("CronSettings"))
                .AddOptions()
                .BuildServiceProvider();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


    }
}
