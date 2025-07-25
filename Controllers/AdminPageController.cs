﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProjectIT15.Models;
using MyProjectIT15.Services;
using System.Globalization;

namespace MyProjectIT15.Controllers
{
    public class AdminPageController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly ApplicationDBContext _context;
		private readonly IWebHostEnvironment environment;

		public AdminPageController(UserManager<User> userManager, ApplicationDBContext context, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _context = context;
			this.environment = environment;
		}

        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            // Active Rooms Count
            var room = _context.Rooms
                .Where(r => r.Status == "Active")
                .ToList() ?? new List<Room>();
            ViewBag.RoomCount = room.Count;

            var bill = _context.Billings
                .Where(r => r.Status == "Unpaid")
                .ToList() ?? new List<Billing>();
            ViewBag.billCount = bill.Count;

            var totalbill = _context.Billings
                .ToList() ?? new List<Billing>();
            ViewBag.totalbillCount = totalbill.Count;

            var availroom = _context.Rooms
                .Where(r => r.Status == "Active" &&
                 !_context.UserRooms.Any(ur => ur.RoomId == r.Id && ur.Status == "Active"))
                .ToList() ?? new List<Room>();
            ViewBag.AvailRoomCount = availroom.Count;

            var payments = _context.Payments
                .ToList() ?? new List<Payment>();
            ViewBag.TotalPaymentCount = payments.Count;

            var paymentspaid = _context.Payments
                .Where(p => p.Status == "Paid")
                .ToList() ?? new List<Payment>();
            ViewBag.TotalPaidPaymentCount = paymentspaid.Count;

            // Total Rooms Count
            var rooms = _context.Rooms.ToList() ?? new List<Room>();
            ViewBag.TotalRoomCount = rooms.Count;

            // Tenants Count
            var tenantRole = _context.Roles.FirstOrDefault(r => r.Name == "Tenant");
            var tenants = _context.Users
                .Where(u => u.EmailConfirmed == true && tenantRole != null &&
                    _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == tenantRole.Id))
                .ToList() ?? new List<User>();
            ViewBag.TenantCount = tenants.Count;

            var alltenants = _context.Users
                .Where(u =>tenantRole != null &&
                    _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == tenantRole.Id))
                .ToList() ?? new List<User>();
            ViewBag.AllTenantCount = alltenants.Count;

            // New Tenants (last 5 days)
            var newTenants = _context.Users
                .Where(u => u.EmailConfirmed == true && u.CreatedAt >= DateTime.Now.AddDays(-5) && tenantRole != null &&
                    _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == tenantRole.Id))
                .Select(u => new
                {
                    Title = "New Tenant Added",
                    Description = $"{u.UserName} assigned to a room",
                    Date = u.CreatedAt
                }).ToList();

            // Meter Readings (current month)
            var meterreadings = _context.MeterReadings
                .Include(mt => mt.RoomMeter)  // Include the related RoomMeter entity
                .Where(mt => mt.ReadingDate != null &&
                             mt.ReadingDate.Year == DateTime.Now.Year &&
                             mt.ReadingDate.Month == DateTime.Now.Month)
                .ToList() ?? new List<MeterReading>();
            ViewBag.TotalMeterReadCount = meterreadings.Count;

            var meterReadings = meterreadings
                .Select(mt => new
                {
                    Title = "Meter Reading Submitted",
                    Description = $"Room {(mt.RoomMeter != null ? mt.RoomMeter.RoomId.ToString() : "Unknown")} submitted a meter reading",
                    Date = mt.ReadingDate
                });
            // Combine and order by the most recent date
            var recentActivities = newTenants
                .Union(meterReadings)
                .OrderByDescending(a => a.Date)
                .Take(10)
                .ToList();

            ViewBag.RecentActivities = recentActivities;
            return View();
        }


        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> tenant()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect if user is not found
            }

            // User Room Information
            var userRoom = _context.UserRooms
                .Include(ur => ur.Room)  // Ensure Room data is loaded
                .FirstOrDefault(ur => ur.TenantId == user.Id && ur.Status == "Active");

            if (userRoom?.Room != null)
            {
                ViewBag.RoomNum = userRoom.Room.Room_Number.ToString();
                ViewBag.RoomRent = (decimal?)userRoom.Room.Monthly_Rent ?? 0;
                ViewBag.RoomImg = !string.IsNullOrEmpty(userRoom.Room.ImageFileName) ?
                                  "/rooms/" + userRoom.Room.ImageFileName : "default.png";
            }
            else
            {
                ViewBag.RoomNum = "N/A";
                ViewBag.RoomRent = 0;
                ViewBag.RoomImg = "default.png";  // Fallback image
            }

            // Payments Count
            ViewBag.TotalPaymentCount = _context.Payments
                .Where(p => p.UserId == user.Id)
                .Count();

            // Billings Count
            ViewBag.TotalBillingCount = _context.Billings
                .Where(b => b.UserId == user.Id)
                .Count();

            // Latest Unpaid Billing
            var latestBilling = _context.Billings
                .Where(b => b.UserId == user.Id && b.Status == "Unpaid")
                .OrderByDescending(b => b.Id)
                .FirstOrDefault();

            if (latestBilling != null)
            {
                ViewBag.TotalBillAmount = (decimal?)latestBilling.TotalAmount ?? 0;
                ViewBag.DueDate = latestBilling.DueDate.ToString("MMMM dd, yyyy");

            }
            else
            {
                ViewBag.TotalBillAmount = 0;
                ViewBag.DueDate = "N/A";
            }

            // Paid Payments Count
            var paymentspaid = _context.Payments
                .Where(p => p.Status == "Paid" && p.UserId == user.Id)
                .ToList();
            ViewBag.TotalPaidPaymentCount = paymentspaid.Count;

            // Meter Readings (current month)
            var meterreadings = _context.MeterReadings
                .Where(mt => mt.UserRoom.TenantId == user.Id)
                .OrderByDescending(mt => mt.Id)
                .FirstOrDefault();

            ViewBag.TotalElecCons = meterreadings?.Consumption ?? 0;
            ViewBag.TotalWaterCons = meterreadings?.WaterConsumption ?? 0;

            // Meter Reading Activities
            var meterReadings = _context.MeterReadings
				.Where(mt => mt.UserRoom.TenantId == user.Id)
				.Select(mt => new
                {
                    Title = "Meter Reading Submitted",
                    Description = $"Room {(mt.RoomMeter != null ? mt.RoomMeter.RoomId.ToString() : "Unknown")} submitted a meter reading",
                    Date = mt.ReadingDate
                })
                .ToList();

            // Payment Activities
            var paymentsAct = _context.Payments
				.Where(p => p.UserId == user.Id)
				.Select(p => new
                {
                    Title = "Payment Transaction",
                    Description = $"You submitted a payment of ₱{p.TotalPaid:N2}.",
                    Date = p.CreatedAt
                })
                .ToList();

            // Recent Activities
            var recentActivities = paymentsAct
                .Union(meterReadings)
                .OrderByDescending(a => a.Date)
                .Take(10)
                .ToList();

            ViewBag.RecentActivities = recentActivities;

            return View();
        }



        [Authorize(Roles = "admin")]
        public IActionResult Tenants()
        {
            // Get the tenant role by name (ensure we get the complete Role object)
            var tenantRole = _context.Roles.FirstOrDefault(r => r.Name == "Tenant");

            // If tenantRole is null, return an empty list
            if (tenantRole == null)
            {
                return View(new List<User>());
            }

            // Check if RoleId is null or invalid in UserRoles table
            var tenants = _context.Users
                .Where(u => _context.UserRoles
                    .Any(ur => ur.UserId == u.Id && ur.RoleId != null && ur.RoleId == tenantRole.Id))  // Ensure RoleId is not null
                .ToList();

            return View(tenants);
        }









        [Authorize(Roles = "owner")]
        public IActionResult owner()
        {
            return View();
        }

		[Authorize(Roles = "admin")]
		public IActionResult room()
        {
            var rooms = _context.Rooms
                .OrderByDescending(p => p.Id)
                .ToList();
            return View(rooms);
        }

		[Authorize(Roles = "admin")]
		public IActionResult Create()
        {

            return View();
        }

		//      [HttpPost]
		//public IActionResult Create(RoomDto roomDto)
		//{


		//          if (roomDto.ImageFile == null) 
		//          {
		//              ModelState.AddModelError("ImageFile", "The image file is required");
		//          }

		//          if (!ModelState.IsValid) 
		//          {
		//              return View(roomDto);
		//          }

		//          string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
		//          newFileName += Path.GetExtension(roomDto.ImageFile!.FileName);

		//          string imageFullPath = environment.WebRootPath + "/rooms/" + newFileName;
		//          using (var stream = System.IO.File.Create(imageFullPath)) 
		//          {
		//          roomDto.ImageFile.CopyTo(stream);
		//          }


		//	Room room = new Room
		//	{
		//		Room_Number = roomDto.Room_Number,
		//              Room_Type = roomDto.Room_Type,
		//              Monthly_Rent = roomDto.Monthly_Rent,
		//              ImageFileName = newFileName,
		//              Status = roomDto.Status,
		//              CreatedAt = DateTime.Now,
		//          };

		//	_context.Rooms.Add(room);
		//	_context.SaveChanges();


		//	return RedirectToAction("room", "AdminPage");


		//}
		[Authorize(Roles = "admin")]
		[HttpPost]
		public async Task<IActionResult> Create(RoomDto roomDto)
		{
			var user = await _userManager.GetUserAsync(User);

			if (roomDto.ImageFile == null)
			{
				ModelState.AddModelError("ImageFile", "The image file is required");
			}

            if (_context.Rooms.Any(r => r.Room_Number == roomDto.Room_Number && r.Status == "Active"))
            {
                ModelState.AddModelError("Room_Number", "The room number must be unique from active rooms.");
                return View(roomDto);
            }

            if (!ModelState.IsValid)
			{
				return View(roomDto);
			}


			string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
			newFileName += Path.GetExtension(roomDto.ImageFile!.FileName);

			string imageFullPath = environment.WebRootPath + "/rooms/" + newFileName;
			using (var stream = System.IO.File.Create(imageFullPath))
			{
				roomDto.ImageFile.CopyTo(stream);
			}

			Room room = new Room
			{
				Room_Number = roomDto.Room_Number,
				Room_Type = roomDto.Room_Type,
				Monthly_Rent = roomDto.Monthly_Rent,
				ImageFileName = newFileName,
				Status = roomDto.Status,
                CreatedAt = DateTime.UtcNow.AddHours(8),
                UserId = user?.Id // optional chaining in case user is null
			};

			_context.Rooms.Add(room);
			await _context.SaveChangesAsync();

			TempData["ShowSuccess"] = true;
			TempData["Success"] = "Room added successfully.";
			return RedirectToAction("room", "AdminPage");
		}

		[Authorize(Roles = "admin")]
		public IActionResult Edit(int Id)
		{
			var room = _context.Rooms.Find(Id);

			if (room == null)
			{
				return RedirectToAction("room", "AdminPage");
			}

			var roomDto = new RoomDto()
			{
				Room_Number = room.Room_Number,
				Room_Type = room.Room_Type,
				Monthly_Rent = room.Monthly_Rent,
				Status = room.Status,
				
			};

			ViewData["RoomId"] = room.Id;
			ViewData["ImageFileName"] = room.ImageFileName;
			ViewData["CreatedAt"] = room.CreatedAt.ToString("MM/dd/yyyy");

			return View(roomDto);
		}
		
        [Authorize(Roles = "admin")]
		[HttpPost]
		public IActionResult Edit(int Id, RoomDto roomDto)
		{
			var room = _context.Rooms.Find(Id);
			if (room == null)
			{
				return RedirectToAction("room", "AdminPage");
			}


			if (!ModelState.IsValid)
			{
				ViewData["RoomId"] = room.Id;
				ViewData["ImageFileName"] = room.ImageFileName;
				ViewData["CreatedAt"] = room.CreatedAt.ToString("MM/dd/yyyy");

				return View(roomDto);
			}

			string newFileName = room.ImageFileName;
			if (roomDto.ImageFile != null)
			{
				newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
				newFileName += Path.GetExtension(roomDto.ImageFile.FileName);

				string imageFullPath = environment.WebRootPath + "/rooms/" + newFileName;
				using (var stream = System.IO.File.Create(imageFullPath))
				{
					roomDto.ImageFile.CopyTo(stream);
				}

				string oldImageFullPath = environment.WebRootPath + "/rooms/" + room.ImageFileName;
				System.IO.File.Delete(oldImageFullPath); 
			}

			room.Room_Number = roomDto.Room_Number;
			room.Room_Type = roomDto.Room_Type;
			room.Monthly_Rent = roomDto.Monthly_Rent;
			room.Status = roomDto.Status;
			room.ImageFileName = newFileName;

			// Check if there is an active user assigned to the room
			bool hasActiveUser = _context.UserRooms
				.Any(ur => ur.RoomId == room.Id && ur.Status == "Active");

			if (roomDto.Status != "Active" && hasActiveUser)
			{
				TempData["ShowError"] = true;
				TempData["Error"] = "There is a user assigned to this Room. The status cannot be updated.";
				return RedirectToAction("room", "AdminPage");
			}

			_context.SaveChanges();

			TempData["ShowSuccess"] = true;
			TempData["Success"] = "Room updated successfully.";

			return RedirectToAction("room", "AdminPage");
		}

		[Authorize(Roles = "admin")]
		public IActionResult Delete(int Id)
		{
			var room = _context.Rooms.Find(Id);
			if (room == null)
			{
				return RedirectToAction("room", "AdminPage");
			}

			string ImageFullPath = environment.WebRootPath + "/rooms/" + room.ImageFileName;
			System.IO.File.Delete(ImageFullPath);

			_context.Rooms.Remove(room);
			_context.SaveChanges(true);

			TempData["ShowSuccess"] = true;
			TempData["Success"] = "Room deleted successfully.";

			return RedirectToAction("room", "AdminPage");
		}


        [Authorize(Roles = "admin")]
        // GET: AdminPage/AssignTenant
        [HttpGet]
        public async Task<IActionResult> AssignTenant()
        {
            ViewBag.Title = "Assign Tenant to Room";

            // Exclude rooms that are already assigned and active
            var assignedRoomIds = _context.UserRooms
                .Where(ur => ur.Status == "Active")
                .Select(ur => ur.RoomId)
                .ToList();

            ViewBag.Rooms = _context.Rooms
                .Where(r => r.Status == "Active" && !assignedRoomIds.Contains(r.Id))
                .ToList();

            // Exclude tenants that are already assigned and active
            var assignedTenantIds = _context.UserRooms
                .Where(ur => ur.Status == "Active")
                .Select(ur => ur.TenantId)
                .ToList();

            var tenants = await _userManager.GetUsersInRoleAsync("Tenant");
            ViewBag.Users = tenants
                .Where(t => !assignedTenantIds.Contains(t.Id))
                .ToList();
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTenant(UserRoomDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Assign Tenant to Room";

                // Exclude rooms that are already assigned and active
                var assignedRoomIds = _context.UserRooms
                    .Where(ur => ur.Status == "Active")
                    .Select(ur => ur.RoomId)
                    .ToList();

                ViewBag.Rooms = _context.Rooms
                    .Where(r => r.Status == "Active" && !assignedRoomIds.Contains(r.Id))
                    .ToList();

                // Exclude tenants that are already assigned and active
                var assignedTenantIds = _context.UserRooms
                    .Where(ur => ur.Status == "Active")
                    .Select(ur => ur.TenantId)
                    .ToList();

                var tenants = await _userManager.GetUsersInRoleAsync("Tenant");
                ViewBag.Users = tenants
                    .Where(t => !assignedTenantIds.Contains(t.Id))
                    .ToList();

                return View(dto);
            }

            // Get the currently logged-in admin user
            var currentAdmin = await _userManager.GetUserAsync(User);

            if (currentAdmin == null)
            {
                TempData["Error"] = "You must be logged in to assign tenants.";
                return RedirectToAction("Login", "Account");  // Redirect to login page if not logged in
            }

            // Set AdminId from the logged-in admin
            var currentAdminId = currentAdmin.Id;

            // Create the UserRoom object with the AdminId populated
            var userRoom = new UserRoom
            {
                RoomId = dto.RoomId,
                TenantId = dto.TenantId,
                AdminId = currentAdminId,  // AdminId is automatically assigned here
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status
            };

            // Add the new UserRoom to the context and save changes
            _context.UserRooms.Add(userRoom);
            await _context.SaveChangesAsync();  // Save changes asynchronously

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Tenant successfully assigned to room.";
            return RedirectToAction("ViewAssignedTenants");
        }

        [Authorize(Roles = "admin")]
        public IActionResult ViewAssignedTenants()
		{
			var assignedTenants = _context.UserRooms
				.Include(ur => ur.Room)  // Include related room details
				.Include(ur => ur.Tenant)  // Include related tenant details
				.Include(ur => ur.Admin)  // Include related admin details
				.ToList();

			return View(assignedTenants);
		}

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> EditAssignedTenant(int id)
        {
            var userRoom = await _context.UserRooms.FindAsync(id);
            if (userRoom == null)
            {
                TempData["ShowError"] = true;
                TempData["Error"] = "Assigned tenant not found.";
                return RedirectToAction("ViewAssignedTenants");
            }

            var dto = new UserRoomDto
            {
                RoomId = userRoom.RoomId,
                TenantId = userRoom.TenantId,
                StartDate = userRoom.StartDate.GetValueOrDefault(),
                EndDate = userRoom.EndDate.GetValueOrDefault(),
                Status = userRoom.Status
            };

            ViewBag.Title = "Edit Assigned Tenant";

            // Load room options – if you want all rooms (not just "Available")
            ViewBag.Rooms = _context.Rooms.ToList();

            // Load tenants
            var tenants = await _userManager.GetUsersInRoleAsync("Tenant");
            ViewBag.Users = tenants.ToList();

            ViewData["UserRoomId"] = id;

            return View(dto);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssignedTenant(int UserRoomId, UserRoomDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Edit Assigned Tenant";
                ViewBag.Rooms = _context.Rooms.ToList(); // You may filter this as needed

                var tenants = await _userManager.GetUsersInRoleAsync("Tenant");
                ViewBag.Users = tenants.ToList();

                ViewData["UserRoomId"] = UserRoomId;
                return View(dto);
            }

            var userRoom = await _context.UserRooms.FindAsync(UserRoomId);
            if (userRoom == null)
            {
                TempData["ShowError"] = true;
                TempData["Error"] = "Assignment not found.";
                return RedirectToAction("ViewAssignedTenants");
            }

            // Optional: Update only certain fields (exclude RoomId/TenantId if they shouldn't change)
            userRoom.RoomId = dto.RoomId;
			userRoom.TenantId = dto.TenantId;
            userRoom.StartDate = dto.StartDate;
            userRoom.EndDate = dto.EndDate;
            userRoom.Status = dto.Status;

            _context.UserRooms.Update(userRoom);
            await _context.SaveChangesAsync();

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Assignment successfully updated.";
            return RedirectToAction("ViewAssignedTenants");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateAssignedTenant(int id)
        {
            var userRoom = await _context.UserRooms.FindAsync(id);
            if (userRoom == null)
            {
                TempData["ShowError"] = true;
                TempData["Error"] = "Assigned tenant not found.";
                return RedirectToAction("ViewAssignedTenants");
            }

            userRoom.Status = "Inactive";

            _context.UserRooms.Update(userRoom);
            await _context.SaveChangesAsync();

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Tenant assignment has been deactivated.";
            return RedirectToAction("ViewAssignedTenants");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactTenant(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["ShowError"] = true;
                TempData["Error"] = "No tenant found.";
                return RedirectToAction("Tenants");
            }

            // Check if the user is already inactive
            if (!user.EmailConfirmed)
            {
                TempData["ShowError"] = true;
                TempData["Error"] = "The tenant is already inactive.";
                return RedirectToAction("Tenants");
            }

            user.EmailConfirmed = false;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Tenant has been deactivated.";
            return RedirectToAction("Tenants");
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActTenant(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                TempData["ShowError"] = true;
                TempData["Error"] = "No tenant found.";
                return RedirectToAction("Tenants");
            }

            // Check if the user is already active
            if (user.EmailConfirmed)
            {
                TempData["ShowError"] = true;
                TempData["Error"] = "The tenant is already active.";
                return RedirectToAction("Tenants");
            }

            user.EmailConfirmed = true;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["ShowSuccess"] = true;
            TempData["Success"] = "Tenant has been activated.";
            return RedirectToAction("Tenants");
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            return View();
        }

		[Authorize(Roles = "admin")]
		public IActionResult Reports(DateTime? startDate, DateTime? endDate)
		{
			try
			{
				// Check if Payments table exists and is not empty
				if (_context.Payments != null && _context.Payments.Any())
				{
					var payments = _context.Payments.ToList();
					ViewBag.TotalPaymentCount = payments.Count;
				}
				else
				{
					ViewBag.TotalPaymentCount = "No payment records available.";
                    return View();
				}

				// Check if Billings table exists and is not empty
				if (_context.Billings != null && _context.Billings.Any())
				{
					var billings = _context.Billings
						.Where(b => b.Status.Trim().ToLower() == "paid")
						.ToList();
					ViewBag.TotalBillingCount = billings.Count;

					if (billings.Count == 0)
					{
						ViewBag.NoBillingsMessage = "No paid billing records available.";
					}

					// Calculate average bill amount if there are paid billings
					if (billings.Count > 0)
					{
						var averageBillAmount = _context.Billings
							.Where(b => b.Status.Trim().ToLower() == "paid")
							.Average(b => (decimal?)b.TotalAmount ?? 0);
						ViewBag.AverageBillAmount = Math.Round(averageBillAmount, 2);
					}
					else
					{
						ViewBag.AverageBillAmount = 0;
					}

					// Calculate total bill amount for all paid billings
					var totalBillAmount = _context.Billings
						.Where(b => b.Status.Trim().ToLower() == "paid")
						.Sum(b => (decimal?)b.TotalAmount ?? 0);
					ViewBag.TotalBillAmount = totalBillAmount;

					// Get the most recent paid billing
					var billing = _context.Billings
						.Where(b => b.Status.Trim().ToLower() == "paid")
						.OrderByDescending(b => b.Id)
						.FirstOrDefault();

					if (billing != null)
					{
						ViewBag.LatestBillAmount = billing.TotalAmount;
						ViewBag.DueDate = billing.DueDate.ToString("MMMM dd, yyyy");
					}
					else
					{
						ViewBag.LatestBillAmount = 0;
						ViewBag.DueDate = "N/A";
					}

					// Filter and group billing data based on the date range
					var query = _context.Billings
						.Where(b => b.Status.Trim().ToLower() == "paid");

					if (startDate.HasValue)
					{
						query = query.Where(b => b.Payments.Any(p => p.CreatedAt >= startDate.Value));
					}

					if (endDate.HasValue)
					{
						query = query.Where(b => b.Payments.Any(p => p.CreatedAt <= endDate.Value));
					}

					var combinedData = query
						.OrderByDescending(b => b.Id)
						.GroupJoin(_context.Payments
							.Where(p => p.Status.Trim().ToLower() == "paid"),
							billing => billing.Id,
							payment => payment.BillingId,
							(billing, payments) => new
							{
								billing.Id,
								billing.User.FirstName,
								billing.User.LastName,
								billing.MeterReading.RoomMeter.Room.Room_Number,
								billing.TotalAmount,
								billing.Status,
								Payments = payments.Select(payment => new
								{
									payment.CreatedAt,
									payment.PaymentMethod
								}).ToList()
							})
						.ToList();

					// Check if combined data is empty
					if (combinedData.Count == 0)
					{
						ViewBag.NoDataMessage = "No data available for the selected date range.";
					}

					// Calculate the sum of TotalAmount from combined data
					var totalAmount = combinedData.Sum(b => b.TotalAmount);
					ViewBag.TotalBillAmount = totalAmount;
					ViewBag.CombinedData = combinedData;
				}
				else
				{
					ViewBag.TotalBillingCount = "No billing records available.";
					ViewBag.AverageBillAmount = 0;
					ViewBag.TotalBillAmount = 0;
					ViewBag.LatestBillAmount = 0;
					ViewBag.DueDate = "N/A";
					ViewBag.NoDataMessage = "No data available as the billing table is empty.";
					ViewBag.CombinedData = new List<object>();
				}
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = $"An error occurred while generating the report: {ex.Message}";
				return View("Error");
			}

			return View();
		}



	}


}

