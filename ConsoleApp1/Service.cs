using MathNet.Numerics.LinearRegression;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Web.Script.Serialization;

namespace ConsoleApp1
{
    class Service
    {
        private Timer timer;
        public Service()
        {
            timer = new Timer(30000) { AutoReset = true };
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (QuerrySql.getMaxTimeHistory(362).Rows[0]["Time"].ToString().Equals(""))
            {
                insertHistory(DateTime.Now.AddDays(-1), DateTime.Now, 362); //insert Data from previous day until now
            }
            else 
            {
                insertHistory(Convert.ToDateTime(QuerrySql.getMaxTimeHistory(362).Rows[0]["Time"]).AddSeconds(1), DateTime.Now, 362); //insert data from last time inserted until now
            }
            computeWeight(362); //362 is idNhaMay
        }

        public void Start()
        {
            while (true)
            {
                DateTime temp = DateTime.Now;
                if (Math.Floor((double)temp.Minute / 5.0) * 5 == temp.Minute) //timer start each 5 minutes: Ex: 5h00, 5h05, 5h10 ....
                {
                    timer.Start();
                    break;
                }
            }
        }
        public void Stop()
        {
            timer.Stop();
        }

        static void insertHistory(DateTime start, DateTime end, int id)
        {
            WebClient web = new WebClient();
            string jsonStr = web.DownloadString("https://www.nldc.evn.vn/Renewable/Scada/GetScadaNhaMay?start=" + start.ToString("yyyyMMddHHmmss") + "&end=" + end.ToString("yyyyMMddHHmmss") + "&idNhaMay=" + id);//get JSON from Request
            JavaScriptSerializer java = new JavaScriptSerializer();
            Success list = (Success)java.Deserialize(jsonStr, typeof(Success)); //convert JSON to List of Model
            if (list.success.Equals("True"))
            {
                foreach (DataModel item in list.data)
                {
                    QuerrySql.insertHistory(Convert.ToDateTime(item.time.Replace('-', '/').Replace('T', ' ')), item.capacity, item.ghi, item.envtemp, id); //Convert 2020-05-19T07:53:00 to 2020/05/19 07:53:00
                }
            }
            else
            {
                Console.WriteLine("Can't get factory's information");
            }
        }

        static void insertForeCast(DateTime start, DateTime end, int id, double m1, double m2, double b)
        {
            WebClient web = new WebClient();
            JavaScriptSerializer java = new JavaScriptSerializer();
            string jsonStr = web.DownloadString("https://www.nldc.evn.vn/Renewable/Forecast/GetThoiTietNhaMay?start=" + start.ToString("yyyyMMddHHmmss") + "&end=" + end.ToString("yyyyMMddHHmmss") + "&idNhaMay=" + id);
            Success listForeCast = (Success)java.Deserialize(jsonStr, typeof(Success));
            List<double> ghi = new List<double>();
            List<double> envTemp = new List<double>();
            List<DateTime> time = new List<DateTime>();
            if (listForeCast.success.Equals("True"))
            {
                foreach (DataModel item in listForeCast.data)
                {
                    ghi.Add(item.ghi);
                    envTemp.Add(item.envtemp);
                    time.Add(Convert.ToDateTime(item.date.Replace('-', '/').Replace('T', ' ')));

                    QuerrySql.insertForeCast(Convert.ToDateTime(item.date.Replace('-', '/').Replace('T', ' ')), item.ghi * m1 + item.envtemp * m2 + b, item.ghi, item.envtemp, id); //Convert 2020-05-19T07:53:00 to 2020/05/19 07:53:00
                }

            }
            else
            {
                Console.WriteLine("Khong the lay thong tin nha may");
            }
        }

        static double[] computeWeight(int id)
        {
            List<double> capacity = new List<double>();
            List<double> ghi = new List<double>();
            List<double> enviromentTemp = new List<double>();
            DateTime timeToGet;
            timeToGet = Convert.ToDateTime(QuerrySql.getMaxTimeHistory(id).Rows[0]["Time"]);
            foreach (DataRow row in QuerrySql.getHistoryByTime(id, timeToGet.AddDays(-1), timeToGet).Rows) //get data from previous 24 hours
            {
                capacity.Add(Convert.ToDouble(row["Capacity"]));
                ghi.Add(Convert.ToDouble(row["Ghi"]));
                enviromentTemp.Add(Convert.ToDouble(row["EnviromentTemp"]));
            }
            double[] result = new double[3];
            result = multi_linear_regression(ghi.ToArray(), enviromentTemp.ToArray(), capacity.ToArray()); //compute weight 
            Console.WriteLine("Cost is: " + cost_function(ghi.ToArray(), enviromentTemp.ToArray(), capacity.ToArray(), result[0], result[1], result[2])); //compute cost
            if (QuerrySql.getForeCast(id).Rows.Count == 0) 
            {
                insertForeCast(timeToGet.AddSeconds(1), timeToGet.AddHours(6), id, result[0], result[1], result[2]);//Compute forecast from Now to 6 hous later
            }
            else
            {
                DateTime temp = Convert.ToDateTime(QuerrySql.getMaxTimeForecast(id).Rows[0]["Time"]);
                insertForeCast(temp.AddSeconds(1), DateTime.Now.AddHours(6), id, result[0], result[1], result[2]);//Compute forecast 5 minutes later from the last forecast
            }
            return result;
        }

        static double[] multi_linear_regression(double[] x1, double[] x2, double[] y)
        {
            double[][] matrix = new double[x1.Length][];
            for (int i = 0; i < x1.Length; i++)
            {
                matrix[i] = new[] { x1[i], x2[i] };
            }
            double[] p = new double[3];
            for (int i = 0; i < 1; i++)
            {
                p = MultipleRegression.QR(matrix, y, intercept: true);
            }
            return p;
        }

        static double cost_function(double[] x1, double[] x2, double[] y, double m1, double m2, double b)
        {
            int count = x1.Length;
            double sum_error = 0;
            double max90 = y[0]; //congsuat_thietke
            foreach (double item in y)
            {
                if (item > max90)
                {
                    max90 = item;
                }
            }
            max90 = max90 * 0.9;
            for (int i = 0; i < count; i++)
            {
                sum_error += y[i] - (m1 * x1[i] + m2 * x2[i] + b) / (max90); //(thucTe - duDoan) / congSuat_thietKe
            }
            return sum_error / count;
        }
    }
}
