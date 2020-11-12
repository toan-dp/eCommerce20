using SqlClientExtention.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace SqlClientExtention.Extentions
{
    /*
        Lưu ý khi sử dụng :
            - gán [OutputParameter] attributr cho tham số dạng output 
              ( phải khác null hoặc không mang kiểu nullable type )
  
            - ToList<ResultModel> nên dùng domain model được generate bởi Entity Framework để tránh không 
              tương thích kiểu dữ liệu , tên biến với database .... 
    */
    public static class SqlClientExtention
    {
        public static SqlParameterCollection AddParams<ParamModel>(this SqlParameterCollection paramsCollection
            , ParamModel model, Type modelType)
            where ParamModel : class
        {
            IDictionary<string, object> parameters = model.ToDictionary();

            foreach (var item in parameters)
            {
                if (Attribute.IsDefined(modelType.GetProperty(item.Key), typeof(ExcludeParameterAttribute)))
                    continue;

                    var sqlParameter = new SqlParameter($"@{item.Key.ToLower()}", item.Value);
                if (Attribute.IsDefined(modelType.GetProperty(item.Key), typeof(OutputParameterAttribute)))
                {
                    sqlParameter.Direction = ParameterDirection.Output;
                }
                else if (item.Value == null)
                {
                    // không truyền null vào sp vì sẽ bị mất default value ở sp
                    continue;
                }

                paramsCollection.Add(sqlParameter);
            }

            return paramsCollection;
        }

        public static SqlParameterCollection AddParams(this SqlParameterCollection paramsCollection, object model)
        {
            IDictionary<string, object> parameters = model.ToDictionary();
            foreach (var item in parameters)
            {
                if (item.Value == null)
                {
                    continue;
                }

                var sqlParameter = new SqlParameter($"@{item.Key.ToLower()}", item.Value);
                paramsCollection.Add(sqlParameter);
            }

            return paramsCollection;
        }

        public static List<T> ToList<T>(this SqlDataReader reader)
            where T : class, new()
        {
            var result = new List<T>();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    var ModelInstance = new T();
                    for (int i = reader.FieldCount - 1; i >= 0; i--)
                    {
                        var dbValue = reader.GetValue(i);
                        var value = (!DBNull.Value.Equals(dbValue)) ? dbValue : null;
                        if(value != null)
                        {
                            var name = reader.GetName(i);
                            try
                            {
                                ModelInstance.SetPropertyValue(name, value);
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    result.Add(ModelInstance);
                }
            }

            return result;
        }
    }
}
