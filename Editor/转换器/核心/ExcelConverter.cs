﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JFramework
{
    internal static partial class ExcelConverter
    {
        private static bool isCompleted;
        
        /// <summary>
        /// 获取Excel数据
        /// </summary>
        /// <param name="table">Excel数据表</param>
        /// <returns>返回Excel数据</returns>
        private static ExcelData GetTableData(ExcelTable table)
        {
            var excelData = new ExcelData();
            for (var i = 0; i < table.rowCount; i++)
            {
                var rowData = new List<string>();
                for (var j = 0; j < table.columnCount; j++)
                {
                    var item = table.GetData(i, j);
                    rowData.Add(item != null ? item.value : "");
                }

                excelData.DataList.Add(rowData);
            }

            excelData.Row = table.rowCount;
            excelData.Column = table.columnCount;
            return excelData;
        }

        /// <summary>
        /// 移除空的列
        /// </summary>
        /// <param name="table">Excel数据表</param>
        /// <returns>返回移除空列的数据</returns>
        private static ExcelData RemoveEmptyColumn(ExcelTable table)
        {
            var nameList = new List<int>();
            for (var i = 0; i < table.columnCount; i++)
            {
                var name = table.GetValue(EditorConst.Name, i);
                if (!string.IsNullOrEmpty(name)) nameList.Add(i);
            }

            var typeList = new List<int>();
            foreach (var name in nameList)
            {
                var type = table.GetValue(EditorConst.Type, name);
                if (FieldParser.IsSupportedType(type)) typeList.Add(name);
            }

            var excelData = new ExcelData();
            for (var i = 0; i < table.rowCount; i++)
            {
                var rowList = new List<string>();
                foreach (var column in typeList)
                {
                    var item = table.GetData(i, column);
                    rowList.Add(item != null ? item.value : "");
                }

                excelData.DataList.Add(rowList);
            }

            excelData.Row = table.rowCount;
            excelData.Column = typeList.Count;
            return excelData;
        }

        /// <summary>
        /// 判断能否转化数据
        /// </summary>
        /// <param name="table">Excel数据表</param>
        /// <returns>返回是否能转换</returns>
        private static bool IsConvertTable(ExcelTable table)
        {
            if (table == null || table.columnCount < 1) return false;
            if (table.rowCount <= EditorConst.Type) return false;
            var columnCount = 0;
            for (var i = 0; i < table.columnCount; i++)
            {
                var type = table.GetValue(EditorConst.Type, i);
                if (string.IsNullOrEmpty(type) || type.Equals(" ") || type.Equals("\r")) continue;
                if (FieldParser.IsSupportedType(type))
                {
                    if (!string.IsNullOrEmpty(table.GetValue(EditorConst.Name, i)))
                    {
                        columnCount++;
                    }
                }
            }

            return columnCount > 0;
        }

        private static bool IsSupportedExcel(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            var fileName = Path.GetFileName(filePath);
            if (fileName.Contains("~$")) return false;
            var lower = Path.GetExtension(filePath).ToLower();
            return lower is ".xlsx" or ".xls" or ".xlsm";
        }

        /// <summary>
        /// 更新进度条
        /// </summary>
        /// <param name="curProgress">当前进度</param>
        /// <param name="maxProgress">最大进度</param>
        private static void UpdateProgress(int curProgress, int maxProgress)
        {
            var title = "Excel数据导入进度 [" + curProgress + " / " + maxProgress + "]";
            var value = curProgress / (float)maxProgress;
            EditorUtility.DisplayProgressBar(title, "", value);
            isCompleted = true;
        }

        /// <summary>
        /// 清除进度条条
        /// </summary>
        private static void RemoveProgress()
        {
            if (isCompleted)
            {
                try
                {
                    EditorUtility.ClearProgressBar();
                }
                catch (Exception e)
                {
                    Debug.LogError(e.ToString());
                }

                isCompleted = false;
            }
        }
        
        public static string GetTableName(string sheetName)
        {
            return sheetName + "Table";
        }

        public static string GetScriptName(string sheetName)
        {
            return sheetName + "Table.cs";
        }

        public static string GetObjectName(string sheetName)
        {
            return sheetName + "Table.asset";
        }

        private class ExcelData
        {
            private readonly List<List<string>> dataList = new List<List<string>>();
            public List<List<string>> DataList => dataList;
            public int Column;
            public int Row;

            /// <summary>
            /// 获取列表中的字段
            /// </summary>
            /// <param name="row">目标行</param>
            /// <param name="column">目标列</param>
            /// <returns></returns>
            public string GetData(int row, int column)
            {
                return dataList[row][column];
            }

            /// <summary>
            /// 设置列表中的字段
            /// </summary>
            /// <param name="row">目标行</param>
            /// <param name="column">目标列</param>
            /// <param name="value">目标值</param>
            public void SetData(int row, int column, string value)
            {
                dataList[row][column] = value;
            }
        }
    }

}