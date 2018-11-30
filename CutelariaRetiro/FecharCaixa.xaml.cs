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
using System.Windows.Shapes;

namespace CutelariaRetiro
{
    /// <summary>
    /// Lógica interna para FecharCaixa.xaml
    /// </summary>
    public partial class FecharCaixa : Window
    {
        public FecharCaixa()
        {
            InitializeComponent();
            FillCaixa();
        }

        private void FillCaixa()
        {
            CaixaBLL bll = new CaixaBLL();
            if (bll.CaixaAberto())
            {
                Caixa cx = bll.GetCaixaAberto();
                decimal saldoInicial = cx.GetSaldoInicial();
                decimal totalDinheiro = cx.GetTotalFormaPg(FormaPagamento.DINHEIRO);
                decimal totalCartao = cx.GetTotalFormaPg(FormaPagamento.CARTAO);
                decimal totalFormasPg = (totalDinheiro + totalCartao);
                decimal totalRetirada = cx.GetTotalRetirada();

                txSaldoDinheiro.Text = $"R$ {(saldoInicial + totalDinheiro).ToString("N2")}";
                txValorFinal.Text = $"R$ {(saldoInicial + totalDinheiro + totalCartao - totalRetirada).ToString("N2")}";
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Fecha()
        {
            CaixaBLL bll = new CaixaBLL();
            Caixa cx = bll.GetCaixaAberto();
            cx = bll.Find(cx.Id);
            cx.DataFechamento = DateTime.Now;

            SalvaTxt(cx);

            MovimentoCaixa mc = new MovimentoCaixa();
            mc.CaixaId = cx.Id;
            mc.Valor = decimal.Parse(txValorFinal.Text.Replace("R$", ""));
            mc.Obs = "Fechamento do caixa";
            mc.FormaPagamento = (int)FormaPagamento.DINHEIRO;
            mc.Tipo = (int)TipoMovCaixa.Saida;

            MovimentoCaixaBLL mcBll = new MovimentoCaixaBLL();
            mcBll.Save(mc);

            cx = bll.Find(cx.Id);
            cx.DataFechamento = DateTime.Now;
            cx.Aberto = false;
            bll.Save(cx);
        }

        private void SalvaTxt(Caixa cx)
        {
            CaixaBLL bll = new CaixaBLL();

            decimal saldoInicial = cx.GetSaldoInicial();
            decimal totalDinheiro = cx.GetTotalFormaPg(FormaPagamento.DINHEIRO);
            decimal totalCartao = cx.GetTotalFormaPg(FormaPagamento.CARTAO);
            decimal totalFormasPg = (totalDinheiro + totalCartao);
            decimal totalRetirada = cx.GetTotalRetirada();

            string txt = $@"N° {cx.Id}
Caixa Cutelaria Retiro (Serviços)
Abertura: {cx.DataAbertura.ToString("dd/MM/yyyy HH:mm")}
Fechamento {cx.DataFechamento.Value.ToString("dd/MM/yyyy HH:mm")}

--------------------------------------
(+) SALDO INICIAL   {saldoInicial}
(+) ENTRADAS NO CAIXA
    DINHEIRO R$ {totalDinheiro.ToString("N2")}
    CARTÃO   R$ {totalCartao.ToString("N2")}
    ----------------------------------
    TOTAL    R$ {totalFormasPg.ToString("N2")}

(-) SAIDAS NO CAIXA  R$ {totalRetirada.ToString("N2")}
(=) SALDO FINAL
    SOMENTE DINHEIRO  R$ {(saldoInicial + totalDinheiro - totalRetirada).ToString("N2")}
    TUDO              R$ {(saldoInicial + totalDinheiro + totalCartao - totalRetirada).ToString("N2")}";

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            path += $@"\CAIXA CUTELARIA - DIA {DateTime.Now.ToString("dd-MM-yyyy")}.txt";
            File.WriteAllText(path, txt);

            System.Diagnostics.Process.Start(path);

            if (!Directory.Exists(@".\Caixa\"))
                Directory.CreateDirectory(@".\Caixa\");

            int counter = 0;
            string backupName = $@".\Caixa\{DateTime.Now.ToString("dd-MM-yyyy")}.txt";
            while (File.Exists(backupName))
            {
                counter += 1;
                backupName = $@".\Caixa\{DateTime.Now.ToString("dd-MM-yyyy")} ({counter}).txt";
            }

            File.Copy(path, backupName);
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Fecha();
            Close();
        }
    }
}