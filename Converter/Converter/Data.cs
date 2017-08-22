using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Infrastructure.Shared.Commands;
using Infrastructure.Shared.DataProviders;

namespace Converter
{
    public class Data : BaseViewModel
    {
        List<List<string>> data { get; set; }
        public string wayToFile { get; set; }
        public string State { get; set; }
        public ICommand ChooseFile { get; set; }
        public ICommand SaveFile { get; set; }
        public Data()
        {
            ChooseFile = new RelayCommand(arg => GetData());
            SaveFile = new RelayCommand(arg => SaveData());
            data = new List<List<string>>();
        }
        public void GetData()
        {
            List<string> listOfData = ReadFile();
            foreach (var str in listOfData)
            {
                data.Add((str.Split(';')).ToList());
            }
            State = "Файл прочитан";
            RaisePropertyChanged("State");
        }
        public void SaveData()
        {
            ConvertData();
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    foreach (var str in data)
                    {
                        sw.WriteLine(str[0] + "," + str[1]);

                    }
                }
            }

            wayToFile = saveFileDialog1.FileName;
            RaisePropertyChanged("wayToFile");
        }
        private List<string> ReadFile()
        {
            List<string> myData = new List<string>();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|csv files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            wayToFile = openFileDialog1.FileName;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                try
                {
                    using (StreamReader sr = new StreamReader(openFileDialog1.OpenFile()))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            myData.Add(line);
                        }
                    }
                    return myData;
                }
                catch (Exception e)
                {
                    System.Windows.Forms.MessageBox.Show("The file could not be read:/n" + e.Message);
                    return null;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("The file could not be opened.");
                return null;
            }

        }
        private void ConvertData()
        {
            data[0][0] = "\"date\"";
            data[0][1] = "\"name\"";
            for (int i=1; i<data.Count; i++)
            {                
                data[i][0] = ConvertDate(data[i].First());
                data[i][1] = ConvertNumbers(data[i].Last());
            }
        }
        private string ConvertDate(string date)
        {
            var mat = date.Split(' ').ToList();
            return "\"" + mat.First().Replace('.', '-') + " " + mat.Last() + "\"";
        }
        private string ConvertNumbers(string numbers)
        {
            double num = 1 / Convert.ToDouble(numbers);
            return "\"" + num.ToString().Replace(',', '.') + "\"";
        }
    }
}
