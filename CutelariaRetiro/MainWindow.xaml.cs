using CutelariaRetiro.BLL;
using CutelariaRetiro.Model;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CutelariaRetiro
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists(@".\Data\"))
                Directory.CreateDirectory(@".\Data\");

            CaixaBLL bll = new CaixaBLL();
            if (bll.CaixaAberto())
                btAbrirFecharCaixa.Content = "Fechar Caixa";
            else
                btAbrirFecharCaixa.Content = "Abrir Caixa";

            FillServicosAndamento();
            FillCaixa();
        }

        private void FillServicosAndamento()
        {
            spServicos.Children.Clear();

            var bll = new ServicoBLL();
            List<Servico> servicos = bll.Search(txPesquisa.Text)
                .Where(s => !s.Finalizado).ToList();

            foreach (var serv in servicos)
            {
                var card = new CardServico(serv);
                card.Edicao += Card_Edicao;
                spServicos.Children.Add(card);
            }
        }

        private void Card_Edicao(Servico serv)
        {
            CadServico cad = new CadServico();
            cad.FillForm(serv.Id);
            cad.ShowDialog();

            FillServicosAndamento();
            FillCaixa();
        }

        private void txPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                FillServicosAndamento();
        }

        private void FillCaixa()
        {
            CaixaBLL bll = new CaixaBLL();
            decimal saldoInicial = 0;
            decimal totalDinheiro = 0;
            decimal totalCartao = 0;
            decimal totalFormasPg = 0;
            decimal totalRetirada = 0;

            if (bll.CaixaAberto())
            {
                Caixa cx = bll.GetCaixaAberto();
                saldoInicial = cx.GetSaldoInicial();
                totalDinheiro = cx.GetTotalFormaPg(FormaPagamento.DINHEIRO);
                totalCartao = cx.GetTotalFormaPg(FormaPagamento.CARTAO);
                totalFormasPg = (totalDinheiro + totalCartao);
                totalRetirada = cx.GetTotalRetirada();
                btRetidadaCaixa.Visibility = Visibility.Visible;
            }
            else
            {
                btRetidadaCaixa.Visibility = Visibility.Hidden;
            }

            lbSaldoInicial.Content = $"R$ {saldoInicial.ToString("N2")}";
            lbTotalDinheiro.Content = $"R$ {totalDinheiro.ToString("N2")}";
            lbTotalCartao.Content = $"R$ {totalCartao.ToString("N2")}";
            lbTotalEntradas.Content = $"R$ {totalFormasPg.ToString("N2")}";
            lbTotalGeralDinheiro.Content = $"R$ {(saldoInicial + totalDinheiro - totalRetirada).ToString("N2")}";
            lbTotalGeralTudo.Content = $"R$ {(saldoInicial + totalDinheiro + totalCartao - totalRetirada).ToString("N2")}";
            lbTotalRetirada.Content = $"R$ {totalRetirada.ToString("N2")}";
        }

        private void btAbrirFecharCaixa_Click(object sender, RoutedEventArgs e)
        {
            CaixaBLL bll = new CaixaBLL();
            if (!bll.CaixaAberto())
            {
                AbrirCaixa ac = new AbrirCaixa();
                ac.ShowDialog();
            }
            else
            {
                FecharCaixa fc = new FecharCaixa();
                fc.ShowDialog();
            }

            if (bll.CaixaAberto())
                btAbrirFecharCaixa.Content = "Fechar Caixa";
            else
                btAbrirFecharCaixa.Content = "Abrir Caixa";

            FillCaixa();
        }

        private void btRetidadaCaixa_Click(object sender, RoutedEventArgs e)
        {
            CaixaBLL bll = new CaixaBLL();
            if (!bll.CaixaAberto())
                return;

            RetiradaCaixa rc = new RetiradaCaixa();
            rc.ShowDialog();

            FillCaixa();
        }

        private void menuMateriais_Click(object sender, RoutedEventArgs e)
        {
            CadMateriais cad = new CadMateriais();
            cad.ShowDialog();
        }

        private void menuTiposServico_Click(object sender, RoutedEventArgs e)
        {
            CadTipoServico gt = new CadTipoServico();
            gt.ShowDialog();

            FillServicosAndamento();
            FillCaixa();
        }

        private void menuGerenciaEstoque_Click(object sender, RoutedEventArgs e)
        {
            GerenciaEstoque ge = new GerenciaEstoque();
            ge.ShowDialog();
        }

        private void menuAtendimento_Click(object sender, RoutedEventArgs e)
        {
            CadServico cad = new CadServico();
            cad.ShowDialog();

            FillServicosAndamento();
            FillCaixa();
        }

        private void btHistoricoCaixa_Click(object sender, RoutedEventArgs e)
        {
            HistoricoCaixa hc = new HistoricoCaixa();
            hc.ShowDialog();
        }

        private void btNovoAtendimento_Click(object sender, RoutedEventArgs e)
        {
            CadServico cad = new CadServico();
            cad.ShowDialog();
            FillServicosAndamento();
        }
    }
}
