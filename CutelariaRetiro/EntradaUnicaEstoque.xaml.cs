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
    /// Lógica interna para EntradaUnicaEstoque.xaml
    /// </summary>
    public partial class EntradaUnicaEstoque : Window
    {
        private Material Material { get; set; }
        public EntradaUnicaEstoque(Material mat)
        {
            InitializeComponent();

            Material = mat;
            lbNomeMat.Content = mat.Descricao;
            var ultimaEntrada = mat.EntradaMaterial.LastOrDefault();
            if (ultimaEntrada != null)
                lbUltimaEntrada.Content = ultimaEntrada.Data.ToString("dd/MM/yyyy");

            lbEstoqueAtual.Content = $"{mat.Estoque} UN";
            txData.SelectedDate = DateTime.Now;
            txLote.Text = $"EN{DateTime.Now.ToString("ddMMyyyy")}-{DateTime.Now.ToString("HHmm")}";
            txQuant.Text = "1";
            txQuant.ToNumeric();
        }

        private void SalvaArquivo()
        {
            StreamWriter sw = new StreamWriter($@".\Entradas\{txLote.Text}.csv");
            sw.WriteLine("Código;Ref.;Material;Preço venda;Estoque atual;Média de Venda;Ultima entrada;Ultima saída;Quantidade de entrada;");

            var mat = new MaterialBLL().Find(Material.Id);
            string linha = "";
            linha += $"{mat.Id};";
            linha += $"{mat.Referencia};";
            linha += $"R$ {mat.Preco.ToString("N2")};";
            linha += $"{mat.Estoque};";
            linha += $"{mat.MediaDiariaMaterial(DateTime.Now.AddMonths(-3), DateTime.Now)};";
            decimal mediaVenda = mat.MediaDiariaMaterial(DateTime.Now.AddMonths(-3), DateTime.Now);
            linha += $"{mediaVenda};";

            var ultimaEntrada = mat.EntradaMaterial.LastOrDefault();
            if (ultimaEntrada != null)
                linha += $"{ultimaEntrada.Data.ToString("dd/MM/yyyy")};";
            else
                linha += $";";

            var ultimaSaida = mat.MaterialServico.LastOrDefault();
            if (ultimaSaida != null)
                linha += $"{ultimaSaida.Servico.Data.ToString("dd/MM/yyyy")};";
            else
                linha += ";";

            linha += $"{txQuant.Text};";

            sw.WriteLine(linha);
            sw.Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(@".\Entradas\"))
                Directory.CreateDirectory(@".\Entradas\");

            MaterialBLL bll = new MaterialBLL();
            var db = new CutelariaRetiroEntities();
            try
            {
                if (db.EntradaMaterial.Count(d => d.Lote.Equals(txLote.Text)) > 0)
                {
                    MessageBox.Show("Já existe uma entrada com o lote " + txLote.Text, "Atenção", MessageBoxButton.OK,
                         MessageBoxImage.Exclamation);
                    return;
                }

                EntradaMaterial entrada = new EntradaMaterial();
                entrada.Data = txData.SelectedDate ?? DateTime.Now;
                entrada.MaterialId = Material.Id;
                entrada.Lote = txLote.Text;
                entrada.Obs = txObs.Text;
                entrada.Quantidade = int.Parse(txQuant.Text);

                bll.EntradaMaterial(entrada);
                SalvaArquivo();

                MessageBox.Show("Entrada de materiais realizada com sucesso",
                    "Mensagem", MessageBoxButton.OK, MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                bll.CancelaEntrada(txLote.Text);
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
