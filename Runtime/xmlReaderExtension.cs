using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Excel;
using System;
using System.Linq;
namespace Com.A9.FileReader
{
    public static class xmlReaderExtension
    {
        public static object SmartConvert(object value, Type tp)
        {
            if (tp.IsEnum)
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    value = 0;
                }
                return Enum.Parse(tp, value.ToString(), true);
            }
            else
            {
                return Convert.ChangeType(value, tp);
            }
        }

        public static T[] CreateArrayWithExcel<T>(string internet, string table) where T : new()
        {
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = null;
            collect = xmlReader.ReadExcel(internet, table, ref columnNum, ref rowNum);

            int rows = 0;
            for (int i = 0; i < rowNum; i++)
            {
                if (string.IsNullOrEmpty(collect[i][0].ToString()) == false && (collect[i][0] is DBNull) == false)
                    rows++;
                else
                    break;
            }

            T[] items = new T[rows - 1];
            Dictionary<string, int> header = new Dictionary<string, int>();
            for (int i = 0; i < columnNum; i++)
            {
                if (string.IsNullOrEmpty(collect[0][i].ToString()) == false)
                    header.Add(collect[0][i].ToString(), i);
            }

            var cols = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            for (int i = 1; i < rows; i++)
            {
                T item = new T();

                foreach (var itm in header)
                {
                    for (int j = 0; j < cols.Length; j++)
                    {
                        if (itm.Key == cols[j].Name)
                        {
                            object box = item;
                            var val = collect[i][header[itm.Key]];
                            var tp = cols[j].FieldType;
                            if (val is DBNull)
                            {
                                if (tp == typeof(int))
                                {
                                    val = 0;
                                }
                            }
                            cols[j].SetValue(box, SmartConvert(val, tp));
                            item = (T)box;
                        }
                    }
                }
                items[i - 1] = item;
            }
            return items;
        }

        public static (string, List<string>)[] CreateColArrayWithExcel(string internet, string table)
        {
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = xmlReader.ReadExcel(xmlReader.game_data_path, table, ref columnNum, ref rowNum);

            (string, List<string>)[] items = new (string, List<string>)[columnNum];

            for (int i = 0; i < items.Length; i++)
            {
                (string, List<string>) item;
                item.Item1 = collect[0][i].ToString();

                int rows = 0;
                for (int k = 0; k < rowNum; k++)
                {
                    if (string.IsNullOrEmpty(collect[k][i].ToString()) == false && (collect[k][i] is DBNull) == false)
                        rows++;
                }

                item.Item2 = new List<string>();
                for (int j = 1; j < rows; j++)
                {
                    if (string.IsNullOrEmpty(collect[j][i].ToString()))
                    {
                        continue;
                    }
                    item.Item2.Add(collect[j][i].ToString());
                }
                items[i] = item;
            }
            return items;
        }
    }
}

