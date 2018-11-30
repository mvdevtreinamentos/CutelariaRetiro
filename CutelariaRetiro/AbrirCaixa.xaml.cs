using CutelariaRetiro.BLL;
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
    /// Lógica interna para AbrirCaixa.xaml
    /// </summary>
    public partial class AbrirCaixa : Window
    {
        public AbrirCaixa()
        {
            InitializeComponent();
            txSaldoInicial.ToMoney();
            txSaldoInicial.Focus();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal saldoInicial = decimal.Parse(txSaldoInicial.Text);
                string obs = txObs.Text;

                CaixaBLL bll = new CaixaBLL();
                bll.AbreCaixa(saldoInicial, obs);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
