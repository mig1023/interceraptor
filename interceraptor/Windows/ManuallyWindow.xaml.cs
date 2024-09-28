using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace interceraptor.Windows
{
    public partial class ManuallyWindow : Window
    {
        public ManuallyWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            this.Owner.Show();
        }

        private Brush ButtonsColorByGroup(int group)
        {
            var colors = "#66CCFF,#FF7FEE9D,#FFFB9DAE".Split(',').ToList();

            if (group < colors.Count)
                return (Brush)(new BrushConverter().ConvertFrom(colors[group]));
            else
                return SystemColors.WindowBrush;
        }

        private UIElement RemoveElement(string name, UIElementCollection childrens)
        {
            var element = LogicalTreeHelper.FindLogicalNode(this, name) as UIElement;
            childrens.Remove(element);

            return element;
        }

        private void PriceManualClose(object button)
        {
            var window = Window.GetWindow(button as DependencyObject);
            window.Hide();
        }

        private void ServiceButtonWithContent(object sender, string id, int count, bool priced = false)
        {
            var service = sender as Button;

            int column = Grid.GetColumn(service);
            int row = Grid.GetRow(service);

            var stack = new DockPanel
            {
                Name = $"stack_{id}",
                Margin = new Thickness(2),
                LastChildFill = true,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            var serviceCount = new Label
            {
                Name = $"count_{id}",
                Content = count,
                Width = 50,
                FontSize = 24,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            stack.Children.Add(serviceCount);
            DockPanel.SetDock(serviceCount, Dock.Left);

            if (!priced)
            {
                var subButton = new Button
                {
                    Name = $"sub_{id}",
                    Content = "X",
                    FontSize = 18,
                    Margin = new Thickness(2),
                    Width = 40,
                    Background = service.Background,
                };
                subButton.Click += (s, r) => ServiceSubClick(s, id);
                stack.Children.Add(subButton);
                DockPanel.SetDock(subButton, Dock.Right);
            }

            Services.Children.Remove(service);
            stack.Children.Add(service);
            DockPanel.SetDock(service, Dock.Right);

            Services.Children.Add(stack);
            Grid.SetColumn(stack, column);
            Grid.SetRow(stack, row);
        }

        private void ServiceButtonWithoutContent(object sender, string id, bool priced = false)
        {
            var service = sender as Button;
            var addButton = LogicalTreeHelper.FindLogicalNode(this, $"add_{id}") as Button;

            var element = RemoveElement($"stack_{id}", Services.Children);
            int column = Grid.GetColumn(element);
            int row = Grid.GetRow(element);

            RemoveElement($"count_{id}", (element as DockPanel).Children);

            if (!priced)
                RemoveElement($"sub_{id}", (element as DockPanel).Children);

            (element as DockPanel).Children.Remove(addButton);

            Services.Children.Add(addButton);
            Grid.SetColumn(addButton, column);
            Grid.SetRow(addButton, row);
        }

        private void PriceManualAddClick(object sender, string id, string price, string comment, object button)
        {
            var services = Create.Services.Get();
            
            if (services.Count(id) == 0)
                ServiceButtonWithContent(sender, id, 1, priced: true);

            services.Add(id, price, comment);

            PriceManualClose(button);
        }

        private void PriceManualRemoveClick(object sender, string id, object button)
        {
            var services = Create.Services.Get();
            services.Sub(id);

            ServiceButtonWithoutContent(sender, id, priced: true);
            PriceManualClose(button);
        }

        private void ServiceAddClick(object sender, string id)
        {
            var services = Create.Services.Get();

            if (services.Properties(id).isPriceManual)
            {
                double left = this.Left > 0 ? this.Left : 2;
                double top = this.Top > 0 ? this.Top : 2;

                PriceManual priceManual = new PriceManual();
                priceManual.Left = left + (this.Width / 2) - (priceManual.Width / 2);
                priceManual.Top = top + (this.Height / 2) - (priceManual.Height / 2);

                priceManual.InitPriceCommentTable(services.Properties(id).isComment, id,
                    (price, comment, button) => PriceManualAddClick(sender, id, price, comment, button),
                    (button) => PriceManualRemoveClick(sender, id, button));

                if (services.Count(id) != 0)
                {
                    CRM.ServicesData service = services.List.Where(x => x.id == id).FirstOrDefault();
                    priceManual.LoadPriceComment(service.price.ToString(), service.comment);
                }

                priceManual.Owner = this;
                priceManual.Show();
            }
            else if (services.Count(id) == 0)
            {
                services.Add(id);

                ServiceButtonWithContent(sender, id, services.Count(id));
            }
            else
            {
                services.Add(id);

                var serviceCount = LogicalTreeHelper.FindLogicalNode(this, $"count_{id}") as Label;
                serviceCount.Content = services.Count(id);
            }
        }

        private void ServiceSubClick(object sender, string id)
        {
            var services = Create.Services.Get();
            services.Sub(id);

            if (services.Count(id) == 0)
            {
                ServiceButtonWithoutContent(sender, id);
            }
            else
            {
                var serviceCount = LogicalTreeHelper.FindLogicalNode(this, $"count_{id}") as Label;
                serviceCount.Content = services.Count(id);
            }
        }

        public void InitServicesTable()
        {
            var services = Create.Services.Get();
            services.Init();

            var crmServices = CRM.Services.Get();

            int maxRow = crmServices.List.Count / 3;
            int lastRow = crmServices.List.Count % 3;

            if (lastRow > 0)
                maxRow += 1;

            for (int i = 0; i < maxRow; i++)
                Services.RowDefinitions.Add(new RowDefinition());

            int column = 0;
            int row = 0;

            var listServices = crmServices.List
                .OrderBy(x => x.group == null)
                .ThenBy(x => x.group)
                .ToList();

            foreach (var service in listServices)
            {
                var button = new Button
                {
                    Name = $"add_{service.id}",
                    Content = new TextBlock
                    {
                        Text = service.title,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        FontSize = 16
                    },
                    Margin = new Thickness(2),
                    Padding = new Thickness(4),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };

                button.Click += (s, r) => ServiceAddClick(s, service.id);

                if (service.group != null)
                {
                    button.Background = ButtonsColorByGroup(service.group ?? 0);
                }

                Services.Children.Add(button);
                Grid.SetRow(button, row);
                Grid.SetColumn(button, column);

                row += 1;

                if (row >= maxRow)
                {
                    row = 0;
                    column += 1;
                }
            } 
        }

        private void RowText(string text, int row, int column, bool selected = false)
        {
            var label = new Label
            {
                Content = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    FontSize = selected ? 14 : 10
                }
            };

            CheckData.Children.Add(label);
            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);
        }

        private void RowLine(string first, string second, string third, string fourth, int row, bool selected = false)
        {
            RowText(first, row, 0, selected);
            RowText(second, row, 1, selected);
            RowText(third, row, 2, selected);
            RowText(fourth, row, 3, selected);
        }

        private async void CloseCheck_Click(object sender, RoutedEventArgs e)
        {
            Services.Visibility = Visibility.Collapsed;
            Additional.Visibility = Visibility.Collapsed;
            Wait.Visibility = Visibility.Visible;

            Create.DocPack docPack = Create.DocPack.Get();
            var services = Create.Services.Get();

            docPack.Init(services.List);

            var calculedDocPack = await docPack.Calculate();

            CloseCheck.IsEnabled = false;
            Wait.Visibility = Visibility.Collapsed;

            RowLine("НАИМЕНОВАНИЕ", "ЦЕНА", String.Empty, "СУММА", 0, selected: true);

            int row = 1;
            decimal summ = 0;

            foreach (var service in docPack.List())
            {
                CheckData.RowDefinitions.Add(new RowDefinition());
                RowLine(service.name, service.price.ToString(), service.qty.ToString(), service.summ.ToString(), row);

                summ += service.summ;
                row += 1;
            }

            RowLine("ИТОГО", String.Empty, String.Empty, summ.ToString(), row, selected: true);

            Verify.Visibility = Visibility.Visible;
            PrintWithMoney.IsEnabled = true;
            PrintWithCard.IsEnabled = true;
        }

        private void PrintResultOutput(Server.PayResponse response)
        {
            if (response.error != null)
            {
                MessageBox.Show($"Ошибка печати чека:\n{response.error.message}");
            }
            else
            {
                MessageBox.Show($"Сдача: {response.cashChange}");
            }
        }

        private async void PrintWithMoney_Click(object sender, RoutedEventArgs e)
        {
            Create.DocPack docPack = Create.DocPack.Get();
            Server.PayResponse response = await docPack.Print(CashMoney.Text, Email.Text, "MONEY");
            PrintResultOutput(response);
        }

        private async void PrintWithCard_Click(object sender, RoutedEventArgs e)
        {
            Create.DocPack docPack = Create.DocPack.Get();
            Server.PayResponse response = await docPack.Print(String.Empty, Email.Text, "CREDIT_CARD");
            PrintResultOutput(response);
        }

        private void WaitSpinner_MediaEnded(object sender, RoutedEventArgs e)
        {
            WaitSpinner.Position = new TimeSpan(0, 0, 1);
            WaitSpinner.Play();
        }

        private void Payback_Checked(object sender, RoutedEventArgs e)
        {
            PaybackText.Foreground = Brushes.Red;
            PaybackDate.IsEnabled = true;
            PaybackFP.IsEnabled = true;
        }

        private void Payback_Unchecked(object sender, RoutedEventArgs e)
        {
            PaybackText.Foreground = Brushes.Black;
            PaybackDate.IsEnabled = false;
            PaybackFP.IsEnabled = false;
        }
    }
}
