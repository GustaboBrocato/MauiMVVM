using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiMVVM.Controllers;
using MauiMVVM.Utilities;

namespace MauiMVVM.ViewModels
{
    public class ProductosViewModels : BaseViewModel
    {
        private ProductosController productosController = new ProductosController();
        private int productId;
        private string _nombre;
        private double _precio;
        private string _foto;
        private int _id;
        private bool _visibilityCreate;
        private bool _visibilityUpdate;
        private Models.Productos _selectedProduct;

        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }

        public string Nombre
        {
            get { return _nombre; }
            set { _nombre = value; OnPropertyChanged(); }
        }

        public double Precio
        {
            get { return _precio; }
            set { _precio = value; OnPropertyChanged(); }
        }

        public string Foto
        {
            get { return _foto; }
            set { _foto = value; OnPropertyChanged(); }
        }

        public bool VisibilityCreate
        {
            get { return _visibilityCreate; }
            set { _visibilityCreate = value; OnPropertyChanged(); }
        }

        public bool VisibilityUpdate
        {
            get { return _visibilityUpdate; }
            set { _visibilityUpdate = value; OnPropertyChanged(); }
        }

        public Models.Productos SelectedProduct
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();

                // Display alert based on whether SelectedProduct is null or not
                ShowProductStatusAlert();
            }
        }

        public ProductosViewModels() 
        {
            CleanCommand = new Command(Cleaner);
            FotoCommand = new Command(() => TomarFoto());
            if(productId == 0) { }
            CreateCommand = new Command(async () => await CreateData());
            UpdateCommand = new Command(async () => await UpdateProducto(productId));

        }

        public ICommand CleanCommand {  get; private set; }
        public ICommand CreateCommand {  get; private set; }
        public ICommand ReadCommand {  get; private set; }
        public ICommand UpdateCommand {  get; private set; }
        public ICommand DeleteCommand {  get; private set; }
        public ICommand FotoCommand { get; private set; }


        void Cleaner()
        {
            Nombre = string.Empty;
            Precio = 0;
            Foto   = string.Empty;
        }

        void ShowProductStatusAlert()
        {
            if (SelectedProduct != null)
            {
                productId = SelectedProduct.Id;
                VisibilityCreate = false;
                VisibilityUpdate = true;
            }
            else
            {
                VisibilityCreate = true;
                VisibilityUpdate = false;
            }
        }


        // CRUD
        async Task CreateData()
        {
            if(Nombre == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Por favor ingrese una descripcion para el producto", "OK");
                return;
            }
            else if (Precio == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Por favor ingrese un precio para el producto", "OK");
                return;
            }

            try 
            {
                var product = new Models.Productos
                {
                    Nombre = Nombre,
                    Precio = Precio,
                    Foto = Foto
                };

                if (productosController != null)
                {
                    bool addedSuccessfully = await productosController.storeProducto(product);

                    if (addedSuccessfully)
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "Producto Creado", "OK");
                        var navigation = Application.Current.MainPage.Navigation;
                        await navigation.PushAsync(new Views.PageListProductos());
                    }
                }

            }
            catch 
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "No se pudo crear el producto", "OK");
            }
        }

        async Task UpdateProducto(int id)
        {
            if (Nombre == null)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Por favor ingrese una descripcion para el producto", "OK");
                return;
            }
            else if (Precio == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "Por favor ingrese un precio para el producto", "OK");
                return;
            }

            try
            {
                var product = new Models.Productos
                {
                    Id = id,
                    Nombre = Nombre,
                    Precio = Precio,
                    Foto = Foto
                };

                if (productosController != null)
                {
                    bool addedSuccessfully = await productosController.storeProducto(product);

                    if (addedSuccessfully)
                    {
                        await Application.Current.MainPage.DisplayAlert("Atención", "Producto Actualizado", "OK");
                        var navigation = Application.Current.MainPage.Navigation;
                        await navigation.PushAsync(new Views.PageListProductos());
                    }
                }

            }
            catch
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "No se pudo actualizar el producto", "OK");
            }
        }

        //Tomar Foto
        async void TomarFoto()
        {
            FileResult photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                string photoPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
                using (Stream sourcephoto = await photo.OpenReadAsync())
                using (FileStream streamlocal = File.OpenWrite(photoPath))
                {
                    await sourcephoto.CopyToAsync(streamlocal);

                    Foto = photoHelper.GetImg64(photo);
                }
            }
        }

    }
}
