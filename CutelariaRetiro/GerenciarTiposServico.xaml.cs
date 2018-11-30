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
    /// Lógica interna para GerenciarTiposServico.xaml
    /// </summary>
    public partial class GerenciarTiposServico : Window
    {
        public GerenciarTiposServico()
        {
            InitializeComponent();

            dataGrid.AplicarPadroesFinanceiro();
            Buscar();
        }

        private void btNovo_Click(object sender, RoutedEventArgs e)
        {
            CadTipoServico cad = new CadTipoServico();
            cad.ShowDialog();

            Buscar();
        }

        private void Buscar()
        {
            TipoServicoBLL bll = new TipoServicoBLL();
            List<TipoServico> list = bll.Search(txPesquisa.Text);
            dataGrid.ItemsSource = list;
        }

        private void btAlterar_Click(object sender, RoutedEventArgs e)
        {
            TipoServico tipo = (TipoServico)dataGrid.SelectedItem;
            if (tipo == null)
                return;

            CadTipoServico cad = new CadTipoServico();
            cad.FillForm(tipo);
            cad.ShowDialog();

            Buscar();
        }
    }
}