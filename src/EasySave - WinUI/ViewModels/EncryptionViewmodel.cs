using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using EasySave___WinUI.Services;
using Microsoft.UI.Xaml.Media;

namespace EasySave___WinUI.ViewModels
{
    public class EncryptionViewModel
    {

        private  EncryptionService _encryptionService;

        private static EncryptionViewModel? _instance;



        private EncryptionViewModel()
        {
        }
        
        public static EncryptionViewModel GetEncryptionViewModelInstance()
        {
            _instance ??= new EncryptionViewModel(); // if _instance is null, create a new instance Singleton pattern


            return _instance; }


        public async Task EncryptFile(string path, List<string> allowedExtensions, string key)
        {
            await Task.Run(() => {
                _encryptionService = EncryptionService.GetEncryptionServiceInstance(path, allowedExtensions, key);

                _encryptionService.Transform(path, allowedExtensions ,key);
            });
            


        }
    }
}
