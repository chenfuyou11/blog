using AutoMapper;
using BlogDemo.Api.Helpers;
using BlogDemo.Core.Entities;
using BlogDemo.Core.Interfaces;
using BlogDemo.Infrastructure.DateBase;
using BlogDemo.Infrastructure.Extensions;
using BlogDemo.Infrastructure.Resources;
using BlogDemo.Infrastructure.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using IdentityModel;

namespace BlogDemo.Api.Controllers
{
    
    [Route("api/posts")]
    public class PostController:Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly ITypeHelperService _typeHelperService;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public PostController(IPostRepository postRepository, IUnitOfWork unitOfWork,IMapper mapper, IUrlHelper urlHelper, ITypeHelperService typeHelperService, IPropertyMappingContainer propertyMappingContainer)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _typeHelperService = typeHelperService;
            _propertyMappingContainer = propertyMappingContainer;
        }
        [AllowAnonymous]
        [HttpGet(Name = "GetPosts")]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.cgzl.hateoas+json" })]
        public async Task<IActionResult> GetHateoas(PostParameters parameters)
        {
            //验证排序属性映射是否存在
            if (!_propertyMappingContainer.ValidateMappingExistsFor<PostDTO, Post>(parameters.OrderBy))
            {
                return BadRequest("Can't finds fields for sorting.");
            }

            //验证Filed是否存在
            if (!_typeHelperService.TypeHasProperties<PostDTO>(parameters.Fields))
            {
                return BadRequest("Filed not exits");
            }
             

            var posts = await _postRepository.GetPostsAsync(parameters);
            var postDto=_mapper.Map<IEnumerable<Post>,IEnumerable<PostDTO>>(posts);

            var shapePostDTO = postDto.ToDynamicIEnumerable(parameters.Fields);
            var previousPageLink = posts.HasPrevious ?
             CreatePostUri(parameters, PaginationResourceUriType.PreviousPage) : null;

            var nextPageLink = posts.HasNext ?
                CreatePostUri(parameters, PaginationResourceUriType.NextPage) : null;


            var shapedWithLinks = shapePostDTO.Select(x =>
            {
                var dict = x as IDictionary<string, object>;
                var postLinks = CreateLinksForPost((int)dict["Id"], parameters.Fields);
                dict.Add("links", postLinks);
                return dict;
            });
            var links = CreateLinksForPosts(parameters, posts.HasPrevious, posts.HasNext);
            var result = new
            {
                value = shapedWithLinks,
                links
            };
             

            var meta = new
            {
                PageSize = posts.PageSize,
                PageIndex = posts.PageIndex,
                TotalItemCount = posts.TotalItemsCount,
                PageCount = posts.PageCount,
                previousPageLink,
                nextPageLink
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetPosts")]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/json" })]
        public async Task<IActionResult> Get(PostParameters postParameters)
        {
            if (!_propertyMappingContainer.ValidateMappingExistsFor<PostDTO, Post>(postParameters.OrderBy))
            {
                return BadRequest("Can't finds fields for sorting.");
            }

            if (!_typeHelperService.TypeHasProperties<PostDTO>(postParameters.Fields))
            {
                return BadRequest("Fields not exist.");
            }

            var postList = await _postRepository.GetPostsAsync(postParameters);

            var postResources = _mapper.Map<IEnumerable<Post>, IEnumerable<PostDTO>>(postList);

            var previousPageLink = postList.HasPrevious ?
                CreatePostUri(postParameters,
                    PaginationResourceUriType.PreviousPage) : null;

            var nextPageLink = postList.HasNext ?
                CreatePostUri(postParameters,
                    PaginationResourceUriType.NextPage) : null;

            var meta = new
            {
                postList.TotalItemsCount,
                postList.PageSize,
                postList.PageIndex,
                postList.PageCount,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));

            return Ok(postResources.ToDynamicIEnumerable(postParameters.Fields));
        }
         



        [HttpGet("{Id}", Name = "GetPost")]
        public async Task<IActionResult> Get(int Id,string fields=null)
        {
            //验证Filed是否存在
            if (!_typeHelperService.TypeHasProperties<PostDTO>(fields))
            {
                return BadRequest("Filed not exits");
            }
            var post = await _postRepository.GetPostId(Id);
            if(post==null)
            {
                return NotFound();
            }
            var postDTO = _mapper.Map<Post, PostDTO>(post);
            var shapePostDTO = postDTO.ToDynamic(fields);
            var links = CreateLinksForPost(Id, fields);

            var result = (IDictionary<string, object>)shapePostDTO;

            result.Add("links", links);
            return Ok(result);

        }



       

        [HttpPost(Name = "CreatePost")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.cgzl.post.create+json" })]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.cgzl.hateoas+json" })]
        public async Task<IActionResult> Post([FromBody] PostAddResource postAddResource)
        {
            if (postAddResource == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return new  MyUnprocessableEntityObjectResult(ModelState);
            }
         

            var newPost = _mapper.Map<PostAddResource, Post>(postAddResource);
            var userName = User.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.PreferredUserName)?.Value;

            newPost.Author = userName;
           // newPost.Author = "admin";
            newPost.LastModified = DateTime.Now;

            _postRepository.AddPost(newPost);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception("Save Failed!");
            }

            var resultResource = _mapper.Map<Post, PostDTO>(newPost);

            var links = CreateLinksForPost(newPost.Id);
            var linkedPostResource = resultResource.ToDynamic() as IDictionary<string, object>;
            linkedPostResource.Add("links", links);

            return CreatedAtRoute("GetPost", new { id = linkedPostResource["Id"] }, linkedPostResource);
        }

        [AllowAnonymous]
        [HttpDelete("{id}", Name = "DeletePost")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postRepository.GetPostId(id);
            if (post == null)
            {
                return NotFound();
            }

            _postRepository.Delete(post);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Deleting post {id} failed when saving.");
            }

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdatePost")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.cgzl.post.update+json" })]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] PostUpdateResource postUpdate)
        {
            if (postUpdate == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new MyUnprocessableEntityObjectResult(ModelState);
            }

            var post = await _postRepository.GetPostId(id);
            if (post == null)
            {
                return NotFound();
            }

            post.LastModified = DateTime.Now;
            _mapper.Map(postUpdate, post);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Updating post {id} failed when saving.");
            }
            return NoContent();
        }


        [HttpPatch("{id}", Name = "PartiallyUpdatePost")]
        public async Task<IActionResult> PartiallyUpdateCityForCountry(int id,
    [FromBody] JsonPatchDocument<PostUpdateResource> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var post = await _postRepository.GetPostId(id);
            if (post == null)
            {
                return NotFound();
            }

            var postToPatch = _mapper.Map<PostUpdateResource>(post);

            patchDoc.ApplyTo(postToPatch, ModelState);

            TryValidateModel(postToPatch);

            if (!ModelState.IsValid)
            {
                return new MyUnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(postToPatch, post);
            post.LastModified = DateTime.Now;
            _postRepository.Update(post);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Patching city {id} failed when saving.");
            }

            return NoContent();
        }









        private string CreatePostUri(PostParameters parameters, PaginationResourceUriType uriType)
        {
            switch (uriType)
            {
                case PaginationResourceUriType.PreviousPage:
                    var previousParameters = new
                    {
                        pageIndex = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", previousParameters);
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", nextParameters);
                default:
                    var currentParameters = new
                    {
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields
                    };
                    return _urlHelper.Link("GetPosts", currentParameters);
            }
        }



        private IEnumerable<LinkResource> CreateLinksForPost(int id, string fields = null)
        {
            var links = new List<LinkResource>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkResource(
                        _urlHelper.Link("GetPost", new { id }), "self", "GET"));
            }
            else
            {
                links.Add(
                    new LinkResource(
                        _urlHelper.Link("GetPost", new { id, fields }), "self", "GET"));
            }

            links.Add(
                 new LinkResource(
                     _urlHelper.Link("DeletePost", new { id }), "delete_post", "DELETE"));

            return links;
        }

        private IEnumerable<LinkResource> CreateLinksForPosts(PostParameters postResourceParameters,
            bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkResource>
            {
                new LinkResource(
                    CreatePostUri(postResourceParameters, PaginationResourceUriType.CurrentPage),
                    "self", "GET")
            };

            if (hasPrevious)
            {
                links.Add(
                    new LinkResource(
                        CreatePostUri(postResourceParameters, PaginationResourceUriType.PreviousPage),
                        "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(
                    new LinkResource(
                        CreatePostUri(postResourceParameters, PaginationResourceUriType.NextPage),
                        "next_page", "GET"));
            }

            return links;
        }




    }
}
