using MathNet.Numerics.LinearRegression;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Script.Serialization;
using Topshelf;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<Service>(s =>
                {
                    s.ConstructUsing(service => new Service());
                    s.WhenStarted(service => service.Start());
                    s.WhenStopped(service => service.Stop());
                });
                x.RunAsLocalSystem();
                x.SetServiceName("ForecastService");
                x.SetDisplayName("Forecast Service");
                x.SetDescription("No description");

            });
            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
            Console.ReadLine();
        }
    }
}
