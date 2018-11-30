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
    /// Lógica interna para SelecionarProdutosPreEntrada.xaml
    /// </summary>
    public partial class SelecionarProdutosPreEntrada : Window
    {
        List<Material> MaterialEntrada { get; set; }

        public SelecionarProdutosPreEntrada()
        {
            InitializeComponent();
            MaterialEntrada = new List<Material>();
            listMateriaisEntradas.ItemsSource = MaterialEntrada;
            listMateriaisEntradas.DisplayMemberPath = "Descricao";
            listMateriais.DisplayMemberPath = "Descricao";
            BuscarNoestoque();
        }

        private void BuscarNoestoque()
        {
            List<Material> list = new MaterialBLL().Search(txPesquisa.Text);
            listMateriais.ItemsSource = list;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Material mat = (Material)listMateriais.SelectedItem;
            if (mat == null)
                return;

            if (MaterialEntrada.Count(m => m.Id == mat.Id) > 0)
                return;

            MaterialEntrada.Add(mat);
            listMateriaisEntradas.Items.Refresh();
        }

        private void txPesquisa_TextChanged(object sender, TextChangedEventArgs e)
        {
            BuscarNoestoque();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            List<Material> list = new MaterialBLL().Search(txPesquisa.Text);
            list.ForEach(mat =>
            {
                if (MaterialEntrada.Count(m => m.Id == mat.Id) == 0)
                    MaterialEntrada.Add(mat);
            });

            listMateriaisEntradas.Items.Refresh();
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            Material mat = (Material)listMateriaisEntradas.SelectedItem;
            if (mat == null)
                return;

            MaterialEntrada.Remove(mat);
            listMateriaisEntradas.Items.Refresh();
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            MaterialEntrada.Clear();
            listMateriaisEntradas.Items.Refresh();
        }

        private void btExportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                path += $@"\ENTRADA DE MERCADORIAS - DIA {DateTime.Now.ToString("dd-MM-yyyy")}.csv";

                if (File.Exists(path))
                    File.Delete(path);

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("Código;Ref.;Material;Preço venda;Estoque atual;Média de Venda;Ultima entrada;Ultima saída;Quantidade de entrada;");
                foreach (var mat in MaterialEntrada)
                {
                    string conteudoLinha = "";
                    conteudoLinha += $"{mat.Id};";
                    conteudoLinha += $"{mat.Referencia};";
                    conteudoLinha += $"{mat.Descricao};";
                    conteudoLinha += $"R$ {mat.Preco.ToString("N2")};";
                    conteudoLinha += $"{mat.Estoque};";

                    decimal mediaVenda = mat.MediaDiariaMaterial(DateTime.Now.AddMonths(-3), DateTime.Now);
                    conteudoLinha += $"{mediaVenda};";

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

                    conteudoLinha += ";";

                    sw.WriteLine(conteudoLinha);
                }

                sw.Close();
                System.Diagnostics.Process.Start(path);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
