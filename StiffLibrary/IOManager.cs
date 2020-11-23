using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace StiffLibrary
{
    public static class IOManager
    {
        public static String[] GetFile(string path, bool excludeLineBreaksInsideTexts = false)
        {
            List<String> lines = new List<string>();
            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, true);
                if (excludeLineBreaksInsideTexts)
                {
                    String file = sr.ReadToEnd();
                    Int32 index = 0;

                    string lineBuffer = "";
                    bool bOnText = false;
                    while(index < file.Length)
                    {
                        char c = file[index];
                        if (c == '"')
                            bOnText = !bOnText;
                        if(!bOnText && c == '\n')
                        {
                            lines.Add(lineBuffer);
                            lineBuffer = "";
                            index++;
                            continue;
                        }
                        lineBuffer += c;
                        index++;
                    }
                    if(lineBuffer != "")
                    {
                        lines.Add(lineBuffer);
                        lineBuffer = "";
                        index++;
                    }
                }
                else
                {
                    while (!sr.EndOfStream)
                    {
                        lines.Add(sr.ReadLine());
                    }
                }

                sr.Close();
                sr.Dispose();
            }
            return lines.ToArray();
        }

        public static bool WriteFile(string path, String[] lines, bool append = false)
        {
            bool success = false;
            StreamWriter sw = new StreamWriter(path, append);
            foreach(String line in lines)
            {
                sw.WriteLine(line);
            }
            sw.Close();
            sw.Dispose();
            success = true;
            return success;
        }

        public static void EncryptFile(string path)
        {
            File.Encrypt(path);
        }

        public static void DecryptFile(string path)
        {
            File.Decrypt(path);
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }
        public static bool DirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public static void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }

        public static String[] GetAllFilePathsInDirectory(string directoryPath, string containingInName="")
        {
            DirectoryInfo di = new DirectoryInfo(directoryPath);
            List<string> filePaths = new List<string>();
            foreach(FileInfo filInfo in di.GetFiles())
            {
                if(containingInName == "")
                {
                    filePaths.Add(filInfo.FullName);
                }
                else if (filInfo.Name.Contains(containingInName))
                {
                    filePaths.Add(filInfo.FullName);
                }
            }
            foreach(DirectoryInfo dirInfo in di.GetDirectories())
            {
                filePaths.AddRange(GetAllFilePathsInDirectory(dirInfo.FullName, containingInName));
            }
            return filePaths.ToArray();
        }
    }

    public struct CSV
    {
        private Dictionary<FName, string[]> _columns; //FName headers and string values
        private int _rowCount;
        public CSV(string[] headers, string[,] rows)//10 5
        {
            //Initialization
            _columns = new Dictionary<FName, string[]>();
            List<FName> headersList = new List<FName>();
            _rowCount = rows.GetLength(0);

            //Set headers
            foreach (string header in headers)
            {
                headersList.Add(new FName(header));
            }

            //Invert rows for columns
            int rowCount = _rowCount;
            int columnCount = rows.GetLength(1);

            string[,] columns = new string[columnCount, rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                for(int w = 0; w < columnCount; w++)
                {
                    columns[w, i] = rows[i, w];
                }
            }
            List<string[]> columnsList = new List<string[]>();
            for(int i = 0; i < columnCount; i++)
            {
                List<string> column = new List<string>();
                for(int w = 0; w < rowCount; w++)
                {
                    column.Add(columns[i, w]);
                }
                columnsList.Add(column.ToArray());
            }
            //finish transposing

            //Add Columns
            for(int i = 0; i < headersList.Count; i++)
            {
                _columns.Add(headersList[i], columnsList[i].ToArray());
            }
        }

        public string[] GetRow(int rowNumber)
        {
            List<string> row = new List<string>();
            foreach(FName header in _columns.Keys)
            {
                row.Add(_columns[header][rowNumber]);
            }
            return row.ToArray();
        }

        public string[] GetColumn(FName header)
        {
            if (!_columns.ContainsKey(header))
            {
                return new string[0];
            }
            string[] copy = _columns[header];
            return copy.ToArray();
        }

        public string[] GetColumn(int columnNumber)
        {
            if(columnNumber < 0 || columnNumber >= _columns.Count)
            {
                return new string[0];
            }
            FName[] copy =_columns.Keys.ToArray();
            FName columnHeader = copy[columnNumber];
            return GetColumn(columnHeader);
        }

        public string GetCell(FName header, int row)
        {
            string[] column = GetColumn(header);
            if(row < 0 || row >= column.Length)
            {
                return string.Empty;
            }
            return column[row];
        }

        public string GetCell(int column, int row)
        {
            string[] getColumn = GetColumn(column);
            if (row < 0 || row >= getColumn.Length)
            {
                return string.Empty;
            }
            return getColumn[row];
        }

        public string[] SelectColumnCells(FName targetHeader, FName compareHeader, string compareValue, bool dontRepeat = false)
        {
            string[] target = GetColumn(targetHeader);
            string[] compare = GetColumn(compareHeader);

            List<int> rows = new List<int>();
            for(int i = 0; i < compare.Length; i++)
            {
                if(compare[i] == compareValue)
                {
                    rows.Add(i);
                }
            }

            List<string> selectedCells = new List<string>();
            foreach(int i in rows)
            {
                if (dontRepeat)
                {
                    if (!selectedCells.Contains(target[i]))
                    {
                        selectedCells.Add(target[i]);
                    }
                }
                else
                {
                    selectedCells.Add(target[i]);
                }                
            }
            return selectedCells.ToArray();
        }

        public string[] SelectColumnCells(FName targetHeader, FName compareHeader, Predicate<string> predicate, bool dontRepeat = false)
        {
            string[] target = GetColumn(targetHeader);
            string[] compare = GetColumn(compareHeader);

            List<int> rows = new List<int>();
            for (int i = 0; i < compare.Length; i++)
            {
                if (predicate(compare[i]))
                {
                    rows.Add(i);
                }
            }

            List<string> selectedCells = new List<string>();
            foreach (int i in rows)
            {
                if (dontRepeat)
                {
                    if (!selectedCells.Contains(target[i]))
                    {
                        selectedCells.Add(target[i]);
                    }
                }
                else
                {
                    selectedCells.Add(target[i]);
                }
            }
            return selectedCells.ToArray();
        }

        public CSV SelectRows(FName compareHeader, string compareValue)
        {
            string[] compare = GetColumn(compareHeader);

            List<int> rows = new List<int>();
            for (int i = 0; i < compare.Length; i++)
            {
                if (compare[i] == compareValue)
                {
                    rows.Add(i);
                }
            }

            List<string[]> selectedRowsList = new List<string[]>();
            foreach (int i in rows)
            {
                selectedRowsList.Add(GetRow(i));
            }
            string[] headers = GetHeaderNames();
            string[,] selectedRows = new string[selectedRowsList.Count, headers.Length];

            for(int i = 0; i < selectedRowsList.Count; i++)
            {
                
                for(int w = 0; w < headers.Length; w++)
                {
                    selectedRows[i, w] = selectedRowsList[i][w];
                }
            }

            return new CSV(headers, selectedRows);
        }

        public CSV SelectRows(FName targetHeader, FName compareHeader, Predicate<string> predicate)
        {
            string[] compare = GetColumn(compareHeader);

            List<int> rows = new List<int>();
            for (int i = 0; i < compare.Length; i++)
            {
                if (predicate(compare[i]))
                {
                    rows.Add(i);
                }
            }

            List<string[]> selectedRowsList = new List<string[]>();
            foreach (int i in rows)
            {
                selectedRowsList.Add(GetRow(i));
            }
            string[] headers = GetHeaderNames();
            string[,] selectedRows = new string[selectedRowsList.Count, headers.Length];

            for (int i = 0; i < selectedRowsList.Count; i++)
            {

                for (int w = 0; w < headers.Length; w++)
                {
                    selectedRows[i, w] = selectedRowsList[i][w];
                }
            }

            return new CSV(headers, selectedRows);
        }

        public bool SetCell(FName column, int row, string newValue)
        {
            if (row < 0 || row >= _rowCount)
                return false;
            if (!_columns.ContainsKey(column))
                return false;
            _columns[column][row] = newValue;
            return true;
        }

        public bool SetCell(int column, int row, string newValue)
        {
            FName[] copy = _columns.Keys.ToArray();
            if (column < 0 || column >= copy.Length)
                return false;
            FName columnHeader = copy[column];
            SetCell(columnHeader, row, newValue);
            return true;
        }

        public bool SetRow(int row, string[] rowValues)
        {
            if (row < 0)
                return false;
            if(row >= _rowCount)
            {
                foreach(FName header in Headers)
                {
                    string[] rows = _columns[header];
                    System.Array.Resize(ref rows, row+1);
                    _columns[header] = rows;
                }
                _rowCount = row + 1;
            }

            int headerCount = 0;
            foreach(FName header in Headers)
            {
                if(row >= _columns[header].Length)
                {
                    _columns[header][row] = rowValues[headerCount];
                }
                headerCount++;
            }
            return true;
        }

        public bool AddColumn(FName header, string defaulValue = "")
        {
            if (_columns.ContainsKey(header))
                return false;
            else
            {
                string[] rows = new string[_rowCount];
                for(int i = 0; i < _rowCount; i++)
                {
                    rows[i] = defaulValue;
                }
                _columns.Add(header, rows);
                return true;
            }
        }

        public FName[] Headers { get { return _columns.Keys.ToArray(); } }

        public string[] GetHeaderNames() {

            List<string> convert = new List<string>();
            foreach(FName header in Headers)
            {
                convert.Add(header.ToString());
            }
            return convert.ToArray();
        }

        public int NumberOfRows { get { return _rowCount; } }

        public bool ValidCSV { get { return _columns.Keys.Count > 0; } }
    }

    public static class CSVManager
    {
        public static CSV GetCSV(string path)
        {
            string[] lines = IOManager.GetFile(path, true);

            List<string> headers = new List<string>();
            List<List<string>> rowsList = new List<List<string>>();
            bool bIsHeader = true;

            foreach(string line in lines)
            {
                List<string> cells = new List<string>();
                string buffer = "";
                bool onText = false;
                
                for(int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '"')
                        onText = !onText;
                    else if(!onText && line[i] == ',')
                    {
                        buffer = buffer.TrimEnd();
                        cells.Add(buffer);
                        buffer = "";
                        continue;
                    }

                    buffer += line[i];
                }
                buffer = buffer.TrimEnd();
                cells.Add(buffer);
                if (bIsHeader)
                {
                    headers = cells;
                    bIsHeader = false;
                }
                else
                {
                    rowsList.Add(cells);
                }
            }
            if (rowsList.Count <= 0)
                return new CSV(new string[1] { "Error" }, new string[1, 1] { { "The CSV had no headers" } });

            int numberOfRows = rowsList.Count;
            int numberOfColumns = headers.Count;
            string[,] rows = new string[numberOfRows, numberOfColumns];

            for(int i = 0; i < numberOfRows; i++)
            {
                for(int w = 0; w < numberOfColumns; w++)
                {
                    if (w >= rowsList[i].Count)
                        rows[i, w] = "";
                    else
                        rows[i, w] = rowsList[i][w];
                }
            }

            return new CSV(headers.ToArray(), rows);
        }

        public static bool WriteCSV(string path, CSV csv)
        {
            //Check if there are at least headers
            if (!csv.ValidCSV)
                return false;

            List<string> linesList = new List<string>();

            //Fill in Headers
            string headersLine = "";
            for(int i = 0; i < csv.Headers.Length; i++)
            {
                if (i != csv.Headers.Length - 1)
                    headersLine += csv.Headers[i].ToString() + ",";
                else
                    headersLine += csv.Headers[i].ToString();
            }
            linesList.Add(headersLine);

            //Fill in Lines/Rows
            for(int i = 0; i < csv.NumberOfRows; i++)
            {
                string[] cells = csv.GetRow(i);
                string line = "";
                for(int w = 0; w < cells.Length; w++)
                {
                    if (w != cells.Length - 1)
                        line += cells[w] + ",";
                    else
                        line += cells[w];
                }
                linesList.Add(line);
            }

            return IOManager.WriteFile(path, linesList.ToArray(), false);
        }
    }
}
