using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class QuerrySql
    {
        public static DataTable getHistoryByTime(int factoryID,DateTime start,DateTime end)
        {
            string sql = @"SELECT * FROM dbo.History WHERE FactoryID=@id AND Time BETWEEN @start AND @end";
            SqlParameter param = new SqlParameter("@id", SqlDbType.Int);
            param.Value = factoryID;
            SqlParameter param1 = new SqlParameter("@start", SqlDbType.DateTime);
            param1.Value = start;
            SqlParameter param2 = new SqlParameter("@end", SqlDbType.DateTime);
            param2.Value = end;
            return DbContext.GetDataBySQLWithParameters(sql, param,param1,param2);
        }

        public static DataTable getForeCast(int factoryID)
        {
            string sql = @"SELECT * FROM dbo.Forecast WHERE FactoryID=@id";
            SqlParameter param = new SqlParameter("@id", SqlDbType.Int);
            param.Value = factoryID;
            return DbContext.GetDataBySQLWithParameters(sql, param);
        }

        public static DataTable getMaxTimeHistory(int factoryID)
        {
            string sql = @"SELECT MAX(Time) AS Time FROM dbo.History WHERE FactoryID=@id";
            SqlParameter param = new SqlParameter("@id", SqlDbType.Int);
            param.Value = factoryID;
            return DbContext.GetDataBySQLWithParameters(sql, param);
        }

        public static DataTable getMaxTimeForecast(int factoryID)
        {
            string sql = @"SELECT MAX(Time) AS Time FROM dbo.Forecast WHERE FactoryID=@id";
            SqlParameter param = new SqlParameter("@id", SqlDbType.Int);
            param.Value = factoryID;
            return DbContext.GetDataBySQLWithParameters(sql, param);
        }

        public static int insertHistory(DateTime time, double capacity, double ghi, double evtTemp,int id)
        {
            string sql = @"INSERT INTO dbo.History
        ( FactoryID, Time, Capacity, Ghi, EnviromentTemp )
VALUES  ( @id, --FactoryID - int
          @time, -- Time - datetime
          @capacity, -- Capacity - float
          @ghi, -- Ghi - float
          @env  -- EnviromentTemp - float
          )";
            SqlParameter param = new SqlParameter("@time", SqlDbType.DateTime);
            param.Value = time;
            SqlParameter param1 = new SqlParameter("@capacity", SqlDbType.Float);
            param1.Value = capacity;
            SqlParameter param2 = new SqlParameter("@ghi", SqlDbType.Float);
            param2.Value = ghi;
            SqlParameter param3 = new SqlParameter("@env", SqlDbType.Float);
            param3.Value = evtTemp;
            SqlParameter param4 = new SqlParameter("@id", SqlDbType.Int);
            param4.Value = id;
            return DbContext.ExecuteSQLWithParameters(sql, param,param1,param2,param3,param4);
        }

        public static int insertForeCast(DateTime time, double capacity, double ghi, double evtTemp, int id)
        {
            string sql = @"INSERT INTO dbo.Forecast
        ( FactoryID ,
          Time ,
          Capacity ,
          Ghi ,
          EnviromentTemp
        )
VALUES  ( @id , -- FactoryID - int
          @time , -- Time - datetime
          @capacity , -- Capacity - float
          @ghi , -- Ghi - float
          @env  -- EnviromentTemp - float
        )";
            SqlParameter param = new SqlParameter("@time", SqlDbType.DateTime);
            param.Value = time;
            SqlParameter param1 = new SqlParameter("@capacity", SqlDbType.Float);
            param1.Value = capacity;
            SqlParameter param2 = new SqlParameter("@ghi", SqlDbType.Float);
            param2.Value = ghi;
            SqlParameter param3 = new SqlParameter("@env", SqlDbType.Float);
            param3.Value = evtTemp;
            SqlParameter param4 = new SqlParameter("@id", SqlDbType.Int);
            param4.Value = id;
            return DbContext.ExecuteSQLWithParameters(sql, param, param1, param2, param3, param4);
        }
    }
}
