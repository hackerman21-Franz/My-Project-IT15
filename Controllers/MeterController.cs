using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using OpenCvSharp;
using Tesseract;

namespace MyProjectIT15.Controllers
{
    public class MeterController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public MeterController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MeterReading()
        {
            ViewBag.OcrResult = TempData["OcrResult"];
            ViewBag.Error = TempData["Error"];
            return View();
        }

        [HttpPost]
        public IActionResult ScanMeter(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Please upload a valid image.";
                return RedirectToAction("Index");
            }

            string tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            using var src = Cv2.ImRead(tempPath, ImreadModes.Color);
            using var gray = new Mat();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, gray, new OpenCvSharp.Size(3, 3), 0);
            Cv2.Threshold(gray, gray, 0, 255, ThresholdTypes.Binary | ThresholdTypes.Otsu);
            string processedPath = Path.Combine(Path.GetTempPath(), "processed.png");
            Cv2.ImWrite(processedPath, gray);

            string tessDataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
            string extractedText = "";

            using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
            using (var img = Pix.LoadFromFile(processedPath))
            using (var page = engine.Process(img))
            {
                extractedText = page.GetText();
            }

            TempData["OcrResult"] = extractedText;

            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult Upload(IFormFile image)
        {
            string resultText = "";

            if (image != null && image.Length > 0)
            {
                try
                {
                    // Get tessdata path from ContentRoot or WebRoot
                    string tessDataPath = Path.Combine(_env.ContentRootPath, "tessdata");

                    using (var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default))
                    {
                        // engine.SetVariable("tessedit_char_whitelist", "0123456789");
                        // engine.SetVariable("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
                        engine.SetVariable("tessedit_char_whitelist",
                            "0123456789" +                  // Numbers
                            "abcdefghijklmnopqrstuvwxyz" + // Lowercase letters
                            "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + // Uppercase letters
                            " .,!?•\n\r");                  // Punctuation, bullet, and line breaks


                        using (var stream = image.OpenReadStream())
                        using (var pix = Pix.LoadFromMemory(ReadFully(stream)))
                        using (var page = engine.Process(pix))
                        {
                            resultText = page.GetText().Trim();
                        }
                    }

                    // Check if resultText is empty or null
                    if (string.IsNullOrEmpty(resultText))
                    {
                        resultText = "No text detected.";
                    }
                }
                catch (System.Exception ex)
                {
                    resultText = "Error: " + ex.Message;
                }
            }
            else
            {
                resultText = "No file uploaded.";
            }

            // Pass the result to the view
            ViewBag.OcrResult = resultText;
            return View("Index");
        }

        private static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}


//namespace MyProjectIT15.Controllers
//{
//    public class MeterController : Controller
//    {
//        private readonly IWebHostEnvironment _env;

//        public MeterController(IWebHostEnvironment env)
//        {
//            _env = env;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Upload(IFormFile image)
//        {
//            string resultText = "";

//            if (image != null && image.Length > 0)
//            {
//                try
//                {
//                    // Preprocess the image before applying OCR
//                    byte[] processedImage = ImageHelper.PreprocessMeterImage(image); // Your image preprocessing function

//                    string tessDataPath = Path.Combine(_env.ContentRootPath, "tessdata");

//                    // Initialize Tesseract OCR engine
//                    using (var engine = new TesseractEngine(tessDataPath, "lets", EngineMode.Default))
//                    {
//                        // Optional whitelist for numbers and the "kWh" unit
//                        engine.SetVariable("tessedit_char_whitelist", "0123456789");
//                        engine.DefaultPageSegMode = PageSegMode.SingleBlock; // Try SingleBlock or SparseText

//                        using (var pix = Pix.LoadFromMemory(processedImage))
//                        using (var page = engine.Process(pix))
//                        {
//                            resultText = page.GetText().Trim();
//                            Console.WriteLine("OCR Result: " + resultText);
//                        }
//                    }
//                }
//                catch (System.Exception ex)
//                {
//                    resultText = "Error: " + ex.Message;
//                }
//            }
//            else
//            {
//                resultText = "No image Uploaded";
//            }

//            // Set OCR result in ViewBag to be displayed in the view
//            ViewBag.OcrResult = resultText;
//            return View("Index");
//        }


//    }
//}