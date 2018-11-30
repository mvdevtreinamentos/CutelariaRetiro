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
    /// Lógica interna para PagamentoAdiantado.xaml
    /// </summary>
    public partial class PagamentoAdiantado : Window
    {
        private int ServicoId { get; set; }
        public bool Confirmado { get; set; }
        private bool FecharServico { get; set; }
        public PagamentoAdiantado(decimal valorServico,
            int servicoId, bool fecharServico)
        {
            InitializeComponent();

            txValorServico.ToMoney();
            txValorAdiantamento.ToMoney();
            txValorRestante.ToMoney();

            ServicoId = servicoId;
            txValorServico.Text = valorServico.ToString("N2");
            FillCb();

            FecharServico = fecharServico;
            if (fecharServico)
            {
                txValorAdiantamento.Text = valorServico.ToString("N2");
                txValorAdiantamento.IsEnabled = false;
                lbSub.Visibility = Visibility.Hidden;
                lbTitulo.Content = "Concluir serviço";
            }
        }

        private void FillCb()
        {
            List<KeyValuePair<int, string>> formasPg = new List<KeyValuePair<int, string>>();
            formasPg.Add(new KeyValuePair<int, string>(-1, "Forma de pagamento"));
            formasPg.Add(new KeyValuePair<int, string>((int)FormaPagamento.DINHEIRO, "Dinheiro"));
            formasPg.Add(new KeyValuePair<int, string>((int)FormaPagamento.CARTAO, "Cartão"));

            comboBox.ItemsSource = formasPg;
            comboBox.DisplayMemberPath = "Value";
            comboBox.SelectedValuePath = "Key";
            comboBox.SelectedIndex = 0;
        }

        private void txValorAdiantamento_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                decimal servico = decimal.Parse(txValorServico.Text);
                decimal adiantamento = decimal.Parse(txValorAdiantamento.Text);
                decimal restante = (servico - adiantamento);
                txValorRestante.Text = restante.ToString("N2");
            }
            catch { }
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            Caixa cx = new CaixaBLL().GetCaixaAberto();
            MovimentoCaixa mc = new MovimentoCaixa();
            mc.ServicoId = ServicoId;
            mc.CaixaId = cx.Id;
            mc.FormaPagamento = (int)comboBox.SelectedValue;
            mc.Valor = decimal.Parse(txValorAdiantamento.Text);
            mc.Tipo = (int)TipoMovCaixa.Entrada;
            mc.Obs = $"Adiantamento de pagamento do serviço N° {ServicoId}";

            if(mc.FormaPagamento == -1)
            {
                MessageBox.Show("Selecione uma forma de pagamento",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MovimentoCaixaBLL bll = new MovimentoCaixaBLL();
            bll.Save(mc);

            if(FecharServico)
            {
                ServicoBLL servBLL = new ServicoBLL();
                var serv = servBLL.Find(ServicoId);
                serv.Finalizado = true;
                servBLL.Save(serv);

                List<MaterialServico> materiaisServ = serv.MaterialServico.ToList();

                foreach(MaterialServico m in materiaisServ)
                {
                    MaterialBLL matBll = new MaterialBLL();
                    var material = matBll.Find(m.MaterialId);
                    material.Estoque -= m.Quantidade;
                    matBll.Save(material);
                }
            }

            Confirmado = true;
            Close();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
