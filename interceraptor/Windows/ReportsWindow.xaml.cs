using System;
using System.Windows;

namespace interceraptor.Windows
{
    public partial class ReportsWindow : Window
    {
        private Cashbox.Printer _cashbox { get; set; }

        private delegate bool Print(); 

        public ReportsWindow()
        {
            InitializeComponent();
            _cashbox = Cashbox.Printer.Get();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Show();
        }

        private void ReportPrinting(Print print)
        {
            if (!print())
            {
                var output = Output.MessageBoxes.Get();
                output.MessageBoxError(_cashbox.LastError());
            }
        }

        private void ReportWithCleaning_Click(object sender, RoutedEventArgs e)
        {
            var output = Output.MessageBoxes.Get();

            if (output.ReportCleaning() == MessageBoxResult.Yes)
                ReportPrinting(() => _cashbox.ReportCleaning());
        }

        private void ReportWithoutCleaning_Click(object sender, RoutedEventArgs e) =>
            ReportPrinting(() => _cashbox.ReportWithoutCleaning());

        private void ReportByDepartment_Click(object sender, RoutedEventArgs e) =>
            ReportPrinting(() => _cashbox.ReportDepartment());

        private void TaxReport_Click(object sender, RoutedEventArgs e) =>
            ReportPrinting(() => _cashbox.ReportTax());

        private void RepeatDocumet_Click(object sender, RoutedEventArgs e) =>
            ReportPrinting(() => _cashbox.RepeatPrint());

        private void ContinueDocument_Click(object sender, RoutedEventArgs e) =>
            ReportPrinting(() => _cashbox.ContinueDocument());

        private void CancelDocument_Click(object sender, RoutedEventArgs e) =>
            ReportPrinting(() => _cashbox.CancelDocument());

        private void Income_Click(object sender, RoutedEventArgs e)
        {
            ReportPrinting(() => _cashbox.CashIncome(MoneyIncome.Text));
            MoneyIncome.Text = String.Empty;
        }

        private void Outcome_Click(object sender, RoutedEventArgs e)
        {
            ReportPrinting(() => _cashbox.CashOutcome(MoneyOutcome.Text));
            MoneyOutcome.Text = String.Empty;
        }
    }
}
