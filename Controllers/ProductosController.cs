using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MauiMVVM.Models;

namespace MauiMVVM.Controllers
{
    public class ProductosController
    {
        SQLiteAsyncConnection? _connection = null;

        public ProductosController() { }

        public async Task Init()
        {
            try
            {
                if (_connection is null)
                {
                    SQLite.SQLiteOpenFlags extensiones = SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.SharedCache;

                    if (string.IsNullOrEmpty(FileSystem.AppDataDirectory))
                    {
                        return;
                    }

                    _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, "ShoppingCartDB"), extensiones);

                    var creacion = await _connection.CreateTableAsync<Productos>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Init(): {ex.Message}");
            }
        }

        //Create
        public async Task<bool> storeProducto(Productos producto)
        {
            await Init();

            if (producto.Id == 0)
            {
                await _connection.InsertAsync(producto);
            }
            else
            {
                await _connection.UpdateAsync(producto);
            }

            // Product added successfully, return true
            return true;
        }

        //Update
        public async Task<int> updateProducto(Productos producto)
        {
            await Init();
            return await _connection.UpdateAsync(producto);
        }

        //Read
        public async Task<List<Productos>> getListProductos()
        {
            await Init();
            return await _connection.Table<Productos>().ToListAsync();
        }

        //Read Element
        public async Task<Productos> getProducto(int pid)
        {
            await Init();
            return await _connection.Table<Productos>().Where(i => i.Id == pid).FirstOrDefaultAsync();
        }

        //Delete
        public async Task<int> deleteProducto(int itemID)
        {
            await Init();
            var itemToDelete = await getProducto(itemID);

            if (itemToDelete != null)
            {
                return await _connection.DeleteAsync(itemToDelete);
            }

            return 0;
        }

        public async Task<int> DeleteAllProductos()
        {
            await Init();
            return await _connection.DeleteAllAsync<Productos>();
        }

    }
}
