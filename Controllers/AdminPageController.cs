using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProjectIT15.Models;
using MyProjectIT15.Services;

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
            return View();
        }
        [Authorize(Roles = "tenant")]
        public IActionResult tenant()
        {
            return View();
        }
        [Authorize(Roles = "owner")]
        public IActionResult owner()
        {
            return View();
        }

		[Authorize(Roles = "admin")]
		public IActionResult room()
        {
            var rooms = _context.Rooms.OrderByDescending(p => p.Id).ToList();
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
				CreatedAt = DateTime.Now,
				UserId = user?.Id // optional chaining in case user is null
			};

			_context.Rooms.Add(room);
			await _context.SaveChangesAsync();

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

			_context.SaveChanges();

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

			return RedirectToAction("room", "AdminPage");
		}



        // GET: AdminPage/AssignTenant
        [HttpGet]
        public async Task<IActionResult> AssignTenant()
        {
            ViewBag.Title = "Assign Tenant to Room";
            ViewBag.Rooms = _context.Rooms
                    .Where(r => r.Status == "Available")
                    .ToList();
            //ViewBag.Users = _context.Users.ToList();

            var tenants = await _userManager.GetUsersInRoleAsync("Tenant");
            ViewBag.Users = tenants.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTenant(UserRoomDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Title = "Assign Tenant to Room";
                ViewBag.Rooms = _context.Rooms
                    .Where(r => r.Status == "Available")
					.ToList();
                //ViewBag.Users = _context.Users.ToList();

                var tenants = await _userManager.GetUsersInRoleAsync("Tenant");
                ViewBag.Users = tenants.ToList();
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

        public IActionResult ViewAssignedTenants()
		{
			var assignedTenants = _context.UserRooms
				.Include(ur => ur.Room)  // Include related room details
				.Include(ur => ur.Tenant)  // Include related tenant details
				.Include(ur => ur.Admin)  // Include related admin details
				.ToList();

			return View(assignedTenants);
		}

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




    }


}

