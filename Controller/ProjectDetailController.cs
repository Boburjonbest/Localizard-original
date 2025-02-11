﻿using AutoMapper;
using Localizard.DAL;
using Localizard.DAL.Repositories;
using Localizard.DAL.Repositories.Implementations;
using Localizard.Data.Entites;
using Localizard.Domain.Entites;
using Localizard.Domain.Enums;
using Localizard.Domain.ViewModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Localizard.Controller;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProjectDetailController : ControllerBase
{
    private readonly IProjectDetailRepo _projectDetail;
    private readonly IMapper _mapper;
    private readonly IProjectDetailRepo _projectDetailRepo;
    private readonly ITranslationRepo _translationRepo;
    private readonly ITagRepo _tag;
    public ProjectDetailController(IMapper mapper, IProjectDetailRepo projectDetail, 
        IProjectDetailRepo projectDetailRepo, ITranslationRepo translationRepo, ITagRepo tag)
    {
        _mapper = mapper;
        _projectDetail = projectDetail;
        _projectDetailRepo = projectDetailRepo;
        _translationRepo = translationRepo;
        _tag = tag;
    }

    [HttpGet]
    public IActionResult GetAllProjectDetails(string? Search = null)
    {
        var projectDetails = _projectDetailRepo.GetAll();
        if (!string.IsNullOrEmpty(Search))
        {
            foreach (var detail in projectDetails)
            {
                Console.WriteLine($"Key: {detail.Key}, TagIds Type: {detail.TagIds?.GetType()}");
            }
            var projectDetailViews = projectDetails.Select(detial => GetDetailMapper(detial)).ToList();
            projectDetailViews = projectDetailViews
                .Where(pd => 
                    pd.Key != null && pd.Key.IndexOf(Search, StringComparison.OrdinalIgnoreCase) >= 0 || pd.Tags != null && pd.Tags.Any(
                        tag => tag.ToString().IndexOf(Search,StringComparison.OrdinalIgnoreCase) >= 0 )).ToList();
        }
        
        
        return Ok(projectDetails);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectDetailById(int id)
    {
        if (!_projectDetail.ProjectDetailExist(id))
            return NotFound();

        var projectDetail = await _projectDetail.GetById(id);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);  

        return Ok(projectDetail);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateProjectDetail([FromBody] CreateProjectDetailView detail)
    {
        if (detail == null)
            return BadRequest(ModelState);

        var checkDetail = _projectDetailRepo.GetAll().Select(d => d.Key).Contains(detail.Key);
        var validTagIds = Enum.GetValues(typeof(TagEnum)).Cast<int>();
        var invalidTags = detail.TagIds.Except(validTagIds).ToList();
        
        if (invalidTags.Any())
            return BadRequest($"Invalid tag IDs: {string.Join(", ", invalidTags)}");
        
        foreach (var translate in detail.Translations)
        {
            var existingTranslation = _translationRepo.GetAll()
                .FirstOrDefault(t => t.SymbolKey == translate.SymbolKey);

            if (existingTranslation != null)
            {
                ModelState.AddModelError("", $"Translation already exists for language {translate.LanguageId}.");
                return StatusCode(422, ModelState);
            }
        }
        
        
        var projectDetail = CraeteDetailMapper(detail);
        
        if (checkDetail)
        {
            ModelState.AddModelError("","Project Detail already exists!");
            return StatusCode(500, ModelState);
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_projectDetailRepo.CreateProjectDetail(projectDetail))
        {
            ModelState.AddModelError("","Something went wrong while saving");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully created;-)");
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateProjectDetail(int id, [FromBody] UpdateProjectDetailView update)
    {
        if (update == null)
            return BadRequest(ModelState);

        var existingDetail = await _projectDetailRepo.GetById(id);

        if (existingDetail == null)
            return NotFound("there is no such detial");

        var projectDetialExists = _projectDetailRepo.GetAll().Any(x => x.Key == update.Key && x.Id != id);

        if (projectDetialExists)
        {
            ModelState.AddModelError("","Project Detail already exist, check it again!");
            return StatusCode(422, ModelState);
        }

        existingDetail.Key = update.Key;

        var translations = _translationRepo.GetAll();
        existingDetail.Translation.Clear();

        foreach (var translate in translations)
        {
            if (update.TranslationIds.Contains(translate.Id)) 
                existingDetail.Translation.Add(translate);
        }
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        
        if (!_projectDetailRepo.UpdateProjectDetail(existingDetail))
        {
            ModelState.AddModelError("", "Something went wrong while saving the project Detail.");
            return StatusCode(500, ModelState);
        }

        return Ok("Successfully updated the project.");
    }
    
    [HttpDelete]
    public IActionResult DeleteProjectDetail(int id)
    {
        if (!_projectDetailRepo.DeleteProjectDetail(id))
        {
            return NotFound(new { message = "Project Detail not Found!" });
        }

        return Ok(new { message = "Project Detail removed!" });
    }
    
    #region GetDetailMapper
    private GetProjectDetailView GetDetailMapper(ProjectDetail detail)
    {
        GetProjectDetailView detailView = new GetProjectDetailView()
        {
            ProjectInfoId = detail.ProjectInfoId,
            Key = detail.Key,
            AvailableTranslations = detail.Translation,
            Tags = detail.TagIds
                .Where(t => Enum.IsDefined(typeof(TagEnum), t))
                .Select(t => new GetTagView()
                {
                    Id = t,
                    Name = Enum.GetName(typeof(TagEnum), t)
                })
                .ToList()
        };
        
        return detailView;
    }
    #endregion
    #region CreateDetailMapper
    private ProjectDetail CraeteDetailMapper(CreateProjectDetailView create)
    {
        ProjectDetail detailView = new ProjectDetail()
        {
            Key = create.Key,
            ProjectInfoId = create.ProjectInfoId,
            Translation = new List<Translation>(),
            TagIds = create.TagIds,
        };
        if (create.Translations != null)
        {
            foreach (var translate in create.Translations)
            {
                var translation = new Translation()
                {
                    SymbolKey = translate.SymbolKey,
                    LanguageId = translate.LanguageId,
                    Text = translate.Text
                };
                
                detailView.Translation.Add(translation);
            }
        }

        return detailView;
    }
    #endregion

}