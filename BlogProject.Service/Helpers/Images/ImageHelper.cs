using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Security.AccessControl;
using BlogProject.Entity.DTOs.Images;
using BlogProject.Entity.Enums;

namespace BlogProject.Service.Helpers.Images
{
    public class ImageHelper : IImageHelper
    {
        private readonly string wwwroot;
        private readonly IWebHostEnvironment env;
        private const string imgFolder = "images";
        private const string articleImagesFolder = "article-images";
        private const string userImagesFolder = "user-images";


        public ImageHelper(IWebHostEnvironment env)
        {
            this.env = env;
            wwwroot = env.WebRootPath;
        }

        private string ReplaceInvalidChars(string fileName)
        {
            return fileName.Replace("İ", "I")
                 .Replace("ı", "i")
                 .Replace("Ğ", "G")
                 .Replace("ğ", "g")
                 .Replace("Ü", "U")
                 .Replace("ü", "u")
                 .Replace("ş", "s")
                 .Replace("Ş", "S")
                 .Replace("Ö", "O")
                 .Replace("ö", "o")
                 .Replace("Ç", "C")
                 .Replace("ç", "c")
                 .Replace("é", "")
                 .Replace("!", "")
                 .Replace("'", "")
                 .Replace("^", "")
                 .Replace("+", "")
                 .Replace("%", "")
                 .Replace("/", "")
                 .Replace("(", "")
                 .Replace(")", "")
                 .Replace("=", "")
                 .Replace("?", "")
                 .Replace("_", "")
                 .Replace("*", "")
                 .Replace("æ", "")
                 .Replace("ß", "")
                 .Replace("@", "")
                 .Replace("€", "")
                 .Replace("<", "")
                 .Replace(">", "")
                 .Replace("#", "")
                 .Replace("$", "")
                 .Replace("½", "")
                 .Replace("{", "")
                 .Replace("[", "")
                 .Replace("]", "")
                 .Replace("}", "")
                 .Replace(@"\", "")
                 .Replace("|", "")
                 .Replace("~", "")
                 .Replace("¨", "")
                 .Replace(",", "")
                 .Replace(";", "")
                 .Replace("`", "")
                 .Replace(".", "")
                 .Replace(":", "")
                 .Replace(" ", "");
        }

        public async Task<ImageUploadedDto> Upload(string name, IFormFile imageFile, ImageType imageType, string folderName = null)
        {
            try
            {
                folderName ??= imageType == ImageType.User ? userImagesFolder : articleImagesFolder;

                // Tam yolları oluştur
                var directory = Path.Combine(wwwroot, imgFolder, folderName);

                // Klasörün var olduğundan emin ol
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Console.WriteLine($"Klasör oluşturuldu: {directory}");
                }

                // Dosya adını oluştur
                string oldFileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string fileExtension = Path.GetExtension(imageFile.FileName);
                name = ReplaceInvalidChars(name);
                string newFileName = $"{name}_{DateTime.Now.Millisecond}{fileExtension}";

                // Tam dosya yolu
                var path = Path.Combine(directory, newFileName);
                Console.WriteLine($"Resim kaydediliyor: {path}");

                // Dosyayı kaydet
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                    await stream.FlushAsync();
                }

                Console.WriteLine("Resim başarıyla kaydedildi");

                // Kaydedilen dosya gerçekten var mı kontrol et
                if (!File.Exists(path))
                {
                    Console.WriteLine("HATA: Dosya kaydedilmesine rağmen bulunamadı");
                    return null;
                }

                // Başarılı
                return new ImageUploadedDto()
                {
                    FullName = $"{folderName}/{newFileName}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Resim yükleme hatası: {ex.Message}");
                Console.WriteLine($"Hata detayı: {ex.StackTrace}");
                return null;
            }
        }

        public void Delete(string imageName)
        {
            var fileToDelete = Path.Combine($"{wwwroot}/{imgFolder}/{imageName}");
            if (File.Exists(fileToDelete))
                File.Delete(fileToDelete);

        }
    }
}