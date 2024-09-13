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
            var colors = "#66CCFF,#FF7FEE9D,#FFFB9DAE".Split(',');
            return (Brush)(new BrushConverter().ConvertFrom(colors[group]));
        }

        private UIElement RemoveElement(string name, UIElementCollection childrens)
        {
            var element = LogicalTreeHelper.FindLogicalNode(this, name) as UIElement;
            childrens.Remove(element);

            return element;
        }

        private void ServiceSubClick(object sender, string id)
        {
            var services = Create.Services.Get();
            services.Sub(id);

            if (services.Count(id) == 0)
            {
                var service = sender as Button;
                var addButton = LogicalTreeHelper.FindLogicalNode(this, $"add_{id}") as Button;

                var element = RemoveElement($"stack_{id}", Services.Children);
                int column = Grid.GetColumn(element);
                int row = Grid.GetRow(element);

                RemoveElement($"count_{id}", (element as DockPanel).Children);
                RemoveElement($"sub_{id}", (element as DockPanel).Children);
                
                (element as DockPanel).Children.Remove(addButton);

                Services.Children.Add(addButton);
                Grid.SetColumn(addButton, column);
                Grid.SetRow(addButton, row);
            }
            else
            {
                var serviceCount = LogicalTreeHelper.FindLogicalNode(this, $"count_{id}") as Label;
                serviceCount.Content = services.Count(id);
            }
        }

        private void ServiceAddClick(object sender, string id)
        {
            var services = Create.Services.Get();
            services.Add(id);

            if (services.Count(id) == 1)
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
                    Content = services.Count(id),
                    Width = 50,
                    FontSize = 24,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                };
                stack.Children.Add(serviceCount);
                DockPanel.SetDock(serviceCount, Dock.Left);

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

                Services.Children.Remove(service);
                stack.Children.Add(service);
                DockPanel.SetDock(service, Dock.Right);

                Services.Children.Add(stack);
                Grid.SetColumn(stack, column);
                Grid.SetRow(stack, row);
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
                        Text = service.name,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
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

        private async void CloseCheck_Click(object sender, RoutedEventArgs e)
        {
            Services.Visibility = Visibility.Collapsed;
            Prices.Visibility = Visibility.Collapsed;
            Wait.Visibility = Visibility.Visible;

            Create.DocPack docPack = Create.DocPack.Get();
            var services = Create.Services.Get();

            docPack.Init(services.List);

            var calculedDocPack = await docPack.Calculate(RGS.Text, KL.Text, Fox.Text);

            string servicesLine = String.Empty;

            foreach (var service in docPack.List())
            {
                servicesLine += service.name + "\t" + service.qty + " шт\t" + service.price + " р\n";
            }

            CloseCheck.IsEnabled = false;
            Wait.Visibility = Visibility.Collapsed;
            CheckText.Content = servicesLine;
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
    }
}
