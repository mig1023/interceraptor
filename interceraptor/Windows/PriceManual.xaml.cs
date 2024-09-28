using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace interceraptor.Windows
{
    public partial class PriceManual : Window
    {
        public delegate void AddAction(string price, string comment, object button);
        public delegate void RemoveAction(object button);

        private string _selectedServiceId { get; set; }

        private AddAction _addAction {get;set;}
        private RemoveAction _removeAction { get; set; }

        public PriceManual()
        {
            InitializeComponent();
        }

        public void InitPriceCommentTable(bool withComment, string id,
            AddAction add, RemoveAction remove)
        {
            _selectedServiceId = id;
            _addAction = add;
            _removeAction = remove;

            var crmServices = CRM.Services.Get();

            ServiceName.Content = crmServices.List
                .Where(x => x.id == id)
                .FirstOrDefault()
                .name;

            if (withComment)
            {
                CommentPanel.Visibility = Visibility.Visible;
                Grid.SetColumnSpan(PricePanel, 1);
            }
            else
            {
                CommentPanel.Visibility = Visibility.Collapsed;
                Grid.SetColumnSpan(PricePanel, 2);
            }
        }

        public void LoadPriceComment(string price, string comment)
        {
            Price.Text = price;
            Comment.Text = comment;
        }

        private void Add_Click(object sender, RoutedEventArgs e) =>
            _addAction(Price.Text, Comment.Text, sender);

        private void Remove_Click(object sender, RoutedEventArgs e) =>
            _removeAction(sender);
    }
}
