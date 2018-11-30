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
    /// Lógica interna para IncluirItemServico.xaml
    /// </summary>
    public partial class IncluirItemServico : Window
    {
        public ItemServico Item { get; set; }
        public IncluirItemServico()
        {
            InitializeComponent();

            txValorUnit.ToMoney();
            txQuant.ToNumeric();
            txTotal.ToMoney();

            List<TipoServico> tipos = new TipoServicoBLL().Search("");
            cbTipoServico.DisplayMemberPath = "Nome";
            cbTipoServico.SelectedValuePath = "Id";
            cbTipoServico.ItemsSource = tipos;

            if (tipos.Count > 0)
                cbTipoServico.SelectedIndex = 0;
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btIncluir_Click(object sender, RoutedEventArgs e)
        {
            Item = new ItemServico();
            Item.TipoServicoId = (int)cbTipoServico.SelectedValue;
            Item.ValorUnit = decimal.Parse(txValorUnit.Text);
            Item.Quantidade = int.Parse(txQuant.Text);
            Item.Total = decimal.Parse(txTotal.Text);
            Item.Observacoes = txObs.Text;

            Close();
        }

        private void CalcularTotal()
        {
            decimal unit = decimal.Parse(txValorUnit.Text);
            int quant = int.Parse(txQuant.Text);
            decimal total = (unit * quant);
            txTotal.Text = total.ToString("N2");
        }

        private void txValorUnit_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CalcularTotal();
            }
            catch { }
        }

        private void cbTipoServico_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int id = (int)cbTipoServico.SelectedValue;
            TipoServico tipo = new TipoServicoBLL().Find(id);
            txValorUnit.Text = tipo.Preco.ToString("N2");
            txQuant.Text = "1";
        }

        private void txQuant_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                CalcularTotal();
            }
            catch { }
        }
    }
}
