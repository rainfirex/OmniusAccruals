using CsvHelper;
using OmniusAccruals.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace OmniusAccruals.Modules
{
    class CSVData
    {
        public const string FI_TYPE_ZADOLZN = "[1] Текущая";
        public const string FI_TYPE = "[11] Счет-фактура";
        public const string FI_TYPE_RASCHET = "Итоговый расчет (EE)";

        /// <summary>
        /// Полный экспорт
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fileName"></param>
        /// <param name="isUtf8"></param>
        public static void exportFull(BindingList<Accrual> list, string fileName, bool isUtf8 = true)
        {
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                csvWriter.WriteHeader<Division>();
                csvWriter.WriteRecords(list);

                writer.Flush();
                string result = Encoding.UTF8.GetString(mem.ToArray());

                Encoding encoding = (isUtf8) ? Encoding.UTF8 : Encoding.GetEncoding(1251);

                File.WriteAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{fileName}", result, encoding);
            }
        }

        /// <summary>
        /// Сбер формат экспорт
        /// </summary>
        /// <param name="list"></param>
        /// <param name="fileName"></param>
        /// <param name="isUtf8"></param>
        /// <returns></returns>
        internal static string exportSB(BindingList<Accrual> list, string fileName, bool isUtf8 = true)
        {
            string path = null;            

            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer, System.Globalization.CultureInfo.CurrentCulture))
            {
                foreach(Accrual accrual in list)
                {
                    if(FI_TYPE_ZADOLZN.Equals(accrual._Bound_F_Debts) && FI_TYPE.Equals(accrual._Bound_F_Doc_Types) && FI_TYPE_RASCHET.Equals(accrual._Bound_F_Calc_Algorithms))
                    {
                        csvWriter.WriteField(accrual.SD_Subscr_CodeNumber);
                        csvWriter.WriteField("");
                        csvWriter.WriteField(accrual.C_FIO);
                        csvWriter.WriteField(accrual.N_Amount.ToString().Replace(",", "."));
                        csvWriter.WriteField(accrual.C_Number);
                        csvWriter.WriteField("");

                        csvWriter.NextRecord();
                    }                    
                }
                writer.Flush();
                var result = Encoding.UTF8.GetString(mem.ToArray());

                if (!String.IsNullOrEmpty(result))
                {
                    path = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\{fileName}";

                    Encoding encoding = (isUtf8) ? Encoding.UTF8 : Encoding.GetEncoding(1251);

                    File.WriteAllText(path, result, encoding);
                }
                return path;
            }
        }
    }
}
