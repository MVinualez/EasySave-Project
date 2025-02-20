using System.Diagnostics;
using EasySave___WinUI.Models;
using System.Text;

namespace EasySave___WinUI.CryptoSoft;

/// <summary>
/// File manager class
/// This class is used to encrypt and decrypt files and directories recursively.
/// </summary>
public class FileManager
{
    private string PathToProcess { get; }
    private List<string> AllowedExtensions { get; }
    private string Key { get; }
    /// <summary>
    /// Ask the user to enter an encryption key
    /// </summary>
    /// 

    public FileManager(string path, List<string> allowedExtensions, string key) {
        PathToProcess = path;
        AllowedExtensions = allowedExtensions;
        Key = key;
    }
 
    private static string GetEncryptionKey()
    {
        string key; 
        do
        {
            Console.Write("🔑 Entrez la clé de chiffrement : ");
            key = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(key))
                Console.WriteLine("⚠️ La clé de chiffrement ne peut pas être vide !");

        } while (string.IsNullOrWhiteSpace(key));

        return key;
    }

    /// <summary>
    /// Encrypt a file or all files in a directory recursively
    /// </summary>
    public void Transform()
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
        if (!AllowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
        {
            Console.WriteLine($"⛔ Le fichier {filePath} n'a pas une extension autorisée ({fileExtension}), chiffrement annulé.");
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
            result[i] = (byte)(fileBytes[i] ^ keyBytes[i % keyBytes.Count]);
        }
        return result;
    }
}
