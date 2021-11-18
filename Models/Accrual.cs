namespace OmniusAccruals.Models
{
    public class Accrual
    {
        public decimal SD_Subscr_CodeNumber { get; set; } // №ЛС
        public string C_FIO { get; set; } // Потребитель
        public string _Bound_F_Debts { get; set; } // Вид задолженности
        public string C_Doc_linked { get; set; } // Основание
        public string _Bound_F_Doc_Types { get; set; } // Тип
        public string D_Date { get; set; } // Дата
        public string C_Number { get; set; } // Номер
        public int N_Number { get; set; } // Порядковый номер
        public string FD_Documents_C_Period_Name { get; set; } // Период
        public string _Bound_F_Status { get; set; } //Статус
        public string D_Post_Date { get; set; } //Дата проведения
        public string C_DT_Period { get; set; } //Расчетный период
        public decimal N_Quantity { get; set; } //Количество
        public string _Bound_F_Calc_Algorithms { get; set; } //Вид расчета
        public decimal N_AmountWithoutTax { get; set; } //Начислено
        public decimal N_Tax_Amount { get; set; } //Сумма налога
        public decimal N_Amount { get; set; } //Сумма начисления
        public string D_Date_Due { get; set; } //Срок оплаты
        public decimal N_Amount_Open { get; set; } //Открытая сумма
        public decimal N_Cost_Decrease { get; set; } // Начислено
        public decimal N_Tax_Amount_Decrease { get; set; } //Сумма налога
        public decimal N_Amount_Decrease { get; set; } //ИТОГО
        public decimal N_Cost_Increase { get; set; } //Начислено
        public decimal N_Tax_Amount_Increase { get; set; } //Сумма налога
        public decimal N_Amount_Increase { get; set; } //ИТОГО
    }
}
