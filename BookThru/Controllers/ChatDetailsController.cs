using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookThru.Models;
using Microsoft.Extensions.Configuration;
using Amazon.DynamoDBv2;
using Amazon;
using System.Configuration;
using Amazon.DynamoDBv2.DocumentModel;

namespace BookThru.Controllers
{
    public class ChatDetailsController : Controller
    {
        private IConfiguration Configuration;
        private readonly BookThruContext _context;
        private AmazonDynamoDBClient dynamoDBClient;

        public ChatDetailsController(BookThruContext context, IConfiguration configuration)
        {
            Configuration = configuration;
            _context = context;
            dynamoDBClient = new AmazonDynamoDBClient(Configuration["Amazon:IAMUserKey"], Configuration["Amazon:IAMUserPass"], RegionEndpoint.USEast1);
        }

        // GET: ChatDetails
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: ChatDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatDetails = await _context.ChatDetails
                .FirstOrDefaultAsync(m => m.ChatDetailsId == id);
            if (chatDetails == null)
            {
                return NotFound();
            }

            return View(chatDetails);
        }

        // GET: ChatDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChatDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChatDetailsId,UserEmail,ChatTime,Message,Status")] ChatDetails chatDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chatDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chatDetails);
        }

        // GET: ChatDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatDetails = await _context.ChatDetails.FindAsync(id);
            if (chatDetails == null)
            {
                return NotFound();
            }
            return View(chatDetails);
        }

        // POST: ChatDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChatDetailsId,UserEmail,ChatTime,Message,Status")] ChatDetails chatDetails)
        {
            if (id != chatDetails.ChatDetailsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chatDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChatDetailsExists(chatDetails.ChatDetailsId))
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
            return View(chatDetails);
        }

        // GET: ChatDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chatDetails = await _context.ChatDetails
                .FirstOrDefaultAsync(m => m.ChatDetailsId == id);
            if (chatDetails == null)
            {
                return NotFound();
            }

            return View(chatDetails);
        }

        // POST: ChatDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chatDetails = await _context.ChatDetails.FindAsync(id);
            _context.ChatDetails.Remove(chatDetails);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChatDetailsExists(int id)
        {
            return _context.ChatDetails.Any(e => e.ChatDetailsId == id);
        }
        
        public async Task<IActionResult> SendMessage(string Message)
        {
            try
            {
                Table chatsTable = Table.LoadTable(dynamoDBClient, "bookthru_chats", DynamoDBEntryConversion.V2);
                Document comments = await chatsTable.GetItemAsync(1);
                DynamoDBList list = (DynamoDBList)comments.FirstOrDefault().Value.AsDynamoDBList();

                Document newComment = new Document();
                newComment["MessageID"] = 1;

                Document comment = new Document();
                comment["UserEmail"] = User.Identity.Name;
                comment["Message"] = Message;
                comment["ChatTime"] = DateTime.Now;
                list.Add(comment);

                newComment["Chats"] = list;

                await chatsTable.UpdateItemAsync(newComment);
            }
            catch (Exception e)
            {
                Table chatsTable = Table.LoadTable(dynamoDBClient, "bookthru_chats", DynamoDBEntryConversion.V2);
                Document comments = new Document();
                DynamoDBList list = new DynamoDBList();

                Document newComment = new Document();
                newComment["MessageID"] = 1;

                Document comment = new Document();
                comment["UserEmail"] = User.Identity.Name;
                comment["Message"] = Message;
                comment["ChatTime"] = DateTime.Now;
                list.Add(comment);

                newComment["Chats"] = list;

                await chatsTable.UpdateItemAsync(newComment);
            }

            return LocalRedirect("Index");
        }
    }
}
