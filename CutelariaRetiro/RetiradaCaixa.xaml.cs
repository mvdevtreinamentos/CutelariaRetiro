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
    /// Lógica interna para RetiradaCaixa.xaml
    /// </summary>
    public partial class RetiradaCaixa : Window
    {
        public RetiradaCaixa()
        {
            InitializeComponent();

            txValor.ToMoney();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            CaixaBLL bll = new CaixaBLL();
            Caixa cx = bll.GetCaixaAberto();

            MovimentoCaixa mc = new MovimentoCaixa();
            mc.CaixaId = cx.Id;
            mc.Obs = txDescricao.Text;
            mc.Tipo = (int)TipoMovCaixa.Saida;
            mc.Valor = decimal.Parse(txValor.Text);
            mc.FormaPagamento = (int)FormaPagamento.DINHEIRO;

            MovimentoCaixaBLL mcBll = new MovimentoCaixaBLL();
            mcBll.Save(mc);

            Close();
        }
    }
}
