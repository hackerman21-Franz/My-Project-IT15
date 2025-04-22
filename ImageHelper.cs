using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

public static class ImageHelper
{
    public static byte[] PreprocessMeterImage(IFormFile image)
    {
        using (var stream = image.OpenReadStream())
        {
            using (var imageObj = Image.Load<Rgba32>(stream))
            {
                imageObj.Mutate(x => x
    .Grayscale()                           // Essential: removes color noise
    .GaussianBlur(0.5f)                    // Light blur to reduce background noise
    .Contrast(1.2f)                        // Enhance digit separation
    .Brightness(1.1f)                      // Mild brightening (not too much)
    .GaussianSharpen(0.5f));               // Subtle sharpening to define edges


                // Optionally, you can resize the image or apply other filters here

                // Save the image to a memory stream
                using (var memoryStream = new MemoryStream())
                {
                    imageObj.SaveAsPng(memoryStream);
                    return memoryStream.ToArray();  // Return the preprocessed image as a byte array
                }
            }
        }
    }
}
