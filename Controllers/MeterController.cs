using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using OpenCvSharp;
using Tesseract;
using Microsoft.EntityFrameworkCore;
using MyProjectIT15.Services;
using Microsoft.AspNetCore.Identity;
using MyProjectIT15.Models;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MyProjectIT15.Controllers
{
    public class MeterController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly ApplicationDBContext _context;
		private readonly UserManager<User> _userManager;

		public MeterController(UserManager<User> userManager, IWebHostEnvironment env, ApplicationDBContext context)
        {
            _context = context;
			_userManager = userManager;
			_env = env;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Meter()
        {
            var meters = _context.Meters.OrderByDescending(p => p.Id).ToList();
            var user = _context.Meters
                .Include(ur => ur.User)  // Include related admin details
                .ToList();
            return View(meters);
        }

        public IActionResult Create()
        {
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Create(MeterDto meterDto)
		{
			var user = await _userManager.GetUserAsync(User);


			if (!ModelState.IsValid)
			{
				return View(meterDto);
			}


			Meter meter = new Meter
			{
				Meter_Number = meterDto.Meter_Number,
				Status = "Active",
				CreatedAt = DateTime.Now,
				UserId = user?.Id // optional chaining in case user is null
			};

			_context.Meters.Add(meter);
			await _context.SaveChangesAsync();

			TempData["ShowSuccess"] = true;
			TempData["Success"] = "Meter added successfully.";

			return RedirectToAction("Meter", "Meter");
		}

        public IActionResult Edit(int Id)
        {
            var meter = _context.Meters.Find(Id);

            if (meter == null)
            {
                return RedirectToAction("Meter", "Meter");
            }

            var meterDto = new MeterDto()
            {
                Meter_Number = meter.Meter_Number,
                Status = meter.Status,

            };

            ViewData["MeterId"] = meter.Id;
            ViewData["CreatedAt"] = meter.CreatedAt.ToString("MM/dd/yyyy");

            return View(meterDto);
        }

        [HttpPost]
        public IActionResult Edit(int Id, MeterDto meterDto)
        {
            var meter = _context.Meters.Find(Id);
            if (meter == null)
            {
                return RedirectToAction("Meter", "Meter");
            }


            if (!ModelState.IsValid)
            {
                ViewData["MeterId"] = meter.Id;
                ViewData["CreatedAt"] = meter.CreatedAt.ToString("MM/dd/yyyy");

                return View(meterDto);
            }

            meter.Meter_Number = meterDto.Meter_Number;
            meter.Status = meterDto.Status;

            _context.SaveChanges();

			TempData["ShowSuccess"] = true;
			TempData["Success"] = "Meter updated successfully.";

			return RedirectToAction("Meter", "Meter");
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Deactivate(int id)
		{
			var meter = await _context.Meters.FindAsync(id);
			if (meter == null)
			{
				TempData["ShowError"] = true;
				TempData["Error"] = "Meter not found.";
				return RedirectToAction("Meter");
			}

			meter.Status = "Inactive";

			_context.Meters.Update(meter);
			await _context.SaveChangesAsync();

			TempData["ShowSuccess"] = true;
			TempData["Success"] = "Meter has been deactivated.";
			return RedirectToAction("Meter");
		}

        public IActionResult RoomMeter()
        {
            var roomMeters = _context.RoomMeters.OrderByDescending(p => p.Id).ToList();
            var room = _context.RoomMeters
                .Include(ur => ur.Room)  
                .ToList();
            var meter = _context.RoomMeters
                .Include(ur => ur.Meter)
                .ToList();

            return View(roomMeters);
        }


        [HttpGet]
        public IActionResult AssignMeter()
        {
            ViewBag.Rooms = _context.Rooms
                    .Where(r => r.Status == "Active")
                    .ToList();
            ViewBag.Meters = _context.Meters
                    .Where(r => r.Status == "Active")
                    .ToList();

            return View();
        }

        [HttpPost]
        public ActionResult AssignMeter(RoomMeterDto roomMeterDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
                Console.WriteLine("Errors: " + string.Join(", ", errors));
                // Repopulate ViewBag!
                ViewBag.Rooms = _context.Rooms
                    .Where(r => r.Status == "Active")
                    .ToList();
                ViewBag.Meters = _context.Meters
                    .Where(r => r.Status == "Active")
                    .ToList();

                return View(roomMeterDto);
            }

            Console.WriteLine($"RoomId: {roomMeterDto.RoomId}, MeterId: {roomMeterDto.MeterId}"); // Debugging line

            RoomMeter roomMeter = new RoomMeter
            {
                RoomId = roomMeterDto.RoomId,
                MeterId = roomMeterDto.MeterId,
                Status = "Active",
                installedDate = DateTime.UtcNow,
            };

            _context.RoomMeters.Add(roomMeter);
            _context.SaveChanges();

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Room Meter assigned successfully.";

            return RedirectToAction("RoomMeter", "Meter");
        }

        public IActionResult EditRoomMeter(int id)
        {
            var roomMeter = _context.RoomMeters.Find(id);

            if (roomMeter == null)
            {
                return RedirectToAction("RoomMeter", "Meter");
            }

            var roomMeterDto = new RoomMeterDto()
            {
                MeterId = roomMeter.MeterId,
                RoomId = roomMeter.RoomId,
                Status = roomMeter.Status,
            };

            ViewBag.Rooms = _context.Rooms.ToList();
            ViewBag.Meters = _context.Meters.ToList();
            ViewBag.Status = new List<string> { "Active", "Inactive" };
            ViewData["InstalledDate"] = roomMeter.installedDate.ToString("MM/dd/yyyy");

            return View(roomMeterDto);
        }

        [HttpPost]
        public IActionResult EditRoomMeter(int id, RoomMeterDto roomMeterDto)
        {
            var roomMeter = _context.RoomMeters.Find(id);
            if (roomMeter == null)
            {
                return RedirectToAction("RoomMeter", "Meter");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Rooms = _context.Rooms.ToList();
                ViewBag.Meters = _context.Meters.ToList();
                ViewBag.Status = new List<string> { "Active", "Inactive" };
                ViewData["InstalledDate"] = roomMeter.installedDate.ToString("MM/dd/yyyy");

                return View(roomMeterDto);
            }

            roomMeter.RoomId = roomMeterDto.RoomId;
            roomMeter.MeterId = roomMeterDto.MeterId;
            roomMeter.Status = roomMeterDto.Status;

            _context.SaveChanges();

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Room Meter updated successfully.";

            return RedirectToAction("RoomMeter", "Meter");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateRoomMeter(int id)
        {
            var roomMeter = await _context.RoomMeters.FindAsync(id);
            if (roomMeter == null)
            {
                TempData["ShowError"] = true;
                TempData["Error"] = "Room Meter not found.";
                return RedirectToAction("RoomMeter");
            }

            roomMeter.Status = "Inactive";

            _context.RoomMeters.Update(roomMeter);
            await _context.SaveChangesAsync();

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Room Meter has been deactivated.";
            return RedirectToAction("RoomMeter");
        }

        public IActionResult MeterReading()
        {
            var meterReadings = _context.MeterReadings
            .OrderByDescending(p => p.Id)
            .Include(mr => mr.RoomMeter)
            .ThenInclude(rm => rm.Room)
            .Include(mr => mr.UserRoom) // Only until UserRoom, no ThenInclude
            .ThenInclude(ur => ur.Tenant)
            .Include(mr => mr.User)
            .ToList();

            return View(meterReadings); // Pass the list of MeterReading objects to the view
        }

        [HttpGet]
        public IActionResult ReadMeter()
        {
            var meterReadingDto = new MeterReadingDto
            {
                ReadingDate = DateTime.UtcNow,
                RoomMeters = _context.RoomMeters
                    .Where(rm => rm.Status == "Active")
                    .Include(rm => rm.Room)
                    .Include(rm => rm.Meter)
                    .Select(rm => new SelectListItem
                    {
                        Value = rm.Id.ToString(),
                        Text = $"Meter {rm.Meter.Meter_Number} - Room {rm.Room.Room_Number}"
                    })
                    .ToList()
            };

            return View(meterReadingDto);
        }

        [HttpPost]
        public async Task<IActionResult> ReadMeter(MeterReadingDto meterReadingDto)
        {
            if (!ModelState.IsValid)
            {
                meterReadingDto.RoomMeters = _context.RoomMeters
                    .Where(rm => rm.Status == "Active")
                    .Include(rm => rm.Room)
                    .Include(rm => rm.Meter)
                    .Select(rm => new SelectListItem
                    {
                        Value = rm.Id.ToString(),
                        Text = $"Meter {rm.Meter.Meter_Number} - Room {rm.Room.Room_Number}"
                    }).ToList();

                return View(meterReadingDto);
            }

            var user = await _userManager.GetUserAsync(User);

            var roomMeter = await _context.RoomMeters
                .Include(rm => rm.Room)
                .FirstOrDefaultAsync(rm => rm.Id == meterReadingDto.RoomMeterId);

            if (roomMeter == null)
            {
                ModelState.AddModelError("", "Selected meter not found.");
                return View(meterReadingDto);
            }

            var userRoom = await _context.UserRooms
                .FirstOrDefaultAsync(ur => ur.RoomId == roomMeter.RoomId && ur.Status == "Active");

            if (userRoom == null)
            {
                ModelState.AddModelError("", "No active user assigned to this room.");
                return View(meterReadingDto);
            }

            var meterReading = new MeterReading
            {
                UserRoomId = userRoom.Id,
                RoomMeterId = meterReadingDto.RoomMeterId,
                ReadingDate = meterReadingDto.ReadingDate,
                PreviousReading = meterReadingDto.PreviousReading,
                CurrentReading = meterReadingDto.CurrentReading,
                Consumption = meterReadingDto.CurrentReading - meterReadingDto.PreviousReading,
                UserId = user?.Id
            };

            var calculateTotalAmount = Math.Round((meterReadingDto.CurrentReading - meterReadingDto.PreviousReading) * 13.0127m, 2);

            _context.MeterReadings.Add(meterReading);
            await _context.SaveChangesAsync();

            var billing = new Billing
            {
                UserId = userRoom.TenantId,
                MeterReadingId = meterReading.Id, // Link the new meter reading
                ReadingDate = meterReadingDto.ReadingDate,
                DueDate = meterReadingDto.ReadingDate.AddDays(14),
                TotalAmount = calculateTotalAmount,
                Status = "Unpaid"
            };

            _context.Billings.Add(billing);
            await _context.SaveChangesAsync();

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Meter Reading and Billing done successsfully.";

            return RedirectToAction("MeterReading", "Meter");
        }

        public IActionResult Billings()
        {
            var billings = _context.Billings
            .OrderByDescending(b => b.Id)
            .Include(u => u.User)
            .Include(mr => mr.MeterReading)
            .ToList();

            return View(billings); 
        }

        public async Task<IActionResult> UserBillings()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

            var billings = await _context.Billings
                .Where(b => b.UserId == currentUserId) // <-- Filter billings by current user ID
                .OrderByDescending(b => b.Id)
                .Include(b => b.User)
                .Include(b => b.MeterReading)
                    .ThenInclude(mr => mr.RoomMeter) // if you want RoomMeter details too
                .ToListAsync();

            return View(billings);
        }





































        //public IActionResult MeterReading()
        //{
        //    ViewBag.OcrResult = TempData["OcrResult"];
        //    ViewBag.Error = TempData["Error"];
        //    return View();
        //}

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