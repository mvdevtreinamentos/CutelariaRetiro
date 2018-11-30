using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CutelariaRetiro
{
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;

        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
                p = replacement;

            return base.VisitParameter(p);
        }
    }

    public static class Extensions
    {
        public static object CopyFrom(this object target, object source)
        {
            foreach (PropertyInfo pInfo in source.GetType().GetProperties())
                target.GetType().GetProperty(pInfo.Name)
                    .SetValue(target, pInfo.GetValue(source, null), null);

            return target;
        }

        public static string CompactarString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            MemoryStream ms = new MemoryStream();
            using (GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true))
            {
                zip.Write(buffer, 0, buffer.Length);
            }

            ms.Position = 0;
            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            System.Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            System.Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return Convert.ToBase64String(gzBuffer);
        }

        public static string RemoverCaracteres(this string text)
        {
            return (text
                .Trim()
                .TrimStart()
                .TrimEnd()
                .Replace(".", string.Empty)
                .Replace("-", string.Empty)
                .Replace(",", string.Empty)
                .Replace("/", string.Empty)
                .Replace(@"\", string.Empty)
                .Replace("_", string.Empty)
                .Replace("ç", "c")
                .Replace("Ç", "C")
                .Replace("ã", "a")
                .Replace("Ã", "A")
                .Replace("õ", "o")
                .Replace("Õ", "O")
                .Replace("ó", "o")
                .Replace("Ó", "O")
                .Replace("é", "e")
                .Replace("É", "E")
                .Replace("í", "i")
                .Replace("Í", "I")
                .Replace("á", "A")
                .Replace("è", "e")
                .Replace("à", "a")
                .Replace("À", "A")
                .Replace("Â", "A")
                .Replace("â", "a")
                .Replace("(", string.Empty)
                .Replace(")", string.Empty));
        }

        public static void MudarFocoComEnter(this KeyEventArgs e)
        {
            var elemento = e.OriginalSource as System.Windows.UIElement;
            if (elemento == null)
                return;
            if (e.Key == Key.Enter)
                elemento.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public static string DescompactarString(string text)
        {
            byte[] gzBuffer = Convert.FromBase64String(text);
            using (MemoryStream ms = new MemoryStream())
            {
                int msgLength = BitConverter.ToInt32(gzBuffer, 0);
                ms.Write(gzBuffer, 4, gzBuffer.Length - 4);

                byte[] buffer = new byte[msgLength];

                ms.Position = 0;
                using (GZipStream zip = new GZipStream(ms, CompressionMode.Decompress))
                {
                    zip.Read(buffer, 0, buffer.Length);
                }
                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static void ToMoney(this TextBox txInput, int casas = 2)
        {
            txInput.Tag = casas;
            txInput.TextWrapping = System.Windows.TextWrapping.NoWrap;
            txInput.AcceptsReturn = false;
            txInput.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
            txInput.PreviewTextInput += TxInput_PreviewTextInput;
            txInput.GotFocus += TxInput_GotFocus;
            txInput.LostFocus += TxInput_LostFocus1;
            txInput.PreviewKeyDown += TxInput_PreviewKeyDown;
            txInput.TextChanged += TxInput_TextChanged;
            txInput.PreviewMouseDown += TxInput_PreviewMouseDown;

            if (string.IsNullOrEmpty(txInput.Text))
                txInput.Text = 0.ToString($"N{casas}");
        }

        private static void TxInput_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var t = (sender as TextBox);
                (sender as TextBox).CaretIndex = t.Text.Length;
                (sender as TextBox).SelectAll();
            }
            catch (Exception ex) { }
        }

        private static void TxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textBox = (sender as TextBox);
                int casas = (int)textBox.Tag;

                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = 0.ToString($"N{casas}");
                    textBox.SelectAll();
                }

                if (string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Text = 0.ToString($"N{casas}");
                    textBox.SelectAll();
                }
            }
            catch { }
        }

        private static void TxInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox textBox = (sender as TextBox);
                int casas = (int)textBox.Tag;

                if (string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = 0.ToString($"N{casas}");

                if (string.IsNullOrEmpty(textBox.Text))
                    textBox.Text = 0.ToString($"N{casas}");
            }
            catch { }
        }

        private static void TxInput_LostFocus1(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {

                TextBox textBox = (sender as TextBox);
                int casas = (int)textBox.Tag;

                if (string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = 0.ToString($"N{casas}");

                if (string.IsNullOrEmpty(textBox.Text))
                    textBox.Text = 0.ToString($"N{casas}");
            }
            catch { }
        }

        public static void ToNumeric(this TextBox txInput, bool acceptTrace = false)
        {
            if (acceptTrace)
                txInput.PreviewTextInput += TxInput_PreviewTextInput2;
            else
                txInput.PreviewTextInput += TxInput_PreviewTextInput1;

            if (string.IsNullOrEmpty(txInput.Text))
                txInput.Text = "0";

            txInput.LostFocus += TxInput_LostFocus2;
            txInput.GotFocus += TxInput_GotFocus;
            txInput.TextChanged += TxInput_TextChanged1;
        }

        private static void TxInput_TextChanged1(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (sender as TextBox);
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0";
                textBox.SelectAll();
            }

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
                textBox.SelectAll();
            }
        }

        private static void TxInput_LostFocus2(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox tx = (sender as TextBox);
            if (string.IsNullOrWhiteSpace(tx.Text))
                tx.Text = "0";
            if (string.IsNullOrEmpty(tx.Text))
                tx.Text = "0";
        }

        private static void TxInput_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            TextBox tx = (TextBox)sender;
            tx.SelectAll();
        }

        private static void TxInput_PreviewTextInput2(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (e.Text.Last() == '-')
                    return;

                Regex rgxNumbers = new Regex("[^0-9]+");
                e.Handled = (rgxNumbers.IsMatch(e.Text));
            }
            catch { }
        }

        private static void TxInput_PreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            Regex rgxNumbers = new Regex("[^0-9]+");
            e.Handled = (rgxNumbers.IsMatch(e.Text));
        }

        private static void TxInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                TextBox txInput = (sender as TextBox);
                txInput.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                txInput.LostFocus += TxInput_LostFocus;

                Regex rgxNumbers = new Regex("[^0-9]+");
                e.Handled = (rgxNumbers.IsMatch(e.Text) || (e.Text.Equals(",")));
                if (e.Text.Equals(",") || e.Text.Equals("."))
                {
                    if (e.Text.Equals(",") && txInput.Text.Contains(","))
                        return;

                    if (e.Text.Equals(".") && txInput.Text.Contains(","))
                        return;

                    if (e.Text.Equals(",") && txInput.Text.Contains(","))
                        return;

                    if (e.Text.Equals(".") && (txInput.Text.Last().Equals('.') || txInput.Text.Last().Equals(',')))
                        return;

                    if (e.Text.Equals(",") && (txInput.Text.Last().Equals('.') || txInput.Text.Last().Equals(',')))
                        return;

                    txInput.Text += e.Text;
                    txInput.SelectionStart = txInput.Text.Length; // add some logic if length is 0
                    txInput.SelectionLength = 0;

                }
                if (e.Text.Equals("-"))
                {
                    if (txInput.Text.Contains("-"))
                        return;

                    txInput.Text = "-";
                    txInput.SelectionStart = txInput.Text.Length; // add some logic if length is 0
                    txInput.SelectionLength = 0;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private static void TxInput_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                TextBox txInput = (sender as TextBox);
                int casas = (int)txInput.Tag;

                if (txInput.Name.Equals("txValorUnit"))
                {

                }

                decimal content = decimal.Parse(txInput.Text);
                txInput.Text = content.ToString($"N{casas}");
            }
            catch { }
        }

        private static string[] IBGE { get; set; }
        static string[] indicadores = "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z 1 2 3 4 5 6 7 8 9 0 ! @ # $ % ¨& * ( ) - +  / =".Split(' ');
        static List<string[]> paginas = new List<string[]>();
        static List<KeyValuePair<string, int>> indice = new List<KeyValuePair<string, int>>();

        /*Chamar este 1 vez no inicio do programa*/
        public static void IndexarIBGE()
        {
            if (IBGE == null)
                IBGE = File.ReadAllLines(@".\IBGE.txt");

            string[] dados = IBGE;

            for (int i = 0; i < indicadores.Length; i++)
            {
                string letraInicial = indicadores[i];
                if (string.IsNullOrEmpty(letraInicial))
                    continue;

                string[] palavras = dados.Where(d => d.ToLower().StartsWith(letraInicial.ToLower())).Distinct().ToArray();
                if (palavras.Length == 0)
                    continue;

                paginas.Add(palavras);
                int indx = indice.Count;
                indice.Add(new KeyValuePair<string, int>(letraInicial, indx));
            }
        }

        private static string Buscar(string municipio, string uf)
        {
            try
            {
                List<string> result = new List<string>();

                char letra = municipio.FirstOrDefault();
                int indicePagina = indice.FirstOrDefault(e =>
                    e.Key.ToLower().Equals(letra.ToString().ToLower())).Value;

                string[] paginaDados = paginas[indicePagina];
                paginaDados = paginaDados.Where(e => e.ToUpper().StartsWith(municipio.ToUpper())).ToArray();
                foreach (string palavra in paginaDados)
                {
                    var p = palavra.ToLower();
                    var m = municipio.ToLower();

                    if (ForcarComparacao(p, m))
                        result.Add(palavra);
                }

                var mun = result.FirstOrDefault(e => e.Contains(uf));
                return (string.IsNullOrEmpty(mun)
                    ? string.Empty
                    : mun.Split('\t')[2]);
            }
            catch { return ""; }
        }

        private static bool ForcarComparacao(string munTxt, string munUsr)
        {
            int total = munUsr.Length;
            decimal expected = ((decimal)munUsr.Length / 100 * 85);
            decimal accerted = 0;

            for (int i = 0; i < munUsr.Length; i++)
            {
                if (munTxt.Length <= i)
                    break;

                if (munTxt[i] == munUsr[i])
                    accerted++;
            }

            bool result = (accerted >= expected);
            return result;
        }

        public static string BuscaIBGE(string municipio, string uf)
        {
            return Buscar(municipio, uf);
        }

        public static void AplicarPadroes(this DataGrid dt, bool isRead = true)
        {
            dt.AutoGenerateColumns = false;
            dt.IsReadOnly = isRead;
            dt.RowBackground = (Brush)new BrushConverter().ConvertFrom("#FFFDFDFD");
            dt.CanUserDeleteRows = !isRead;
            dt.CanUserAddRows = !isRead;
            dt.CanUserResizeRows = false;
            dt.SelectionMode = DataGridSelectionMode.Single;
            dt.SelectionUnit = DataGridSelectionUnit.FullRow;
            dt.FontSize = 14;
            dt.MinRowHeight = 22;
            dt.Cursor = Cursors.Hand;
            dt.HorizontalGridLinesBrush = Brushes.LightGray;
            dt.VerticalGridLinesBrush = Brushes.LightGray;

            dt.KeyDown += Dt_KeyDown;
            dt.PreviewKeyDown += Dt_PreviewKeyDown;

        }
        public static void AplicarPadroesFinanceiro(this DataGrid dt,
             bool isRead = true, bool createEvent = true)
        {
            dt.AplicarPadroes(isRead);
            dt.FontSize = 15;
            dt.MinRowHeight = 20;
            dt.AlternatingRowBackground = Brushes.Lavender;

            if (createEvent)
            {
                dt.KeyDown += Dt_KeyDown;
                dt.PreviewKeyDown += Dt_PreviewKeyDown;
            }
        }

        private static void Dt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter ||
                Keyboard.Modifiers == ModifierKeys.Control ||
                Keyboard.Modifiers == ModifierKeys.Alt ||
                Keyboard.Modifiers == ModifierKeys.Shift ||
                e.Key == Key.Left ||
                e.Key == Key.Right)
                e.Handled = true;
        }

        private static void Dt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                DataGrid dataGrid = (sender as DataGrid);

                if (dataGrid.Items.Count == 0)
                    return;



                if (e.Key == Key.Insert)
                {
                    if ((dataGrid.SelectedIndex - 1) < 0)
                        return;

                    dataGrid.SelectedItem = dataGrid.SelectedItems[dataGrid.SelectedIndex + 1];
                }

                if (e.Key == Key.System)
                {
                    if ((dataGrid.SelectedIndex + 1) > (dataGrid.Items.Count - 1))
                        return;

                    dataGrid.Focus();
                    dataGrid.SelectedIndex += 1;
                }

                if (e.Key == Key.Enter)
                {
                    if ((dataGrid.SelectedIndex - 1) < 0)
                        return;
                    dataGrid.SelectedIndex -= 1;
                }

                if (e.Key == Key.Down)
                {
                    if ((dataGrid.SelectedIndex + 1) > (dataGrid.Items.Count - 1))
                        return;

                    dataGrid.SelectedIndex += 1;
                }
                else
                {
                    if ((dataGrid.SelectedIndex - 1) < 0)
                        return;
                    dataGrid.SelectedIndex -= 1;
                }
            }
            catch { }
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }
}
