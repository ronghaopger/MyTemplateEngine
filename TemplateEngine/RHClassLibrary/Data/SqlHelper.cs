using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace RHClassLibrary.Data
{
    public class SqlHelper
    {
        //连接字符串
        private String connectString;
        public SqlHelper()
        {
            //如果数据库配置文件存在，加载配置
            if (File.Exists(PathHelper.GetMapPath("/Config/db.config")))
            {
                this.connectString = @"Server=(local);Database=WL_CMS;User=sa;Password=ronghao;Pooling=True;Min Pool Size=3;Max Pool Size=10;Connect Timeout=30;";
            }
        }
        /// <summary>/// 
        /// 设置SqlCommand的参数        
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="obj"></param>
        private void SetParameters<T>(SqlCommand cmd, T t)
        {
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties();  
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.Name == "Item")
                    continue;
                object value = propertyInfo.GetValue(t, null);
                bool isNull = false;
                if (value == null)
                {
                    isNull = true;
                }
                SqlDbType st = new SqlDbType();
                switch (propertyInfo.PropertyType.ToString())
                {
                    case "System.SmallInt":
                        st = SqlDbType.SmallInt;
                        break;
                    case "System.Int32":
                        st = SqlDbType.Int;
                        break;
                    case "System.String":
                        st = SqlDbType.VarChar;
                        if(isNull)
                          value = String.Empty;
                        break;
                    case "System.DateTime":
                    case "System.Nullable`1[System.DateTime]":
                        st = SqlDbType.DateTime;
                        if (isNull)
                            value = DBNull.Value;
                        break;
                    case "System.Bool":
                        st = SqlDbType.Bit;
                        break;
                    case "System.Guid":
                        st = SqlDbType.UniqueIdentifier;
                        break;
                }
                cmd.Parameters.Add(new SqlParameter("@" + propertyInfo.Name, st));
                cmd.Parameters["@" + propertyInfo.Name].Value = value;
            }
        }
        /// <summary>
        /// 执行SQL语句，并返回受影响的行数
        /// </summary>
        /// <returns>受影响的行数</returns>
        public int ExcuteSql(String SqlStr)
        {
            using (SqlConnection connection = new SqlConnection(connectString))
            {
                using (SqlCommand cmd = new SqlCommand(SqlStr, connection))
                {
                    try
                    {
                        connection.Open();
                        return cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// 执行SQL语句，并返回受影响的行数
        /// </summary>
        /// <returns>受影响的行数</returns>
        public int ExcuteSql<T>(String SqlStr,T t)
        {
            using (SqlConnection connection = new SqlConnection(connectString))
            {
                using (SqlCommand cmd = new SqlCommand(SqlStr, connection))
                {
                    SetParameters<T>(cmd, t);
                    try
                    {
                        connection.Open();
                        return cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// 读取并返回数据集合
        /// </summary>
        /// <returns></returns>
        public SqlDataReader ExcuteReader(String SqlStr)
        {
            using (SqlConnection connection = new SqlConnection(connectString))
            {
                using (SqlCommand cmd = new SqlCommand(SqlStr, connection))
                {
                    try
                    {
                        connection.Open();
                        return cmd.ExecuteReader();
                    }
                    catch (SqlException ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// 返回DataRow
        /// </summary>
        /// <returns></returns>
        public DataRow ExcuteDataRow(String SqlStr)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectString))
            {
                using (SqlDataAdapter cmd = new SqlDataAdapter(SqlStr, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.Fill(dt);
                        return dt.Rows[0];
                    }
                    catch (SqlException ex)
                    {
                        throw ex;
                    }
                }
            }

            //DataRow dr;
            //DataTable dt = new DataTable();
            //SqlDataReader reader = ExcuteReader();
            //try
            //{
            //    if (reader.Read())
            //    {
            //        dr = dt.NewRow();
            //        for (int i = 0; i < reader.FieldCount; i++)
            //        {
            //            Type datatype = reader[i].GetType();
            //            if(datatype == typeof(DBNull))
            //                datatype = typeof(String);
            //            dt.Columns.Add(reader.GetName(i), datatype);
            //            dr[i] = reader[i];
            //        }
            //        dt.Rows.Add(dr);
            //    }
            //    return dt.Rows[0];
            //}
            //catch (SqlException ex)
            //{
            //    LogClass.WriteErrorLog("SqlHelper", "ExcuteDataRow", "sql执行错误", ex.Message);
            //    return null;
            //}
        }
        /// <summary>
        /// 返回DataTable
        /// </summary>
        /// <returns></returns>
        public DataTable ExcuteDataTable(String SqlStr)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectString))
            {
                using (SqlDataAdapter cmd = new SqlDataAdapter(SqlStr, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.Fill(dt);
                        return dt;
                    }
                    catch (SqlException ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// 返回DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet ExcuteDataSet(String SqlStr)
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectString))
            {
                using (SqlDataAdapter cmd = new SqlDataAdapter(SqlStr, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.Fill(ds);
                        return ds;
                    }
                    catch (SqlException ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public List<T> List<T>(String SqlStr) where T : CommonIndexer, new()
        {
            List<T> data = new List<T>();
            DataTable dt = ExcuteDataTable(SqlStr);
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                for (int i = 0; i < dr.ItemArray.Length; i++)
                {
                    t[dr.Table.Columns[i].ColumnName] = dr[i].ToString();
                }
                data.Add(t);
            }
            return data;
        }
    }
}
