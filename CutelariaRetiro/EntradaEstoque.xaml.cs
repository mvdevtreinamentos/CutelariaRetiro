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
    /// Lógica interna para EntradaEstoque.xaml
    /// </summary>
    public partial class EntradaEstoque : Window
    {
        string Arquivo { get; set; }
        public EntradaEstoque(string arquivo)
        {
            InitializeComponent();
            dataGrid.AplicarPadroes();
            Arquivo = arquivo;

            txData.SelectedDate = DateTime.Now;
            txLote.Text = $"EN{DateTime.Now.ToString("ddMMyyyy")}-{DateTime.Now.ToString("HHmmss")}";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ImportacaoEstoque> list = new List<ImportacaoEstoque>();
                string[] lines = File.ReadAllLines(Arquivo);
                for (int i = 1; i < lines.Length; i++)
                {
                    string linha = lines[i];
                    string[] colunas = linha.Split(';');

                    try
                    {
                        int cod = 0;
                        if (!int.TryParse(colunas[0], out cod))
                            continue;

                        list.Add(new ImportacaoEstoque()
                        {
                            Codigo = colunas[0],
                            Ref = colunas[1],
                            Material = colunas[2],
                            PrecoVenda = colunas[3],
                            EstoqueAtual = colunas[4],
                            MediaVenda = colunas[5],
                            UltimaEntrada = colunas[6],
                            UltimaSaida = colunas[7],
                            QuantEntrada = colunas[8]
                        });
                    }
                    catch { }
                }

                dataGrid.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                     "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                File.Copy(Arquivo,
                    $@".\Entradas\{txLote.Text}.csv", true);

                List<ImportacaoEstoque> list = (dataGrid.ItemsSource as List<ImportacaoEstoque>);
                foreach (var imp in list)
                {
                    EntradaMaterial entrada = new EntradaMaterial();
                    entrada.Data = txData.SelectedDate ?? DateTime.Now;
                    entrada.MaterialId = int.Parse(imp.Codigo);
                    entrada.Lote = txLote.Text;
                    entrada.Obs = txObs.Text;
                    entrada.Quantidade = int.Parse(imp.QuantEntrada);

                    bll.EntradaMaterial(entrada);
                }

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

    public class ImportacaoEstoque
    {
        public string Codigo { get; set; }
        public string Ref { get; set; }
        public string Material { get; set; }
        public string PrecoVenda { get; set; }
        public string EstoqueAtual { get; set; }
        public string MediaVenda { get; set; }
        public string UltimaEntrada { get; set; }
        public string UltimaSaida { get; set; }
        public string QuantEntrada { get; set; }

    }
}
