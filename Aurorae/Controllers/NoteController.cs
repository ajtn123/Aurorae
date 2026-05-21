using Aurorae.Models.DbModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aurorae.Controllers;

public class NoteController(AuroraeDb db) : Controller
{
    [HttpGet("/notes")]
    public async Task<IActionResult> Index([FromQuery] string? filter = null)
    {
        var query = db.Notes
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(n => EF.Functions.Like(n.Title, $"%{filter}%") || EF.Functions.Like(n.Content, $"%{filter}%"));

        var notes = await query
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();
        return View("Index", notes);
    }

    [HttpGet("/notes/new")]
    public IActionResult New()
    {
        return View("Edit", new Note());
    }

    [HttpPost("/notes")]
    public async Task<IActionResult> Create([FromForm] Note note)
    {
        if (!ModelState.IsValid)
            return View("Edit", note);

        note.CreatedAt = DateTimeOffset.UtcNow;
        note.UpdatedAt = DateTimeOffset.UtcNow;

        db.Notes.Add(note);
        await db.SaveChangesAsync();

        return Redirect($"/notes/{note.Id}");
    }

    [HttpGet("/notes/{id}")]
    public async Task<IActionResult> Detail(long id)
    {
        if (await db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id) is not { } note)
            return NotFound();

        return View("Detail", note);
    }

    [HttpGet("/notes/{id}/edit")]
    public async Task<IActionResult> Edit(long id)
    {
        if (await db.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id) is not { } note)
            return NotFound();

        return View("Edit", note);
    }

    [HttpPost("/notes/{id}")]
    public async Task<IActionResult> Update(long id, [FromForm] Note note)
    {
        if (!ModelState.IsValid)
            return View("Edit", note);

        if (await db.Notes.FirstOrDefaultAsync(n => n.Id == id) is not { } existing)
            return NotFound();

        existing.Title = note.Title;
        existing.Content = note.Content;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync();

        return Redirect($"/notes/{id}");
    }

    [HttpPost("/notes/{id}/delete")]
    public async Task<IActionResult> Delete(long id)
    {
        if (await db.Notes.FirstOrDefaultAsync(n => n.Id == id) is not { } note)
            return NotFound();

        db.Notes.Remove(note);
        await db.SaveChangesAsync();

        return Redirect("/notes");
    }
}
