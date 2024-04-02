using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiMVVM.Models
{
    [SQLite.Table("productosDB")]
    public class Productos
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(255), NotNull]
        public string Nombre { get; set; }
        [NotNull]
        public double Precio { get; set; }
        public string Foto { get; set; }
    }
}
