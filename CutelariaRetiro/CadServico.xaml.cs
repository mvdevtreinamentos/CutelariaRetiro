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
    /// Lógica interna para CadServico.xaml
    /// </summary>
    public partial class CadServico : Window
    {
        public CadServico()
        {
            InitializeComponent();

            btConcluirServico.Visibility = Visibility.Hidden;
            btExcluir.Visibility = Visibility.Hidden;
            txData.SelectedDate = DateTime.Now;
            txNumBloco.ToNumeric();
            dataGrid.AplicarPadroesFinanceiro();
            datagridMateriais.AplicarPadroesFinanceiro();
            txTotalMateriais.ToMoney();
            txTotalServicos.ToMoney();
            txAdiantamento.ToMoney();
            txFalta.ToMoney();

            var cx = new CaixaBLL().GetCaixaAberto();
            if (cx != null)
                btPagamentoAdiantado.Visibility = Visibility.Visible;
            else
                btPagamentoAdiantado.Visibility = Visibility.Hidden;
        }

        private void SalvarServico()
        {
            try
            {
                Valid();

                ServicoBLL bll = new ServicoBLL();
                int id = int.Parse(txNumBloco.Text);
                Servico serv = (id == 0
                    ? new Servico()
                    : bll.Find(id));
                if (serv == null)
                    serv = new Servico();

                serv.Id = id;
                serv.Data = txData.SelectedDate.Value;
                serv.Cliente = txCliente.Text;
                serv.Telefone = txTelefone.Text;
                serv.Obs = txObs.Text;

                bll.Save(serv);

                txNumBloco.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Valid()
        {
            if (btConcluirServico.Visibility == Visibility.Hidden)
            {
                ServicoBLL bll = new ServicoBLL();
                if (bll.ExisteNumeroBloco(int.Parse(txNumBloco.Text)))
                    throw new Exception("Já existe um serviço com o N° do bloco informado");
            }

            if (int.Parse(txNumBloco.Text) <= 0)
                throw new Exception("Informe um N° de bloco válido");

            if (txData.SelectedDate == null)
                throw new Exception("Informe uma data correta");

            if (string.IsNullOrEmpty(txCliente.Text))
                throw new Exception("Informe o nome do cliente");
        }

        public void FillForm(int id)
        {
            try
            {
                var bll = new ServicoBLL();
                Servico serv = bll.Find(id);
                dataGrid.ItemsSource = serv.ItemServico.ToList();
                datagridMateriais.ItemsSource = serv.MaterialServico.ToList();
                txNumBloco.Text = serv.Id.ToString();
                txData.SelectedDate = serv.Data;
                txCliente.Text = serv.Cliente;
                txTelefone.Text = serv.Telefone;
                txObs.Text = serv.Obs;
                txTotalMateriais.Text = serv.GetTotalMateriais().ToString("N2");
                txTotalServicos.Text = serv.GetTotalServicos().ToString("N2");
                txAdiantamento.Text = serv.GetValorAdiantado().ToString("N2");
                txFalta.Text = serv.GetValorRestantePagar().ToString("N2");

                btExcluir.Visibility = Visibility.Visible;
                btConcluirServico.Visibility = Visibility.Visible;
                lbTitulo.Content = $"Editando serviço, N° {txNumBloco.Text}";
                txNumBloco.IsEnabled = false;
            }
            catch (Exception ex) { }
        }

        private void btIncluirServico_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Valid();
                SalvarServico();

                IncluirItemServico iis = new IncluirItemServico();
                iis.ShowDialog();

                if (iis.Item == null)
                    return;

                iis.Item.ServicoId = int.Parse(txNumBloco.Text);
                ServicoBLL bll = new ServicoBLL();
                bll.AdicionaItem(iis.Item);

                FillForm(iis.Item.ServicoId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atenção",
                       MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btExcluir_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show($"Deseja realmente excluir o serviço e seus itens? \nEsta ação não poderá ser revertida!",
                "Excluir serviço", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (res == MessageBoxResult.No)
                return;

            ServicoBLL bll = new ServicoBLL();
            bll.Remove(int.Parse(txNumBloco.Text));
            Close();
        }

        private void btRemoverServico_Click(object sender, RoutedEventArgs e)
        {
            ItemServico item = (ItemServico)dataGrid.SelectedItem;
            if (item == null)
                return;

            ServicoBLL bll = new ServicoBLL();
            try
            {
                bll.RemoveItem(item.Id);
            }
            catch { }
            FillForm(int.Parse(txNumBloco.Text));
        }

        private void btPagamentoAdiantado_Click(object sender, RoutedEventArgs e)
        {
            Valid();

            var cx = new CaixaBLL().GetCaixaAberto();
            if (cx == null)
            {
                MessageBox.Show("O caixa deve estar aberto antes de realizar um adiantamento de serviço",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            ServicoBLL bll = new ServicoBLL();
            decimal total = bll.Find(int.Parse(txNumBloco.Text)).GetTotalGeral();
            PagamentoAdiantado pa = new PagamentoAdiantado(total,
                int.Parse(txNumBloco.Text), false);
            pa.ShowDialog();
        }

        private void btIncluirMaterial_Click(object sender, RoutedEventArgs e)
        {
            IncluirMaterialServico ims = new IncluirMaterialServico();
            ims.ShowDialog();

            if (ims.Cancelado)
                return;

            try
            {
                Valid();
                SalvarServico();

                ims.Material.ServicoId = int.Parse(txNumBloco.Text);

                ServicoBLL bll = new ServicoBLL();
                bll.AdicionaMaterial(ims.Material);

                FillForm(ims.Material.ServicoId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Atenção",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btRemoverMaterial_Click(object sender, RoutedEventArgs e)
        {
            MaterialServico mat = (MaterialServico)datagridMateriais.SelectedItem;
            if (mat == null)
                return;

            ServicoBLL bll = new ServicoBLL();
            bll.RemoveMaterial(mat.Id);

            FillForm(mat.ServicoId);
        }

        private void btConcluirServico_Click(object sender, RoutedEventArgs e)
        {
            Valid();

            var cx = new CaixaBLL().GetCaixaAberto();
            if (cx == null)
            {
                MessageBox.Show("O caixa deve estar aberto antes de realizar o pagamento do serviço",
                    "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ServicoBLL bll = new ServicoBLL();
            decimal total = bll.Find(int.Parse(txNumBloco.Text)).GetTotalGeral();
            PagamentoAdiantado pa = new PagamentoAdiantado(total,
                int.Parse(txNumBloco.Text), true);
            pa.ShowDialog();

            if (pa.Confirmado)
                Close();
        }
    }
}
