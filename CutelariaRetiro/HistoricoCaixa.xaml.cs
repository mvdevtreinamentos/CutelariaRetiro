using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
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

namespace CutelariaRetiro
{
    /// <summary>
    /// Lógica interna para HistoricoCaixa.xaml
    /// </summary>
    public partial class HistoricoCaixa : Window
    {
        public HistoricoCaixa()
        {
            InitializeComponent();

            txDia.Text = "01";
            txMes.Text = DateTime.Now.Month.ToString();
            txAno.Text = DateTime.Now.Year.ToString();
        }

        private void Buscar()
        {
            List<string> res = new List<string>();
            DirectoryInfo info = new DirectoryInfo(@".\Caixa\");
            foreach (var file in info.GetFiles())
            {
                string[] parts = file.Name.Split('-');

                if (parts[0].Contains(txDia.Text) &&
                    parts[1].Contains(txMes.Text) &&
                    parts[2].Contains(txAno.Text))
                    res.Add(file.Name.Replace(".txt", ""));
            }

            txConteudo.Text = string.Empty;
            listBox.ItemsSource = res;
        }

        private void btBuscar_Click(object sender, RoutedEventArgs e)
        {
            Buscar();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string content = File.ReadAllText($@".\Caixa\{listBox.SelectedItem.ToString()}.txt");
                content += "\n\n\n";
                txConteudo.Text = content;
            }
            catch { }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            txConteudo.FontSize += 1;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            txConteudo.FontSize -= 1;
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            string s = txConteudo.Text;
            System.Windows.Forms.PrintDialog dialog = new System.Windows.Forms.PrintDialog();

            PrintDocument p = new PrintDocument();
            p.PrintPage += delegate (object sender1, PrintPageEventArgs e1)
            {
                e1.Graphics.DrawString(s, new System.Drawing.Font("Sergoe UI", (float)txConteudo.FontSize), new System.Drawing.SolidBrush(System.Drawing.Color.Black), new System.Drawing.RectangleF(0, 0, p.DefaultPageSettings.PrintableArea.Width, p.DefaultPageSettings.PrintableArea.Height));
            };


            dialog.Document = p;
            dialog.ShowDialog();

            try
            {
                p.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem == null)
                return;

            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();

            if (string.IsNullOrEmpty(dialog.SelectedPath))
                return;

            File.Copy($@".\Caixa\{listBox.SelectedItem.ToString()}.txt", $@"{dialog.SelectedPath}\Caixa Cutelaria do dia {listBox.SelectedItem.ToString()}.txt", true);
            MessageBox.Show("O arquivo foi exportado com sucesso!",
                "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
