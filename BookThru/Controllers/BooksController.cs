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
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

namespace BookThru.Controllers
{
    public class BooksController : Controller
    {
        private const string bucketName = "wehire";
        private const string IAMUserKey = "AKIAI27NEVOTDANXC7YQ";
        private const string IAMUserPass = "5eOsHBc7Y1CLuEB6+YMLuuNB/Daf+KHGXOT3PMkI";

        private readonly BookThruContext _context;
        private readonly IEmailSender _emailSender;


        public BooksController(BookThruContext context,
            IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        // GET: Books
        public async Task<IActionResult> Friends()
        {
            var messages = await _context.Message.ToListAsync();

            List<string> m = new List<string>();

            foreach (var item in messages)
            {
                if (item.FromId.Equals(User.Identity.Name) && !m.Contains(item.ToId))
                {
                    m.Add(item.ToId);
                }

                if (item.ToId.Equals(User.Identity.Name) && !m.Contains(item.FromId))
                {
                    m.Add(item.FromId);
                }
            }

            return View(m);
            //return View(await _context.Book.Where(x=>x.Uploaded> DateTime.Now.Date).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> PaymentSuccess()
        {
            await _emailSender.SendEmailAsync(User.Identity.Name, "Payment Done",
                $"You have a mesasge from ");

            return View();
            //return View(await _context.Book.Where(x=>x.Uploaded> DateTime.Now.Date).ToListAsync());
        }

        // GET: Books
        [HttpGet]
        public async Task<IActionResult> Messages(string id)
        {
            var lists = await _context.Message.Where(x => (x.FromId.Equals(User.Identity.Name) && x.ToId.Equals(id)) ||
            x.ToId.Equals(User.Identity.Name) && x.FromId.Equals(id)).ToListAsync();
            return View(lists);
            //return View(await _context.Book.Where(x=>x.Uploaded> DateTime.Now.Date).ToListAsync());
        }


        // GET: Books
        [HttpPost]
        public async Task<IActionResult> Messages(string message, string toid)
        {
            Message m = new Message
            {
                Content = message,
                FromId = User.Identity.Name,
                ToId = toid,
                Sent = DateTime.Now.Date
            };

            _context.Message.Add(m);
            await _context.SaveChangesAsync();

            await _emailSender.SendEmailAsync(toid, "New Message",
                $"You have a mesasge from " + User.Identity.Name);


            var lists = await _context.Message.Where(x => (x.FromId.Equals(User.Identity.Name) && x.ToId.Equals(toid)) ||
            x.ToId.Equals(User.Identity.Name) && x.FromId.Equals(toid)).ToListAsync();
            return View(lists);
            //return View(await _context.Book.Where(x=>x.Uploaded> DateTime.Now.Date).ToListAsync());
        }


        // GET: Books
        public async Task<IActionResult> Index()
        {

            return View(await _context.Book.ToListAsync());
            //return View(await _context.Book.Where(x=>x.Uploaded> DateTime.Now.Date).ToListAsync());
        }


        // GET: Books
        public async Task<IActionResult> Search(string SearchText)
        {

            return View(await _context.Book.Where(x=>x.Name.Contains(SearchText)).ToListAsync());
            //return View(await _context.Book.Where(x=>x.Uploaded> DateTime.Now.Date).ToListAsync());
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


            CreateModel model = new CreateModel
            {
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
                book.User = _context.Users.Where(user => user.Email == User.Identity.Name).FirstOrDefault();
                book.Category = _context.Category.Where(cat => cat.CategoryId == book.CategoryId).FirstOrDefault();
                book.CourseCode = _context.CourseCode.Where(course => course.CourseCodeId == book.CourseCodeId).FirstOrDefault();
                book.User = _context.Users.Where(user => user.Email == User.Identity.Name).FirstOrDefault();
                book.Uploaded = DateTime.Now.Date;
                await UploadFileAsync(file.OpenReadStream(), book.ImageURL);
                book.CurrentBidder = "";

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

            var bookBid = new BookBid
            {
                Id = _context.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault().Id,
                DateOfBid = DateTime.Now.Date,
                BidPrice = Amount,
                BookId = BookId
            };

            _context.BookBid.Add(bookBid);

            if (book.MinimumBidPrice > Amount)
            {
                book.Message = "Please enter a greater amount";
            }
            else
            {
                book.CurrentBidder = User.Identity.Name;
                book.MinimumBidPrice = Amount;
                book.Message = "";
            }
            _context.Update(book);
            _context.SaveChanges();

            return LocalRedirect("/Books/Details/" + BookId);
        }


        public IActionResult Charge(string stripeEmail, string stripeToken)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = 500,
                Description = "Book Purchase",
                Currency = "CAD",
                CustomerId = customer.Id
            });

            return View();
        }

    }
}
