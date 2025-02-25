using System.Diagnostics;
using EasySave___WinUI.Models;
using System.Text;
using EasySave___WinUI.ViewModels;
using static System.Net.Mime.MediaTypeNames;
using EasySave___WinUI.ViewModels;
using Windows.UI.StartScreen;
using System.Diagnostics.Eventing.Reader;

namespace EasySave___WinUI.Services;

/// <summary>
/// File manager class
/// This class is used to encrypt and decrypt files and directories recursively.
/// </summary>
public class EncryptionService
{
    private string PathToProcess { get; }
    private List<string> AllowedExtensions { get; }
    private string Key { get; }

    private static EncryptionService? _instance;


    private EncryptionService(string path, List<string> allowedExtensions, string key)
    {
        PathToProcess = path;
        AllowedExtensions = allowedExtensions;
        Key = key;
    }


    public static EncryptionService GetEncryptionServiceInstance(string path, List<string> allowedExtensions, string key)
    {
        _instance ??= new EncryptionService(path, allowedExtensions, key);


        return _instance;
    }


    public void Transform(string PathToProcess, List<string> AllowedExtensions, string Key)
    {
        if (File.Exists(PathToProcess))
        {
            TransformFile(PathToProcess);  // Si c'est un fichier, on le chiffre directement
        }
        else if (Directory.Exists(PathToProcess))
        {
            Console.WriteLine($"📂 Début du chiffrement du dossier {PathToProcess} et de ses sous-dossiers...");
            foreach (var file in Directory.GetFiles(PathToProcess, "*.*", SearchOption.AllDirectories))
            {
                TransformFile(file);  // Récursivement pour chaque fichier
            }
            Console.WriteLine($"✅ Chiffrement du dossier {PathToProcess} terminé !");
        }
        else
        {
            Console.WriteLine($"🔴 Le chemin {PathToProcess} n'existe pas !");
        }
    }

    /// <summary>
    /// Encrypt a single file if it matches the allowed extensions
    /// </summary>
    private void TransformFile(string filePath)
    {
        string fileExtension = Path.GetExtension(filePath);
        long fileSize = new FileInfo(filePath).Length;
        if (!AllowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"⛔ Le fichier {filePath} n'a pas une extension autorisée ({fileExtension}), chiffrement annulé.");
            return;
        }

        if(fileSize >= 2000000000) {
            return; 
        }

        Console.WriteLine($"🔐 Début du chiffrement de {filePath}...");
        Stopwatch stopwatch = Stopwatch.StartNew();
        var fileBytes = File.ReadAllBytes(filePath);
        Console.WriteLine($"📏 Taille avant chiffrement : {fileBytes.Length} octets");
        var keyBytes = ConvertToByte(Key);
        fileBytes = XorMethod(fileBytes, keyBytes);
        File.WriteAllBytes(filePath, fileBytes);
        stopwatch.Stop();
        Console.WriteLine($"✅ Chiffrement terminé en {stopwatch.ElapsedMilliseconds} ms !");
    }

    /// <summary>
    /// Convert a string into a byte array
    /// </summary>
    private static byte[] ConvertToByte(string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    /// <summary>
    /// XOR encryption method
    /// </summary>
    private static byte[] XorMethod(IReadOnlyList<byte> fileBytes, IReadOnlyList<byte> keyBytes)
    {
        var result = new byte[fileBytes.Count];
        for (var i = 0; i < fileBytes.Count; i++)
        {
            if(keyBytes.Count != 0) {
                result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Count]);
            } else {
                result[i] = fileBytes[i];
            }
        }
        return result;
    }
}
