using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HanHanReptile
{
    public static class SqlHelper
    {
        //连接字符串---------------------------------------------------------------------------
        //SQL身份验证连接
        //string ConnString = "Data Source=.;Initial Catalog=DemoBookManageDB;User Id=sa;Password=123456";
        //<add connectionString="server=.;uid=home;pwd=;database=EFFristModel" name="connStr"/>
        //Windows身份验证连接
        //string ConnString = "Data Source=.;Initial Catalog=dbTest;Integrated Security = True";
        //默认情况下，ADO.NET连接池是被启用的

        //禁用连接池后每次连接时间变长
        //禁用连接池
        //string ConnString = "Data Source=.;Initial Catalog=dbTest;Integrated Security = True;Pooling=false";
        //ADO.NET连接池注意事项--------------------------------------------------------------------
        /* 凡是需要用到“池”的地方，一般都会存在两种情况
         * 1. 创建对象比较费时
         * 2. 对象使用比较频繁
         * 池的作用：提高了创建对象的效率
         */


        //ADO.NET连接池使用总结：----------------------------------------------------
        /* 1. 第一次打开连接会创建一个连接对象
         * 2. 当这个连接对象关闭时（调用Close()方法时），会将当前那个连接对象放到连接池中
         * 3. 下一个连接对象，如果连接字符串与池中现有连接对象的连接字符串完全一致，则会使用池中现有的连接对象，不会重新创建一个
         * 4. 只有对象调用Close()的时候连接对象才会放到连接池，如果一个连接对象一直在使用，则下次在创建一个连接对象时发现连接池中没有，就会再创建一个新的连接对象，在连接池中的对象，如果一段时间没有被访问或程序关闭连接对象就会销毁
         */



        //添加引用：System.Configuration
        //public static string connString = "Data Source=MT-ZWT-139\\MSSQLSERVER12;Initial Catalog=HanHanCartoon;User ID=sa;Password=111";
        public static string connString = "Data Source=.;Initial Catalog=HanHanCartoon20200328;User ID=sa;Password=123456";
        /// <summary>
        /// 执行查询语句（单一结果）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="ps">参数</param>
        /// <returns>返回单一结果</returns>
        public static object ExecuteScalar(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (ps.Length > 0)
                {
                    cmd.Parameters.AddRange(ps);
                 }
                try
                {
                     conn.Open();
                    return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        

        /// <summary>
        /// 执行增删改语句
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="ps">参数</param>
        /// <returns>返回受影响的行数</returns>
        public static int ExecuteNonQuery(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                if (ps.Length > 0)
                {
                    cmd.Parameters.AddRange(ps);
                }
                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 执行查询语句（结果集）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="ps">参数</param>
        /// <returns>返回SqlDataReader结果集</returns>
        public static SqlDataReader ExecuteReader(string sql, params SqlParameter[] ps)
        {
            return ExecuteReader(sql, CommandType.Text, ps);

        }
        /// <summary>
        /// 执行查询语句，可选择命令类型（结果集）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="ps">参数</param>
        /// <returns>返回SqlDataReader结果集</returns>
        public static SqlDataReader ExecuteReader(string sql, CommandType cmdType, params SqlParameter[] ps)
        {
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(sql, conn);
            //指定命令类型为存储过程（默认为CommandType.Text,即执行sql文本）
            cmd.CommandType = cmdType;
            if (ps.Length > 0)
            {
                cmd.Parameters.AddRange(ps);
            }
            try
            {
                conn.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 执行查询语句（DataTable）
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="ps">参数</param>
        /// <returns>返回DataTable结果集</returns>
        public static DataTable ExecuteTable(string sql, params SqlParameter[] ps)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);
                if (ps.Length > 0)
                {
                    adapter.SelectCommand.Parameters.AddRange(ps);
                }
                DataTable dt = new DataTable();
                try
                {
                    adapter.Fill(dt);
                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }

            }
        }


        public static object ExecuteProcedure(string procName, params SqlParameter[] ps)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand(procName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (ps.Length > 0)
                {
                    cmd.Parameters.AddRange(ps);
                }
                try
                {
                    conn.Open();
                     return cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        /// <summary>
        /// 获取数据库的时间
        /// </summary>
        /// <returns>返回数据库的时间</returns>
        public static DateTime GetServerTime()
        {
            string sql = "Select GETDATE()";
            return Convert.ToDateTime(ExecuteScalar(sql));
        }
    }
}
