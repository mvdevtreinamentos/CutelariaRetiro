//------------------------------------------------------------------------------
// <auto-generated>
//    O código foi gerado a partir de um modelo.
//
//    Alterações manuais neste arquivo podem provocar comportamento inesperado no aplicativo.
//    Alterações manuais neste arquivo serão substituídas se o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CutelariaRetiro.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class MaterialServico
    {
        public int Id { get; set; }
        public int ServicoId { get; set; }
        public int MaterialId { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public decimal Total { get; set; }
    
        public virtual Material Material { get; set; }
        public virtual Servico Servico { get; set; }
    }
}
