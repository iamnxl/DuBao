using MathNet.Numerics.LinearRegression;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            WebClient web = new WebClient();
            string jsonStr = web.DownloadString("https://www.nldc.evn.vn/Renewable/Scada/GetScadaNhaMay?start=20200510000000&end=20200511000000&idNhaMay=362");//get JSON from Request
            JavaScriptSerializer java = new JavaScriptSerializer();
            SuccessHistory list = (SuccessHistory)java.Deserialize(jsonStr, typeof(SuccessHistory)); //convert JSON to List of Model
            List<double> ghi = new List<double>();
            List<double> envTemp = new List<double>();
            List<double> capacity = new List<double>();
            if (list.success.Equals("True"))
            {
                foreach (DataHistory item in list.data)
                {
                    ghi.Add(item.ghi);
                    envTemp.Add(item.envtemp);
                    capacity.Add(item.capacity);
                }
            }
            else
            {
                Console.WriteLine("Khong the lay thong tin nha may");
            }
            var x1Values = ghi.ToArray();
            var x2Values = envTemp.ToArray();
            var yValues = capacity.ToArray();
            double[] result=new double[3];
            for(int i = 0; i < 1; i++)
            {

                result = multi_linear_regression(x1Values, x2Values, yValues); //compute Weights of X1, X2 and store it in "result" 
            }
            Console.WriteLine("Cost is: "+ cost_function(x1Values, x2Values, yValues, result[0], result[1], result[2])); //compute cost
            #region MyRegion
            //jsonStr = web.DownloadString("https://www.nldc.evn.vn/Renewable/Forecast/GetThoiTietNhaMay?start=20200512000000&end=20200514000000&idNhaMay=326");
            //SuccessForeCast listForeCast = (SuccessForeCast)java.Deserialize(jsonStr, typeof(SuccessForeCast));
            //if (listForeCast.success.Equals("True"))
            //{
            //    foreach (DataForeCast item in listForeCast.data)
            //    {
            //        Console.WriteLine(item.date + " - " + item.air_temperature);
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Khong the lay thong tin nha may");
            //} 
            #endregion
            Console.ReadLine();
        }

        static double[] multi_linear_regression(double[] x1, double[] x2, double[] y)
        {
            double[][] matrix =new double[x1.Length][];
            for(int i = 0; i < x1.Length; i++)
            {
                matrix[i] = new[] { x1[i], x2[i] };
            }
            double[] p = MultipleRegression.QR(matrix,y,intercept: true);
            return p;
        }

        static double cost_function(double[] x1, double[] x2, double[] y, double m1, double m2, double b)
        {
            int count = x1.Length;
            double sum_error = 0;
            double max90 = y[0]; //congsuat_thietke
            foreach(double item in y){
                if (item > max90)
                {
                    max90 = item;
                }
            }
            max90 = max90 * 0.9;
            for (int i = 0; i < count; i++)
            {
                sum_error += y[i] - (m1 * x1[i] + m2 * x2[i] + b)/(max90); //(thucTe - duDoan) / congSuat_thietKe
            }
            return sum_error / count;
        }
    }
}
