using CutelariaRetiro.BLL;
using CutelariaRetiro.Model;
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

namespace CutelariaRetiro
{
    /// <summary>
    /// Lógica interna para IncluirMaterialServico.xaml
    /// </summary>
    public partial class IncluirMaterialServico : Window
    {
        public bool Cancelado { get; set; }
        public MaterialServico Material { get; private set; }
        public IncluirMaterialServico()
        {
            InitializeComponent();

            txQuant.ToNumeric();
            txValorUnit.ToMoney();
            txTotal.ToMoney();

            Cancelado = true;

            Buscar();
        }

        private void Buscar()
        {
            List<Material> mats = new MaterialBLL().Search(txPesquisa.Text);
            listBox.DisplayMemberPath = "Descricao";
            listBox.ItemsSource = mats;
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Buscar();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Material mat = (Material)listBox.SelectedItem;
            if (mat == null)
                return;

            lbDescricao.Content = mat.Descricao;
            lbREF.Content = mat.Referencia;
            txValorUnit.Text = mat.Preco.ToString("N2");
            txQuant.Text = "1";

            CalculaTotal();
        }

        private void CalculaTotal()
        {
            try
            {
                decimal unit = decimal.Parse(txValorUnit.Text);
                int quant = int.Parse(txQuant.Text);
                decimal total = (unit * quant);
                txTotal.Text = total.ToString("N2");
            }
            catch { }
        }

        private void txValorUnit_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculaTotal();
        }

        private void txQuant_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculaTotal();
        }

        private void btComfirmar_Click(object sender, RoutedEventArgs e)
        {
            Material mat = (Material)listBox.SelectedItem;
            if (mat == null)
                return;

            Material = new MaterialServico();
            Material.MaterialId = mat.Id;
            Material.Preco = decimal.Parse(txValorUnit.Text);
            Material.Quantidade = int.Parse(txQuant.Text);
            Material.Total = decimal.Parse(txTotal.Text);

            Cancelado = false;
            Close();
        }
    }
}
