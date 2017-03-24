using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using UsingWebAPIWithEF6.Models;
using DTO;
using AutoMapper;
using Newtonsoft.Json.Linq;

namespace UsingWebAPIWithEF6.Controllers
{
    public class BooksController : ApiController
    {
        private UsingWebAPIWithEF6Context db = new UsingWebAPIWithEF6Context();

        // GET: api/Books
        /// <summary>
        /// 获取所有书籍
        /// </summary>
        /// <returns></returns>
       [HttpGet]
        public async Task<IHttpActionResult> GetBooks( )
        {
            var book=new List<Book>();
            var bookMap = new List<BookDto>();
            try
            {
                book = await db.Books.Include(b => b.Author).ToListAsync();
                Mapper.Initialize(i => i.CreateMap<Book, BookDto>());
                bookMap = Mapper.Map<List<Book>, List<BookDto>>(book);
                if (bookMap == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {

                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(ex.Message),
                };
            }
            return Ok(bookMap);
        }
        /// <summary>
        /// 获取指定书籍
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Books/5
        [ResponseType(typeof(BookDetailDto))]
        public async Task<IHttpActionResult> GetBook(int id)
        {
            //var book = await db.Books.Include(b => b.Author).Select(b => new BookDetailDto()
            //{
            //    Id = b.Id,
            //    Title = b.Title,
            //    Year = b.Year,
            //    Price = b.Price,
            //    AuthorName = b.Author.Name,
            //    Genre = b.Genre
            //}).SingleOrDefaultAsync(b => b.Id == id);
            var book =await db.Books.Include(b => b.Author).SingleOrDefaultAsync(b=> b.Id==id);
            Mapper.Initialize(i => i.CreateMap<Book, BookDto>());
            Mapper.Map<BookDto>(book);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }
        /// <summary>
        /// 修改指定书籍
        /// </summary>
        /// <param name="id"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        // PUT: api/Books/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutBook(int id, Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != book.Id)
            {
                return BadRequest();
            }

            db.Entry(book).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Books
        /// <summary>
        /// 增加书籍
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> PostBook([FromBody]Book book)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Books.Add(book);
            await db.SaveChangesAsync();
            db.Entry(book).Reference(x => x.Author).Load();
            var dto = new BookDto()
            {
                Id = book.Id,
                Title=book.Title,
                AuthorName = book.Author.Name
            };
            return CreatedAtRoute("DefaultApi", new { id = book.Id }, dto);
        }

        // DELETE: api/Books/5
        /// <summary>
        /// 删除指定书籍
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(Book))]
        public async Task<IHttpActionResult> DeleteBook(int id)
        {
            Book book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            db.Books.Remove(book);
            await db.SaveChangesAsync();

            return Ok(book);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool BookExists(int id)
        {
            return db.Books.Count(e => e.Id == id) > 0;
        }
    }
}