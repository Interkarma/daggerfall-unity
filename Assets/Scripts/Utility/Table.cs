// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// A very basic data table storing columns and rows of string information controlled by a simple schema.
    /// Source data can be loaded from a CSV-like file with the following format:
    ///   - Dash starts a comment
    ///   schema: id,*name       (defines two columns, first called 'id' and second called 'name', * means this is the primary key)
    ///   0, Apples              (0, Apples populates the two columns defined by schema, with an 'id' of "0" and 'name' of "Apples")
    ///   1, Oranges             (and so on)
    /// Schema must be defined before first data row.
    /// Column names in schema must all be unique, at least one must be the key.
    /// Column values are always stored as strings.
    /// Key column value must be unique in every row.
    /// Can use spaces in values, but not commas (separating character only).
    /// Lookups are done by keys or by indices.
    /// </summary>
    public class Table
    {
        #region Fields

        int columnCount;
        int primaryColumnIndex;
        Column[] columns;
        Dictionary<string, int> columnIndexDict;

        #endregion

        #region Properties

        public int ColumnCount
        {
            get { return GetColumnCount(); }
        }

        public int RowCount
        {
            get { return GetRowCount(); }
        }

        #endregion

        #region Structures

        /// <summary>
        /// Stores name and all row values of a column, and a dictionary for locating value by key.
        /// </summary>
        struct Column
        {
            public string name;
            public List<string> values;
            public Dictionary<string, int> keyIndexDict;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Table()
        {
        }

        /// <summary>
        /// Load constructor.
        /// </summary>
        /// <param name="lines">String array of lines in data file.</param>
        public Table(string[] lines)
        {
            LoadTable(lines);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a data table from an array lines.
        /// </summary>
        /// <param name="lines"></param>
        void LoadTable(string[] lines)
        {
            // Clear existing schema
            columnCount = 0;
            primaryColumnIndex = -1;
            columns = null;
            columnIndexDict = new Dictionary<string, int>();

            // Iterate over all lines
            int rowsLoaded = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                // Skip comment lines
                string text = lines[i].Trim();
                if (text.StartsWith("-"))
                    continue;

                // Skip empty lines
                if (string.IsNullOrEmpty(text))
                    continue;

                // Parse schema
                if (text.StartsWith("schema:", StringComparison.InvariantCultureIgnoreCase))
                {
                    LoadSchema(text);
                    continue;
                }

                // Ignore anything else if schema not loaded
                if (columnCount == 0 || primaryColumnIndex == -1)
                    continue;

                // Load row data
                LoadRow(text, i);
                rowsLoaded++;
            }

            // Throw when schema not found
            if (columnCount == 0 || primaryColumnIndex == -1)
                throw new Exception("Schema not found in source table.");
        }

        /// <summary>
        /// Checks if column name exists in table.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>True if column exists.</returns>
        public bool HasColumn(string columnName)
        {
            if (columnIndexDict.ContainsKey(columnName))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if a key value is present in table.
        /// </summary>
        /// <param name="key">Key value.</param>
        /// <returns>True if key value present.</returns>
        public bool HasValue(string key)
        {
            if (columns[primaryColumnIndex].keyIndexDict.ContainsKey(key))
                return true;

            return false;
        }

        /// <summary>
        /// Gets specified value from column.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="key">Key value.</param>
        /// <returns>Value as string.</returns>
        public string GetValue(string columnName, string key)
        {
            // Validate columnName
            if (!HasColumn(columnName))
                throw new Exception(string.Format("GetValue() columnName '{0}' does not exist.", columnName));

            // Validate key
            if (!HasValue(key))
                throw new Exception(string.Format("GetValue() key '{0}' does not exist.", key));

            int valueIndex = columns[primaryColumnIndex].keyIndexDict[key];
            int columnIndex = columnIndexDict[columnName];

            return columns[columnIndex].values[valueIndex];
        }

        /// <summary>
        /// Gets specified value from column.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="index">Index of row.</param>
        /// <returns>Value as string.</returns>
        public string GetValue(string columnName, int index)
        {
            // Validate columnName
            if (!HasColumn(columnName))
                throw new Exception(string.Format("GetValue() columnName '{0}' does not exist.", columnName));

            // Validate index
            if (index < 0 || index > RowCount)
                throw new Exception(string.Format("GetValue() index '{0}' does not exist.", index));

            int columnIndex = GetColumnIndex(columnName);

            return columns[columnIndex].values[index];
        }

        /// <summary>
        /// Sets specified value in column.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <param name="key">Key value.</param>
        /// <param name="value">Value to set.</param>
        public void SetValue(string columnName, string key, string value)
        {
            // Validate columnName
            if (!HasColumn(columnName))
                throw new Exception(string.Format("SetValue() columnName '{0}' does not exist.", columnName));

            // Validate key
            if (!HasValue(key))
                throw new Exception(string.Format("SetValue() key '{0}' does not exist.", key));

            int valueIndex = columns[primaryColumnIndex].keyIndexDict[key];
            int columnIndex = columnIndexDict[columnName];

            columns[columnIndex].values[valueIndex] = value;
        }

        /// <summary>
        /// Gets column index by name.
        /// </summary>
        /// <param name="columnName">Column name.</param>
        /// <returns>Column index.</returns>
        public int GetColumnIndex(string columnName)
        {
            if (!HasColumn(columnName))
                return -1;

            return columnIndexDict[columnName];
        }

        /// <summary>
        /// Gets column name by index.
        /// </summary>
        /// <param name="index">Index of column.</param>
        /// <returns>Name of column, or empty string.</returns>
        public string GetColumnName(int index)
        {
            if (ColumnCount < 1 || index >= ColumnCount)
                return string.Empty;

            return columns[index].name;
        }

        /// <summary>
        /// Gets index of row by key value.
        /// </summary>
        /// <param name="key">Key value.</param>
        /// <returns>Index of row, or -1 if not found.</returns>
        public int GetRowIndex(string key)
        {
            if (ColumnCount < 1 || RowCount < 1)
                return -1;

            if (columns[primaryColumnIndex].keyIndexDict.ContainsKey(key))
            {
                return columns[primaryColumnIndex].keyIndexDict[key];
            }

            return -1;
        }

        /// <summary>
        /// Gets a row array by index.
        /// </summary>
        /// <param name="index">Index of row.</param>
        /// <returns>String array for row, or empty array if row not found.</returns>
        public string[] GetRow(int index)
        {
            // Must have at least one column and one row, and be in range
            if (ColumnCount < 1 || RowCount < 1 || index >= RowCount)
                return new string[0];

            string[] row = new string[ColumnCount];
            for (int i = 0; i < ColumnCount; i++)
            {
                row[i] = columns[i].values[index];
            }

            return row;
        }

        /// <summary>
        /// Gets a row array by key value.
        /// </summary>
        /// <param name="key">Key value.</param>
        /// <returns>String array for row, or empty array if row not found.</returns>
        public string[] GetRow(string key)
        {
            int index = GetRowIndex(key);
            if (index != -1)
            {
                return GetRow(index);
            }

            return new string[0];
        }

        /// <summary>
        /// Sets row data back to table.
        /// Key value must not be changed as this is used to set back correct row.
        /// Any other value can be changed as needed.
        /// </summary>
        /// <param name="row">Row array.</param>
        /// <returns>True if row set.</returns>
        public bool SetRow(string[] row)
        {
            if (row == null || row.Length != ColumnCount)
                return false;

            int index = GetRowIndex(row[primaryColumnIndex]);
            if (index == -1)
                return false;

            for (int i = 0; i < ColumnCount; i++)
            {
                columns[i].values[index] = row[i];
            }

            return true;
        }

        /// <summary>
        /// Try to get value as int.
        /// </summary>
        public int GetInt(string columnName, string key)
        {
            // Get string value
            string value = GetValue(columnName, key);

            // Try to parse to an int
            int result;
            if (!int.TryParse(value, out result))
            {
                return -1;
            }

            return result;
        }

        #endregion

        #region Private Methods

        void LoadSchema(string text)
        {
            // Get schema columns from comma-separated list to right of colon
            string[] parts = text.Split(':');
            string[] columnNames = parts[1].Split(',');

            // Get column count
            columnCount = columnNames.Length;
            if (columnCount == 0)
                throw new Exception("Table must have at least one column in schema");

            // Load schema
            columns = new Column[columnCount];
            for (int i = 0; i < columnNames.Length; i++)
            {
                // Get column name and get primary column index
                string name = columnNames[i].Trim();
                if (name.StartsWith("*"))
                {
                    if (primaryColumnIndex != -1)
                        throw new Exception("Only one column can be key.");

                    primaryColumnIndex = i;
                    name = name.Substring(1, name.Length - 1);
                }

                // Create new column
                Column column = new Column();
                column.name = name;
                column.values = new List<string>();
                column.keyIndexDict = new Dictionary<string, int>();
                columns[i] = column;

                // Add column to index dict
                columnIndexDict.Add(name, i);
            }

            // Validate primary
            if (primaryColumnIndex == -1)
                throw new Exception("Table must tag at least one column as primary using *.");
        }

        void LoadRow(string text, int lineNumber)
        {
            // Split line by commas
            string[] parts = text.Split(',');
            if (parts.Length != columnCount)
                throw new Exception(string.Format("Row on line {0} does not match schema.", lineNumber));

            // Add values to columns
            for (int i = 0; i < parts.Length; i++)
            {
                // Add value
                string value = parts[i].Trim();
                columns[i].values.Add(value);

                // Link primary key value to current index
                if (i == primaryColumnIndex)
                {
                    if (columns[i].keyIndexDict.ContainsKey(value))
                    {
                        Debug.LogErrorFormat("Duplicate key found: {0}", value);
                    }
                    else
                    {
                        columns[i].keyIndexDict.Add(value, columns[i].values.Count - 1);
                    }
                }
            }
        }

        int GetColumnCount()
        {
            return columnCount;
        }

        int GetRowCount()
        {
            if (GetColumnCount() < 1)
                return 0;

            return columns[0].values.Count;
        }

        #endregion
    }
}