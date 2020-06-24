using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portal.PostService.Data;
using Portal.PostService.Models;

namespace Portal.PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _db;

        public PostController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Post>> Create([FromBody] Post post)
        {
            //post.Id = Guid.NewGuid();
            post.TimeCreated = DateTime.Now;

            _db.Add(post);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = post.Id }, post);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Post>> Update([FromBody] Post post)
        {
            var model = await _db.Posts.FindAsync(post.Id);

            model.Name = post.Name;
            model.Description = post.Description;
            model.CityId = post.CityId;
            model.CategoryId = post.CategoryId;

            _db.Update(model);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = post.Id }, post);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> Get()
        {
            var post = await _db.Posts.ToListAsync();
            return Ok(post);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> GetByUser(string userId)
        {
            var post = await _db.Posts.Where(p => p.UserId == userId).ToListAsync();
            return Ok(post);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Post>> Get(string id)
        {
            var post = await _db.Posts.FindAsync(Guid.Parse(id));
            return Ok(post);
        }
    }
}
