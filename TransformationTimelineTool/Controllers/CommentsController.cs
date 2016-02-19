using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TransformationTimelineTool.DAL;
using TransformationTimelineTool.Models;
using TransformationTimelineTool.Helpers;
using System.Web.Configuration;

namespace TransformationTimelineTool.Controllers
{
    public class CommentsController : Controller
    {
        private TimelineContext db = new TimelineContext();

        private async Task<List<Comment>> GetParents()
        {
            return await db.Comments.Where(c => (c.ReplyTo == 0)).ToListAsync();
        }

        private async Task<List<Comment>> GetChildren(int ParentId)
        {
            return await db.Comments.Where(c => (c.ReplyTo == ParentId)).ToListAsync();
        }

        private async Task<Comment> GetCommentAsync(int Id)
        {
            return await db.Comments.Where(c => (c.Id == Id)).SingleAsync();
        }

        private Comment GetComment(int Id)
        {
            return db.Comments.Where(c => (c.Id == Id)).Single();
        }

        private async Task<List<Comment>> GetComments(int order)
        {
            return await db.Comments.Where(c => (c.Order == order)).ToListAsync();
        }

        private async Task<int> GetMaxOrder()
        {
            if (db.Comments.Count() > 0)
                return await db.Comments.MaxAsync(c => c.Order);
            return 0;
        }

        private LinkedList<Comment> SortComments(List<Comment> all)
        {
            HashSet<Comment> HashComments = new HashSet<Comment>(all);
            return SortCommentEx(HashComments, new LinkedList<Comment>(), all.Count());
        }

        private LinkedList<Comment> SortCommentEx(HashSet<Comment> All, LinkedList<Comment> Result, int Count)
        {
            try
            {
                if (Count == Result.Count()) return Result;
                Comment CurrentComment = All.First();
                if (CurrentComment.ReplyTo == 0)
                {
                    Result.AddLast(All.First());
                    All.Remove(CurrentComment);
                }
                else
                {
                    LinkedListNode<Comment> ParentNode = Result.Find(GetComment(CurrentComment.ReplyTo));
                    List<Comment> Children = All.Where(c => c.ReplyTo == CurrentComment.ReplyTo).ToList();
                    Children.Reverse();
                    foreach (Comment child in Children)
                    {
                        Result.AddAfter(ParentNode, child);
                        All.Remove(child);
                    }
                }
                return SortCommentEx(All, Result, Count);
            } catch (Exception ex)
            {
                Utils.log(ex.ToString());
            }
            return null;
        }
        
        private static void PrintCommentList(ICollection<Comment> list)
        {
            foreach (var c in list)
            {
                Utils.log("(" + c.Id + ") " + c.AuthorName + ": " + c.Content + " @ " + c.Date);
            }
        }

        // POST: Comments
        [HttpPost]
        public async Task<ActionResult> GetComments()
        {
            var Comments = await db.Comments.ToListAsync();
            return Json(
            SortComments(Comments).Select(e => new
            {
                Id = e.Id,
                Content = HttpUtility.HtmlEncode(e.Content),
                Author = e.AuthorName,
                Date = new TimeSpan(e.Date.ToUniversalTime().Ticks - new DateTime(1970, 1, 1).Ticks).TotalMilliseconds,
                ReplyTo = e.ReplyTo,
                Order = e.Order,
                Depth = e.Depth
            }).OrderBy(c => c.Order));
        }

        [HttpPost]
        public async Task<ActionResult> AddComment(string comment, int replyto = 0)
        {
            var currentUser = Utils.GetFullName();
            Comment Comment = new Comment();
            Comment.AuthorName = currentUser;
            Comment.Date = DateTime.Now;
            Comment.Content = comment;
            Comment.ReplyTo = replyto;
            if (replyto > 0)
            {
                var Parent = await GetCommentAsync(replyto);
                Comment.Order = Parent.Order;
                Comment.Depth = Parent.Depth + 1;
            } else
            {
                Comment.Order = await GetMaxOrder() + 1;
            }
            db.Comments.Add(Comment);
            await db.SaveChangesAsync();
            NotifyAdministrator(Comment);
            return Json(Comment);
        }

        private async void NotifyAdministrator(Comment comment)
        {
            string SendTo = WebConfigurationManager.AppSettings["adminEmail"];
            string ServerDomain = WebConfigurationManager.AppSettings["serverURL"];
            string MailSubject = "A new comment has been added";
            string CommentContent = HttpUtility.HtmlEncode(comment.Content);
            string MailBody = "Author: " + comment.AuthorName + "<br />";
            MailBody += "Comment: " + CommentContent + "<br />";
            MailBody += "Added on: " + comment.Date.ToString("yyyy-MM-dd HH:mm") + "<br />";
            MailBody += "Click <a href=" + ServerDomain + ">here</a> to go to the Timeline Tool";
            Utils.SendMailAsync(SendTo, MailSubject, MailBody);
        }
        
        [HttpPost]
        public async Task<ActionResult> DeleteComment(string id)
        {
            int CommentId = Int32.Parse(id);
            Comment comment = await db.Comments.FindAsync(CommentId);
            comment.Content = "Deleted";
            await db.SaveChangesAsync();
            return Json(comment);
        }

        // GET: Comments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = await db.Comments.FindAsync(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Content,Date")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
