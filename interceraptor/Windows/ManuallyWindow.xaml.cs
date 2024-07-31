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

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Owner.Show();
        }

        private Brush ButtonsColorByGroup(int group)
        {
            var colors = "#66CCFF,#FF7FEE9D,#FFFB9DAE".Split(',');
            return (Brush)(new BrushConverter().ConvertFrom(colors[group]));
        }

        public void InitServicesTable()
        {
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
                    Content = new TextBlock
                    {
                        Text = service.name,
                        TextAlignment = TextAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    },
                    Margin = new Thickness(2),
                };

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
    }
}
