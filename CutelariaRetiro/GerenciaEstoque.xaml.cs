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
using System.Windows.Forms;
using System.IO;

namespace CutelariaRetiro
{
    /// <summary>
    /// Lógica interna para GerenciaEstoque.xaml
    /// </summary>
    public partial class GerenciaEstoque : Window
    {
        public GerenciaEstoque()
        {
            InitializeComponent();
            FillMateriais();

            txDataInicio.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 01);
            txDataFim.SelectedDate = DateTime.Now;
            gridDetalhes.Visibility = Visibility.Hidden;
        }

        private void FillMateriais()
        {
            MaterialBLL bll = new MaterialBLL();
            var list = bll.Search(txPesquisa.Text);
            listMateriais.DisplayMemberPath = "Descricao";
            list.ForEach(m => m.Descricao = $"{m.Descricao}, {m.Estoque} Un");
            listMateriais.ItemsSource = list;
        }

        private void txPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillMateriais();
            gridDetalhes.Visibility = Visibility.Hidden;
        }

        private void listMateriais_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillMovimentacoes();
            FillDetalhes();
        }

        private void FillDetalhes()
        {
            Material mat = (Material)listMateriais.SelectedItem;
            if (mat == null)
                return;
            
            MaterialBLL bll = new MaterialBLL();
            mat = bll.Find(mat.Id);
            var movimentosEstoque = bll.GetMovimentosEstoque(mat.Id, txDataInicio.SelectedDate ?? DateTime.Now,
                txDataFim.SelectedDate ?? DateTime.Now);

            gridDetalhes.Visibility = Visibility.Visible;
            lbNomeMaterial.Content = mat.Descricao;
            lbTotalEntradas.Content = movimentosEstoque.Count(m => m.Tipo.Equals("Entrada")).ToString();
            lbTotalQuantEntradas.Content = $"{ movimentosEstoque.Where(m => m.Tipo.Equals("Entrada")).Select(m => m.Quant).Sum().ToString()} UN";
            lbTotalSaidas.Content = movimentosEstoque.Count(m => m.Tipo.Equals("Saída")).ToString();
            lbTotalQuantSaidas.Content = $"{ movimentosEstoque.Where(m => m.Tipo.Equals("Saída")).Select(m => m.Quant).Sum().ToString()} UN";
            lbEstoqueAtual.Content = mat.Estoque.ToString();

            int mediaVendaPeriodo = mat.MediaDiariaMaterial(txDataInicio.SelectedDate ?? DateTime.Now,
                txDataFim.SelectedDate ?? DateTime.Now);

            lbMediaVendaDiaria.Content = $"{mediaVendaPeriodo} UN";

            int diasRestantesEstoque = 0;

            try
            {
                diasRestantesEstoque = (mat.Estoque / mediaVendaPeriodo);
            }
            catch { }

            if (mediaVendaPeriodo == 0)
                bannerDiasRestantes.Visibility = Visibility.Hidden;
            else
                bannerDiasRestantes.Visibility = Visibility.Visible;

            lbDiasRestantes.Content = $"Restam {diasRestantesEstoque} dias de estoque para este material";
            if (diasRestantesEstoque <= 3)
            {
                bannerDiasRestantes.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFE0E0");
                bannerDiasRestantes.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FFFF7373");
                lbDiasRestantes.Foreground = (Brush)new BrushConverter().ConvertFrom("Red");
            }
            else
            {
                bannerDiasRestantes.Background = (Brush)new BrushConverter().ConvertFrom("#FFCAE7FF");
                bannerDiasRestantes.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF006C67");
                lbDiasRestantes.Foreground = (Brush)new BrushConverter().ConvertFrom("Black");
            }

        }

        private void FillMovimentacoes()
        {
            Material mat = (Material)listMateriais.SelectedItem;
            if (mat == null)
                return;

            MaterialBLL bll = new MaterialBLL();
            var movimentosEstoque = bll.GetMovimentosEstoque(mat.Id, txDataInicio.SelectedDate ?? DateTime.Now,
                txDataFim.SelectedDate ?? DateTime.Now);

            if (rdoEntradas.IsChecked.Value)
                movimentosEstoque = movimentosEstoque.Where(m => m.Tipo.Equals("Entrada")).ToList();
            if(rdoSaidas.IsChecked.Value)
                movimentosEstoque = movimentosEstoque.Where(m => m.Tipo.Equals("Saída")).ToList();

            spMovimentos.Children.Clear();
            movimentosEstoque.ForEach(m => spMovimentos.Children.Add(new CardMovimentoEstoque(m)));
        }

        private void btFiltrar_Click(object sender, RoutedEventArgs e)
        {
            gridFiltro.Visibility = Visibility.Hidden;
            btFiltro.Visibility = Visibility.Visible;

            FillMovimentacoes();
        }

        private void btFiltro_Click(object sender, RoutedEventArgs e)
        {
            gridFiltro.Visibility = Visibility.Visible;
            btFiltro.Visibility = Visibility.Hidden;
        }

        private void btAbrirCadastro_Click(object sender, RoutedEventArgs e)
        {
            Material mat = (Material)listMateriais.SelectedItem;
            if (mat == null)
                return;

            mat = new MaterialBLL().Find(mat.Id);

            CadMateriais cad = new CadMateriais();
            cad.listBox.Visibility = Visibility.Hidden;
            cad.txPesquisa.Visibility = Visibility.Hidden;
            cad.btExcluir.Visibility = Visibility.Hidden;
            cad.btNovo.Visibility = Visibility.Hidden;
            cad.lbPesquisa.Visibility = Visibility.Hidden;
            cad.FillForm(mat);
            cad.ShowDialog();

            mat = new MaterialBLL().Find(mat.Id);
        }

        private void btExportarEstoqueExcel_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowDialog();

            if (string.IsNullOrEmpty(fd.SelectedPath))
                return;

            List<Material> materiais = new MaterialBLL().Search("");

            string path = fd.SelectedPath + $@"\Estoque Cutelaria Retiro - {DateTime.Now.ToString("dd-MM-yyyy")}.csv";
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("Ref.;Material;Preço venda;Estoque atual;Ultima entrada;Ultima saída;Média de venda;Dias restantes de estoque;");
            foreach (var mat in materiais)
            {
                string conteudoLinha = "";
                conteudoLinha += $"{mat.Referencia};";
                conteudoLinha += $"{mat.Descricao};";
                conteudoLinha += $"R$ {mat.Preco.ToString("N2")};";
                conteudoLinha += $"{mat.Estoque};";

                var ultimaEntrada = mat.EntradaMaterial.LastOrDefault();
                if (ultimaEntrada != null)
                    conteudoLinha += $"{ultimaEntrada.Data.ToString("dd/MM/yyyy")};";
                else
                    conteudoLinha += $";";

                var ultimaSaida = mat.MaterialServico.LastOrDefault();
                if (ultimaSaida != null)
                    conteudoLinha += $"{ultimaSaida.Servico.Data.ToString("dd/MM/yyyy")};";
                else
                    conteudoLinha += ";";
                
                int mediaVendaPeriodo = mat.MediaDiariaMaterial(txDataInicio.SelectedDate ?? DateTime.Now,
                    txDataFim.SelectedDate ?? DateTime.Now);

                int diasRestantesEstoque = 0;

                try
                {
                    diasRestantesEstoque = (mat.Estoque / mediaVendaPeriodo);
                }
                catch { }

                conteudoLinha += $"{mediaVendaPeriodo};";
                conteudoLinha += $"{diasRestantesEstoque};";

                sw.WriteLine(conteudoLinha);
            }

            sw.Close();

            System.Diagnostics.Process.Start(path);
        }

        private void btEntradaEstoque_Click(object sender, RoutedEventArgs e)
        {
            if (brdOpcoesEntrada.Visibility == Visibility.Visible)
                brdOpcoesEntrada.Visibility = Visibility.Hidden;
            else
                brdOpcoesEntrada.Visibility = Visibility.Visible;
        }

        private void btExportarPlaniliaEntrada_Click(object sender, RoutedEventArgs e)
        {
            brdOpcoesEntrada.Visibility = Visibility.Hidden;
            SelecionarProdutosPreEntrada sp = new SelecionarProdutosPreEntrada();
            sp.ShowDialog();
        }

        private void btImportarPlaniliaEntrada_Click(object sender, RoutedEventArgs e)
        {
            brdOpcoesEntrada.Visibility = Visibility.Hidden;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Planílias CSV|*.csv";
            dialog.ShowDialog();

            if (string.IsNullOrEmpty(dialog.FileName))
                return;

            EntradaEstoque ee = new EntradaEstoque(dialog.FileName);
            ee.ShowDialog();

            FillMovimentacoes();
            FillDetalhes();
        }

        private void btEntradaUnicaMaterial_Click(object sender, RoutedEventArgs e)
        {
            Material mat = (Material)listMateriais.SelectedItem;
            if (mat == null)
                return;

            EntradaUnicaEstoque eue = new EntradaUnicaEstoque(mat);
            eue.ShowDialog();

            FillMovimentacoes();
            FillDetalhes();
        }

        private void btAlterarEstoque_Click(object sender, RoutedEventArgs e)
        {
            Material mat = (Material)listMateriais.SelectedItem;
            if (mat == null)
                return;

            AlterarEstoque ae = new AlterarEstoque(mat);
            ae.ShowDialog();

            FillMovimentacoes();
            FillDetalhes();
        }
    }
}
