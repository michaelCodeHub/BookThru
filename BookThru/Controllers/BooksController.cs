using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookThru.Models;
using Amazon.S3;
using Amazon;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Authorization;

namespace BookThru.Controllers
{
    public class BooksController : Controller
    {
        private const string bucketName = "wehire";
        private const string IAMUserKey = "AKIAI27NEVOTDANXC7YQ";
        private const string IAMUserPass = "5eOsHBc7Y1CLuEB6+YMLuuNB/Daf+KHGXOT3PMkI";

        private readonly BookThruContext _context;

        public BooksController(BookThruContext context)
        {
            _context = context;
        }

        // GET: Books
  
        public async Task<IActionResult> Index()
        {
            return View(await _context.Book.ToListAsync());
        }

        // GET: Books/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.BookId == id);
            book.User = _context.Users.Where(user => user.Email == User.Identity.Name).FirstOrDefault();
            book.Category = _context.Category.Where(cat => cat.CategoryId == book.CategoryId).FirstOrDefault();
            book.CourseCode = _context.CourseCode.Where(course => course.CourseCodeId == book.CourseCodeId).FirstOrDefault();
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
  
        public IActionResult Create()
        {


            CreateModel model = new CreateModel {
                Categories = _context.Category.ToList(),
                CourseCodes = _context.CourseCode.ToList()
            };

            return View(model);
        }

        // POST: Books/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
 
        public async Task<IActionResult> Create([Bind("Book,Categories,CourseCodes")] CreateModel createModel)
        {
            var book = createModel.Book;
            if (ModelState.IsValid)
            {
                if (this.Request.Form.Files.Count < 1 || this.Request.Form.Files[0].Length < 1) throw new Exception("Please include a image file to upload.");
                var file = this.Request.Form.Files[0];
                Random rnd = new Random();
                book.ImageURL = rnd.Next(99999999).ToString() + "." + file.ContentType.Substring(file.ContentType.Length - 3);
                book.User = _context.Users.Where(user=>user.Email == User.Identity.Name).FirstOrDefault();
                book.Category = _context.Category.Where(cat=> cat.CategoryId== book.CategoryId).FirstOrDefault();
                book.CourseCode = _context.CourseCode.Where(course => course.CourseCodeId== book.CourseCodeId).FirstOrDefault();
                book.User = _context.Users.Where(user => user.Email == User.Identity.Name).FirstOrDefault();
                book.Uploaded = DateTime.Now.Date;
                await UploadFileAsync(file.OpenReadStream(), book.ImageURL);


                _context.Add(book);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
  
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            book.User = _context.Users.Where(user => user.Email == User.Identity.Name).FirstOrDefault();
            book.Category = _context.Category.Where(cat => cat.CategoryId == book.CategoryId).FirstOrDefault();
            book.CourseCode = _context.CourseCode.Where(course => course.CourseCodeId == book.CourseCodeId).FirstOrDefault();
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,Name,CategoryId,CourseCode,Editon,Description,MinimumBidPrice,BuyNowPrice,Uploaded")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.BookId == id);
            book.User = _context.Users.Where(user => user.Email == User.Identity.Name).FirstOrDefault();
            book.Category = _context.Category.Where(cat => cat.CategoryId == book.CategoryId).FirstOrDefault();
            book.CourseCode = _context.CourseCode.Where(course => course.CourseCodeId == book.CourseCodeId).FirstOrDefault();
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Book.FindAsync(id);
            _context.Book.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.BookId == id);
        }

        private static async Task UploadFileAsync(System.IO.Stream fileStream, String imageName)
        {
            try
            {

                var amazonS3Config = new AmazonS3Config();
                amazonS3Config.RegionEndpoint = RegionEndpoint.USEast1;// use your region endpoint
                IAmazonS3 s3Client = new AmazonS3Client(IAMUserKey, IAMUserPass, amazonS3Config);
                var fileTransferUtility = new TransferUtility(s3Client);

                var uploadRequest =
                    new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        InputStream = fileStream,
                        Key = imageName,
                        CannedACL = S3CannedACL.PublicRead
                    };

                uploadRequest.UploadProgressEvent +=
                    new EventHandler<UploadProgressArgs>
                        (uploadRequest_UploadPartProgressEvent);

                await fileTransferUtility.UploadAsync(uploadRequest);

            }
            catch (AmazonS3Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error encountered on server. Message:'{0}' when writing an object" + e.Message, e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Error encountered on server. Message:'{0}' when writing an object" + e.Message, e.Message);
            }
        }

        static void uploadRequest_UploadPartProgressEvent(object sender, UploadProgressArgs e)
        {
            // Process event.
            System.Diagnostics.Debug.WriteLine("{0}/{1}", e.TransferredBytes, e.TotalBytes);
        }


        public async Task<IActionResult> MakeBid(int BookId, int Amount)
        {

            if (!User.IsInRole("User"))
			{
				return LocalRedirect("/Identity/Account/Login");
			}

            var book = await _context.Book.FindAsync(BookId);

            if(book.MinimumBidPrice > Amount)
            {
                book.Message =  "Please enter a greater amount";
            }
            else
            {
                book.MinimumBidPrice = Amount;
                book.Message = "";
            }
            _context.Update(book);
            _context.SaveChanges();

            return LocalRedirect("/Books/Details/"+BookId);
        }

    }

}
