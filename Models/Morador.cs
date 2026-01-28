using System;

namespace wpf_exemplo.Models
{
    public class Morador
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Bloco { get; set; }
        public string Apartamento { get; set; }
        public string Telefone { get; set; }

        public int? FingerprintId1 { get; set; }
        public int? FingerprintId2 { get; set; }

        public bool Ativo { get; set; }
    }
}
