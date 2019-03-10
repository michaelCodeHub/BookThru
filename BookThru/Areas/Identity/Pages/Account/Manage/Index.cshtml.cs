using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using BookThru.Data;
using BookThru.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookThru.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private const string bucketName = "wehire";
        private const string IAMUserKey = "AKIAI27NEVOTDANXC7YQ";
        private const string IAMUserPass = "5eOsHBc7Y1CLuEB6+YMLuuNB/Daf+KHGXOT3PMkI";

        private readonly UserManager<BookThruUser> _userManager;
        private readonly SignInManager<BookThruUser> _signInManager;
        private readonly IEmailSender _emailSender;

        private readonly BookThruContext _context;

        public IndexModel(
            UserManager<BookThruUser> userManager,
            SignInManager<BookThruUser> signInManager,
            IEmailSender emailSender,
            BookThruContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _context = context;
        }

        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }


            public UserInfo UserInfo { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userName = await _userManager.GetUserNameAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var userInfo = _context.UserInfo.Where(x => x.Id == user.Id).FirstOrDefault();
            if (userInfo != null)
            {
                Username = userName;
                userInfo.PictureUrl = "https://s3.amazonaws.com/wehire/" + userInfo.PictureUrl;

                Input = new InputModel
                {
                    Email = email,
                    PhoneNumber = phoneNumber,
                    UserInfo = userInfo

                };

            }
            else
            {
                Username = userName;

                Input = new InputModel
                {
                    Email = email,
                    PhoneNumber = phoneNumber,
                    UserInfo = userInfo
                };
            }

            IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            var userInfo = _context.UserInfo.Where(x => x.Id == user.Id).FirstOrDefault();
            if (userInfo != null)
            {
                //if (this.Request.Form.Files.Count < 2)
                //    throw new Exception("Please include a image file to upload.");
                IFormFile imageFile = null;
                try
                {
                    var file = this.Request.Form.Files[0];
                    if (file.FileName.Contains(".jpeg") || file.FileName.Contains(".jpg") || file.FileName.Contains(".png"))
                    {
                        imageFile = this.Request.Form.Files[0];
                    }
                }
                catch (Exception)
                {

                }
                Random rnd = new Random();

                if (Input.UserInfo.PhoneNumber != userInfo.PhoneNumber ||
                    Input.UserInfo.FirstName != userInfo.FirstName ||
                    Input.UserInfo.LastName != userInfo.LastName)
                {

                    userInfo.FirstName = Input.UserInfo.FirstName;
                    userInfo.LastName = Input.UserInfo.LastName;
                    userInfo.PhoneNumber = Input.UserInfo.PhoneNumber;

                    if (imageFile != null)
                    {
                        userInfo.PictureUrl = rnd.Next(99999999).ToString() + "." + imageFile.FileName;
                        await UploadFileAsync(imageFile.OpenReadStream(), Input.UserInfo.PictureUrl);
                    }

                    _context.UserInfo.Update(userInfo);

                    await _context.SaveChangesAsync();


                }
            }
            else
            {
                //if (this.Request.Form.Files.Count < 2)
                //	throw new Exception("Please include a image file to upload.");

                IFormFile imageFile = null;
                IFormFile resumeFile = null;
                try
                {
                    imageFile = this.Request.Form.Files[0];
                    resumeFile = this.Request.Form.Files[1];
                }
                catch (Exception)
                {

                }
                //Input.JobSeeker.PictureUrl = "";
                //Input.JobSeeker.ResumeUrl = "";
                Random rnd = new Random();

                if (imageFile != null)
                {
                    Input.UserInfo.PictureUrl = rnd.Next(99999999).ToString() + "." + imageFile.FileName;
                    await UploadFileAsync(imageFile.OpenReadStream(), Input.UserInfo.PictureUrl);
                }

                UserInfo jobSeekerNew = new UserInfo
                {
                    Id = user.Id,
                    FirstName = Input.UserInfo.FirstName,
                    LastName = Input.UserInfo.LastName,
                    PhoneNumber = Input.UserInfo.PhoneNumber,
                    PictureUrl = Input.UserInfo.PictureUrl,

                };
                var jobSeekerrId = await _context.UserInfo.AddAsync(jobSeekerNew);
                _context.SaveChangesAsync();
                if (jobSeekerrId == null)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred setting Recruiter with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendVerificationEmailAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var email = await _userManager.GetEmailAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToPage();
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
    }
}
